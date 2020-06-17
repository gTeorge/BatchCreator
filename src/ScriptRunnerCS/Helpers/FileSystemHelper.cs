using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;

namespace ScriptRunnerCS.Helpers
{
    public class FileSystemHelper
    {
        public readonly string JSON = "json files (*.json)|*.json";
        public readonly string Bat = "batch files (*.bat)|*.bat";

        public string GetFolderName()
        {
            var fbd = new FolderBrowserDialog()
            {
                UseDescriptionForTitle = true,
                Description = "Select folder with the scripts"
            };

            var result = fbd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(fbd.SelectedPath))
            {
                return fbd.SelectedPath;
            }
            else
            {
                return string.Empty;
            }

        }

        public string[] GetFileNames(string folder)
        {
            if (!string.IsNullOrEmpty(folder))
            {
                return Directory.GetFiles(folder);
            }
            else
            {
                return new string[] { };
            }
        }

        public string LoadFile()
        {
            var fileContent = string.Empty;
            var ofd = new OpenFileDialog()
            {
                InitialDirectory = "c:\\",
                Filter = "json files (*.json)|*.json",
                FilterIndex = 2,
                RestoreDirectory = true,
                Title = "Open config file"
            };

            var result = ofd.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrEmpty(ofd.FileName))
            {
                var fStream = ofd.OpenFile();
                using (var sReader = new StreamReader(fStream))
                {
                    fileContent = sReader.ReadToEnd();
                }

                return fileContent;
            }
            else
            {
                return string.Empty;
            }
        }

        public void SaveFile(string fileContent, string filter)
        {
            var sfd = new SaveFileDialog()
            {
                InitialDirectory = "c:\\",
                Filter = filter,
                Title = "Save config file",
            };
            var result = sfd.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrEmpty(sfd.FileName))
            {
                var fStream = sfd.OpenFile();
                using (var sWriter = new StreamWriter(fStream))
                {
                    sWriter.Write(fileContent);
                }
            }
        }
    }
}
