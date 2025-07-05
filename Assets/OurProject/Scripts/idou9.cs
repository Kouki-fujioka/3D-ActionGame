using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

using static UnityEditor.PlayerSettings;

public class idou9 : MonoBehaviour
{
    Animator anim;

    float moveZ;
    float moveX;

    [Tooltip("通常の移動速度")] float normalSpeed = 6f;

    [Tooltip("移動方向")] Vector3 moveDirection = Vector3.zero;
    [Tooltip("開始位置")] Vector3 startpos;
    [Tooltip("回転")] Vector3 muki;
    [Tooltip("回転記憶")] Vector3 mukikioku;

    [Tooltip("マウス移動量x")] float mx;
    [Tooltip("マウス移動量y")] float my;

    [Tooltip("x回転速度")] float xkaitensokudo = 10f;
    [Tooltip("y回転速度")] float ykaitensokudo = 10f;


    [Tooltip("上限界角度")] float upseigen = 290f;
    [Tooltip("下限界角度")] float downseigen = 70f;



    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>(); //animatorのコンポーネントを取得

        // マウスカーソルを非表示にし、位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        startpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        // 移動設定

        muki = transform.eulerAngles;
        mukikioku = transform.eulerAngles;
        muki.x = 0f;
        muki.z = 0f;

        transform.eulerAngles = muki;

        //前後移動
        moveZ = (Input.GetAxis("Vertical"));
        //左右移動
        moveX = (Input.GetAxis("Horizontal"));

        moveDirection = new Vector3(moveX, 0, moveZ).normalized * normalSpeed * Time.deltaTime;

        this.transform.Translate(moveDirection.x, moveDirection.y, moveDirection.z);

        // 移動のアニメーション
        anim.SetFloat("MoveSpeed", moveDirection.magnitude);

        transform.eulerAngles = mukikioku;

                float zmoveZ = Mathf.Abs(moveDirection.z);
        float zmoveX = Mathf.Abs(moveDirection.x);



//移動方向によりアニメーションの向き・停止決定

        if (zmoveZ > 0f || zmoveX > 0f)
        {
            if (zmoveZ > zmoveX)
            {

                if (moveDirection.z >= 0f)
                {
                    anim.SetBool("Forward", true);
                    anim.SetBool("Back", false);
                    anim.SetBool("Right", false);
                    anim.SetBool("Left", false);
                }
                else
                {
                    anim.SetBool("Forward", false);
                    anim.SetBool("Back", true);
                    anim.SetBool("Right", false);
                    anim.SetBool("Left", false);
                }
            }
            else
            {
                if (moveDirection.x >= 0)
                {
                    anim.SetBool("Forward", false);
                    anim.SetBool("Back", false);
                    anim.SetBool("Right", true);
                    anim.SetBool("Left", false);
                }
                else
                {
                    anim.SetBool("Forward", false);
                    anim.SetBool("Back", false);
                    anim.SetBool("Right", false);
                    anim.SetBool("Left", true);
                }
            }
        }
        else
        {
            anim.SetBool("Forward", false);
            anim.SetBool("Back", false);
            anim.SetBool("Right", false);
            anim.SetBool("Left", false);
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

        muki.z = 0f;

        transform.eulerAngles = muki;

    }
}