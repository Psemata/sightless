using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIGameManager : VisualElement
{
    VisualElement pauseScreen;
    VisualElement gameOverScreen;
    VisualElement creditScreen;

    public new class UxmlFactory : UxmlFactory<UIGameManager, UxmlTraits> { }

    public UIGameManager()
    {
        this.RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
    }

    void OnGeometryChange(GeometryChangedEvent evt)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Death += GoToGameOver;
            GameManager.Instance.Paused += GoToPause;
            GameManager.Instance.End += Finish;
            GameManager.Instance.Game += Game;
        }

        pauseScreen = this.Q("PauseMenu");
        gameOverScreen = this.Q("GameOver");
        creditScreen = this.Q("CreditMenu");

        pauseScreen.style.display = DisplayStyle.None;
        gameOverScreen.style.display = DisplayStyle.None;
        creditScreen.style.display = DisplayStyle.None;

        pauseScreen?.Q("resume")?.RegisterCallback<ClickEvent>(ev => Resume());
        pauseScreen?.Q("menu")?.RegisterCallback<ClickEvent>(ev => GoBackToMenu());

        gameOverScreen?.Q("respawn")?.RegisterCallback<ClickEvent>(ev => Respawn());

        creditScreen?.Q("menu")?.RegisterCallback<ClickEvent>(ev => GoBackToMenu());
    }

    /// <summary>
    /// Go to the Pause menu
    /// </summary>
    void GoToPause(object sender, EventArgs evt)
    {
        pauseScreen.style.display = DisplayStyle.Flex;
        gameOverScreen.style.display = DisplayStyle.None;
        creditScreen.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Go to the GameOver menu
    /// </summary>
    void GoToGameOver(object sender, EventArgs evt)
    {
        pauseScreen.style.display = DisplayStyle.None;
        gameOverScreen.style.display = DisplayStyle.Flex;
        creditScreen.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Go to the Credit menu
    /// </summary>
    void Finish(object sender, EventArgs evt)
    {
        pauseScreen.style.display = DisplayStyle.None;
        gameOverScreen.style.display = DisplayStyle.None;
        creditScreen.style.display = DisplayStyle.Flex;
    }

    void Game(object sender, EventArgs evt)
    {
        pauseScreen.style.display = DisplayStyle.None;
        gameOverScreen.style.display = DisplayStyle.None;
        creditScreen.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    private void Resume()
    {
        pauseScreen.style.display = DisplayStyle.None;
        gameOverScreen.style.display = DisplayStyle.None;
        creditScreen.style.display = DisplayStyle.None;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Game);
        }
    }

    /// <summary>
    /// Go to the main menu
    /// </summary>
    private void GoBackToMenu()
    {
        pauseScreen.style.display = DisplayStyle.None;
        gameOverScreen.style.display = DisplayStyle.None;
        creditScreen.style.display = DisplayStyle.None;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Menu);
        }
    }

    /// <summary>
    /// Resume the game and respawn
    /// </summary>
    private void Respawn()
    {
        gameOverScreen.style.display = DisplayStyle.None;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UpdateGameState(GameState.Respawn);
        }
    }

}
