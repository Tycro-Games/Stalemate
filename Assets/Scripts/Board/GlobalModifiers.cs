using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalModifiers", menuName = "Game/GlobalModifiers")]
public class GlobalModifiers : ScriptableObject {
  public int HpModifier = 1;
  public bool OneActionPerUnit = true;
  public bool MovesEachUnitIndividually = true;



}
