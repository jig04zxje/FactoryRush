using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI goldText;

    private void Start()
    {
        ScoreManager.Instance.OnGoldChanged.AddListener(UpdateGold);
        UpdateGold();
    }

    void UpdateGold()
    {
        goldText.text = ScoreManager.Instance.GetGold().ToString();
    }
}