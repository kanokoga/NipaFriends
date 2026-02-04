using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaFriends
{
    public class ObjectPool<T> where T : class
    {
        public int PooledObjectCount => this.pool.Count;

        private Stack<T> pool = new Stack<T>();
        private System.Func<T> creationFunc;


        public ObjectPool(System.Func<T> creationFunc)
          => this.creationFunc = creationFunc;

        public T Get()
        {
            T obj;
            if (this.pool.Count > 0)
            {
                obj = this.pool.Pop();
            }
            else
            {
                obj = this.CreateObject();
            }
            return obj;
        }

        public void Return(T o)
        {
            this.pool.Push(o);
        }

        private T CreateObject()
        {
            return this.creationFunc();
        }
    }
}
