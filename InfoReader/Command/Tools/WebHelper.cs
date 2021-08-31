using System;
using System.IO;
using System.Net;
using System.Text;
using Sync.Tools;

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

        static string GetWebPageContentInternal(string url,out Exception executingException, int timeout = 100000, int maxRetryCount = 5, int currentRetry = 0)
        {
            executingException = null;
            try
            {
                Uri u = new Uri(UrlAutoComplete(url));
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Timeout = timeout;
                StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream() ?? new MemoryStream());
                return reader.ReadToEnd();
            }
            catch (Exception e)
            {
                executingException = e;
                return null;
            }
            
        }
        public static string GetWebPageContent(string url,int maxRetryCount = 5, int timeout = 100000)
        {
            int currentRetry = 0;
            string ret;
            Exception executingException;
            do
            {
                ret = GetWebPageContentInternal(url,out executingException, timeout, maxRetryCount, currentRetry);
                if (string.IsNullOrEmpty(ret))
                    currentRetry++;
            } while (string.IsNullOrEmpty(ret) && currentRetry <= maxRetryCount);
            if(string.IsNullOrEmpty(ret))
                IO.CurrentIO.WriteColor(executingException.ToString(),ConsoleColor.Red);
            return ret ?? "";
        }

    }
}