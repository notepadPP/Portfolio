using UnityEngine;

namespace Framework.Common.Scene
{

    abstract public class SceneBase : Element
    {
        public override void DoDestroy()
        {
        }

        public override void OnInitialize()
        {

        }
        public void SceneInitialize(object obj)
        {
            OnSceneInitialize(obj);
        }
        abstract public void OnSceneInitialize(object obj);
    }

}