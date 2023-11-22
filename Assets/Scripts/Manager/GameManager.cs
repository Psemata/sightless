using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum GameState
{
    Menu,
    Video,
    LaunchGame,
    Game,
    Respawn,
    Phase1,
    EndPhase1,
    Phase2,
    End,
    Death,
    Pause
}

public class GameManager : MonoBehaviour
{
    public GameState State { get; set; }

    public static GameManager Instance;

    // Spawns of the monster
    private Vector2 spawn1;
    private Vector2 spawn2;

    private GameObject monster;
    private GameObject karna;

    private bool videoAlreadyPlayed;
    private GameObject video;
    
    public Blit blit;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        blit.SetActive(false);
        videoAlreadyPlayed = false;
        UpdateGameState(GameState.Menu);
    }

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            UpdateGameState(GameState.Pause);
        }
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.Menu:
                SceneManager.LoadScene("InterfaceScene");
                break;
            case GameState.LaunchGame:
                SceneManager.LoadScene("MasterScene");
                break;
            case GameState.Game:
                break;
            case GameState.Respawn:
                this.karna = GameObject.Find("Karna").gameObject;
                Vector2 pos = this.karna.GetComponent<CharacterCheckpoint>().lastCheckpointPos;
                this.karna.GetComponent<CharacterController2D>().RespawnPoint(pos);
                this.monster.GetComponent<MonsterAI>().alreadyAttacked = false;
                this.monster.SetActive(false);
                UpdateGameState(GameState.Game);
                break;
            case GameState.Phase1:
                SpawnMonsterPhase1();
                break;
            case GameState.EndPhase1:
                DespawnMonsterPhase1();
                break;
            case GameState.Phase2:
                SpawnMonsterPhase2();
                break;
            case GameState.End:
                this.OnEnd(new EventArgs());
                break;
            case GameState.Pause:
                this.OnPause(new EventArgs());
                break;
            case GameState.Death:
                this.OnDeath(new EventArgs());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void SpawnMonsterPhase1()
    {
        // Move the monster to spawn 1
        this.monster.SetActive(true);
        this.monster.transform.position = this.spawn1;
        // Return to normal state
        UpdateGameState(GameState.Game);
    }

    void DespawnMonsterPhase1()
    {
        this.monster.SetActive(false);
        // Return to normal state
        UpdateGameState(GameState.Game);
    }

    void SpawnMonsterPhase2()
    {
        // Move the monster to spawn 2
        this.monster.SetActive(true);
        this.monster.transform.position = this.spawn2;
        this.monster.GetComponent<MonsterAI>().AttackMode();

        // Return to normal state
        UpdateGameState(GameState.Game);
    }

    void EndVideo(VideoPlayer vp)
    {
        videoAlreadyPlayed = true;
        vp.Pause();
        UpdateGameState(GameState.Game);
        this.blit.SetActive(true);
        this.video.SetActive(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += GameLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= GameLoaded;
    }

    void GameLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MasterScene")
        {
            this.spawn1 = GameObject.Find("Spawn1").transform.position;
            this.spawn2 = GameObject.Find("Spawn2").transform.position;
            this.monster = GameObject.Find("Monstre").gameObject;
            this.monster.SetActive(false);
            this.video = GameObject.Find("Video").gameObject;
            GameObject ui = GameObject.Find("UIGameManager");
            this.OnGame(new EventArgs());
            if (!videoAlreadyPlayed)
            {
                this.video.GetComponent<VideoPlayer>().loopPointReached += EndVideo;
            }
            else
            {
                this.video.SetActive(false);
            }

        }
    }

    public event EventHandler Paused;

    protected virtual void OnPause(EventArgs e)
    {
        EventHandler handler = Paused;
        handler?.Invoke(this, e);
    }

    public event EventHandler Death;
    protected virtual void OnDeath(EventArgs e)
    {
        EventHandler handler = Death;
        handler?.Invoke(this, e);
    }

    public event EventHandler End;
    protected virtual void OnEnd(EventArgs e)
    {
        EventHandler handler = End;
        handler?.Invoke(this, e);
    }

    public event EventHandler Game;
    protected virtual void OnGame(EventArgs e)
    {
        EventHandler handler = Game;
        handler?.Invoke(this, e);
    }
}
