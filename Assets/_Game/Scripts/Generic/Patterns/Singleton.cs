using UnityEngine;

namespace Game.Generics
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public static T Instance;

        protected virtual void Awake()
        {
            Instance = (T)this;
        }
    }
}
