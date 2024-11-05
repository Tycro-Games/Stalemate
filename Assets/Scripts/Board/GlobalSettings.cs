using UnityEngine;

public static class GlobalSettings {
  private static GlobalModifiers modifiers;

  public static int GetHpModifier()
  {
    // Load the ScriptableObject if it hasn't been loaded yet
    if (modifiers == null)
    {
      modifiers = Resources.Load<GlobalModifiers>("GlobalModifiersData");
    }
    return modifiers != null ? modifiers.HpModifier : 0;
  }
  public static bool GetOneActionPerUnit()
  {
    // Load the ScriptableObject if it hasn't been loaded yet
    if (modifiers == null)
    {
      modifiers = Resources.Load<GlobalModifiers>("GlobalModifiersData");
    }
    return modifiers.OneActionPerUnit;
  }
  public static bool GetMovesEachUnitIndividually()
  {
    // Load the ScriptableObject if it hasn't been loaded yet
    if (modifiers == null)
    {
      modifiers = Resources.Load<GlobalModifiers>("GlobalModifiersData");
    }
    return modifiers.MovesEachUnitIndividually;
  }
}
