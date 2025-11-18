using System;
using System.Text;
using UnityEngine;
namespace Framework.Common.IO
{

    public static partial class File
    {

        public static bool SaveJson(string path, object data)
        {
            try
            {
                if (data == null) { Debug.LogError("data is null"); return false; }
                var json = JsonUtility.ToJson(data);
                if (string.IsNullOrEmpty(json)) { Debug.LogError("not convert json data"); return false; }

                ExistsCreateDirectory(path);
                SaveBinary(path, Encoding.UTF8.GetBytes(json));
            }
            catch (Exception e) { Debug.LogException(e); return false; }

            Debug.Log($"Save success, path {path}");
            return true;
        }
        
        public static bool SaveBinary(string path, byte[] data)
        {
            try
            {
                ExistsCreateDirectory(path);
                using (var fStream = System.IO.File.Open(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    fStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception e) { Debug.LogException(e); return false; }
            return true;
        }
    }
}