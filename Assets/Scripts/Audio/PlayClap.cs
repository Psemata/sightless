using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayClap : MonoBehaviour
{
    public AudioSource clap;
    public AudioSource step;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            clap.Play();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            step.loop = true;
            step.Play();
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            step.loop = false;
        }
    }
}
