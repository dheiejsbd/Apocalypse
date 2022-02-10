using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HQFPSTemplate.Items
{
    [Serializable]
    public class ItemGenerator
    {
        public enum Method
        {
            Specific,
            RandomFromCategory,
            Random
        }

        [SerializeField]
        public Method GenerateMethod = Method.Specific;

        [DatabaseCategory]
        [SerializeField]
        public string Category = null;

        [DatabaseItem]
        [SerializeField]
        public string Name = null;

        [SerializeField]
        [MinMax(1,100, false)]
        private Vector2Int CountRange = new Vector2Int(1,100);


        public int GetRandomCount()
        {
            return Mathf.Clamp((int)Random.Range(CountRange.x, CountRange.y + 1), 1, 100);
        }
    }

    [RequireComponent(typeof(Inventory))]
    public class StartupInventory : EntityComponent
    {
        [Serializable]
        public class ItemContainerStartupItems
        {
            public string Name;

            [Space]

            [Reorderable]
            public ItemGeneratorList StartupItems = null;
        }

        [SerializeField]
        private ItemContainerStartupItems[] m_ItemContainersStartupItems;


        public override void OnEntityStart() => AddItemsToInventory();

        private void AddItemsToInventory()
        {
            Inventory inventory = GetComponent<Inventory>();

            if (inventory != null)
            {
                foreach (var container in m_ItemContainersStartupItems)
                {
                    ItemContainer itemContainer = inventory.GetContainerWithName(container.Name);

                    if (itemContainer != null)
                    {
                        foreach (var item in container.StartupItems)
                        {
                            if (item.GenerateMethod == ItemGenerator.Method.Specific)
                                itemContainer.AddItem(item.Name, item.GetRandomCount());
                            else if (item.GenerateMethod == ItemGenerator.Method.RandomFromCategory)
                            {
                                ItemInfo itemInfo = ItemDatabase.GetRandomItemFromCategory(item.Category);

                                if (itemInfo != null)
                                    itemContainer.AddItem(itemInfo.Id, item.GetRandomCount());
                            }
                            else if (item.GenerateMethod == ItemGenerator.Method.Random)
                            {
                                var category = ItemDatabase.GetRandomCategory();

                                if (category != null)
                                {
                                    ItemInfo itemInfo = ItemDatabase.GetRandomItemFromCategory(category.Name);

                                    if (itemInfo != null)
                                        itemContainer.AddItem(itemInfo.Id, item.GetRandomCount());
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}