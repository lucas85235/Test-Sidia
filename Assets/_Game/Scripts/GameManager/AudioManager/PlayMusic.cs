using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public int clipIndex = 0;

    private void Start()
    {
        AudioManager.Instance.PlayMusic(clipIndex);
    }
}
