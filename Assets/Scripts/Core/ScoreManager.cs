using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private int gold = 0;

    public UnityEvent OnGoldChanged = new UnityEvent();

    private void Awake()
    {
        Instance = this;
    }

    public void AddGold(int amount)
    {
        gold += amount;

        OnGoldChanged.Invoke();   // báo cho UI c?p nh?t
    }

    public int GetGold()
    {
        return gold;
    }
}