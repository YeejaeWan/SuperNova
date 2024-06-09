using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    bool isLive;
    public bool bossKill; // 적이 보스 적인 경우를 나타내는 변수
  

    Rigidbody2D rigid;
    Collider2D coll;
    Animator anim;
    SpriteRenderer spriter;
    WaitForFixedUpdate wait;
  

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();

    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
            return;

        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
        rigid.velocity = Vector2.zero;
    }

    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive)
            return;

        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        isLive = true;

        // Enemy 재활용 부분
        coll.enabled = true;
        rigid.simulated = true;
        spriter.sortingOrder = 2;
        anim.SetBool("Dead", false);

        health = maxHealth;
    }

    public void Init(SpawnData data)
    {
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        maxHealth = data.health;
        health = data.health;

        if (data.spriteType == animCon.Length - 1)
        {
            spriter.transform.localScale = new Vector3(5f, 5f, 5f);
            bossKill = true;
        }
        else if(data.spriteType == animCon.Length - 2)
        {
            spriter.transform.localScale = new Vector3(5f, 5f, 5f);
            bossKill = false;
        }
        else if (data.spriteType == animCon.Length - 3)
        {
            spriter.transform.localScale = new Vector3(3f, 3f, 3f);
            bossKill = false;
        }
        else if (data.spriteType == animCon.Length - 4)
        {
            spriter.transform.localScale = new Vector3(2f, 2f, 2f);
            bossKill = false;
        }
        else
        {
            spriter.transform.localScale = Vector3.one;
            bossKill = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Bullet") || !isLive)
            return;

        health -= collision.GetComponent<Bullet>().damage;

      
        StartCoroutine(KnockBack());

        if (health > 0)
        {
            anim.SetTrigger("Hit");
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Hit);
        }
        else
        {
            // Die
            isLive = false;
            // 비활성화 부분
            coll.enabled = false;
            rigid.simulated = false;
            spriter.sortingOrder = 1;

            // 시체 애니메이션
            anim.SetBool("Dead", true);
            GameManager.instance.kill++;
            GameManager.instance.GetExp();

            if (GameManager.instance.isLive)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            }

            if (bossKill)
            {
                GameManager.instance.GameVictory();
            }
        }
    }

    IEnumerator KnockBack()
    {    
        
       
        yield return wait; // 하나의 물리 프레임 딜레이

        
         Vector3 playerPos = GameManager.instance.player.transform.position;
         Vector3 dirVec = transform.position - playerPos;
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);
        


    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
