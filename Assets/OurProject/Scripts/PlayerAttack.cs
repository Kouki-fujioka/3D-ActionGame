using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private CharacterStatus playerStatus;

    private const string PlayerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            return;
        }

        ApplyDamageIfPossible(other);
    }

    /// <summary>
    /// 判定 (プレイヤ)
    /// </summary>
    /// <param name="collider"></param>
    /// <returns></returns>
    private bool IsPlayer(Collider collider)
    {
        return collider.CompareTag(PlayerTag);
    }

    /// <summary>
    /// ダメージ適用
    /// </summary>
    /// <param name="collider"></param>
    private void ApplyDamageIfPossible(Collider collider)
    {
        IDamage damageable = collider.GetComponent<IDamage>();

        if (damageable != null)
        {
            damageable.Damage(playerStatus.Attack);
        }
    }
}
