using System;
using UnityEngine;

public class PopupButtonData
{
    public string Label { get; private set; }
    public Action Callback { get; private set; }
    public GameObject ButtonPrefabOverride { get; private set; }

    public PopupButtonData(string label, Action callback = null, GameObject buttonPrefabOverride = null)
    {
        Label = label;
        Callback = callback;
        ButtonPrefabOverride = buttonPrefabOverride;
    }
}
