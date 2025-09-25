using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CharaAttack : MonoBehaviour
{
    //シリアル化している。charadataを指定。
    [SerializeField] private charadata charadata;
    //剣を持っているゲームオブジェクト(親)を指定。
    [SerializeField] GameObject AttackChara;

    int HHcount;
    int ATK;
    ICharaAttack Hcount;


    void Start()
    {
        if (charadata != null)
        {
            //charadataの最大HPを代入。
            ATK = charadata.ATK;

        }

        //ICharaAttackのインターフェースが定義されたコンポーネント(スクリプト)をHcounに代入。
        Hcount = AttackChara.GetComponent<ICharaAttack>();


    }



    //剣がゲームオブジェクトに侵入した瞬間に呼び出し
    void OnTriggerEnter(Collider other)
    {

        //ICharaAttackのインターフェースが定義されたスクリプトのHitCount()を呼び出し、返り値(残りヒット数)をHHcountに代入する。
        HHcount = Hcount.HitCount();


        //HHcountが0以下ならもう既にヒットしている。return;で処理を終わらせる。
        if (HHcount <= 0)
        {

            return;
        }



        if (other.tag == "Player")
        {

            //コライダーのあるゲームオブジェクトのインターフェースを呼び出す
            IDamageable damageable = other.GetComponent<IDamageable>();



            //damageableにnull値が入っていないかチェック
            if (damageable != null)
            {
                //ダメージが入るのが確定したのでヒット数-1
                Hcount.HitCountDown();


                //damageableのダメージ処理メソッドを呼び出す。引数としてcharadataのATKを指定
                damageable.Damage(ATK);


            }

        }

    }

}