using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Character Ability Spec", menuName = "Scriptable Object/Charcter Ability Spec", order = int.MinValue)]
public class CharacterAbilitySpec : ScriptableObject
{
    #region Ability Spec
    [Header("Spec: Ability Attribute Rate")]
    // =========================================================== //
    [SerializeField] float[] MeleeComboDamageRate;
    public float GetPrimarySwordComboDamageRate(int Index)
    {
        // 전달되는 인덱스 값이 1씩 높게 들어오고 있음

        //Debug.Log("Give Idx: " + Index + " | Cur Array Length: " + MeleeComboDamageRate.Length);

        if (Index >= 0 && Index < MeleeComboDamageRate.Length)
        {
            return MeleeComboDamageRate[Index];
        }

        else if(Index == -1 && MeleeComboDamageRate.Length != 0)
        {
            return MeleeComboDamageRate[MeleeComboDamageRate.Length - 1];
        }

        else
        {
            Debug.LogError("Invalid index provided!");
            return 0;
        }
    }
    // =========================================================== //
    [SerializeField] float BulletDamageRate;
    [SerializeField] float BulletCost;
    public float GetBulletDamageRate { get { return BulletDamageRate; } }
    public float GetBulletCost { get { return BulletCost; } }
    // =========================================================== //
    #endregion
    [SerializeField] float StoneWallRate;
    public float GetStoneWallRate => StoneWallRate;

    #region Ability CoolTime
    [SerializeField] float DodgeCoolTime = 1f;
    [SerializeField] float DodgeDuration = .2f;
    public float GetDodgeCoolTime { get { return DodgeCoolTime; } }
    public float GetDodgeDuration { get { return DodgeDuration; } }
    #endregion
}
