using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApexGuidTexturePreprocess {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public string inputDir;

        public MainWindow() {
            InitializeComponent();
            txtb_inputpath.Text = inputDir;
        }


        private void ProcessRSON() {
            Mouse.OverrideCursor = Cursors.Wait;

            string outputFile = inputDir + "\\Output\\database.json";
            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            Dictionary<string, string> guidDict = new Dictionary<string, string>();

            var files = Directory.EnumerateFiles(inputDir, "*.rson").ToList();
            int count = files.Count();

            for (int i = 0; i < count; i++) {
                TextStatus.Dispatcher.Invoke(() => TextStatus.Text = "Processing " + Path.GetFileName(files[i]), DispatcherPriority.Background);
                TextNumber.Dispatcher.Invoke(() => TextNumber.Text = $"{i}/{count}", DispatcherPriority.Background);

                using (StreamReader openfile = File.OpenText(files[i])) {
                    string fileString = openfile.ReadToEnd();
                    fileString = fileString.Substring(fileString.IndexOf("memoryInfo"));

                    List<string> listGuid = new List<string>();
                    List<string> listPath = new List<string>();

                    foreach (Match match in Regex.Matches(fileString, "(?<=guid: )(.*?)(?=\\r)|(?<=pak: )(.*?)(?=\\r)")) {
                        if (match.Groups[1].Value != "") listGuid.Add(match.Groups[1].Value);
                        if (match.Groups[2].Value != "") listPath.Add(match.Groups[2].Value);

                    }

                    for (int listIndex = 0; listIndex < listGuid.Count; listIndex++) {
                        if (guidDict.ContainsKey(listGuid[listIndex].ToString())) {

                        }
                        else {
                            guidDict.Add(listGuid[listIndex].ToString(), listPath[listIndex].ToString());
                        }
                    }
                }
            }

            TextNumber.Text = "";
            TextStatus.Text = "Writing Output File";

            File.WriteAllText(outputFile, JsonConvert.SerializeObject(guidDict, Formatting.Indented));

            TextStatus.Text = "";
            Mouse.OverrideCursor = null;

            MessageBox.Show("Finished", "Finished Generating", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void btn_inputselect_Click(object sender, RoutedEventArgs e) {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.Title = "Select Input Folder";
            dialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) {
                inputDir = dialog.FileName;
                txtb_inputpath.Text = inputDir;
            }
        }

        private void btn_inputopen_Click(object sender, RoutedEventArgs e) {
            Process.Start(inputDir);
        }

        private void btn_run_Click(object sender, RoutedEventArgs e) {
            if (inputDir == null) {
                string title = "Empty";
                string message = "Empty";
                MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            ProcessRSON();
        }
    }
}
