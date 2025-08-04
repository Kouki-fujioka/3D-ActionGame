using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemystate1 : MonoBehaviour
{
    Animator anim;
    [SerializeField] charadata charadata;
    [SerializeField] MonoBehaviour Enemykinsetu;

    int State;
    bool koudou;
    IEnemy Enemykoudou;

    void Start()
    {
        anim = GetComponent<Animator>();
        //IEnemyのインターフェースを宣言したコンポーネント(スクリプト)を手に入れ、Enemykoudouへ代入。
        Enemykoudou = GetComponent<IEnemy>();
    }


    void Update()
    {
        //毎フレーム処理を行う必要はない。コルーチンでEnemytimeの時間だけ待機
        StartCoroutine(Enemytime());

        //Attackパラメータの値を代入し0より大きいなら攻撃中なのでreturn;して処理を終了する。
        int Attack = anim.GetInteger("Attack");
        if (Attack > 0)
        {
            return;
        }




        //値が入っていない場合に備えnullチェック。
        if (Enemykoudou != null)
        {
            //スクリプトのEnemyAIkoudou()を呼び出し、返ってきた値をStateに代入する。
            State = Enemykoudou.EnemyAIkoudou();


            koudou = true;
            //Switch文でStateの値に応じて条件分岐。
            switch (State)
            {
                //Stateが0なら停止。
                case 0:

                    anim.SetInteger("Attack", 0);
                    break;

                //Stateが1なら攻撃。
                case 1:

                    anim.SetInteger("Attack", 1);
                    break;
            }
        }

    }

    IEnumerator Enemytime()
    {
        yield return new WaitForSeconds(charadata.Enemytime);
        koudou = false;
    }
}