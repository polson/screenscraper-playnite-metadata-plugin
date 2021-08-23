using System.Collections.Generic;

namespace ScreenScraperMetadata.Models
{
    public class Header
    {
        public string APIversion { get; set; }
        public string dateTime { get; set; }
        public string commandRequested { get; set; }
        public string success { get; set; }
        public string error { get; set; }
    }

    public class Serveurs
    {
        public string cpu1 { get; set; }
        public string cpu2 { get; set; }
        public string threadsmin { get; set; }
        public string nbscrapeurs { get; set; }
        public string apiacces { get; set; }
        public string closefornomember { get; set; }
        public string closeforleecher { get; set; }
    }

    public class Ssuser
    {
        public string id { get; set; }
        public string numid { get; set; }
        public string niveau { get; set; }
        public string contribution { get; set; }
        public string uploadsysteme { get; set; }
        public string uploadinfos { get; set; }
        public string romasso { get; set; }
        public string uploadmedia { get; set; }
        public string propositionok { get; set; }
        public string propositionko { get; set; }
        public string quotarefu { get; set; }
        public string maxthreads { get; set; }
        public string maxdownloadspeed { get; set; }
        public string requeststoday { get; set; }
        public string requestskotoday { get; set; }
        public string maxrequestspermin { get; set; }
        public string maxrequestsperday { get; set; }
        public string maxrequestskoperday { get; set; }
        public string visites { get; set; }
        public string datedernierevisite { get; set; }
        public string favregion { get; set; }
    }

    public class Nom
    {
        public string region { get; set; }
        public string text { get; set; }
        public string langue { get; set; }
    }

    public class Regions
    {
        public string? shortname { get; set; }
    }

    public class Systeme
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class Editeur
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class Developpeur
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class Joueurs
    {
        public string text { get; set; }
    }

    public class Note
    {
        public string text { get; set; }
    }

    public class Synopsis
    {
        public string? langue { get; set; }
        public string? text { get; set; }
    }

    public class Classification
    {
        public string? type { get; set; }
        public string? text { get; set; }
    }

    public class Date
    {
        public string? region { get; set; }
        public string? text { get; set; }
        public string? date_us { get; set; }
    }

    public class Genre
    {
        public string? id { get; set; }
        public string? nomcourt { get; set; }
        public string? principale { get; set; }
        public string? parentid { get; set; }
        public List<Nom> noms { get; set; }
    }

    public class Famille
    {
        public string? id { get; set; }
        public string? nomcourt { get; set; }
        public string? principale { get; set; }
        public string? parentid { get; set; }
        public List<Nom> noms { get; set; }
    }

    public class Media
    {
        public string type { get; set; }
        public string? parent { get; set; }
        public string url { get; set; }
        public string? region { get; set; }
        public string? crc { get; set; }
        public string? md5 { get; set; }
        public string? sha1 { get; set; }
        public string? size { get; set; }
        public string? format { get; set; }
        public string? posx { get; set; }
        public string? posy { get; set; }
        public string? posw { get; set; }
        public string? posh { get; set; }
    }

    public class Rom
    {
        public string id { get; set; }
        public string? romnumsupport { get; set; }
        public string? romtotalsupport { get; set; }
        public string? romfilename { get; set; }
        public string? romregions { get; set; }
        public string? romtype { get; set; }
        public string? romsupporttype { get; set; }
        public string? romsize { get; set; }
        public string? romcrc { get; set; }
        public string? rommd5 { get; set; }
        public string? romsha1 { get; set; }
        public string? romcloneof { get; set; }
        public string? beta { get; set; }
        public string? demo { get; set; }
        public string? proto { get; set; }
        public string? trad { get; set; }
        public string? hack { get; set; }
        public string? unl { get; set; }
        public string? alt { get; set; }
        public string? best { get; set; }
        public string? netplay { get; set; }
        public string? editeur { get; set; }
    }

    public class Jeu
    {
        public string id { get; set; }
        public string romid { get; set; }
        public string notgame { get; set; }
        public List<Nom> noms { get; set; }
        public Regions? regions { get; set; }
        public string? cloneof { get; set; }
        public Systeme systeme { get; set; }
        public Editeur? editeur { get; set; }
        public Developpeur? developpeur { get; set; }
        public Joueurs? joueurs { get; set; }
        public Note? note { get; set; }
        public string? topstaff { get; set; }
        public string? rotation { get; set; }
        public List<Synopsis>? synopsis { get; set; }
        public List<Classification>? classifications { get; set; }
        public List<Date>? dates { get; set; }
        public List<Genre>? genres { get; set; }
        public List<Famille>? familles { get; set; }
        public List<Media> medias { get; set; }
        public List<Rom> roms { get; set; }
        public Rom? rom { get; set; }
    }

    public class Response
    {
        public Serveurs serveurs { get; set; }
        public Ssuser ssuser { get; set; }
        public Jeu jeu { get; set; }
    }

    public class JeuInfo
    {
        public Header header { get; set; }
        public Response response { get; set; }
    }
}