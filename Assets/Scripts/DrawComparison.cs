using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DrawComparison
{
    public static AnimationCurve Curve;

    private static Texture2D Shrink(this Texture2D texture, int width, int height, int x, int y, int fullWidth, int fullHeight)
    {
        if (fullWidth - x <= 0 || fullHeight - y <= 0)
        {
            return null;
        }
        RenderTexture rt = RenderTexture.GetTemporary(fullWidth, fullHeight);
        rt.Create();
        rt.filterMode = FilterMode.Point;
        texture.filterMode = FilterMode.Point;
        Graphics.Blit(texture, rt);

        Texture2D smallerTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        smallerTex.filterMode = FilterMode.Point;
        var old_rt = RenderTexture.active;
        RenderTexture.active = rt;

        smallerTex.ReadPixels(new Rect(x, y, width, height), 0, 0);
        smallerTex.Apply();

        RenderTexture.active = old_rt;

        return smallerTex;
    }

    private static void FindOpaquePixels(ref Color[] pixels, int width, int height, System.Action<int> callback, int mode = 0) // X, XBack, Y, YBack
    {
        bool isX = Mathf.FloorToInt(mode / 2) == 0;
        bool isBackwards = (mode % 2) == 1;

        for (int x = (isBackwards ? 1 : 0) * ((isX ? width : height) - 1); isBackwards ? (x >= 0) : (x < (isX ? width : height)); x += (isBackwards ? -1 : 1))
        {
            for (int y = 0; y < (isX ? height : width); y ++)
            {
                var i = (isX ? x : y) + width * (isX ? y : x);

                if (pixels[i].a > 0)
                {
                    callback.Invoke(x);
                    return;
                }
            }
        }

        callback.Invoke(-1);
    }

    private static Texture2D ShrinkToFit(this Texture2D texture)
    {
        var pixels1 = texture.GetPixels();

        var boundsRect = new Rect(0, 0, texture.width, texture.height);
        FindOpaquePixels(ref pixels1, texture.width, texture.height, (x) =>
        {
            boundsRect.x = x;
        }, 0);

        if (boundsRect.x == -1) // empty texture
        {
            return null;
        }

        FindOpaquePixels(ref pixels1, texture.width, texture.height, (x) =>
        {
            boundsRect.xMax = x;
        }, 1);
        FindOpaquePixels(ref pixels1, texture.width, texture.height, (x) =>
        {
            boundsRect.yMax = x;
        }, 3);
        FindOpaquePixels(ref pixels1, texture.width, texture.height, (x) =>
        {
            boundsRect.yMin = x;
        }, 2);

        return Shrink(texture, (int) boundsRect.width, (int)boundsRect.height, (int)boundsRect.x, (int)boundsRect.y, texture.width, texture.height);
    }
    private class QuadrantResultsInfo
    {
        public Dictionary<Color, float> inkUsed = new Dictionary<Color, float>();
        public Dictionary<Color, float> referenceInk = new Dictionary<Color, float>();

        public int totalOwnInk;
        public int totalReferenceInk;
        public float matchingPositionsPercent;
        public float matchingColorsPercent;
    }

    private static QuadrantResultsInfo QuadrantInkAccuracy(ref Color[] a, ref Color[] b, int x, int y, int width, int height, int fullwidth)
    {
        var result = new QuadrantResultsInfo();

        var divisor = ((width-x) * (height-y));

        for (int x1 = x; x1 < width; x1++)
        {
            for (int y1 = y; y1 < height; y1++)
            {
                var i = x1 + fullwidth * y1;

                var aPixel = a[i];
                var bPixel = b[i];

                if (aPixel.r + aPixel.g + aPixel.b == 3)
                {
                    aPixel.a = 0;
                }
                else if (aPixel.a > 0)
                {
                    aPixel.a = 1;
                }

                if (bPixel.r + bPixel.g + bPixel.b == 3)
                {
                    bPixel.a = 0;
                }
                else if (bPixel.a > 0)
                {
                    bPixel.a = 1;
                }


                if (aPixel.a > 0)
                {
                    result.totalOwnInk++;

                    if (result.inkUsed.ContainsKey(aPixel))
                    {
                        result.inkUsed[aPixel]++;
                    }
                    else
                    {
                        result.inkUsed.Add(aPixel, 1);
                    }
                }
                if (bPixel.a > 0)
                {
                    result.totalReferenceInk++;

                    if (result.referenceInk.ContainsKey(bPixel))
                    {
                        result.referenceInk[bPixel]++;
                    }
                    else
                    {
                        result.referenceInk.Add(bPixel, 1);
                    }
                }

                bool sameColor = aPixel == bPixel;

                if (aPixel.a > 0 && bPixel.a > 0)
                {
                    result.matchingPositionsPercent += 1;

                    if (sameColor)
                    {
                        result.matchingColorsPercent += 1;
                    }
                } else if (aPixel.a != 0 || bPixel.a != 0)
                {
                    result.matchingColorsPercent = Mathf.Max(0, result.matchingColorsPercent - 0.25f);
                    result.matchingPositionsPercent = Mathf.Max(0, result.matchingPositionsPercent - 0.25f);
                }
            }
        }

        if (result.totalReferenceInk > 0)
        {
            result.matchingColorsPercent = Mathf.Clamp01(result.matchingColorsPercent * 1.75f / result.totalReferenceInk);
            result.matchingPositionsPercent = Mathf.Clamp01(result.matchingPositionsPercent * 1.75f / result.totalReferenceInk);
        } else if (result.totalOwnInk > 0)
        {
            result.matchingColorsPercent = -result.totalOwnInk/divisor;
            result.matchingPositionsPercent = -result.totalOwnInk/divisor;
        }


        return result;
    }

    public static float CompareImages(Texture2D drawn, Texture2D wanted, out Texture2D drawnSeen, out Texture2D referenceUsed)
    {
        var smallerTex = drawn.Shrink(64, 64, 0, 0, 64, 64);//.ShrinkToFit();
        var smallerTex2 = smallerTex.ShrinkToFit();
        drawnSeen = new Texture2D(64, 64);
        referenceUsed = new Texture2D(64, 64);
        if (smallerTex2 == null)
        {
            return -1;
        }
        var wantedTex = wanted;//.ShrinkToFit();
        if (wantedTex == null)
        {
            return -1;
        }
        //smallerTex = smallerTex.Shrink(wantedTex.width, wantedTex.height, 0, 0, wantedTex.width, wantedTex.height);
        drawnSeen = smallerTex;
        referenceUsed = wantedTex;

        var pixels1 = smallerTex.GetPixels();
        var pixels2 = wantedTex.GetPixels();

        float quadrants = 16.0f;
        float totalQuadrants = quadrants * quadrants;

        float matchingColorsPercentMerged = 0;
        float matchingPositionsPercentMerged = 0;

        float totalOwnInk = 0;
        float totalReferenceInk = 0;

        Dictionary<ValidColors, float> totalInkA = new Dictionary<ValidColors, float>();
        Dictionary<ValidColors, float> totalInkB = new Dictionary<ValidColors, float>();

        for (float x = 0; x < smallerTex.width; x += smallerTex.width / quadrants)
        {
            for (float y = 0; y < smallerTex.height; y += smallerTex.height / quadrants)
            {
                var results = QuadrantInkAccuracy(ref pixels1, ref pixels2, Mathf.FloorToInt(x), Mathf.FloorToInt(y),
                    Mathf.Min(Mathf.FloorToInt(x + smallerTex.width / quadrants), smallerTex.width - 1),
                    Mathf.Min(Mathf.FloorToInt(y + smallerTex.height / quadrants), smallerTex.height - 1), smallerTex.width);

                if (results.totalReferenceInk == 0 && results.totalOwnInk == 0)
                {
                    totalQuadrants = totalQuadrants - 1;
                    continue;
                }

                matchingColorsPercentMerged += Mathf.Clamp01(results.matchingColorsPercent * 8f - 0.35f);
                matchingPositionsPercentMerged += Mathf.Clamp01(results.matchingPositionsPercent * 8f - 0.35f);

                totalOwnInk += results.totalOwnInk;
                totalReferenceInk += results.totalReferenceInk;

                foreach (var kvp in results.referenceInk)
                {
                    var col = new Color(Mathf.Floor(kvp.Key.r * 255) / 255.0f, Mathf.Floor(kvp.Key.g * 255) / 255.0f, Mathf.Floor(kvp.Key.b * 255) / 255.0f, 1);
                    var g = Guessable.colorMap.ContainsKey(col) ? Guessable.colorMap[col] : ValidColors.Black;
                    if (!totalInkB.ContainsKey(g))
                    {
                        totalInkB.Add(g, kvp.Value);
                    } else
                    {
                        totalInkB[g] += kvp.Value;
                    }
                }

                foreach (var kvp in results.inkUsed)
                {
                    var col = new Color(Mathf.Floor(kvp.Key.r * 255) / 255.0f, Mathf.Floor(kvp.Key.g * 255) / 255.0f, Mathf.Floor(kvp.Key.b * 255) / 255.0f, 1);
                    var g = Guessable.colorMap.ContainsKey(col) ? Guessable.colorMap[col] : ValidColors.Black;
                    if (!totalInkA.ContainsKey(g))
                    {
                        totalInkA.Add(g, kvp.Value);
                    }
                    else
                    {
                        totalInkA[g] += kvp.Value;
                    }
                }
            }
        }

        var inkDifference = Mathf.Abs((totalOwnInk / totalReferenceInk) - 1);
        inkDifference = Mathf.Clamp(1.1f - inkDifference, 0, 1.1f); // now inkSimilarityMultiplier

        // Debug.Log("InkSimilarityMultiplier: " + inkDifference);

        matchingColorsPercentMerged /= totalQuadrants;
        matchingPositionsPercentMerged /= totalQuadrants;

        var mergedAccuracy = Mathf.Clamp01((matchingColorsPercentMerged * 0.25f + matchingPositionsPercentMerged * 0.75f) * 3.0f);

        // Debug.Log("Merged Accuracy: " + mergedAccuracy);

        mergedAccuracy = Mathf.Clamp01(mergedAccuracy * 1.25f) * inkDifference;

        var divisor = 0.0f;

        var totalInkAccuracy = 0.0f;

        foreach (var kvp in Guessable.colorMap)
        {
            float a = 0, b = 0;

            if (totalInkA.ContainsKey(kvp.Value))
            {
                a = totalInkA[kvp.Value];
            }
            if (totalInkB.ContainsKey(kvp.Value))
            {
                b = totalInkB[kvp.Value];
            }

            if (a > 0 || b > 0)
            {
                if (b == 0)
                {
                    b = a;
                    a = 0;
                }

                float diff = Mathf.Abs(a * (1.0f/b));
                diff = 1f - Mathf.Max(0, diff);

                if (diff < 0)
                {
                    diff *= 0.5f;
                }

                totalInkAccuracy += diff;

                divisor = divisor + 1;
            }
        }

        totalInkAccuracy /= divisor;
        totalInkAccuracy = Mathf.Clamp(totalInkAccuracy, -1f, 1f);

        var totalInkDiff = ((totalOwnInk - totalReferenceInk) / totalReferenceInk);

        // Debug.Log("Total Ink Deviation: " + totalInkAccuracy);

        mergedAccuracy = Mathf.Lerp(0, mergedAccuracy, 0.5f * ((1 - totalInkAccuracy * totalInkAccuracy) + (1 - totalInkDiff * totalInkDiff)));

        // Debug.Log("Total: " + mergedAccuracy);

        mergedAccuracy = Curve.Evaluate(Mathf.Clamp01(mergedAccuracy));

        // Debug.Log("Total (Skewed): " + mergedAccuracy);

        return mergedAccuracy;
    }
}
