using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class SlicedFillMesh : BaseImageMesh
{
    private static readonly Vector3[] s_Vertices = new Vector3[4];
    private static readonly Vector2[] s_UVs = new Vector2[4];
    private static readonly Vector2[] s_SlicedVertices = new Vector2[4];
    private static readonly Vector2[] s_SlicedUVs = new Vector2[4];

    public SlicedFillMesh(ExtImage extImage) : base(extImage)
    {
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        if (alphaHitTestMinimumThreshold <= 0)
            return true;

        if (alphaHitTestMinimumThreshold > 1)
            return false;

        if (extImage.overrideSprite == null)
            return true;

        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(extImage.rectTransform, screenPoint, eventCamera, out local))
            return false;

        Rect rect = extImage.GetPixelAdjustedRect();

        // Convert to have lower left corner as reference point.
        local.x += extImage.rectTransform.pivot.x * rect.width;
        local.y += extImage.rectTransform.pivot.y * rect.height;

        Rect spriteRect = extImage.overrideSprite.rect;
        Vector4 border = extImage.overrideSprite.border;
        Vector4 adjustedBorder = GetAdjustedBorders(border / extImage.pixelsPerUnit, rect);

        for (int i = 0; i < 2; i++)
        {
            if (local[i] <= adjustedBorder[i])
                continue;

            if (rect.size[i] - local[i] <= adjustedBorder[i + 2])
            {
                local[i] -= rect.size[i] - spriteRect.size[i];
                continue;
            }

            float lerp = Mathf.InverseLerp(adjustedBorder[i], rect.size[i] - adjustedBorder[i + 2], local[i]);
            local[i] = Mathf.Lerp(border[i], spriteRect.size[i] - border[i + 2], lerp);
        }

        // Normalize local coordinates.
        Rect textureRect = extImage.overrideSprite.textureRect;
        Vector2 normalized = new Vector2(local.x / textureRect.width, local.y / textureRect.height);

        // Convert to texture space.
        float x = Mathf.Lerp(textureRect.x, textureRect.xMax, normalized.x) / extImage.overrideSprite.texture.width;
        float y = Mathf.Lerp(textureRect.y, textureRect.yMax, normalized.y) / extImage.overrideSprite.texture.height;

        switch (extImage.fillDirection)
        {
            case FillDirection.Right:
                if (x > extImage.fillAmount)
                    return false;
                break;
            case FillDirection.Left:
                if (1f - x > extImage.fillAmount)
                    return false;
                break;
            case FillDirection.Up:
                if (y > extImage.fillAmount)
                    return false;
                break;
            case FillDirection.Down:
                if (1f - y > extImage.fillAmount)
                    return false;
                break;
        }

        try
        {
            return extImage.overrideSprite.texture.GetPixelBilinear(x, y).a >= alphaHitTestMinimumThreshold;
        }
        catch (UnityException e)
        {
            Debug.LogError("Using alphaHitTestMinimumThreshold greater than 0 on Image whose sprite texture cannot be read. " + e.Message + " Also make sure to disable sprite packing for this sprite.");
            return true;
        }
    }

    public override bool OnPopulateMesh(VertexHelper vh)
    {
        if (extImage.overrideSprite == null)
            return false;
        GenerateSlicedFilledSprite(vh);
        return true;
    }

    private void GenerateSlicedFilledSprite(VertexHelper vh)
    {
        vh.Clear();

        if (extImage.fillAmount < 0.001f)
            return;
        Rect rect = extImage.GetPixelAdjustedRect();
        Vector4 outer = DataUtility.GetOuterUV(extImage.overrideSprite);
        Vector4 padding = DataUtility.GetPadding(extImage.overrideSprite);

        if (!extImage.hasBorder)
        {
            Vector2 size = extImage.overrideSprite.rect.size;

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            // Image's dimensions used for drawing. X = left, Y = bottom, Z = right, W = top.
            Vector4 vertices = new Vector4(
                rect.x + rect.width * (padding.x / spriteW),
                rect.y + rect.height * (padding.y / spriteH),
                rect.x + rect.width * ((spriteW - padding.z) / spriteW),
                rect.y + rect.height * ((spriteH - padding.w) / spriteH));

            GenerateFilledSprite(vh, vertices, outer, extImage.fillAmount);
            return;
        }

        Vector4 inner = DataUtility.GetInnerUV(extImage.overrideSprite);
        Vector4 border = GetAdjustedBorders(extImage.overrideSprite.border / extImage.pixelsPerUnit, rect);

        padding = padding / extImage.pixelsPerUnit;

        s_SlicedVertices[0] = new Vector2(padding.x, padding.y);
        s_SlicedVertices[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

        s_SlicedVertices[1].x = border.x;
        s_SlicedVertices[1].y = border.y;

        s_SlicedVertices[2].x = rect.width - border.z;
        s_SlicedVertices[2].y = rect.height - border.w;

        for (int i = 0; i < 4; ++i)
        {
            s_SlicedVertices[i].x += rect.x;
            s_SlicedVertices[i].y += rect.y;
        }

        s_SlicedUVs[0] = new Vector2(outer.x, outer.y);
        s_SlicedUVs[1] = new Vector2(inner.x, inner.y);
        s_SlicedUVs[2] = new Vector2(inner.z, inner.w);
        s_SlicedUVs[3] = new Vector2(outer.z, outer.w);

        float rectStartPos;
        float _1OverTotalSize;
        if (extImage.fillDirection == FillDirection.Left || extImage.fillDirection == FillDirection.Right)
        {
            rectStartPos = s_SlicedVertices[0].x;

            float totalSize = s_SlicedVertices[3].x - s_SlicedVertices[0].x;
            _1OverTotalSize = totalSize > 0f ? 1f / totalSize : 1f;
        }
        else
        {
            rectStartPos = s_SlicedVertices[0].y;

            float totalSize = s_SlicedVertices[3].y - s_SlicedVertices[0].y;
            _1OverTotalSize = totalSize > 0f ? 1f / totalSize : 1f;
        }

        for (int x = 0; x < 3; x++)
        {
            int x2 = x + 1;

            for (int y = 0; y < 3; y++)
            {

                if (!extImage.fillCenter && x == 1 && y == 1)
                    continue;

                int y2 = y + 1;

                float sliceStart, sliceEnd;
                switch (extImage.fillDirection)
                {
                    case FillDirection.Right:
                        sliceStart = (s_SlicedVertices[x].x - rectStartPos) * _1OverTotalSize;
                        sliceEnd = (s_SlicedVertices[x2].x - rectStartPos) * _1OverTotalSize;
                        break;
                    case FillDirection.Up:
                        sliceStart = (s_SlicedVertices[y].y - rectStartPos) * _1OverTotalSize;
                        sliceEnd = (s_SlicedVertices[y2].y - rectStartPos) * _1OverTotalSize;
                        break;
                    case FillDirection.Left:
                        sliceStart = 1f - (s_SlicedVertices[x2].x - rectStartPos) * _1OverTotalSize;
                        sliceEnd = 1f - (s_SlicedVertices[x].x - rectStartPos) * _1OverTotalSize;
                        break;
                    case FillDirection.Down:
                        sliceStart = 1f - (s_SlicedVertices[y2].y - rectStartPos) * _1OverTotalSize;
                        sliceEnd = 1f - (s_SlicedVertices[y].y - rectStartPos) * _1OverTotalSize;
                        break;
                    default: // Just there to get rid of the "Use of unassigned local variable" compiler error
                        sliceStart = sliceEnd = 0f;
                        break;
                }

                if (sliceStart >= extImage.fillAmount)
                    continue;

                Vector4 vertices = new Vector4(s_SlicedVertices[x].x, s_SlicedVertices[y].y, s_SlicedVertices[x2].x, s_SlicedVertices[y2].y);
                Vector4 uvs = new Vector4(s_SlicedUVs[x].x, s_SlicedUVs[y].y, s_SlicedUVs[x2].x, s_SlicedUVs[y2].y);
                float fillAmount = (extImage.fillAmount - sliceStart) / (sliceEnd - sliceStart);

                GenerateFilledSprite(vh, vertices, uvs, fillAmount);
            }
        }
    }

    private void GenerateFilledSprite(VertexHelper vh, Vector4 vertices, Vector4 uvs, float fillAmount)
    {
        if (extImage.fillAmount < 0.001f)
            return;

        float uvLeft = uvs.x;
        float uvBottom = uvs.y;
        float uvRight = uvs.z;
        float uvTop = uvs.w;

        if (fillAmount < 1f)
        {
            switch (extImage.fillDirection)
            {
                case FillDirection.Right:
                    vertices.z = vertices.x + (vertices.z - vertices.x) * fillAmount;
                    uvRight = uvLeft + (uvRight - uvLeft) * fillAmount;
                    break;
                case FillDirection.Left:
                    vertices.x = vertices.z - (vertices.z - vertices.x) * fillAmount;
                    uvLeft = uvRight - (uvRight - uvLeft) * fillAmount;
                    break;
                case FillDirection.Up:
                    vertices.w = vertices.y + (vertices.w - vertices.y) * fillAmount;
                    uvTop = uvBottom + (uvTop - uvBottom) * fillAmount;
                    break;
                case FillDirection.Down:
                    vertices.y = vertices.w - (vertices.w - vertices.y) * fillAmount;
                    uvBottom = uvTop - (uvTop - uvBottom) * fillAmount;
                    break;
            }
        }

        s_Vertices[0] = new Vector3(vertices.x, vertices.y);
        s_Vertices[1] = new Vector3(vertices.x, vertices.w);
        s_Vertices[2] = new Vector3(vertices.z, vertices.w);
        s_Vertices[3] = new Vector3(vertices.z, vertices.y);

        s_UVs[0] = new Vector2(uvLeft, uvBottom);
        s_UVs[1] = new Vector2(uvLeft, uvTop);
        s_UVs[2] = new Vector2(uvRight, uvTop);
        s_UVs[3] = new Vector2(uvRight, uvBottom);
        int startIndex = vh.currentVertCount;

        for (int i = 0; i < 4; i++)
            vh.AddVert(s_Vertices[i], extImage.color, s_UVs[i]);

        vh.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
        vh.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
    }

}
