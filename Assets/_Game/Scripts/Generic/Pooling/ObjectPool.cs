using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int amount = 5;

    private List<GameObject> instances;
    private int lastInstanceIndex = 0;

    private void Awake()
    {
        GameObject conteiner = new GameObject();
        conteiner.name = "ObjectPool_" + prefab.name;

        instances = new List<GameObject>();

        for (int i = 0; i < amount; i++)
        {
            var obj = Instantiate(prefab, conteiner.transform);
            obj.SetActive(false);
            instances.Add(obj);
        }
    }

    public GameObject GetInstance()
    {
        var instance = instances[lastInstanceIndex];

        lastInstanceIndex++;
        if (lastInstanceIndex >= instances.Count)
            lastInstanceIndex = 0;

        return instance;
    }
}
