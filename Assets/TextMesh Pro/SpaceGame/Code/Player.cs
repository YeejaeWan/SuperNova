using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Vector2 inputVec;
    public float speed;
    public Scanner scanner;
    public Hand[] hands;
    public RuntimeAnimatorController[] animCon;

    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    // ��ų �߻� ���� ����
    public GameObject skillBulletPrefab;
    public Transform skillBulletSpawnPoint;
    public float skillRange = 5f; // ��ų�� ����
    public LayerMask enemyLayerMask; // �� LayerMask ����

    // ��ų ��Ÿ�� ���� ����
    public float skillCooldown = 10f; // ��ų ��Ÿ�� (��)
    private float skillCooldownTimer = 0f; // ���� ��Ÿ�� Ÿ�̸�
    public Button skillButton; // ��ų ��ư
    public Text skillCooldownText; // ��ų ��Ÿ�� UI �ؽ�Ʈ

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // ��Ÿ�� Ÿ�̸� ������Ʈ
        if (skillCooldownTimer > 0)
        {
            skillCooldownTimer -= Time.deltaTime;
            skillCooldownText.text = skillCooldownTimer.ToString("F1");
            skillButton.interactable = false;

            if (skillCooldownTimer <= 0)
            {
                skillButton.interactable = true; // ��Ÿ���� ������ ��ư Ȱ��ȭ
                skillCooldownText.text = "";
            }
        }
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        hands = GetComponentsInChildren<Hand>(true);

        // ��ų ��ư Ŭ�� �̺�Ʈ ����
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(FireSkill);
        }
    }

    // ��ų �߻� �Լ�
    public void FireSkill()
    {
        if (skillBulletSpawnPoint == null || skillBulletPrefab == null)
        {
            Debug.LogError("SkillBulletSpawnPoint or SkillBulletPrefab is not assigned.");
            return;
        }

        if (skillCooldownTimer > 0)
        {
            Debug.Log("Skill is on cooldown.");
            return;
        }

        // ��ų ��� �� ��Ÿ�� ����
        skillCooldownTimer = skillCooldown;
        skillButton.interactable = false; // ��ų ��ư ��Ȱ��ȭ
        StartCoroutine(FireSkillRoutine());
    }

    IEnumerator FireSkillRoutine()
    {
        // ���� ��ĵ
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, skillRange, enemyLayerMask);

        Debug.Log("Scanning for enemies...");
        foreach (var hit in hits)
        {
            Debug.Log("Detected enemy: " + hit.name);
        }

        if (hits.Length > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                // ���� �������� �����Ͽ� Ÿ����
                Collider2D randomTarget = hits[Random.Range(0, hits.Length)];
                Vector2 direction = (randomTarget.transform.position - skillBulletSpawnPoint.position).normalized;

                GameObject skillBullet = Instantiate(skillBulletPrefab, skillBulletSpawnPoint.position, Quaternion.identity);
                skillBullet.GetComponent<SkillBullet>().Init(direction, 3f);

                yield return new WaitForSeconds(0.1f);
            }
        }
        else
        {
            Debug.Log("No enemies found in range.");
        }
    }


    // Gizmos�� ����Ͽ� ��ų ������ �ð�ȭ
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, skillRange);
    }

 

    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];
    }


    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        Vector2 nextVec = inputVec.normalized * speed *Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position+ nextVec);
         
    }

    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed", inputVec.magnitude);

        if (inputVec.x !=0)
        {
            spriter.flipX = inputVec.x < 0;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!GameManager.instance.isLive)
            return;

        GameManager.instance.health -= Time.deltaTime * 10; 

        if(GameManager.instance.health<0)
        {
            for(int index=2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }

            anim.SetTrigger("Dead");
            GameManager.instance.GameOver();
        }

    }
}
