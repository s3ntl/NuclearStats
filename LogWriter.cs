
using System.IO;



namespace NS
{
    public static class LogWriter
    {
        private static string fileName = "NuclearStatsLog";
        private static string logPath = Plugin.LogPath.Value;
        private static string path = $"{logPath}" + @"\NuclearStatsLog.txt";

        //C:\\Users\\lordc\\Desktop\\Escalation Server\\NuclearStats\\{0}.txt
        //C:\\Users\\s3nt1\\Desktop\\{0}.txt

        public static void WriteLog(string line)
        {
            
            //Plugin.DebugLog($"pathprefix: {path}");
            if (File.Exists(path))
            {
                using (StreamWriter writer = new StreamWriter(path, true)) 
                {
                    writer.WriteLine(line); 
                }
            }
            else
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.WriteLine(line);
                }
            }
            //Plugin.DebugLog($"pathpostfix: {path}"); 
        }
    }
}
