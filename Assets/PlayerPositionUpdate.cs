using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionUpdate : MonoBehaviour
{
    public GameObject player;
    private Material mat;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.SetVector("_Position",player.transform.position);
    }
}
