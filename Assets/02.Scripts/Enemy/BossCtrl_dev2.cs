using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossCtrl_dev2 : MonoBehaviour
{
    public Animator animator;       // �ν����Ϳ��� �Ҵ�
    public Transform tr;            // �ν����Ϳ��� �Ҵ�
    public Transform playerTr;      // �ν����Ϳ��� �Ҵ�
    public Rigidbody rb;            //������ �ٵ�
    public float attackDist = 2f;   // ��Ÿ �õ� ����
    //public NavMeshAgent navi;       // �ν����Ϳ��� �Ҵ�
    public int hitMax = 2;          //�� ����Ŭ�� �ִ�� �¾��ִ� Ƚ��
    public float moveSpeed = 2f;    //�̵� �ӵ�
    public float dashSpeed = 5f;    //��� �ӵ�
    public float moveRotSpeed = 5f; //ȸ�� �ӵ�

    private Vector3 moveDir = Vector3.zero;
    private int currentHit;         //���� ����Ŭ�� ���� Ƚ��
    private bool isHit = false;     //�¾Ҵ�!!

    private bool isActing = false;  //�ൿ���ΰ�??

    private AnimatorStateInfo aniState; //�ִϸ����� ���� ���� ����

    public enum bossState       //���� ����
    {
        Idle,Attack,Walk,Run,Hit,Death,Jump,BackDash
    }

    private bossState currentState;        //�����������

    void Start()
    {
        
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        //navi = GetComponent<NavMeshAgent>();

        if (animator == null) Debug.LogError("Animator is not assigned!");
        if (tr == null) Debug.LogError("Transform (tr) is not assigned!");
        if (playerTr == null) Debug.LogError("Player Transform (playerTr) is not assigned!");
        if (rb == null) Debug.LogError("Rigidbody (navi) is not assigned!");
        //if (navi == null) Debug.LogError("NavMeshAgent (navi) is not assigned!");

        currentState = bossState.Idle;      //���� ���� �⺻���� ����

        aniState = animator.GetCurrentAnimatorStateInfo(0); //�ִϸ����� ���� ������ ���� ����
                                                            //aniState. ���� �Լ� ��� ����
    }

    private void FixedUpdate()
    {
        if ((currentState == bossState.Idle || currentState == bossState.Walk))
            if (moveDir != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(tr.forward ,moveDir, moveRotSpeed * Time.deltaTime);
        }

    }

    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);          //dist �÷��̾���� �Ÿ� ����
        moveDir = playerTr.position - tr.position;

        if ((currentState == bossState.Idle || currentState == bossState.Walk)) //�⺻, Ȥ�� �̵� ������ ��,
        {
            if (aniState.IsName("Attack") || isActing)  return;
            //�⺻ ���������� ���� ���� �ִϸ��̼� ���� �� --> return;

            if (dist <= attackDist)                     //��Ÿ ���� �ȿ� �����ִٸ�
            {                                           // #���� or �ڷ� ������
                isActing = true;                        //�ൿ���ΰ�? = TRUE
                var i = UnityEngine.Random.Range(0, 2); // 0 �Ǵ� 1 ���� ��ȯ
                if (i == 0)                             // ���� 0 �̸�
                {
                    currentState = bossState.Attack;    // ���� ���·� ��ȯ�ϰ�
                    animator.SetFloat("SpeedX", 0f);    // ��� �̵� �ִϸ��̼� ����
                    animator.SetFloat("SpeedY", 0f);
                    animator.SetTrigger("Attack");      // ���� �ִϸ��̼� ����
                    //navi.destination = tr.position;     // navi ����
                    //navi.isStopped = true;
                    Debug.Log("Attack State");
                }
                else
                {
                    currentState = bossState.Walk;                          // ���� 0 �� �ƴϸ�
                    animator.SetFloat("SpeedX", 0f);                        // �¿� �ִϸ��̼� �ӵ� 0
                    animator.SetFloat("SpeedY", -1, 0.01f, Time.deltaTime); // �ڷ� �ȱ�
                    //navi.destination = tr.position;                         // navi ����
                    //navi.isStopped = true;
                    Debug.Log("Walk State");
                }
                Invoke("OnAnimationEnd", aniState.length);  // �ִϸ��̼�End �Լ��� '�ִϸ��̼� ����' �ð� �ڿ� ����
                Debug.Log($"length : {aniState.length}");
            }
            else if (currentState == bossState.Idle)    //�÷��̾ ��Ÿ ���� �ȿ� ���� �⺻ ������ ��
            {                                           // #�÷��̾� ���� or �¿�� ��Ȳ�ϱ�
                isActing = true;                        // �ൿ = TRUE
                currentState = bossState.Walk;          // ���� ���� = �ȱ�
                var i = UnityEngine.Random.Range(0, 2); // 0 �Ǵ� 1 ���� ��ȯ
                if (i == 0)                             // ���� 0 �̸�
                {
                    i = UnityEngine.Random.value < 0.5f ? -1 : 1;           // i �� 50% Ȯ���� -1 �Ǵ� 1 ��ȯ
                    animator.SetFloat("SpeedX", i, 0.01f, Time.deltaTime);  // �¿� �ӵ��� i �� ��ȯ
                    //navi.destination = tr.position;                         // navi ����
                    //navi.isStopped = true;
                }
                else
                {                                           // ���� 1 �̸�
                    rb.MovePosition(tr.position + moveDir * moveSpeed * Time.deltaTime);
                    //navi.destination = playerTr.position;   // �÷��̾� ���� ����
                    //navi.isStopped = false;
                }
                Debug.Log("Walk State");
                Invoke("OnAnimationEnd", 0.5f);         // 0.5 �� �ڿ� �ִϸ��̼�End �Լ� ����
            }
        }

    }

    public void OnAnimationEnd()        // �ִϸ��̼�End
    {
        isActing = false;               // �ൿ���ΰ�? = FALSE
        currentState = bossState.Idle;  // ���� ���� = �⺻ ����
        Debug.Log("Animation Ended");   
    }

    public void Hit()                   // �ǰݽ� �� ������..
    {
        currentState = bossState.Hit;
        animator.SetTrigger("Hit");
        Invoke("HitMinus", 1.0f);
        //navi.destination = tr.position;
        //navi.isStopped = true;
        Debug.Log("Hit");
    }

    public void HitMinus()
    {
        if (currentHit > 0)
        {
            currentHit--;
        }
    }
}