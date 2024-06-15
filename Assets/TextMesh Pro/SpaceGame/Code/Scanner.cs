using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public List<Transform> nearestTargets; //가장 가까운 타겟들
    internal object nearestTarget;

    void FixedUpdate()
    {
        // 매개변수 -> 캐스팅 시작 위치, 원의 반지름, 캐스팅 방향, 캐스팅 길이, 대상 레이어
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTargets = GetNearestTargets();
    }

    List<Transform> GetNearestTargets()
    {
        List<Transform> result = new List<Transform>();

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (result.Count < 10) // 최대 10개의 타겟 추적
            {
                result.Add(target.transform);
            }
        }

        return result;
    }
}
