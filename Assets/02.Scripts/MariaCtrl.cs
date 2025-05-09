using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class MariaCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    public string horizontal = "Horizontal";
    public string vertical = "Vertical";
    public string fire1 = "Fire1";
    public string mouseX = "Mouse X";
    public string speedX = "SpeedX";
    public string speedY = "SpeedY";
    public string idleJump = "IdleJump_T";
    public string run = "IsRun_B";
    public string movingJump = "MovingJump_T";
    private float h = 0f, v = 0f, r = 0f; // 마우스 x 
    private Transform tr;
    private float turnSpeed = 300f;
    private float moveSpeed = 5f;
    
    
    

    void Start()
    {
        anim = GetComponent<Animator>();
        tr = transform;
    }

    
    void Update()
    {
        h = Input.GetAxis(horizontal);
        v = Input.GetAxis(vertical);
        r = Input.GetAxis(mouseX);

        Sprint();
        PlayerRotate();
        PlayerMove();
        IdleJump();
        MovingJump();

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

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            moveSpeed = 8f;
            anim.SetBool(run, true);
        }
        else
        {
            moveSpeed = 5f;
            anim.SetBool(run, false);
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

}
