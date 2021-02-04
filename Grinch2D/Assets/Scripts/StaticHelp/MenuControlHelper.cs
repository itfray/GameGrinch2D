using UnityEngine;
using UnityEngine.UI;


public static class MenuControlHelper
{
    /// <summary>
    /// Method transform float seconds value in time string
    /// </summary>
    /// <param name="time"> time in seconds </param>
    /// <returns> time string format: "mm:ss" </returns>
    public static string SecondsToTimeStr(float time)
    {
        int min = (int)time / 60;
        int sec = (int)time % 60;

        if (min > 99)
        {
            min = 99;
            sec = 59;
        }

        string NumTo2DigStr(int num)
        {
            return num < 10 ? "0" + num : num.ToString();
        }

        return NumTo2DigStr(min) + ":" + NumTo2DigStr(sec);
    }

    /// <summary>
    /// Method sets sprite for star bar by value of count stars
    /// </summary>
    /// <param name="count_stars"> count stars </param>
    /// <param name="star_bar"> image of star bar </param>
    /// <param name="star_bar_sprts"> sprites for star bar </param>
    public static void UpdateSpriteStarBar(int count_stars, Image star_bar, Sprite[] star_bar_sprts)
    {
        if (star_bar == null || star_bar_sprts == null) return;
        if (star_bar_sprts.Length == 0) return;

        count_stars--;
        if (count_stars < 0)
        {
            star_bar.gameObject.SetActive(false);
        }
        else
        {
            star_bar.gameObject.SetActive(true);
            if (count_stars >= star_bar_sprts.Length)
                count_stars = star_bar_sprts.Length - 1;
            star_bar.sprite = star_bar_sprts[count_stars];                                   // set sprite from sprites list for image of star bar
        }
    }
}
