using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseImageMesh
{
    protected ExtImage extImage;
    public BaseImageMesh(ExtImage extImage)
    {
        this.extImage = extImage;
    }
    public virtual float alphaHitTestMinimumThreshold { get; set; }
    public abstract bool OnPopulateMesh(VertexHelper vh);
    public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
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
    protected Vector2 MapCoordinate(Vector2 local, Rect rect)
    {
        Rect rect2 = extImage.overrideSprite.rect;
        if (extImage.type == ExtImageType.Simple)
        {
            return new Vector2(local.x * rect2.width / rect.width, local.y * rect2.height / rect.height);
        }
        return local;
    }
    protected void PreserveSpriteAspectRatio(ref Rect rect, Vector2 spriteSize)
    {
        float num = spriteSize.x / spriteSize.y;
        float num2 = rect.width / rect.height;
        if (num > num2)
        {
            float height = rect.height;
            rect.height = rect.width * (1f / num);
            rect.y += (height - rect.height) * extImage.rectTransform.pivot.y;
        }
        else
        {
            float width = rect.width;
            rect.width = rect.height * num;
            rect.x += (width - rect.width) * extImage.rectTransform.pivot.x;
        }
    }
    protected Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
    {
        Rect rect = extImage.rectTransform.rect;
        for (int i = 0; i <= 1; i++)
        {
            if (rect.size[i] != 0f)
            {
                float num = adjustedRect.size[i] / rect.size[i];
                border[i] *= num;
                border[i + 2] *= num;
            }

            float num2 = border[i] + border[i + 2];
            if (adjustedRect.size[i] < num2 && num2 != 0f)
            {
                float num = adjustedRect.size[i] / num2;
                border[i] *= num;
                border[i + 2] *= num;
            }
        }

        return border;
    }

    protected static void AddQuad(VertexHelper vertexHelper, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
    {
        int currentVertCount = vertexHelper.currentVertCount;
        for (int i = 0; i < 4; i++)
        {
            vertexHelper.AddVert(quadPositions[i], color, quadUVs[i]);
        }

        vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
        vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
    }

    protected static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
    {
        int currentVertCount = vertexHelper.currentVertCount;
        vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
        vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
        vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
        vertexHelper.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
        vertexHelper.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
    }
}
