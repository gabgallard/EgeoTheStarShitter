using System.Collections.Generic;
using System.Linq;
using ABXY.Layers.Editor.Curve_Editor.Wrappers;
using UnityEditor;
using UnityEngine;

namespace ABXY.Layers.Editor.Curve_Editor
{
    public class CurveList
    {

        private Dictionary<int, CurveContainer> id2Curve = new Dictionary<int, CurveContainer>();
        private Dictionary<CurveContainer, int> curve2ID = new Dictionary<CurveContainer, int>();
        private Dictionary<string, CurveContainer> sp2Curve = new Dictionary<string, CurveContainer>();
        List<CurveContainer> curves = new List<CurveContainer>();


        public void Add(SerializedProperty curveProperty, string legendString, Color curveColor)
        {
            CurveContainer curveContainer = new CurveContainer(curveProperty, legendString, curveColor);
            curves.Add(curveContainer);
            id2Curve.Add(curveContainer.id, curveContainer);
            curve2ID.Add(curveContainer, curveContainer.id);
            sp2Curve.Add(curveProperty.propertyPath, curveContainer);
        }

        public void Remove(SerializedProperty curveProperty)
        {
            CurveContainer curveContainer = curves.Find(x => x.curveSP == curveProperty);
            if (curveContainer == null)
                return;

            curves.RemoveAll(x => x.curveSP == curveProperty);
            id2Curve.Remove(curveContainer.id);
            curve2ID.Remove(curveContainer);
            sp2Curve.Remove(curveProperty.propertyPath);
        }

        public bool[] GetSelectionArray()
        {
            return curves.Select(x => x.selected).ToArray();
        }

        public void SetSelectionArray(bool[] selections)
        {
            if (selections.Length != curves.Count)
            {
                Debug.LogError("Selections array is a different length than the number of curves");
                return;
            }

            for (int index = 0; index < selections.Length; index++)
            {
                curves[index].selected = selections[index];
            }
        }

        public CurveContainer[] GetCurves()
        {
            return curves.ToArray();
        }

        public CurveContainer GetCurve(SerializedProperty curveProp)
        {
            CurveContainer container = null;
            sp2Curve.TryGetValue(curveProp.propertyPath, out container);
            return container;
        }

        public void UpdateCurves()
        {
            foreach (CurveContainer curve in curves)
                curve.UpdateCurve();
        }

        public void ApplyCurves()
        {
            foreach (CurveContainer curve in curves)
                curve.ApplyCurve();
        }

        public class CurveContainer
        {
            public int id
            {
                get
                {
                    return curveWrapper.id;
                }
            }
            public SerializedProperty curveSP { get; private set; }
            public string legendString { get; set; }

            public Color curveColor;

            public CurveWrapperWrapper curveWrapper { get; private set; }

            public bool selected { get { return !curveWrapper.hidden; } set { curveWrapper.hidden = !value; } }

            public bool readOnly = false;

            public bool hidden = false;

            /// <summary>
            /// Clamps the curve to these values in the vertical axis. Infinity and negative infinity are allowable values
            /// </summary>
            public Vector2 verticalRange = new Vector2(0f, 1f);

            public CurveContainer(SerializedProperty curveSP, string legendString, Color curveColor)
            {
                this.curveSP = curveSP;
                this.legendString = legendString;
                this.curveColor = curveColor;
                UpdateCurve();


            }

            public void UpdateCurve()
            {
                curveWrapper = new CurveWrapperWrapper();
                curveWrapper.id = Random.Range(0, int.MaxValue);
                curveWrapper.groupId = -1;
                curveWrapper.color = curveColor;
                curveWrapper.hidden = hidden;
                curveWrapper.readOnly = readOnly;
                curveWrapper.renderer = new NormalCurveRendererWrapper(curveSP.animationCurveValue);
                curveWrapper.renderer.SetCustomRange(verticalRange.x, verticalRange.y);
                //TODO: Need to figure out if this is necessary
                //curveWrapper.getAxisUiScalarsCallback = () => {};
                curveWrapper.getAxisUiScalarsCallback = GetAxisScalars;
                curveWrapper.useScalingInKeyEditor = true;
            }

            public Vector2 GetAxisScalars()
            {
                return new Vector2(200f, 1);
            }

            public void ApplyCurve()
            {
                curveSP.animationCurveValue = curveWrapper.curve;
            }
        }
    }
}
