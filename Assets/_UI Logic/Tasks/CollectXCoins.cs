using UnityEngine;

public class CollectXCoins : TaskAction
{
    private const int baseMilestone = 10;

    public CollectXCoins(int multi)
    {
        totalValue = (int)(baseMilestone * multi * 1.5f);
    }

    public override bool CheckForCompletion(int currentValue)
    {
        Debug.Log($"Task current value = {currentValue}, total = {totalValue}");
        return currentValue > totalValue;
    }
}