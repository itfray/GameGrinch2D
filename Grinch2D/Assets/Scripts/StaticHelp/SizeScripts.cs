using UnityEngine;

public static class SizeScripts
{
    /// <summary>
    /// Function is calculate size game object by BoxCollider2D component
    /// </summary>
    /// <param name="objBox"> box collider of game object </param>
    /// <returns> size game object </returns>
    public static Vector2 sizeObjBy(BoxCollider2D objBox)
    {
        if (objBox)
            return new Vector2(Mathf.Abs(objBox.size.x * objBox.transform.localScale.x),
                               Mathf.Abs(objBox.size.y * objBox.transform.localScale.y));
        return Vector2.zero;
    }

    /// <summary>
    /// Function is calculate size game object by CapsuleCollider2D component
    /// </summary>
    /// <param name="objBox"> box collider of game object </param>
    /// <returns> size game object </returns>
    public static Vector2 sizeObjBy(CapsuleCollider2D objBox)
    {
        if (objBox)
            return new Vector2(Mathf.Abs(objBox.size.x * objBox.transform.localScale.x),
                               Mathf.Abs(objBox.size.y * objBox.transform.localScale.y));
        return Vector2.zero;
    }

    /// <summary>
    /// Function is calculate size game object by Renderer component
    /// </summary>
    /// <param name="objRndr"> renderer of game object </param>
    /// <returns> size game object </returns>
    public static Vector2 sizeObjBy(Renderer objRndr)
    {
        if (objRndr) return objRndr.bounds.max - objRndr.bounds.min;
        return Vector2.zero;
    }
}
