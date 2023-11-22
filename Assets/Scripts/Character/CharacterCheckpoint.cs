using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCheckpoint : MonoBehaviour
{
    public Vector2 lastCheckpointPos;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.transform.tag == "Checkpoint"){
            this.lastCheckpointPos = collision.transform.position;
        }
    }
}
