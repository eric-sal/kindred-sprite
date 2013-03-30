#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;

public class OnChangeAttribute : PropertyAttribute {
    public readonly string callback;

    public OnChangeAttribute(string callback) {
        this.callback = callback;
    }
}

#endif