using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Collections;
using UniGLTF;
using UnityEngine.UIElements;

public class CharaNavi2 : MonoBehaviour
{
    // 目的地となるGameObjectをシリアル化。
    [SerializeField] Transform targetPlayer;
    [SerializeField] charadata charadata;
    NavMeshAgent myAgent;

    Vector3 PlayerPosition;
    Vector3 ThisPosition;
    Vector3 OldPosition;
    Vector3 NextPosition;

    Vector3 Idouryou;
    Vector3 moveDirection = Vector3.zero;
    Animator anim;

    float Enemykankaku;
    float Enemyhaikaikaishi;
    float Enemyhaikaimin;
    float Enemyhaikaimax;

    int PlusMinus;

    int koudou;

    void Start()
    {
        // Nav Mesh Agentのコンポーネントを取得。
        myAgent = GetComponent<NavMeshAgent>();


        anim = GetComponent<Animator>();

        Enemykankaku = charadata.Haikaikankaku;
        Enemyhaikaikaishi = charadata.Haikaikyori;
        Enemyhaikaimin = charadata.Haikaimin;
        Enemyhaikaimax = charadata.Haikaimax;
    }

    void Update()
    {


        // Attackパラメータの値を取得。
        int Attack = anim.GetInteger("Attack");

        if (Attack == 0)

        {
            // Attackパラメータが0ならNavMeshをアクティブ。
            myAgent.isStopped = false;


            PlayerPosition = targetPlayer.position;
            ThisPosition = this.transform.position;


            // 前の位置と今の位置の差を求める(ここもDistanceを使ってもいい)。
            Vector3 idouhandan = OldPosition - ThisPosition;

            // ベクトルを正規化する
            moveDirection = idouhandan.normalized;
            // 正規化した値をパラメータに代入する。
            anim.SetFloat("Idouryou", moveDirection.magnitude);

        }
        else
        {
            // Attackパラメータが0でないならNavMeshを非アクティブにして移動を止める。
            myAgent.isStopped = true;
            return;
        }

        // 今の位置を記憶しておき、次の処理で前の位置として使う。
        OldPosition = this.transform.position;


        // Playerと自分の間の距離を測定
        float kyori = Vector3.Distance(PlayerPosition, ThisPosition);
        // 間の距離が徘徊開始距離より離れているならNavMeshを非アクティブに
        if (kyori > Enemyhaikaikaishi)

        {
            myAgent.isStopped = true;
            return;
        }


        // koudou=1の時のみコルーチンしてリターン。koudou=2の間はリターンし続ける
        if (koudou == 2)
        {
            return;

        }
        StartCoroutine(Enemytime());
        if (koudou == 1)
        {
            koudou = 2;
            return;
        }


        // 今の自分の位置に加算する乱数を決める。
        float RandamX = Random.Range(Enemyhaikaimin, Enemyhaikaimax);
        float RandamZ = Random.Range(Enemyhaikaimin, Enemyhaikaimax);

        //XとZそれぞれに2分の1の確率で-1を掛け算する。
        PlusMinus = Random.Range(1, 3);
        if (PlusMinus == 2)
        {
            RandamX = RandamX * (-1);
        }
        PlusMinus = Random.Range(1, 3);
        if (PlusMinus == 2)
        {
            RandamZ = RandamZ * (-1);
        }

        // 今の位置に乱数を加算し次の位置を決める。
        NextPosition.x = ThisPosition.x + RandamX;
        NextPosition.z = ThisPosition.z + RandamZ;

        //次の位置に向かって移動する
        myAgent.SetDestination(NextPosition);
        //1回徘徊処理が終了したのでkoudouに1を代入。
        koudou = 1;
    }

    IEnumerator Enemytime()
    {
        //Enemykankakuの時間だけ待機。
        yield return new WaitForSeconds(Enemykankaku);
        //待機後にkoudouを0にすることで再び徘徊処理が行えるようになる。
        koudou = 0;
    }


}
