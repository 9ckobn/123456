using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class TaskAction
{
    public int totalValue = 0;
    
    public virtual bool CheckForCompletion(int currentValue)
    {
        return currentValue > totalValue;
    }
}