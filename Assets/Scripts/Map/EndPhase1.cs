using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPhase1 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name == "Karna")
        {
            GameManager.Instance.UpdateGameState(GameState.EndPhase1);
        }
    }
}
