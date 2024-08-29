public class CollectXCoins : TaskAction
{
    private const int baseMilestone = 10;
    
    public CollectXCoins(int multi)
    {
        totalValue = (int)(baseMilestone * multi * 1.5f);
    }
}