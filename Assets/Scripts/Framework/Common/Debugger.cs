using System.Runtime.CompilerServices;
using UnityEngine;

namespace Framework.Common
{
    public static class Debugger
    {
        public static bool Enable { get { return Debug.unityLogger.logEnabled; } set { Debug.unityLogger.logEnabled = value; } }
        public static void Log(object message, [CallerMemberName] string memberName = "") => DebugLog(LogType.Log, message, memberName);
        public static void LogError(object message, [CallerMemberName] string memberName = "") => DebugLog(LogType.Log, message, memberName);
        public static void LogException(System.Exception exception, [CallerMemberName] string memberName = "") => DebugLog(LogType.Log, exception, memberName);
        public static void LogAssertion<T>(T message, [CallerMemberName] string memberName = "")
        {
            if (message == null)
                DebugLog(LogType.Assert, $"<{typeof(T)}> {message} is null", memberName);
            else
                DebugLog(LogType.Assert, message, memberName);
        }
        private static void DebugLog(LogType logType, object message, string memberName) => Debug.unityLogger.Log(logType, $"{memberName} {message}");
    }

}
