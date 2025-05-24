using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
public class PlayerCtrl : MonoBehaviour
{
    [Header("체력 시스템")]
    public float maxHealth = 100f; // 최대 체력
    private float curHealth; // 현재 체력
    [SerializeField] private Collider playerWeaponCollider; // 무기 콜라이더

    [Header("Lock-on")]
    public Transform lockOnTarget;
    public Transform lockPoint; // 락온 타겟의 위치
    private bool isLockingOn = false;
    public static bool DisableControl = false;
    private bool isInvincible = false; // 무적 여부
    private bool isHitRecently = false; // 최근 피격 여부
    private Vector3 dashVelocity = Vector3.zero; // 대쉬 속도 저장용


    [Header("Lock-on-UI")]
    public GameObject lockOnUI; // UI 오브젝트 (Image)
    public Vector3 uiOffset = new Vector3(0, 2.0f, 0); // 타겟의 머리 위 위치 조정

    private readonly string horizontal = "Horizontal";
    private readonly string vertical = "Vertical";
    private readonly string mouseX = "Mouse X";
    private readonly string mouseY = "Mouse Y";
    private readonly string speedX = "SpeedX";
    private readonly string speedY = "SpeedY";
    private readonly string idleJump = "IdleJump_T";
    private readonly string shield = "Shield_B";

    private readonly int hashAttack = Animator.StringToHash("Attack_T");
    private readonly int hashParry = Animator.StringToHash("Parry_T");
    private readonly int hashStagger = Animator.StringToHash("Stagger_T");
    private readonly int hashDash = Animator.StringToHash("Dash_T");

    [SerializeField] private Transform cameraPivot;

    private Animator anim;
    private Transform tr;
    private Rigidbody rb;
    private AudioSource source;

    private float moveSpeed = 5f;
    private float turnSpeed = 300f;
    private float h = 0f, v = 0f, r = 0f;

    [Header("패링 관련")]
    public float parryWindow = 0.3f;
    private bool isParrying = false;

    public AudioClip swordClip;

    [Range(0, 100)] public float xSencitivity = 100.0f;
    [Range(0, 100)] public float ySencitivity = 100.0f;
    public float yMinLimit = -45f;
    public float yMaxLimit = 45f;
    public float xMinLimit = 360f;
    public float xMaxLimit = 360f;
    public float yRot = 0f;
    public float xRot = 0f;

    public enum PlayerState
    {
        Idle,
        Move,
        Jump,
        Attack,
        Parry,
        Stagger,
        Dash,
        Dead
    }

    private PlayerState currentState = PlayerState.Idle;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f);
        source = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        tr = transform;
        rb = GetComponent<Rigidbody>();

        curHealth = maxHealth; // 현재 체력을 최대 체력으로 초기화

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void Update()
    {
        if (tr == null || DisableControl || currentState == PlayerState.Dead) return;

        h = Input.GetAxis(horizontal);
        v = Input.GetAxis(vertical);
        r = Input.GetAxis(mouseX);
        LockOnUI();
        HandleInput();
        Shield();
        IdleJump();
        PlayerRotate();
        PlayerRotateY();
        CursorLockUnLock();
        Move();
    }
    private void FixedUpdate()
    {
        if(currentState == PlayerState.Dash)
        {
            rb.velocity = dashVelocity; // 대쉬 속도 적용
        }
    }
    public void PlayerEnableWeapon()
    {
        playerWeaponCollider.enabled = true; // 무기 콜라이더 활성화
        Debug.Log("플레이어 무기 활성화");
    }

    public void PlayerDisableWeapon()
    {
        playerWeaponCollider.enabled = false; // 무기 콜라이더 비활성화
        Debug.Log("플레이어 무기 비활성화");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!playerWeaponCollider.enabled) return;
        if (currentState != PlayerState.Attack) return;

        EnermyCtrl enemy = other.GetComponent<EnermyCtrl>();
        if (enemy == null)
            enemy = other.GetComponentInParent<EnermyCtrl>();

        if (enemy != null)
        {
            Debug.Log($"{other.name}에게 무기 명중!");
            enemy.TakeDamage(25f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || isHitRecently) return;
        {
            curHealth -= damage;
            Debug.Log($"플레이어 피격 ! 현재 체력 : {curHealth}");
            
            if(curHealth <= 0)
            {
                Die();
                return;
            }
                OnHit();            
        }
    }

    private void Die()
    {
        currentState = PlayerState.Dead;
        anim.SetTrigger("Die_T"); // Die 애니메이션 필요
        DisableControl = true;
        Debug.Log("플레이어 사망");
    }

    private void LockOnUI()
    {
        if (isLockingOn && lockOnTarget != null)
        {
            if (lockOnUI != null)
            {
               
                Vector3 screenPos = Camera.main.WorldToScreenPoint(lockPoint.position);
                lockOnUI.transform.position = screenPos;
                lockOnUI.SetActive(true);
            }

            Vector3 dir = lockOnTarget.position - tr.position;
            dir.y = 0f;

            // 플레이어 회전
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                tr.rotation = Quaternion.Slerp(tr.rotation, lookRot, Time.deltaTime * 10f);
            }
            // 카메라 회전 (pivot을 보스 방향으로)
            Vector3 camDir = lockOnTarget.position - cameraPivot.position;
            cameraPivot.rotation = Quaternion.Slerp(cameraPivot.rotation, Quaternion.LookRotation(camDir), Time.deltaTime * 5f);
        }
        else
        {
            if (lockOnUI != null)
            {
                lockOnUI.SetActive(false);
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            TryParry();
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryAttack();
        }

        if (currentState != PlayerState.Idle && currentState != PlayerState.Move) return;

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            float hInput = Input.GetAxisRaw("Horizontal");
            float vInput = Input.GetAxisRaw("Vertical");

            if (Mathf.Abs(hInput) > 0.1f || Mathf.Abs(vInput) > 0.1f)
            {
                TryDash(hInput, vInput); // 방향을 직접 넘겨주자!
            }
            else
            {
                // 방향 입력 없으면 바라보는 방향으로 대쉬
                TryDash(0f, 0f);
            }
        }

        if(Input.GetMouseButtonDown(2))
        {
            isLockingOn = lockOnTarget != null && !isLockingOn;
        }
    }

    void TryDash(float hInput, float vInput)
    {
        if (currentState == PlayerState.Attack || currentState == PlayerState.Parry || currentState == PlayerState.Dash || currentState == PlayerState.Stagger)
        {
            Debug.Log("? Dash 상태 불가 → 현재 상태: " + currentState);
            return;
        }  
        Debug.Log("? TryDash 실행됨");
        isInvincible = true; // 대쉬 시작시 무적
        currentState = PlayerState.Dash;
        anim.SetTrigger(hashDash);

        Debug.Log($"▶ Dash 입력: h = {hInput}, v = {vInput}");
        // 이동 방향 계산
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Debug.Log($"▶ 카메라 방향: forward = {camForward}, right = {camRight}");
        Vector3 moveDir = (camForward * vInput + camRight * hInput).normalized;
        Debug.Log($"▶ 계산된 moveDir = {moveDir}");
        if (moveDir.magnitude == 0)
        {
            moveDir = tr.forward; // 입력 없으면 바라보는 방향으로
            Debug.Log($"▶ moveDir이 0이므로 tr.forward로 대체 = {moveDir}");
        }
        float dashForce = 8f;
        dashVelocity = moveDir * dashForce;
        Debug.Log($"▶ rb.velocity 적용 = {rb.velocity}");
        // 일정 시간 후 상태 복구 (애니메이 길이와 맞춰야함)
        Invoke(nameof(EndDash), 0.6f); // <- Dash 애니 길이 기준으로 조정
    }
    void EndDash()
    {
        isInvincible = false; // 대쉬 종료시 무적 해제
        rb.velocity = Vector3.zero; // 대쉬 후 속도 초기화
        dashVelocity = Vector3.zero; // 대쉬 속도 초기화

        if (currentState == PlayerState.Dash)
            currentState = PlayerState.Idle;
    }
    void Move()
    {
        if (currentState == PlayerState.Dash || currentState == PlayerState.Stagger || currentState == PlayerState.Attack)
            return;

        Vector3 dir = new Vector3(h, 0, v).normalized;

        if (dir.magnitude > 0.1f)
        {
            Vector3 camForward = cameraPivot.forward;
            Vector3 camRight = cameraPivot.right;
            camForward.y = 0f;
            camRight.y = 0f;
            Vector3 moveDir = (camForward * v + camRight * h).normalized;

            transform.Translate(moveDir * moveSpeed * Time.deltaTime, Space.World);
            anim.SetFloat(speedX, h, 0.1f, Time.deltaTime);
            anim.SetFloat(speedY, v, 0.1f, Time.deltaTime);
            
            if(currentState != PlayerState.Attack && currentState != PlayerState.Parry && currentState != PlayerState.Stagger)
            currentState = PlayerState.Move;
        }
        else
        {
            anim.SetFloat(speedX, 0f, 0.1f, Time.deltaTime);
            anim.SetFloat(speedY, 0f, 0.1f, Time.deltaTime);
            currentState = PlayerState.Idle;
        }
    }

    void TryAttack()
    {
       
        if (currentState != PlayerState.Idle && currentState != PlayerState.Move) return;

        currentState = PlayerState.Attack;
        anim.SetTrigger(hashAttack);
    }

    void TryParry()
    {
        if (currentState != PlayerState.Idle && currentState != PlayerState.Move) return;

        isParrying = true;
        currentState = PlayerState.Parry;
        anim.SetTrigger(hashParry);
        Invoke(nameof(EndParry), parryWindow);
    }

    void EndParry()
    {
        isParrying = false;
        if (currentState == PlayerState.Parry)
            currentState = PlayerState.Idle;
    }

    public void OnHit()
    {
        if (isInvincible || isHitRecently) return; // 무적이거나 최근 피격이면 무시

        isHitRecently = true; // 피격 상태로 변경
        
        if (isParrying)
        {
            Debug.Log("패링 성공!");
            // 이펙트, 슬로우, 반격 준비 등 추가 가능
            return;
        }

        currentState = PlayerState.Stagger;
        anim.SetTrigger(hashStagger);

        Invoke(nameof(RecoverFromStagger), 1f);
        Invoke(nameof(ResetHitState), 0.5f);
    }
    void ResetHitState()
    {
        isHitRecently = false; // 피격 상태 해제
    }

    void RecoverFromStagger()
    {
        currentState = PlayerState.Idle;
    }

    private void Shield()
    {
        bool isRightMouse = Input.GetMouseButton(1);
        anim.SetBool(shield, isRightMouse);
    }

    private static void CursorLockUnLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void PlayerRotateY()
    {
        if (isLockingOn) return; // 락온중에 마우스 회전 막음

        yRot += Input.GetAxis(mouseY) * ySencitivity * Time.deltaTime;
        xRot += Input.GetAxis(mouseX) * xSencitivity * Time.deltaTime;
        yRot = Mathf.Clamp(yRot, yMinLimit, yMaxLimit);
        cameraPivot.localEulerAngles = new Vector3(-yRot, 0f, 0f);
    }

    private void PlayerRotate()
    {
        if (isLockingOn) return; // 락온중에 마우스 회전 막음
        tr.Rotate(Vector3.up * r * turnSpeed * Time.deltaTime);
    }

    private void IdleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentState == PlayerState.Jump) return;

            currentState = PlayerState.Jump;
            anim.SetTrigger(idleJump);
            rb.velocity = Vector3.up * 5f;
        }
    }

    public void OnAttackEnd()
    {
        if (currentState == PlayerState.Attack)
        {
            currentState = PlayerState.Idle;
            Debug.Log("공격 종료,아이들 복귀");
        }
    }

    public void SwordSound()
    {
        source.PlayOneShot(swordClip, 1.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState == PlayerState.Jump)
            currentState = PlayerState.Idle;
    }
}