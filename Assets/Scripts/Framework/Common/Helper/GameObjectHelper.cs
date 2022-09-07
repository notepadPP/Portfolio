using UnityEngine;
using UnityEngine.UI;

namespace Framework.Common.Helper
{
    public static partial class GameObjectHelper
    {
        public static void Destory(this GameObject obj)
        {
            if (Application.isPlaying) GameObject.Destroy(obj);
            else GameObject.DestroyImmediate(obj);
        }
        public static void SetActive(this Camera camera, bool active)
        {
            if (camera == null) return;
            camera.gameObject.SetActive(active);
        }
        public static void SetActive(this Canvas canvas, bool active)
        {
            if (canvas == null) return;
            canvas.gameObject.SetActive(active);
        }
        public static void SetActive(this Transform transform, bool active)
        {
            if (transform == null) return;
            transform.gameObject.SetActive(active);
        }
        public static void SetActive(this Button button, bool active)
        {
            if (button == null) return;
            button.gameObject.SetActive(active);
        }
    }
}