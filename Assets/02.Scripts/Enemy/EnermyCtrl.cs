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

    [Header("체력 시스템")]
    public float maxHealth = 150f; // 최대 체력
    private float curHealth; // 현재 체력

    void Start()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").transform;
        navi = GetComponent<NavMeshAgent>();

        curHealth = maxHealth; // 현재 체력을 최대 체력으로 초기화

        if (animator == null) Debug.LogError("Animator is not assigned!");
        if (tr == null) Debug.LogError("Transform (tr) is not assigned!");
        if (playerTr == null) Debug.LogError("Player Transform (playerTr) is not assigned!");
        if (navi == null) Debug.LogError("NavMeshAgent (navi) is not assigned!");

        weaponCollider.enabled = false; // 처음에는 무기 콜라이더 비활성화
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage; // 체력 감소
        Debug.Log($"적 피격! 현재 체력: {curHealth}");

        if (curHealth <= 0)
        {
            Die(); // 죽음 처리
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (weaponCollider.enabled && other.CompareTag("Player"))
        {
            if (!weaponCollider.enabled) return; // 무기 콜라이더가 비활성화된 경우 무시

            PlayerCtrl player = other.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(30); // 플레이어에게 피해를 줌
            }
            other.GetComponent<PlayerCtrl>().OnHit();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        navi.isStopped = true; // 죽으면 이동 정지
        this.enabled = false; // 스크립트 비활성화
        Debug.Log("적이 죽었습니다!");
        // 추가적인 죽음 처리 로직 (예: 오브젝트 비활성화, 애니메이션 종료 후 삭제 등)
    }

    public void DisableWeapon()
    {
        weaponCollider.enabled = false;

    }

    public void EnableWeapon()
    {
        weaponCollider.enabled = true;

    }
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
}
