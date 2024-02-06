using System;
using UnityEngine;

namespace NipaFriends
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (Init == false)
                {
                    instance = (T)FindFirstObjectByType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(T) + " is nothing");
                    }
                    
                    Init = true;
                }

                return instance;
            }
        }

        private static bool Init = false;

        protected virtual void OnDestroy()
        {
            Init = false;
        }
    }
}