using System;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
namespace Framework.Common.IO
{

    public static partial class File
    {
        public static bool LoadJson(string path, out string json)
        {
            json = string.Empty;
            if (LoadBinary(path, out byte[] bytes) == false)
                return false;
            json = Encoding.UTF8.GetString(bytes);
            return true;
        }
        public static bool LoadBinary(string path, out byte[] data)
        {
#if UNITY_IOS
            path = $"file:///{path}";
#endif// UNITY_IOS

            try
            {
                using (var request = UnityWebRequest.Get(path))
                {
                    var req = request.SendWebRequest();
                    while (!req.isDone) { }
                    Debug.Log($"{req.isDone}, {path}");

                    if (request.result == UnityWebRequest.Result.ProtocolError || request.result == UnityWebRequest.Result.ConnectionError)
                    {
                        Debug.LogError($"{nameof(LoadBinary)} : {path} {request.result}, {request.error}");
                        data = null;
                        return false;
                    }

                    data = request.downloadHandler.data;
                }

                if (data == null || data.Length < 1) { Debug.LogError(nameof(data)); return false; }
            }
            catch (Exception e) { data = null; Debug.LogException(e); return false; }

            return true;
        }



        public static IEnumerator LoadSound(string strPath, Action<AudioClip> callback)
        {
            AudioType audioType = GetAudioType(strPath);
            string strUri = "file://" + strPath;
            UriBuilder builder = new UriBuilder(strUri) { Scheme = Uri.UriSchemeFile };

            using (UnityWebRequest pkHttp = UnityWebRequestMultimedia.GetAudioClip(builder.ToString(), audioType))
            {
                yield return pkHttp.SendWebRequest();

                if (pkHttp.result == UnityWebRequest.Result.ConnectionError || pkHttp.result == UnityWebRequest.Result.ProtocolError || pkHttp.result == UnityWebRequest.Result.DataProcessingError)
                {
                    Debug.LogWarning("@@@@@  - Load Sound Error");
                }
                else
                {
                    AudioClip pkClip = null;

                    pkClip = DownloadHandlerAudioClip.GetContent(pkHttp);

                    Debug.LogWarning($"@@@@@  - Load Sound Success pkClip is null : {pkClip == null}");

                    callback?.Invoke(pkClip);
                    //SuperManager.instance.idtSoundManager.AddSound(strPath, pkClip);

                    Debug.LogWarning("@@@@@  - Load Sound Success" + strPath);
                }
            }
        }
        public static async Task<AudioClip> LoadSoundTask(string strPath)
        {
            AudioType audioType = GetAudioType(strPath);
            string strUri = "file://" + strPath;
            UriBuilder builder = new UriBuilder(strUri) { Scheme = Uri.UriSchemeFile };
            AudioClip pkClip = null;

            using (UnityWebRequest pkHttp = UnityWebRequestMultimedia.GetAudioClip(builder.ToString(), audioType))
            {
                UnityWebRequestAsyncOperation pkOperation = pkHttp.SendWebRequest();

                try
                {
                    while (!pkOperation.isDone)
                    {
                        await Task.Delay(5);
                    }

                    if (pkHttp.result == UnityWebRequest.Result.ConnectionError || pkHttp.result == UnityWebRequest.Result.ProtocolError || pkHttp.result == UnityWebRequest.Result.DataProcessingError)
                    {
                        Debug.LogWarning("@@@@@  - LoadSoundMp3ViaMultimedia Load Sound Error");
                    }
                    else
                    {
                        pkClip = DownloadHandlerAudioClip.GetContent(pkHttp);
                    }
                }
                catch (Exception pkEx)
                {
                    Debug.Log($"{pkEx.Message}, {pkEx.StackTrace}");
                }
            }

            return pkClip;
        }
        public static Texture2D LoadImage(string path, bool markTextureNonReadable = true, bool generateMipmaps = true, bool linearColorSpace = false)
        {
            string extension = System.IO.Path.GetExtension(path).ToLowerInvariant();
            TextureFormat format = (extension == ".jpg" || extension == ".jpeg") ? TextureFormat.RGB24 : TextureFormat.RGBA32;

            Texture2D texture = new Texture2D(2, 2, format, generateMipmaps, linearColorSpace);
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("path is null or empty");
                    return null;
                }
                if (LoadBinary(path, out byte[] bytes) == false)
                {
                    Debug.LogError("LoadBinary is false");
                    return null;
                }
                if (texture.LoadImage(bytes, markTextureNonReadable) == false)
                {
                    Debug.LogWarning("Couldn't load image at path: " + path);
                    UnityEngine.Object.DestroyImmediate(texture);
                    return null;
                }


            }
            catch (Exception e)
            {
                Debug.LogException(e);
                UnityEngine.Object.DestroyImmediate(texture);
                return null;
            }
            return texture;
        }
    }

}