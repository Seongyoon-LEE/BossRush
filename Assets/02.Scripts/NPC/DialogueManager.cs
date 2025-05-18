using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject pressEText; // 'E' ǥ�ø� ���� Text ������Ʈ

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueActive = false; 

     void Awake()
    {
        if(Instance == null) Instance = this;   
    }

    public void StartDialogue(string[] lines)
    {
        Debug.Log("StartDialogue �����! lines ����: " + lines.Length);

        dialogueQueue.Clear();

        foreach (var line in lines)
        {
            dialogueQueue.Enqueue(line);
            Debug.Log("ť�� �߰��� ���: " + line);
        }
            dialoguePanel.SetActive(true); // �г� ���� 
            isDialogueActive = true; // ��ȭ�� Ȱ��ȭ��
            Time.timeScale = 0; // ���� �Ͻ� ����
            MariaCtrl.DisableControl = true;

            DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            Debug.Log("��ȭ ��!");
            EndDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        Debug.Log("��� ���: " + line); // ? �߰��غ�!!

        if (line.Contains("[����Ʈ ����]"))
        {
            // ����Ʈ ���� ��� ó��
            dialogueText.text = "���̷����� ���ָ� Ǯ�� �����ðڽ��ϱ�?\n[�����̽�: ����]";
        }
        else
        {
            dialogueText.text = line;
        }
    }
    
    void Update()
    {
        if(dialoguePanel.activeSelf &&Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextLine();
        }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false; // ��ȭ�� ��Ȱ��ȭ��
        Time.timeScale = 1; // ���� �簳
        MariaCtrl.DisableControl = false;
    }

    public void ShowPressE(bool show)
    {
        Debug.Log("ShowPressE �����: " + show);
        if (pressEText == null)
        {
            Debug.LogWarning("pressEText�� ������� �ʾҽ��ϴ�!");
            return;
        }
        pressEText.SetActive(show);
    }
}
