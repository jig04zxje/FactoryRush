using UnityEngine;
using FactoryRush.Scripts.Inventory;
using FactoryRush.Scripts.Production;
using FactoryRush.Scripts.Timer;

namespace FactoryRush.Scripts.Core
{
    /// <summary>
    /// Serves as the master conductor for scene initialization (Bootstrap Pattern).
    /// Enforces a strict, predictable order of manager initializations before
    /// telling the GameStateManager to start the game.
    /// </summary>
    public class LevelBootstrapper : MonoBehaviour
    {
        private void Start()
        {
            // 1. Initialize independent data systems
            if (InventoryManager.Instance != null) InventoryManager.Instance.Init();
            if (ScoreManager.Instance != null) ScoreManager.Instance.Init();

            // 2. Initialize dependent/reactive systems
            if (ProductionManager.Instance != null) ProductionManager.Instance.Init();
            if (TimerManager.Instance != null) TimerManager.Instance.Init();

            // 3. All logic systems ready. Trigger the game start sequence.
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.StartGame();
            }
        }
    }
}
