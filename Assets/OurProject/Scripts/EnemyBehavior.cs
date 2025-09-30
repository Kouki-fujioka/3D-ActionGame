using UnityEngine;

public class EnemyBehavior : MonoBehaviour, IEnemy
{
    [SerializeField] private CharacterStatus enemyStatus;
    [SerializeField] private GameObject player;

    private Vector3 distanceFromPlayer;

    /// <summary>
    /// 敵 AI 行動取得
    /// </summary>
    /// <returns></returns>
    public EnemyState GetAIAction() // EnemyState: 同アセンブリ, トップレベル (名前空間未定義) → using 不要
    {
        distanceFromPlayer = GetDistanceFromPlayer();

        float maxAxisDistance = Mathf.Max(Mathf.Abs(distanceFromPlayer.x), Mathf.Abs(distanceFromPlayer.z));    // X 軸 or Z 軸

        if (maxAxisDistance <= enemyStatus.ShortAttackRange)    // 攻撃範囲内
        {
            return EnemyState.Attack;
        }

        return EnemyState.Idle;
    }

    /// <summary>
    /// 距離取得
    /// </summary>
    private Vector3 GetDistanceFromPlayer()
    {
        return transform.position - player.transform.position;
    }
}
