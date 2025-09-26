using UnityEngine;

public class EnemySwordAttack : MonoBehaviour, IAttack
{
    [SerializeField] private GameObject trailObject;    // 軌跡オブジェクト

    private Animator animator;
    private TrailRenderer trailRenderer;

    private int remainingHitCount;
    private bool isAttacking;

    #region IAttack Implementation

    /// <summary>
    /// 残存ヒット数取得
    /// </summary>
    /// <returns></returns>
    public int GetRemainingHitCount()
    {
        return remainingHitCount;
    }

    /// <summary>
    /// 残存ヒット数デクリメント
    /// </summary>
    public void DecreaseRemainingHitCount()
    {
        if (remainingHitCount > 0)
            remainingHitCount--;
    }

    /// <summary>
    /// 判定 (攻撃中)
    /// </summary>
    /// <returns></returns>
    public bool IsAttacking()
    {
        return isAttacking;
    }

    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (trailObject != null)
        {
            trailRenderer = trailObject.GetComponent<TrailRenderer>();
            trailRenderer.emitting = false; // トレイル描画停止
        }
    }

    /// <summary>
    /// 攻撃開始時処理
    /// </summary>
    public void StartAttack()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = true;  // トレイル描画再生
        }

        remainingHitCount = 1;  // 単発攻撃
        isAttacking = true;

        animator.SetInteger("Attack", 1);   // 攻撃アニメーション開始
    }

    /// <summary>
    /// 攻撃ヒット時処理
    /// </summary>
    public void OnHit()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false; // トレイル描画停止
        }

        remainingHitCount = 0;
        isAttacking = false;
    }

    /// <summary>
    /// 攻撃終了時処理
    /// </summary>
    public void EndAttack()
    {
        animator.SetInteger("Attack", 0);   // 攻撃アニメーション終了
        isAttacking = false;
    }
}
