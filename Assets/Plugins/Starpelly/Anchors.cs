using UnityEngine;

namespace Starpelly
{
    public class Anchors
    {
        //------------Top-------------------
        public static void TopLeft(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(0, 1);
            uitransform.anchorMax = new Vector2(0, 1);
            uitransform.pivot = new Vector2(0, 1);
        }

        public static void TopMiddle(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(0.5f, 1);
            uitransform.anchorMax = new Vector2(0.5f, 1);
            uitransform.pivot = new Vector2(0.5f, 1);
        }


        public static void TopRight(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(1, 1);
            uitransform.anchorMax = new Vector2(1, 1);
            uitransform.pivot = new Vector2(1, 1);
        }

        //------------Middle-------------------
        public static void MiddleLeft(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(0, 0.5f);
            uitransform.anchorMax = new Vector2(0, 0.5f);
            uitransform.pivot = new Vector2(0, 0.5f);
        }

        public static void Mmiddle(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(0.5f, 0.5f);
            uitransform.anchorMax = new Vector2(0.5f, 0.5f);
            uitransform.pivot = new Vector2(0.5f, 0.5f);
        }

        public static void MiddleRight(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(1, 0.5f);
            uitransform.anchorMax = new Vector2(1, 0.5f);
            uitransform.pivot = new Vector2(1, 0.5f);
        }

        //------------Bottom-------------------
        public static void BottomLeft(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(0, 0);
            uitransform.anchorMax = new Vector2(0, 0);
            uitransform.pivot = new Vector2(0, 0);
        }

        public static void BottomMiddle(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(0.5f, 0);
            uitransform.anchorMax = new Vector2(0.5f, 0);
            uitransform.pivot = new Vector2(0.5f, 0);
        }

        public static void BottomRight(GameObject uiObject)
        {
            RectTransform uitransform = uiObject.GetComponent<RectTransform>();

            uitransform.anchorMin = new Vector2(1, 0);
            uitransform.anchorMax = new Vector2(1, 0);
            uitransform.pivot = new Vector2(1, 0);
        }
    }

    public static class RectTransformExtensions
    {
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }
}