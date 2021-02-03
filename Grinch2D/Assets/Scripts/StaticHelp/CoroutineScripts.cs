using System.Collections;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CoroutineScripts
{
    /// <summary>
    /// Method executes specified method after specified number of seconds
    /// </summary>
    /// <param name="action"> invoked method </param>
    /// <param name="seconds"> wait for seconds </param>
    /// <returns> IEnumerator </returns>
    public static IEnumerator ExecWithWait(System.Action action, float seconds)
    {
        yield return new WaitForSeconds(seconds);

        action?.Invoke();
    }
}
