using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SerializedPropertyTree
{
    public SerializedProperty serializedProperty { get; private set; }

    private SerializedObjectTree objectTree;

    public SerializedPropertyTree(SerializedObjectTree objectTree, SerializedProperty serializedProperty)
    {
        this.objectTree = objectTree ?? throw new System.ArgumentNullException(nameof(objectTree));
        this.serializedProperty = serializedProperty ?? throw new System.ArgumentNullException(nameof(serializedProperty));
    }

    public SerializedObjectTree serializedObject { get { return objectTree; } }

    public Object exposedReferenceValue
    {
        get
        {
            return serializedProperty.exposedReferenceValue;
        }

        set
        {
            serializedProperty.exposedReferenceValue = value;
        }
    }

    // Returns a copy of the SerializedProperty iterator in its current state. This is useful if you want to keep a reference to the current property but continue with the iteration.
    public SerializedPropertyTree Copy()
    {
        return new SerializedPropertyTree( serializedObject, serializedProperty.Copy());
    }

    private Dictionary<string, SerializedPropertyTree> cachedProperties = new Dictionary<string, SerializedPropertyTree>();

    // Retrieves the SerializedProperty at a relative path to the current property.
    public SerializedPropertyTree FindPropertyRelative(string relativePropertyPath)
    {
        if (!cachedProperties.ContainsKey(relativePropertyPath))
        {
            SerializedProperty property = serializedProperty.FindPropertyRelative(relativePropertyPath);
            if (property == null)
                return null;

            cachedProperties.Add(relativePropertyPath, new SerializedPropertyTree(serializedObject, property));
        }

        return cachedProperties[relativePropertyPath];
    }

    // Retrieves an iterator that allows you to iterator over the current nexting of a serialized property.
    public System.Collections.IEnumerator GetEnumerator()
    {
        return serializedProperty.GetEnumerator();
    }

    private Dictionary<int, SerializedPropertyTree> arrayCache = new Dictionary<int, SerializedPropertyTree>();

    // Returns the element at the specified index in the array.
    public SerializedPropertyTree GetArrayElementAtIndex(int index)
    {
        if (!arrayCache.ContainsKey(index))
        {
            SerializedProperty property = serializedProperty.GetArrayElementAtIndex(index);
            if (property == null)
                return null;

            arrayCache.Add(index, new SerializedPropertyTree(serializedObject, property));
        }

        return arrayCache[index];
    }

    
    // Move to next visible property.
    public bool NextVisible(bool enterChildren)
    {
        return serializedProperty.NextVisible(enterChildren);
    }

    // Remove all elements from the array.
    public void ClearArray()
    {
        serializedProperty.ClearArray();
        ClearCache();
    }

    private void ClearCache()
    {
        cachedProperties.Clear();
        arrayCache.Clear();
    }

    public void Dispose()
    {
        serializedProperty.Dispose();
    }



    // See if contained serialized properties are equal.
    public static bool EqualContents(SerializedProperty x, SerializedProperty y)
    {
        return SerializedProperty.EqualContents(x, y);
    }


    // See if raw data inside both serialized property is equal.
    public static bool DataEquals(SerializedProperty x, SerializedProperty y)
    {
        return SerializedProperty.DataEquals(x, y);
    }

    // Does this property represent multiple different values due to multi-object editing? (RO)
    public bool hasMultipleDifferentValues
    {
        get
        {
            return serializedProperty.hasMultipleDifferentValues;
        }
    }


    // Nice display name of the property (RO)
    public string displayName
    {
        get
        {
            return serializedProperty.displayName;
        }
    }


    // Name of the property (RO)
    public string name
    {
        get
        {
            return serializedProperty.name;
        }
    }


    // Type name of the property (RO)
    public string type
    {
        get
        {
            return serializedProperty.type;
        }
    }


    // Type name of the element of an Array property (RO)
    public string arrayElementType
    {
        get
        {
            return serializedProperty.arrayElementType;
        }
    }


    // Tooltip of the property (RO)
    public string tooltip
    {
        get
        {
            return serializedProperty.tooltip;
        }
    }

    // Nesting depth of the property (RO)
    public int depth
    {
        get
        {
            return serializedProperty.depth;
        }
    }


    // Full path of the property (RO)
    public string propertyPath
    {
        get
        {
            return serializedProperty.propertyPath;
        }
    }


    // Is this property editable? (RO)
    public bool editable
    {
        get
        {
            return serializedProperty.editable;
        }
    }


    // Is this property animated? (RO)
    public bool isAnimated
    {
        get
        {
            return serializedProperty.isAnimated;
        }
    }



    // Is this property expanded in the inspector?
    public bool isExpanded
    {
        get
        {
            return serializedProperty.isExpanded;
        }
        set
        {
            serializedProperty.isExpanded = value;
        }
    }


    // Does it have child properties? (RO)
    public bool hasChildren
    {
        get
        {
            return serializedProperty.hasChildren;
        }
    }

    // Does it have visible child properties? (RO)
    public bool hasVisibleChildren
    {
        get
        {
            return serializedProperty.hasVisibleChildren;
        }
    }


    // Is property part of a prefab instance? (RO)
    public bool isInstantiatedPrefab
    {
        get
        {
            return serializedProperty.isInstantiatedPrefab;
        }
    }


    // Is property's value different from the prefab it belongs to?
    public bool prefabOverride
    {
        get
        {
            return serializedProperty.prefabOverride;
        }
        set
        {
            serializedProperty.prefabOverride = value;
        }
    }


    // Is property a default override property which is enforced to always be overridden? (RO)
    public bool isDefaultOverride
    {
        get
        {
            return serializedProperty.isDefaultOverride;
        }
    }

    // Type of this property (RO).
    public SerializedPropertyType propertyType
    {
        get
        {
            return serializedProperty.propertyType;
        }
    }


    // Value of an integer property.
    public int intValue
    {
        get
        {
            return serializedProperty.intValue;
        }
        set
        {
            serializedProperty.intValue = value;
        }
    }


    // Value of an long property.
    public long longValue
    {
        get
        {
            return serializedProperty.longValue;
        }
        set
        {
            serializedProperty.longValue = value;
        }
    }

    // Value of a boolean property.
    public bool boolValue
    {
        get
        {
            return serializedProperty.boolValue;
        }
        set
        {
            serializedProperty.boolValue = value;
        }
    }


    // Value of a float property.
    public float floatValue
    {
        get
        {
            return serializedProperty.floatValue;
        }
        set
        {
            serializedProperty.floatValue = value;
        }
    }

    // Value of a double property.
    public double doubleValue
    {
        get
        {
            return serializedProperty.doubleValue;
        }
        set
        {
            serializedProperty.doubleValue = value;
        }
    }

    // Value of a string property.
    public string stringValue
    {
        get
        {
            return serializedProperty.stringValue;
        }
        set
        {
            serializedProperty.stringValue = value;
        }
    }


    // Value of a color property.
    public Color colorValue
    {
        get
        {
            return serializedProperty.colorValue;
        }
        set
        {
            serializedProperty.colorValue = value;
        }
    }


    // Value of a animation curve property.
    public AnimationCurve animationCurveValue
    {
        get
        {
            return serializedProperty.animationCurveValue;
        }
        set
        {
            serializedProperty.animationCurveValue = value;
        }
    }


    // Value of an object reference property.
    public Object objectReferenceValue
    {
        get
        {
            return serializedProperty.objectReferenceValue;
        }
        set
        {
            serializedProperty.objectReferenceValue = value;
        }
    }

    


    public int objectReferenceInstanceIDValue
    {
        get
        {
            return serializedProperty.objectReferenceInstanceIDValue;
        }
        set
        {
            serializedProperty.objectReferenceInstanceIDValue = value;
        }
    }


    // Enum index of an enum property.
    public int enumValueIndex
    {
        get
        {
            return serializedProperty.enumValueIndex;
        }
        set
        {
            serializedProperty.enumValueIndex = value;
        }
    }

    // Names of enumeration of an enum property.
    public string[] enumNames
    {
        get
        {
            return serializedProperty.enumNames;
        }
    }

    // Names of enumeration of an enum property, nicified.
    public string[] enumDisplayNames
    {
        get
        {
            return serializedProperty.enumDisplayNames;
        }
    }

    // Value of a 2D vector property.
    public Vector2 vector2Value
    {
        get
        {
            return serializedProperty.vector2Value;
        }
        set
        {
            serializedProperty.vector2Value = value;
        }
    }

    // Value of a 3D vector property.
    public Vector3 vector3Value
    {
        get
        {
            return serializedProperty.vector3Value;
        }
        set
        {
            serializedProperty.vector3Value = value;
        }
    }


    // Value of a 4D vector property.
    public Vector4 vector4Value
    {
        get
        {
            return serializedProperty.vector4Value;
        }
        set
        {
            serializedProperty.vector4Value = value;
        }
    }


    // Value of a 2D int vector property.
    public Vector2Int vector2IntValue
    {
        get
        {
            return serializedProperty.vector2IntValue;
        }
        set
        {
            serializedProperty.vector2IntValue = value;
        }
    }

    // Value of a 3D int vector property.
    public Vector3Int vector3IntValue
    {
        get
        {
            return serializedProperty.vector3IntValue;
        }
        set
        {
            serializedProperty.vector3IntValue = value;
        }
    }

    // Value of a quaternion property.
    public Quaternion quaternionValue
    {
        get
        {
            return serializedProperty.quaternionValue;
        }
        set
        {
            serializedProperty.quaternionValue = value;
        }
    }

    // Value of a rectangle property.
    public Rect rectValue
    {
        get
        {
            return serializedProperty.rectValue;
        }
        set
        {
            serializedProperty.rectValue = value;
        }
    }


    // Value of a rectangle int property.
    public RectInt rectIntValue
    {
        get
        {
            return serializedProperty.rectIntValue;
        }
        set
        {
            serializedProperty.rectIntValue = value ;
        }
    }


    // Value of bounds property.
    public Bounds boundsValue
    {
        get
        {
            return serializedProperty.boundsValue;
        }
        set
        {
            serializedProperty.boundsValue = value;
        }
    }


    // Value of bounds int property.
    public BoundsInt boundsIntValue
    {
        get
        {
            return serializedProperty.boundsIntValue;
        }
        set
        {
            serializedProperty.boundsIntValue = value;
        }
    }


    // Move to next property.
    public bool Next(bool enterChildren)
    {
        return serializedProperty.Next(enterChildren);
    }


    // Move to first property of the object.
    public void Reset()
    {
        serializedProperty.Reset();
    }


    // Count remaining visible properties.
    public int CountRemaining()
    {
        return serializedProperty.CountRemaining();
    }



    // Count visible children of this property, including this property itself.
    public int CountInProperty()
    {
        return serializedProperty.CountInProperty();
    }

    public bool DuplicateCommand()
    {
        string[] splitPath = serializedProperty.propertyPath.Split('.');
        bool result = serializedProperty.DuplicateCommand();
        string parentPath = string.Join(".", splitPath.Take(splitPath.Length - 1));
        SerializedPropertyTree parent = serializedObject.FindProperty(parentPath);
        parent.ClearCache();
        return result;
    }


    // Deletes the serialized property.
    public bool DeleteCommand()
    {
        string[] splitPath = serializedProperty.propertyPath.Split('.');
        bool result = serializedProperty.DeleteCommand();

        string parentPath = string.Join(".", splitPath.Take(splitPath.Length - 1));
        SerializedPropertyTree parent = serializedObject.FindProperty(parentPath);
        parent.ClearCache();
        return result;

    }



    // Retrieves the SerializedProperty that defines the end range of this property.
    public SerializedPropertyTree GetEndProperty()
    {
        return new SerializedPropertyTree(serializedObject,  serializedProperty.GetEndProperty());
    }

    // Retrieves the SerializedProperty that defines the end range of this property.
    public SerializedProperty GetEndProperty(bool includeInvisible)
    {
        return serializedProperty.GetEndProperty(includeInvisible);
    }

    // Is this property an array? (RO)
    public bool isArray
    {
        get
        {
            return serializedProperty.isArray;
        }
    }

    // The number of elements in the array.
    public int arraySize
    {
        get
        {
            return serializedProperty.arraySize;
        }
        set
        {
            serializedProperty.arraySize = value;
        }
    }

    public void InsertArrayElementAtIndex(int index)
    {
        serializedProperty.InsertArrayElementAtIndex(index);
        ClearCache();
    }


    // Delete the element at the specified index in the array.
    public void DeleteArrayElementAtIndex(int index)
    {
        serializedProperty.DeleteArrayElementAtIndex(index);
        ClearCache();
    }



    // Move an array element from srcIndex to dstIndex.
    public bool MoveArrayElement(int srcIndex, int dstIndex)
    {
        bool result = serializedProperty.MoveArrayElement(srcIndex, dstIndex);
        ClearCache();
        return result;
    }


    // Is this property a fixed buffer? (RO)
    public bool isFixedBuffer
    {
        get { return serializedProperty.isFixedBuffer; }
    }



    // The number of elements in the fixed buffer (RO).
    public int fixedBufferSize
    {
        get
        {
            return serializedProperty.fixedBufferSize;
        }
    }

    public SerializedProperty GetFixedBufferElementAtIndex(int index)
    {
        return serializedProperty.GetFixedBufferElementAtIndex(index);
    }

    public static implicit operator SerializedProperty(SerializedPropertyTree d) => d.serializedProperty;

}
