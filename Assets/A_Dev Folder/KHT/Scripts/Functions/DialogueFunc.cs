using UnityEngine;

public interface IDialogueFunc
{
    void Action();
}

public class Dialogue700002 : IDialogueFunc
{
    public void Action()
    {
        Debug.Log("Dialogue700002");
    }
}
