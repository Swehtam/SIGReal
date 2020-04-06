using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PlayersManager : MonoBehaviour
{
    private GameSettingsManager gameSettingsManager;
    //Lista reserva para quando for reinicar o modo
    private List<PlayersInfo> backUpList = new List<PlayersInfo>();
    private int category;

    //Lista para salvar todos os players cadastrados na partida
    public List<PlayersInfo> PlayerList { get; private set; } = new List<PlayersInfo>();
    //Script com o modo e a categoria
    
    public string Mode { get; private set; }
    //Contador de players, máximo 10 (a referência de no máximo 10 está na função AddPlayer);
    public int PlayersCount { get; private set; } = 0;
    
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        gameSettingsManager = GameObject.Find("GameSettingsController").GetComponent<GameSettingsManager>();
        category = gameSettingsManager.GetCategory();
        Mode = gameSettingsManager.GetMode();
    }

    //Adicionar players na lista, chamado pelo script AddPlayersManager
    public void AddPlayer(string name, string color, bool healthStatus)
    {
        PlayerList.Add(new PlayersInfo(name, color, healthStatus));
        backUpList.Add(PlayerList[PlayersCount]);
    }

    //Mudar o x e y do player na lista, chamado pelo script AddPlayersManager
    public void SetXY(float x, float y)
    {
        PlayerList[PlayersCount].x = x;
        PlayerList[PlayersCount].y = y;
    }

    //Incrementar o contador, chamado pelo script AddPlayersManager
    public void IncrementCounter()
    {
        PlayersCount++;
    }

    //Retornar um player para adicionar na lista do script do modo e remover deste script para nao ocorrer repetição de player, chamado pelos scripts EndlessPlayersList e ChallengerPlayersList
    public PlayersInfo GetPlayer(int number)
    {
        PlayersInfo player = PlayerList[number];
        PlayerList.RemoveAt(number);
        PlayersCount--;
        return player;
    }

    //Colocar no playersList todos os players salvos no bakcup para poder reiniciar o jogo, chamado pelos scripts EndlessTricksManager e ChallengerTricksManager
    public void RefillPlayerList()
    {
        foreach(PlayersInfo player in backUpList)
        {
            PlayerList.Add(player);
        }

        PlayersCount = backUpList.Count;
    }
}

public class PlayersInfo
{
    public string name;
    public bool healthStatus;
    public float x;
    public float y;
    public GameObject player;
    //Cor do jogador
    public string color;

    public PlayersInfo(string name)
    {
        this.name = name;
    }

    public PlayersInfo(string name, string color)
    {
        this.name = name;
        this.color = color;
    }

    public PlayersInfo(string name, string color, bool healthStatus)
    {
        this.name = name;
        this.color = color;
        this.healthStatus = healthStatus;
    }
}
