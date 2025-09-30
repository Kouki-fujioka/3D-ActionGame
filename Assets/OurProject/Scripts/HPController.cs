using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour, IDamage
{
    [SerializeField] private CharacterStatus charaStatus;   // キャラステータス
    [SerializeField] private Slider hpSlider;   // HP バー
    private int currentHp;

    private void Awake()
    {
        currentHp = charaStatus.MaxHP;
        hpSlider.value = 1f;
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damageValue"></param>
    public void Damage(int damageValue)
    {
        if (charaStatus == null)
        {
            return;
        }

        // ダメージ計算
        int actualDamage = Mathf.Max(damageValue - charaStatus.Defense, 1);
        currentHp -= actualDamage;

        // HP バー更新
        if (hpSlider != null)
        {
            hpSlider.value = Mathf.Clamp01((float)currentHp / charaStatus.MaxHP);   // 0.0 ~ 1.0
        }

        if (currentHp <= 0)
        {
            Death();
        }
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public void Death()
    {
        Destroy(gameObject);
    }
}
