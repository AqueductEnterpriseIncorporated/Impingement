using UnityEngine;
using UnityEngine.UI;

namespace Impingement.Attributes
{
    public class StaminaDisplay : MonoBehaviour
    {
        [SerializeField] private StaminaController _staminaController;
        //[SerializeField] private TMP_Text _staminaValueText;
        [SerializeField] private Image _staminaImage;

        private void Update()
        {
            _staminaImage.fillAmount = _staminaController.GetCurrentStaminaPoints() / _staminaController.GetMaximumStaminaPoints();
        }
    }
}