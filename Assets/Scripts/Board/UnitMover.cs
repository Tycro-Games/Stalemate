using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitMover : MonoBehaviour {
  [SerializeField]
  private float maxAccelerationTime = 0.5f;

  [SerializeField]
  private AnimationCurve deviationCurve = null;
  [SerializeField]
  private Vector3 deviationAxis = Vector3.zero;
  [SerializeField]
  private float deviationMax = 3.0f;
  [SerializeField]
  private AnimationCurve speedCurve = null;

  [SerializeField]
  private GameObject startingMovementSprite = null;
  [SerializeField]
  private GameObject endingMovementSprite = null;

  private List<GameObject> spriteGameObject = new();
  private HashSet<GameObject> previewMovements = new();
  private int countDone = 0;
  private void Start() {
    GameObject sprite = new GameObject("Sprite");
    sprite.AddComponent<SpriteRenderer>();
    sprite.AddComponent<UnitRenderer>();
    sprite.transform.localScale = Vector3.one * 5;
    sprite.SetActive(false);
    for (int i = 0; i < 30; i++) {
      spriteGameObject.Add(Instantiate(sprite, transform));
    }
    Destroy(sprite);
  }

  public void DeleteAllPreviews() {
    foreach (var move in previewMovements) {
      Destroy(move);
    }
    previewMovements.Clear();
  }
  public IEnumerator PreviewMovementUnits(List<UnitRenderer> init, List<UnitRenderer> fin,
                                          bool isRed) {
    //
    var pieces = GetComponent<Board>().pieces;
    float sign = -1.0f;
    // invert sprite
    if (isRed) {
      sign = 1.0f;
    }
    for (int i = 0; i < fin.Count; i++) {
      if (fin[i] == null || fin[i] == init[i]) {
        init[i] = null;
        fin[i] = null;
      } else {
        List<UnitRenderer> intermediateUnits =
            Board.GetUnitRenderersBetween(init[i], fin[i], ref pieces);
        // Instantiate preview for intermediate units (if desired)
        foreach (var unit in intermediateUnits) {
          SpawnMovementSquare(unit, startingMovementSprite, sign);
        }
      }
    }
    init.RemoveAll(item => item == null);
    fin.RemoveAll(item => item == null);
    // foreach (var unit in init) {
    //   SpawnMovementSquare(unit, startingMovementSprite, sign);
    // }
    foreach (var unit in fin) {
      SpawnMovementSquare(unit, endingMovementSprite, sign);
    }
    for (int i = 0; i < init.Count; i++) {
      Debug.Log(init[i].name + " " + fin[i].name);
    }

    yield return null;
  }

  private void SpawnMovementSquare(UnitRenderer unit, GameObject sprite, float sign) {
    GameObject intermediateExp =
        Instantiate(sprite, unit.transform.position, Quaternion.identity, transform);
    intermediateExp.transform.localScale *= sign;

    previewMovements.Add(intermediateExp);
  }

  public IEnumerator MoveUnits(List<UnitRenderer> init, List<UnitRenderer> fin) {
    if (init.Count == 0)
      yield break;
    AudioManager.instance.PlayOneShot(AudioManager.instance.moveSound);
    countDone = init.Count;
    for (int i = 0; i < fin.Count; i++) {
      if (fin[i] == null) {
        fin[i] = init[i];
      }
    }

    for (int i = 0; i < fin.Count; i++) {
      var unitData = fin[i].GetUnitSettings();

      spriteGameObject[i].SetActive(true);
      spriteGameObject[i].transform.position = init[i].transform.position;
      UnitRenderer unitRenderer = spriteGameObject[i].GetComponent<UnitRenderer>();
      unitRenderer.SetUnitSettingsAndHp(unitData, fin[i].GetHp());

      fin[i].SetUnitSettings(new UnitBoardInfo());
    }

    for (int i = 0; i < init.Count; i++) {
      Debug.Log(init[i].name + " " + fin[i].name);

      StartCoroutine(MoveToTarget(spriteGameObject[i].transform, fin[i].transform));
    }

    while (countDone > 0) {
      yield return null;
    }

    for (int i = 0; i < init.Count; i++) {
      UnitRenderer unitRenderer = spriteGameObject[i].GetComponent<UnitRenderer>();
      fin[i].SetUnitSettingsAndHp(unitRenderer.GetUnitSettings(), fin[i].GetHp());

      spriteGameObject[i].SetActive(false);
    }
  }
  private float TimeManagement(ref float currentTime) {
    currentTime += Time.deltaTime;
    return currentTime / maxAccelerationTime;  // return values from 0 to 1
  }
  private IEnumerator MoveToTarget(Transform toMoveTransform, Transform target) {
    float currentTime = 0.0f;
    // this function just moves a thing from point A to B
    var initialPosition = toMoveTransform.position;
    while (toMoveTransform.position != target.position) {
      var time = TimeManagement(ref currentTime);

      var pos = Vector3.Lerp(initialPosition, target.position,
                             speedCurve.Evaluate(time));  // magic animation curve

      var deviationLen = Mathf.Lerp(0.0f, deviationMax, deviationCurve.Evaluate(time));
      var deviation = deviationAxis * deviationLen;
      // Visualize the path using Gizmos.DrawLine
      toMoveTransform.position = pos + deviation;
      if (time >= 1.0f)
        break;
      yield return null;
    }

    countDone--;
  }
}
