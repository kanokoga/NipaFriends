using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaFriends;
using System;

namespace NipaFriends
{
    [Serializable]
    public class AdvPoolFactory<T> where T : MonoBehaviour
    {
        [SerializeField] private T prefab;
        public int PooledObjectCount => this.pool.PooledObjectCount;
        private ObjectPool<T> pool;
        private Transform poolTransform;
        private Action<T> onCreated;

        public AdvPoolFactory()
        {
            this.pool = new ObjectPool<T>(this.CreateObject);
        }

        public void Init(Transform pool)
        {
            this.poolTransform = pool;
        }

        public T GetObject()
            => this.pool.GetObject();

        public void PoolObject(T obj)
            => this.pool.PoolObject(obj);

        private T CreateObject()
        {
            var obj = UnityEngine.Object.Instantiate(this.prefab);
            obj.transform.SetParent(this.poolTransform);
            obj.gameObject.SetActive(false);
            return obj;
        }
    }
}
