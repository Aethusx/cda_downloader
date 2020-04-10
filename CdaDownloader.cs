using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

static class CdaDownloader
{
    static HttpClient web = new HttpClient();
    static Regex regex_link = new Regex(@"https:\/\/www.cda.pl\/video\/([^\/\s]+)");
    static Regex regex_file = new Regex(@"""file"":""(.*?)(?:"")");
    static readonly string[] available_quality = { "360p", "480p", "720p", "1080p" };

    static CdaDownloader()
    {
        web.DefaultRequestHeaders.Add("Referer", "https://www.cda.pl");
        web.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0");
        web.DefaultRequestHeaders.Add("Accept-Encoding", "identity");
    }

    /* cda robi przemagiczne rzeczy z tymi stringami */
    public static string DecryptKey(string key, bool https = false)
    {
        string result = String.Empty;
        key = Uri.UnescapeDataString(key);

        /* spytaj sie cda o co im chodzi nie mnie */
        key = key.Replace("_XDDD", "");
        key = key.Replace("_CDA", "");
        key = key.Replace("_ADC", "");
        key = key.Replace("_CXD", "");
		key = key.Replace("_QWE", "");
        key = key.Replace("_Q5", "");
		
        foreach (char c in key)
            result += (c >= 33 && c <= 126) ? (char)(33 + ((c + 14) % 94)) : c;

        result = result.Replace(".cda.mp4", "");
        result = result.Replace(".2cda.pl", ".cda.pl");
        result = result.Replace(".3cda.pl", ".cda.pl");

        return (https) ? "https://" : "http://" + result + ".mp4";
    }

    public static string GetVideoLink(string link, string quality = null, bool https = false)
    {
        if (link.EndsWith("/vfilm"))
            link = link.Substring(0, link.Length - 5);

        if (link.EndsWith("/"))
            link = link.Substring(0, link.Length - 1);

        if (link.StartsWith("http://"))
            link = "https://" + link.Substring(7, link.Length - 7);

        if (!regex_link.Match(link).Success)
            return null;

        if (available_quality.Contains(quality))
            link = link + "?wersja=" + quality;

        /* mozna wyciagac to łopatologicznie ze z html tagów potem json objekt ale regex jest szybszy i krótszy :P :D */
        var task = Task.Run(() => web.GetAsync(link).Result.Content.ReadAsStringAsync());
        task.Wait();

        Match match = regex_file.Match(task.Result);

        if (match.Success && match.Groups.Count >= 2)
            return DecryptKey(match.Groups[1].Value, https);

        return null;
    }
}
