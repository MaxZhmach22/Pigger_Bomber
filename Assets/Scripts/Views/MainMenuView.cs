using UnityEngine;
using UnityEngine.UI;

namespace PiggerBomber
{
    public sealed class MainMenuView : MonoBehaviour
    {
        #region Fields

        [field: SerializeField] public Button StartGameBtn { get; private set; }
        [field: SerializeField] public Button QuitButton { get; private set; }

        #endregion


        #region ClassLifeCycles

        private void Start() =>
            gameObject.SetActive(false);

        private void OnDestroy()
        {
            StartGameBtn.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();
        } 
        #endregion
    }
}
