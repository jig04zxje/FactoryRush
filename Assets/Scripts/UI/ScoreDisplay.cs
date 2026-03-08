using FactoryRush.Scripts.Core;
using UnityEngine;
using TMPro;


/// <summary>
/// Displays current total gold on the UI.
/// Listens to OnGoldChanged event from ScoreManager to update the text.
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    /// <summary>
    /// UI Text displaying the score (using TextMeshPro).
    /// </summary>
    [SerializeField] private TextMeshProUGUI totalGoldText;

    /// <summary>
    /// Updates display on Start.
    /// </summary>
    private void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnGoldChanged.AddListener(UpdateDisplay);
            UpdateDisplay(ScoreManager.Instance.GetGold());
        }
        else
        {
            Debug.LogWarning("ScoreDisplay: ScoreManager.Instance is null in Start()!");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe on destroy to prevent memory leaks or null reference errors.
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnGoldChanged.RemoveListener(UpdateDisplay);
        }
    }

    /// <summary>
    /// Updates the UI text with the current gold.
    /// </summary>
    /// <param name="gold">Current gold amount.</param>
    public void UpdateDisplay(int gold)
    {
        if (totalGoldText != null)
            totalGoldText.text = $"Gold: {gold}";
    }

}
