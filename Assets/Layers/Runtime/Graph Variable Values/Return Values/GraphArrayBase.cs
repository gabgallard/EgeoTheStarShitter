using ABXY.Layers.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GraphArrayBase: ICollection, IList
{

    protected GraphVariable innerVariable = new GraphVariable();

    private GraphVariable.RetrievalTypes retrievalType;

    protected GraphArrayBase(GraphVariable innerVariable, GraphVariable.RetrievalTypes retrievalType)
    {
        this.innerVariable = innerVariable;
        this.retrievalType = retrievalType;
    }

    protected List<GraphVariableBase> sourceArray
    {
        get
        {
            switch (retrievalType)
            {
                case GraphVariable.RetrievalTypes.DefaultValue:
                    return innerVariable.defaultArrayElements;
                case GraphVariable.RetrievalTypes.ActualValue:
                    return innerVariable.arrayElements;
            }
            return null;
        }
    }


    internal void ReplaceInnerVariable(GraphVariable innerList)
    {
        innerVariable = innerList;
    }

    internal GraphVariable GetInnerVariable()
    {
        return innerVariable;
    }

    public object this[int index] {
        get
        {
            return sourceArray[index].Value();
        }
        set
        {
            sourceArray[index].SetValue(value);
        }
    }

    public int Count
    {
        get
        {
            return sourceArray.Count;
        }
    }

    public bool IsReadOnly => false;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    public bool IsFixedSize => false;

    public int Add(object value)
    {
        Insert(sourceArray.Count, value);
        return sourceArray.Count - 1;
    }

    public void Clear()
    {
        sourceArray.Clear();
    }

    public bool Contains(object value)
    {
        return sourceArray.Select(x => x.Value()).Contains(value);
    }

    public void CopyTo(Array array, int index)
    {
        sourceArray.Select(x => x.Value()).ToArray().CopyTo(array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return sourceArray.Select(x => x.Value()).GetEnumerator();
    }

    public int IndexOf(object value)
    {
        return sourceArray.Select(x => x.Value()).ToList().IndexOf(value);
    }

    public void Insert(int index, object value)
    {
        GraphVariableBase newElement = new GraphVariableBase();
        newElement.typeName = innerVariable.arrayType;
        newElement.SetValue(value);
        sourceArray.Insert(index, newElement);
    }

    public void Remove(object value)
    {
        if (value == null)
            return;
        var removeItem = sourceArray.Find(x => value.Equals(x.Value()));

        if (removeItem == null)
            return;

        sourceArray.Remove(removeItem);
    }

    public void RemoveAt(int index)
    {
        sourceArray.RemoveAt(index);
    }
}
