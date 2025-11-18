using System.Collections.Generic;
using System;
using UnityEngine.Android;

namespace Framework.Common
{
    public static class Native
    {
        public enum PermissionState
        {
            Granted,
            Denied,
            DeniedAndDontAskAgain,
            Unknown,

        }
        public static void CheckPermissions(string[] permissions, Action<Dictionary<string, PermissionState>> actionIfPermissionEnd)
        {
            Dictionary<string, PermissionState> permissionDic = new Dictionary<string, PermissionState>();
#if UNITY_ANDROID
            // 권한 요청 응답에 따른 동작 콜백
            PermissionCallbacks pCallbacks = new PermissionCallbacks();
            pCallbacks.PermissionGranted += Granted =>
            {
                Debug.Log($"Permissoin : {Granted} Allow");
                permissionDic.Add(Granted, PermissionState.Granted);
                if (permissionDic.Count == permissions.Length)
                {
                    actionIfPermissionEnd?.Invoke(permissionDic);
                }
            };

            pCallbacks.PermissionDenied += Denied =>
            {
                Debug.Log($"Permissoin : {Denied} Deny");
                permissionDic.Add(Denied, PermissionState.Denied);
                if (permissionDic.Count == permissions.Length)
                {
                    actionIfPermissionEnd?.Invoke(permissionDic);
                }
            };

            pCallbacks.PermissionDeniedAndDontAskAgain += DeniedAndDontAskAgain =>
            {
                Debug.Log($"Permissoin : {DeniedAndDontAskAgain} Don't Ask");
                permissionDic.Add(DeniedAndDontAskAgain, PermissionState.DeniedAndDontAskAgain);
                if (permissionDic.Count == permissions.Length)
                {
                    actionIfPermissionEnd?.Invoke(permissionDic);
                }
            };

            // 권한 요청
            Permission.RequestUserPermissions(permissions, pCallbacks);

#elif UNITY_IOS
            foreach(var permission in permissions)
            {
                permissionDic.Add(permission, PermissionState.Unknown);
            }
            actionIfPermissionEnd?.Invoke(permissionDic);
#endif
        }
        public static void ShowToast(string strMessage)
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
		currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
		{
			toastClass.CallStatic<AndroidJavaObject>("makeText", currentActivity.Call<AndroidJavaObject>("getApplicationContext"), new AndroidJavaObject("java.lang.String", message), 0).Call("show");
		}));
#elif UNITY_IOS
            iOS_ShowToast(strMessage);
#endif
#endif
        }

#if !UNITY_EDITOR
#if UNITY_ANDROID
        private static AndroidJavaObject currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        private static AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
#elif UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void iOS_ShowToast(string toastText);
#endif
#endif
    }
}