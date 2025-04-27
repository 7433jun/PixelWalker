using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Buff Info", menuName = "Scriptable Object/Buff Info", order = 1003)]
public class BuffInfo : ScriptableObject
{
    [SerializeField] List<Sprite> buffSpriteList;

    public List<Sprite> BuffSpriteList => buffSpriteList;
}
