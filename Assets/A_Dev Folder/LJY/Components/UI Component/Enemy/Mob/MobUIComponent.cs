using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MobUIComponent : UIComponentBase
{
    public delegate void FOnMobDead();
    public FOnMobDead OnMobDead;

    public Canvas EnemyUISet;
    public Slider SpawnedHPBar { get; private set; }

    bool OnVisibility = false;
    float CanVisibleTime = 3f;
    float VisibleTime = 0;

    protected override void Awake()
    {
        base.Awake();
        OnInitialized += SetMobHPRate;

        OnStatusChanged += SetMobHPRate;
        OnStatusChanged += SetVisibility;

        OnMobDead += DeleteWidget;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if(OnVisibility)
        {
            if(VisibleTime < CanVisibleTime)
            {
                VisibleTime += Time.deltaTime;
            }
            else
            {
                VisibleTime = 0;
                SetHidden();
            }
        }
    }

    // OnHit 변수를 통해 비지빌리티 온 오프를 설정할 예정 (에너미 베이스에서)

    public void CreateWidget_Slider()
    {
        if(!UtilityLibrary.IsValid(HP_ProgressBar))
        {
            Debug.Log("Enemy Mob HP Bar Is Not Valid!");

            return;
        }

        GameObject FoundCanvas = GameObject.Find("Floating Canvas");

        if (FoundCanvas)
        {
            EnemyUISet = FoundCanvas.GetComponent<Canvas>();
        }
        else
        {
            Debug.Log("Not Found << Floating Canvas >>");
            return;
        }

        MobHPProgressBar UIData;
        SpawnedHPBar = Instantiate(HP_ProgressBar, EnemyUISet.transform);
        UIData = SpawnedHPBar.GetComponent<MobHPProgressBar>();
        UIData.CanvasPanel = EnemyUISet;
        UIData.Owner = Owner;
    }

    void SetMobHPRate()
    {
        SpawnedHPBar.value = GetOwnerSpec().GetCurHP / GetOwnerSpec().GetMaxHP;
    }

    void SetVisibility()
    {
        VisibleTime = 0;
        SpawnedHPBar.gameObject.SetActive(true);
        OnVisibility = true;
    }

    void SetHidden()
    {
        SpawnedHPBar.gameObject.SetActive(false);
        OnVisibility = false;
    }

    void DeleteWidget()
    {
        Destroy(SpawnedHPBar.gameObject);
    }
}
