using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private CharacterStatus attackerStatus;    // 攻撃実行キャラステータス
    [SerializeField] private GameObject attackerObject; // 攻撃実行キャラ
    private int attackPower;
    private IAttack hitCounter;

    private void Awake()
    {
        attackPower = attackerStatus.Attack;
        hitCounter = attackerObject.GetComponent<IAttack>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hitCounter == null || attackPower <= 0)
        {
            return;
        }

        int remainingHits = hitCounter.GetRemainingHitCount();
        if (remainingHits <= 0)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable == null)
        {
            return;
        }

        hitCounter.DecreaseRemainingHitCount();
        damageable.Damage(attackPower);
    }
}
