using System;
using System.IO;
using System.Net;
using System.Text;

namespace InfoReaderPlugin.Command.Tools
{
    public static class WebHelper
    {
        public static string Combine(string baseUrl,params string[] parts)
        {
            StringBuilder urlBuilder = new StringBuilder(baseUrl);
            if (!baseUrl.EndsWith("/"))
            {
                urlBuilder.Append("/");
            }
            for (int i = 0; i < parts.Length; i++)
            {
                urlBuilder.Append(parts[i]);
                if (i < parts.Length - 1)
                    urlBuilder.Append("/");
            }

            return urlBuilder.ToString();
        }
        public static string UrlAutoComplete(string orignal,string defSchema = "http://")
        {
            if (string.IsNullOrEmpty(orignal))
            {
                throw new ArgumentNullException();
            }
            string schema = string.Empty;
            string urlWithoutSchema;
            int colonIndex = -1;
            for (int i = 0; i < orignal.Length; i++)
            {
                if (orignal[i] != ':') continue;
                colonIndex = i;
                break;
            }

            if (colonIndex > 0 && orignal.Substring(colonIndex, 3) == "://")
            {
                schema = orignal.Substring(0, colonIndex + 3);
                urlWithoutSchema = orignal.Substring(colonIndex + 3);
            }
            else
            {
                schema = defSchema;
                urlWithoutSchema = orignal;
            }
            if (string.IsNullOrEmpty(schema) || string.IsNullOrEmpty(urlWithoutSchema))
                throw new ArgumentException();
            return schema + Uri.EscapeUriString(urlWithoutSchema);
        }
        public static string GetWebPageContent(string url)
        {
            try
            {
                Uri u = new Uri(UrlAutoComplete(url));
                HttpWebRequest request = WebRequest.CreateHttp(url);
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream() ?? new MemoryStream());
                return reader.ReadToEnd();
            }
            catch (Exception)
            {
                return null;
            }
            
        }

    }
}