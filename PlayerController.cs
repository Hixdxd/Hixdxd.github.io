using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float maxSpeed = 4f;
    public float jumpForce = 5f;
    public float playerHP = 100f;
    private float atkTime;
    private float attackTime;
    public float atkCoolTime = 0.5f;
    bool isJump = true;


    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    private bool canAttack;
    private bool canSkillE;
    private bool canSkillR;

    Vector3 dir;

    public GameObject Eskill;
    //public GameObject Rskill;

    public Transform skillPos;
    private void Awake()
    {

        rb= GetComponent<Rigidbody2D>();
        sr= GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

    }

    void Update()
    {
        //이동
        if (Input.GetButtonUp("Horizontal"))
        { 
            rb.velocity = new Vector2(0.5f * rb.velocity.normalized.x, rb.velocity.y);
            

        }

        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumping"))
        {
            if (isJump == true)
            {
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                anim.SetBool("isJumping", true);
            }
                
        }
            

        /*if (Input.GetButtonDown("Horizontal")){

            sr.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }*/

        if (rb.velocity.normalized.x == 0) {
            anim.SetBool("isWalking", false);
        }

        else
        {
            anim.SetBool("isWalking", true);
        }
        //데미지 구현

        /*if (Input.GetKeyDown("q"))
        {
            attackTime = 0.5f;
            anim.SetBool("isAtking", true);
            TakeHit(50);
            StartCoroutine("AttackTimer");
        }
        else
        {
            atkTime -= Time.deltaTime;
        }*/


        if (Input.GetKeyDown("e"))
        {
            float eSpeed = 15f;
            //attackTime = 6f;
            GameObject E = Instantiate(Eskill, skillPos.position, Quaternion.identity);
            Vector3 direction = transform.right*-1;

            // 발사체에 속도 적용
            Rigidbody2D ERigidbody = E.GetComponent<Rigidbody2D>();
            ERigidbody.velocity = direction * eSpeed;
            

        }
        if (Input.GetKeyDown("r"))
        {

        }


    }


    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rb.AddForce(Vector2.right * h, ForceMode2D.Impulse);


        if (rb.velocity.x > maxSpeed)
        {
            rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            dir = Vector3.right;

        }

        else if (rb.velocity.x < maxSpeed * (-1))
        {
            rb.velocity = new Vector2(maxSpeed * (-1), rb.velocity.y);
            transform.rotation = Quaternion.Euler(0, 0, 0);
            dir = Vector3.left;
        }
            

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("IsGrounded"))
        {
            //isJump = false;
            anim.SetBool("isJumping", false);
        }

        
    }

    public void TakeHit(float damage) // 피격처리
    {
        playerHP -= damage;

        if (playerHP < 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator AttackTimer()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackTime);
        canAttack = true;
    }
}
