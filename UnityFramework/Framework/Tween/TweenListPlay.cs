using System.Collections.Generic;
using UnityEngine;

namespace Framework.Tween
{
    public class TweenListPlay : TweenBase
    {
        [SerializeField] List<TweenBase> List = new List<TweenBase>();

        protected override void OnReset()
        {
            foreach (TweenBase tween in List)
            {
                tween.Rewind();
            }
        }

        //List<TweenBase> finishList = new List<TweenBase>();
        protected override void OnStart()
        {
            timer = 0;
            foreach (TweenBase tween in List)
            {
                timer += tween.Timer;
            }
            if (List.Count > 0)
            {
                List[0].Play(NextCheck);
            }    
        }

        protected override void OnStop()
        {
            foreach (TweenBase tween in List)
            {
                tween.Stop();
            }
        }

        protected override void OnUpdate(float percent)
        {
        }

        protected override void OnUpdateFinish()
        {
        }
        private void NextCheck(TweenBase tween)
        {
            int index = List.FindIndex(obj => obj == tween);
            if (index < 0 || (index + 1) >= List.Count)
            {
                time = timer;
                return;
            }
            index++;
            List[index].Play(NextCheck);
        }
    }
}
