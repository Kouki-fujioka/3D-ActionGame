using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//IEnemyのインターフェースを宣言しているので、public int EnemyAIkoudou()のメソッドを作る必要あり。
public class Enemykoudou : MonoBehaviour,IEnemy
{
    //シリアル化している。charadataの敵を指定。
    [SerializeField] charadata charadata;
    [SerializeField] GameObject Player;
    Vector3 distance; //Playerとの距離
    int State;



    public int EnemyAIkoudou()
    {


        //キャラ距離計算の処理
        //敵の位置からPlayerの位置を引いた後にMathf.Absで絶対値を出すことで距離がわかる。

        distance = transform.position - Player.transform.position;

        float distanceX = Mathf.Abs(distance.x);
        float distanceZ = Mathf.Abs(distance.z);



        //X座標とZ座標の距離のどちらが大きいか調べ、大きいほうの距離が敵のShortAttackRange以下であればStateを1として返す。攻撃を行う。

        if (distanceX > distanceZ)
        {
            if (charadata.ShortAttackRange >= distanceX)
            {

                State = 1;

                return State;
            }
        }
        else
        {
            if (charadata.ShortAttackRange >= distanceZ)
            {
                State = 1;
                return State;

            }
        }

        //条件を満たさない場合はStateを0として返す。何もしない。
        State = 0;
        return State;

    }
}