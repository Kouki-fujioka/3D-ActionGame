using UnityEngine;
using UnityEngine.AI;

public class CharaNavi : MonoBehaviour
{
    // 目的地となるGameObjectをシリアル化。
    [SerializeField] Transform targetObject;
    NavMeshAgent myAgent;
    Vector3 Nextposition;
    Vector3 Position;
    Vector3 Idouryou;
    Vector3 moveDirection = Vector3.zero;
    Animator anim;

    void Start()
    {
        // Nav Mesh Agentのコンポーネントを取得。
        myAgent = GetComponent<NavMeshAgent>();
        // Attackパラメータの値を取得。
        anim = GetComponent<Animator>();
    }

    void Update()
    {

        int Attack = anim.GetInteger("Attack");


        if (Attack == 0)
        {
            // NavMeshをアクティブ化
            myAgent.isStopped = false;
            // targetのゲームオブジェクトに向かって移動。
            myAgent.SetDestination(targetObject.position);
            // 次の位置を取得(今回はtargetObjectの位置と同じ)
            Nextposition = myAgent.steeringTarget;
            // 今の位置を取得
            Position = this.transform.position;
            // 次の位置から今の位置を引く
            Idouryou = Nextposition - this.transform.position;
            // ベクトルを正規化する
            moveDirection = Idouryou.normalized;
            // 正規化した値をパラメータに代入する。
            anim.SetFloat("Idouryou", moveDirection.magnitude);
        }
        else
        {
            // NavMeshを非アクティブに
            myAgent.isStopped = true;
        }
    }
}
