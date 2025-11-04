using System.Collections;
using UnityEngine;

namespace Utility
{
public class LerpBetween2Points : MonoBehaviour
{
    [SerializeField]
    private Transform a;

    [SerializeField]
    private Transform b;

    [SerializeField]
    private AnimationCurve speedCurve = null;

    private float currentTime = 0;

    [SerializeField]
    private float duration = 1.0f;

    [SerializeField]
    private float speed = 2.0f;

    private bool AtB = true;

    private void OnEnable()
    {
        currentTime = 0;
    }

    public void StartMovingA()
    {
        StopAllCoroutines();
        currentTime = 0;
        StartCoroutine(MoveToTarget(a));
    }

    public void StartMovingB()
    {
        StopAllCoroutines();
        currentTime = 0;
        StartCoroutine(MoveToTarget(b));
    }

    private IEnumerator MoveToTarget(Transform target)
    {
        while (transform.position != target.position)
        {
            var pos = Vector3.MoveTowards(transform.position, target.position,
                                          Time.deltaTime * speed *
                                              speedCurve.Evaluate(GetTimeFactor())); // magic animation curve

            transform.position = pos;
            yield return null;
        }

        AtB = !AtB;
    }

    private float GetTimeFactor()
    {
        currentTime += Time.deltaTime;
        return currentTime / duration;
    }

    public void MoveToTarget(Vector3 dir)
    {
        transform.position = Vector2.MoveTowards(transform.position, transform.position + dir,
                                                 Time.deltaTime * speed * speedCurve.Evaluate(GetTimeFactor()));
        if (speedCurve.Evaluate(GetTimeFactor()) <= 0)
            Destroy(gameObject);
    }
}
}
