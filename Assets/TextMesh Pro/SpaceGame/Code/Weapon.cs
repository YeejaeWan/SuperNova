using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    public int prefabId;
    public float damage;
    public int count; // 무기 갯수
    public float speed;

    float timer;
    Player player;

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        switch (id)
        {
            case 0:
                // Time.deltaTime -> 한프레임이 소비한 시간, z축을 건드려서 회전
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
                break;
            default:
                timer += Time.deltaTime;

                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
        }

        // 테스트용
        if (Input.GetButtonDown("Jump"))
        {
            LevelUp(10, 1);
        }
    }

    // 레벨업
    public void LevelUp(float damage, int count)
    {
        this.damage = damage * Character.Damage;
        this.count += count;

        if (id == 0)
            Batch();

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    // 초기화
    public void Init(ItemData data)
    {
        // 기본 세팅
        name = "Weapon " + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        // Property Set
        id = data.itemId;
        damage = data.baseDamage * Character.Damage;
        count = data.baseCount + Character.Count;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }

        switch (id)
        {
            case 0:
                speed = 150 * Character.WeaponSpeed; // 시계방향
                Batch();
                break;
            default:
                speed = 0.4f * Character.WeaponRate;
                break;
        }

        // Hand Set
        Hand hand = player.hands[(int)data.itemType];
        hand.spriter.sprite = data.hand;
        hand.gameObject.SetActive(true);

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Batch()
    {
        for (int index = 0; index < count; index++)
        {
            Transform bullet;

            // 자식 오브젝트에서 가져옴
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                // 자식 오브젝트에서 가져온 것이 모자라면 부모에서 가져옴
                bullet = GameManager.instance.pool.Get(prefabId).transform;
                bullet.parent = transform; // 부모로 바꿈 (Player)
            }

            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            // 근접무기는 관통 숫자 상관 x, -1은 무한으로 관통
            bullet.GetComponent<Bullet>().Init(damage, -100, Vector3.zero);
        }
    }

    // 총알 발사
    void Fire()
    {
        List<Transform> targets = player.scanner.nearestTargets;
        if (targets == null || targets.Count == 0)
            return;

        int targetCount = Mathf.Min(count, targets.Count);

        for (int i = 0; i < targetCount; i++)
        {
            Vector3 targetPos = targets[i].position;
            Vector3 dir = targetPos - transform.position;
            dir = dir.normalized;

            Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
            bullet.position = transform.position; // 플레이어 위치에서 발사
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); // 회전
            bullet.GetComponent<Bullet>().Init(damage, count, dir);

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }
}
