using System;
using UnityEngine;

namespace NipaFriends
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {

        public static T Instance
        {
            get
            {
                if (HasInstance == false)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError(typeof(T) + " is nothing");
                    }
                    else
                    {
                        HasInstance = true;
                    }
                }

                return instance;
            }
        }

        private static T instance;
        private static bool HasInstance = false;

        protected virtual void OnDestroy()
        {
            instance = null;
            HasInstance = false;
        }
    }
}
