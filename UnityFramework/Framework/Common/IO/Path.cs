using UnityEngine;
namespace Framework.Common.IO
{

    public static class Path
    {
        private static string persistentDataPath = string.Empty;
        public static string PersistentDataPath
        {
            get
            {
                if (string.IsNullOrEmpty(persistentDataPath))
                    persistentDataPath = UnityEngine.Application.persistentDataPath;
                return persistentDataPath;
            }
        }

        private static string dataPath = string.Empty;
        public static string DataPath
        {
            get
            {
                if (string.IsNullOrEmpty(dataPath))
                    dataPath = UnityEngine.Application.dataPath;
                return dataPath;
            }
        }

        private static string streamingAssetsPath = string.Empty;
        public static string StreamingAssetsPath
        {
            get
            {
                if (string.IsNullOrEmpty(streamingAssetsPath))
                    streamingAssetsPath = UnityEngine.Application.streamingAssetsPath;
                return streamingAssetsPath;
            }
        }
        public static string temporaryCachePath = string.Empty;
        public static string TemporaryCachePath
        {
            get
            {
                if (string.IsNullOrEmpty(temporaryCachePath))
                    temporaryCachePath = UnityEngine.Application.temporaryCachePath;
                return temporaryCachePath;
            }
        }
    }
        
}