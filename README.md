# Zombie Runner
2023년도 1학기 캡스톤 디자인 과제물





CameraFollow.cs

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraFollow : MonoBehaviour
        {
    public Transform target;

    public float smoothSpeed = 3;
    public Vector2 offset;
    public float limitMinX, limitMaxX, limitMinY, limitMaxY;
    float cameraHalfWidth, cameraHalfHeight;

    private void Start()
    {
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;
    }

    private void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(
            Mathf.Clamp(target.position.x + offset.x, limitMinX + cameraHalfWidth, limitMaxX - cameraHalfWidth),   // X
            Mathf.Clamp(target.position.y + offset.y, limitMinY + cameraHalfHeight, limitMaxY - cameraHalfHeight), // Y
            -10);                                                                                                  // Z
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothSpeed);
    }
    }



Enemy.cs


    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Enemy : MonoBehaviour
    {
        public float ZHP = 100f;

        private void Update()
    {
        
    }
    public void TakeHit(float damage) // ÇÇ°ÝÃ³¸®
    {
        ZHP -= damage; 
        Debug.Log("Enemy HP: " + ZHP);
        if (ZHP <= 0)
        {
            
            Destroy(gameObject);
        }
    }
}



Skill.cs



    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Skill : MonoBehaviour
    {
        public float lifeTime = 4f;
        public float damage = 40f;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            lifeTime -= Time.deltaTime;

            if (lifeTime < 0)
            {
                Destroy(gameObject);
            }
        }

    
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                collision.gameObject.GetComponent<Enemy>().TakeHit(damage);
            
            }
            Destroy(gameObject);
        }
        }
        
        
        
Patrol.cs



	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.AI;

	public class Patrol : MonoBehaviour
	{
    public Animator animator;

    public float zbDamage = 15f;


    public float moveSpeed = 2f;  // 이동 속도
    public float leftBound = -6f;   // 왼쪽 경계
    public float rightBound = 6f;   // 오른쪽 경계
    public float breakTime = 3f;

    public GameObject startRayPos;
    public GameObject target;
    private Vector3 initialPosition;
    public Vector3 targetPos;
    public float distanceX;
    public float distanceY;

    private enum State{idle, patrolling, tracking, attack, dead}
    State state = State.idle;

    public float trackingTimer; // 추적 제한 시간
    private int moveDirection = 1;  // 이동 방향 (1: 오른쪽, -1: 왼쪽)

    void Awake()
    {
        // enemyTr = this.GetComponent<Transform>();
        animator = this.GetComponent<Animator>();
        initialPosition = this.gameObject.transform.position;
    }
    // Start is called before the first frame update
    void Start()
    {
        state = State.patrolling;
        trackingTimer = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("현재 상태는 " + state + "입니다.");
        _Patrol();
        Tracking();
        Attack();

        

    }

    private void _Patrol()
    {

        if (state != State.tracking)
        {

            // 현재 위치에서 이동 방향으로 일정 거리만큼 이동
            if (state == State.patrolling)
            {
                animator.SetBool("patrol", true);
                transform.position += new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0, 0);

                // 이동 경로가 왼쪽 경계에 닿았을 때
                if (transform.position.x < initialPosition.x + leftBound)
                {
                    // 이동 방향을 오른쪽으로 전환
                    moveDirection = 1;
                    state = State.idle;
                    Invoke("EndOfBreak", breakTime);

                }
                // 이동 경로가 오른쪽 경계에 닿았을 때
                else if (transform.position.x > initialPosition.x + rightBound)
                {
                    // 이동 방향을 왼쪽으로 전환
                    moveDirection = -1;

                    state = State.idle;
                    Invoke("EndOfBreak", breakTime);
                }

            }
            else
            {
                animator.SetBool("patrol", false);
                state = State.idle;
            }
        }


        


    }
    private void EndOfBreak()
    {

        if (this.transform.rotation.y == 0 && state == State.idle && moveDirection == -1)
        {
            transform.rotation = Quaternion.Euler(0, -180f, 0);
            state = State.patrolling;
        }
        else if (this.transform.rotation.y != 0 && state == State.idle && moveDirection == 1)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            state = State.patrolling;
        }
        else
        {
            return;
        }
        

    }

    private void Tracking()
    {

        //플레이어 탐색을 위한 Ray 발사
        Debug.DrawRay(startRayPos.transform.position, this.transform.right * 15, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(startRayPos.transform.position, this.transform.right, 15f);
        

        if (hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            target = hit.collider.gameObject;
            Debug.Log(target.transform.name + "의 collider와 충돌");
            //추적상태 활성화
            state = State.tracking;
            trackingTimer = 0f;
        }

        if (target != null)
        {
            targetPos = target.transform.position;
            distanceX = this.transform.position.x - targetPos.x;
            distanceY = this.transform.position.y - targetPos.y;
        }
        if (state == State.tracking)
        {
            animator.SetBool("patrol", false);
            if (this.transform.position.x < targetPos.x)
            {
                animator.SetBool("tracking", true);
                transform.rotation = Quaternion.Euler(0, 0, 0);
                moveDirection = 1;
                transform.position += new Vector3(moveDirection * moveSpeed * 2 * Time.deltaTime, 0, 0);

            }
            else if (this.transform.position.x > targetPos.x)
            {
                animator.SetBool("tracking", true);
                transform.rotation = Quaternion.Euler(0, -180f, 0);
                moveDirection = -1;
                transform.position += new Vector3(moveDirection * moveSpeed * 2 * Time.deltaTime, 0, 0);
            }
        }

        if (trackingTimer >= 10.0f )
        {
            trackingTimer = 0f;
            state = State.patrolling;
            if (state != State.tracking) //추적이 종료되고 순찰중이 아닐 때
            {
                target = null;
                targetPos = new Vector3(0, 0, 0);
                distanceX = 0;
                distanceY = 0;
                initialPosition = this.gameObject.transform.position;
                animator.SetBool("tracking", false);
                animator.SetBool("patrol", true);
            }
        }
        else if (state == State.tracking)
        {
            trackingTimer += Time.deltaTime;
            
        }


        
    }

    private void Attack()
    {
        if (state != State.patrolling)
        {
            if (distanceX < 0)
            {
                distanceX *= -1;

            }
            if (distanceY < 0)
            {
                distanceY *= -1;

            }
            if ((distanceX < 2f && distanceX > 0) && ((distanceY < 2f && distanceY > 0)))
            {
                state = State.attack;
                animator.SetBool("attack", true);
            }
            else if (distanceX > 2f || distanceY > 2f)
            {
                state = State.tracking;
                animator.SetBool("attack", false);
                
            }
        }
        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().TakeHit(zbDamage);

        }

    }
	}
	
	
	
	
PlayerController.cs



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
    private bool canSkillE = false;
    private bool canSkillR =false;

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

        if (canSkillE == true)
        {
            ThrowSkill();
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
    void ThrowSkill()
    {
        if (Input.GetKeyDown("e"))
        {
            float eSpeed = 15f;
            //attackTime = 6f;
            GameObject E = Instantiate(Eskill, skillPos.position, Quaternion.identity);
            Vector3 direction = transform.right * -1;

            // 발사체에 속도 적용
            Rigidbody2D ERigidbody = E.GetComponent<Rigidbody2D>();
            ERigidbody.velocity = direction * eSpeed;

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

    private void OnTriggerStay2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("ESkillChanger")&&Input.GetKeyDown("f"))
        {
            canSkillE = true;
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


  
