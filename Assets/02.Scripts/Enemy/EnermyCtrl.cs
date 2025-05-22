using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnermyCtrl : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform tr;
    [SerializeField] private NavMeshAgent navi;
    [SerializeField] private Animator animator;
    public float attackDist = 2f;
    public float traceDist = 10f;
    public float skillDist = 30f;
    public Collider weaponCollider;

    private bool isSkillAnimating = false;
    public float skillCooldown = 3f; // 스킬 쿨다운 시간
    private float lastSkillTime = -Mathf.Infinity; // 마지막 스킬 사용 시간
   
    void Start()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").transform;
        navi = GetComponent<NavMeshAgent>();

        if (animator == null) Debug.LogError("Animator is not assigned!");
        if (tr == null) Debug.LogError("Transform (tr) is not assigned!");
        if (playerTr == null) Debug.LogError("Player Transform (playerTr) is not assigned!");
        if (navi == null) Debug.LogError("NavMeshAgent (navi) is not assigned!");
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.tag == "Player")
    //    {
    //        PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
    //        curHealth -= playerCtrl.damage;

    //        Debug.Log("Attack : " + curHealth);
    //    }

    //}
    public void SkillAni()
    {
        if (animator != null && !isSkillAnimating)
        {
            animator.SetBool("isWalk", false);
            animator.SetTrigger("Skill");
            Debug.Log("Skill Triggered");
            isSkillAnimating = true; // 스킬 애니메이션 시작 플래그 설정
            lastSkillTime = Time.time; // 스킬 사용 시간 기록
            // 애니메이션 종료 시점에 isSkillAnimating을 false로 설정하는 이벤트 또는 코루틴 필요
        }
    }
    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);

        if (dist <= attackDist)
        {
            animator.SetBool("isAttack", true);
            navi.isStopped = true;
            isSkillAnimating = false; // 공격 범위에 들어오면 스킬 애니메이션 중지 간주
        }
        else if (dist <= traceDist)
        {
            animator.SetBool("isWalk", true);
            animator.SetBool("isAttack", false);
            navi.isStopped = false;
            navi.destination = playerTr.position;
            isSkillAnimating = false; // 추적 범위에 들어오면 스킬 애니메이션 중지 간주
        }
        else if (dist <= skillDist)
        {
            // 스킬 쿨다운이 지났고, 현재 스킬 애니메이션이 재생 중이 아닐 때만 스킬 발동
            if (Time.time >= lastSkillTime + skillCooldown && !isSkillAnimating)
            {
                SkillAni();
                Debug.Log("Skill State");
            }
        }
        else // idle 상태
        {
            animator.SetBool("isWalk", false); // 안전을 위해 추가
            animator.SetBool("isAttack", false);
            navi.isStopped = true;
            isSkillAnimating = false; // 기본 상태에서는 스킬 애니메이션 중지 간주
        }
    }
    public void OnSkillAnimationEnd()
    {
        isSkillAnimating = false;
        Debug.Log("Skill Animation Ended");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(weaponCollider.enabled && other.CompareTag("Player"))
        {
            Debug.Log("보스의 무기 공격 성공!");
            other.GetComponent<PlayerCtrl>().OnHit();
        }
    }

    public void EnableWeapon()
    {
        weaponCollider.enabled = true;
        Debug.Log("무기활성화");
    }
    public void DisableWeapon()
    {
        weaponCollider.enabled = false;
        Debug.Log("무기비활성화");
    }


}
