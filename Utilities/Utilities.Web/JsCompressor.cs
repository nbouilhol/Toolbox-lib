using System;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace Mvc.Helper.UI
{
    public class JsCompressor
    {
        public static void Compress(string input, string output)
        {
            if (!Directory.Exists(output))
                throw new ArgumentException(string.Format("The output directory '{0}' doesn’t exist", output));
            if (Directory.Exists(input))
                CompressDirectory(input, output);
            else if (File.Exists(input))
                CompressFile(input, output);
            else
                throw new ArgumentException(string.Format("{0} isn’t a known directory or file", input));
        }

        public static void CompressDirectory(string root, string outputDirectory)
        {
            Console.WriteLine("Compressing all .js files within: " + root);
            foreach (string file in Directory.GetFiles(root, "*.js"))
                CompressFile(file, outputDirectory);
            foreach (string directory in Directory.GetDirectories(root))
            {
                if ((new DirectoryInfo(directory).Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                {
                    string newOuputDirectory = outputDirectory + directory.Substring(root.Length) + "\\";
                    CompressDirectory(directory, newOuputDirectory);
                }
            }
        }

        public static void CompressFile(string file, string outputDirectory)
        {
            string name = Path.GetFileName(file);
            if (name.Contains(".min."))
                return;
            Console.WriteLine("Compressing file: " + name);
            var minifier = new Minifier();
            using (var sw = new StreamWriter(outputDirectory + name.Replace(".js", ".min.js")))
            using (var sr = new StreamReader(file))
                sw.Write(minifier.MinifyJavaScript(sr.ReadToEnd()));
        }
    }
}