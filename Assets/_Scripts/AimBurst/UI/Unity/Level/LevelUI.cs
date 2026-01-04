using AimBurst.UI.Contracts.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AimBurst.UI.Unity.Level {
    internal sealed class LevelUI : MonoBehaviour, ILevel {
        [SerializeField] private Image fillImage;
        [SerializeField] private TextMeshProUGUI levelTxt;

        public void SetLevelIndex(int index) => levelTxt.SetText($"Level {index}");
        public void SetProgress(float progressPercentage) => fillImage.fillAmount = progressPercentage;
    }
}