using UnityEngine;
using System;


public class GiftHandler : MonoBehaviour
{
    public GameObject giftObj;                                             // sample gift object

    public delegate void GiftEventHnd();                                   // type handler of events of GiftHandler
    public event GiftEventHnd OnTaked;                                     // invoke when gift is taked 

    /// <summary>
    /// Method for take gift
    /// </summary>
    protected virtual void TakeGift(GameObject gift)
    {
        gift.SetActive(false);                                           // hide gift object on game scene
        OnTaked?.Invoke();                                               // invoke event
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == giftObj.tag)
        {
            TakeGift(other.gameObject);
        }
    }
}
