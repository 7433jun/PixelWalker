using System.Collections.Generic;
using UnityEngine;
using Project.Enums;

public class AchievementBase : MonoBehaviour
{
    bool IsCompleted; // 업적 달성 여부 (표시 ui 변동 트리거)

    [System.Serializable]
    public struct Achievement
    {
        public string AchievementName; // 업적 이름
        public Sprite AchievementIcon; // 업적 아이콘
        public string AchievementDescription; // 업적 설명
        public Sprite[] AchievementRewardIcon;  // 보상 아이콘
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
            Debug.LogWarning("업적 달성 오류! 이미 클리어 취급 되어 있음 (혹은 이미 완료한 업적에 대한 보상 받기가 활성화 된 상태)");
            return;
        }

        PlayerBase Player = UtilityLibrary.GetPlayerCharacterInGame();

        if(!Player)
        {
            Debug.LogError("업적 보상 - 플레이어 캐릭터 데이터 로드 실패");
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
