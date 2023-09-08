namespace Framework.Common.Template
{
    public abstract class Singleton<T> : SingletonBase where T : SingletonBase, new()
    {
        private static object lockObject = new object();
        private static T instance = default;

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
                        instance = SingletonManager.Instance.AddSingleton<T>();
                    return instance;
                }
            }
        }

    }

}