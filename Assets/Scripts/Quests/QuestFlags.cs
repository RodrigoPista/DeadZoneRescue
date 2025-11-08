public static class QuestFlags
{
    public static bool HasCannedFood = false;
    public static bool HasVendas = false;
    public static bool HasCura = false;

    public static void ResetAll()
    {
        HasCannedFood = false;
        HasVendas = false;
        HasCura = false;
    }
}
