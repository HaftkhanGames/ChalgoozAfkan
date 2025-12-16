using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    [Tooltip("Unique ID for saving system (e.g., 'Level_01')")]
    public string levelId; // Useful for saving progress securely
    public int levelNumber;
    public string levelName;
    [TextArea(3, 5)] public string description; // Optional: description for UI

    [Header("Level Prefab")]
    public GameObject levelPrefab;

    [Header("Settings")]
    [Range(1, 5)] public int difficulty = 1;
    public float timeLimit = 120f; // Seconds

    [Header("Requirements")]
    public int requiredStarsToUnlock = 0;

    [Header("Rewards")]
    public int maxStars = 3;
    public int coinsReward = 100;
    
    // Helper helper to get level name clearly
    public string GetDisplayName()
    {
        return string.IsNullOrEmpty(levelName) ? $"Level {levelNumber}" : levelName;
    }
}