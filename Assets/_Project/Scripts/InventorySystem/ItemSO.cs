using UnityEngine;

namespace ProjectBorderland.InventorySystem
{
    [CreateAssetMenu(menuName = "Items/Create new Item")]
    /// <summary>
    /// Handles player inventory.
    /// </summary>
    public class ItemSO : ScriptableObject
    {
        //==============================================================================
        // Variables
        //==============================================================================
        public string Name;
        public Sprite sprite;
        public GameObject model;
    }
}