using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ApexGuidTexture {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
            ProcessGUID();
        }

        private void ProcessGUID() {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            string json = File.ReadAllText(path + "\\output.json");
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            var files = Directory.EnumerateFiles(path + "\\_images").ToList();
            int count = files.Count();

            for (int i = 0; i < count; i++) {
                string fileguid = Path.GetFileNameWithoutExtension(files[i]);
                fileguid = fileguid.Substring(0, 2) + fileguid.Substring(2).ToUpper();
                string outValue;
                bool hasValue = values.TryGetValue(fileguid, out outValue);
                if (hasValue) {
                    if (outValue.Contains("\"t")) {
                        outValue = outValue.Substring(outValue.LastIndexOf("]") + 1);
                        outValue = outValue.Substring(0, outValue.Length - 1);
                    }
                    outValue = Path.GetFileNameWithoutExtension(outValue);
                    File.Move(files[i], files[i].Replace(fileguid.ToLower(), outValue));
                }
                else {
                    //Console.WriteLine("Key not present");
                }
            }
        }

    }
}
