using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{

    private bool isPlayerInRange = false; // 플레이어가 NPC와 상호작용할 수 있는 범위에 있는지 여부

    private readonly string playerTag = "Player"; // 플레이어 태그

    public string[] dialogueLines = new string[]
{
    "안녕하세요, 용사님.",
    "이곳은 스켈레톤의 저주를 받은 땅입니다.",
    "당신만이 이 저주를 풀 수 있어요.",
    "도와주시겠습니까? [퀘스트 수락]"
};
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true; // 플레이어가 NPC와 상호작용할 수 있는 범위에 들어옴
            DialogueManager.Instance.ShowPressE(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false; // 플레이어가 NPC와 상호작용할 수 있는 범위에서 나감
            DialogueManager.Instance.ShowPressE(false);
        }
    }


    void Update()
    {

        if (isPlayerInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("E key pressed!");
                DialogueManager.Instance.StartDialogue(dialogueLines);
                Debug.Log("StartDialogue 호출됨");
                DialogueManager.Instance.ShowPressE(false);
            }
        }




        //if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        //{

        //    // 대화 시작
        //    DialogueManager.Instance.StartDialogue(dialogueLines);
        //    DialogueManager.Instance.ShowPressE(false); // 대화 시작 후 'E' 표시 숨김


        //}
    }
}
