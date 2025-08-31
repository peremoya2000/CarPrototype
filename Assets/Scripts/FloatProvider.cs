using System;
using UnityEngine;

public class FloatProvider : MonoBehaviour
{
    private Func<float> source;

    public void SetDataSource(Func<float> source)
    {
        this.source = source;
    }

    public float GetData()
    {
        if (source != null)
        {
            return source.Invoke();
        }
        else
        {
            Debug.LogError("Data not available", this);
            return 0f;
        }
    }
}
