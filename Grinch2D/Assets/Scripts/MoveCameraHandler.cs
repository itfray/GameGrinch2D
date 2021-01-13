using UnityEngine;


/// <summary>
/// class MoveCameraHandler is handler for camera that follow object
/// </summary>
public class MoveCameraHandler : MonoBehaviour
{
    public Transform following;                 // object that camera follow

    void Start()
    {
        UpdatePosition();
    }

    void Update()
    {
        UpdatePosition();
    }

    /// <summary>
    /// Set camera position on following object position
    /// </summary>
    void UpdatePosition()
    {
        if (following)
            transform.position = new Vector3(following.position.x, following.position.y, transform.position.z);
    }
}
