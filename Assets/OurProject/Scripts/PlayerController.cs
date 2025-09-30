using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;
    private Rigidbody rb;

    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 6f;
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float jumpForce = 450f;
    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded = true;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower = 1500f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1.5f;
    private bool canDash = true;

    [Header("Mouse Settings")]
    [SerializeField] private float rotateSpeedX = 10f;
    [SerializeField] private float rotateSpeedY = 10f;
    [SerializeField] private float angleLimitUp = 290f;
    [SerializeField] private float angleLimitDown = 70f;
    private float pitch = 0f;
    private float yaw = 0f;
    private float mouseX;
    private float mouseY;

    [Header("Attack Settings")]
    [SerializeField] private float comboEndDelay = 1f;
    private float attackState;
    private bool canAttack = true;
    private bool comboAvailable = false;

    [Header("IK Settings")]
    [SerializeField] private Transform rightFootTarget;
    [SerializeField] private Transform leftFootTarget;

    [Header("Effects")]
    [SerializeField] private GameObject trailObject;
    [SerializeField] private GameObject fireTrailObject;
    [SerializeField] private GameObject fireParticleObject;
    private TrailRenderer trail;
    private TrailRenderer fireTrail;
    private ParticleSystem fireEffect;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        trail = trailObject.GetComponent<TrailRenderer>();
        trail.emitting = false; // トレイル描画停止

        fireTrail = fireTrailObject.GetComponent<TrailRenderer>();
        fireTrail.emitting = false; // トレイル描画停止

        fireEffect = fireParticleObject.GetComponent<ParticleSystem>();
        fireEffect.Stop();  // パーティクル再生停止
    }

    private void Update()
    {
        attackState = animator.GetFloat("Attack");

        HandleMovement();
        HandleAttack();
        HandleDash();
        HandleJump();
        HandleRotation();
    }

    /// <summary>
    /// 移動, アニメーション制御
    /// </summary>
    /// <returns></returns>
    private void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")); // 移動方向ベクトル

        Vector3 forward = Camera.main.transform.forward;    // カメラ前方向
        forward.y = 0;  // 上下成分削除
        forward.Normalize();
        Vector3 right = Camera.main.transform.right;    // カメラ右方向
        right.y = 0;    // 上下成分削除
        right.Normalize();

        float speed = attackState == 0 ? normalSpeed : attackSpeed;
        moveDirection = (forward * input.z + right * input.x).normalized * speed * Time.deltaTime;  // 移動ベクトル

        transform.Translate(moveDirection, Space.World);    // 移動ベクトル分 (moveDirection) 移動

        animator.SetFloat("MoveSpeed", moveDirection.magnitude);
        UpdateMoveAnimation(input);
    }

    /// <summary>
    /// アニメーション更新
    /// </summary>
    /// <param name="input"></param>
    private void UpdateMoveAnimation(Vector3 input)
    {
        animator.SetBool("Forward", input.z > 0);
        animator.SetBool("Back", input.z < 0);
        animator.SetBool("Right", input.x > 0);
        animator.SetBool("Left", input.x < 0);
    }

    /// <summary>
    /// 攻撃制御
    /// </summary>
    private void HandleAttack()
    {
        if (!canAttack) return;

        // First attack (左クリック)
        if (!comboAvailable && attackState == 0 && Input.GetMouseButtonDown(0))
        {
            rb.linearVelocity = Vector3.zero;
            animator.SetFloat("Attack", 1f);
        }

        // Combo attack (左クリック)
        if (comboAvailable && Input.GetMouseButtonDown(0) && attackState < 4)
        {
            rb.linearVelocity = Vector3.zero;
            comboAvailable = false;
            attackState += 1f;
            animator.SetFloat("Attack", attackState);
        }

        // Fire attack (右クリック)
        if (comboAvailable && Input.GetMouseButtonDown(1))
        {
            rb.linearVelocity = Vector3.zero;
            animator.SetFloat("Attack", 10f);
            canAttack = false;
            comboAvailable = false;
            fireEffect.Play();  // パーティクル再生
            return;
        }
    }

    /// <summary>
    /// ダッシュ制御
    /// </summary>
    private void HandleDash()
    {
        // ダッシュ (左シフト)
        if (Input.GetButtonDown("Fire3") && canDash)
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;  // 移動方向ベクトル (正規化 → 距離調整)
            // Transform.TransformDirection(direction): ローカル方向 (プレイヤ正面) → ワールド方向
            rb.AddForce(transform.TransformDirection(direction) * dashPower, ForceMode.Impulse);
            canDash = false;
            StartCoroutine(DashRoutine());
        }
    }

    /// <summary>
    /// ダッシュ時間管理 (並列処理)
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashRoutine()
    {
        yield return new WaitForSeconds(dashDuration);  // ダッシュ持続時間待機
        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashCooldown);  // ダッシュクールタイム待機
        canDash = true;
    }

    /// <summary>
    /// ジャンプ制御
    /// </summary>
    private void HandleJump()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;    // レイ発射位置

        if (Physics.SphereCast(origin, 0.3f, Vector3.down, out RaycastHit hit, 0.6f))   // レイヒット (接地) 時
        {
            isGrounded = true;
            animator.SetBool("Jump", false);

            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  // ジャンプ (ワールド空間上方向)
            }
        }
        else   // 空中
        {
            isGrounded = false;
            animator.SetBool("Jump", true);
        }
    }

    /// <summary>
    /// 回転制御
    /// </summary>
    private void HandleRotation()
    {
        mouseX = Input.GetAxis("Mouse X") * rotateSpeedX * Time.deltaTime;  // 左右回転量 (右方向 → 正)
        mouseY = Input.GetAxis("Mouse Y") * rotateSpeedY * Time.deltaTime;  // 上下回転量 (上方向 → 正)

        yaw += mouseX;
        pitch -= mouseY;    // 上方向 → 負
        pitch = Mathf.Clamp(pitch, -angleLimitUp, angleLimitDown);  // 上下回転制限

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);  // プレイヤ, カメラ → 横回転
        Camera.main.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);  // カメラ → 縦回転
    }

    #region AnimationEvents

    /// <summary>
    /// 攻撃開始時
    /// </summary>
    private void AttackStart()
    {
        if (attackState == 10f)
            fireTrail.emitting = true;  // トレイル描画再生
        else if (attackState > 0 && attackState < 5)
            trail.emitting = true;  // トレイル描画再生
    }

    /// <summary>
    /// 斬撃終了時
    /// </summary>
    private void Hit()
    {
        if (attackState == 10f)
        {
            fireTrail.emitting = false; // トレイル描画停止
        }
        else
        {
            comboAvailable = true;
            trail.emitting = false; // トレイル描画停止
        }
    }

    /// <summary>
    /// 攻撃終了時
    /// </summary>
    private void AttackEnd()
    {
        comboAvailable = false;
        animator.SetFloat("Attack", 0f);    // 攻撃アニメーション終了

        if (attackState == 4f || attackState == 10f)
        {
            canAttack = false;
            fireEffect.Stop();  // パーティクル再生停止
            StartCoroutine(ComboEndRoutine());
        }
    }

    /// <summary>
    /// 右足着地時
    /// </summary>
    private void FootR()
    {
        // 足音
    }

    /// <summary>
    /// 左足着地時
    /// </summary>
    private void FootL()
    {
        // 足音
    }

    #endregion

    /// <summary>
    /// コンボ終了ディレイ管理 (並列処理)
    /// </summary>
    /// <returns></returns>
    private IEnumerator ComboEndRoutine()
    {
        yield return new WaitForSeconds(comboEndDelay); // コンボ終了ディレイ待機
        canAttack = true;
    }

    /// <summary>
    /// IK 用コールバック
    /// </summary>
    private void OnAnimatorIK()
    {
        if (!isGrounded) return;

        float moveSpeed = animator.GetFloat("MoveSpeed");

        ApplyFootIK(AvatarIKGoal.RightFoot, rightFootTarget, moveSpeed);
        ApplyFootIK(AvatarIKGoal.LeftFoot, leftFootTarget, moveSpeed);
    }

    /// <summary>
    /// IK 設定
    /// </summary>
    /// <param name="foot"></param>
    /// <param name="footTarget"></param>
    /// <param name="moveSpeed"></param>
    private void ApplyFootIK(AvatarIKGoal foot, Transform footTarget, float moveSpeed)
    {
        if (Physics.Raycast(footTarget.position, Vector3.down, out RaycastHit hit, 1f)) // レイヒット (接地) 時
        {
            // 足位置補正 (空中 → 地面)
            Vector3 adjustedPos = new Vector3(footTarget.position.x, footTarget.position.y - hit.distance + 0.12f, footTarget.position.z);
            footTarget.position = adjustedPos;

            // IK 強度設定
            animator.SetIKPositionWeight(foot, moveSpeed == 0 ? 1f : 0.02f);    // 停止時 → 位置地面固定 (IK 強度 = 1)
            animator.SetIKRotationWeight(foot, 1f); // 回転方向地面固定 (IK 強度 = 1)

            // IK 位置設定
            animator.SetIKPosition(foot, footTarget.position);

            // IK 回転設定
            animator.SetIKRotation(foot, footTarget.rotation);
        }
    }
}
