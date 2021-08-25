#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Playnite.SDK;
using Playnite.SDK.Metadata;
using Playnite.SDK.Plugins;
using ScreenScraperMetadata.Models;
using ScreenScraperMetadata.Services;

namespace ScreenScraperMetadata
{
    public class ScreenScraperMetadataProvider : OnDemandMetadataProvider
    {
        private readonly MetadataRequestOptions options;
        private readonly ScreenScraperMetadata plugin;
        private readonly List<string> searchRegions;
        private readonly ScreenScraperServiceClient service;
        private readonly ScreenScraperMetadataSettings settings;
        private readonly Jeu? ssGameInfo;

        public ScreenScraperMetadataProvider(MetadataRequestOptions options, ScreenScraperMetadata plugin,
            ScreenScraperMetadataSettings settings)
        {
            this.options = options;
            this.plugin = plugin;
            this.settings = settings;
            service = new ScreenScraperServiceClient(settings);
            ssGameInfo = service.GetJeuInfo(options.GameData)?.response.jeu;
            AvailableFields = GetAvailableFields();
            searchRegions = GetRegionsToCheck();
        }

        public override List<MetadataField> AvailableFields { get; }

        public override MetadataFile? GetCoverImage()
        {
            var media = FindMediaItems("box-2D").FirstOrDefault();
            return DownloadToMetadataFile(media);
        }

        public override MetadataFile? GetBackgroundImage()
        {
            var fanarts = FindMediaItems("fanart");
            var screenshots = FindMediaItems("ss").Concat(FindMediaItems("sstitle"));
            var medias = new List<Media>();
            switch (settings.BackgroundPreference)
            {
                case ScreenScraperMetadataSettings.BackgroundPreferenceEnum.Fanart:
                    medias.AddRange(fanarts);
                    break;
                case ScreenScraperMetadataSettings.BackgroundPreferenceEnum.Screenshot:
                    medias.AddRange(screenshots);
                    break;
                case ScreenScraperMetadataSettings.BackgroundPreferenceEnum.PreferFanart:
                    medias.AddRange(fanarts.Concat(screenshots));
                    break;
                case ScreenScraperMetadataSettings.BackgroundPreferenceEnum.PreferScreenshot:
                    medias.AddRange(screenshots.Concat(fanarts));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Media? media;
            if (options.IsBackgroundDownload || medias.Count <= 1)
            {
                media = medias.FirstOrDefault();
            }
            else
            {
                var imageFileOptions =  medias.Select(m => new ImageFileOption(m.url)).ToList();
                var url = plugin.PlayniteApi.Dialogs.ChooseImageFile(imageFileOptions, "Choose background image")?.Path;
                media = medias.Find(m => m.url.Equals(url));
            }

            return DownloadToMetadataFile(media);
        }

        public override string? GetDescription()
        {
            var synopsis = ssGameInfo?.synopsis?.Find(
                item => item.langue == "en"
            );
            var text = synopsis?.text;
            return HttpUtility.HtmlDecode(text);
        }

        public override List<string>? GetDevelopers()
        {
            var developer = ssGameInfo?.developpeur?.text;
            if (developer != null)
                return new List<string>
                {
                    developer
                };
            return base.GetDevelopers();
        }

        public override List<string?>? GetGenres()
        {
            return ssGameInfo?.genres?.Select(
                    item => item.noms.Find(nom => nom.langue == "en")?.text
                )
                .Where(genre => !string.IsNullOrEmpty(genre))
                .ToList();
        }

        public override MetadataFile? GetIcon()
        {
            if (!settings.ShouldUseGameLogosAsIcon)
            {
                return null;
            }

            var media = FindMediaItems("wheel").FirstOrDefault() ?? FindMediaItems("wheel-hd").FirstOrDefault();
            return DownloadToMetadataFile(media);
        }

        public override string? GetName()
        {
            var found = searchRegions.Select(region =>
                    ssGameInfo?.noms.Find(nom => region.Equals(nom.region))
                ).First()
                ?.text;
            return found;
        }

        public override string? GetPlatform()
        {
            var platformId = ssGameInfo?.systeme.id;
            return platformId != null ? service.GetPlatformNameById(platformId) : base.GetPlatform();
        }

        public override List<string>? GetPublishers()
        {
            var publisher = ssGameInfo?.editeur?.text;
            return publisher != null ? new List<string> { publisher } : base.GetDevelopers();
        }

        public override DateTime? GetReleaseDate()
        {
            var region = GetGameRegion();
            var dateResult = ssGameInfo?.dates?.Find(dateInfo =>
                dateInfo.region == region
            ) ?? ssGameInfo?.dates?.FirstOrDefault();

            if (dateResult == null) return base.GetReleaseDate();

            try
            {
                return DateTime.ParseExact(
                    dateResult.text,
                    dateResult.text?.Length == 4 ? "yyyy" : "yyyy-MM-dd",
                    null
                );
            }
            catch (Exception)
            {
                return base.GetReleaseDate();
            }
        }

        public override string? GetAgeRating()
        {
            var ageRatingPreference = plugin.PlayniteApi.ApplicationSettings.AgeRatingOrgPriority;
            var dateResult = ssGameInfo?.classifications?.Find(rating =>
                rating.type == ageRatingPreference.ToString()
            ) ?? ssGameInfo?.classifications?.FirstOrDefault();
            if (dateResult != null) return dateResult.type + " " + dateResult.text;
            return base.GetAgeRating();
        }

        private List<string> GetRegionsToCheck()
        {
            var gameRegion = GetGameRegion();
            var gameRegionPrependStr = gameRegion != null ? gameRegion + "," : "";
            var regionsStr = gameRegionPrependStr + settings.RegionPreferences;
            var preferredRegions = new string(
                    regionsStr.ToCharArray()
                        .Where(c => !Char.IsWhiteSpace(c))
                        .ToArray())
                .Split(',')
                .Distinct()
                .ToList();

            if (!preferredRegions.HasItems())
            {
                //Use some defaults
                preferredRegions.Add("ss");
                preferredRegions.Add("us");
                preferredRegions.Add("wor");
                preferredRegions.Add("jp");
            }

            return preferredRegions;
        }

        private List<MetadataField> GetAvailableFields()
        {
            var metadataFields = new List<MetadataField>();
            if (ssGameInfo == null) return metadataFields;

            metadataFields.Add(MetadataField.Name);
            if (ssGameInfo.genres?.HasItems() == true)
            {
                metadataFields.Add(MetadataField.Genres);
            }

            if (ssGameInfo.dates?.HasItems() == true)
            {
                metadataFields.Add(MetadataField.ReleaseDate);
            }

            if (ssGameInfo.developpeur != null)
            {
                metadataFields.Add(MetadataField.Developers);
            }

            if (ssGameInfo.editeur != null)
            {
                metadataFields.Add(MetadataField.Publishers);
            }

            if (ssGameInfo.synopsis?.HasItems() == true)
            {
                metadataFields.Add(MetadataField.Description);
            }

            if (ssGameInfo.medias.HasItems())
            {
                metadataFields.Add(MetadataField.CoverImage);
                metadataFields.Add(MetadataField.BackgroundImage);
                if (settings.ShouldUseGameLogosAsIcon)
                {
                    metadataFields.Add(MetadataField.Icon);
                }
            }

            if (ssGameInfo.classifications?.HasItems() == true)
            {
                metadataFields.Add(MetadataField.AgeRating);
            }

            metadataFields.Add(MetadataField.Platform);

            return metadataFields;
        }

        private string? GetGameRegion()
        {
            if (!HasRomFile()) return null;

            const string pattern = @"\((.+?)\)";
            var filename = GetRomFileName();
            foreach (Match m in Regex.Matches(filename, pattern))
            {
                var regionCandidate = m.Groups[0].Value;
                regionCandidate = regionCandidate.Replace(")", "");
                regionCandidate = regionCandidate.Replace("(", "");

                switch (regionCandidate)
                {
                    case "A":
                        return "au"; //Australia
                    case "B":
                        return "br"; //Brazil
                    case "C":
                        return "cn"; //China
                    case "E":
                    case "EU":
                        return "eu"; //Europe
                    case "FN":
                        return "fi"; //Finland
                    case "F":
                        return "fr"; //France
                    case "GR":
                        return "gr"; //Greece
                    case "I":
                        return "it"; //Italy
                    case "J":
                    case "1":
                        return "jp"; //Japan
                    case "K":
                        return "kr"; //Korea
                    case "NL":
                        return "nl"; //Netherlands
                    case "S":
                        return "sp"; //Spain
                    case "SW":
                        return "se"; //Sweden
                    case "U":
                    case "4":
                    case "USA":
                    case "US":
                        return "us"; //USA
                    case "UK":
                        return "uk"; //UK
                }
            }

            return null;
        }

        private IEnumerable<Media> FindMediaItems(string type)
        {
            return searchRegions.Select(region =>
                    ssGameInfo?.medias.Find(item =>
                        type.Equals(item.type)
                        && (region.Equals(item.region) || item.region == null)
                    ))
                .Where(item => item != null)
                .Select(item => item!)
                .Distinct();
        }

        private bool HasRomFile()
        {
            return File.Exists(options.GameData.GameImagePath);
        }

        private string GetRomFileName()
        {
            return Path.GetFileName(options.GameData.GameImagePath);
        }

        private MetadataFile? DownloadToMetadataFile(Media? media)
        {
            if (media == null)
            {
                return null;
            }
            var bytes = service.DownloadFile(media.url);
            var name = media.url.GetHashCode() + "." + media.format;
            return new MetadataFile(name, bytes, media.url);
        }
    }
}