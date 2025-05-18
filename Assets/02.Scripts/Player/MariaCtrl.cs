using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MariaCtrl : MonoBehaviour
{
    public static bool DisableControl = false;

    private readonly string horizontal = "Horizontal";
    private readonly string vertical = "Vertical";
    private readonly string fire1 = "Fire1";
    private readonly string mouseX = "Mouse X";
    private readonly string mouseY = "Mouse Y";
    private readonly string speedX = "SpeedX";
    private readonly string speedY = "SpeedY";
    private readonly string idleJump = "IdleJump_T";
    private readonly string run = "IsRun_B";
    private readonly string movingJump = "MovingJump_T";
    private readonly string attack = "Attack_T";
    private readonly string shield = "Shield_B";
    [SerializeField] private Transform cameraPivot;
    

    private Animator anim;
    private float h = 0f, v = 0f, r = 0f;  // 마우스 x 
    private Transform tr;
    private float turnSpeed = 300f;
    private float moveSpeed = 5f;
    private bool isJumping;
    private bool isAttacking = false; // 공격중인지 아닌지 판단
    private bool isSprint = false;
    private bool isShielding = false;
    private Rigidbody rb;
    private AudioSource source;
    public AudioClip swordClip; // 공격 소리

    [Range(0, 100)] public float xSencitivity = 100.0f;
    [Range(0, 100)] public float ySencitivity = 100.0f;
    public float yMinLimit = -45f;
    public float yMaxLimit = 45f;
    public float xMinLimit = 360f;
    public float xMaxLimit = 360f;
    public float yRot = 0f; public float xRot = 0f;


    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        tr = transform;
        rb = GetComponent<Rigidbody>();
        isJumping = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    void Update()
    {
        if (tr == null) return; // Transform이 null이면 리턴
        if(DisableControl) return; // DisableControl이 true이면 리턴

        Shield();
        r = Input.GetAxis(mouseX);

        if (!isAttacking)
        {
            h = Input.GetAxis(horizontal);
            v = Input.GetAxis(vertical);
            Sprint();
            PlayerMove();
            IdleJump();
        }

        PlayerRotate();
        MovingJump();
        Attack();
        PlayerRotateY();
        CursorLockUnLock();


        /*tr.Translate(Vector3.right * h * moveSpeed * Time.deltaTime);
        {
            anim.SetFloat(speedX, h, 0.01f, Time.deltaTime); // 좌우 이동
        }
        tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        {
            anim.SetFloat(speedY, v, 0.01f, Time.deltaTime); // 앞뒤 이동
        }
        */

    }

    private void Shield()
    {
        if (Input.GetMouseButton(1))
        {
            
            isShielding = true;
            anim.SetBool(shield, true);
            
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isShielding = false;
            anim.SetBool(shield, false);
            
        }
    }

    private static void CursorLockUnLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void PlayerRotateY()
    {
        yRot += Input.GetAxis(mouseY) * ySencitivity * Time.deltaTime;
        xRot += Input.GetAxis(mouseX) * xSencitivity * Time.deltaTime;
        yRot = Mathf.Clamp(yRot, yMinLimit, yMaxLimit);
        cameraPivot.localEulerAngles = new Vector3(-yRot, 0f, 0f); // 마우스 상하 조작은 cameraPivot이 조작한다.
    }

    private void Sprint()
    {
        if(isShielding)
        {
            moveSpeed = 2f;
            anim.SetBool(run, false);
            isSprint = false;
            return;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = 8f;
            anim.SetBool(run, true);
            isSprint = true;
            isAttacking = false;
            
        }
        else
        {
            moveSpeed = 5f;
            anim.SetBool(run, false);
            isSprint = false;
        }
    }
    
    private void PlayerRotate()
    {
        tr.Rotate(Vector3.up * r * turnSpeed * Time.deltaTime); // tr.Rotate(0,y,0);
    }

    private void PlayerMove()
    {
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
        {
            anim.SetFloat(speedX, h, 0.01f, Time.deltaTime); // 좌우 이동
            anim.SetFloat(speedY, v, 0.01f, Time.deltaTime); // 앞뒤 이동
        }
    }

    private void IdleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && h == 0 && v == 0 ) // idle jump
        {
            if (isJumping) return;
            
            isJumping = true;
            anim.SetTrigger(idleJump);
            rb.velocity = Vector3.up * 3f;
        }
    }
    
    private void MovingJump()
    {
        if(isJumping) return;
        if (Input.GetKeyDown(KeyCode.Space) && v > 0.1f) // moving jump
        {
            isJumping = true;
            anim.SetTrigger(movingJump);
            rb.velocity = Vector3.up * 5f;
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.Space) && v > 0.1f) // moving jump
        {
            isJumping = true;
            anim.SetTrigger(movingJump);
            rb.velocity = Vector3.up * 7f;
        }
    }

    private void Attack()
    {
        
        if (isSprint) return;
        if (Input.GetButtonDown(fire1) && h == 0f && v == 0f &&  !anim.GetBool(run))
        {
                h = 0; v = 0; 
                isAttacking = true;
                anim.SetTrigger(attack);
                moveSpeed = 0f;
                Invoke("OnAttackEnd", 1.5f); // 1.5초 뒤에 공격이 끝난다
        }
    }
    private void OnAttackEnd()
    {
        isAttacking = false; // 공격이 끝날때
    }

    public void SwordSound()
    {
        source.PlayOneShot(swordClip, 1.0f);
    }
    private void OnCollisionEnter(Collision collision)  
    {
        isJumping = false;
    }
}
