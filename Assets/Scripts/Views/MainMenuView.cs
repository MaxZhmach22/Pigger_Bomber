using UnityEngine;
using UnityEngine.UI;

namespace PiggyBomber
{
    public sealed class MainMenuView : MonoBehaviour
    {
        [field: SerializeField] public Button StartGameBtn { get; private set; }

        private void Start() =>
            gameObject.SetActive(false);
    }
}
