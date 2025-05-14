using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonCtrl : MonoBehaviour
{
    [SerializeField] private Transform playerTr;
    [SerializeField] private Transform skeletonTr;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navi;
    [SerializeField] private float attackDis = 3f;
    [SerializeField] private float traceDis = 20.0f;

    private readonly int hashAttack = Animator.StringToHash("IsAttack_B");
    private readonly int hashTrace = Animator.StringToHash("IsTrace_B");


    void Start()
    {
        skeletonTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        navi = GetComponent<NavMeshAgent>();
        playerTr = GameObject.FindWithTag("Player").transform;
    }

   
    void Update()
    {
       float dis =  Vector3.Distance(skeletonTr.position, playerTr.position);

        if(dis <= attackDis)
        {
            animator.SetBool(hashAttack, true);
            navi.isStopped = true;
        }
        else if(dis <= traceDis) 
        {
            animator.SetBool(hashAttack, false);
            animator.SetBool(hashTrace, true);
            navi.isStopped = false;
            navi.destination = playerTr.position;
        }
        else
        {
            animator.SetBool(hashTrace, false);
            navi.isStopped = true;
        }
    }
}
