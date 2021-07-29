using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor.Wrappers
{
    public class TimeFormatWrapperUtils
    {
        private static System.Type type { get { return typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TimeArea.TimeFormat"); } }
        public static object ToNativeTimeFormat(TimeFormatWrapper selectionMode)
        {
            return System.Convert.ChangeType((int)selectionMode, type);
        }

        public static TimeFormatWrapper FromNativeTimeFormat(object selectionMode)
        {
            return (TimeFormatWrapper)System.Convert.ChangeType(selectionMode, typeof(int));
        }
    }

    public enum TimeFormatWrapper
    {
        None, // Unformatted time
        TimeFrame, // Time:Frame
        Frame // Integer frame
    }
}