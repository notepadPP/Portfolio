namespace Framework.Common.Template
{
    public abstract class MonoSingleton<T> : Element where T : Element
    {
        private static DontDestroyOnLoad DontDestroyOnLoadObj = null;
        private static object lockObject = new object();
        private static T instance = null;

        public static T Instance
        {
            // 쓰래드 안전화 - Thread-Safe
            get
            {
                // 한번에 한 스래드만 lock블럭 실행
                lock (lockObject)
                {
                    // instance가 NULL일때 새로 생성한다.
                    if (instance == null)
                    {
                        if(DontDestroyOnLoadObj == null)
                            DontDestroyOnLoadObj = FindObjectOfType<DontDestroyOnLoad>();
                        if (DontDestroyOnLoadObj == null)
                        {
                            DontDestroyOnLoadObj = new UnityEngine.GameObject("DontDestroyOnLoad").AddComponent<DontDestroyOnLoad>();
                        }
                        instance = new UnityEngine.GameObject(typeof(T).ToString()).AddComponent<T>();
                        instance.transform.parent = DontDestroyOnLoadObj.transform;
                        instance.Initialize();
                    }
                    return instance;
                }
            }
        }
    }

}