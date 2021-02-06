using UnityEngine;

/// <summary>
/// DisappBlockHnd is class handler of disappearing block
/// </summary>
public class DisappBlockHnd : DisappearHandler
{
    public Animator animator;
    public const string activeParam = "Activate";                               // activate parameter

    /// <summary>
    /// Activates appearing/disappearing of block
    /// </summary>
    public override void Appear(bool value)
    {
        if (animator) animator.SetBool(activeParam, !value);

        base.Appear(value);
    }
}
