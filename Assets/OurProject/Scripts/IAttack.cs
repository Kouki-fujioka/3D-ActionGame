public interface IAttack
{
    int GetRemainingHitCount();
    void DecreaseRemainingHitCount();
    bool IsAttacking();
}
