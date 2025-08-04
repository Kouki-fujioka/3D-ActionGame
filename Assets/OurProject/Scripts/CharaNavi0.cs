using UnityEngine;
using UnityEngine.AI;

public class CharaNavi0 : MonoBehaviour
{
    // 目的地となるGameObjectをシリアル化。
    [SerializeField] Transform targetObject;
    NavMeshAgent myAgent;


    void Start()
    {
        // Nav Mesh Agentのコンポーネントを取得。
        myAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        // targetのゲームオブジェクトに向かって移動。
        myAgent.SetDestination(targetObject.position);
    }
}
