using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using AssetStudio;

namespace AssetExporter
{
    class Program
    {
        static AssetsManager assetsManager = new AssetsManager();
        static void Main(string[] args)
        {
            switch (args.Length)
            {
                case 2:
                    if (!File.Exists(args[0]))
                        goto default;

                    string AssetFileDir = Path.GetDirectoryName(args[0]);
                    string OutputDir = Path.Combine(AssetFileDir, "AssetsExport");
                    ExportFiles(args[0], args[1], OutputDir);
                    break;

                case 3:
                    if (!File.Exists(args[0]))
                        goto default;
                    ExportFiles(args[0], args[1], args[2]);
                    break;

                default:
                    Console.WriteLine(@"Usage: AssetExporter ""AssetBundlePath"" ""assetname1;assetname2;..."" [""OutputDir""] ");
                    break;
            }
        }

        static void ExportFiles(string _AssetBundlePath, string _ExportFileList, string _OutputDir)
        {
            if (File.Exists(_AssetBundlePath))
            {

                Console.WriteLine("Loading assetbundle: " + '"' + _AssetBundlePath + '"');
                assetsManager.LoadFiles(_AssetBundlePath);

                string pat = @"([^;]+)";
                MatchCollection matches = Regex.Matches(_ExportFileList, pat);
                Match[] arr = new Match[matches.Count];
                matches.CopyTo(arr, 0);
                HashSet<string> export_name_set = new HashSet<string>();
                foreach (var item in arr)
                {
                    export_name_set.Add(item.Value);
                }
                Console.WriteLine("Export files: ");
                foreach (var serifile in assetsManager.assetsFileList)
                {
                    foreach (var assetobj in serifile.Objects)
                    {
                        NamedObject n_obj = assetobj as NamedObject;
                        if (n_obj != null)
                        {
                            if (export_name_set.Contains(n_obj.m_Name))
                            {
                                Console.WriteLine(n_obj.m_Name);
                                ExportAsset(n_obj, _OutputDir);
                            }
                        }
                    }
                }
            }
        }

        static bool ExportAsset(NamedObject obj, string exportPath)
        {
            switch (obj.type)
            {
                case ClassIDType.TextAsset:
                    return ExportTextAsset(obj, exportPath);
                default:
                    return ExportRawFile(obj, exportPath);
            }

        }

        static bool ExportTextAsset(NamedObject obj, string exportPath)
        {
            var m_TextAsset = (TextAsset)(obj);
            string extension = ".txt";
            var exportFullName = Path.Combine(exportPath, obj.m_Name + extension);
            Directory.CreateDirectory(Path.GetDirectoryName(exportFullName));
            File.WriteAllBytes(exportFullName, m_TextAsset.m_Script);
            return true;
        }

        static bool ExportRawFile(NamedObject obj, string exportPath)
        {
            var exportFullName = Path.Combine(exportPath, obj.m_Name + ".dat");
            Directory.CreateDirectory(Path.GetDirectoryName(exportFullName));
            File.WriteAllBytes(exportFullName, obj.GetRawData());
            return true;
        }
    }
}
