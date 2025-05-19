using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject pressEText; // 'E' ǥ�ø� ���� Text ������Ʈ
    public GameObject acceptButton; // ����Ʈ ���� ��ư
    public GameObject declinButton; // ����Ʈ ���� ��ư

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
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
        Cursor.lockState = CursorLockMode.None; // ���콺 Ŀ�� ��� ����
        Cursor.visible = true; // ���콺 Ŀ�� ���̱�

        DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        acceptButton.SetActive(false); // ����Ʈ ���� ��ư ��Ȱ��ȭ
        declinButton.SetActive(false); // ����Ʈ ���� ��ư ��Ȱ��ȭ

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        Debug.Log("��� ���: " + line); // ? �߰��غ�!!

        if (line.Contains("[����Ʈ ����]"))
        {
            // ����Ʈ ���� ��� ó��
            dialogueText.text = "���̷����� ���ָ� Ǯ�� �����ðڽ��ϱ�?\n[������ �����ø� ������ �����ϴ�.]";
            acceptButton.SetActive(true); // ����Ʈ ���� ��ư Ȱ��ȭ
            declinButton.SetActive(true); // ����Ʈ ���� ��ư Ȱ��ȭ

            
        }
        else
        {
            dialogueText.text = line;
        }
    }

    void Update()
    {
        if (!isDialogueActive) return;

        // ���콺 Ŭ�� or �����̽��ٷ� ��ȭ ����
        bool isClick = Input.GetMouseButtonDown(0);
        bool isSpace = Input.GetKeyDown(KeyCode.Space);


        if (dialogueText.text.Contains("�����ðڽ��ϱ�?"))
        {
            return;
        }

            if(isClick || isSpace)
        {
            // ��ȭ ����
            DisplayNextLine();
        }
        
        
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false; // ��ȭ�� ��Ȱ��ȭ��
        Time.timeScale = 1; // ���� �簳
        MariaCtrl.DisableControl = false;

        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ���
        Cursor.visible = false; // ���콺 Ŀ�� �����
    }

    public void ShowPressE(bool show)
    {
        pressEText.SetActive(show);
    }

    public void OnAccept()
    {
        EndDialogue();
        SceneManager.LoadScene("SkeletonScene"); // ����Ʈ ���� �� �� �̵�
    }

    public void OnDecline()
    {
        EndDialogue();
        // ����Ʈ ���� �� ó�� (��: ��ȭ ����)
        Debug.Log("����Ʈ ������");
    }

}


