using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasRenderer))]
public class UIOutline : MaskableGraphic
{
    [SerializeField]
    private Texture m_Texture;

    [SerializeField, Range(0f, 500f)]
    private float outlineWidth = 100f;

    [SerializeField, Range(0f, 500f)]
    private float cornerRadius = 50f;

    [SerializeField, Range(1, 20)]
    private int cornerSegments = 1;

    [SerializeField, Range(0f, 1f)]
    private float mappingBias = 0.5f;

    [SerializeField]
    private bool fillCenter;

    private readonly Vector3[]      _corners = new Vector3[4];
    private readonly List<UIVertex> _verts   = new();

    public override Texture mainTexture => m_Texture == null ? s_WhiteTexture : m_Texture;

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        SetVerticesDirty();
        SetMaterialDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // 角のクランピング
        Rect  rect                = rectTransform.rect;
        float clampedCornerRadius = Mathf.Min(Mathf.Min(rect.width, rect.height) / 2f, cornerRadius);

        // 角のオフセット
        rectTransform.GetLocalCorners(_corners);
        _corners[0] += new Vector3(clampedCornerRadius, clampedCornerRadius, 0f);
        _corners[1] += new Vector3(clampedCornerRadius, -clampedCornerRadius, 0f);
        _corners[2] += new Vector3(-clampedCornerRadius, -clampedCornerRadius, 0f);
        _corners[3] += new Vector3(-clampedCornerRadius, clampedCornerRadius, 0f);

        // 形の算出
        float   height        = _corners[1].y - _corners[0].y;
        float   width         = _corners[2].x - _corners[1].x;
        float[] edgeLengths   = { height, width, height, width };
        float   circumference = 2f * Mathf.PI * Mathf.Lerp(clampedCornerRadius, clampedCornerRadius + outlineWidth, mappingBias);
        float   around        = height * 2f + width * 2f + circumference;
        float   cornerLength  = circumference / 4f;
        float   segmentLength = cornerLength / cornerSegments;

        var vert = new UIVertex { color = color };
        _verts.Clear();

        // 角の作成
        float u = 0f;

        for (int c = 0; c < 4; c++)
        {
            Vector3 origin = _corners[c];

            for (int i = 0; i < cornerSegments + 1; i++)
            {
                float angle     = (float) i / cornerSegments * Mathf.PI / 2f + Mathf.PI * 0.5f - Mathf.PI * c * 1.5f;
                var   direction = new Vector3(Mathf.Cos(-angle), Mathf.Sin(-angle), 0f);

                vert.position = origin + direction * clampedCornerRadius;
                vert.uv0      = new Vector2(u, 0f);
                _verts.Add(vert);

                vert.position = origin + direction * (clampedCornerRadius + outlineWidth);
                vert.uv0      = new Vector2(u, 1f);
                _verts.Add(vert);

                if (fillCenter)
                {
                    vert.position = rect.center;
                    vert.uv0      = new Vector2(u, 0f);
                    _verts.Add(vert);
                }

                if (i < cornerSegments)
                {
                    u += segmentLength / around;
                }
                else
                {
                    u += edgeLengths[c] / around;
                }
            }
        }

        // end vertex追加
        vert     = _verts[0];
        vert.uv0 = new Vector2(1f, 0f);
        _verts.Add(vert);

        vert     = _verts[1];
        vert.uv0 = new Vector2(1f, 1f);
        _verts.Add(vert);

        if (fillCenter)
        {
            vert     = _verts[2];
            vert.uv0 = new Vector2(1f, 1f);
            _verts.Add(vert);
        }

        // vertexの追加 → VertexHelper
        foreach (UIVertex vertex in _verts)
            vh.AddVert(vertex);

        // triangle追加 → VertexHelper
        if (fillCenter)
        {
            for (int v = 0; v < vh.currentVertCount - 3; v += 3)
            {
                vh.AddTriangle(v, v + 1, v + 4);
                vh.AddTriangle(v, v + 4, v + 3);

                vh.AddTriangle(v + 2, v, v + 3);
                vh.AddTriangle(v + 2, v + 3, v + 5);
            }
        }
        else
        {
            for (int v = 0; v < vh.currentVertCount - 2; v += 2)
            {
                vh.AddTriangle(v, v + 1, v + 3);
                vh.AddTriangle(v, v + 3, v + 2);
            }
        }
    }
}
