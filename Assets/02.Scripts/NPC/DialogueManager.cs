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
    public GameObject pressEText; // 'E' 표시를 위한 Text 컴포넌트
    public GameObject acceptButton; // 퀘스트 수락 버튼
    public GameObject declinButton; // 퀘스트 거절 버튼

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isDialogueActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
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
        Cursor.lockState = CursorLockMode.None; // 마우스 커서 잠금 해제
        Cursor.visible = true; // 마우스 커서 보이기

        DisplayNextLine();
    }
    public void DisplayNextLine()
    {
        acceptButton.SetActive(false); // 퀘스트 수락 버튼 비활성화
        declinButton.SetActive(false); // 퀘스트 거절 버튼 비활성화

        if (dialogueQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = dialogueQueue.Dequeue();
        Debug.Log("대사 출력: " + line); // ? 추가해봐!!

        if (line.Contains("[퀘스트 수락]"))
        {
            // 퀘스트 수락 대사 처리
            dialogueText.text = "스켈레톤의 저주를 풀러 떠나시겠습니까?\n[수락을 누르시면 마을을 떠납니다.]";
            acceptButton.SetActive(true); // 퀘스트 수락 버튼 활성화
            declinButton.SetActive(true); // 퀘스트 거절 버튼 활성화

            
        }
        else
        {
            dialogueText.text = line;
        }
    }

    void Update()
    {
        if (!isDialogueActive) return;

        // 마우스 클릭 or 스페이스바로 대화 진행
        bool isClick = Input.GetMouseButtonDown(0);
        bool isSpace = Input.GetKeyDown(KeyCode.Space);


        if (dialogueText.text.Contains("떠나시겠습니까?"))
        {
            return;
        }

            if(isClick || isSpace)
        {
            // 대화 진행
            DisplayNextLine();
        }
        
        
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        isDialogueActive = false; // 대화가 비활성화됨
        Time.timeScale = 1; // 게임 재개
        MariaCtrl.DisableControl = false;

        Cursor.lockState = CursorLockMode.Locked; // 마우스 커서 잠금
        Cursor.visible = false; // 마우스 커서 숨기기
    }

    public void ShowPressE(bool show)
    {
        pressEText.SetActive(show);
    }

    public void OnAccept()
    {
        EndDialogue();
        SceneManager.LoadScene("SkeletonScene"); // 퀘스트 수락 후 씬 이동
    }

    public void OnDecline()
    {
        EndDialogue();
        // 퀘스트 거절 후 처리 (예: 대화 종료)
        Debug.Log("퀘스트 거절됨");
    }

}


