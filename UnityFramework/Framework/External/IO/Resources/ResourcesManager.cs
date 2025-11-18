using Framework.Common.IO.Addressables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Common.IO.Resources
{
    public class ResourcesManager : Template.Singleton<ResourcesManager>
    {
        Dictionary<string, ResourceData> resources = null;
        public override void OnInitialize()
        {
            resources = new Dictionary<string, ResourceData>();
        }
        public override void DoDestroy()
        {
            foreach (string key in resources.Keys)
                RemoveResource(key);
            resources.Clear();
        }


        // Function to get preloaded resources
        public T GetResources<T>(string path) where T : UnityEngine.Object
        {
            if (resources.TryGetValue(path, out ResourceData t) == false)
                return null;
            return t.Object as T;
        }
        // Resources load function
        public void LoadResource<T>(string key, Action<T> onComplete = null) where T : UnityEngine.Object
        {
            try
            {
                if (resources.TryGetValue(key, out ResourceData t))
                {
                    onComplete?.Invoke(t.Object as T);
                    return;
                }
                T obj = UnityEngine.Resources.Load<T>(key);
                if (obj != null)
                {
                    AddResources(key, obj, false);
                    onComplete?.Invoke(t.Object as T);
                    return;
                }
                AddressablesManager.Instance.Load<T>(key, (obj) =>
                {
                    AddResources(key, obj, true);
                    onComplete?.Invoke(obj);
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                onComplete?.Invoke(null);
            }
        }
        public void LoadResource(List<string> keys, Action<List<UnityEngine.Object>> onComplete = null)
        {
            try
            {
                int count = 0;
                List<UnityEngine.Object> list = new List<UnityEngine.Object>();
                foreach (string key in keys)
                {
                    if (resources.TryGetValue(key, out ResourceData t))
                    {
                        count++;
                        list.Add(t.Object as UnityEngine.GameObject);
                        if (count == keys.Count)
                            onComplete?.Invoke(list);
                        continue;
                    }
                    UnityEngine.Object obj = UnityEngine.Resources.Load(key);
                    if (obj != null)
                    {
                        AddResources(key, obj, false);
                        count++;
                        list.Add(obj as UnityEngine.GameObject);
                        if (count == keys.Count)
                            onComplete?.Invoke(list);
                        continue;
                    }
                    AddressablesManager.Instance.Load<UnityEngine.Object>(key, (obj) =>
                    {
                        AddResources(key, obj, true);
                        count++;
                        list.Add(obj);
                        if (count == keys.Count)
                            onComplete?.Invoke(list);
                    });
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                onComplete?.Invoke(null);
            }
        }
        public void RemoveResource(string key)
        {
            if (resources.TryGetValue(key, out ResourceData t) == false) return;
            if (t.Object == null) return;
            if (t.IsPatchResource)
                UnityEngine.Resources.UnloadAsset(t.Object);
            else
                AddressablesManager.Instance.Unload(t.Object);
            resources.Remove(key);
        }
        public void RemoveResource(UnityEngine.Object obj)
        {
            ResourceData resourceData = resources.Values.Where(data => data.Object == obj).FirstOrDefault();
            if (resourceData == null) return;
            RemoveResource(resourceData.Name);

        }
        private void AddResources(string path, UnityEngine.Object obj, bool IsPatchResource)
        {
            if (resources.TryGetValue(path, out ResourceData t) == false)
            {
                resources.Add(path, new ResourceData(path, obj, IsPatchResource));
            }
        }

    }
}
