using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Media.Imaging;
using static System.Net.WebRequestMethods;

namespace AddPrefix
{
    public class FileRename
    {
        public static DateTime GetDateTaken(string path)
        {
            var f = new FileInfo(path);
            using (FileStream fs = new FileStream(f.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    BitmapSource img = BitmapFrame.Create(fs);
                    BitmapMetadata md = (BitmapMetadata)img.Metadata;
                    if (md.DateTaken != null)
                        return DateTime.Parse(md.DateTaken);

                    return f.LastWriteTime;
                }
                catch
                {
                    return f.LastWriteTime;
                }
            }
        }

        public static string[] GetFilesFromPath(string path, string[] extensions, bool recursive)
        {
            return Directory.EnumerateFiles(path, "*.*", recursive? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Where(s => extensions.Any(ext => ext == Path.GetExtension(s).ToLower()))
                .ToArray();
        }

        public static string GetNewName(string filepath, string dateFormat)
        {
            var dateTaken = GetDateTaken(filepath);
            var fileName = filepath.Split('\\').Last();
            var filePrefix = dateTaken.ToString(dateFormat) + "-";
            var newFileName = fileName.StartsWith(filePrefix)? fileName : $"{filePrefix}{fileName}";
            
            return filepath.Replace(fileName, newFileName);
        }

        public static void SecureMove(string fileName, string newFilename)
        {
            try
            {
                System.IO.File.Move(fileName, newFilename);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{fileName} : { ex.Message}");
                Console.Write("Type R to retry:");
                var key = Console.ReadLine();
                if (key.ToLower().Trim() == "r")
                {
                    SecureMove(fileName, newFilename);
                }
            }
        }

        public static void Process(string path, string[] extensions, string dateFormat, bool recursive, bool previewMode)
        {
            if (string.IsNullOrEmpty(path))
            {
                Console.WriteLine("path parameter should not be null.");
                return;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Directory not found <{path}>.");
                return;
            }

            Console.WriteLine($"# Processing folder   : {path}");
            Console.WriteLine($"# Extensions          : {string.Join(",", extensions)}");
            Console.WriteLine($"# Format              : {dateFormat}");
            Console.WriteLine($"# Include sub folders : {(recursive ? "yes" : "no")}");
            Console.WriteLine($"# Preview Mode        : {(previewMode ? "yes" : "no")}");

            var files = GetFilesFromPath(path, extensions, recursive);
            Console.WriteLine($"# File(s) found       : {files.Length} ");

            Console.WriteLine("# Pre-processing...");
            var renamings = new Dictionary<string, string>();
            var errorCount = 0;
            foreach (var filename in files)
            {
                try
                {
                    var newFilename = GetNewName(filename, dateFormat);
                    if (filename == newFilename)
                    {
                        Console.WriteLine($"# skipped: {filename}");
                    }
                    else
                    {
                        Console.WriteLine($"mv {filename} {newFilename}");
                        renamings.Add(filename, newFilename);
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine($"ERROR : {e.Message}");
                    errorCount++;
                }
            }

            Console.WriteLine($"# {renamings.Count} file(s) to rename");

            if (errorCount > 0)
            {
                Console.WriteLine($"Error occured during pre-processing : {errorCount} error(s)");
            }
            else
            {
                if (!previewMode)
                {
                    Console.WriteLine("Processing...");
                    foreach (var renaming in renamings)
                        SecureMove(renaming.Key, renaming.Value);
                }
            }
        }
    }

    public static class ConsoleParametersExtensions
    {
        public static bool HasParameter(this string[] args, string key, string shortKey)
        {
            return args.Any(x => x == "--" + key) || args.Any(x => x == "-" + shortKey);
        }

        public static string GetParameterValue(this string[] args, string key, string shortKey, string defaultValue = null)
        {
            string result = defaultValue;

            if (HasParameter(args, key, shortKey))
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--" + key || args[i] == "-" + shortKey)
                    {
                        if (args.Length - 1 > i)
                        {
                            result = args[i + 1];
                        }

                        break;
                    }
                }
            }

            return result;
        }
    }

    public class TextResource
    {
        public static string ReadContent(Assembly assembly, string resourceName)
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

    class Program
    {
        private static string DefaultExtensions = 
            ".jpg,.jpeg,.png,.webp,.gif,.mp4,.avi,.mpg,.mpeg,.mov,.mkv";

        private static string DefaultDateFormat = 
            "yyyyMMdd-HHmmss";

        static void Main(string[] args)
        {
            if (args.HasParameter("path", "p"))
            {
                FileRename.Process(
                    args.GetParameterValue("path", "p"),
                    args.GetParameterValue("extensions", "e", DefaultExtensions).Split(','),
                    args.GetParameterValue("format", "f", DefaultDateFormat),
                    args.HasParameter("recursive", "r"),
                    args.HasParameter("preview", "P"));
            }
            else if (args.HasParameter("version", "v"))
            {
                Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
            else
            {
                Console.WriteLine(TextResource.ReadContent(Assembly.GetExecutingAssembly(), "AddPrefix.Manual.txt"));
            }
        }
    }
}
