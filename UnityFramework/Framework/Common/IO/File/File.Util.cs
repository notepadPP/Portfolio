using UnityEngine;
namespace Framework.Common.IO
{

    public static partial class File
    {
        private static AudioType GetAudioType(string strPath)
        {
            
            string extension = System.IO.Path.GetExtension(strPath).ToLowerInvariant();
            switch (extension)
            {
                case ".mp3": return AudioType.MPEG;
                case ".ogg": return AudioType.OGGVORBIS;
                case ".wav": return AudioType.WAV;
                case ".mod": return AudioType.MOD;
                case ".it": return AudioType.IT;
                case ".s3m": return AudioType.S3M;
                case ".xm": return AudioType.XM;
                case ".aiff":
                case ".aif":
                    return AudioType.AIFF;

                default: return AudioType.UNKNOWN;
            }
        }

        public static void ExistsCreateDirectory(string path)
        {
            var dirPath = System.IO.Path.GetDirectoryName(path);
            if (!System.IO.Directory.Exists(dirPath))
                System.IO.Directory.CreateDirectory(dirPath);
        }
    }
}