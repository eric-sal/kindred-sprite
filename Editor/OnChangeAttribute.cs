#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

public class OnChangeAttribute : PropertyAttribute {
    public readonly string callback;
    public readonly Type objectReferenceType;

    public OnChangeAttribute(string callback) {
        this.callback = callback;
    }

    public OnChangeAttribute(string callback, Type objectReferenceType) {
        this.callback = callback;
        this.objectReferenceType = objectReferenceType;
    }
}

#endif