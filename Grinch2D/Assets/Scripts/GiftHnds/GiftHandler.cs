using UnityEngine;


public class GiftHandler : MonoBehaviour
{
    public GameObject giftObj;                                             // sample gift object

    /// <summary>
    /// Method for take gift
    /// </summary>
    protected virtual void TakeGift(GameObject gift)
    {
        gift.SetActive(false);                                           // hide gift object on game scene
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == giftObj.tag)
        {
            TakeGift(other.gameObject);
        }
    }
}
