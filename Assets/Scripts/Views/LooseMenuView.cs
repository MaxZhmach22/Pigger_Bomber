using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PiggerBomber
{
    public sealed class LooseMenuView : MonoBehaviour
    {
        #region Fields

        [field: SerializeField] public Button TryAgainButton { get; private set; }
        [field: SerializeField] public Button QuitButton { get; private set; }
        [field: SerializeField] public TMP_Text ScoreTxt { get; private set; }

        #endregion


        #region ClassLifeCycles

        private void Start() =>
           gameObject.SetActive(false);

        private void OnDestroy()
        {
            TryAgainButton.onClick.RemoveAllListeners();
            QuitButton.onClick.RemoveAllListeners();
        } 

        #endregion

    }
}


