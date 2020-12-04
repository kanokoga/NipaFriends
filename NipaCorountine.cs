using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class NipaCorountine : MonoBehaviour
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
            if (c != null)
            {
                this.StopCoroutine(c);
            }
            return this.StartCoroutine(process);
        }
    }
}