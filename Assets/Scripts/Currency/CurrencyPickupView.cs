using Photon.Pun;
using UnityEngine;

namespace Impingement.Currency
{
    public class CurrencyPickupView : MonoBehaviour
    {
        [SerializeField] private GameObject _objectToDestroy;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private GameObject _spawnVFX;
        [SerializeField] private int _minAmountOfCurrency;
        [SerializeField] private int _maxAmountOfCurrency;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<PlayerCurrencyController>(out var currencyController))
            {
                ManageConsume(currencyController);
            }
        }

        private void ManageConsume(PlayerCurrencyController currencyController)
        {
            currencyController.MyCurrency += Random.Range(_minAmountOfCurrency, _maxAmountOfCurrency);
            PhotonNetwork.Instantiate("VFX/" + _spawnVFX.name, transform.position, transform.rotation);
            //_audioSource.Play();
            PhotonNetwork.Destroy(_objectToDestroy);
        }
    }
}