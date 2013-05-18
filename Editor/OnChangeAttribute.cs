using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Attribute that acts as an event listener on a property.
/// - Ex: [OnChange ("UpdateReloadData")] public bool reloadData = false;
///   Whenever the reloadData property is changed in the Unity editor, the UpdateReloadData()
///   method in the class on which reloadData is defined, will be called.
///
/// It appears as though this attribute can only be added to GameObjects that inherit from MonoBehaviour.
/// </summary>
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
