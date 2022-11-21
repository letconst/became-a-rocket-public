using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class GimmickHandlerBase : MonoBehaviour
{
    /// <summary>
    /// 自身のcollider
    /// </summary>
    protected Collider2D SelfCollider;

    protected virtual void Start()
    {
        SelfCollider = GetComponent<Collider2D>();
    }
}
