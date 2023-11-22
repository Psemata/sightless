using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMonsterSound : MonoBehaviour
{
    public AudioSource footstep;
    public float footstepVolume = 0.2f;
    public AudioClip[] footstepsSounds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PlayFootstepMonsterSound() {
        int random = Random.Range(0,footstepsSounds.Length-1);
        footstep.clip = footstepsSounds[random];
        footstep.volume = footstepVolume;
        footstep.Play();
    }
}
