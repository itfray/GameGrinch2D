using UnityEngine;

/// <summary>
/// DamageHandler is class for game object, 
/// that can damage other game objects.
/// </summary>
public class DamageHandler : MonoBehaviour
{
    public int damage = 1;                                      // damage value
    public GameObject owner;                                    // information of damage owner
}
