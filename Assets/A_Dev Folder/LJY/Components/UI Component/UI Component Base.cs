using UnityEngine;
using UnityEngine.UI;

public class UIComponentBase : MonoBehaviour
{
    #region OnPercentChanged
    public delegate void FOnPercentChanged();
    public FOnPercentChanged OnInitialized;
    public FOnPercentChanged OnStatusChanged;
    #endregion

    #region HP Progress Bar
    [Header("Common UI")]
    [SerializeField] protected CharacterBase Owner;

    [SerializeField] protected Slider HP_ProgressBar;
    #endregion

    protected virtual void Awake()
    {

    }

    #region Utility
    protected CharacterSpecData GetOwnerSpec()
    {
        if (!UtilityLibrary.IsValid(Owner))
        {
            Debug.Log("Owner Is Not Valid");
        }

        return Owner.CommonSpec;
    }

    protected CharacterAbilitySpec GetOwnerAblitySpec()
    {
        if (!UtilityLibrary.IsValid(Owner))
        {
            Debug.Log("Owner Is Not Valid");
        }

        return Owner.AbilitySpec;
    }
    #endregion

    protected void SetHPRate()
    {
        if (!UtilityLibrary.IsValid(HP_ProgressBar))
        {
            Debug.Log(Owner.CommonSpec.GetCharacterName + " HP UI is Not Valid");
            return;
        }

        float NewRate = Mathf.Clamp(GetOwnerSpec().GetCurHP / GetOwnerSpec().GetMaxHP, 0, 1);

        HP_ProgressBar.value = NewRate;

        if(Owner.CompareTag("Enemy"))
        {
            Debug.Log(NewRate);
        }
    }

}
