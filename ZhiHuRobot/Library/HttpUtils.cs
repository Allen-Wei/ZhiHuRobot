using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.IO;

namespace ZhiHuRobot.Library
{
    public class HttpUtils
    {
        #region Get Async
        public static async Task<byte[]> Bytes(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public static async Task<string> String(string url)
        {
            return (await Bytes(url)).DumpToString();
        }
        public static async Task<HtmlDocument> Html(string url)
        {
            var html = await HttpUtils.String(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }

        #endregion

        #region Get Sync
        public static byte[] BytesSync(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadData(url);
            }
        }
        public static string StringSync(string url)
        {
            return (BytesSync(url)).DumpToString();
        }
        public static HtmlDocument HtmlSync(string url)
        {
            var html = HttpUtils.StringSync(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        #endregion



        #region Post Async
        public static async Task<byte[]> Bytes(string url, string data)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent(data, Encoding.UTF8);
                HttpResponseMessage response = await client.PostAsync(url, content);
                return await response.Content.ReadAsByteArrayAsync();
            }
        }
        public static async Task<string> String(string url, string data)
        {
            return (await HttpUtils.Bytes(url, data)).DumpToString();
        }

        public static async Task<HtmlDocument> Html(string url, string data)
        {
            var html = await HttpUtils.String(url, data);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        #endregion




        #region Post Sync
        public static byte[] BytesSync(string url, string data)
        {
            WebRequest req = WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            req.ContentLength = data.Length;

            using (StreamWriter writer = new StreamWriter(req.GetRequestStream()))
            {
                writer.Write(data);
            }

            using (StreamReader reader = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                return Encoding.UTF8.GetBytes(reader.ReadToEnd());
            }

        }
        public static string StringSync(string url, string data)
        {
            return HttpUtils.BytesSync(url, data).DumpToString();
        }

        public static HtmlDocument HtmlSync(string url, string data)
        {
            var html = HttpUtils.StringSync(url, data);
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        #endregion
    }
}
