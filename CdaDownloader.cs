﻿using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

static class CdaDownloader
{
    static WebClient web = new WebClient();
    static Regex regex_link = new Regex(@"https:\/\/www.cda.pl\/video\/([^\/\s]+)");
    static Regex regex_file = new Regex(@"""file"":""(.*?)(?:"")");
    static readonly string[] available_quality = { "360p", "480p", "720p", "1080p" };

    static CdaDownloader()
    {
        web.Headers.Add("Referer", "https://www.cda.pl");
        web.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:74.0) Gecko/20100101 Firefox/74.0");
        web.Headers.Add("Accept-Encoding", "identity");
    }

    /* cda robi przemagiczne rzeczy z tymi stringami */
    public static string DecryptKey(string key, bool https = false)
    {
        string result = String.Empty;

        foreach(char c in key)
            result += (c >= 33 && c <= 126) ? (char)(33 + ((c + 14) % 94)) : c;

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
        Match match = regex_file.Match(web.DownloadString(link));

        if (match.Success && match.Groups.Count >= 2)
            return DecryptKey(Uri.UnescapeDataString(match.Groups[1].Value), https);

        return null;
    }
}