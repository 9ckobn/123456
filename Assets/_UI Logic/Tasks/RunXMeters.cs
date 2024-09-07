using UnityEngine;

public class RunXMeters : TaskAction
{
    private const int baseTotal = 1000;

    public RunXMeters(int multi)
    {
        totalValue = baseTotal * multi;
    }

    public override bool CheckForCompletion(int currentValue)
    {
        Debug.Log(totalValue + "   " + currentValue);
        return currentValue > totalValue;
    }
}