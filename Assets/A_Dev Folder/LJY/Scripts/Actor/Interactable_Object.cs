using UnityEngine;

public class Interactable_Object : MonoBehaviour
{
    // 상자, npc, 상점
    protected PlayerBase Player;
    protected Animator Anim;
    protected CircleCollider2D DetectCollider;

    void Awake()
    {
        Anim = GetComponent<Animator>();
        DetectCollider = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        Player = UtilityLibrary.GetPlayerCharacterInGame();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Player.PCont.InteractTarget = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Player.PCont.InteractTarget = null;

        EndInteraction();
    }

    public virtual void InteractEffect()
    {
        Player.PCont.IsInteraction = true;
    }

    protected virtual void EndInteraction()
    {
        Player.PCont.IsInteraction = false;
    }
}
