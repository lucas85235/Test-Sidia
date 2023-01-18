using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class InspectorButton : MonoBehaviour
{
    [SerializeField]
    public Button.ButtonClickedEvent clickFirstEvent;

    public void FirstButton_Click()
    {
        clickFirstEvent?.Invoke();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InspectorButton))]
public class InspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InspectorButton myScript = (InspectorButton)target;
        string buttonName = (myScript.clickFirstEvent != null && myScript.clickFirstEvent.GetPersistentEventCount() > 0) ?
            myScript.clickFirstEvent.GetPersistentMethodName(0) :
            "not defined";
        if (string.IsNullOrEmpty(buttonName)) buttonName = "not defined";
        if (GUILayout.Button(buttonName))
        {
            myScript.FirstButton_Click();
        }
        DrawDefaultInspector();
    }
}
#endif
