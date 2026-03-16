using UnityEngine;
using TMPro; 

namespace FactoryRush.Scripts.Map
{
    public class SlotTooltipManager : MonoBehaviour
    {
        public static SlotTooltipManager Instance { get; private set; }

        [Header("UI References")]
        public GameObject tooltipPanel;
        public TextMeshProUGUI tooltipText;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            HideTooltip();
        }

        private void Update()
        {
            if (tooltipPanel != null && tooltipPanel.activeSelf)
            {
                Vector2 mousePos = Input.mousePosition;
                tooltipPanel.transform.position = mousePos + new Vector2(20f, -20f);
            }
        }

        public void ShowTooltip(string text)
        {
            if (tooltipText != null && tooltipPanel != null)
            {
                tooltipText.text = text;
                tooltipPanel.SetActive(true);
            }
        }

        public void HideTooltip()
        {
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(false);
            }
        }
    }
}