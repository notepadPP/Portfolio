
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Framework.Common
{
    public static class FileIO
    {
        public static string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(persistentDataPath))
                    persistentDataPath = Application.persistentDataPath;
                return persistentDataPath;
            }
        }
        private static string persistentDataPath = string.Empty;
        public static string DataPath
        {
            get
            {
                if (string.IsNullOrEmpty(dataPath))
                    dataPath = Application.dataPath;
                return dataPath;
            }
        }
        private static string dataPath = string.Empty;
        public static string StreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(streamingAssetsPath))
                    streamingAssetsPath = Application.streamingAssetsPath;
                return streamingAssetsPath;
            }
        }
        private static string streamingAssetsPath = string.Empty;

        public static bool LoadString(string path, out string str)
        {

            str = string.Empty;
            if(LoadBinary(path, out byte[] data) == true)
            {
                try
                {
                    str = Encoding.UTF8.GetString(data);
                    return true;
                }
                catch(Exception e)
                {
                    Debugger.LogException(e);
                }
            }
            return false;
        }
        public static bool LoadBinary(string path, out byte[] data)
        {
#if UNITY_IOS
            path = $"file:///{path}";
#endif// UNITY_IOS
            try
            {
                using (var www = UnityWebRequest.Get(path))
                {
                    var req = www.SendWebRequest();
                    while (!req.isDone) { }
                    Debugger.Log($"{req.isDone}, {path}");
                    if (www.isHttpError || www.isNetworkError)
                    {
                        Debugger.LogError($"{path} {www.isHttpError}, {www.isNetworkError}, {www.error}");
                        data = null;
                        return false;
                    }
                    data = www.downloadHandler.data;
                }
                if (data == null || data.Length < 1) { Debugger.LogError(nameof(data)); return false; }
            }
            catch (Exception e) { data = null; Debugger.LogException(e); return false; }
            return true;
        }

    }
}