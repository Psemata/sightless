using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPhase2 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Karna")
        {
            GameManager.Instance.UpdateGameState(GameState.Phase2);
        }
    }
}
