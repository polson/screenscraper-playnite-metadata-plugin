using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Playnite.SDK;
using Playnite.SDK.Models;
using RestSharp;
using ScreenScraperMetadata.Models;

namespace ScreenScraperMetadata.Services
{
    public class ScreenScraperServiceClient
    {
        private const string BaseUrl = @"https://www.screenscraper.fr/api2/";
        private readonly RestClient client = new(BaseUrl);
        private readonly ScreenScraperMetadataSettings settings;
        private Dictionary<string, string> systemIdToNameMap = new();
        private Dictionary<string, string> systemNameToIdMap = new();

        public ScreenScraperServiceClient(ScreenScraperMetadataSettings settings)
        {
            this.settings = settings;
            InitSystemIds();
        }

        public JeuInfo? GetJeuInfo(Game gameInfo)
        {
            var request = new RestRequest("jeuInfos.php")
                .AddParameter("devid", "BortSampson55")
                .AddParameter("devpassword", "gegcnAK7zdT")
                .AddParameter("ssid", settings.Username)
                .AddParameter("sspassword", settings.Password)
                .AddParameter("softname", "SuperScraper-0.1")
                .AddParameter("output", "json");

            if (settings.ShouldUsePlayniteGameName)
            {
                request.AddParameter("romnom", gameInfo.Name);
            }
            else
            {
                request.AddParameter("romnom", gameInfo.GetRomFileName() ?? gameInfo.Name);
                request.AddParameter("romtaille", gameInfo.GetRomFileSize());
            }

            var specificationId = gameInfo.Platforms?[0]?.SpecificationId;
            if (specificationId != null)
            {
                systemNameToIdMap.TryGetValue(specificationId, out var systemId);
                if (systemId != null) request.AddParameter("systemeid", systemId);
            }

            if (gameInfo.HasRomFile())
            {
                request.AddParameter("romtype", "rom");
                if (settings.ShouldUseMd5Hash)
                {
                    request.AddParameter("md5", gameInfo.GetRomMd5Hash());
                }
            }

            var response = client.Execute<JeuInfo>(request);
            try
            {
                return JsonConvert.DeserializeObject<JeuInfo>(response.Content);
            }
            catch (JsonException e)
            {
                LogManager.GetLogger().Warn("Unable to parse json response: " + e.Message);
            }

            return null;
        }

        public byte[] DownloadFile(string url)
        {
            var restClient = new RestClient();
            return restClient.DownloadData(new RestRequest(url, Method.GET));
        }

        private void InitSystemIds()
        {

            systemNameToIdMap = new Dictionary<string, string>
            {
                { "3do", "29" },
                { "amstrad_cpc", "65" },
                { "apple_2", "86" },
                { "atari_8bit", "43" },
                { "atari_2600", "26" },
                { "atari_5200", "40" },
                { "atari_7800", "41" },
                { "atari_jaguar", "27" },
                { "atari_lynx", "28" },
                { "atari_st", "42" },
                { "bandai_wonderswan", "45" },
                { "bandai_wonderswan_color", "46" },
                { "coleco_vision", "48" },
                { "commodore_64", "66" },
                { "commodore_amiga", "64" },
                { "commodore_amiga_cd32", "130" },
                { "commodore_vci20", "73" },
                { "mattel_intellivision", "115" },
                { "nintendo_3ds", "17" },
                { "nintendo_64", "14" },
                { "nintendo_ds", "15" },
                { "nintendo_famicom_disk", "106" },
                { "nintendo_gameboy", "9" },
                { "nintendo_gameboyadvance", "12" },
                { "nintendo_gameboycolor", "10" },
                { "nintendo_gamecube", "13" },
                { "nintendo_nes", "3" },
                { "nintendo_super_nes", "4" },
                { "nintendo_switch", "225" },
                { "nintendo_virtualboy", "11" },
                { "nintendo_wii", "16" },
                { "nintendo_wiiu", "18" },
                { "snk_neogeopocket", "25" },
                { "snk_neogeopocket_color", "82" },
                { "nec_pcfx", "72" },
                { "nec_supergrafx", "105" },
                { "nec_turbografx_16", "31" },
                { "nec_turbografx_cd", "114" },
                { "pc_dos", "135" },
                { "pc_windows", "138" },
                { "sony_playstation", "57" },
                { "sony_playstation2", "58" },
                { "sony_playstation3", "59" },
                { "sony_psp", "61" },
                { "sony_vita", "62" },
                { "sega_saturn", "22" },
                { "sega_32x", "19" },
                { "sega_cd", "20" },
                { "sega_genesis", "1" },
                { "sega_gamegear", "21" },
                { "sega_mastersystem", "2" },
                { "sega_dreamcast", "23" },
                { "snk_neogeo_cd", "70" },
                { "sinclair_zx81", "77" },
                { "sinclair_zxspectrum", "76" },
                { "vectrex", "102" },
                { "xbox", "32" },
                { "xbox360", "33" }
            };

            systemIdToNameMap =
            systemNameToIdMap.ToDictionary(kv => kv.Value, kv => kv.Key);

            //Add extra names
            systemNameToIdMap.Add("commodore_plus4", "66");
        }
    }

    public static class GameInfoExtensions
    {
        private static string? GetRomPath(this Game gameInfo)
        {
            return gameInfo.Roms[0]?.Path;
        }
        
        public static bool HasRomFile(this Game gameInfo)
        {
            return File.Exists(gameInfo.GetRomPath());
        }

        public static long GetRomFileSize(this Game gameInfo)
        {
            var romPath = gameInfo.GetRomPath();
            if (romPath == null) return 0;
            try
            {
                return new FileInfo(romPath).Length;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public static string? GetRomFileName(this Game gameInfo)
        {
            return Path.GetFileName(gameInfo.Roms[0]?.Path);
        }


        public static string GetRomMd5Hash(this Game gameInfo)
        {
            var romPath = gameInfo.GetRomPath();
            if (romPath == null) return "";
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(romPath);
            var bytes = md5.ComputeHash(stream);
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}