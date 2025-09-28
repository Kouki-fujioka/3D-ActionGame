using System.Collections;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    [SerializeField] private CharacterStatus enemyStatus;
    [SerializeField] private EnemyBehavior enemyBehavior;

    private Animator animator;
    private static class AnimatorParams
    {
        public const string Attack = "Attack";
    }

    private EnemyState currentState;
    private bool isActionInProgress;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isActionInProgress || IsAttacking())
        {
            return;
        }

        if (enemyBehavior == null) return;

        currentState = enemyBehavior.GetAIAction();
        StartCoroutine(PerformAction());
    }
    
    /// <summary>
    /// 判定 (攻撃中)
    /// </summary>
    /// <returns></returns>
    private bool IsAttacking()
    {
        return animator.GetInteger(AnimatorParams.Attack) > 0;
    }

    /// <summary>
    /// 敵 AI 行動実行 (並列処理)
    /// </summary>
    private IEnumerator PerformAction()
    {
        isActionInProgress = true;

        switch (currentState)
        {
            // EnemyState: 同アセンブリ, トップレベル (名前空間未定義) → using 不要
            case EnemyState.Idle:
                animator.SetInteger(AnimatorParams.Attack, 0);  // 攻撃アニメーション停止
                break;

            case EnemyState.Attack:
                animator.SetInteger(AnimatorParams.Attack, 1);  // 攻撃アニメーション開始
                break;

            default:
                animator.SetInteger(AnimatorParams.Attack, 0);  // 攻撃アニメーション停止
                break;
        }

        yield return new WaitForSeconds(enemyStatus.ActionInterval); // インターバル時間待機
        isActionInProgress = false;
    }
}
