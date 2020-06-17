using ScriptRunnerCS.Helpers;
using ScriptRunnerCS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Data;

namespace ScriptRunnerCS
{
    public partial class MainWindow : Window
    {
        FileSystemHelper FSHelper = new FileSystemHelper();
        private ConfigModel _config { get; set; }
        private bool _allSet = false;

        public MainWindow()
        {
            var test = JsonConvert.DeserializeObject(File.ReadAllText("config.json"));
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
            listView.ItemsSource = _config.Scripts;

            ExtensionsSelectBox.SelectedIndex = 0; // sets filter to fist file extension as filter

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = VisibilityFilter;
        }

        private bool VisibilityFilter(object item)
        {
            if(item is null)
            {
                return false;
            }
            return (item as ScriptModel).IsVisible;
        }
        private void RefreshView() 
            => CollectionViewSource.GetDefaultView(listView.ItemsSource).Refresh();

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = FSHelper.LoadFile();
            if (!string.IsNullOrEmpty(fileContent))
            {
                _config = JsonConvert.DeserializeObject<ConfigModel>(fileContent);
                listView.ItemsSource = _config.Scripts;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
                view.Filter = VisibilityFilter;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var fileContent = JsonConvert.SerializeObject(_config, Formatting.Indented);
            FSHelper.SaveFile(fileContent, FSHelper.JSON);
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var folder = FSHelper.GetFolderName();
            var files = FSHelper.GetFileNames(folder);

            _config.Scripts = new List<ScriptModel>();


            foreach (var file in files)
            {
                _config.Scripts.Add(new ScriptModel { IsSet = true, Name = file, IsVisible = true });
            }

            listView.ItemsSource = _config.Scripts;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            view.Filter = VisibilityFilter;
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
                MessageBox.Show("Input has wrong format, please enter integer.", "Error - wrong format!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CreateBatchButton_Click(object sender, RoutedEventArgs e)
        {
            if (_config.Scripts == null)
            {
                MessageBox.Show("List is empty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            FSHelper.SaveFile(sb.ToString(),FSHelper.Bat);
        }

        private void GridViewColumnHeaderSet_Click(object sender, RoutedEventArgs e)
        {
            var state = !_allSet;
            _allSet = state;

            foreach (var file in _config.Scripts)
            {
                file.IsSet = state;
            }

            RefreshView();
        }

        private void ExtensionsSelectBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_config.Scripts is null)
            { 
                return;
            }

            var comboBox = sender as ComboBox;
            var filter = comboBox.SelectedItem as string;

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
