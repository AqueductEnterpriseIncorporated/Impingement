using Impingement.Control;
using UnityEngine;

namespace Impingement.Inventory
{
    /// <summary>
    /// An inventory Item that can be placed in the action bar and "Used".
    /// </summary>
    /// <remarks>
    /// This class should be used as a base. Subclasses must implement the `Use`
    /// method.
    /// </remarks>
    [CreateAssetMenu(menuName = ("Inventory/Action Item"))]
    public class ActionItem : InventoryItem
    {
        [Tooltip("Does an instance of this Item get consumed every time it's used.")]
        [SerializeField] private bool _consumable = false;
        [SerializeField] private AudioClip _audioClipOnUse;

        /// <summary>
        /// Trigger the use of this Item. Override to provide functionality.
        /// </summary>
        /// <param name="user">The character that is using this action.</param>
        public virtual void Use(PlayerController player, AudioSource audioSource)
        {
            Debug.Log("Using action: " + this);
            if(audioSource is null || _audioClipOnUse is null) { return; }
            Debug.Log("Playing SFX");
            audioSource.PlayOneShot(_audioClipOnUse);
        }

        public bool isConsumable()
        {
            return _consumable;
        }
    }
}