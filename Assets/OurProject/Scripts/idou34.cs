using UnityEngine;
using System.Collections;

public class idou34 : MonoBehaviour
{
    Animator anim;
    Rigidbody zyuuryoku;
    RaycastHit zimen;


    [SerializeField] float moveZ;
    [SerializeField] float moveY;
    [SerializeField] float moveX;

    [SerializeField, Tooltip("通常の移動速度")] float normalSpeed = 6f;
    [SerializeField, Tooltip("攻撃時の移動速度")] float AttackSpeed = 2f;

    [SerializeField, Tooltip("ジャンプ速度")] float jumpSpeed = 450f;

    [Tooltip("接地判定")] bool Groundtuku = true;

    Transform GroundObject;
    [Tooltip("地面ターゲット")] Vector3 Groundtarget;
    Vector3 rayiti;
    float raydistance;
    Vector3 rayhoukou = Vector3.down;
    RaycastHit rayzyouhou;

    [SerializeField, Tooltip("ダッシュ力")] float dashSpeed = 3f;
    [SerializeField, Tooltip("ダッシュ時間")] float dashtime = 0.3f;
    [SerializeField, Tooltip("ダッシュクールタイム")] float dashcool = 1.5f;
    [SerializeField, Tooltip("ダッシュ許可")] bool dashkyoka = true;


    [Tooltip("通常移動方向")] Vector3 moveDirection = Vector3.zero;
    [Tooltip("ダッシュ移動方向")] Vector3 dashhoukou = Vector3.zero;



    [Tooltip("開始位置")] Vector3 startpos;
    [Tooltip("回転")] Vector3 muki;
    [Tooltip("回転記憶")] Vector3 mukikioku;
    [Tooltip("光計算")] Vector3 raykeisan;




    [Tooltip("マウス移動量x")] float mx;
    [Tooltip("マウス移動量y")] float my;

    [SerializeField, Tooltip("x回転速度")] float xkaitensokudo = 10f;
    [SerializeField, Tooltip("y回転速度")] float ykaitensokudo = 10f;

    [SerializeField, Tooltip("上限界角度")] float upseigen = 290f;
    [SerializeField, Tooltip("下限界角度")] float downseigen = 70f;

    [SerializeField, Tooltip("攻撃")] float Attack;
    [SerializeField, Tooltip("コンボ許容")] bool Combo = false;
    [SerializeField, Tooltip("攻撃可能")] bool Attackdekiru = true;
    [SerializeField, Tooltip("コンボ終了時のスキの時間")] float ComboEndtime = 1f;

    [SerializeField] Transform FootObjectR;
    [SerializeField] Transform FootObjectL;
    [SerializeField] GameObject TrailObject;
    [SerializeField] GameObject FireAttack1Object;
    [SerializeField] GameObject FireAttack2Object;

    [Tooltip("軌跡")] TrailRenderer Kiseki;
    [Tooltip("軌跡2")] TrailRenderer Kiseki2;
    [Tooltip("ファイアアタック1のパーティクル")] ParticleSystem Fire1;

    void Start()
    {
        anim = GetComponent<Animator>(); //animatorのコンポーネントを取得
        zyuuryoku = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得

        // マウスカーソルを非表示にし、位置を固定
        //#if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //#endif

        Kiseki = TrailObject.GetComponent<TrailRenderer>();
        Kiseki.emitting = false;

        startpos = transform.position;
    }

    void Update()
    {
        anim = GetComponent<Animator>();
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

        float zmoveZ = Mathf.Abs(moveDirection.z);
        float zmoveX = Mathf.Abs(moveDirection.x);


        //移動方向によりアニメーションの向き・停止決定

        anim.SetFloat("MoveSpeed", moveDirection.magnitude);

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
                //通常攻撃1回以上時に右クリックでのファイアアタック処理。
                if (Input.GetMouseButtonDown(1))
                {
                    //攻撃時にダッシュとジャンプによる移動停止。

                    zyuuryoku.linearVelocity = Vector3.zero;

                    anim.SetFloat("Attack", 10f);

                    Attackdekiru = false;
                    Combo = false;
                    Fire1 = FireAttack1Object.GetComponent<ParticleSystem>();
                    Fire1.Play();

                    return;
                }

                //コンボ中の通常攻撃処理。
                if (Input.GetMouseButtonDown(0))
                {
                   if(Attack < 4 )
                    {
                        //攻撃時にダッシュとジャンプによる移動停止。

                        zyuuryoku.linearVelocity = Vector3.zero;
                        Combo = false;
                        Attack += 1f;
                        anim.SetFloat("Attack", Attack);

                    }
                }
            }
            else
            {
                //通常攻撃の最初の1回の処理。
                if (Attack == 0)
                {

                    if (Input.GetMouseButtonDown(0))
                    {
                        //攻撃時にダッシュとジャンプによる移動停止。
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
            // 浮いていない時の処理
            anim = GetComponent<Animator>();
            float idouryou = anim.GetFloat("MoveSpeed");


            rayiti = FootObjectR.position;

            Physics.Raycast(rayiti, rayhoukou, out rayzyouhou, 1f);

            raydistance = rayzyouhou.distance;

            float raykeisan1 = rayiti.y - raydistance + 0.12f;

            FootObjectR.position = new Vector3(rayiti.x, raykeisan1, rayiti.z);


            // 右足のIKを有効化

            if (idouryou == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1.0f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 0.02f);
            }

            anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1.0f);

            // 右足のIKのターゲットを設定
            anim.SetIKPosition(AvatarIKGoal.RightFoot, FootObjectR.position);
            anim.SetIKRotation(AvatarIKGoal.RightFoot, FootObjectR.rotation);

            rayiti = FootObjectL.position;

            Physics.Raycast(rayiti, rayhoukou, out rayzyouhou, 1f);

            raydistance = rayzyouhou.distance;
            raykeisan1 = rayiti.y - raydistance + 0.11f;

            FootObjectL.position = new Vector3(rayiti.x, raykeisan1, rayiti.z);


            // 左足のIKを有効化

            if (idouryou == 0)
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1.0f);
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 0.02f);
            }

            anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1.0f);

            // 左足のIKのターゲットを設定
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, FootObjectL.position);
            anim.SetIKRotation(AvatarIKGoal.LeftFoot, FootObjectL.rotation);
        }
    }

    IEnumerator Dashowari()
    {
        // ダッシュを終わらせる  

        yield return new WaitForSeconds(dashtime);
        anim = GetComponent<Animator>();

        zyuuryoku = GetComponent<Rigidbody>(); //Rigidbodyのコンポーネントを取得
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

    void AttackStart()
    {
        //ファイアアタックか通常攻撃か条件分岐

        Attack = anim.GetFloat("Attack");
        if (Attack == 10f)
        {

            Kiseki2.emitting = true;

        }
        if (Attack > 0 && Attack < 5)
        {


            Kiseki.emitting = true;
        }
    }

    void Hit()
    {
        //攻撃終了
        //ファイアアタックか通常攻撃かで条件分岐

        Attack = anim.GetFloat("Attack");
        if (Attack == 10f)
        {

            Kiseki2.emitting = false;
        }
        else
        {
            Combo = true;



            Kiseki.emitting = false;
        }
    }

    void AttackEnd()
    {
        //攻撃アニメーション終了時の処理
        Combo = false;

        Attack = anim.GetFloat("Attack");
        anim.SetFloat("Attack", 0f);
        //通常攻撃4回目終了
        if (Attack == 4f)
        {
            anim.SetFloat("Attack", 0f);
            Attackdekiru = false;
            StartCoroutine(ComboEnd());
        }
        //ファイアアタック終了
        if (Attack == 10f)
        {
            Fire1 = FireAttack1Object.GetComponent<ParticleSystem>();
            Fire1.Stop();

            anim.SetFloat("Attack", 0f);

            StartCoroutine(ComboEnd());
        }

    }

}