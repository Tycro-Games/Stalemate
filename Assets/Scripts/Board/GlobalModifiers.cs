using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalModifiers", menuName = "Game/GlobalModifiers")]
public class GlobalModifiers : ScriptableObject {
  [SerializeField]
  private int hpModifier = 1;

  public int HpModifier => hpModifier;  // Public getter to access the value
}
