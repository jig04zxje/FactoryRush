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
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void AddGold(int amount)
        {
            _totalGold += amount;
            OnGoldChanged?.Invoke(_totalGold);
            Debug.Log($"Gold Added: {amount}. Total: {_totalGold}");
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
