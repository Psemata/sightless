using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayerSound : MonoBehaviour
{
    public AudioSource footstep;
    public float footstepVolume = 0.2f;
    public AudioClip[] footstepsSounds;
    public AudioSource wave;
    public float waveVolume = 0.2f;
    public AudioClip[] waveSounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayFootstepPlayerSound() {
        int random = Random.Range(0,footstepsSounds.Length-1);
        footstep.clip = footstepsSounds[random];
        footstep.volume = footstepVolume;
        footstep.Play();
    }
    void PlayClapSound() {
        int random = Random.Range(0,waveSounds.Length-1);
        wave.clip = waveSounds[random];
        wave.volume = waveVolume;
        wave.Play();
    }
}
