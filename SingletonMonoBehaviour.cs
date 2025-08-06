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
        public static bool HasInstance { get; private set; } = false;

        private static T instance;
   

        protected void SetInstance(T newInstance)
        {
            if (HasInstance == true)
            {
                Debug.LogWarning($"An instance of {typeof(T)} already exists. Overriding it.");
            }

            instance = newInstance;
            HasInstance = true;
        }
        
        protected virtual void OnDestroy()
        {
            if (HasInstance == true && instance == this)
            {
                instance = null;
                HasInstance = false;
            }
        }
    }
}
