using UnityEngine;

public sealed class EndlessBackground
{
    /// <summary>自身のrenderer</summary>
    public readonly SpriteRenderer Renderer;

    /// <summary>上隣の背景</summary>
    public EndlessBackground Next;

    /// <summary>下隣の背景</summary>
    public EndlessBackground Prev;

    public EndlessBackground(SpriteRenderer renderer, EndlessBackground next = null, EndlessBackground prev = null)
    {
        Renderer = renderer;
        Next     = next;
        Prev     = prev;
    }
}
