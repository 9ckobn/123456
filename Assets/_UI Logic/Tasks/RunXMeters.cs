public class RunXMeters : TaskAction
{
    private const int baseTotal = 1000;

    public RunXMeters(int multi)
    {
        totalValue = baseTotal * multi;
    }
}