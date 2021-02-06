using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// KeyHandler is class for handle interraction with key object
/// </summary>
public class KeyHandler : DisappearHandler
{
    public GameObject lockBlockPrefab;                                          // prefab of locked block
    public GameObject lockBlockParent;                                          // preant field for locked blocks

    public float waitSecAppearBlocks = 0f;                                      // wait for seconds of appearing one block
    public float waitSecDiappearBlocks = 1f;                                    // wait for seconds of disappearing one block

    public BoxCollider2D collider2d;
    public SpriteRenderer spriteRndr;

    public delegate void KeyEventHandler();
    public event KeyEventHandler OnCreateLockBlocks;                            // event called when all blocks created
    public event KeyEventHandler OnDestroyLockBlocks;                           // event called when all blocks destoyed
    public event KeyEventHandler OnAppearKey;                                   // event called when key appeared
    public event KeyEventHandler OnDisappearKey;                                // event called when key disappeared
    public event KeyEventHandler OnAppearLockBlock;                             // event called when one of locked blocks appeared
    public event KeyEventHandler OnDisappearLockBlock;

    private List<GameObject> lock_blocks = new List<GameObject>();              // locked blocks
    public List<GameObject> lockedBlocks { get { return lock_blocks; } }        // locked blocks

    private bool is_blocks_created = false;                                        // locked block is created
    public bool isLockBlocksCreated { get { return is_blocks_created; } }

    private Coroutine appear_crtine;

    /// <summary>
    /// Creates locked blocks by specified positions
    /// </summary>
    /// <param name="positions"> specified positions </param>
    public void CreateLockBlocks(List<Vector2> positions)
    {
        StartCoroutine(CreateBlocks(positions));
    }

    /// <summary>
    /// Destroys locked blocked
    /// </summary>
    public void DestroyLockBlocks()
    {
        StartCoroutine(DestroyBlocks());
    }

    /// <summary>
    /// Activates appearing / disappearing key and locked blocks 
    /// </summary>
    /// <param name="value"> value { true : appearing, false : disappearing } </param>
    public override void Appear(bool value)
    {
        if (appear_crtine != null)
            StopCoroutine(appear_crtine);
        AppearKey(value);
        appear_crtine = StartCoroutine(AppearBlocks(value, value ? waitSecAppearBlocks : waitSecDiappearBlocks));
    }

    /// <summary>
    /// Creates locked blocks by specified positions
    /// </summary>
    /// <param name="positions"> specified positions </param>
    /// <returns> null </returns>
    private IEnumerator CreateBlocks(List<Vector2> positions)
    {
        if (lockBlockPrefab == null) yield break;

        foreach (Vector2 pos in positions)
        {
            lock_blocks.Add(Instantiate(lockBlockPrefab, pos, Quaternion.identity, lockBlockParent.transform));
            yield return null;
        }

        is_blocks_created = true;
        OnCreateLockBlocks?.Invoke();
    }

    /// <summary>
    /// Destroys locked blocked
    /// </summary>
    /// <returns> null </returns>
    private IEnumerator DestroyBlocks()
    {
        foreach (GameObject block in lock_blocks)
        {
            Destroy(block);
            yield return null;
        }
        lock_blocks.Clear();

        is_blocks_created = false;
        OnDestroyLockBlocks?.Invoke();
    }

    /// <summary>
    /// Activates appearing / disappearing key
    /// </summary>
    /// <param name="value"> value { true : appearing, false : disappearing } </param>
    private void AppearKey(bool value)
    {
        if (collider2d) 
            collider2d.enabled = value;
        if (spriteRndr) 
            spriteRndr.color = new Color(spriteRndr.color.r, spriteRndr.color.g, spriteRndr.color.b, value? 1: 0);

        if (value)
            OnAppearKey?.Invoke();
        else
            OnDisappearKey?.Invoke();
    }

    /// <summary>
    /// Activates appearing / disappearing locked blocks
    /// </summary>
    /// <param name="value"> value { true : appearing, false : disappearing } </param>
    /// <param name="waitSec"> wait for seconds of appearing / disappearing one block </param>
    /// <returns> null </returns>
    private IEnumerator AppearBlocks(bool value, float waitSec)
    {
        foreach (GameObject block in lock_blocks)
        {
            block.SetActive(value);
            if (value)
                OnAppearLockBlock?.Invoke();
            else
                OnDisappearLockBlock?.Invoke();

            yield return new WaitForSeconds(waitSec);
        }

        base.Appear(value);
    }
}
