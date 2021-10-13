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
                systemNameToIdMap.TryGetValue(gameInfo.Platforms[0]?.SpecificationId.ToString(), out var systemId);
                if (systemId != null) request.AddParameter("systemeid", systemId);
            }

            if (gameInfo.HasRomFile())
            {
                request.AddParameter("romtype", "rom");
                request.AddParameter("md5", gameInfo.GetRomMd5Hash());
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

        //all the commented systems don't have a specification in P9  
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
                { "pc_dos", "135" },
                { "vectrex", "102" },
                { "mattel_intellivision", "115" },
                { "nintendo_3ds", "17" },
                { "nintendo_64", "14" },
                { "nintendo_famicom_disk", "106" },
                { "nintendo_gameboy", "9" },
                { "nintendo_gameboyadvance", "12" },
                { "nintendo_gameboycolor", "10" },
                { "nintendo_gamecube", "13" },
                { "nintendo_ds", "15" },
                { "nintendo_switch", "225" },
                { "nintendo_nes", "3" },
                { "nintendo_super_nes", "4" },
                { "nintendo_virtualboy", "11" },
                { "nintendo_wii", "16" },
                { "nintendo_wiiu", "18" },
                { "snk_neogeopocket", "25" },
                { "snk_neogeopocket_color", "82" },
                { "xbox", "32" },
                { "xbox360", "33" },
                { "nec_pcfx", "72" },
                { "nec_supergrafx", "105" },
                { "nec_turbografx_16", "31" },
                { "nec_turbografx_cd", "114" },
                { "pc_windows", "138" },
                { "sony_playstation2", "58" },
                { "sony_psp", "61" },
                { "sony_playstation", "57" },
                { "sega_saturn", "22" },
                { "sega_32x", "19" },
                { "sega_cd", "20" },
                { "sega_genesis", "1" },
                { "sega_gamegear", "21" },
                { "sega_mastersystem", "2" },
                { "sega_dreamcast", "23" },
                { "snk_neogeo_cd", "70" },
                { "sinclair_zx81", "77" },
                { "sinclair_zxspectrum", "76" }
                //{ "Arcade", "75" },
                //{ "Atari Jaguar CD", "171" },
                //{ "Bally Astrocade", "44" },
                //{ "Commodore Amiga (AGA)", "111" },
                //{ "Commodore Amiga CDTV", "129" },
                //{ "Emerson Arcadia 2001", "94" },
                //{ "Capcom CP System I", "6" },
                //{ "Capcom CP System II", "7" },
                //{ "Capcom CP System III", "8" },
                //{ "CAVE CV1000", "47" },
                //{ "Channel F", "80" },
                //{ "Daphne", "49" },
                //{ "Dragon 32", "91" },
                //{ "Handheld Electronic Game", "52" },
                //{ "Gaelco", "194" },
                //{ "Microsoft MSX", "113" },
                //{ "Microsoft MSX2", "116" },
                //{ "Magnavox Odyssey2", "104" },
                //{ "NEC PC-9801", "208" },
                //{ "OpenBOR", "214" },
                //{ "Oric Atmos", "131" },
                //{ "Philips CD-i", "133" },
                //{ "Pokemon Mini", "211" },
                //{ "Sammy Atomiswave", "53" },
                //{ "ScummVM", "123" },
                //{ "Sega SG 1000", "109" },
                //{ "SNK Neo Geo", "142" },
                //{ "Texas Instruments TI 99/4A", "205" },
                //{ "Tandy TRS-80 Color Computer", "144" },
                //{ "Sharp X68000", "79" },
            };

            systemIdToNameMap =
            systemNameToIdMap.ToDictionary(kv => kv.Value, kv => kv.Key);

            //Add extra names
            systemNameToIdMap.Add("commodore_plus4", "66");
            //systemNameToIdMap.Add("Apple IIgs", "86");
            //systemNameToIdMap.Add("Apple III", "86");
            //systemNameToIdMap.Add("Nintendo Game & Watch", "52");
            //systemNameToIdMap.Add("Nintendo 64DD", "14");
            //systemNameToIdMap.Add("Nintendo Satellaview", "2");
            //systemNameToIdMap.Add("Nintendo Sufami Turbo", "2");
            //systemNameToIdMap.Add("PC Engine SuperGrafx", "5");


            //systemIdToNameMap.Add("Mame 2003 Plus", "75");
        }
    }

    public static class GameInfoExtensions
    {
        public static bool HasRomFile(this Game gameInfo)
        {
            return File.Exists(gameInfo.Roms[0]?.Path);
        }

        public static long GetRomFileSize(this Game gameInfo)
        {
            if (gameInfo.Roms[0]?.Path == null) return 0;

            try
            {
                return new FileInfo(gameInfo.Roms[0]?.Path).Length;
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
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(gameInfo.Roms[0].Path);
            var bytes = md5.ComputeHash(stream);
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}