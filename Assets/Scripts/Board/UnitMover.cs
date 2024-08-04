using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMover : MonoBehaviour
{
    [SerializeField]
    private float maxAccelerationTime = 0.5f;


    [SerializeField] private AnimationCurve deviationCurve = null;
    [SerializeField] private Vector3 deviationAxis = Vector3.zero;
    [SerializeField] private float deviationMax = 3.0f;
    [SerializeField]
    private AnimationCurve speedCurve = null;


    private List<GameObject> spriteGameObject = new();

    private int countDone = 0;
    private void Start()
    {
        GameObject sprite = new GameObject("Sprite");
        sprite.AddComponent<SpriteRenderer>();
        sprite.AddComponent<UnitRenderer>();
        sprite.transform.localScale = Vector3.one * 5;
        sprite.SetActive(false);
        for (int i = 0; i < 30; i++)
        {
            spriteGameObject.Add(Instantiate(sprite, transform));
        }
        Destroy(sprite);
    }
    public IEnumerator MoveUnits(List<UnitRenderer> init, List<UnitRenderer> fin)
    {
        if (init.Count == 0)
            yield break;
        AudioManager.instance.PlayOneShot(AudioManager.instance.moveSound);
        countDone = init.Count;
        for (int i = 0; i < fin.Count; i++)
        {
            if (fin[i] == null)
            {
                fin[i] = init[i];
            }

        }

        for (int i = 0; i < fin.Count; i++)
        {
            var unitData = fin[i].GetUnitSettings();

            spriteGameObject[i].SetActive(true);
            spriteGameObject[i].transform.position = init[i].transform.position;
            UnitRenderer unitRenderer = spriteGameObject[i].GetComponent<UnitRenderer>();
            unitRenderer.SetUnitSettings(unitData);

            fin[i].SetUnitSettings(new UnitBoardInfo());
        }

        for (int i = 0; i < init.Count; i++)
        {


            Debug.Log(init[i].name + " " + fin[i].name);

            StartCoroutine(MoveToTarget(spriteGameObject[i].transform, fin[i].transform));
        }

        while (countDone > 0)
        {
            yield return null;
        }

        for (int i = 0; i < init.Count; i++)
        {
            UnitRenderer unitRenderer = spriteGameObject[i].GetComponent<UnitRenderer>();
            fin[i].SetUnitSettings(unitRenderer.GetUnitSettings());
            spriteGameObject[i].SetActive(false);

        }

    }
    private float TimeManagement(ref float currentTime)
    {
        currentTime += Time.deltaTime;
        return currentTime / maxAccelerationTime; //return values from 0 to 1
    }
    private IEnumerator MoveToTarget(Transform toMoveTransform, Transform target)
    {
        float currentTime = 0.0f;
        //this function just moves a thing from point A to B
        var initialPosition = toMoveTransform.position;
        while (toMoveTransform.position != target.position)
        {
            var time = TimeManagement(ref currentTime);

            var pos = Vector3.Lerp(initialPosition, target.position,
                speedCurve.Evaluate(time)); //magic animation curve


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
