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
                .AddParameter("output", "json")
                .AddParameter("romnom", gameInfo.GetRomFileName() ?? gameInfo.Name)
                .AddParameter("romtaille", gameInfo.GetRomFileSize());

            if (gameInfo.Platforms != null)
            {
                MetadataSpecProperty specProperty = new(gameInfo.Platforms[0].SpecificationId);

                systemNameToIdMap.TryGetValue(specProperty.ToString(), out var systemId);
                if (systemId != null) request.AddParameter("systemeid", systemId);
            }
            
            if (gameInfo.HasRomFile())
            {
                request.AddParameter("romtype", "rom");
     //           request.AddParameter("md5", gameInfo.GetRomMd5Hash());
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

        public string? GetPlatformNameById(string platformId)
        {
            systemIdToNameMap.TryGetValue(platformId, out var name);
            return name;
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
                { "3DO Interactive Multiplayer", "29" },
                { "Nintendo 3DS", "17" },
                { "Commodore Amiga", "64" },
                { "Commodore Amiga (AGA)", "111" },
                { "Commodore Amiga CD32", "130" },
                { "Commodore Amiga CDTV", "129" },
                { "Amstrad CPC", "65" },
                { "Apple II", "86" },
                { "Arcade", "75" },
                { "Emerson Arcadia 2001", "94" },
                { "Bally Astrocade", "44" },
                { "Atari 800", "43" },
                { "Atari 2600", "26" },
                { "Atari 5200", "40" },
                { "Atari 7800", "41" },
                { "Atari Jaguar", "27" },
                { "Atari Jaguar CD", "171" },
                { "Atari Lynx", "28" },
                { "Atari ST/STE/TT/Falcon", "42" },
                { "Capcom CP System I", "6" },
                { "Capcom CP System II", "7" },
                { "Capcom CP System III", "8" },
                { "CAVE CV1000", "47" },
                { "Commodore 64", "66" },
                { "Channel F", "80" },
                { "Coleco ColecoVision", "48" },
                { "Daphne", "49" },
                { "Dragon 32", "91" },
                { "Sega Dreamcast", "23" },
                { "Nintendo Family Computer Disk System", "106" },
                { "Handheld Electronic Game", "52" },
                { "Sega Game Gear", "21" },
                { "Nintendo Game Boy", "9" },
                { "Nintendo Game Boy Advance", "12" },
                { "Nintendo Game Boy Color", "10" },
                { "Nintendo GameCube", "13" },
                { "Sega Genesis", "1" },
                { "Mattel Intellivision", "115" },
                { "Sega Master System", "2" },
                { "Sega CD", "20" },
                { "Microsoft MSX", "113" },
                { "Microsoft MSX2", "116" },
                { "Nintendo 64", "14" },
                { "Nintendo DS", "15" },
                { "SNK Neo Geo", "142" },
                { "SNK Neo Geo CD", "70" },
                { "Nintendo Entertainment System", "3" },
                { "SNK Neo Geo Pocket", "25" },
                { "SNK Neo Geo Pocket Color", "82" },
                { "OpenBOR", "214" },
                { "Oric Atmos", "131" },
                { "DOS", "135" },
                { "NEC PC-9801", "208" },
                { "NEC PC-FX", "72" },
                { "PC (Windows)", "138" },
                { "NEC SuperGrafx", "105" },
                { "NEC TurboGrafx 16", "31" },
                { "NEC TurboGrafx-CD", "114" },
                { "Microsoft Xbox", "32" },
                { "Philips CD-i", "133" },
                { "Microsoft Xbox 360", "33" },
                { "Pokemon Mini", "211" },
                { "Sony PlayStation 2", "58" },
                { "Sony Playstation Portable", "61" },
                { "Sony Playstation", "57" },
                { "Sega Saturn", "22" },
                { "ScummVM", "123" },
                { "Sega 32X", "19" },
                { "Gaelco", "194" },
                { "Sega SG 1000", "109" },
                { "Super Nintendo Entertainment System", "4" },
                { "Nintendo Switch", "225" },
                { "Texas Instruments TI 99/4A", "205" },
                { "Tandy TRS-80 Color Computer", "144" },
                { "GCE Vectrex", "102" },
                { "Commodore VIC20", "73" },
                { "Magnavox Odyssey2", "104" },
                { "Nintendo Virtual Boy", "11" },
                { "Nintendo Wii", "16" },
                { "Nintendo Wii U", "18" },
                { "Bandai WonderSwan", "45" },
                { "Bandai WonderSwan Color", "46" },
                { "Sharp X68000", "79" },
                { "Sinclair ZX 81", "77" },
                { "Sinclair ZX Spectrum", "76" },
                { "Sammy Atomiswave", "53" }
            };

            systemIdToNameMap =
                systemNameToIdMap.ToDictionary(kv => kv.Value, kv => kv.Key);

            //Add extra names
            systemNameToIdMap.Add("Apple IIgs", "86");
            systemNameToIdMap.Add("Apple III", "86");
            systemNameToIdMap.Add("Commodore PLUS4", "66");
            systemNameToIdMap.Add("Nintendo Game & Watch", "52");
            systemNameToIdMap.Add("Nintendo 64DD", "14");
            systemNameToIdMap.Add("Nintendo Satellaview", "2");
            systemNameToIdMap.Add("Nintendo Sufami Turbo", "2");
            systemNameToIdMap.Add("PC Engine SuperGrafx", "5");
            systemIdToNameMap.Add("Mame 2003 Plus", "75");
        }
    }

    public static class GameInfoExtensions
    {
        public static bool HasRomFile(this Game gameInfo)
        {
            return File.Exists(gameInfo.Roms[0].Path);
        }

        public static long GetRomFileSize(this Game gameInfo)
        {
            if (gameInfo.Roms[0].Path == null) return 0;

            try
            {
                return new FileInfo(gameInfo.Roms[0].Path).Length;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        public static string? GetRomFileName(this Game gameInfo)
        {
            return Path.GetFileName(gameInfo.Roms[0].Path);
        }


        public static string GetRomMd5Hash(this Game gameInfo)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(gameInfo.Roms[0].Path);
            var bytes = md5.ComputeHash(stream);
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}