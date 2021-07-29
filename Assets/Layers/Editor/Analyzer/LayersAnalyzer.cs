using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LayersAnalyzer
{
    public float height = 0;

    protected LayersAnalyzer(float height)
    {
        this.height = height;
    }

    public virtual void ReceiveEvent(LayersAnalyzerEvent levent)
    {

    }

    public virtual void DrawData(Rect position)
    {

    }

}
