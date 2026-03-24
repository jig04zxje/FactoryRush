using UnityEngine;

namespace FactoryRush.Scripts.Core
{
    /// <summary>
    /// ScriptableObject defining an achievement.
    /// Can be used for Gem milestones or other stats.
    /// </summary>
    [CreateAssetMenu(fileName = "Achievement_", menuName = "FactoryRush/Achievement")]
    public class AchievementSO : ScriptableObject
    {
        [Header("Identity")]
        public string id;
        public string title;
        [TextArea(3, 5)]
        public string description;
        public Sprite icon;

        [Header("Requirement")]
        public int gemThreshold;

        // Note: For future expansion, you could add an enum like RequirementType { Gems, MachinesBuilt, TotalGold }
    }
}
