using UnityEngine;
using System;

public class PlayerExp : MonoBehaviour
{
    public static PlayerExp Instance;
    public static event Action OnLevelUpEvent;


    [Header("Level")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 50;

    [Header("Scaling")]
    [Tooltip("1.5 = +50% each level")]
    public float expGrowthMultiplier = 1.5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;
        Debug.Log($"Gained {amount} EXP. Current: {currentExp}/{expToNextLevel}");

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * expGrowthMultiplier);
        OnLevelUpEvent?.Invoke();
        Debug.Log($"LEVEL UP! Now Level {level}. Next EXP: {expToNextLevel}");
    }
}
