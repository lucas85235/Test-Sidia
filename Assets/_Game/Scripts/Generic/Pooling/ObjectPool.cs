using System.Collections.Generic;
using UnityEngine;

namespace Game.Generics
{
    public class ObjectPool : MonoBehaviour
    {
        [SerializeField] private GameObject prefab;
        [SerializeField] private int amount = 5;

        private List<GameObject> _instances;
        private int _lastInstanceIndex = 0;

        private void Awake()
        {
            GameObject conteiner = new GameObject();
            conteiner.name = "ObjectPool_" + prefab.name;

            _instances = new List<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                var obj = Instantiate(prefab, conteiner.transform);
                obj.SetActive(false);
                _instances.Add(obj);
            }
        }

        public GameObject GetInstance()
        {
            var instance = _instances[_lastInstanceIndex];

            _lastInstanceIndex++;
            if (_lastInstanceIndex >= _instances.Count)
                _lastInstanceIndex = 0;

            return instance;
        }
    }    
}
