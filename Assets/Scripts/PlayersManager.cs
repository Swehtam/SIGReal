using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayersManager : MonoBehaviour
{
    //Lista para salvar todos os players cadastrados na partida
    private List<PlayersInfo> playerList = new List<PlayersInfo>();
    private List<PlayersInfo> backUpList = new List<PlayersInfo>();
    //Contador de players, máximo 10 (a referência de no máximo 10 está na função AddPlayer);
    private int playersCount = 0;
    //Script com o modo e a categoria
    private GameSettingsManager gameSettingsManager;
    private int category;
    private string mode;

    public int PlayersCount
    {
        get { return playersCount; }
    }

    public GameObject playerPrefab;
    public Transform playerListTransform;
    public InputField nameInput;
    public RectTransform movableComponents;

    private void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        gameSettingsManager = GameObject.Find("GameSettingsController").GetComponent<GameSettingsManager>();
        category = gameSettingsManager.GetCategory();
        mode = gameSettingsManager.GetMode();
    }

    public void AddPlayer()
    {
        if (!string.IsNullOrWhiteSpace(nameInput.text))
        {
            if (playersCount < 10)
            {
                playerList.Add(new PlayersInfo(nameInput.text));
                GameObject player = Instantiate(playerPrefab, playerListTransform);
                player.GetComponentInChildren<Text>().text = nameInput.text;
                if(PlayersCount != 0)
                {
                    player.transform.localPosition = new Vector3(playerList[PlayersCount-1].x, playerList[PlayersCount-1].y - 200);
                }
                playerList[playersCount].x = player.transform.localPosition.x;
                playerList[playersCount].y = player.transform.localPosition.y;


                backUpList.Add(playerList[playersCount]);
                playersCount++;
                nameInput.text = "";

                if(PlayersCount > 5)
                {
                    movableComponents.sizeDelta = new Vector2(1080, 1920 + ((PlayersCount - 5) * 200));
                }
            }
            else
            {
                //Colocar um aviso de que o número máximo de Players é de 10
                Debug.Log("full bitch");
            }
        }
    }

    public PlayersInfo GetPlayer(int number)
    {
        PlayersInfo player = playerList[number];
        playerList.RemoveAt(number);
        playersCount--;
        return player;
    }

    public void NextScene()
    {
        if(playersCount > 1)
        {
            //Lembrar de checar qual o modo que o player vai jogar e também a categoria
            SceneManager.LoadScene(mode, LoadSceneMode.Single);
        }
        else
        {
            //Colocar mensagem de que tem adicionar players para jogar
        }
        
    }

    public void RefillPlayerList()
    {
        foreach(PlayersInfo player in backUpList)
        {
            playerList.Add(player);
        }

        playersCount = backUpList.Count;
    }

    public void BackButton()
    {
        gameSettingsManager.GoBackScene("CategorysScene");
        Destroy(gameObject);
    }
}

public class PlayersInfo
{
    public string name;
    public float x;
    public float y;
    public GameObject player;

    //Cor do jogador, ainda nao é importante
    //private Color color;

    public PlayersInfo(string name)
    {
        this.name = name;
    }

    /*public PlayersInfo(string name, Color color)
    {
        this.name = name;
        this.color = color;
    }*/
}
