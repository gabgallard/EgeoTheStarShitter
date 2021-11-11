using ABXY.Layers.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class GraphArray<T> : GraphArrayBase, ICollection<T>, IEnumerable<T>, IEnumerable, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>
{

    internal GraphArray(GraphVariable innerVariable, GraphVariable.RetrievalTypes retrievalType) : base(innerVariable, retrievalType)
    {
    }

    /// <summary>
    /// Creates a Graph Array from the source array. This copies all elements to the graph array
    /// </summary>
    /// <param name="sourceCollection"></param>
    public GraphArray(IEnumerable<T> sourceCollection): base(null, GraphVariable.RetrievalTypes.ActualValue)
    {
        InitializeEmptyArray();
        AddRange(sourceCollection);
    }

    public GraphArray() : base(null, GraphVariable.RetrievalTypes.ActualValue)
    {
        InitializeEmptyArray();
    }

    private void InitializeEmptyArray()
    {
        innerVariable = new GraphVariable();
        innerVariable.typeName = typeof(List<GraphVariable>).FullName;
        innerVariable.arrayType = typeof(T).FullName;
    }


    public static implicit operator GraphArray<T>(List<T> array)
    {
        return new GraphArray<T>(array);
    }

    public static implicit operator GraphArray<T>(T[] array)
    {
        return new GraphArray<T>(array);
    }

    public static implicit operator List<T>(GraphArray<T> array)
    {
        return array.ToList<T>();
    }

    public static implicit operator T[](GraphArray<T> array)
    {
        return array.ToArray<T>();
    }



    public new T this[int index] {
        get
        {
            return (T)sourceArray[index].Value();
        }
        set
        {
            sourceArray[index].SetValue(value);
        }
    }


    

    //public int Capacity { get; set; }

    public void Add(T item)
    {
        Insert(sourceArray.Count, item);
    }

    

    public void AddRange(IEnumerable<T> collection)
    {
        foreach (var element in collection)
            Add(element);
    }
    public ReadOnlyCollection<T> AsReadOnly()
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().AsReadOnly();
    }
    public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().BinarySearch(index, count, item, comparer);
    }
    public int BinarySearch(T item)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().BinarySearch(item);
    }
    public int BinarySearch(T item, IComparer<T> comparer)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().BinarySearch(item, comparer);
    }
    public bool Contains(T item)
    {
        return sourceArray.Select(x => (T)x.Value()).Contains(item);
    }


    public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().ConvertAll(converter);
    }
    public void CopyTo(T[] array, int arrayIndex)
    {
        sourceArray.Select(x => (T)x.Value()).ToList().CopyTo(array, arrayIndex);
    }
    public void CopyTo(T[] array)
    {
        sourceArray.Select(x => (T)x.Value()).ToList().CopyTo(array);
    }
    public void CopyTo(int index, T[] array, int arrayIndex, int count)
    {
        sourceArray.Select(x => (T)x.Value()).ToList().CopyTo(index, array, arrayIndex, count);
    }
    /*
    public void CopyFrom(IEnumerable<T> sourceCollection)
    {
        if (sourceCollection == null)
            throw new ArgumentNullException("Source IEnumerable cannot be null");
        foreach (T element in sourceCollection)
        {
            GraphVariableBase elementVar = new GraphVariableBase();
            elementVar.typeName = typeof(T).FullName;
            elementVar.SetValue(element);
            elementVar.variableID = System.Guid.NewGuid().ToString();
            innerVariable.arrayElements.Add(elementVar);
        }
    }*/

    public bool Exists(Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().Exists(match);
    }
    public T Find(Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().Find(match);
    }
    public List<T> FindAll(Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindAll(match);
    }
    public int FindIndex(int startIndex, int count, Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindIndex(startIndex, count, match);
    }
    public int FindIndex(int startIndex, Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindIndex(startIndex, match);
    }
    public int FindIndex(Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindIndex(match);
    }
    public T FindLast(Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindLast(match);
    }
    public int FindLastIndex(int startIndex, int count, Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindLastIndex(startIndex, count, match);
    }
    public int FindLastIndex(int startIndex, Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindLastIndex(startIndex, match);
    }
    public int FindLastIndex(Predicate<T> match)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().FindLastIndex(match);
    }
    public void ForEach(Action<T> action)
    {
        sourceArray.Select(x => (T)x.Value()).ToList().ForEach(action);
    }

    public List<T> GetRange(int index, int count)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().GetRange(index, count);
    }
    public int IndexOf(T item, int index, int count)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().IndexOf(item, index, count);
    }
    public int IndexOf(T item, int index)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().IndexOf(item, index);
    }
    public int IndexOf(T item)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().IndexOf(item);
    }


    public void Insert(int index, T item)
    {

        GraphVariableBase newElement = new GraphVariableBase();
        newElement.typeName = innerVariable.arrayType;
        newElement.variableID = System.Guid.NewGuid().ToString();
        newElement.SetValue(item);
        sourceArray.Insert(index,newElement);
    }

    public void InsertRange(int index, IEnumerable<T> collection)
    {
        int insertionIndex = index;
        foreach(var item in collection)
        {
            Insert(insertionIndex, item);
            insertionIndex++;
        }
    }
    public int LastIndexOf(T item)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().LastIndexOf(item);
    }
    public int LastIndexOf(T item, int index)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().LastIndexOf(item, index);
    }
    public int LastIndexOf(T item, int index, int count)
    {
        return sourceArray.Select(x => (T)x.Value()).ToList().LastIndexOf(item, index, count);
    }
    public bool Remove(T item)
    {
        if (item == null)
            return false;
        var removeItem = sourceArray.Find(x => item.Equals(x.Value()));

        if (removeItem == null)
            return false;

        return sourceArray.Remove(removeItem);
    }

    /*public int RemoveAll(Predicate<T> match)
{

}*/
    public void RemoveRange(int index, int count)
    {
        sourceArray.RemoveRange(index, count);
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return sourceArray.Select(x => (T)x.Value()).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return sourceArray.Select(x => (T)x.Value()).GetEnumerator();
    }
    /*
public void Reverse(int index, int count);
public void Reverse();
public void Sort(Comparison<T> comparison);
public void Sort(int index, int count, IComparer<T> comparer);
public void Sort();
public void Sort(IComparer<T> comparer);
public T[] ToArray();
public void TrimExcess();
public bool TrueForAll(Predicate<T> match);*/


}
