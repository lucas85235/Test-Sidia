using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _instance;

    public static GameAssets Instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<GameAssets>("GameAssets"));
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    [SerializeField] private ObjectPool damageTexts;
    [SerializeField] private ObjectPool rollTexts;
    [SerializeField] private ObjectPool healthTexts;

    public FloatingText CreateText(Vector3 position, int damageAmount, ObjectPool textPool)
    {
        position.y += 0.75f;

        var damageFloating = textPool.GetInstance();
        damageFloating.transform.position = position;
        damageFloating.SetActive(true);

        var damagePopup = damageFloating.GetComponent<FloatingText>();
        damagePopup.Init(damageAmount);

        return damagePopup;
    }

    public static FloatingText CreateDamageText(Vector3 position, int damageAmount)
    {
        return _instance.CreateText(position, damageAmount, _instance.damageTexts);
    }

    public static FloatingText CreateRollText(Vector3 position, int damageAmount)
    {
        return _instance.CreateText(position, damageAmount, _instance.rollTexts);
    }

    public static FloatingText CreateHealthText(Vector3 position, int damageAmount)
    {
        return _instance.CreateText(position, damageAmount, _instance.healthTexts);
    }
}
