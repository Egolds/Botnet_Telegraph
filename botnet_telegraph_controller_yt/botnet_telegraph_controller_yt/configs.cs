namespace botnet_telegraph_controller_yt
{
    static class configs
    {
        public static string server { get; set; }
        public static string server_title { get; set; }

        public static string spliter { get; } = "{split}";
        public static string spliter2 { get; } = "{split2}";

        public static string auth_file { get; } = "TokenData";
        public static string bots_list_file { get; } = "BotsList";

        public static int MinutesBeforOffline { get; set; } = 5;

        public static string[] BotCommands = new string[] {
            "nothing",
            "open_link",
            "download_execute",
            "exit"
        };
    }
}
