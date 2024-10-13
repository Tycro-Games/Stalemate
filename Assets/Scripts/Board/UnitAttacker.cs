using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using FMODUnity;
using UnityEngine;

public enum AttackTypes { EMPTY_SPACE, DESTROY_UNIT, HIT_UNIT }
public class UnitAttacker : MonoBehaviour {
  //[SerializeField] private AnimationCurve animationCurve;
  [SerializeField]
  private GameObject emptyAttackEffect;
  [SerializeField]
  private GameObject destroyUnitEffect;
  [SerializeField]
  private GameObject hitUnitEffect;

  [SerializeField]
  private GameObject previewAttack;
  private List<GameObject> previewAttacks = new List<GameObject>();

  private void Start() {}
  public IEnumerator AttackUnits(List<Tuple<Vector2, AttackTypes>> attackPositions, bool isRed) {
    if (attackPositions.Count == 0)
      yield break;
    attackPositions =
        attackPositions.GroupBy(x => new { x.Item1, x.Item2 }).Select(x => x.First()).ToList();
    for (int i = 0; i < attackPositions.Count; i++) {
      // add based on type
      GameObject explosionEffect;
      switch (attackPositions[i].Item2) {
        case AttackTypes.EMPTY_SPACE:
          explosionEffect = emptyAttackEffect;
          break;
        case AttackTypes.DESTROY_UNIT:
          explosionEffect = destroyUnitEffect;
          break;
        case AttackTypes.HIT_UNIT:
          explosionEffect = hitUnitEffect;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
      GameObject exp =
          Instantiate(explosionEffect, attackPositions[i].Item1, Quaternion.identity, transform);
      exp.GetComponent<ExplosionRenderer>().DrawColor(isRed);
    }
    AudioManager.instance.PlayOneShot(AudioManager.instance.shootSound);
    yield return null;
  }

  public void DeleteAllPreviews() {
    foreach (var attack in previewAttacks) {
      Destroy(attack);
    }
    previewAttacks.Clear();
  }
  public IEnumerator PreviewAttackUnits(List<Tuple<Vector2, AttackTypes>> attackPositions,
                                        bool isRed) {
    attackPositions =
        attackPositions.GroupBy(x => new { x.Item1, x.Item2 }).Select(x => x.First()).ToList();
    foreach (var attackPosition in attackPositions) {
      GameObject exp =
          Instantiate(previewAttack, attackPosition.Item1, Quaternion.identity, transform);
      previewAttacks.Add(exp);
    }
    yield return null;
  }
}
