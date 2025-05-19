using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SceneStartCutscene : MonoBehaviour
{
    public PlayableDirector director; // 컷신을 재생할 director
    public GameObject player; // 플레이어 오브젝트
    public GameObject boss; // 보스 오브젝트
    public GameObject virtualCam; // 가상 카메라 오브젝트
    void Start()
    {
        if (director != null) director.Play();

        if (player != null)
        {
            var playerCtrl = player.GetComponent<MariaCtrl>();
            if (playerCtrl != null)
                playerCtrl.enabled = false;
        }

        if (boss != null)
        {
            var bossCtrl = boss.GetComponent<EnermyCtrl>();
            if (bossCtrl != null)
                bossCtrl.enabled = false;
        }
    }
    public void StartBattle()
    {
        Debug.Log("▶ StartBattle() 호출됨!");

        if (player != null && player.GetComponent<MariaCtrl>() != null)
            player.GetComponent<MariaCtrl>().enabled = true;
            
        if (boss != null && boss.GetComponent<EnermyCtrl>() != null)
            boss.GetComponent<EnermyCtrl>().enabled = true;

        if (virtualCam != null)
            virtualCam.SetActive(false);  // 가상 카메라 비활성화!
    }


}
