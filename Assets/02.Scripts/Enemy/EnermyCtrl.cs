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
    public float skillCooldown = 3f; // ��ų ��ٿ� �ð�
    private float lastSkillTime = -Mathf.Infinity; // ������ ��ų ��� �ð�

    [Header("ü�� �ý���")]
    public float maxHealth = 150f; // �ִ� ü��
    private float curHealth; // ���� ü��

    void Start()
    {
        tr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        playerTr = GameObject.FindWithTag("Player").transform;
        navi = GetComponent<NavMeshAgent>();

        curHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ

        if (animator == null) Debug.LogError("Animator is not assigned!");
        if (tr == null) Debug.LogError("Transform (tr) is not assigned!");
        if (playerTr == null) Debug.LogError("Player Transform (playerTr) is not assigned!");
        if (navi == null) Debug.LogError("NavMeshAgent (navi) is not assigned!");

        weaponCollider.enabled = false; // ó������ ���� �ݶ��̴� ��Ȱ��ȭ
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage; // ü�� ����
        Debug.Log($"�� �ǰ�! ���� ü��: {curHealth}");

        if (curHealth <= 0)
        {
            Die(); // ���� ó��
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (weaponCollider.enabled && other.CompareTag("Player"))
        {
            if (!weaponCollider.enabled) return; // ���� �ݶ��̴��� ��Ȱ��ȭ�� ��� ����

            PlayerCtrl player = other.GetComponent<PlayerCtrl>();
            if (player != null)
            {
                player.TakeDamage(30); // �÷��̾�� ���ظ� ��
            }
            other.GetComponent<PlayerCtrl>().OnHit();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
        navi.isStopped = true; // ������ �̵� ����
        this.enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
        Debug.Log("���� �׾����ϴ�!");
        // �߰����� ���� ó�� ���� (��: ������Ʈ ��Ȱ��ȭ, �ִϸ��̼� ���� �� ���� ��)
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
            isSkillAnimating = true; // ��ų �ִϸ��̼� ���� �÷��� ����
            lastSkillTime = Time.time; // ��ų ��� �ð� ���
            // �ִϸ��̼� ���� ������ isSkillAnimating�� false�� �����ϴ� �̺�Ʈ �Ǵ� �ڷ�ƾ �ʿ�
        }
    }
    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);

        if (dist <= attackDist)
        {
            animator.SetBool("isAttack", true);
            navi.isStopped = true;
            isSkillAnimating = false; // ���� ������ ������ ��ų �ִϸ��̼� ���� ����
        }
        else if (dist <= traceDist)
        {
            animator.SetBool("isWalk", true);
            animator.SetBool("isAttack", false);
            navi.isStopped = false;
            navi.destination = playerTr.position;
            isSkillAnimating = false; // ���� ������ ������ ��ų �ִϸ��̼� ���� ����
        }
        else if (dist <= skillDist)
        {
            // ��ų ��ٿ��� ������, ���� ��ų �ִϸ��̼��� ��� ���� �ƴ� ���� ��ų �ߵ�
            if (Time.time >= lastSkillTime + skillCooldown && !isSkillAnimating)
            {
                SkillAni();
                Debug.Log("Skill State");
            }
        }
        else // idle ����
        {
            animator.SetBool("isWalk", false); // ������ ���� �߰�
            animator.SetBool("isAttack", false);
            navi.isStopped = true;
            isSkillAnimating = false; // �⺻ ���¿����� ��ų �ִϸ��̼� ���� ����
        }
    }
    
    public void OnSkillAnimationEnd()
    {
        isSkillAnimating = false;
        Debug.Log("Skill Animation Ended");
    }
}
