using System;
using System.IO;
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
                var reader = new XmlSerializer(typeof(WatcherSettings));
                var file = new StreamReader(fullFilePath);
                try
                {
                    var settings = (WatcherSettings)reader.Deserialize(file);
                    return settings;
                }
                catch (Exception ex)
                {
                    //if file is corrupt or we can't continue for some reason, just abandon it..
                    //maybe in the future, we do something smarter. for now, we don't care.
                }
                finally
                {
                    file.Close();
                }
            }
            return new WatcherSettings();
        }

        public void Save()
        {
            var folderPath = GetSaveFolderPath();
            var fullFilePath = Path.Combine(folderPath, SaveFileName);
            var file = new StreamWriter(fullFilePath);

            var x = new XmlSerializer(GetType());
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
