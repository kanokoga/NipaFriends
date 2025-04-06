using UnityEngine;
using System.Collections.Concurrent;
using System.Threading;
using System;

namespace NipaFriends
{
    public class NipaThread : MonoBehaviour
    {
        private static readonly ConcurrentQueue<Action> mainThreadQueue = new ConcurrentQueue<Action>();

        private void Update()
        {
            while(mainThreadQueue.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        public static void RunTask(Action task, Action callback)
        {
            // Run the task in a background thread
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    task?.Invoke();
                }
                catch(Exception e)
                {
                    Debug.LogError($"Task exception: {e}");
                }

                if(callback != null)
                {
                    // Queue the callback to be executed on the main thread
                    mainThreadQueue.Enqueue(callback);
                }
            });
        }
    }
}
