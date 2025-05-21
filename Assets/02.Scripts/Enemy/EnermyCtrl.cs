using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnermyCtrl : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform skeletonTr;
    [SerializeField] private NavMeshAgent navi;
    [SerializeField] private Animator animator;
    public float attackDist = 3.0f;
    public float traceDist = 20.0f;

    public int maxHealth;
    public int curHealth;

    void Start()
    {
        playerTr = GameObject.FindWithTag("Player").transform;
        skeletonTr = GetComponent<Transform>();
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
        float dist = Vector3.Distance(skeletonTr.position,playerTr.position);
        if(dist <= attackDist)
        {
            animator.SetBool("IsAttack_B", true);
            navi.isStopped = true;
        }
        else if(dist <= traceDist)
        {
            animator.SetBool("IsAttack_B", false);
            animator.SetBool("IsTrace_B", true);
            navi.isStopped = false;
            navi.destination = playerTr.position;
        }
        else
        {
            animator.SetBool("IsTrace_B", false);
            navi.isStopped = true;
        }
    }
    void CallEventHandle()
    {

    }
}
