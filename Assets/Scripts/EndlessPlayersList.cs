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
    private bool playerGiveUp = false;
    private bool endGame = false;

    //Variaveis para instaciar um player na lista de players
    public GameObject playerPrefab;
    public Transform playersListTransform;
    //Caixas de texto
    public GameObject trickBox;
    public GameObject turnBox;
    public GameObject winBox;

    [Serializable]
    public struct PlayerColor
    {
        public String colorName;
        public Sprite image;
    }

    public PlayerColor[] playersColor;

    //Deve ser Awake para criar a lista de players antes que outro script precise utilizar da lista
    void Awake()
    {
        RandomizeList();
        LoadFirstPlayer();
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

        aux.RefillPlayerList();
    }

    //Metodo para instaciar players quando mudar para a tela do modo, este metodo é chamado pelo metodo "RandomizeList"
    public void InstantiatePlayer(float separator, float i, int screenWidth, int playerWidth)
    {
        //Instaciar o objeto na tela dentro do canvas e separar um do outro
        GameObject player = Instantiate(playerPrefab, playersListTransform);
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
        //Muda a imagem do player e desativa seu botão para outro player nao escolher
        foreach (PlayerColor pColor in playersColor)
        {
            if (pColor.colorName.Equals(playerList[playersCount].color))
            {
                player.GetComponentInChildren<Image>().sprite = pColor.image;
                break;
            }
        }
        playerList[playersCount].x = player.transform.localPosition.x;
        playerList[playersCount].y = player.transform.localPosition.y;
        playerList[playersCount].player = player;
    }

    private void LoadFirstPlayer()
    {
        turnBox.GetComponentInChildren<Text>().text = playerList[0].name + "!!\nSua vez";
    }

    public void ChangePlayerTurn()
    {
        if (!playerGiveUp)
        {
            StartCoroutine(MovePlayers());
            StartCoroutine(FadeOutFirstPlayer());
            StartCoroutine(FadeOutTrickBox());

            //Ativar Caixa de turno
            turnBox.SetActive(true);
            turnBox.GetComponentInChildren<Text>().text = playerList[1].name + "!!\nSua vez";
            StartCoroutine(FadeInTurnBox());
        }
    }

    //Metodo caso o player desista, variavel tem que ser verdadeira para nao adicionar o player no final da lista e tratar sua posição
    public void PlayerGiveUp()
    {
        if(!playerGiveUp)
        {
            if (playersCount > 2)
            {
                StartCoroutine(MovePlayers());
                playerGiveUp = true;
                StartCoroutine(FadeOutFirstPlayer());
                turnBox.GetComponentInChildren<Text>().text = playerList[1].name + "!!\nSua vez";
            }
            else if (playersCount == 2)
            {
                //Desaparece a caixa de turno
                StartCoroutine(FadeOutTurnBox());
                //Pega o vencedor e mostra na caixa de vencedor
                winBox.SetActive(true);
                StartCoroutine(FadeInWinBox());
                winBox.GetComponentInChildren<Text>().text = "Parabéns!!\n" + playerList[1].name + " venceu";
                //Move o vencedor para o centro (player 2)
                StartCoroutine(MoveWinner());
                //Remove o primeiro o mestre
                playerGiveUp = true;
                endGame = true;
                StartCoroutine(FadeOutFirstPlayer());
            }
        }
    }

    //Move todos os players para a esquerda, 
    //nao preciso tratar aqui caso o player desista, pois a animação termina antes de remover o player da lista
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
                    player.player.transform.localPosition = Vector3.Lerp(new Vector3(player.x, player.y), new Vector3(player.x - playerSeparator - playerWidth, player.y), t*2f);
                    t += Time.deltaTime;
                    yield return null;
                }
                i++;
            }
        }

        playersMoved = true;
    }

    //Move o vencedor para o meio da tela
    IEnumerator MoveWinner()
    {
        float t = 0f;
        while (t < 1f)
        {
            playerList[1].player.transform.localPosition = Vector3.Lerp(new Vector3(playerList[1].x, playerList[1].y), new Vector3(0, playerList[1].y), t * 1.5f);
            t += Time.deltaTime;
            yield return null;
        }
    }

    //Metodo para fazer com que o ultimo player apareça
    IEnumerator FadeInLastPlayer()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            playerList[playersCount - 1].player.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator FadeInTurnBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            turnBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //Aparece a caixa com o vencedor
    IEnumerator FadeInWinBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            winBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //Metodo para fazer com que o primeiro player suma
    IEnumerator FadeOutFirstPlayer()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            playerList[0].player.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        if (endGame != true)
        {
            firstPlayerRemoved = true;
        }
        else
        {
            Destroy(playerList[0].player);
            playersCount--;
            playerList.RemoveAt(0);
            playerGiveUp = false;
        }
        
    }

    IEnumerator FadeOutTrickBox()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            trickBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar Caixa de prendas
        trickBox.SetActive(false);
    }

    //Metodo para fazer a caixa de turno desaparecer
    IEnumerator FadeOutTurnBox()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            turnBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar Caixa de turno
        turnBox.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        //Caso ja tenha terminado as Coroutinas "FadeOutFirstPlayer" e "MovePlayers"
        if (firstPlayerRemoved == true && playersMoved == true)
        {
            //Caso ele nao tenha desistido coloca-lo no final da lista
            if (!playerGiveUp)
            {
                playerList.Add(playerList[0]);
            }
            else
            {
                Destroy(playerList[0].player);
                playersCount--;
            }
            playerList.RemoveAt(0);

            //Caso não tenha desistido, colocar o antigo primeiro da lista na posição do antigo ultimo da lista
            if (!playerGiveUp)
            {
                playerList[playersCount - 1].player.transform.localPosition = new Vector3(playerList[playersCount - 2].x, playerList[playersCount - 2].y);
                playerList[playersCount - 1].x = playerList[playersCount - 2].x;
            }

            for (int i = 0; i < playersCount; i++)
            {
                //Atualizar todos os players da lista caso o mestre tenha desistido ou atualiza até o penultimo caso contrario, pois o ultimo (antigo primeiro) ja foi atualizado
                if (i != playersCount - 1 || playerGiveUp)
                {
                    playerList[i].x = playerList[i].x - playerSeparator - playerWidth;
                }
            }

            //Aparecer ultimo player caso não tenha sido uma ação de desistência
            if (!playerGiveUp)
            {
                StartCoroutine(FadeInLastPlayer());
            }
            
            playerGiveUp = false;
            firstPlayerRemoved = false;
            playersMoved = false;
        }
    }
}