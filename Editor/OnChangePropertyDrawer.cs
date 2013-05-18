#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

[CustomPropertyDrawer(typeof(OnChangeAttribute))]
public class OnChangePropertyDrawer : PropertyDrawer {
    OnChangeAttribute onChangeAttribute { get { return (OnChangeAttribute)attribute; } }

    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label) {
        EditorGUI.BeginChangeCheck();

        object val = null;
        switch (prop.propertyType) {
        case SerializedPropertyType.Boolean:
            val = EditorGUI.Toggle(position, label, prop.boolValue);
            break;
        case SerializedPropertyType.Float:
            val = EditorGUI.FloatField(position, label, prop.floatValue);
            break;
        case SerializedPropertyType.Integer:
            val = EditorGUI.IntField(position, label, prop.intValue);
            break;
        case SerializedPropertyType.ObjectReference:
            val = EditorGUI.ObjectField(position, label, prop.objectReferenceValue, onChangeAttribute.objectReferenceType, true);
            break;
        case SerializedPropertyType.String:
            val = EditorGUI.TextField(position, label, prop.stringValue);
            break;
        }

        if (EditorGUI.EndChangeCheck()) {
            // If this is a nested attribute, then the property's instance can be found with GetParent.
            // Otherwise, it's going to be the targetObject.
            object instance = (prop.depth > 1) ? GetParent(prop) : prop.serializedObject.targetObject;

            MethodInfo method = instance.GetType().GetMethod(onChangeAttribute.callback);
            method.Invoke(instance, new object[] { val });
        }
    }

    // Method for getting parent taken from:
    // - http://answers.unity3d.com/questions/425012/get-the-instance-the-serializedproperty-belongs-to.html#answer-425602
    public object GetParent(SerializedProperty prop) {
        object obj = prop.serializedObject.targetObject;
        string path = prop.propertyPath.Replace(".Array.data[", "[");
        string[] elements = path.Split('.');

        foreach (string element in elements.Take(elements.Length - 1)) {
            if (element.Contains("[")) {
                string elementName = element.Substring(0, element.IndexOf("["));
                int index = int.Parse(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue(obj, elementName, index);
            } else {
                obj = GetValue(obj, element);
            }
        }

        return obj;
    }

    public object GetValue(object source, string name) {
        object returnValue = null;

        if (source != null) {
            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f == null) {
                PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null) {
                    returnValue = p.GetValue(source, null);
                }
            } else {
                returnValue = f.GetValue(source);
            }
        }

        return returnValue;
    }

    public object GetValue(object source, string name, int index) {
        IEnumerable enumerable = GetValue(source, name) as IEnumerable;
        IEnumerator enm = enumerable.GetEnumerator();
        while (index-- >= 0) {
            enm.MoveNext();
        }

        return enm.Current;
    }
}

#endif
