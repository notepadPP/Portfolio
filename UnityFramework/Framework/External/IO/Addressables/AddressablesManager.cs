using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Framework.Common.IO.Addressables
{
    public class AddressablesManager : Template.Singleton<AddressablesManager>
    {
        const string unconditionalDownloadLabel = "Remote";
        private Dictionary<string, List<IResourceLocation>> labelResourceLocation = null;
        public override void OnInitialize()
        {
            labelResourceLocation = new Dictionary<string, List<IResourceLocation>>();
        }
        public override void DoDestroy()
        {
        }
        #region public
        public List<IResourceLocation> GetResourceLocations(string label)
        {
            labelResourceLocation.TryGetValue(label, out List<IResourceLocation> locations);
            return locations;
        }
        public void ResourcesLocation(string label = unconditionalDownloadLabel)
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(label);
            IList<IResourceLocation> result = handle.WaitForCompletion();
            foreach (IResourceLocation resourceLocation in result)
            {
                AddLabel(unconditionalDownloadLabel, resourceLocation);
            }
        }
        public void GetDownloadSize(string label, Action<long> onComplete) => behaviour.StartCoroutine(GetDownloadSizeAsync(label, onComplete));
        public void Download(string label, Action<float> progress) => behaviour.StartCoroutine(DownloadAsync(label, progress));

        public void Load<T>(string key, Action<T> onComplete) where T : UnityEngine.Object => behaviour.StartCoroutine(LoadAsync(key, onComplete));
        public void Loads<T>(List<string> key, Action<List<T>> onComplete) where T : UnityEngine.Object => behaviour.StartCoroutine(LoadsAsync(key, onComplete));
        public void Unload(UnityEngine.Object obj) => UnityEngine.AddressableAssets.Addressables.Release(obj);
        public void Clear()
        {
            // 다운로드 저장한 각라벨들을 제거해주자
            foreach (string key in labelResourceLocation.Keys)
                UnityEngine.AddressableAssets.Addressables.ClearDependencyCacheAsync(key);

            //// 유니티에서 제공하는 캐싱 제거함수 이것은 강제로 모든 캐싱을 지운다.
            //Caching.CleanCache();
        }
        #endregion
        #region private
        private IEnumerator ResourcesLocationAsync(string label, Action onComplete = null)
        {
            AsyncOperationHandle<IList<IResourceLocation>> handle = UnityEngine.AddressableAssets.Addressables.LoadResourceLocationsAsync(label);
            yield return handle;
            foreach (IResourceLocation resourceLocation in handle.Result)
            {
                AddLabel(unconditionalDownloadLabel, resourceLocation);
            }
            onComplete?.Invoke();
            UnityEngine.AddressableAssets.Addressables.Release(handle);
        }
        public IEnumerator GetDownloadSizeAsync(string label, Action<long> onComplete)
        {
            if (labelResourceLocation.TryGetValue(label, out List<IResourceLocation> locations) == false)
                yield return ResourcesLocationAsync(label);
            if (labelResourceLocation.TryGetValue(label, out locations) == false)
                yield break;
            long size = 0;
            foreach (IResourceLocation location in locations)
            {
                AsyncOperationHandle<long> handle = UnityEngine.AddressableAssets.Addressables.GetDownloadSizeAsync(label);
                yield return handle;
                size += handle.Result;
            }
            onComplete?.Invoke(size);
        }
        private IEnumerator DownloadAsync(string label, Action<float> progress)
        {
            if (labelResourceLocation.TryGetValue(label, out List<IResourceLocation> locations) == false)
                yield return ResourcesLocationAsync(label);

            if (labelResourceLocation.TryGetValue(label, out locations) == true)
            {
                AsyncOperationHandle handle = UnityEngine.AddressableAssets.Addressables.DownloadDependenciesAsync(locations);
                while (handle.IsDone == false && handle.Status != AsyncOperationStatus.Failed)
                {
                    progress?.Invoke(handle.PercentComplete);
                    yield return null;
                }
                progress?.Invoke(1.0f);
                UnityEngine.AddressableAssets.Addressables.Release(handle);
            }
        }
        private void AddLabel(string label, IResourceLocation resourceLocation)
        {
            if (labelResourceLocation.TryGetValue(label, out List<IResourceLocation> locations) == false)
                labelResourceLocation.Add(label, locations = new List<IResourceLocation>());
            if (locations.Find(obj => obj.PrimaryKey == resourceLocation.PrimaryKey) == null)
                locations.Add(resourceLocation);
        }

        

        private IEnumerator LoadAsync<T>(string key, Action<T> onComplete) where T : UnityEngine.Object
        {
            AsyncOperationHandle<T> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
            yield return handle;
            if (handle.IsDone)
                onComplete?.Invoke(handle.Result);
            else
                onComplete?.Invoke(null);
        }
        private IEnumerator LoadsAsync<T>(List<string> keys, Action<List<T>> onComplete) where T : UnityEngine.Object
        {
            List<T> list = new List<T>();
            foreach(string key in keys)
            {
                AsyncOperationHandle<T> handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<T>(key);
                yield return handle;
                if (handle.IsDone)
                    list.Add(handle.Result);
            }
            onComplete?.Invoke(list);
        }
        #endregion
    }
}

