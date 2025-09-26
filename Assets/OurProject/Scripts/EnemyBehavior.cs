using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IEnemy
{
    [SerializeField] private CharacterStatus enemyStatus;
    [SerializeField] private GameObject player;

    private Vector3 distanceFromPlayer;

    private enum EnemyState
    {
        Idle = 0,   // 待機
        Attack = 1  // 近距離攻撃
    }

    /// <summary>
    /// 敵 AI 行動取得
    /// </summary>
    /// <returns></returns>
    public int GetAIAction()
    {
        distanceFromPlayer = GetDistanceFromPlayer();

        float maxAxisDistance = Mathf.Max(Mathf.Abs(distanceFromPlayer.x), Mathf.Abs(distanceFromPlayer.z));    // X 軸 or Z 軸

        if (maxAxisDistance <= enemyStatus.ShortAttackRange)    // 攻撃範囲内
        {
            return (int)EnemyState.Attack;
        }

        return (int)EnemyState.Idle;
    }

    /// <summary>
    /// 距離取得
    /// </summary>
    private Vector3 GetDistanceFromPlayer()
    {
        return transform.position - player.transform.position;
    }
}
