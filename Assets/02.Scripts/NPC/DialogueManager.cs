using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject pressEText; // 'E' 표시를 위한 Text 컴포넌트

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueActive = false; 

     void Awake()
    {
        if(Instance == null) Instance = this;   
    }

    public void StartDialogue(string[] lines)
    {
        Debug.Log("StartDialogue 실행됨! lines 길이: " + lines.Length);

        dialogueQueue.Clear();

        foreach (var line in lines)
        {
            dialogueQueue.Enqueue(line);
            Debug.Log("큐에 추가된 대사: " + line);
        }
            dialoguePanel.SetActive(true); // 패널 열기 
            isDialogueActive = true; // 대화가 활성화됨
            Time.timeScale = 0; // 게임 일시 정지
            MariaCtrl.DisableControl = true;

            DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        if (dialogueQueue.Count == 0)
        {
            Debug.Log("대화 끝!");
            EndDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        Debug.Log("대사 출력: " + line); // ? 추가해봐!!

        if (line.Contains("[퀘스트 수락]"))
        {
            // 퀘스트 수락 대사 처리
            dialogueText.text = "스켈레톤의 저주를 풀러 떠나시겠습니까?\n[스페이스: 진행]";
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
        isDialogueActive = false; // 대화가 비활성화됨
        Time.timeScale = 1; // 게임 재개
        MariaCtrl.DisableControl = false;
    }

    public void ShowPressE(bool show)
    {
        Debug.Log("ShowPressE 실행됨: " + show);
        if (pressEText == null)
        {
            Debug.LogWarning("pressEText가 연결되지 않았습니다!");
            return;
        }
        pressEText.SetActive(show);
    }
}
