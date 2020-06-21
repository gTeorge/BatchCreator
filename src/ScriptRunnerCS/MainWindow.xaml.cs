using Newtonsoft.Json;
using ScriptRunnerCS.Helpers;
using ScriptRunnerCS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ScriptRunnerCS
{
    public partial class MainWindow : Window
    {
        FileSystemHelper FSHelper = new FileSystemHelper();
        private ConfigModel _config { get; set; }
        private bool isAllSet = false;

        public MainWindow()
        {
            _config = JsonConvert.DeserializeObject<ConfigModel>(File.ReadAllText("config.json"));
            _config.Scripts = new List<ScriptModel>() {
                new ScriptModel
                {
                    IsSet = false,
                    IsVisible = true,
                    Name = "Select Folder or open saved config file."
                }
            };

            InitializeComponent();

            DataContext = _config;
            SetView();
            ExtensionsSelectBox.SelectedIndex = 0;
        }

        private void SetView()
        {
            listView.ItemsSource = _config.Scripts;
            (CollectionViewSource.GetDefaultView(listView.ItemsSource) as CollectionView).Filter = VisibilityFilter;
        }

        private bool VisibilityFilter(object item) => (item as ScriptModel).IsVisible;

        private void RefreshView() => CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = FSHelper.LoadFile();
            if (!string.IsNullOrEmpty(fileContent))
            {
                _config = JsonConvert.DeserializeObject<ConfigModel>(fileContent);
                SetView();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = JsonConvert.SerializeObject(_config, Formatting.Indented);
            FSHelper.SaveFile(fileContent, FSHelper.extensions["json"]);
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var files = FSHelper.GetFileNames(FSHelper.GetFolderName()).ToList<string>();

            _config.Scripts = new List<ScriptModel>();
            files.ForEach(x => _config.Scripts.Add(new ScriptModel { IsSet = true, Name = x, IsVisible = true }));
            
            SetView();
        }

        private void CommandTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _config.Command = CommandTextBox.Text;
        }

        private void DelayTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                _config.DelaySec = Int32.Parse(DelayTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show
                (
                    "Input has wrong format, please enter integer.",
                    "Error - wrong format!",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void CreateBatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_config.Scripts is null)
            {
                MessageBox.Show
                (
                    "List is empty",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return;
            }

            var sb = new StringBuilder();
            sb.Append("echo Starting & ");

            foreach (var file in _config.Scripts)
            {
                if (file.IsSet)
                {
                    sb.Append(_config.Command);
                    sb.Append(" ");
                    sb.Append(file.Name);
                    sb.Append(" & timeout ");
                    sb.Append(_config.DelaySec);
                    sb.Append(" & ");
                }
            }
            sb.Append("echo finnished");

            FSHelper.SaveFile(sb.ToString(), FSHelper.extensions["bat"]);
        }

        private void GridViewColumnHeaderSet_Click(object sender, RoutedEventArgs e)
        {
            isAllSet = !isAllSet;
            _config.Scripts.ForEach(x => x.IsSet = isAllSet);

            RefreshView();
        }

        private void ExtensionsSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_config.Scripts is null)
            {
                return;
            }

            var filter = (sender as ComboBox).SelectedItem as string;

            switch (filter)
            {
                case null:
                    break;

                case "all":
                    _config.Scripts.ForEach(x =>
                   {
                       x.IsVisible = true;
                   });
                    break;

                default:
                    _config.Scripts.ForEach(x =>
                    {
                        x.IsVisible = x.Name.EndsWith(filter) ? true : false;
                        if (!x.IsVisible)
                        {
                            x.IsSet = false;
                        }
                    });
                    break;
            }

            RefreshView();
        }
    }
}
