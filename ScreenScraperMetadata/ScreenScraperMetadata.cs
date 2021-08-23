using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Playnite.SDK;
using Playnite.SDK.Plugins;

namespace ScreenScraperMetadata
{
    public class ScreenScraperMetadata : MetadataPlugin
    {
        private UserControl? cachedView;

        public ScreenScraperMetadata(IPlayniteAPI api) : base(api)
        {
            Settings = new ScreenScraperMetadataSettings(this);
        }

        private ScreenScraperMetadataSettings Settings { get; }

        public override Guid Id { get; } = Guid.Parse("7f06b81a-3271-44ee-a234-b32fc15c42f1");

        public override List<MetadataField> SupportedFields { get; } = new()
        {
            MetadataField.CoverImage,
            MetadataField.BackgroundImage,
            MetadataField.Description,
            MetadataField.Developers,
            MetadataField.Genres,
            MetadataField.Icon,
            MetadataField.Name,
            MetadataField.Platform,
            MetadataField.Publishers,
            MetadataField.ReleaseDate,
            MetadataField.AgeRating,
            MetadataField.CriticScore
        };

        // Change to something more appropriate
        public override string Name => "ScreenScraper.fr";

        public override OnDemandMetadataProvider GetMetadataProvider(MetadataRequestOptions options)
        {
            return new ScreenScraperMetadataProvider(options, this, Settings);
        }

        public override ISettings GetSettings(bool firstRunSettings)
        {
            return Settings;
        }

        public override UserControl GetSettingsView(bool firstRunSettings)
        {
            return cachedView ??= new ScreenScraperMetadataSettingsView();
        }
    }
}