using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOrder
{

    public static IEnumerator Do(MonoBehaviour monoBehaviour, 
        params IEnumerator[] coroutines)
    {
        for (int i = 0; i < coroutines.Length; i++)
        {
            yield return coroutines[i];
        }
    }
}
