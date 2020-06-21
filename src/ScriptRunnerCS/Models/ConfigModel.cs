using System.Collections.Generic;
using System.ComponentModel;

namespace ScriptRunnerCS.Models
{
    public class ConfigModel : INotifyPropertyChanged
    {
        public string Command { get; set; }
        public int DelaySec { get; set; }
        public List<ScriptModel> Scripts { get; set; } = new List<ScriptModel>();
        public List<string> FileExtensions { get; set; } = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
