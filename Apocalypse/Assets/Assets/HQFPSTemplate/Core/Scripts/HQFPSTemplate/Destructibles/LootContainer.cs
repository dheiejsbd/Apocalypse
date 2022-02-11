using UnityEngine;
using Random = UnityEngine.Random;
using HQFPSTemplate.Items;

namespace HQFPSTemplate
{
    public class LootContainer : DestructibleObject
    {
		[BHeader("Loot")]

		[SerializeField]
        [Reorderable]
		private ItemGeneratorList m_PossibleLoot = null;

		[SerializeField]
		[Range(0, 10)]
		[Tooltip("How many items will be spawned.")]
		private int m_LootSpawnAmount = 1;

		[SerializeField]
		private Vector3 m_LootSpawnOffset = Vector3.zero;


        protected override void DestroyObject(DamageInfo data)
        {
			SpawnLoot();

			base.DestroyObject(data);
        }

        private void SpawnLoot()
		{
            int lastSelected = -1;

            for (int i = 0; i < m_LootSpawnAmount; i++)
            {
                var loot = m_PossibleLoot.ToArray().Select(ref lastSelected, ItemSelection.Method.RandomExcludeLast);

                GameObject pickup = null;

                if (loot.GenerateMethod == ItemGenerator.Method.Specific)
                    pickup = ItemDatabase.GetItemByName(loot.Name).Pickup;
                else if (loot.GenerateMethod == ItemGenerator.Method.RandomFromCategory)
                    pickup = ItemDatabase.GetRandomItemFromCategory(loot.Category).Pickup;
                else if (loot.GenerateMethod == ItemGenerator.Method.Random)
                {
                    var category = ItemDatabase.GetRandomCategory();

                    if (category != null)
                        pickup = ItemDatabase.GetRandomItemFromCategory(category.Name).Pickup;
                }

                if(pickup != null)
                    Instantiate(pickup, transform.position + transform.TransformVector(m_LootSpawnOffset), Quaternion.identity);
            }
        }
	}
}
