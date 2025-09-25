using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private charadata charadata;

    void OnTriggerEnter(Collider other)
    {
        // "Player" タグが付いているならスキップ
        if (other.CompareTag("Player"))
        {
            return;
        }

        // other のゲームオブジェクトのインターフェースを呼び出す
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            // ダメージ処理を実行
            damageable.Damage(charadata.ATK);
        }
    }
}