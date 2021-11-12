using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PiggerBomber
{
    public sealed class GameUiView : MonoBehaviour
    {
        [field: SerializeField] public Button BombPlantButton { get; private set; }
        [field: SerializeField] public Button BackToMenuButton { get; private set; }
        [field: SerializeField] public TMP_Text ScoreTxt { get; private set; }
        [field: SerializeField] public TMP_Text ApplesNeddToEatCount { get; private set; }

        private void Start() =>
            gameObject.SetActive(false);

        private void OnDestroy()
        {
            BackToMenuButton.onClick.RemoveAllListeners();
            BombPlantButton.onClick.RemoveAllListeners();
        }
    }
}

