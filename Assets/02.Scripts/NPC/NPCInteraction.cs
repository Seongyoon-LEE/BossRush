using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    private bool isPlayerInRange = false; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� ������ �ִ��� ����
    private readonly string playerTag = "Player"; // �÷��̾� �±�
    public string[] dialogueLines;
    [SerializeField] Transform tr;
    [SerializeField] Transform playerTr; // �÷��̾��� Transform
    [SerializeField] float InteractDistance = 2f; // ��ȣ�ۿ� �Ÿ�

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            isPlayerInRange = true; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� ������ ����
            DialogueManager.Instance.ShowPressE(true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            isPlayerInRange = false; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� �������� ����
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
                isPlayerInRange = true; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� ������ ����
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

