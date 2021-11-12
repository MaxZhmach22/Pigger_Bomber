using UnityEngine;
using Zenject;

namespace PiggerBomber
{
    internal sealed class Apple : MonoBehaviour
    {
        [field: SerializeField] public int Score { get; private set; }

        private IAppleController _appleController;

        [Inject]
        private void Init(IAppleController appleController)
        {
            _appleController = appleController;
        }

        private void Start()
        {
            
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                _appleController.EatApple(this);
                gameObject.SetActive(false);
            }
        }
      
    }
}