using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public static class SelectionModeWrapperUtils 
    {
        private static System.Type type { get { return typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SelectionMode");  } }
        public static object ToNativeSelectionMode(SelectionModeWrapper selectionMode)
        {
            return System.Convert.ChangeType((int)selectionMode, type);
        }

        public static SelectionModeWrapper FromNativeSelectionMode(object selectionMode)
        {
            return (SelectionModeWrapper)System.Convert.ChangeType(selectionMode, typeof(int));
        }
    }

    public enum SelectionModeWrapper
    {
        None = 0,
        Selected = 1,
        SemiSelected = 2
    }
}