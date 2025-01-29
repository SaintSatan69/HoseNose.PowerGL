
namespace HoseRenderer
{
    public class Logger
    {
        public readonly string Source;
        public Logger(string source) { 
            Source = source;
            GenerateLogsFolder();
        }
        public void Log(string message) {
            string PathRoot = MainRenderer.Application_Directory + "\\Logs\\";
            string Filepath = PathRoot + this.Source + ".log";
            if (File.Exists(Filepath)) { 
                var FileLoc = File.Open(Filepath, FileMode.Append);
                var data = MessageToByte(message);
                FileLoc.Write(data,0,data.Length);
            }
            else 
            {
                try
                {
                    File.Create(Filepath).Close();
                    var FileLoc = File.Open(Filepath, FileMode.Open, FileAccess.ReadWrite);
                    var data = MessageToByte(message);
                    FileLoc.Write(data, 0, data.Length);
                }
                catch(Exception ex) {
                    string exceptionmessage = "Error in writing bytes to filesteam of the provider agent: " + this.Source + " by:" + (ex.GetType().ToString());
                    throw new Exception(exceptionmessage);
                }
            }
        }
        private static byte[] MessageToByte(string message) { 
            List<byte> bytes = new List<byte>();
            string Message = message + Environment.NewLine;
            for (int i = 0; i < Message.Length;i++)
            {
                bytes.Add((byte)Message[i]);
            }
            return bytes.ToArray();
        }
        private static void GenerateLogsFolder()
        {
            string logsfolder = MainRenderer.Application_Directory + "\\Logs";
            if (!Directory.Exists(logsfolder))
            {
                Directory.CreateDirectory(logsfolder);
            }
        }
    }
}
