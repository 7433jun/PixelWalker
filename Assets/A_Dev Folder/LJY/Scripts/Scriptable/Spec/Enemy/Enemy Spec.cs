using UnityEngine;
using Project.Enums;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[CreateAssetMenu(fileName = "Character Spec Data", menuName = "Scriptable Object/Enemy Spec", order = int.MinValue)]
public class EnemySpecData : CharacterSpecData
{
    [Header("Enemy Spec")]
    [SerializeField] GameObject[] DropTable_Prefabs;
    // ============================================= //
    [SerializeField] int PotionDropMinNum = 1; // ������ �ϳ��� ���
    [SerializeField] int PotionDropMaxNum = 1; // ������ �ϳ��� ���
    [SerializeField] int GoldDropMinNum = 100;
    [SerializeField] int GoldDropMaxNum = 100;
    // ============================================= //

    public void SpawnDropItem(Vector2 SpawnPoint)
    {
        bool bSuccessSpawn = false;

        for (int i = 0; i < DropTable_Prefabs.Length; i++)
        {
            DropItem ItemInfoToSpawn = DropTable_Prefabs[i].GetComponent<DropItem>();
            GameObject ItemToSpawn = DropItemPool.Instance.GetItemByItemType(ItemInfoToSpawn.ItemType);
            ItemToSpawn.transform.position = SpawnPoint + new Vector2(Random.Range(-1f, 2f), 0);

            switch (ItemInfoToSpawn.ItemType)
            {
                case EItemType.HP_Potion:
                    bSuccessSpawn = UtilityLibrary.RandomBool();
                    //Debug.Log("HP: " + bSuccessSpawn);

                    if (bSuccessSpawn)
                    {
                        ItemInfoToSpawn.DropValue = UtilityLibrary.RandomIntegerValue(PotionDropMinNum, PotionDropMaxNum);
                        ItemToSpawn.SetActive(true);
                        ItemInfoToSpawn.AppearBounce();
                    }
                    break;

                case EItemType.MP_Potion:
                    bSuccessSpawn = UtilityLibrary.RandomBool();
                    //Debug.Log("MP: " + bSuccessSpawn);

                    if (bSuccessSpawn)
                    {
                        ItemInfoToSpawn.DropValue = UtilityLibrary.RandomIntegerValue(PotionDropMinNum, PotionDropMaxNum);
                        ItemToSpawn.SetActive(true);
                        ItemInfoToSpawn.AppearBounce();
                    }
                    break;

                case EItemType.Gold:
                    // ���� ������ ���
                    ItemInfoToSpawn.DropValue = UtilityLibrary.RandomIntegerValue(GoldDropMinNum, GoldDropMaxNum);
                    ItemToSpawn.SetActive(true);
                    ItemInfoToSpawn.AppearBounce();
                    break;
            }
        }
    }
}
