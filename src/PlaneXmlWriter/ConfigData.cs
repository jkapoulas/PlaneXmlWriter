using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlaneXmlWriter
{
    public sealed class ConfigData
    {
        public const string ConfigFileName = "appsettings.json";

        static JsonSerializerOptions _options;


        private static ConfigData _instance;
        public static ConfigData Instance
        {
            get 
            {
                if (_instance == null)
                {
                    Read();
                }
                return _instance; 
            }
        }
        public static void Read()
        {
            if(!File.Exists(ConfigFileName))
            {
                _instance = new ConfigData();
                Save();
            }
            else
            {
                try
                {
                    var configFileText = File.ReadAllText(ConfigFileName);
                    _instance = JsonSerializer.Deserialize<ConfigData>(configFileText, _options);
                }
                catch(Exception ex)
                {
                    _instance = new ConfigData();
                    Save();
                }
            }
        }

        public static void Save()
        {
            var configFileText = JsonSerializer.Serialize<ConfigData>(_instance, _options);
            File.WriteAllText(ConfigFileName, configFileText);
        }

        static ConfigData()
        {
            _options = new JsonSerializerOptions
            {
                Converters ={
                    new JsonStringEnumConverter( JsonNamingPolicy.CamelCase)
                },

            };
        }

        public string LastFilePath { get; set; }

    }
}
