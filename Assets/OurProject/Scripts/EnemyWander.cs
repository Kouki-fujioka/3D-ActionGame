using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyWander : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Character Status")]
    [SerializeField] private CharacterStatus characterStatus;

    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 previousPosition;
    private Vector3 nextWanderPosition;

    private float wanderInterval;
    private float wanderStartDistance;
    private float wanderDistanceMin;
    private float wanderDistanceMax;
    private enum WanderState
    {
        Ready,      // 待機中
        Started,    // 徘徊中
        Cooldown    // クールダウン中
    }
    private WanderState wanderState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        previousPosition = transform.position;
        wanderStartDistance = characterStatus.DetectPlayerRange;
        wanderDistanceMin = characterStatus.WanderDistanceMin;
        wanderDistanceMax = characterStatus.WanderDistanceMax;
        wanderInterval = characterStatus.WanderInterval;
    }

    private void Update()
    {
        if (IsAttacking() || !IsPlayerWithinWanderDistance())
        {
            StopAgent();
            return;
        }

        UpdateAnimatorMovement();

        switch (wanderState)
        {
            case WanderState.Ready:
                MoveToRandomWanderPosition();
                wanderState = WanderState.Started;
                break;

            case WanderState.Started:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)    // 目的地到着時
                {
                    wanderState = WanderState.Cooldown;
                    StartCoroutine(WanderCooldown());
                }
                break;

            case WanderState.Cooldown:
                break;
        }
    }

    /// <summary>
    /// 判定 (攻撃中)
    /// </summary>
    /// <returns></returns>
    private bool IsAttacking()
    {
        return animator.GetInteger("Attack") != 0;
    }

    /// <summary>
    /// 判定 (徘徊開始)
    /// </summary>
    /// <returns></returns>
    private bool IsPlayerWithinWanderDistance()
    {
        float distanceToPlayer = Vector3.Distance(target.position, transform.position); // 距離
        return distanceToPlayer <= wanderStartDistance;
    }

    /// <summary>
    /// 移動停止
    /// </summary>
    private void StopAgent()
    {
        agent.isStopped = true;
    }

    /// <summary>
    /// 移動量アニメーション反映, 移動再開
    /// </summary>
    private void UpdateAnimatorMovement()
    {
        Vector3 movement = transform.position - previousPosition;   // 移動量
        animator.SetFloat("MovementMagnitude", movement.magnitude);
        previousPosition = transform.position;
        agent.isStopped = false;
    }

    /// <summary>
    /// 目的地設定
    /// </summary>
    private void MoveToRandomWanderPosition()
    {
        Vector3 randomOffset = GetRandomWanderOffset();
        nextWanderPosition = transform.position + randomOffset;
        agent.SetDestination(nextWanderPosition);
    }

    /// <summary>
    /// オフセット (ランダム徘徊用) 取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetRandomWanderOffset()
    {
        float xOffset = Random.Range(wanderDistanceMin, wanderDistanceMax) * (Random.value > 0.5f ? 1 : -1);
        float zOffset = Random.Range(wanderDistanceMin, wanderDistanceMax) * (Random.value > 0.5f ? 1 : -1);
        return new Vector3(xOffset, 0, zOffset);
    }

    /// <summary>
    /// 目的地変更間隔管理 (並列処理)
    /// </summary>
    /// <returns></returns>
    private IEnumerator WanderCooldown()
    {
        yield return new WaitForSeconds(wanderInterval);
        wanderState = WanderState.Ready;
    }
}
