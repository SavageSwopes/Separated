using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioSource audio;
    [SerializeField] private string volumeName;
    [SerializeField] private Slider musicSlider;

    public void SetVolume()
    {
        float volume = musicSlider.value;
        //audio.SetFloat(volumeName, Mathf.Log10(volume) * 20);
    }
}

