using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Framework.Common
{
    public static class Debug
    {
        public static void SetDebugger(bool debugger) => UnityEngine.Debug.unityLogger.logEnabled = debugger;
        public static void Log(object message, [CallerMemberName] string memberName = "")  => DebugLog(LogType.Log, message, memberName);
        public static void Log(object message, UnityEngine.Object context, [CallerMemberName] string memberName = "") =>  DebugLog(LogType.Log, message, context, memberName);
        public static void LogError(object message, [CallerMemberName] string memberName = "") => DebugLog(LogType.Error, message, memberName);
        public static void LogError(object message, UnityEngine.Object context, [CallerMemberName] string memberName = "") => DebugLog(LogType.Error, message, context, memberName);
        public static void LogWarning(object message, [CallerMemberName] string memberName = "")=> DebugLog(LogType.Warning, message.ToString(), memberName);
        public static void LogWarning(object message, UnityEngine.Object context, [CallerMemberName] string memberName = "") => DebugLog(LogType.Warning, message, context, memberName);
        public static void LogException(System.Exception exception, [CallerMemberName] string memberName = "") => DebugLog(LogType.Exception, exception.ToString(), memberName);
        public static void Assert(bool condition, [CallerMemberName] string memberName = "") => DebugLog(LogType.Assert, condition, memberName);
        public static void LogAssertion(object message, [CallerMemberName] string memberName = "") => DebugLog(LogType.Assert, message, memberName);
        private static void DebugLog(LogType logType, object message, Object context, string memberName)
        {

            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            for (int i = 0; i < st.FrameCount; ++i)
            {
                System.Diagnostics.StackFrame sf = st.GetFrame(st.FrameCount - (i + 1));
                MethodBase method = sf.GetMethod();
                if (method == null) continue;
                string methodinfo = string.Empty;
                if (method != null)
                    UnityEngine.Debug.unityLogger.Log(logType, (object)$"{method.DeclaringType.Name}::{method.Name}({sf.GetFileLineNumber()}:{sf.GetFileColumnNumber()}) {message}", context);
                else
                    UnityEngine.Debug.unityLogger.Log(logType, message, context);
                break;
            }
        }
        private static void DebugLog(LogType logType, object message, string memberName)
        {
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
            for (int i = 0; i < st.FrameCount; ++i)
            {
                System.Diagnostics.StackFrame sf = st.GetFrame(st.FrameCount - (i + 1));
                MethodBase method = sf.GetMethod();
                if (method == null) continue;
                string methodinfo = string.Empty;
                if (method != null)
                    UnityEngine.Debug.unityLogger.Log(logType, $"{method.DeclaringType.Name}::{method.Name} {message}");
                else
                    UnityEngine.Debug.unityLogger.Log(logType, message);
                break;
            }
        }
    }
}