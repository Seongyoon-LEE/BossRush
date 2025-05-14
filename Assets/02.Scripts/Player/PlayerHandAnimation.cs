using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// 1.Left Shift && W 키를 누를시 총을 접는 애니메이션을 재생하는 스크립트
// 2.Left Shift || W 둘중 하나라도 떼면 애니메이션이 멈추고 총을 겨누는 애니메이션이 재생되도록 구현  
public class PlayerHandAnimation : MonoBehaviour
{
    public Animation anim;
    private readonly string runAni = "running";
    private readonly string runStopAni = "runStop";
    private readonly string fireInput = "Fire1";
    private readonly string fireAni = "fire";

    private bool isRunning;

    void Start()
    {
        //anim = GetComponentInChildren<Animation>(); 자식들의 애니메이션이 많아지면 찾기 힘듬 
        //anim = GetComponentsInChildren<Animation>()[0]; 자식들중 배열에 맞는 자식의 애니메이션을 가지고온다.

        //자기 자신의 첫번째 자식 오브젝트를 찾고 그 자식의 첫번째 오브젝트의 자식 오브젝트를 찾는다
        anim = transform.GetChild(0).GetChild(0).GetComponent<Animation>();
        isRunning = false;
    }
    void Update()
    {
        PlayerRunAni();
        PlayerFire();
    }
    public void PlayerFire()
    {
        if (Input.GetButton(fireInput) && !isRunning)
        {
            anim.Play(fireAni);
        }
    }
    private void PlayerRunAni()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            anim.Play(runAni);
            isRunning = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.W))
        {
            anim.Play(runStopAni);
            isRunning = false;
        }
    }
}
