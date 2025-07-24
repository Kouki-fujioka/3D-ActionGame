using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

using static UnityEditor.PlayerSettings;

public class idou21 : MonoBehaviour
{
    Animator anim;
    Rigidbody zyuuryoku;
    RaycastHit zimen;


    float moveZ;
    float moveY;
    float moveX;

    float normalSpeed = 6f; //通常の移動速度

    float jumpSpeed = 450f; //ジャンプ速度
    float dashSpeed = 3f; //ダッシュ力
    float dashtime = 0.3f; //ダッシュ時間
    float dashcool = 1.5f; //ダッシュクールタイム
    bool dashkyoka = true; //ダッシュ許可


    Vector3 moveDirection = Vector3.zero; //通常移動方向
    Vector3 dashhoukou = Vector3.zero;　//ダッシュ移動方向



    Vector3 startpos;   //開始位置
    Vector3 muki;   //回転
    Vector3 mukikioku; //回転記憶
    Vector3 raykeisan; //光計算




    float mx;   //マウス移動量x
    float my;   //マウス移動量y

    float xkaitensokudo = 10f;   //x回転速度
    float ykaitensokudo = 10f;   //y回転速度

    float upseigen = 290f;   //上限界角度
    float downseigen = 70f;   //下限界角度

    float Attack;   //攻撃
    bool Combo= false;   //コンボ許容
    bool Attackdekiru = true;   //攻撃可能
    float ComboEndtime = 1f;   //コンボ終了時のスキの時間

    void Start()
    {
        anim = GetComponent<Animator>(); //animatorのコンポーネントを取得
        zyuuryoku = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得


        // マウスカーソルを非表示にし、位置を固定
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        startpos = transform.position;
    }


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


        //攻撃のアニメーション
        if (Attackdekiru == true)
        {

            Attack = anim.GetFloat("Attack");


            if (Combo == true)
            {
                if (Attack == 4)
                {
                    anim.SetFloat("Attack", 0f);
                    Attackdekiru = false;
                    StartCoroutine(ComboEnd());
                }

                if (Input.GetMouseButtonDown(0))
                {
                    Combo = false;
                    Attack += 1f;
                    anim.SetFloat("Attack", Attack);
                }

            }
            else
            {

                if (Attack == 0)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        anim.SetFloat("Attack", 1f);
                    }
                }
            }
        }

            // 移動のアニメーション
            anim.SetFloat("MoveSpeed", moveDirection.magnitude);
        
        // ダッシュ

        if (Input.GetButtonDown("Fire3") && dashkyoka == true)
        {

            dashhoukou = new Vector3(moveX, 0, moveZ).normalized;
            zyuuryoku.AddForce(transform.TransformDirection(dashhoukou) * dashSpeed, ForceMode.Impulse);
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
                zyuuryoku.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
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

        muki.z = 0f;

        transform.eulerAngles = muki;


    }

    IEnumerator Dashowari()
    {
        // ダッシュを終わらせる  
        yield return new WaitForSeconds(dashtime);

        zyuuryoku.linearVelocity = Vector3.zero;


        // ダッシュクールタイムだけ待機  
        yield return new WaitForSeconds(dashcool);
        dashkyoka = true;

    }

    IEnumerator ComboEnd()
    {
        // コンボ終了時待機  
        yield return new WaitForSeconds(ComboEndtime);
        Attackdekiru = true;

    }


    void FootR()
    {
        //本当はここに足音入れる
    }


    void FootL()
    {
        //本当はここに足音入れる
    }



    void Hit()
    {
        //攻撃終了
        Combo = true;

    }

    void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        Combo = false;
        anim = GetComponent<Animator>();
        anim.SetFloat("Attack", 0f);

    }

}