using UnityEngine;


/// <summary>
/// StarHandler is class for game objects, that 
/// have star bar and have posible take stars.
/// </summary>
public class StarHandler : MonoBehaviour
{
    public GameObject starObj;                                             // sample star object
    public int max_count_stars = 3;                                        // maximum count of taked stars
    public int count_stars = 0;                                            // currect count of take stars

    public delegate void StarEventHnd();                                   // type handler of events of StarHandler
    public event StarEventHnd OnTaked;                                     // invoke when star is taked 

    /// <summary>
    /// Method resets stars counter
    /// </summary>
    public void ResetCounter()
    {
        count_stars = 0;
    }


    /// <summary>
    /// Method for take star
    /// </summary>
    /// <param name="star"></param>
    protected virtual void TakeStar(GameObject star)
    {
        if (count_stars < max_count_stars)
        {
            count_stars++;
            star.SetActive(false);                                           // hide star object on game scene

            OnTaked?.Invoke();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == starObj.tag)                                         // if tag of checked object contains in list damageObjs
        {
            TakeStar(other.gameObject);
        }
    }
}
