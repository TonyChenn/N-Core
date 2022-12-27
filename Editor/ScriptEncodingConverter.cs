using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
public class ScriptEncodingConverter
{

    public static bool DetectFileEncoding(string file, string name)
    {
        var encodingVerifier = Encoding.GetEncoding(name, new EncoderExceptionFallback(), new DecoderExceptionFallback());
        using (var reader = new StreamReader(file, encodingVerifier, true, 1024))
        {
            try
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                }
                return reader.CurrentEncoding.BodyName == name;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    [MenuItem("Assets/转为UTF8")]
    static void Converter2UTF8()
    {
        MonoScript[] msarr = Selection.GetFiltered<MonoScript>(SelectionMode.DeepAssets);
        if (null != msarr && msarr.Length > 0)
        {
            List<string> files = new List<string>();
            foreach (var item in msarr)
            {
                string path = AssetDatabase.GetAssetPath(item);
                if (DetectFileEncoding(path, "gb2312"))
                {
                    var text = File.ReadAllText(path, Encoding.GetEncoding(936));
                    File.WriteAllText(path, text, Encoding.UTF8);
                    files.Add(path);
                    AssetDatabase.ImportAsset(path);
                }
            }
            var info = files.Count > 0 ? $"处理文件 {files.Count} 个，更多 ↓ \n{string.Join("\n", files)}" : "没有发现编码问题！";
        }
    }
}

//class SharpScriptImporter : AssetPostprocessor
//{
//    //所有的资源的导入，删除，移动，都会调用此方法，注意，这个方法是static的
//    public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//    {
//        //仅对有修改的脚本进行处理
//        var scripts = importedAsset.Where(v => v.EndsWith(".cs"));
//        if (scripts.Count() == 0) return;
//        List<string> files = new List<string>();
//        foreach (var path in scripts)
//        {
//            // 如果是 gb2312 编码就改成 utf-8
//            if (ScriptEncodingConverter.DetectFileEncoding(path, "gb2312"))
//            {
//                var text = File.ReadAllText(path, Encoding.GetEncoding(936));
//                File.WriteAllText(path, text, Encoding.UTF8);
//                files.Add(path);
//            }
//        }

//        AssetDatabase.Refresh();
//    }
//}