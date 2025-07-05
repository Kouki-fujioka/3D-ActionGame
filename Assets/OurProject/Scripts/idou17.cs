using System.Collections;
using UnityEngine;

public class idou17 : MonoBehaviour
{
    Animator animator;
    Rigidbody rb;
    RaycastHit zimen;

    [SerializeField] float moveZ;
    [SerializeField] float moveY;
    [SerializeField] float moveX;
    [SerializeField, Tooltip("�ʏ�̈ړ����x")] float normalSpeed = 6f;
    [SerializeField, Tooltip("�W�����v���x")] float jumpSpeed = 450f;
    [SerializeField, Tooltip("�_�b�V����")] float dashSpeed = 1500f;
    [SerializeField, Tooltip("�_�b�V������")] float dashtime = 0.3f;
    [SerializeField, Tooltip("�_�b�V���N�[���^�C��")] float dashcool = 1.5f;
    [SerializeField, Tooltip("�_�b�V������")] bool dashkyoka = true;

    [Tooltip("�ړ�����")] Vector3 moveDirection = Vector3.zero;
    [Tooltip("�_�b�V���ړ�����")] Vector3 dashhoukou = Vector3.zero;
    [Tooltip("�J�n�ʒu")] Vector3 starpos;
    [Tooltip("��]")] Vector3 muki;
    [Tooltip("��]�L��")] Vector3 mukikioku;
    [Tooltip("���v�Z")] Vector3 raykeisan;

    [Tooltip("�}�E�X�ړ���x")] float mx;
    [Tooltip("�}�E�X�ړ���y")] float my;
    [SerializeField, Tooltip("x��]���x")] float xkaitensokudo = 10f;
    [SerializeField, Tooltip("y��]���x")] float ykaitensokudo = 10f;

    [SerializeField, Tooltip("����E�p�x")] float upseigen = 290f;
    [SerializeField, Tooltip("�����E�p�x")] float downseigen = 70f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // �}�E�X�J�[�\�����\���ɂ��A�ʒu���Œ�
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        starpos = transform.position;
    }

    void Update()
    {
        // �ړ��ݒ�
        muki = transform.eulerAngles;
        mukikioku = transform.eulerAngles;
        muki.x = 0f;
        muki.z = 0f;
        transform.eulerAngles = muki;

        // �O��ړ�
        moveZ = (Input.GetAxis("Vertical"));
        // ���E�ړ�
        moveX = (Input.GetAxis("Horizontal"));

        moveDirection = new Vector3(moveX, 0, moveZ).normalized * normalSpeed * Time.deltaTime;
        this.transform.Translate(moveDirection.x, moveDirection.y, moveDirection.z);

        // �ړ��A�j���[�V����
        animator.SetFloat("MoveSpeed", moveDirection.magnitude);

        // �_�b�V��
        if (Input.GetButtonDown("Fire3") && dashkyoka == true)
        {
            dashhoukou = new Vector3(moveX, 0, moveZ).normalized;
            rb.AddForce(transform.TransformDirection(dashhoukou) * dashSpeed, ForceMode.Impulse);
            dashkyoka = false;

            StartCoroutine(Dashowari());
        }

        transform.eulerAngles = mukikioku;

        // �n�ʔ���ƃW�����v
        raykeisan = transform.position;
        raykeisan.y += 0.5f;

        if (Physics.SphereCast(raykeisan, 0.3f, Vector3.down, out zimen, 0.6f))
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }
        }

        // �}�E�X����
        mx = Input.GetAxis("Mouse X");
        my = Input.GetAxis("Mouse Y");

        mx += mx * xkaitensokudo * Time.deltaTime;
        my += my * ykaitensokudo * Time.deltaTime;

        transform.Rotate(-my, mx, 0);
        muki = transform.eulerAngles;

        if (muki.x > downseigen)
        {
            if (muki.x > 180f)
            {
                if (upseigen > muki.x)
                {
                    muki.x = upseigen;
                }
            }
            else
            {
                muki.x = downseigen;
            }
        }

        muki.z = 0;
        transform.eulerAngles = muki;
    }

    IEnumerator Dashowari()
    {
        // �_�b�V���I��点��
        yield return new WaitForSeconds(dashtime);
        rb.linearVelocity = Vector3.zero;

        // �_�b�V���N�[���^�C�������ҋ@  
        yield return new WaitForSeconds(dashcool);
        dashkyoka = true;
    }
}
