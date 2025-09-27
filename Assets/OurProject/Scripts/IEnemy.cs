public enum EnemyState
{
    Idle = 0,   // 待機
    Attack = 1  // 近距離攻撃
}

public interface IEnemy
{
    EnemyState GetAIAction();
}
