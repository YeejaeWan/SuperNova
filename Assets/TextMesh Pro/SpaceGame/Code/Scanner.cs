using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    public float scanRange;
    public LayerMask targetLayer;
    public RaycastHit2D[] targets;
    public List<Transform> nearestTargets; //���� ����� Ÿ�ٵ�

    void FixedUpdate()
    {
        // �Ű����� -> ĳ���� ���� ��ġ, ���� ������, ĳ���� ����, ĳ���� ����, ��� ���̾�
        targets = Physics2D.CircleCastAll(transform.position, scanRange, Vector2.zero, 0, targetLayer);
        nearestTargets = GetNearestTargets();
    }

    List<Transform> GetNearestTargets()
    {
        List<Transform> result = new List<Transform>();
        float diff = 100;

        foreach (RaycastHit2D target in targets)
        {
            Vector3 myPos = transform.position;
            Vector3 targetPos = target.transform.position;
            float curDiff = Vector3.Distance(myPos, targetPos);

            if (result.Count < 10) // �ִ� 10���� Ÿ�� ����
            {
                result.Add(target.transform);
            }
        }

        return result;
    }
}