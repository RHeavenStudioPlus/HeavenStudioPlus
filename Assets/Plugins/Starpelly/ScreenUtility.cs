using UnityEngine;

public static class ScreenUtility
{
    public static Camera camera;

    public static float Left
    {
        get
        {
            if (camera)
                return camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;

            if (Camera.main)
                return Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;

            return 0.0f;
        }
    }

    public static float Right
    {
        get
        {
            if (camera)
                return camera.ViewportToWorldPoint(new Vector3(1.0f, 0f, 0f)).x;

            if (Camera.main)
                return Camera.main.ViewportToWorldPoint(new Vector3(1.0f, 0f, 0f)).x;

            return 0.0f;
        }
    }

    public static float Top
    {
        get
        {
            if (camera)
                return camera.ViewportToWorldPoint(new Vector3(0f, 1.0f, 0f)).y;

            if (Camera.main)
                return Camera.main.ViewportToWorldPoint(new Vector3(0f, 1.0f, 0f)).y;

            return 0.0f;
        }
    }

    public static float Bottom
    {
        get
        {
            if (camera)
                return camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

            if (Camera.main)
                return Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

            return 0.0f;
        }
    }

    public static Vector3 Center
    {
        get
        {
            if (camera)
                return camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

            if (Camera.main)
                return Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));

            return Vector3.zero;
        }
    }

    public static bool ScreenContainsPoint(Vector3 worldPosition)
    {
        return Camera.main.rect.Contains(Camera.main.WorldToViewportPoint(worldPosition));
    }

    public static void ConstrainCamera(Camera camera, Bounds bounds)
    {
        float left = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).x;
        float right = camera.ViewportToWorldPoint(new Vector3(1.0f, 0f, 0f)).x;
        float top = camera.ViewportToWorldPoint(new Vector3(0f, 1.0f, 0f)).y;
        float bottom = camera.ViewportToWorldPoint(new Vector3(0f, 0f, 0f)).y;

        if (top > bounds.max.y)
        {
            float topDiff = bounds.max.y - top;
            camera.transform.position += new Vector3(0, topDiff, 0);
        }
        else if (bottom < bounds.min.y)
        {
            float botDiff = bounds.min.y - bottom;
            camera.transform.position += new Vector3(0, botDiff, 0);
        }

        if (right > bounds.max.x)
        {
            float rightDiff = bounds.max.x - right;
            camera.transform.position += new Vector3(rightDiff, 0, 0);
        }
        else if (left < bounds.min.x)
        {
            float leftDiff = bounds.min.x - left;
            camera.transform.position += new Vector3(leftDiff, 0, 0);
        }
    }
}