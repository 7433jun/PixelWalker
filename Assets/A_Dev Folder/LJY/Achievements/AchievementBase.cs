using System.Collections.Generic;
using UnityEngine;
using Project.Enums;

public class AchievementBase : MonoBehaviour
{
    bool IsCompleted; // ���� �޼� ���� (ǥ�� ui ���� Ʈ����)

    [System.Serializable]
    public struct Achievement
    {
        public string AchievementName; // ���� �̸�
        public Sprite AchievementIcon; // ���� ������
        public string AchievementDescription; // ���� ����
        public Sprite[] AchievementRewardIcon;  // ���� ������
    }

    public Achievement Achievements = new Achievement();

    [System.Serializable]
    public struct Reward
    {
        public ERewardType RewardType;
        public GameObject RewardItem;
        public int RewardNum;
    }

    [Header("Reward")]
    public Reward[] RewordItems;

    public void AchievementComplete()
    {
        if(IsCompleted)
        {
            Debug.LogWarning("���� �޼� ����! �̹� Ŭ���� ��� �Ǿ� ���� (Ȥ�� �̹� �Ϸ��� ������ ���� ���� �ޱⰡ Ȱ��ȭ �� ����)");
            return;
        }

        PlayerBase Player = UtilityLibrary.GetPlayerCharacterInGame();

        if(!Player)
        {
            Debug.LogError("���� ���� - �÷��̾� ĳ���� ������ �ε� ����");
            return;
        }
        IsCompleted = true;

        for(int i = 0; i < RewordItems.Length; i++)
        {
            ERewardType RewardItemType = ERewardType.HP_Potion;

            switch (RewardItemType)
            {
                case ERewardType.HP_Potion:
                    Player.PlayerSpec.AddPotion(RewordItems[i].RewardNum, EHealType.HPType);
                    break;

                case ERewardType.MP_Potion:
                    Player.PlayerSpec.AddPotion(RewordItems[i].RewardNum, EHealType.MPType);
                    break;

                case ERewardType.Gold:
                    Player.PlayerSpec.AddGold(RewordItems[i].RewardNum);
                    break;

                case ERewardType.Equipment:

                    break;
            }
        }
    }
}
