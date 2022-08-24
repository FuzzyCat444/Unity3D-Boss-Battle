using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Together
{
    public static IEnumerator Do(MonoBehaviour monoBehaviour, 
        params IEnumerator[] coroutines)
    {
        Done done = new Done(coroutines.Length);
        
        for (int i = 0; i < coroutines.Length; i++)
        {
            IEnumerator coroutine = coroutines[i];
            monoBehaviour.StartCoroutine(
                WaitForCoroutine(monoBehaviour, coroutine, done));
        }

        while (done.NotDone())
        {
            yield return null;
        }
    }

    class Done
    {
        int n;

        public Done(int n)
        {
            this.n = n;
        }

        public void CoroutineDone()
        {
            n--;
        }

        public bool NotDone()
        {
            return n != 0;
        }
    }

    static IEnumerator WaitForCoroutine(MonoBehaviour monoBehaviour, 
        IEnumerator coroutine, Done done)
    {
        yield return coroutine;
        done.CoroutineDone();
    }
}
