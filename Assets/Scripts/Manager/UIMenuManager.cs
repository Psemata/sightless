using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIMenuManager : VisualElement
{
    VisualElement mainMenuScreen;

    public new class UxmlFactory : UxmlFactory<UIMenuManager, UxmlTraits> { }

    public UIMenuManager()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
        mainMenuScreen = this.Q("MainMenuTemplate");

        mainMenuScreen?.Q("play")?.RegisterCallback<ClickEvent>(ev => NewGame());
        mainMenuScreen?.Q("quit")?.RegisterCallback<ClickEvent>(ev => Quit());
    }

    void NewGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.LaunchGame);
        }
    }

    void Quit()
    {
        Debug.Log("QUIT : Application");
        Application.Quit();
    }

}
