using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaFriends;
using System;

namespace NipaFriends
{
    public class PoolFactory<T> where T : MonoBehaviour
    {
        public int PooledObjectCount => this.pool.PooledObjectCount;
        private T prefab;
        private ObjectPool<T> pool;
        private Transform poolTransform;
        private Action<T> onCreated;

        public PoolFactory(T prefab, Transform pool)
        {
            this.prefab = prefab;
            this.pool = new ObjectPool<T>(this.CreateObject);
            this.poolTransform = pool;
        }

        public T GetObject()
            => this.pool.GetObject();

        public void PoolObject(T obj)
        {
            this.pool.PoolObject(obj);
        }

        private T CreateObject()
        {
            var obj = UnityEngine.Object.Instantiate(this.prefab);
            obj.transform.SetParent(this.poolTransform);
            obj.gameObject.SetActive(false);
            return obj;
        }
    }
}
