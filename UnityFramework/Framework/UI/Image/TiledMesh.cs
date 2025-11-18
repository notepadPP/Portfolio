using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class TiledMesh : BaseImageMesh
{
    public TiledMesh(ExtImage extImage) : base(extImage)
    {
    }
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (alphaHitTestMinimumThreshold <= 0f)
        {
            return true;
        }

        if (alphaHitTestMinimumThreshold > 1f)
        {
            return false;
        }

        if (extImage.overrideSprite == null)
        {
            return true;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(extImage.rectTransform, screenPoint, eventCamera, out var localPoint))
        {
            return false;
        }

        Rect rect = extImage.GetPixelAdjustedRect();
        if (extImage.preserveAspect)
        {
            PreserveSpriteAspectRatio(ref rect, new Vector2(extImage.overrideSprite.texture.width, extImage.overrideSprite.texture.height));
        }

        localPoint.x += extImage.rectTransform.pivot.x * rect.width;
        localPoint.y += extImage.rectTransform.pivot.y * rect.height;
        localPoint = MapCoordinate(localPoint, rect);
        float u = localPoint.x / (float)extImage.overrideSprite.texture.width;
        float v = localPoint.y / (float)extImage.overrideSprite.texture.height;
        try
        {
            return extImage.overrideSprite.texture.GetPixelBilinear(u, v).a >= alphaHitTestMinimumThreshold;
        }
        catch (UnityException ex)
        {
            Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + ex.Message + " Also make sure to disable sprite packing for this sprite.", extImage);
            return true;
        }
    }

    public override bool OnPopulateMesh(VertexHelper vh)
    {
        GenerateTiledSprite(vh);
        return true;
    }

    private void GenerateTiledSprite(VertexHelper toFill)
    {
        Vector4 vector;
        Vector4 vector2;
        Vector2 vector4;
        Vector4 vector3;
        if (extImage.overrideSprite != null)
        {
            vector = DataUtility.GetOuterUV(extImage.overrideSprite);
            vector2 = DataUtility.GetInnerUV(extImage.overrideSprite);
            vector3 = extImage.overrideSprite.border;
            vector4 = extImage.overrideSprite.rect.size;
        }
        else
        {
            vector = Vector4.zero;
            vector2 = Vector4.zero;
            vector3 = Vector4.zero;
            vector4 = Vector2.one * 100f;
        }

        Rect pixelAdjustedRect = extImage.GetPixelAdjustedRect();
        float num = (vector4.x - vector3.x - vector3.z) / extImage.pixelsPerUnitMultiplier;
        float num2 = (vector4.y - vector3.y - vector3.w) / extImage.pixelsPerUnitMultiplier;
        vector3 = GetAdjustedBorders(vector3 / extImage.pixelsPerUnitMultiplier, pixelAdjustedRect);
        Vector2 vector5 = new Vector2(vector2.x, vector2.y);
        Vector2 vector6 = new Vector2(vector2.z, vector2.w);
        float x = vector3.x;
        float num3 = pixelAdjustedRect.width - vector3.z;
        float y = vector3.y;
        float num4 = pixelAdjustedRect.height - vector3.w;
        toFill.Clear();
        Vector2 uvMax = vector6;
        if (num <= 0f)
        {
            num = num3 - x;
        }

        if (num2 <= 0f)
        {
            num2 = num4 - y;
        }

        if (extImage.overrideSprite != null && (extImage.hasBorder || extImage.overrideSprite.packed || (extImage.overrideSprite.texture != null && extImage.overrideSprite.texture.wrapMode != 0)))
        {
            long num5 = 0L;
            long num6 = 0L;
            if (extImage.fillCenter)
            {
                num5 = (long)Math.Ceiling((num3 - x) / num);
                num6 = (long)Math.Ceiling((num4 - y) / num2);
                double num7 = 0.0;
                num7 = ((!extImage.hasBorder) ? ((double)(num5 * num6) * 4.0) : (((double)num5 + 2.0) * ((double)num6 + 2.0) * 4.0));
                if (num7 > 65000.0)
                {
                    Debug.LogError("Too many sprite tiles on Image \"" + extImage.name + "\". The tile size will be increased. To remove the limit on the number of tiles, set the Wrap mode to Repeat in the Image Import Settings", extImage);
                    double num8 = 16250.0;
                    double num9 = ((!extImage.hasBorder) ? ((double)num5 / (double)num6) : (((double)num5 + 2.0) / ((double)num6 + 2.0)));
                    double num10 = Math.Sqrt(num8 / num9);
                    double num11 = num10 * num9;
                    if (extImage.hasBorder)
                    {
                        num10 -= 2.0;
                        num11 -= 2.0;
                    }

                    num5 = (long)Math.Floor(num10);
                    num6 = (long)Math.Floor(num11);
                    num = (num3 - x) / (float)num5;
                    num2 = (num4 - y) / (float)num6;
                }
            }
            else if (extImage.hasBorder)
            {
                num5 = (long)Math.Ceiling((num3 - x) / num);
                num6 = (long)Math.Ceiling((num4 - y) / num2);
                double num12 = ((double)(num6 + num5) + 2.0) * 2.0 * 4.0;
                if (num12 > 65000.0)
                {
                    Debug.LogError("Too many sprite tiles on Image \"" + extImage.name + "\". The tile size will be increased. To remove the limit on the number of tiles, set the Wrap mode to Repeat in the Image Import Settings", extImage);
                    double num13 = 16250.0;
                    double num14 = (double)num5 / (double)num6;
                    double num15 = (num13 - 4.0) / (2.0 * (1.0 + num14));
                    double d = num15 * num14;
                    num5 = (long)Math.Floor(num15);
                    num6 = (long)Math.Floor(d);
                    num = (num3 - x) / (float)num5;
                    num2 = (num4 - y) / (float)num6;
                }
            }
            else
            {
                num6 = (num5 = 0L);
            }

            if (extImage.fillCenter)
            {
                for (long num16 = 0L; num16 < num6; num16++)
                {
                    float num17 = y + (float)num16 * num2;
                    float num18 = y + (float)(num16 + 1) * num2;
                    if (num18 > num4)
                    {
                        uvMax.y = vector5.y + (vector6.y - vector5.y) * (num4 - num17) / (num18 - num17);
                        num18 = num4;
                    }

                    uvMax.x = vector6.x;
                    for (long num19 = 0L; num19 < num5; num19++)
                    {
                        float num20 = x + (float)num19 * num;
                        float num21 = x + (float)(num19 + 1) * num;
                        if (num21 > num3)
                        {
                            uvMax.x = vector5.x + (vector6.x - vector5.x) * (num3 - num20) / (num21 - num20);
                            num21 = num3;
                        }

                        AddQuad(toFill, new Vector2(num20, num17) + pixelAdjustedRect.position, new Vector2(num21, num18) + pixelAdjustedRect.position, extImage.color, vector5, uvMax);
                    }
                }
            }

            if (!extImage.hasBorder)
            {
                return;
            }

            uvMax = vector6;
            for (long num22 = 0L; num22 < num6; num22++)
            {
                float num23 = y + (float)num22 * num2;
                float num24 = y + (float)(num22 + 1) * num2;
                if (num24 > num4)
                {
                    uvMax.y = vector5.y + (vector6.y - vector5.y) * (num4 - num23) / (num24 - num23);
                    num24 = num4;
                }

                AddQuad(toFill, new Vector2(0f, num23) + pixelAdjustedRect.position, new Vector2(x, num24) + pixelAdjustedRect.position, extImage.color, new Vector2(vector.x, vector5.y), new Vector2(vector5.x, uvMax.y));
                AddQuad(toFill, new Vector2(num3, num23) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num24) + pixelAdjustedRect.position, extImage.color, new Vector2(vector6.x, vector5.y), new Vector2(vector.z, uvMax.y));
            }

            uvMax = vector6;
            for (long num25 = 0L; num25 < num5; num25++)
            {
                float num26 = x + (float)num25 * num;
                float num27 = x + (float)(num25 + 1) * num;
                if (num27 > num3)
                {
                    uvMax.x = vector5.x + (vector6.x - vector5.x) * (num3 - num26) / (num27 - num26);
                    num27 = num3;
                }

                AddQuad(toFill, new Vector2(num26, 0f) + pixelAdjustedRect.position, new Vector2(num27, y) + pixelAdjustedRect.position, extImage.color, new Vector2(vector5.x, vector.y), new Vector2(uvMax.x, vector5.y));
                AddQuad(toFill, new Vector2(num26, num4) + pixelAdjustedRect.position, new Vector2(num27, pixelAdjustedRect.height) + pixelAdjustedRect.position, extImage.color, new Vector2(vector5.x, vector6.y), new Vector2(uvMax.x, vector.w));
            }

            AddQuad(toFill, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, extImage.color, new Vector2(vector.x, vector.y), new Vector2(vector5.x, vector5.y));
            AddQuad(toFill, new Vector2(num3, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, extImage.color, new Vector2(vector6.x, vector.y), new Vector2(vector.z, vector5.y));
            AddQuad(toFill, new Vector2(0f, num4) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, extImage.color, new Vector2(vector.x, vector6.y), new Vector2(vector5.x, vector.w));
            AddQuad(toFill, new Vector2(num3, num4) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, extImage.color, new Vector2(vector6.x, vector6.y), new Vector2(vector.z, vector.w));
        }
        else
        {
            Vector2 b = new Vector2((num3 - x) / num, (num4 - y) / num2);
            if (extImage.fillCenter)
            {
                AddQuad(toFill, new Vector2(x, y) + pixelAdjustedRect.position, new Vector2(num3, num4) + pixelAdjustedRect.position, extImage.color, Vector2.Scale(vector5, b), Vector2.Scale(vector6, b));
            }
        }
    }
}
