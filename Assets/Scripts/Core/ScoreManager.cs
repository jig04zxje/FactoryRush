using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Core
{
    [System.Serializable]
    public class IntUnityEvent : UnityEvent<int> { }

    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        [SerializeField] private int totalGold;
        public int TotalGold => totalGold;

        public IntUnityEvent OnGoldChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void AddGold(int amount)
        {
            if (amount <= 0) return;
            totalGold += amount;
            OnGoldChanged?.Invoke(totalGold);
        }

        public int GetGold() => totalGold;

        public void ResetGold()
        {
            totalGold = 0;
            OnGoldChanged?.Invoke(totalGold);
        }
    }
}
