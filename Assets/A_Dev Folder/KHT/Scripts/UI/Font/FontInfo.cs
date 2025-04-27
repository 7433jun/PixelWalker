using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Font Info", menuName = "Scriptable Object/Font Info", order = 1004)]
public class FontInfo : ScriptableObject
{
    [SerializeField] TMP_FontAsset defaultTMP_Font;

    [Header("Order is \"Project.Enums.Language\"")]
    [SerializeField] List<TMP_FontAsset> tMP_FontList;

    public TMP_FontAsset DefaultTMP_Font => defaultTMP_Font;
    public List<TMP_FontAsset> TMP_FontList => tMP_FontList;
}
