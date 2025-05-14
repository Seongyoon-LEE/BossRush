using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MariaCtrl : MonoBehaviour
{
    private readonly string horizontal = "Horizontal";
    private readonly string vertical = "Vertical";
    private readonly string fire1 = "Fire1";
    private readonly string mouseX = "Mouse X";
    private readonly string speedX = "SpeedX";
    private readonly string speedY = "SpeedY";
    private readonly string idleJump = "IdleJump_T";
    private readonly string run = "IsRun_B";
    private readonly string movingJump = "MovingJump_T";
    private readonly string attack = "Attack_T";

    private Animator anim;
    private float h = 0f, v = 0f, r = 0f;  // ���콺 x 
    private Transform tr;
    private float turnSpeed = 300f;
    private float moveSpeed = 5f;
    private bool isAttacking = false; // ���������� �ƴ��� �Ǵ�
    private bool isSprint = false;
    private AudioSource source;
    public AudioClip swordClip; // ���� �Ҹ�
    
    

    void Start()
    {
        
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        tr = transform;
    }

    
    void Update()
    {
        if (isAttacking == false)
        {
            h = Input.GetAxis(horizontal);
            v = Input.GetAxis(vertical);
            r = Input.GetAxis(mouseX);
            Sprint();
            PlayerMove();
        }
        PlayerRotate();
        IdleJump();
        MovingJump();
        Attack();
        

        /*tr.Translate(Vector3.right * h * moveSpeed * Time.deltaTime);
        {
            anim.SetFloat(speedX, h, 0.01f, Time.deltaTime); // �¿� �̵�
        }
        tr.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        {
            anim.SetFloat(speedY, v, 0.01f, Time.deltaTime); // �յ� �̵�
        }
        */

    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = 8f;
            anim.SetBool(run, true);
            isAttacking = false;
            isSprint = true;
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
            anim.SetFloat(speedX, h, 0.01f, Time.deltaTime); // �¿� �̵�
            anim.SetFloat(speedY, v, 0.01f, Time.deltaTime); // �յ� �̵�
        }
    }

    private void IdleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && h == 0 && v == 0) // idle jump
        {
            anim.SetTrigger(idleJump);
        }
    }
    
    private void MovingJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && v > 0.1f) // moving jump
        {
            anim.SetTrigger(movingJump);
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
                Invoke("OnAttackEnd", 1.5f); // 1.5�� �ڿ� ������ ������
        }
        
        
       
    }
    private void OnAttackEnd()
    {
        isAttacking = false; // ������ ������
    }

    public void SwordSound()
    {
        source.PlayOneShot(swordClip, 1.0f);
    }
}
