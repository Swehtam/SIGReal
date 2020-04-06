using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePlayer : MonoBehaviour
{
    private List<PlayersInfo> playerList = new List<PlayersInfo>();
    private int playersCount = 0;
    Text playerName;
    //Prefab para instanciar a escolha de infectados ou saudáveis
    public GameObject specificPlayerPrefab;
    public Transform playerListTransform;
    //Caixas de texto
    public GameObject healthStatusBox;
    //Botões
    public Button selectedPlayer;

    private void Awake()
    {
        CreatePlayerList();
    }

    IEnumerator FadeInHealthStatusBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            healthStatusBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeOutHealthStatusBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            healthStatusBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        healthStatusBox.SetActive(false);
    }

    //Método para obter todos os players da rodada
    public void CreatePlayerList()
    {
        GameObject playerController = GameObject.Find("PlayersController");
        PlayersManager aux = playerController.GetComponent<PlayersManager>();

        //Pegar players aleatorios da lista do outro script para colocar nesse
        int total = aux.PlayersCount;
        for (int i = 0; i < total; i++)
        {
            playerList.Add(aux.GetPlayer(i));
        }
    }

    //Método para filtrar apenas players em uma única condição de saúde
    public List<PlayersInfo> GetSpecificPlayers(bool healthStatus)
    {
        List<PlayersInfo> specificPlayers = new List<PlayersInfo>();


        foreach (PlayersInfo player in playerList)
        {
            if (player.healthStatus == healthStatus)
            {
                PlayersInfo selectedPlayer = new PlayersInfo(player.name, player.color, player.healthStatus);
                specificPlayers.Add(selectedPlayer);

            }
        }

        return specificPlayers;
    }

    //Método para instanciar na tela apenas players em uma condição de saúde
    public void ShowSpecificPlayers(bool healthStatus, float separator, float i, int screenHight, int playerHight)
    {
        List<PlayersInfo> specificPlayers = GetSpecificPlayers(healthStatus);

        foreach (PlayersInfo players in specificPlayers)
        {

            GameObject specificPlayer = Instantiate(specificPlayerPrefab, playerListTransform);
            specificPlayer.GetComponentInChildren<Text>().text = players.name;
            if (i == 0)
            {
                //Pega a borda da esquerda da tela e adiciona o separador + metade do tamanho do player
                specificPlayer.transform.localPosition = specificPlayer.transform.localPosition + new Vector3(0, (-screenHight / 2) + separator + (playerHight / 2));
            }
            else
            {
                //Pega a posição do x do player anterior e adiciona o separador + o tamanho de um player
                specificPlayer.transform.localPosition = specificPlayer.transform.localPosition + new Vector3(0, specificPlayers[playersCount - 1].y + separator + playerHight);
            }
            specificPlayers[playersCount].x = specificPlayer.transform.localPosition.x;
            specificPlayers[playersCount].y = specificPlayer.transform.localPosition.y;
            specificPlayers[playersCount].player = specificPlayer;
        }
    }

    public void SelectPlayer()
    {
        playerName = selectedPlayer.GetComponentInChildren<Text>();
        selectedPlayer.onClick.AddListener(ChangeHealthStatus);

    }

    //Método para infectar ou curar player
    public void ChangeHealthStatus()
    {
        bool health = true;

        foreach (PlayersInfo player in playerList)
        {
            if (player.name.Equals(playerName))
            {
                health = player.healthStatus;
                //Se estiver saudável, é infectado; caso contrário, é curado.
                if (health == true)
                {
                    health = false;
                    healthStatusBox.GetComponentInChildren<Text>().text = player.name + ", '\nvocê agora tem COVID-19! Clique em AVANÇAR.";
                }
                else
                {
                    health = true;
                    healthStatusBox.GetComponentInChildren<Text>().text = player.name + ", '\nvocê agora está SAUDÁVEL! Clique em AVANÇAR.";
                }
            }
        }        
        
        StartCoroutine(FadeInHealthStatusBox());

    }

}
