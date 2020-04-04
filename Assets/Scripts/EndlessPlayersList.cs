using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EndlessPlayersList : MonoBehaviour
{
    private List<PlayersInfo> playerList = new List<PlayersInfo>();
    private int playersCount = 0;
    //Largura da tela
    private readonly int screenWidth = 1080;
    //Largura do icone do player
    private readonly int playerWidth = 200;
    //Diferença da posição entre os icones dos players
    private float playerSeparator;
    private bool firstPlayerRemoved = false;
    private bool playersMoved = false;

    //Variaveis para instaciar um player na lsita de players
    public GameObject playerPrefab;
    public Transform canvasTransform;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeList();
    }

    //Metodo para pegar a lista de players criada no outro script e ordena-los nesta lista de forma randomica
    public void RandomizeList()
    {
        //Criar o randomizador e achar o objeto que passou de cena sem destruir
        System.Random random = new System.Random();
        GameObject playerController = GameObject.Find("PlayersController");
        PlayersManager aux = playerController.GetComponent<PlayersManager>();

        //Pegar players aleatorios da lista do outro script para colocar nesse
        int total = aux.PlayersCount;
        for (int i = 0; i < total; i++)
        {
            //pega um numero aleatorio entre a lista de players e adiciona na lista deste script
            int number = random.Next(aux.PlayersCount);
            playerList.Add(aux.GetPlayer(number));

            float separator;
            //Se for 5 ou mais tem que pegar a largura da tela e dividir igualmente para 5
            //o restante dos players acima de 5 ficaram fora da tela, mas ao atualizar a lista eles aparecerão
            if (total >= 5)
            {
                //tamanho separando os players na lista; é o resto da subtração do tamanho da tela e o tamanho total que os players vão ocupar
                //divido por 6, pois são 6 espaços em branco que separam entre os players e os players e as bordas da tela
                separator = (screenWidth - (5 * playerWidth)) / 6;
            }
            else
            {
                //tamanho separando os players na lista; é o resto da subtração do tamanho da tela e o tamanho total que os players vão ocupar
                //divido por pelo numero de players+1, pois os espaços em branco que separam entre os players e os players e as bordas da tela é igual ao total de players+1
                separator = (screenWidth - (total * playerWidth)) / (total + 1);
            }
            playerSeparator = separator;
            InstantiatePlayer(separator, i, screenWidth, playerWidth);
            playersCount++;
        }

        //Destruir objeto com a lista antiga
        Destroy(playerController);
    }

    //Metodo para instaciar players quando mudar para a tela do modo, este metodo é chamado pelo metodo "RandomizeList"
    public void InstantiatePlayer(float separator, float i, int screenWidth, int playerWidth)
    {
        //Instaciar o objeto na tela dentro do canvas e separar um do outro
        GameObject player = Instantiate(playerPrefab, canvasTransform);
        player.GetComponentInChildren<Text>().text = playerList[playersCount].name;
        if (i == 0)
        {
            //Pega a borda da esquerda da tela e adiciona o separador + metade do tamanho do player
            player.transform.localPosition = player.transform.localPosition + new Vector3((-screenWidth / 2) + separator + (playerWidth / 2), 0);
        }
        else
        {
            //Pega a posição do x do player anterior e adiciona o separador + o tamanho de um player
            player.transform.localPosition = player.transform.localPosition + new Vector3(playerList[playersCount - 1].x + separator + playerWidth, 0);
        }
        playerList[playersCount].x = player.transform.localPosition.x;
        playerList[playersCount].y = player.transform.localPosition.y;
        playerList[playersCount].player = player;
    }

    IEnumerator FadeOutFirstPlayer()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            playerList[0].player.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        playerList.Add(playerList[0]);
        playerList.RemoveAt(0);
        firstPlayerRemoved = true;
    }

    IEnumerator MovePlayers()
    {
        float t = 0f;
        while (t < 1f)
        {
            int i = 0;
            foreach (PlayersInfo player in playerList)
            {
                if (i != 0)
                {
                    player.player.transform.localPosition = Vector3.Lerp(new Vector3(player.x, player.y), new Vector3(player.x - playerSeparator - playerWidth, player.y), t*1.5f);
                    t += Time.deltaTime;
                    yield return null;
                }
                i++;
            }
        }

        playersMoved = true;
    }

    IEnumerator FadeInLastPlayer()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            playerList[playersCount - 1].player.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ChangePlayerTurn()
    {
        StartCoroutine(FadeOutFirstPlayer());
        StartCoroutine(MovePlayers());
    }

    // Update is called once per frame
    void Update()
    {
        if(firstPlayerRemoved == true && playersMoved == true)
        {
            playerList[playersCount - 1].player.transform.localPosition = new Vector3(playerList[playersCount - 2].x, playerList[playersCount - 2].y);
            playerList[playersCount - 1].x = playerList[playersCount - 2].x;
            for (int i = 0; i < playersCount; i++)
            {
                if (i != playersCount - 1)
                {
                    playerList[i].x = playerList[i].x - playerSeparator - playerWidth;
                }
            }

            StartCoroutine(FadeInLastPlayer());
            firstPlayerRemoved = false;
            playersMoved = false;
        }
    }
}