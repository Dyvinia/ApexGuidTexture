using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApexGuidTextureApp {
    internal class Program {
        static void Main(string[] args) {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string databaseName = "\\database.json";
            string folderName = "\\_images";

            bool missingFiles = false;
            if (!File.Exists(path + databaseName)) {
                Console.WriteLine("Missing json file " + path + databaseName);
                missingFiles = true;
            }
            if (!Directory.Exists(path + folderName)) {
                Console.WriteLine("Missing directory " + path + folderName);
                missingFiles = true;
            }
            if (missingFiles) {
                Console.WriteLine("");
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
                return;
            }

            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path + databaseName));

            var files = Directory.EnumerateFiles(path + folderName).ToList();
            int count = files.Count();

            for (int i = 0; i < count; i++) {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                if (!fileName.Contains("0x")) {
                    Console.WriteLine("Skipping " + fileName);
                }
                else {
                    string fileGUID = fileName.Substring(0, 2) + fileName.Substring(2).ToUpper();
                    string outValue;
                    bool hasValue = values.TryGetValue(fileGUID, out outValue);
                    if (hasValue) {
                        if (outValue.Contains("\"t")) {
                            outValue = outValue.Substring(outValue.LastIndexOf("]") + 1);
                            outValue = outValue.Substring(0, outValue.Length - 1);
                        }
                        outValue = Path.GetFileNameWithoutExtension(outValue);
                        File.Move(files[i], files[i].Replace(fileGUID.ToLower(), outValue));
                        Console.WriteLine($"Renaming {fileGUID.ToLower()}{Path.GetExtension(files[i])} to {outValue}{Path.GetExtension(files[i])}");
                    }
                    else {
                        Console.WriteLine("Missing Key " + fileGUID);
                    }
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
        }
    }
}
