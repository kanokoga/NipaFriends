using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class SimplePool<T> where T : class
    {
        private Queue<T> pool = new Queue<T>();
        private System.Func<T> creater;
        public SimplePool(System.Func<T> creater, int initialCount)
        {
            this.creater = creater;
            for (var i = 0; i < initialCount; i++)
            {
                this.pool.Enqueue(this.CreateObject());
            }
        }

        public T ReleaseObject()
        {
            if (this.pool.Count > 0)
            {
                return this.pool.Dequeue();
            }
            else
            {
                return this.CreateObject();
            }
        }

        public void CacheObject(T o)
        {
            this.pool.Enqueue(o);
        }

        private T CreateObject()
        {
            return this.creater();
        }
    }
}