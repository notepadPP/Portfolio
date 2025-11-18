using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class SimpleMesh : BaseImageMesh
{
    public SimpleMesh(ExtImage extImage) : base(extImage)
    {
    }

    public override bool OnPopulateMesh(VertexHelper vh)
    {
        if (!extImage.useSpriteMesh)
        {
            GenerateSimpleSprite(vh, extImage.preserveAspect);
        }
        else
        {
            GenerateSprite(vh, extImage.preserveAspect);
        }
        return true;
    }

    private void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
    {
        Vector4 drawingDimensions = GetDrawingDimensions(lPreserveAspect);
        Vector4 vector = ((extImage.overrideSprite != null) ? DataUtility.GetOuterUV(extImage.overrideSprite) : Vector4.zero);
        Color color = extImage.color;
        vh.Clear();
        vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector.x, vector.y));
        vh.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector.x, vector.w));
        vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector.z, vector.w));
        vh.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector.z, vector.y));
        vh.AddTriangle(0, 1, 2);
        vh.AddTriangle(2, 3, 0);
    }

    private void GenerateSprite(VertexHelper vh, bool lPreserveAspect)
    {
        Vector2 vector = new Vector2(extImage.overrideSprite.rect.width, extImage.overrideSprite.rect.height);
        Vector2 vector2 = extImage.overrideSprite.pivot / vector;
        Vector2 pivot = extImage.rectTransform.pivot;
        Rect rect = extImage.GetPixelAdjustedRect();
        if (lPreserveAspect & (vector.sqrMagnitude > 0f))
        {
            PreserveSpriteAspectRatio(ref rect, vector);
        }

        Vector2 vector3 = new Vector2(rect.width, rect.height);
        Vector3 size = extImage.overrideSprite.bounds.size;
        Vector2 vector4 = (pivot - vector2) * vector3;
        Color color = extImage.color;
        vh.Clear();
        Vector2[] vertices = extImage.overrideSprite.vertices;
        Vector2[] uv = extImage.overrideSprite.uv;
        for (int i = 0; i < vertices.Length; i++)
        {
            vh.AddVert(new Vector3(vertices[i].x / size.x * vector3.x - vector4.x, vertices[i].y / size.y * vector3.y - vector4.y), color, new Vector2(uv[i].x, uv[i].y));
        }

        ushort[] triangles = extImage.overrideSprite.triangles;
        for (int j = 0; j < triangles.Length; j += 3)
        {
            vh.AddTriangle(triangles[j], triangles[j + 1], triangles[j + 2]);
        }
    }

    private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
    {
        Vector4 vector = ((extImage.overrideSprite == null) ? Vector4.zero : DataUtility.GetPadding(extImage.overrideSprite));
        Vector2 spriteSize = ((extImage.overrideSprite == null) ? Vector2.zero : new Vector2(extImage.overrideSprite.rect.width, extImage.overrideSprite.rect.height));
        Rect rect = extImage.GetPixelAdjustedRect();
        int num = Mathf.RoundToInt(spriteSize.x);
        int num2 = Mathf.RoundToInt(spriteSize.y);
        Vector4 vector2 = new Vector4(vector.x / (float)num, vector.y / (float)num2, ((float)num - vector.z) / (float)num, ((float)num2 - vector.w) / (float)num2);
        if (shouldPreserveAspect && spriteSize.sqrMagnitude > 0f)
        {
            PreserveSpriteAspectRatio(ref rect, spriteSize);
        }

        return new Vector4(rect.x + rect.width * vector2.x, rect.y + rect.height * vector2.y, rect.x + rect.width * vector2.z, rect.y + rect.height * vector2.w);
    }

}
