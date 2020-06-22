using System.ComponentModel;

namespace ScriptRunnerCS.Models
{
    public class ScriptModel : INotifyPropertyChanged
    {   
        public bool IsSet { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
