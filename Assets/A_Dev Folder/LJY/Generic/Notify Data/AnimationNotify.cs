using UnityEngine;
using Project.Enums;

public class AnimationNotify : MonoBehaviour
{
    public void DisableObject()
    {
        gameObject.SetActive(false);
    }
    
    public void PlaySoundToEffect(AudioClip AudioToPlay)
    {
        UtilityLibrary.PlaySound2D(AudioToPlay, EVolumeType.Effect, gameObject);
    }
}
