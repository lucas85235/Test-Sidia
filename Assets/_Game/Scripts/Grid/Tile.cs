using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int row;
    public int column;
    public GameObject occupant;
    public Color highlightColor = Color.red;

    private bool _isSelectable = false;
    private Renderer _render;
    private Color _defaultColor;
    private Color _selectableColor;

    public bool IsSelectad { get; private set; } = false;

    private void Start()
    {
        _render = GetComponent<Renderer>();
        _defaultColor = _render.material.color;
        var color = Color.green;
        color.a = 0.8f;
        _selectableColor = color;
    }

    public void SelectableMode(bool state)
    {
        _isSelectable = state;

        if (!_isSelectable)
        {
            _render.material.color = _defaultColor;
            IsSelectad = false;
        }
        else _render.material.color = _selectableColor;
    }

    private void OnMouseOver()
    {
        if (_isSelectable)
        {
            _render.material.color = highlightColor;
            IsSelectad = true;
        }
    }

    private void OnMouseExit()
    {
        if (_isSelectable)
        {
            _render.material.color = _selectableColor;
            IsSelectad = false;
        }
    }
}
