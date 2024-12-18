using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Opcoes : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider volumeSlider;
    public Slider sensitivitySlider;

    [Header("Game Settings")]
    public AudioSource audioSource;
    public float sensitivity = 1.0f;

    private void Start()
    { 
        volumeSlider.value = audioSource.volume;
        sensitivitySlider.value = sensitivity;

        volumeSlider.onValueChanged.AddListener(SetVolume);
        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
        Debug.Log($"Volume ajustado para: {value}");
    }

    public void SetSensitivity(float value)
    {
        sensitivity = value;
        Debug.Log($"Sensibilidade ajustada para: {value}");
    }
}
