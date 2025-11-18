using UnityEngine;

namespace Framework.Common
{
    public static partial class Extension
    {
        public static void SetActive(this Element behaviour, bool active) => behaviour?.gameObject.SetActive(active);
        public static void SetActive(this MonoBehaviour behaviour, bool active) => behaviour?.gameObject.SetActive(active);
        public static void SetActive(this Component component, bool active) => component?.gameObject.SetActive(active);
        public static void Destroy(this Component behaviour) => behaviour?.gameObject.Destroy();
        public static void Destroy(this UnityEngine.GameObject go)
        {
            if (go == null) return;
            Destroy(go);
        }
        public static UnityEngine.GameObject Instantiate(this UnityEngine.GameObject go, RectTransform rectTransform)
        {
            if (go == null) return null;
            UnityEngine.GameObject output = UnityEngine.GameObject.Instantiate(go, rectTransform);

            output.transform.localPosition = Vector3.zero;
            output.transform.localScale = Vector3.one;
            output.transform.localRotation = Quaternion.identity;
            RectTransform outputRectTransform = output.transform as RectTransform;
            if (outputRectTransform != null)
            {
                outputRectTransform.anchoredPosition3D = Vector3.zero;
            }
            return output;
        }

    }
}