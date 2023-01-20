using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Generics;
using Random = UnityEngine.Random;

public class DiceRoll : MonoBehaviour
{
    [SerializeField] private DiceHud[] playersHud = new DiceHud[2];

    public Action<int> OnRoll;
    private int _rollValue;
    private Rigidbody _rb;
    private Target _target = Target.Player1;

    public Target CurrentPlayer
    {
        get => _target;
        set
        {
            if (_target != value)
            {
                playersHud[(int)_target].button.interactable = false;
            }

            _target = value;
        }
    }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        playersHud[0].button.onClick.AddListener(Roll);
        playersHud[1].button.onClick.AddListener(Roll);
        ActiveRoolButton(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && playersHud[(int)_target].button.interactable)
        {
            Roll();
        }
    }

    public void ActiveRoolButton(bool active)
    {
        playersHud[(int)CurrentPlayer].button.interactable = active;

        if (active)
            playersHud[(int)_target].diceAreaPopup.Open();
        else playersHud[(int)_target].diceAreaPopup.Close();
    }

    public void Roll()
    {
        playersHud[(int)CurrentPlayer].button.interactable = false;

        Vector3 randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(0.5f, 1f), Random.Range(-1f, 1f));
        var randomForce = randomDirection * Random.Range(300, 600);

        _rb.AddForce(randomForce);
        _rb.AddTorque(randomForce);
        Invoke(nameof(GetRollValue), 2f);
        AudioManager.Instance.PlaySoundEffect(1);
    }

    private void GetRollValue()
    {
        Vector3 euler = transform.eulerAngles;
        euler.x = Mathf.Round(euler.x / 90) * 90;
        euler.y = Mathf.Round(euler.y / 90) * 90;
        euler.z = Mathf.Round(euler.z / 90) * 90;

        _rollValue = (int)(euler.y / 90) + 1;
        playersHud[(int)CurrentPlayer].button.interactable = false;
        OnRoll?.Invoke(_rollValue);
    }

    [System.Serializable]
    public struct DiceHud
    {
        public Button button;
        public Popup diceAreaPopup;
    }
}
