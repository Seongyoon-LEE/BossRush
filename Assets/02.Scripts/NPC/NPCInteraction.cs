using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{

    private bool isPlayerInRange = false; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� ������ �ִ��� ����

    private readonly string playerTag = "Player"; // �÷��̾� �±�

    public string[] dialogueLines = new string[]
{
    "�ȳ��ϼ���, ����.",
    "�̰��� ���̷����� ���ָ� ���� ���Դϴ�.",
    "��Ÿ��� �� ���ָ� Ǯ �� �־��.",
    "�����ֽðڽ��ϱ�? [����Ʈ ����]"
};
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = true; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� ������ ����
            DialogueManager.Instance.ShowPressE(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerInRange = false; // �÷��̾ NPC�� ��ȣ�ۿ��� �� �ִ� �������� ����
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
                Debug.Log("StartDialogue ȣ���");
                DialogueManager.Instance.ShowPressE(false);
            }
        }




        //if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        //{

        //    // ��ȭ ����
        //    DialogueManager.Instance.StartDialogue(dialogueLines);
        //    DialogueManager.Instance.ShowPressE(false); // ��ȭ ���� �� 'E' ǥ�� ����


        //}
    }
}
