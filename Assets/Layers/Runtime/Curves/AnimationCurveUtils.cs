using System.Collections.Generic;
using UnityEngine;

namespace ABXY.Layers.Runtime.Curves
{
    public static class AnimationCurveUtils
    {
        public static AnimationCurve HorizontalLine(float value)
        {
            return new AnimationCurve(new Keyframe(0f, value));
        }

        public static AnimationCurve Line(Vector2 start, Vector2 end)
        {
            Vector2 angleVector = end - start;
            float slope = angleVector.y / angleVector.x;
            return new AnimationCurve(new Keyframe(start.x,start.y,0f, slope), new Keyframe(end.x, end.y, slope, 0f));
        }

        public static AnimationCurve LogarithmicCurve(float startTime, float endTime, float logarithmBase)
        {
            List<Keyframe> keysList = new List<Keyframe>();
            float timeStep = 2f;

            // bumping up the startTime to deal with the limit
            startTime = Mathf.Max(startTime, 0.0001f);

            for (float currentTime = startTime; currentTime < endTime; currentTime *= timeStep)
            {
                keysList.Add(MakeLogKeyframe(startTime, currentTime, logarithmBase));
            }
            //Making last keyframe
            keysList.Add(MakeLogKeyframe(startTime, endTime, logarithmBase));
            return new AnimationCurve(keysList.ToArray());
        }

        private static Keyframe MakeLogKeyframe(float startTime, float currentTime, float logarithmBase)
        {
            float keyValue = CalcLogValue(currentTime, startTime, logarithmBase);
            float delta = currentTime / 50f;
            float after = CalcLogValue(currentTime + delta, startTime, logarithmBase);
            float before = CalcLogValue(currentTime - delta, startTime, logarithmBase);
            float slope = (after-before) / (delta * 2);
            return new Keyframe(currentTime, keyValue, slope, slope);
        }

        private static float CalcLogValue(float distance, float minDistance, float logBase)
        {
            if ((distance > minDistance) && (logBase != 1.0f))
            {
                distance = ((distance - minDistance) * logBase) + minDistance;
            }

            //preventing infinities due to the limit
            if (distance < 0.000001f)
                distance = 0.000001f;
            return minDistance / distance;
        }
    }
}
