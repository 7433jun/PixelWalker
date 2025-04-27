using UnityEngine;
using Project.Enums;

public class Chest : Interactable_Object
{
    [SerializeField] EChestType ChestType;
    [SerializeField] GameObject Mimic;

    public override void InteractEffect()
    {
        base.InteractEffect();

        switch(ChestType)
        {
            case EChestType.Normal:
                Anim.SetTrigger("OnInteracted");
                break;

            case EChestType.Special:
                Anim.SetTrigger("OnInteracted");
                break;

            case EChestType.Mimic:
                Vector3 SpawnLocation = transform.position;
                Vector3 ActorRotation = transform.localEulerAngles;
                GameObject SpawnedMimic = Instantiate(Mimic);
                SpawnedMimic.transform.position = SpawnLocation;
                if(ActorRotation.y > 179 && ActorRotation.y < 181)
                {
                    CharacterBase MimicData = SpawnedMimic.GetComponentInChildren<CharacterBase>();
                    MimicData.AddTag(MimicData.Tag_Invincible);
                    MimicData.Flip();
                }
                Destroy(gameObject);
                break;
        }
    }

    protected override void EndInteraction()
    {
        base.EndInteraction();
    }

    public void Chest_OpenChest()
    {
    }
}
