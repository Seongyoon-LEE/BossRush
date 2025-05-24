using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
public class PlayerCtrl : MonoBehaviour
{
    [Header("ü�� �ý���")]
    public float maxHealth = 100f; // �ִ� ü��
    private float curHealth; // ���� ü��
    [SerializeField] private Collider playerWeaponCollider; // ���� �ݶ��̴�

    [Header("Lock-on")]
    public Transform lockOnTarget;
    public Transform lockPoint; // ���� Ÿ���� ��ġ
    private bool isLockingOn = false;
    public static bool DisableControl = false;
    private bool isInvincible = false; // ���� ����
    private bool isHitRecently = false; // �ֱ� �ǰ� ����
    private Vector3 dashVelocity = Vector3.zero; // �뽬 �ӵ� �����


    [Header("Lock-on-UI")]
    public GameObject lockOnUI; // UI ������Ʈ (Image)
    public Vector3 uiOffset = new Vector3(0, 2.0f, 0); // Ÿ���� �Ӹ� �� ��ġ ����

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

    [Header("�и� ����")]
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

        curHealth = maxHealth; // ���� ü���� �ִ� ü������ �ʱ�ȭ

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
            rb.velocity = dashVelocity; // �뽬 �ӵ� ����
        }
    }
    public void PlayerEnableWeapon()
    {
        playerWeaponCollider.enabled = true; // ���� �ݶ��̴� Ȱ��ȭ
        Debug.Log("�÷��̾� ���� Ȱ��ȭ");
    }

    public void PlayerDisableWeapon()
    {
        playerWeaponCollider.enabled = false; // ���� �ݶ��̴� ��Ȱ��ȭ
        Debug.Log("�÷��̾� ���� ��Ȱ��ȭ");
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
            Debug.Log($"{other.name}���� ���� ����!");
            enemy.TakeDamage(25f);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible || isHitRecently) return;
        {
            curHealth -= damage;
            Debug.Log($"�÷��̾� �ǰ� ! ���� ü�� : {curHealth}");
            
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
        anim.SetTrigger("Die_T"); // Die �ִϸ��̼� �ʿ�
        DisableControl = true;
        Debug.Log("�÷��̾� ���");
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

            // �÷��̾� ȸ��
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                tr.rotation = Quaternion.Slerp(tr.rotation, lookRot, Time.deltaTime * 10f);
            }
            // ī�޶� ȸ�� (pivot�� ���� ��������)
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
                TryDash(hInput, vInput); // ������ ���� �Ѱ�����!
            }
            else
            {
                // ���� �Է� ������ �ٶ󺸴� �������� �뽬
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
            Debug.Log("? Dash ���� �Ұ� �� ���� ����: " + currentState);
            return;
        }  
        Debug.Log("? TryDash �����");
        isInvincible = true; // �뽬 ���۽� ����
        currentState = PlayerState.Dash;
        anim.SetTrigger(hashDash);

        Debug.Log($"�� Dash �Է�: h = {hInput}, v = {vInput}");
        // �̵� ���� ���
        Vector3 camForward = cameraPivot.forward;
        Vector3 camRight = cameraPivot.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Debug.Log($"�� ī�޶� ����: forward = {camForward}, right = {camRight}");
        Vector3 moveDir = (camForward * vInput + camRight * hInput).normalized;
        Debug.Log($"�� ���� moveDir = {moveDir}");
        if (moveDir.magnitude == 0)
        {
            moveDir = tr.forward; // �Է� ������ �ٶ󺸴� ��������
            Debug.Log($"�� moveDir�� 0�̹Ƿ� tr.forward�� ��ü = {moveDir}");
        }
        float dashForce = 8f;
        dashVelocity = moveDir * dashForce;
        Debug.Log($"�� rb.velocity ���� = {rb.velocity}");
        // ���� �ð� �� ���� ���� (�ִϸ��� ���̿� �������)
        Invoke(nameof(EndDash), 0.6f); // <- Dash �ִ� ���� �������� ����
    }
    void EndDash()
    {
        isInvincible = false; // �뽬 ����� ���� ����
        rb.velocity = Vector3.zero; // �뽬 �� �ӵ� �ʱ�ȭ
        dashVelocity = Vector3.zero; // �뽬 �ӵ� �ʱ�ȭ

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
        if (isInvincible || isHitRecently) return; // �����̰ų� �ֱ� �ǰ��̸� ����

        isHitRecently = true; // �ǰ� ���·� ����
        
        if (isParrying)
        {
            Debug.Log("�и� ����!");
            // ����Ʈ, ���ο�, �ݰ� �غ� �� �߰� ����
            return;
        }

        currentState = PlayerState.Stagger;
        anim.SetTrigger(hashStagger);

        Invoke(nameof(RecoverFromStagger), 1f);
        Invoke(nameof(ResetHitState), 0.5f);
    }
    void ResetHitState()
    {
        isHitRecently = false; // �ǰ� ���� ����
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
        if (isLockingOn) return; // �����߿� ���콺 ȸ�� ����

        yRot += Input.GetAxis(mouseY) * ySencitivity * Time.deltaTime;
        xRot += Input.GetAxis(mouseX) * xSencitivity * Time.deltaTime;
        yRot = Mathf.Clamp(yRot, yMinLimit, yMaxLimit);
        cameraPivot.localEulerAngles = new Vector3(-yRot, 0f, 0f);
    }

    private void PlayerRotate()
    {
        if (isLockingOn) return; // �����߿� ���콺 ȸ�� ����
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
            Debug.Log("���� ����,���̵� ����");
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