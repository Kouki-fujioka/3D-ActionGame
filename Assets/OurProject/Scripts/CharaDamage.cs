using UnityEngine;
using UnityEngine.UI;

public class CharaDamage : MonoBehaviour, IDamageable
{
    //シリアル化している。charadataのMazokusoldierを指定。
    [SerializeField] private charadata charadata;
    [SerializeField] Slider Slider;
    int HP;

    void Start()
    {
        //charadataがnullでないことを確認
        if (charadata != null)
        {
            Slider.value = 1;
            //charadataの最大HPを代入。
            HP = charadata.MAXHP;
        }
    }

    // ダメージ処理のメソッド　valueにはPlayer1のATKの値が入ってる
    public void Damage(int value)
    {

        // charadataがnullでないかをチェック
        if (charadata != null)
        {
            // PlayerのATKからMazokusoldierのDEFを引いた値をHPから引く
            HP -= value - charadata.DEF;
            Slider.value = (float)HP / (float)charadata.MAXHP;
        }


        // HPが0以下ならDeath()メソッドを呼び出す。
        if (HP <= 0)
        {
            Death();
        }
    }

    // 死亡処理のメソッド
    public void Death()
    {
        // ゲームオブジェクトを破壊
        Destroy(gameObject);
    }
}
