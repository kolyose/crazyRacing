using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static Util Instance;

    void Awake()
    {
        Instance = this;
    }

    public void SetTimeout(Action callback, float timeout)
    {
        StartCoroutine(WaitForSecondsCoroutine(callback, timeout));
    }

    private IEnumerator WaitForSecondsCoroutine(Action callback, float timeout)
    {
        yield return new WaitForSeconds(timeout);
        callback();
        yield break;
    }

    /*
     * Function provides a wrapper around top-level JSON arrays - just as workaround for JsonUtility's issue, because of which it can't to parse such array
     */
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
