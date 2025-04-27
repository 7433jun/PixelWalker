using Project.Enums;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public EItemType ItemType;
    public int DropValue;
    [SerializeField] PlayerBase Player;
    public Rigidbody2D rb { get; private set; }

    [SerializeField] AudioClip GetSound;

    private void Awake()
    {
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Player = UtilityLibrary.GetPlayerCharacterInGame();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject HitObject = collision.gameObject;
        if(HitObject.CompareTag("Player"))
        {
            //Debug.Log("Get Item");
            GetItem(ItemType);
        }
    }

    void GetItem(EItemType Type)
    {
        switch(Type)
        {
            case EItemType.HP_Potion:
                Player.PlayerSpec.AddPotion(DropValue, EHealType.HPType);
                break;

            case EItemType.MP_Potion:
                Player.PlayerSpec.AddPotion(DropValue, EHealType.MPType);
                break;

            case EItemType.Gold:
                Player.PlayerSpec.AddGold(DropValue);
                break;
        }

        UtilityLibrary.PlaySound2D(GetSound, EVolumeType.Effect);
        gameObject.SetActive(false);
    }

    public void AppearBounce()
    {
        //rb.AddForce(new Vector2(0, 3));
    }
}
