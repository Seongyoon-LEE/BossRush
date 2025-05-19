using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private bool isPlayerInRange = false; // 플레이어가 NPC와 상호작용할 수 있는 범위에 있는지 여부
    private readonly string playerTag = "Player"; // 플레이어 태그
    public string[] dialogueLines;
    [SerializeField] Transform tr;
    [SerializeField] Transform playerTr; // 플레이어의 Transform
    [SerializeField] float InteractDistance = 2f; // 상호작용 거리

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            isPlayerInRange = true; // 플레이어가 NPC와 상호작용할 수 있는 범위에 들어옴
            DialogueManager.Instance.ShowPressE(true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            isPlayerInRange = false; // 플레이어가 NPC와 상호작용할 수 있는 범위에서 나감
            DialogueManager.Instance.ShowPressE(false);
        }
    }
    private void Start()
    {
        tr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag(playerTag).GetComponent<Transform>();
    }

    void Update()
    {
        float distance = Vector3.Distance(playerTr.position, tr.position);
        if(distance < InteractDistance)
        {
            if(!isPlayerInRange)
            {
                isPlayerInRange = true; // 플레이어가 NPC와 상호작용할 수 있는 범위에 있음
                DialogueManager.Instance.ShowPressE(true);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                DialogueManager.Instance.StartDialogue(dialogueLines);
                DialogueManager.Instance.ShowPressE(false);
            }
        }
        else if (isPlayerInRange)
        {
            isPlayerInRange = false;
            DialogueManager.Instance.ShowPressE(false);
        }
    }
}

