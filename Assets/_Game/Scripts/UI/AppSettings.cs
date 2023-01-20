using UnityEngine;

[CreateAssetMenu(fileName = "AppSettings", menuName = "ScriptableObjects/AppSettings", order = 1)]
public class AppSettings : ScriptableObject
{
    public Color blackBackgroundColor;
    public float popupOpenAnimationTime;
    public float popupCloseAnimationTime;
}
