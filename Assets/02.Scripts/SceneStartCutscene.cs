using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SceneStartCutscene : MonoBehaviour
{
    public PlayableDirector director; // �ƽ��� ����� director
    public GameObject player; // �÷��̾� ������Ʈ
    public GameObject boss; // ���� ������Ʈ
    public GameObject virtualCam; // ���� ī�޶� ������Ʈ
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
        Debug.Log("�� StartBattle() ȣ���!");

        if (player != null && player.GetComponent<MariaCtrl>() != null)
            player.GetComponent<MariaCtrl>().enabled = true;
            
        if (boss != null && boss.GetComponent<EnermyCtrl>() != null)
            boss.GetComponent<EnermyCtrl>().enabled = true;

        if (virtualCam != null)
            virtualCam.SetActive(false);  // ���� ī�޶� ��Ȱ��ȭ!
    }


}
