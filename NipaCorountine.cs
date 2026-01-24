using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class NipaCorountine : SingletonMonoBehaviour<NipaCorountine>
    {
        public static NipaCorountine Instance => _instance;
        private static NipaCorountine _instance;

        private void Awake()
        {
            _instance = this;
        }

        public Coroutine StartProcess(IEnumerator process)
            => this.StartCoroutine(process);

        public void StopProcess(Coroutine c)
            => this.StopCoroutine(c);

        public Coroutine StartProcess(IEnumerator process, Coroutine c)
        {
            if(c != null)
            {
                this.StopCoroutine(c);
            }

            return this.StartCoroutine(process);
        }

        public void StartLazyAction(System.Action action, float delyay)
        {
            this.StopCoroutine(
                this.LazyProcess(action, new WaitForSeconds(delyay), 1)
            );
        }

        public void StartLazyAction(System.Action action, int frameDelay = 1)
        {
            this.StopCoroutine(
                this.LazyProcess(action, new WaitForEndOfFrame(), frameDelay)
            );
        }

        private IEnumerator LazyProcess(System.Action action, YieldInstruction delay, int repeat)
        {
            for(int i = 0; i < repeat - 1; i++)
            {
                yield return delay;
            }

            action();
        }
    }
}
