using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    public Coroutine StartRoutine(IEnumerator routine)
    {
        return StartCoroutine(routine);
    }

    public void StopRoutine(Coroutine routine)
    {
        if (routine != null)
            StopCoroutine(routine);
    }
}