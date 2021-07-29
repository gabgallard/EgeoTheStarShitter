using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.ThirdParty.Xnode
{
    public static class PopupLocationHelperWrapper
    {
    


        public static Rect GetDropDownRect(Rect buttonRect, Vector2 size, PopupLocationWrapper[] locationPriorityOrder)
        {
            foreach(PopupLocationWrapper location in locationPriorityOrder)
            {
                if (CanFit(buttonRect, size, location))
                {
                    return GetRect(buttonRect, size, location);
                }
            }
            return new Rect(0, 0, 0, 0);
        }

        private static Rect GetRect(Rect buttonRect, Vector2 size, PopupLocationWrapper location)
        {
            Rect targetRect = new Rect(0, 0, 0, 0);
            switch (location)
            {
                case PopupLocationWrapper.Below:
                    targetRect = new Rect(buttonRect.x, buttonRect.y + buttonRect.height, size.x, size.y);
                    break;
                case PopupLocationWrapper.Above:

                    targetRect = new Rect(buttonRect.x, buttonRect.y - size.y, size.x, size.y);
                    break;
                case PopupLocationWrapper.Left:
                    targetRect = new Rect(buttonRect.x - size.x, buttonRect.y, size.x, size.y);
                    break;
                case PopupLocationWrapper.Right:
                    targetRect = new Rect(buttonRect.x + buttonRect.width, buttonRect.y , size.x, size.y);
                    break;
            }

            Rect newTargetRect = FitRectToScreen(targetRect, false, true);
            newTargetRect.width = targetRect.width;
            newTargetRect.height = targetRect.height;
            return newTargetRect;
        }

        private static Rect FitRectToScreen(Rect defaultRect, bool forceCompletelyVisible, bool useMouseScreen)
        {
            System.Type containerWindowType = typeof(EditorWindow).Assembly.GetType("UnityEditor.ContainerWindow");
            MethodInfo method = containerWindowType.GetMethod("FitRectToScreen", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            return (Rect)method.Invoke(null, new object[] { defaultRect, forceCompletelyVisible, useMouseScreen });
        }

        private static Rect FitWithin (Rect outer, Rect inner)
        {
            Vector2 horizontalDimension = FitWithin(new Vector2(outer.x, outer.x + outer.width), new Vector2(inner.x, inner.x + inner.width));
            Vector2 verticalDimension = FitWithin(new Vector2(outer.y, outer.y + outer.height), new Vector2(inner.y, inner.y + inner.height));
            return new Rect(horizontalDimension.x, verticalDimension.x, horizontalDimension.y - horizontalDimension.x, verticalDimension.y - verticalDimension.x);
        }

        private static Vector2 FitWithin(Vector2 larger, Vector2 smaller)
        {
            if (larger.x > smaller.x)
            {
                float difference = larger.x - smaller.x;
                smaller.x += difference;
                smaller.y += difference;
            }

            if (larger.y < smaller.y)
            {
                float difference = smaller.y - larger.y;
                smaller.x -= difference;
                smaller.y -= difference;
            }
            return smaller;
        }

        private static bool CanFit(Rect buttonRect, Vector2 size, PopupLocationWrapper location)
        {
            Rect screenRect = FitRectToScreen(new Rect(float.MinValue / 2f, float.MinValue / 2f, float.MaxValue, float.MaxValue), true, true);
            switch (location)
            {
                case PopupLocationWrapper.Below:
                    float bottomDistance = screenRect.height + screenRect.y - buttonRect.y - buttonRect.height;
                    return bottomDistance > size.y;
                case PopupLocationWrapper.Above:
                    float topDistance =  buttonRect.y - screenRect.y;
                    return topDistance > size.y;
                case PopupLocationWrapper.Left:
                    float leftDistance = buttonRect.x - screenRect.x;
                    return leftDistance > size.x ;
                case PopupLocationWrapper.Right:
                    float rightDistance = screenRect.x + screenRect.width - buttonRect.x - buttonRect.width;
                    return rightDistance > size.x;
            }
            return false;
        }
    }
}
