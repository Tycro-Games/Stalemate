using UnityEngine;

public static class GlobalSettings
{
    private static GlobalModifiers modifiers;

    public static int GetHpModifier()
    {
        if (modifiers == null)
        {
            modifiers = Resources.Load<GlobalModifiers>("GlobalModifiersData");
        }
        return modifiers != null ? modifiers.HpModifier : 0;
    }
    public static bool GetOneActionPerUnit()
    {
        if (modifiers == null)
        {
            modifiers = Resources.Load<GlobalModifiers>("GlobalModifiersData");
        }
        return modifiers.OneActionPerUnit;
    }
    public static bool GetMovesEachUnitIndividually()
    {
        if (modifiers == null)
        {
            modifiers = Resources.Load<GlobalModifiers>("GlobalModifiersData");
        }
        return modifiers.MovesEachUnitIndividually;
    }
}
