using UnityEngine;
using UnityEngine.Events;

namespace FactoryRush.Scripts.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private int _totalGold = 200; // Starting gold as per design doc

        private int _totalGems = 0; 

        public UnityEvent<int> OnGoldChanged = new UnityEvent<int>();
        public UnityEvent<int> OnGemsChanged = new UnityEvent<int>();

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
            _totalGold = 60; // Starting gold as per design doc
            OnGoldChanged?.Invoke(_totalGold);
        }

        public void AddGold(int amount)
        {
            _totalGold += amount;
            OnGoldChanged?.Invoke(_totalGold);
        }

        public void AddGem(int amount)
        {
            _totalGems += amount;
            OnGemsChanged?.Invoke(_totalGems);
        }

        public bool SpendGem(int amount)
        {
            if (_totalGems >= amount)
            {
                _totalGems -= amount;
                OnGemsChanged?.Invoke(_totalGems);
                return true;
            }
            return false;
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

        public int GetGems()
        {
            return _totalGems;
        }

        public int GetGold()
        {
            return _totalGold;
        }
    }
}
