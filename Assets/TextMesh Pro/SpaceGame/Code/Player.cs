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
    // 스킬 발사 관련 변수
    public GameObject skillBulletPrefab;
    public Transform skillBulletSpawnPoint;
    public float skillRange = 5f; // 스킬의 범위
    public LayerMask enemyLayerMask; // 적 LayerMask 설정

    // 스킬 쿨타임 관련 변수
    public float skillCooldown = 10f; // 스킬 쿨타임 (초)
    private float skillCooldownTimer = 0f; // 현재 쿨타임 타이머
    public Button skillButton; // 스킬 버튼
    public Text skillCooldownText; // 스킬 쿨타임 UI 텍스트

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 쿨타임 타이머 업데이트
        if (skillCooldownTimer > 0)
        {
            skillCooldownTimer -= Time.deltaTime;
            skillCooldownText.text = skillCooldownTimer.ToString("F1");
            skillButton.interactable = false;

            if (skillCooldownTimer <= 0)
            {
                skillButton.interactable = true; // 쿨타임이 끝나면 버튼 활성화
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

        // 스킬 버튼 클릭 이벤트 설정
        if (skillButton != null)
        {
            skillButton.onClick.AddListener(FireSkill);
        }
    }

    // 스킬 발사 함수
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

        // 스킬 사용 후 쿨타임 시작
        skillCooldownTimer = skillCooldown;
        skillButton.interactable = false; // 스킬 버튼 비활성화
        StartCoroutine(FireSkillRoutine());
    }

    IEnumerator FireSkillRoutine()
    {
        // 적을 스캔
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
                // 적을 무작위로 선택하여 타겟팅
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


    // Gizmos를 사용하여 스킬 범위를 시각화
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
