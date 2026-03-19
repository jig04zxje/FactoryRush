using UnityEngine;
using UnityEngine.Events;

namespace FactoryRush.Scripts.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private int _totalGold = 200; // Starting gold as per design doc

        public UnityEvent<int> OnGoldChanged = new UnityEvent<int>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Ensure gold persists across scene reloads (Retry).
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Init()
        {
            // Reset gold when a new game starts.
            if (GameStateManager.Instance != null)
            {
                GameStateManager.Instance.OnGameStarted.AddListener(ResetGold);
            }
        }

        public void ResetGold()
        {
            _totalGold = 300; // Starting gold as per design doc
            OnGoldChanged?.Invoke(_totalGold);
        }

        public void AddGold(int amount)
        {
            _totalGold += amount;
            OnGoldChanged?.Invoke(_totalGold);
        }

        public bool SpendGold(int amount)
        {
            if (_totalGold >= amount)
            {
                _totalGold -= amount;
                OnGoldChanged?.Invoke(_totalGold);
                return true;
            }
            return false;
        }

        public int GetGold()
        {
            return _totalGold;
        }
    }
}
