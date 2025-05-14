using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]

public class ZombieCtrl : MonoBehaviour
{
    //1.NavMeshAgent를 이용 해서
    //2.Player가 추적 범위 안에 들어오면 추적하고
    //3.공격 범위 안에 들어오면 공격하는 로직 구현과 애니메이션 연동
    //4.추적범위와 공격범위를 구하려면 거리를 구해야함, 플레이어와 좀비의 위치를 알아야함
    [SerializeField] private NavMeshAgent navi;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform zombieTr;
    public Transform playerTr;


    public float traceDist = 20.0f; // 추점 범위
    public float attackDist = 3f; // 공격 범위

    private readonly int hashAttack = Animator.StringToHash("IsAttack_B"); //동적할당과 동시 문자열을 읽어서 정수로 변환 
    private readonly int hashTrace = Animator.StringToHash("IsTrace_B");

    void Start()
    {
        navi = GetComponent<NavMeshAgent>();
        playerTr = GameObject.FindWithTag("Player").transform; // 하이어라키에서 Player라는 태그를 가진 오브젝트의 트랜스폼을 가져옴 
        animator = GetComponent<Animator>();
        zombieTr = GetComponent<Transform>();
    }

    
    void Update()
    {
        float dist = Vector3.Distance(zombieTr.position,playerTr.position);
        
        if (dist <= attackDist) // 공격중일 때
        {
            animator.SetBool(hashAttack, true);
            
            navi.isStopped = true; // 공격중일 때 네비 추적정지
        }
        else if(dist <= traceDist) // 추적중일 때
        {
            animator.SetBool(hashAttack, false);
            animator.SetBool(hashTrace, true);
            navi.isStopped = false; // 추적 범위 안에 들어오면 네비 추적 시작 
            navi.destination = playerTr.position;
        }
        else
        {
            animator.SetBool(hashTrace, false);
            navi.isStopped = true; // 추적 범위 밖일 때 정지
        }
    }
}
