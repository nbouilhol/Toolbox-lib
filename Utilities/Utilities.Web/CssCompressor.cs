using System;
using System.IO;

namespace Mvc.Helper.UI
{
    public class CssCompressor
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
            Console.WriteLine("Compressing all .css files within: " + root);
            foreach (string file in Directory.GetFiles(root, "*.css"))
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
            var cssCompressor = new Yahoo.Yui.Compressor.CssCompressor();
            using (var sw = new StreamWriter(outputDirectory + name.Replace(".css", ".min.css")))
            using (var sr = new StreamReader(file))
                sw.Write(cssCompressor.Compress(sr.ReadToEnd()));
        }
    }
}