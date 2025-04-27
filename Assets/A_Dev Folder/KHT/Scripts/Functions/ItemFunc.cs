using UnityEngine;

public interface IItemFunc
{
    void Use();
    void Equip();
    void Unequip();
}

public class EmptyItem : IItemFunc
{
    public void Use()
    {
        Debug.LogError("Use() : Empty ItemIndex");
    }

    public void Equip()
    {
        Debug.LogError("Equip() : Empty ItemIndex");
    }

    public void Unequip()
    {
        Debug.LogError("Unequip() : Empty ItemIndex");
    }
}

public class Item2011001 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011001 Equip Function");

        //PlayerBase playerBase = UtilityLibrary.GetPlayerCharacterInGame();
        //
        //if (!playerBase)
        //    return;
        //
        //playerBase.PlayerSpec.EnableRegenHP(true);
    }

    public void Unequip()
    {
        Debug.Log("Item2011001 Unequip Function");

        //PlayerBase playerBase = UtilityLibrary.GetPlayerCharacterInGame();
        //
        //if (!playerBase)
        //    return;
        //
        //playerBase.PlayerSpec.EnableRegenHP(false);
    }
}

public class Item2011002 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011002 Equip Function");

        //PlayerBase playerBase = UtilityLibrary.GetPlayerCharacterInGame();
        //
        //if (!playerBase)
        //    return;
        //
        //playerBase.PlayerSpec.EnableDoubleJump(true);
    }

    public void Unequip()
    {
        Debug.Log("Item2011002 Unequip Function");

        //PlayerBase playerBase = UtilityLibrary.GetPlayerCharacterInGame();
        //
        //if (!playerBase)
        //    return;
        //
        //playerBase.PlayerSpec.EnableDoubleJump(false);
    }
}

public class Item2011003 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011003 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011003 Unequip Function");
    }
}

public class Item2011004 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011004 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011004 Unequip Function");
    }
}

public class Item2011005 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011005 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011005 Unequip Function");
    }
}

public class Item2011006 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011006 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011006 Unequip Function");
    }
}

public class Item2011007 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011007 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011007 Unequip Function");
    }
}

public class Item2011008 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011008 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011008 Unequip Function");
    }
}

public class Item2011009 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011009 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011009 Unequip Function");
    }
}

public class Item2011010 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011010 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011010 Unequip Function");
    }
}

public class Item2011011 : IItemFunc
{
    public void Use() { }

    public void Equip()
    {
        Debug.Log("Item2011011 Equip Function");
    }

    public void Unequip()
    {
        Debug.Log("Item2011011 Unequip Function");
    }
}