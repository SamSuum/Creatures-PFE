using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Loot Table", menuName =  "Inventory loot table")]
public class LootTable :ScriptableObject
{
    [Serializable]
    public class LootItem
    {
        public GameObject itemPrefab;
        public int minSpawn;
        public int maxSpawn;
        [Range(0f, 100f)] public float spawnChance;
    }



    public List<LootItem> lootItems = new List<LootItem>();
    [Range(0f, 100f)] public float spawnChancePerSlot = 20;

    public void InitiliazeLootTable()
    {
        float totalSpawnChance = 0f;

        foreach (LootItem item in lootItems)
        {
            totalSpawnChance += item.spawnChance;
        }

        if(totalSpawnChance >100)
        {
            NormaliseSpawnChance();
        }
    }

    private void NormaliseSpawnChance()
    {
        float normalisationFactor = 100f / CalculateTotalSpawnChance();

        foreach(LootItem item in lootItems)
        {
            item.spawnChance *= normalisationFactor;
        }
    }

    private float CalculateTotalSpawnChance()
    {
        float totalSpawnChance = 0f;
        foreach(LootItem item in lootItems)
        {
            totalSpawnChance += item.spawnChance;
        }

        return totalSpawnChance;
    }

    public void SpawnLoot(List<Slot> allChestSlots)
    {
        foreach (Slot chestSlot in allChestSlots)
        {
            if(UnityEngine.Random.Range(0f,100f) <= spawnChancePerSlot)
            {
                SpawnRandomItem(chestSlot);
            }
        }
    }

    private void SpawnRandomItem(Slot slot)
    {
        LootItem chosenitem = ChooseItem();
        if(chosenitem != null)
        {
            int spawnCount = UnityEngine.Random.Range(chosenitem.minSpawn, (chosenitem.maxSpawn+1));

            GameObject spawnedItem = Instantiate(chosenitem.itemPrefab, Vector3.zero, Quaternion.identity);
            spawnedItem.SetActive(false);

            Item itemComponent = spawnedItem.GetComponent<Item>();

            if(itemComponent !=null)
            {
                itemComponent.Amount = spawnCount;
            }

            slot.SetItem(itemComponent);
            slot.UpdateData();
        }
    }

    private LootItem ChooseItem()
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float cumulativeChance = 0f;

        foreach (LootItem item in lootItems)
        {
            cumulativeChance += item.spawnChance;
            if(randomValue <= cumulativeChance)
            {
                return item;
            }
        }
        return null;

    }
}
