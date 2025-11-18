using System.Collections.Generic;
using UnityEngine;

namespace Framework.Tween
{
    public class TweenGroupPlay : TweenBase
    {
        public override float Timer => CalcTimer();
        [SerializeField] List<TweenBase> groupList = new List<TweenBase>();
        List<TweenBase> finishList = new List<TweenBase>();

        protected override void OnReset()
        {
            foreach (TweenBase tween in groupList)
            {
                tween.Rewind();
            }
        }
        private float CalcTimer()
        {
            timer = 0;
            foreach (TweenBase tween in groupList)
            {
                if (tween.Timer > timer)
                    timer = tween.Timer;
            }
            return timer;
        }
        protected override void OnStart()
        {
            foreach (TweenBase tween in groupList)
            {
                tween.Play();
            }
        }

        protected override void OnUpdate(float percent)
        {
        }

        protected override void OnUpdateFinish()
        {
        }

        protected override void OnStop()
        {
            foreach (TweenBase tween in groupList)
            {
                tween.Stop();
            }
        }
    }
}
