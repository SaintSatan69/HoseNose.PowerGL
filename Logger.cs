
using System.Diagnostics;

namespace HoseRenderer
{
    /// <summary>
    /// A class that handles internal logging of the program and powershell so that if something goes wrong its easier to see what is broken
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// Source is the Source of logs such as the IPC initialization or powershell processing something
        /// </summary>
        public readonly string Source;
        /// <summary>
        /// the directory that logs go in will default to the users temp folder
        /// </summary>
        public readonly string Log_Directory;
        /// <summary>
        /// A logger that will accept messages from any thread that can access the object
        /// </summary>
        /// <param name="source"></param>
        /// <param name="logfilefoler"></param>
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
        private readonly int ThreadID;
        /// <summary>
        /// A Logger that will only take log requests from the threadID its attached to help make it a little more thread safe with out lock
        /// </summary>
        /// <param name="source"></param>
        /// <param name="logfilefoler"></param>
        /// <param name="threadID"></param>
        public Logger(string source, string? logfilefoler,int threadID = 0)
        {
            ThreadID = threadID;
            Source = source;
            if (logfilefoler != null)
            {
                Log_Directory = logfilefoler;
            }
            else
            {
                Log_Directory = $@"C:\users\{Environment.UserName}\appdata\local\temp\PowerGL\logs";
            }
            this.GenerateLogsFolder();
        }
        /// <summary>
        /// writes a message provided to the respective log file in a non-overwriting way 
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="Exception"></exception>
        public void Log(string message) {
            if (Environment.CurrentManagedThreadId != this.ThreadID && this.ThreadID != 0)
            {
                //Note implement a silent abort/somesort of log if a thread that isn't the thread_ID of the spawned logger is trying to access the threads logger/log method (such as a secondary thread trying to read main threads Logger for the rendering engine
                Debugger.Log(1,"","Another Thread attempted to log a message on a different threads locked logger");
                Console.WriteLine($"Thread {Environment.CurrentManagedThreadId} Attmped to log on a different threads locked logger");
                return;
            }
            string PathRoot = this.Log_Directory;
            string Filepath = PathRoot + @"\" + this.Source + ".log";
            if (File.Exists(Filepath)) { 
                
                var FileLoc = new FileStream(Filepath, FileMode.Append,FileAccess.Write,FileShare.ReadWrite);
                var data = MessageToByte(message);
                
                FileLoc.Write(data,0,data.Length);
                FileLoc.Flush();
                FileLoc.Close();
            }
            else 
            {
                try
                {
                    File.Create(Filepath).Close();
                    var FileLoc = new FileStream(Filepath, FileMode.Open, FileAccess.Write,FileShare.ReadWrite);
                    var data = MessageToByte(message);
                    FileLoc.Write(data, 0, data.Length);
                    FileLoc.Flush();
                    FileLoc.Close();
                    Thread.Sleep(10);
                }
                catch(Exception ex) {
                    string exceptionmessage = "Error in writing bytes to filesteam of the provider agent: " + this.Source + " by:" + (ex.GetType().ToString());
                    throw new Exception(exceptionmessage);
                }
            }
        }
        private static byte[] MessageToByte(string message) { 
            List<byte> bytes = new List<byte>();
            string Message = $"[{ DateTime.Now.ToString("HH:mm:ss:ff")}]:" + message + Environment.NewLine;
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
