using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossCtrl : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform tr;
    [SerializeField] private NavMeshAgent navi;
    [SerializeField] private Animator animator;
    public float attackDist = 3.0f;
    public float traceDist = 20.0f;

    public int maxHealth;
    public int curHealth;
    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        tr = GetComponent<Transform>();
        navi = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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

    void Update()
    {
        float dist = Vector3.Distance(tr.position, playerTr.position);
        if (dist <= attackDist)
        {
            animator.SetBool("isAttack", true);
            navi.isStopped = true;
        }
        else if (dist <= traceDist)
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("isWalk", true);
            navi.isStopped = false;
            navi.destination = playerTr.position;
        }
        else
        {
            animator.SetBool("isWalk", false);
            navi.isStopped = true;
        }
    }
}
