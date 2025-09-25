using System.Collections;
using UnityEngine;

public class idou34 : MonoBehaviour
{
    [Header("Components")]
    private Animator animator;
    private Rigidbody rigidbody;

    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 6f;
    [SerializeField] private float attackSpeed = 2f;
    [SerializeField] private float jumpForce = 450f;

    private Vector3 moveDirection = Vector3.zero;
    private bool isGrounded = true;

    [Header("Dash Settings")]
    [SerializeField] private float dashPower = 3f;
    [SerializeField] private float dashDuration = 0.3f;
    [SerializeField] private float dashCooldown = 1.5f;
    private bool canDash = true;

    [Header("Mouse Settings")]
    [SerializeField] private float rotateSpeedX = 10f;
    [SerializeField] private float rotateSpeedY = 10f;
    [SerializeField] private float angleLimitUp = 290f;
    [SerializeField] private float angleLimitDown = 70f;

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
    [SerializeField] private GameObject fireAttackObject;
    [SerializeField] private GameObject fireTrailObject;

    private TrailRenderer trail;
    private TrailRenderer fireTrail;
    private ParticleSystem fireEffect;

    private void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        trail = trailObject.GetComponent<TrailRenderer>();
        trail.emitting = false;

        fireEffect = fireAttackObject.GetComponent<ParticleSystem>();
        fireEffect.Stop();

        fireTrail = fireTrailObject.GetComponent<TrailRenderer>();
        fireTrail.emitting = false;
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

    private void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        float speed = attackState == 0 ? normalSpeed : attackSpeed;
        moveDirection = input.normalized * speed * Time.deltaTime;

        transform.Translate(moveDirection);

        animator.SetFloat("MoveSpeed", moveDirection.magnitude);
        UpdateMoveAnimation(input);
    }

    private void UpdateMoveAnimation(Vector3 input)
    {
        animator.SetBool("Forward", input.z > 0);
        animator.SetBool("Back", input.z < 0);
        animator.SetBool("Right", input.x > 0);
        animator.SetBool("Left", input.x < 0);
    }

    private void HandleAttack()
    {
        if (!canAttack) return;

        // Fire attack
        if (comboAvailable && Input.GetMouseButtonDown(1))
        {
            rigidbody.linearVelocity = Vector3.zero;
            animator.SetFloat("Attack", 10f);
            canAttack = false;
            comboAvailable = false;
            fireEffect.Play();
            return;
        }

        // Combo attack
        if (comboAvailable && Input.GetMouseButtonDown(0) && attackState < 4)
        {
            rigidbody.linearVelocity = Vector3.zero;
            comboAvailable = false;
            attackState += 1f;
            animator.SetFloat("Attack", attackState);
        }

        // First attack
        if (!comboAvailable && attackState == 0 && Input.GetMouseButtonDown(0))
        {
            rigidbody.linearVelocity = Vector3.zero;
            animator.SetFloat("Attack", 1f);
        }
    }

    private void HandleDash()
    {
        if (Input.GetButtonDown("Fire3") && canDash)
        {
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
            rigidbody.AddForce(transform.TransformDirection(direction) * dashPower, ForceMode.Impulse);
            canDash = false;
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        yield return new WaitForSeconds(dashDuration);
        rigidbody.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void HandleJump()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        if (Physics.SphereCast(origin, 0.3f, Vector3.down, out RaycastHit hit, 0.6f))
        {
            isGrounded = true;
            animator.SetBool("Jump", false);

            if (Input.GetButtonDown("Jump"))
            {
                rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        else
        {
            isGrounded = false;
            animator.SetBool("Jump", true);
        }
    }

    private void HandleRotation()
    {
        mouseX = Input.GetAxis("Mouse X") * rotateSpeedX * Time.deltaTime;
        mouseY = Input.GetAxis("Mouse Y") * rotateSpeedY * Time.deltaTime;

        transform.Rotate(-mouseY, mouseX, 0);

        Vector3 rotation = transform.eulerAngles;
        if (rotation.x > angleLimitDown)
        {
            if (rotation.x > 180f && angleLimitUp > rotation.x)
                rotation.x = angleLimitUp;
            else
                rotation.x = angleLimitDown;
        }

        rotation.z = 0f;
        transform.eulerAngles = rotation;
    }

    private IEnumerator ComboEndRoutine()
    {
        yield return new WaitForSeconds(comboEndDelay);
        canAttack = true;
        Debug.Log("Combo ended");
    }

    #region AnimationEvents
    private void AttackStart()
    {
        if (attackState == 10f)
            fireTrail.emitting = true;
        else if (attackState > 0 && attackState < 5)
            trail.emitting = true;
    }

    private void Hit()
    {
        if (attackState == 10f)
            fireTrail.emitting = false;
        else
        {
            comboAvailable = true;
            trail.emitting = false;
        }
    }

    private void AttackEnd()
    {
        comboAvailable = false;
        animator.SetFloat("Attack", 0f);

        if (attackState == 4f || attackState == 10f)
        {
            canAttack = false;
            fireEffect.Stop();
            StartCoroutine(ComboEndRoutine());
        }
    }
    #endregion

    private void OnAnimatorIK()
    {
        if (!isGrounded) return;

        float moveSpeed = animator.GetFloat("MoveSpeed");

        ApplyFootIK(AvatarIKGoal.RightFoot, rightFootTarget, moveSpeed);
        ApplyFootIK(AvatarIKGoal.LeftFoot, leftFootTarget, moveSpeed);
    }

    private void ApplyFootIK(AvatarIKGoal foot, Transform footTarget, float moveSpeed)
    {
        if (Physics.Raycast(footTarget.position, Vector3.down, out RaycastHit hit, 1f))
        {
            Vector3 adjustedPos = new Vector3(footTarget.position.x, footTarget.position.y - hit.distance + 0.12f, footTarget.position.z);
            footTarget.position = adjustedPos;

            animator.SetIKPositionWeight(foot, moveSpeed == 0 ? 1f : 0.02f);
            animator.SetIKRotationWeight(foot, 1f);
            animator.SetIKPosition(foot, footTarget.position);
            animator.SetIKRotation(foot, footTarget.rotation);
        }
    }
}
