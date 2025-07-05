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
    [SerializeField, Tooltip("通常の移動速度")] float normalSpeed = 6f;
    [SerializeField, Tooltip("ジャンプ速度")] float jumpSpeed = 450f;
    [SerializeField, Tooltip("ダッシュ力")] float dashSpeed = 1500f;
    [SerializeField, Tooltip("ダッシュ時間")] float dashtime = 0.3f;
    [SerializeField, Tooltip("ダッシュクールタイム")] float dashcool = 1.5f;
    [SerializeField, Tooltip("ダッシュ許可")] bool dashkyoka = true;

    [Tooltip("移動方向")] Vector3 moveDirection = Vector3.zero;
    [Tooltip("ダッシュ移動方向")] Vector3 dashhoukou = Vector3.zero;
    [Tooltip("開始位置")] Vector3 starpos;
    [Tooltip("回転")] Vector3 muki;
    [Tooltip("回転記憶")] Vector3 mukikioku;
    [Tooltip("光計算")] Vector3 raykeisan;

    [Tooltip("マウス移動量x")] float mx;
    [Tooltip("マウス移動量y")] float my;
    [SerializeField, Tooltip("x回転速度")] float xkaitensokudo = 10f;
    [SerializeField, Tooltip("y回転速度")] float ykaitensokudo = 10f;

    [SerializeField, Tooltip("上限界角度")] float upseigen = 290f;
    [SerializeField, Tooltip("下限界角度")] float downseigen = 70f;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // マウスカーソルを非表示にし、位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        starpos = transform.position;
    }

    void Update()
    {
        // 移動設定
        muki = transform.eulerAngles;
        mukikioku = transform.eulerAngles;
        muki.x = 0f;
        muki.z = 0f;
        transform.eulerAngles = muki;

        // 前後移動
        moveZ = (Input.GetAxis("Vertical"));
        // 左右移動
        moveX = (Input.GetAxis("Horizontal"));

        moveDirection = new Vector3(moveX, 0, moveZ).normalized * normalSpeed * Time.deltaTime;
        this.transform.Translate(moveDirection.x, moveDirection.y, moveDirection.z);

        // 移動アニメーション
        animator.SetFloat("MoveSpeed", moveDirection.magnitude);

        // ダッシュ
        if (Input.GetButtonDown("Fire3") && dashkyoka == true)
        {
            dashhoukou = new Vector3(moveX, 0, moveZ).normalized;
            rb.AddForce(transform.TransformDirection(dashhoukou) * dashSpeed, ForceMode.Impulse);
            dashkyoka = false;

            StartCoroutine(Dashowari());
        }

        transform.eulerAngles = mukikioku;

        // 地面判定とジャンプ
        raykeisan = transform.position;
        raykeisan.y += 0.5f;

        if (Physics.SphereCast(raykeisan, 0.3f, Vector3.down, out zimen, 0.6f))
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }
        }

        // マウス向き
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
        // ダッシュ終わらせる
        yield return new WaitForSeconds(dashtime);
        rb.linearVelocity = Vector3.zero;

        // ダッシュクールタイムだけ待機  
        yield return new WaitForSeconds(dashcool);
        dashkyoka = true;
    }
}
