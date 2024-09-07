using System;
using UnityEngine;

[Serializable]
public class TaskAction
{
    public int totalValue = 0;

    public virtual bool CheckForCompletion(int currentValue)
    {
        Debug.Log("base checking");
        return false;
    }
}