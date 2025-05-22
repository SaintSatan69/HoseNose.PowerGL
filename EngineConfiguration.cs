using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Policy;
using System.Text.Json;
namespace HoseRenderer
{
    /// <summary>
    /// An Object Representing the Render engines configuration to be serialized and deserialized from the ENGINECONFIG.ENGINECONFIG json file
    /// </summary>
    public class EngineConfiguration : IEquatable<EngineConfiguration?>
    {
        //MAKE SURE IF YOU ADD MORE PROPERTIES YOU GIVE THEM DEFAULT PROPERTIES FOR WHEN GENERATEDEFAULTCONFIG() IS CALLED
        /// <summary>
        /// A Serial Number 
        /// </summary>
        [JsonProperty]
        public int ConfigurationNumber { get; private set; } = 0;
        /// <summary>
        /// The Width of the Window
        /// </summary>
        [JsonProperty]
        public int WindowX { get; private set; } = 1920;
        /// <summary>
        /// The Height of the window
        /// </summary>
        [JsonProperty]
        public int WindowY { get; private set; } = 1080;
        /// <summary>
        /// The Path to the folder that contains the HoseRenderer exe and DLL for uniform resource access to engine default resources
        /// </summary>
        [JsonProperty]
        public string EnginePath { get; } = @"C:\Program Files\PowerShell\7-preview\Modules\PowerGl";
        /// <summary>
        /// The Path to the .EngineConfig used at run time to configure some engine specific settings such as window size, title, etc.
        /// </summary>
        [JsonProperty]
        public string ConfigFilePath { get; } = @"C:\Program Files\Powershell\7-preview\Modules\PowerGL\PowerGL.EngineConfig";
        /// <summary>
        /// A bool determining whether vsync is enabled. there is no implementation of a tick speed so physics will move faster if the FPS is over 60
        /// </summary>
        [JsonProperty]
        public bool IsVSyncEnabled { get; private set; } = true;
        /// <summary>
        /// The Title of the PowerGL window
        /// </summary>
        [JsonProperty]
        public string WindowTitle { get; private set; } = "HoseNose.PowerGL";
        /// <summary>
        /// The Version of the engine that the configuration was made in
        /// </summary>
        [JsonProperty]
        public string EngineBuildVersion { get; private set; } = MainRenderer.EngineVersion;

        /// <summary>
        /// States whether or not the http listener gets enabled as an alternate API to the named pipe
        /// </summary>
        [JsonProperty]
        public bool IsHTTPAPIEnabled { get; private set; } = false;
        /// <summary>
        /// States whether or not the listener is readonly {GET}
        /// </summary>
        [JsonProperty]
        public bool IsHTTPReadOnly {  get; private set; } = false;
        /// <summary>
        /// The Port at which the HTTP API would be listening to
        /// </summary>
        [JsonProperty]
        public int HTTPPort { get; private set; } = 6969;
        /// <summary>
        /// Main constructor for the engine configuration
        /// </summary>
        public EngineConfiguration(string ConfigFilePath,string EnginePath, int WindowSizeX, int WindowSizeY, bool IsVSyncEnabled)
        {
            this.ConfigFilePath = ConfigFilePath;
            this.EnginePath = EnginePath;
            this.WindowX = WindowSizeX;
            this.WindowY = WindowSizeY;
            this.IsVSyncEnabled = IsVSyncEnabled;
        }
        /// <summary>
        /// Parametereless construction for use for derserialization/Or if there is no engine configuration defined
        /// </summary>
        public EngineConfiguration(){}
        /// <summary>
        /// Used By Engine at init to gather engine configuration setting defined by the user of the engine
        /// </summary>
        /// <param name="ConfigFilePath"></param>
        /// <returns></returns>
        public static EngineConfiguration ReadEngineConfig(string ConfigFilePath)
        {
            //if the engine fails to load its config it will attempt to generate a default one in the users temp\powergl folder to stop it from exploding when they give the engine a bad config, otherwise it will deserialize
            if (ConfigFilePath == "")
            {
                EngineConfiguration config = GenerateDefaultConfig();
                WriteEngineConfig(MainRenderer.UserDir + @"\PowerGL\PowerGL.EngineConfig", config);
                return config;
            }
            if (File.Exists(ConfigFilePath))
            {
                byte[] _Config_File = File.ReadAllBytes(ConfigFilePath);
                Utf8JsonReader utfreader = new Utf8JsonReader(_Config_File);
                return System.Text.Json.JsonSerializer.Deserialize<EngineConfiguration>(ref utfreader);
            }
            else
            {
                EngineConfiguration config = GenerateDefaultConfig();
                WriteEngineConfig(config.ConfigFilePath,config);
                return config;
            }
        }
        /// <summary>
        /// Writes the JSON representation of the EngineConfiguration object to disk for distributing/modifying engine attributes
        /// </summary>
        /// <param name="ConfigFilePath"></param>
        /// <param name="config"></param>
        public static void WriteEngineConfig(string ConfigFilePath, EngineConfiguration config)
        {
            config.ConfigurationNumber++;
            Stream Config_file = File.Open(ConfigFilePath,FileMode.Create,FileAccess.ReadWrite,FileShare.Read);
            Config_file.Write(System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(config,JsonSerializerOptions.Default));
            Config_file.Close();
        }
        /// <summary>
        /// A wrapper for the parameterless ctor, this can be called instead of the raw ctor to make what your trying to do more clear
        /// </summary>
        /// <returns>The default EngineConfiguration</returns>
        private static EngineConfiguration GenerateDefaultConfig()
        {
            return new EngineConfiguration();
        }
        /// <summary>
        /// Modifies the current engine configuration and writes it to the same file it currently is
        /// </summary>
        public void UpdateConfiguration(string Property, string Value)
        {
            switch (Property)
            {
                case "WindowX":
                    this.WindowX = Convert.ToInt32(Value);
                    break;
                case "WindowY":
                    this.WindowY = Convert.ToInt32(Value);
                    break;
                case "Title":
                    this.WindowTitle = Value;
                    break;
                case "Vsync":
                    this.IsVSyncEnabled = Convert.ToBoolean(Value);
                    break;
                default:
                    return;
            }
        }
        public void SaveConfig()
        {
            WriteEngineConfig(this.ConfigFilePath, this);
        }
        public override bool Equals(object? obj)
        {
            return Equals(obj as EngineConfiguration);
        }

        public bool Equals(EngineConfiguration? other)
        {
            return other is not null &&
                   WindowX == other.WindowX &&
                   WindowY == other.WindowY &&
                   IsVSyncEnabled == other.IsVSyncEnabled &&
                   WindowTitle == other.WindowTitle;
        }
    }
}