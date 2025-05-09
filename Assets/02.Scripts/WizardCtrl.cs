using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardCtrl : MonoBehaviour
{
    public Transform tr;
    public Animator anim;
    public string horizontal = "Horizontal";
    public string vertical = "Vertical";
    public string mouseX = "Mouse X";
    public string speedX = "SpeedX";
    public string speedY = "SpeedY";
    public float h = 0f, v = 0f, r = 0f;
    public float movespeed = 5f;
    public float turnSpeed = 90f;

    void Start()
    {
        tr=GetComponent<Transform>();
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {


        h = Input.GetAxis(horizontal);
        v = Input.GetAxis(vertical);
        r = Input.GetAxis(mouseX);

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        tr.Translate(moveDir.normalized * movespeed * Time.deltaTime);
        {
            anim.SetFloat(speedX, h * 0.01f * Time.deltaTime);
            anim.SetFloat(speedY, v * 0.01f * Time.deltaTime);
        }
        PlayerRotate();
    }
    private void PlayerRotate()
    {
        tr.Rotate(Vector3.up * r * turnSpeed * Time.deltaTime); // tr.Rotate(0,y,0);
    }
}
