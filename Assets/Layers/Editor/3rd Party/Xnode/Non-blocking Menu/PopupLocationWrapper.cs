using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public enum PopupLocationWrapper
    {
        Below,
        Above,
        Left,
        Right,
    }

    public static class PopupLocationWrapperExtensions
    {
        public static System.Type BaseType
        {
            get
            {
                return typeof(EditorWindow).Assembly.GetType("UnityEditor.PopupLocation");
            }
        }

        public static object ToBaseType(this PopupLocationWrapper value)
        {
            System.Type popupType = BaseType;
            object result = System.Enum.Parse(popupType, value.ToString());
            return result;
        }
    }
}