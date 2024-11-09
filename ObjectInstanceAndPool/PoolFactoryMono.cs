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
    ///　MonoBehaviour継承タイプ
    /// </summary>
    [Serializable]
    public class PoolFactoryMono : MonoBehaviour
    {
        [SerializeField] protected PoolFactory<MonoBehaviour> poolFactory;


        protected virtual void Awake()
        {
            this.poolFactory = new PoolFactory<MonoBehaviour>(this.transform);
        }

        public virtual MonoBehaviour GetObject()
            => this.poolFactory.GetObject();

        public virtual void PoolObject(MonoBehaviour obj)
            => this.poolFactory.PoolObject(obj);
    }
}
