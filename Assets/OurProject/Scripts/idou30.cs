using UnityEngine;
using System.Collections;

public class idou30 : MonoBehaviour
{
    Animator anim;
    Rigidbody zyuuryoku;
    RaycastHit zimen;

    float moveZ;
    float moveY;
    float moveX;
    int idouanimkioku;

    float normalSpeed = 6f; //通常の移動速度
    float AttackSpeed = 2f; //攻撃時の移動速度
    float jumpSpeed = 600f; //ジャンプ速度

    bool Groundtuku = true;  //地面ついてる

    Transform GroundObject;
    Vector3 Groundtarget;   //地面ターゲット
    Vector3 rayiti;
    float raydistance;
    Vector3 rayhoukou = Vector3.down;
    RaycastHit rayzyouhou;

    float dashSpeed = 1500f; //ダッシュ力
    float dashtime = 0.3f; //ダッシュ時間
    float dashcool = 1.5f; //ダッシュクールタイム
    bool dashkyoka = true; //ダッシュ許可

    Vector3 moveDirection = Vector3.zero; //通常移動方向
    Vector3 dashhoukou = Vector3.zero;　//ダッシュ移動方向

    Vector3 startpos;   //開始位置
    Vector3 muki;   //回転
    Vector3 mukikioku; //回転記憶
    Vector3 raykeisan; //光計算
    Vector3 rayhititi; //光当たった位置

    float mx;   //マウス移動量x
    float my;   //マウス移動量y

    float xkaitensokudo = 10f;   //x回転速度
    float ykaitensokudo = 10f;   //y回転速度

    float upseigen = 290f;   //上限界角度
    float downseigen = 70f;   //下限界角度

    float Attack;   //攻撃
    bool Combo = false;   //コンボ許容
    bool Attackdekiru = true;   //攻撃可能
    float ComboEndtime = 1f;   //コンボ終了時のスキの時間

    [SerializeField] Transform FootObjectR;
    [SerializeField] Transform FootObjectL;

    void Start()
    {
        anim = GetComponent<Animator>(); //animatorのコンポーネントを取得
        zyuuryoku = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得

        // マウスカーソルを非表示にし、位置を固定
#if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
#endif

        startpos = transform.position;
    }

    void Update()
    {

        Attack = anim.GetFloat("Attack");


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

        //移動計算

        if (Attack == 0f)
        {
            moveDirection = new Vector3(moveX, 0, moveZ).normalized * normalSpeed * Time.deltaTime;
        }
        else
        {
            moveDirection = new Vector3(moveX, 0, moveZ).normalized * AttackSpeed * Time.deltaTime;
        }

        this.transform.Translate(moveDirection.x, moveDirection.y, moveDirection.z);
        anim.SetFloat("MoveSpeed", moveDirection.magnitude);




        //移動方向によりアニメーションの向き・停止決定
        float zmoveZ = Mathf.Abs(moveDirection.z);
        float zmoveX = Mathf.Abs(moveDirection.x);



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

        //攻撃のアニメーション
        if (Attackdekiru == true)
        {
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
                    zyuuryoku = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得
                    zyuuryoku.linearVelocity = Vector3.zero;
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
                        zyuuryoku = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得
                        zyuuryoku.linearVelocity = Vector3.zero;
                        anim.SetFloat("Attack", 1f);
                    }
                }

            }
        }
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
            Groundtuku = true;
            anim.SetBool("Jump", false);

            if (Input.GetButtonDown("Jump"))
            {
                zyuuryoku.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            }

        }
        else
        {
            Groundtuku = false;
            anim.SetBool("Jump", true);
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

    void OnAnimatorIK()
    {


        if (Groundtuku == true)
        {

            float idouryou = anim.GetFloat("MoveSpeed");


            rayiti = FootObjectR.position;

            Physics.Raycast(rayiti, rayhoukou, out rayzyouhou, 1f);

            raydistance = rayzyouhou.distance;

            float raykeisan1 = rayiti.y - raydistance + 0.12f;

            FootObjectR.position = new Vector3(rayiti.x, raykeisan1, rayiti.z);

            // 右足のIKを有効化する

            if (idouryou == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.02f);
            }

            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

            // 右足のIKのターゲットを設定する
            anim.SetIKPosition(AvatarIKGoal.RightFoot, FootObjectR.position);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, FootObjectR.rotation);

            rayiti = FootObjectL.position;

            Physics.Raycast(rayiti, rayhoukou, out rayzyouhou, 1f);
            raydistance = rayzyouhou.distance;
            raykeisan1 = rayiti.y - raydistance + 0.11f;

            FootObjectL.position = new Vector3(rayiti.x, raykeisan1, rayiti.z);

            // 左足のIKを有効化する

            if (idouryou == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.02f);
            }

            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);

            // 左足のIKのターゲットを設定する
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, FootObjectL.position);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, FootObjectL.rotation);
        }
    }

    IEnumerator Dashowari()
    {
        // ダッシュを終わらせる  

        yield return new WaitForSeconds(dashtime);
        anim = GetComponent<Animator>();


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

        anim.SetFloat("Attack", 0f);
    }

}
