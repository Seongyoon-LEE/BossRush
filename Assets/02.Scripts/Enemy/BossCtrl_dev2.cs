using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BossCtrl_dev2 : MonoBehaviour
{
    public Animator animator;       // 인스펙터에서 할당
    public Transform tr;            // 인스펙터에서 할당
    public Transform playerTr;      // 인스펙터에서 할당
    public Rigidbody rb;            //리지드 바디
    public float attackDist = 2f;   // 평타 시도 범위
    //public NavMeshAgent navi;       // 인스펙터에서 할당
    public int hitMax = 2;          //한 사이클에 최대로 맞아주는 횟수
    public float moveSpeed = 2f;    //이동 속도
    public float dashSpeed = 5f;    //대시 속도
    public float moveRotSpeed = 5f; //회전 속도

    private Vector3 moveDir = Vector3.zero;
    private int currentHit;         //현재 사이클에 맞은 횟수
    private bool isHit = false;     //맞았다!!

    private bool isActing = false;  //행동중인가??

    private AnimatorStateInfo aniState; //애니메이터 상태 담을 변수

    public enum bossState       //보스 상태
    {
        Idle,Attack,Walk,Run,Hit,Death,Jump,BackDash
    }

    private bossState currentState;        //보스현재상태

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

        currentState = bossState.Idle;      //보스 상태 기본으로 설정

        aniState = animator.GetCurrentAnimatorStateInfo(0); //애니메이터 상태 가져올 정보 저장
                                                            //aniState. 으로 함수 사용 가능
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
        float dist = Vector3.Distance(tr.position, playerTr.position);          //dist 플레이어와의 거리 저장
        moveDir = playerTr.position - tr.position;

        if ((currentState == bossState.Idle || currentState == bossState.Walk)) //기본, 혹은 이동 상태일 때,
        {
            if (aniState.IsName("Attack") || isActing)  return;
            //기본 상태이지만 아직 공격 애니메이션 중일 때 --> return;

            if (dist <= attackDist)                     //평타 범위 안에 들어와있다면
            {                                           // #공격 or 뒤로 빠지기
                isActing = true;                        //행동중인가? = TRUE
                var i = UnityEngine.Random.Range(0, 2); // 0 또는 1 랜덤 반환
                if (i == 0)                             // 만약 0 이면
                {
                    currentState = bossState.Attack;    // 공격 상태로 전환하고
                    animator.SetFloat("SpeedX", 0f);    // 모든 이동 애니메이션 정지
                    animator.SetFloat("SpeedY", 0f);
                    animator.SetTrigger("Attack");      // 공격 애니메이션 시작
                    //navi.destination = tr.position;     // navi 정지
                    //navi.isStopped = true;
                    Debug.Log("Attack State");
                }
                else
                {
                    currentState = bossState.Walk;                          // 만약 0 이 아니면
                    animator.SetFloat("SpeedX", 0f);                        // 좌우 애니메이션 속도 0
                    animator.SetFloat("SpeedY", -1, 0.01f, Time.deltaTime); // 뒤로 걷기
                    //navi.destination = tr.position;                         // navi 정지
                    //navi.isStopped = true;
                    Debug.Log("Walk State");
                }
                Invoke("OnAnimationEnd", aniState.length);  // 애니메이션End 함수를 '애니메이션 길이' 시간 뒤에 실행
                Debug.Log($"length : {aniState.length}");
            }
            else if (currentState == bossState.Idle)    //플레이어가 평타 범위 안에 없고 기본 상태일 때
            {                                           // #플레이어 추적 or 좌우로 방황하기
                isActing = true;                        // 행동 = TRUE
                currentState = bossState.Walk;          // 현재 상태 = 걷기
                var i = UnityEngine.Random.Range(0, 2); // 0 또는 1 랜덤 반환
                if (i == 0)                             // 만약 0 이면
                {
                    i = UnityEngine.Random.value < 0.5f ? -1 : 1;           // i 는 50% 확률로 -1 또는 1 반환
                    animator.SetFloat("SpeedX", i, 0.01f, Time.deltaTime);  // 좌우 속도는 i 로 반환
                    //navi.destination = tr.position;                         // navi 정지
                    //navi.isStopped = true;
                }
                else
                {                                           // 만약 1 이면
                    rb.MovePosition(tr.position + moveDir * moveSpeed * Time.deltaTime);
                    //navi.destination = playerTr.position;   // 플레이어 추적 시작
                    //navi.isStopped = false;
                }
                Debug.Log("Walk State");
                Invoke("OnAnimationEnd", 0.5f);         // 0.5 초 뒤에 애니메이션End 함수 실행
            }
        }

    }

    public void OnAnimationEnd()        // 애니메이션End
    {
        isActing = false;               // 행동중인가? = FALSE
        currentState = bossState.Idle;  // 현재 상태 = 기본 상태
        Debug.Log("Animation Ended");   
    }

    public void Hit()                   // 피격시 때 구현중..
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