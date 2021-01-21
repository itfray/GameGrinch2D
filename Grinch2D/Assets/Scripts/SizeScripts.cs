using UnityEngine;

public static class SizeScripts
{
    /// <summary>
    /// Function is calculate size game object by BoxCollider2D component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns> size game object </returns>
    public static Vector2 sizeObjByBoxCollider2D(GameObject obj)
    {
        BoxCollider2D objBox = obj.GetComponent<BoxCollider2D>();
        if (objBox == null) return Vector2.zero;
        return new Vector2(Mathf.Abs(objBox.size.x * obj.transform.localScale.x),
                           Mathf.Abs(objBox.size.y * obj.transform.localScale.y));
    }

    /// <summary>
    /// Function is calculate size game object by CapsuleCollider2D component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns> size game object </returns>
    public static Vector2 sizeObjByCapsuleCollider2D(GameObject obj)
    {
        CapsuleCollider2D objBox = obj.GetComponent<CapsuleCollider2D>();
        if (objBox == null) return Vector2.zero;
        return new Vector2(Mathf.Abs(objBox.size.x * obj.transform.localScale.x),
                           Mathf.Abs(objBox.size.y * obj.transform.localScale.y));
    }

    /// <summary>
    /// Function is calculate size game object by Renderer component
    /// </summary>
    /// <param name="obj"> any game object </param>
    /// <returns> size game object </returns>
    public static Vector2 sizeObjByRenderer(GameObject obj)
    {
        Renderer rndr = obj.GetComponent<Renderer>();
        return rndr.bounds.max - rndr.bounds.min;
    }
}
