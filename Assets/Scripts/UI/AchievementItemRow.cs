using TMPro;
using UnityEngine;
using UnityEngine.UI;
using FactoryRush.Scripts.Core;

namespace FactoryRush.Scripts.UI
{
    /// <summary>
    /// Represents a single achievement entry in the Main Menu list.
    /// </summary>
    public class AchievementItemRow : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject lockedOverlay;

        [Header("Visual Settings")]
        [SerializeField] private Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
        [SerializeField] private Color unlockedColor = Color.white;

        public void Setup(AchievementSO ach, bool isUnlocked)
        {
            if (titleText != null) titleText.text = isUnlocked ? ach.title : "???";
            if (descriptionText != null) descriptionText.text = isUnlocked ? ach.description : $"Đạt mốc {ach.gemThreshold} Gem để mở khóa.";
            
            if (iconImage != null)
            {
                iconImage.sprite = ach.icon;
                // Darken icon if locked
                iconImage.color = isUnlocked ? Color.white : new Color(0.2f, 0.2f, 0.2f, 0.8f);
            }

            if (lockedOverlay != null)
            {
                lockedOverlay.SetActive(!isUnlocked);
            }
        }
    }
}
