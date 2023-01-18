using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GameManager : MonoBehaviour
{
    private Grid _grid;

    private void Start()
    {
        _grid = GetComponent<Grid>();
        _grid.Create();
    }
}
