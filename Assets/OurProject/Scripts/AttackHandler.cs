using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private CharacterStatus attackerStatus;    // 攻撃実行キャラステータス
    [SerializeField] private GameObject attackerObject; // 攻撃実行キャラ
    private IAttack hitCounter;
    private int attackPower;

    private void Awake()
    {
        hitCounter = attackerObject.GetComponent<IAttack>();
        attackPower = attackerStatus.Attack;
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

        IDamage hpController = other.GetComponent<IDamage>();

        if (hpController == null)
        {
            return;
        }

        hitCounter.DecreaseRemainingHitCount();
        hpController.Damage(attackPower);
    }
}
