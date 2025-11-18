using Framework.Common.Template;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Framework.Common.Scene
{

    public class SceneManager : Singleton<SceneManager>
    {
        UnityEngine.SceneManagement.Scene currentScene;
        public override void DoDestroy()
        {
        }

        public override void OnInitialize()
        {
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        }

        public void LoadScene(string sceneName, object obj = null)
        {
            behaviour.StartCoroutine(LoadSceneAsync(sceneName, obj));
        }

        private IEnumerator LoadSceneAsync(string sceneName, object obj)
        {
            Common.Events.EventManager.Instance.Clear(Events.EventManager.EventType.Default);
            AsyncOperation async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
            async.allowSceneActivation = false;
            
            while (!async.isDone && async.progress < 0.9f) yield return null;
            
            async.allowSceneActivation = true;
            
            while (async.progress < 1.0f) yield return null;

            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

            GameObject[] gameObjects = currentScene.GetRootGameObjects();
            
            foreach (GameObject gameObject in gameObjects)
            {
                SceneBase sceneBase = gameObject.GetComponentInChildren<SceneBase>();
                if (sceneBase == null) continue;
                sceneBase.Initialize();
                sceneBase.SceneInitialize(obj);
                break;
            }

            foreach (GameObject gameObject in gameObjects)
            {
                Interface.IElement element = gameObject.GetComponentInChildren<Interface.IElement>(true);
                if (element == null) continue;
                element.Initialize();
                break;
            }
        }
    }
}