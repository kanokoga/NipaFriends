using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaFriends;
using System;

namespace NipaFriends
{
    /// <summary>
    /// プール付きのオブジェクト生成クラス。
    /// 非MonoBehaviourタイプ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PoolFactory<T> where T : MonoBehaviour
    {
        [SerializeField] private T prefab;
        [SerializeField] private Transform poolTransform;
        private ObjectPool<T> pool;


        public PoolFactory(Transform pool = null)
        {
            if(pool!=null)
            {
                this.poolTransform = pool;
            }
            this.pool = new ObjectPool<T>(this.CreateObject);
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
