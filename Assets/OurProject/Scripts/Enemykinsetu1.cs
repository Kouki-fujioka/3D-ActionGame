using RPGCharacterAnims.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ICharaAttackのインターフェースを宣言しているので、public int HitCount();とpublic void HitCountdown();のメソッドを作る必要あり。
public class Enemykinsetu1 : MonoBehaviour, ICharaAttack
{
    Animator anim;
    TrailRenderer Kiseki;   //軌跡
    [SerializeField] GameObject TrailObject;
    int Hcount;   //攻撃ヒット回数
    bool Attacktime;   //攻撃中

    public int HitCount()
    {

        //現在の残りヒット数を返す。
        return Hcount;
    }

    public void HitCountDown()
    {
        //ダメージが入ることが確定した際に残りヒット数を減らす。
        --Hcount;

    }
    public bool Attacktimekanshi()
    {
        //攻撃中か攻撃中でないかを返す。
        return Attacktime;

    }





    void Start()
    {
        //軌跡のTrailを最初は無効化しておく。
        Kiseki = TrailObject.GetComponent<TrailRenderer>();
        Kiseki.emitting = false;
        anim = GetComponent<Animator>();
    }


    void AttackStart()
    {

        //剣振り開始。軌跡のTrailを有効化

        Kiseki.emitting = true;
        Hcount = 1;
        Attacktime = true;
    }

    void Hit()
    {
        //剣振り終了。軌跡のTrailを無効化

        Kiseki.emitting = false;
        Hcount = 0;
        Attacktime = false;
    }

    void AttackEnd()
    {
        //アニメーション終了 Attackパラメータを0にする。
        anim.SetInteger("Attack", 0);

    }
    

}