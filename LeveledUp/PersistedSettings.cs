using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LeveledUp
{
    public class WatcherSettings
    {
        public string FolderToWatch { get; set; }
        public string WatchedFileTypes { get; set; }
        public string Command { get; set; }

        private const string SaveFileName = "settings.xml";

        private static string GetSaveFolderPath()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var saveFolderPath = Path.Combine(appDataPath + @"\LeveledUp\Save");
            if (!Directory.Exists(saveFolderPath))
                Directory.CreateDirectory(saveFolderPath);
            return saveFolderPath;
        }

        public static WatcherSettings Load()
        {
            var folderPath = GetSaveFolderPath();
            var fullFilePath = Path.Combine(folderPath, SaveFileName);
            if (File.Exists(fullFilePath))
            {
                try
                {
                    var reader = new XmlSerializer(typeof (WatcherSettings));
                    var file = new StreamReader(fullFilePath);
                    var settings = (WatcherSettings) reader.Deserialize(file);
                    return settings;
                }
                catch (Exception ex)
                {
                    //if file is corrupt or we can't continue for some reason, just abandon it..
                }
            }
            return new WatcherSettings();
        }

        public void Save()
        {
            var folderPath = GetSaveFolderPath();
            var fullFilePath = Path.Combine(folderPath, SaveFileName);
            var file = new StreamWriter(fullFilePath);
            
            var x = new XmlSerializer(this.GetType());
            x.Serialize(file, this);
            file.Close();
        }

        public WatcherSettings()
        {
            //defaults
            FolderToWatch = string.Empty;
            WatchedFileTypes = "*.js|*.css|*.cshtml|*.html|*.htm";
            Command = "";
        }
    }
}
