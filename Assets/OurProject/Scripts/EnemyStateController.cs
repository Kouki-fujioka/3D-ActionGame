using System.Collections;
using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    [SerializeField] private CharacterStatus characterStatus;
    [SerializeField] private IEnemy enemyBehavior;

    private Animator animator;
    private static class AnimatorParams
    {
        public const string Attack = "Attack";
    }

    private enum EnemyActionState
    {
        Idle = 0,
        Attack = 1
    }
    private EnemyActionState currentState;
    private bool isActionInProgress = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 攻撃中や行動中は処理しない
        if (isActionInProgress || IsAttacking())
        {
            return;
        }

        if (enemyBehavior == null) return;

        // 行動を決定
        currentState = (EnemyActionState)enemyBehavior.EnemyAIkoudou();
        StartCoroutine(PerformAction());
    }

    private bool IsAttacking()
    {
        return animator.GetInteger(AnimatorParams.Attack) > 0;
    }

    /// <summary>
    /// 決定した行動を実行するコルーチン
    /// </summary>
    private IEnumerator PerformAction()
    {
        isActionInProgress = true;

        switch (currentState)
        {
            case EnemyActionState.Idle:
                animator.SetInteger(AnimatorParams.Attack, 0);
                break;

            case EnemyActionState.Attack:
                animator.SetInteger(AnimatorParams.Attack, 1);
                break;

            default:
                animator.SetInteger(AnimatorParams.Attack, 0);
                break;
        }

        // 一定時間待機してから次の行動へ
        yield return new WaitForSeconds(characterStatus.Enemytime);
        isActionInProgress = false;
    }
}
