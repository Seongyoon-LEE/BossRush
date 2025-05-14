using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 1.자기자신 스스로 z축으로 빨리 이동한다.
public class BulletCtrl : MonoBehaviour
{
    public float speed = 1500f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * speed);
    }

    
}
