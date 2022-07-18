using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaneXmlWriter
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var dataGridType = typeof(DataGrid); // HACK
            InitializeComponent();

            if(!String.IsNullOrEmpty(ConfigData.Instance.LastFilePath))
            {
                if(File.Exists(ConfigData.Instance.LastFilePath))
                {
                    var vm = new XmlFileAccess();
                    vm.Load(ConfigData.Instance.LastFilePath);
                    DataContext = vm;
                }
            }
        }
        private async void SaveClicked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as XmlFileAccess;
            if(XmlFileAccess.Xml != null)
                await vm.Save();
        }

        private async void LoadClicked(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as XmlFileAccess;
            var dlg = new OpenFileDialog();
            dlg.Filters.Add(new FileDialogFilter() { Name = "XML Files", Extensions = { "xml" } });
            var result = await dlg.ShowAsync(this);
            if (result != null)
            {
                string[] fileNames = result;
                var file = fileNames.First();
                var xml = new XmlFileAccess();
                xml.Load(file);
                this.DataContext = xml;
                ConfigData.Instance.LastFilePath = file;
                ConfigData.Save();
            }
        }

        private void CancelClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
