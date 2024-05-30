using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ExtImage : MaskableGraphic, ICanvasRaycastFilter
{

    [SerializeField]
    private Sprite m_Sprite;
    public Sprite sprite
    {
        get { return m_Sprite; }
        set
        {
            if (SetPropertyUtility.SetClass(ref m_Sprite, value))
            {
                SetAllDirty();
                TrackImage();
            }
        }
    }

    [NonSerialized]
    private Sprite m_OverrideSprite;
    public Sprite overrideSprite
    {
        get { return activeSprite; }
        set
        {
            if (SetPropertyUtility.SetClass(ref m_OverrideSprite, value))
            {
                SetAllDirty();
                TrackImage();
            }
        }
    }
    public override Texture mainTexture
    {
        get
        {
            if (activeSprite != null)
                return activeSprite.texture;

            return material != null && material.mainTexture != null ? material.mainTexture : s_WhiteTexture;
        }
    }

    public bool hasBorder
    {
        get
        {
            if (activeSprite != null)
            {
                Vector4 v = activeSprite.border;
                return v.sqrMagnitude > 0f;
            }

            return false;
        }
    }

    public override Material material
    {
        get
        {
            if (m_Material != null)
                return m_Material;

            if (activeSprite && activeSprite.associatedAlphaSplitTexture != null)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    return Image.defaultETC1GraphicMaterial;
            }

            return defaultMaterial;
        }
        set { base.material = value; }
    }
    [SerializeField]
    private ExtImageType m_Type;
    public ExtImageType type
    {
        get { return m_Type; }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_Type, value))
                SetVerticesDirty();
        }
    }
    [SerializeField]
    private FillDirection m_FillDirection;
    public FillDirection fillDirection
    {
        get { return m_FillDirection; }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_FillDirection, value))
                SetVerticesDirty();
        }
    }
    [SerializeField]
    private bool m_PreserveAspect;
    public bool preserveAspect
    {
        get { return m_PreserveAspect; }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_PreserveAspect, value))
                SetVerticesDirty();
        }
    }
    [SerializeField]
    private bool m_UseSpriteMesh;
    public bool useSpriteMesh
    {
        get
        {
            return m_UseSpriteMesh;
        }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_UseSpriteMesh, value))
            {
                SetVerticesDirty();
            }
        }
    }
    [Range(0, 1)]
    [SerializeField]
    private float m_FillAmount = 1f;
    public float fillAmount
    {
        get { return m_FillAmount; }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_FillAmount, Mathf.Clamp01(value)))
                SetVerticesDirty();
        }
    }
    [SerializeField]
    private float m_PixelsPerUnitMultiplier = 1f;
    public float pixelsPerUnitMultiplier
    {
        get { return m_PixelsPerUnitMultiplier; }
        set { m_PixelsPerUnitMultiplier = Mathf.Max(0.01f, value); }
    }

    public float pixelsPerUnit
    {
        get
        {
            float spritePixelsPerUnit = 100;
            if (activeSprite)
                spritePixelsPerUnit = activeSprite.pixelsPerUnit;

            float referencePixelsPerUnit = 100;
            if (canvas)
                referencePixelsPerUnit = canvas.referencePixelsPerUnit;

            return m_PixelsPerUnitMultiplier * spritePixelsPerUnit / referencePixelsPerUnit;
        }
    }
    [SerializeField]
    private bool m_FillCenter = true;
    public bool fillCenter
    {
        get { return m_FillCenter; }
        set
        {
            if (SetPropertyUtility.SetStruct(ref m_FillCenter, value))
                SetVerticesDirty();
        }
    }
    private Sprite activeSprite { get { return m_OverrideSprite != null ? m_OverrideSprite : m_Sprite; } }

    public enum Flip : int
    {
        None = 0,
        Horizontal = 1,
        Vertical = 2,
        HorizontalVertical = Horizontal | Vertical,
    }
    [SerializeField] Flip flip = Flip.None;
    BaseImageMesh meshHelper = null;
    private void SetType(ExtImageType type)
    {
        switch (type)
        {
            case ExtImageType.Simple:
                meshHelper = new SimpleMesh(this);
                break;
            case ExtImageType.Sliced:
                meshHelper = new SlicedFillMesh(this);
                break;
            case ExtImageType.Tiled:
                meshHelper = new TiledMesh(this);
                break;
            case ExtImageType.Filled:
                meshHelper = null;
                break;
        }

    }
    protected override void OnEnable()
    {
        base.OnEnable();
        TrackImage();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (m_Tracked)
            UnTrackImage();
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        SetVerticesDirty();
    }
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        SetType(m_Type);
        if (meshHelper == null || meshHelper.OnPopulateMesh(vh) == false)
            base.OnPopulateMesh(vh);
        ModifyFlipMesh(vh);
    }
    public override void SetNativeSize()
    {
        if (activeSprite != null)
        {
            float x = activeSprite.rect.width / pixelsPerUnit;
            float y = activeSprite.rect.height / pixelsPerUnit;
            base.rectTransform.anchorMax = base.rectTransform.anchorMin;
            base.rectTransform.sizeDelta = new Vector2(x, y);
            SetAllDirty();
        }
    }
    public void ModifyFlipMesh(VertexHelper verts)
    {
        List<UIVertex> buffer = new List<UIVertex>();
        verts.GetUIVertexStream(buffer);
        ModifyVertices(buffer);
        verts.AddUIVertexTriangleStream(buffer);
    }

    public void ModifyVertices(List<UIVertex> verts)
    {
        RectTransform rt = this.transform as RectTransform;

        for (int i = 0; i < verts.Count; ++i)
        {
            UIVertex v = verts[i];

            // Modify positions
            v.position = new Vector3(
                (flip.HasFlag(Flip.Horizontal) ? (v.position.x + (rt.rect.center.x - v.position.x) * 2) : v.position.x),
                (flip.HasFlag(Flip.Vertical) ? (v.position.y + (rt.rect.center.y - v.position.y) * 2) : v.position.y),
                v.position.z
            );

            // Apply
            verts[i] = v;
        }
    }
    private bool m_Tracked = false;

#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
    private static List<ExtImage> m_TrackedTexturelessImages = new List<ExtImage>();
    private static bool s_Initialized;
#endif

    private void TrackImage()
    {
        if (activeSprite != null && activeSprite.texture == null)
        {
#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
            if (!s_Initialized)
            {
                SpriteAtlasManager.atlasRegistered += RebuildImage;
                s_Initialized = true;
            }

            m_TrackedTexturelessImages.Add(this);
#endif
            m_Tracked = true;
        }
    }
    private void UnTrackImage()
    {
#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
        m_TrackedTexturelessImages.Remove(this);
#endif
        m_Tracked = false;
    }
#if UNITY_2017_4 || UNITY_2018_2_OR_NEWER
    private static void RebuildImage(SpriteAtlas spriteAtlas)
    {
        for (int i = m_TrackedTexturelessImages.Count - 1; i >= 0; i--)
        {
            ExtImage image = m_TrackedTexturelessImages[i];
            if (spriteAtlas.CanBindTo(image.activeSprite))
            {
                image.SetAllDirty();
                m_TrackedTexturelessImages.RemoveAt(i);
            }
        }
    }

    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        if (meshHelper == null)
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, sp, eventCamera);
        else
            return meshHelper.IsRaycastLocationValid(sp, eventCamera);
    }
#endif
}
