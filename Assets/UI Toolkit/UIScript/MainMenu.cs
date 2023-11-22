using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class MainMenu : MonoBehaviour
{

    private Label title;
    private Button newGame;
    private Button loadGame;
    private Button settings;
    private Button exit;

    private void OnEnable()
    {
        //var r = GetComponent<UIDocument>().rootVisualElement;

        //newGame = r.Q<Button>("newGame");

        //newGame.clicked += NewGame;
    }

    private void NewGame()
    {
    }
    private void LoadGame()
    {
        //Load the saving data

        LauchGame();
    }
    private void LauchGame()
    {
       
    }
    private void Exit()
    {

    }
}
