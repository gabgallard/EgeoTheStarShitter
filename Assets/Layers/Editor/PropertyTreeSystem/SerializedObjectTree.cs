
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SerializedObjectTree 
{
    private SerializedObject serializedObject;

    private Dictionary<string, SerializedPropertyTree> cachedProperties = new Dictionary<string, SerializedPropertyTree>();

    public SerializedObjectTree(Object obj)
    {
        obj = obj ?? throw new System.ArgumentNullException(nameof(obj));
        this.serializedObject = new SerializedObject(obj);
    }

    public SerializedObjectTree(SerializedObject serializedObject)
    {
        this.serializedObject = serializedObject ?? throw new System.ArgumentNullException(nameof(serializedObject));
    }

    public Object context
    {
        get
        {
            return serializedObject.context;
        }
    }

    public bool hasModifiedProperties
    {
        get
        {
            return serializedObject.hasModifiedProperties;
        }
    }

    public bool isEditingMultipleObjects
    {
        get
        {
            return serializedObject.isEditingMultipleObjects;
        }
    }

    public int maxArraySizeForMultiEditing
    {
        get
        {
            return serializedObject.maxArraySizeForMultiEditing;
        }
        set
        {
            serializedObject.maxArraySizeForMultiEditing = value;
        }
    }

    public Object targetObject
    {
        get
        {
            return serializedObject.targetObject;
        }
    }

    public Object[] targetObjects
    {
        get
        {
            return serializedObject.targetObjects;
        }
    }

    public bool ApplyModifiedProperties()
    {
        return serializedObject.ApplyModifiedProperties();
    }

    public bool ApplyModifiedPropertiesWithoutUndo()
    {
        return serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    public void CopyFromSerializedProperty(SerializedProperty prop)
    {
        serializedObject.CopyFromSerializedProperty(prop);
    }

    public void CopyFromSerializedPropertyIfDifferent(SerializedProperty prop)
    {
        serializedObject.CopyFromSerializedPropertyIfDifferent(prop);
    }

    public SerializedPropertyTree FindProperty(string path)
    {
        if (!cachedProperties.ContainsKey(path)) {
            SerializedProperty property = serializedObject.FindProperty(path);
            if (property == null)
                return null;

            cachedProperties.Add(path, new SerializedPropertyTree(this,property));
        }

        return cachedProperties[path];
    }

    public SerializedProperty GetIterator()
    {
        return serializedObject.GetIterator();
    }

    public void SetIsDifferentCacheDirty()
    {
        serializedObject.SetIsDifferentCacheDirty();
    }

    public void Update()
    {
        serializedObject.Update();
    }

    public bool UpdateIfRequiredOrScript()
    {
        return serializedObject.UpdateIfRequiredOrScript();
    }

    public void ClearCache()
    {
        cachedProperties.Clear();
    }

    public static implicit operator SerializedObject(SerializedObjectTree d) => d.serializedObject;
}
