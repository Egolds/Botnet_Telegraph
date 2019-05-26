using System.Net;

namespace botnet_telegraph_controller_yt
{
    static class web
    {
        public static string GetHTML(string URI)
        {
            using(WebClient wc = new WebClient())
            {
                wc.Proxy = null;

                return wc.DownloadString(URI);
            }
        }

        public static string SendPOST(string URI, string PostData)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Proxy = null;

                return wc.UploadString(URI, "POST", PostData);
            }
        }
    }
}
