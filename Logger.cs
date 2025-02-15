﻿
namespace HoseRenderer
{
    public class Logger
    {
        public readonly string Source;
        public readonly string Log_Directory;
        public Logger(string source,string? logfilefoler) { 
            Source = source;
            if (logfilefoler != null) {
                Log_Directory = logfilefoler;
            }
            else
            {
                Log_Directory = $@"C:\users\{Environment.UserName}\appdata\local\temp\PowerGL\logs";
            }
            this.GenerateLogsFolder();
        }
        public void Log(string message) {
            string PathRoot = this.Log_Directory;
            string Filepath = PathRoot + @"\" + this.Source + ".log";
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
                    FileLoc.Close();
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
        private void GenerateLogsFolder()
        {
            string logsfolder = this.Log_Directory;
            if (!Directory.Exists(logsfolder))
            {
                Directory.CreateDirectory(logsfolder);
            }
        }
    }
}
