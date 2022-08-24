using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier
{
    public static Vector2 Cubic(float t, Vector2 p0, Vector2 p1, Vector2 p2,
        Vector2 p3)
    {
        float t_ = 1.0f - t;
        float t_2 = t_ * t_;
        float t_3 = t_ * t_2;

        float t2 = t * t;
        float t3 = t * t2;
        return t_3 * p0 + 3.0f * t_2 * t * p1 + 3.0f * t_ * t2 * p2 + t3 * p3;
    }

    public static Vector2 CubicDeriv(float t, Vector2 p0, Vector2 p1, 
        Vector2 p2, Vector2 p3)
    {
        float t2 = t * t;
        float t3 = t * t2;
        float c0 = -3.0f * t2 + 6.0f * t - 3.0f;
        float c1 = 3.0f * t2 - 4.0f * t + 1.0f;
        float c2 = -9.0f * t2 + 6.0f * t;
        float c3 = 3.0f * t2;

        return c0 * p0 + c1 * p1 + c2 * p2 + c3 * p3;
    }

    // p must have 3n elements (min 6)
    public static Vector2 CubicLoop(float t, Vector2[] p)
    {
        if (p.Length < 6 || p.Length % 3 != 0)
            return Vector2.zero;
        int pathLength = p.Length / 3;
        float tMod = (t % pathLength + pathLength) % pathLength;
        int curve = (int) tMod;
        int index = 3 * curve;
        int len = p.Length;
        Vector2 p0 = p[index % len];
        Vector2 p1 = p[(index + 1) % len];
        Vector2 p2 = p[(index + 2) % len];
        Vector2 p3 = p[(index + 3) % len];
        return Cubic(tMod - curve, p0, p1, p2, p3);
    }

    public static int LoopLength(Vector2[] p)
    {
        return p.Length / 3;
    }

    public class Window
    {
        float srcX, srcY, srcW, srcH;
        float dstX, dstY, dstW, dstH;
        public Window(float srcX, float srcY, float srcW, float srcH,
            float dstX, float dstY, float dstW, float dstH)
        {
            this.srcX = srcX;
            this.srcY = srcY;
            this.srcW = srcW;
            this.srcH = srcH;
            this.dstX = dstX;
            this.dstY = dstY;
            this.dstW = dstW;
            this.dstH = dstH;
        }

        public Vector2[] TransformPoints(Vector2[] p)
        {
            Vector2[] ret = new Vector2[p.Length];
            for (int i = 0; i < p.Length; i++)
            {
                Vector2 v = p[i];
                v -= new Vector2(srcX, srcY);
                v *= new Vector2(dstW / srcW, dstH / srcH);
                v += new Vector2(dstX, dstY);
                ret[i] = v;
            }
            return ret;
        }
    }
}
