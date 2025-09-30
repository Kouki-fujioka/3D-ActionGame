using UnityEngine;

[CreateAssetMenu(menuName = "GameData/Character Status Data", fileName = "NewCharacterStatus")]
public class CharacterStatus : ScriptableObject
{
    [Header("基本情報")]
    public string CharacterName;
    public int Level;

    [Header("ステータス")]
    public int MaxHP;
    public int MaxMP;
    public int Attack;
    public int Defense;
    public int MagicPower;
    public int MagicResistance;
    public int MoveSpeed;

    [Header("ドロップ情報")]
    public int ExperienceReward;    // 取得経験値
    public int GoldReward;  // 取得ゴールド

    [Header("戦闘設定")]
    public int ShortAttackRange;    // 近接攻撃可能距離
    public float ActionInterval;    // 行動間隔

    [Header("徘徊行動設定")]
    public float DetectPlayerRange; // 徘徊開始距離
    public float WanderDistanceMin; // 徘徊最小距離
    public float WanderDistanceMax; // 徘徊最大距離
    public float WanderInterval;    // 目的地変更間隔
    public float WanderMoveSpeed;   // 移動速度 (徘徊時)
}
