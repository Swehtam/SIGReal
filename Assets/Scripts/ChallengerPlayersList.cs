using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ChallengerPlayersList : MonoBehaviour
{
    private List<PlayersInfo> playerList = new List<PlayersInfo>();
    private int playersCount = 0;

    private ChallengerTricksManager challengerTricksManager; 
    //Largura da tela
    private readonly int screenWidth = 1080;
    //Largura do icone do player
    private readonly int playerWidth = 200;
    //Diferença da posição entre os icones dos players
    private float playerSeparator;
    private bool firstPlayerRemoved = false;
    private bool playersMoved = false;
    //Mudar
    private bool endGame = false;

    //Contadores para saber quantos saudaveis e doentes tem
    public int HealthyPlayersCount { get; private set; } = 1;
    public int SickPlayersCount { get; private set; } = 0;

    //Prefabs
    public GameObject playerPrefab;
    public Transform playersListTransform;
    public GameObject pickPlayerPrefab;
    public Transform choosePlayerBoxTransform;
    //Caixas de texto
    public GameObject trickBox;
    public GameObject turnBox;
    public GameObject phaseBox;
    public GameObject winBox;

    //Botões
    public GameObject nextPlayerButton;
    public GameObject trickDoneButton;
    public GameObject trickNotDoneButton;
    public GameObject choosePlayerButton;
    public GameObject getTrickButton;
    public GameObject continueButton;

    [Serializable]
    public struct PlayerColor
    {
        public String colorName;
        public Sprite healthyImage;
        public Sprite sickImage;
    }

    public PlayerColor[] playersColor;

    //Deve ser Awake para criar a lista de players antes que outro script precise utilizar da lista
    void Awake()
    {
        RandomizeList();
        LoadFirstPlayer();
    }

    void Start()
    {
        challengerTricksManager = gameObject.GetComponent<ChallengerTricksManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Caso ja tenha terminado as Coroutinas "FadeOutFirstPlayer" e "MovePlayers"
        if (firstPlayerRemoved == true && playersMoved == true)
        {
            playerList.Add(playerList[0]);
            playerList.RemoveAt(0);

            //colocar o antigo primeiro da lista na posição do antigo ultimo da lista
            playerList[playersCount - 1].player.transform.localPosition = new Vector3(playerList[playersCount - 2].x, playerList[playersCount - 2].y);
            playerList[playersCount - 1].x = playerList[playersCount - 2].x;
            for (int i = 0; i < playersCount; i++)
            {
                //Atualizar todos os players da lista caso o mestre tenha desistido ou atualiza até o penultimo caso contrario, pois o ultimo (antigo primeiro) ja foi atualizado
                if (i != playersCount - 1)
                {
                    playerList[i].x = playerList[i].x - playerSeparator - playerWidth;
                }
            }

            firstPlayerRemoved = false;
            playersMoved = false;

            //Aparecer ultimo player
            StartCoroutine(FadeInLastPlayer());

            //Checagem se ta na terceira fase e se o mestre atual é o player saudavel ou não
            if (challengerTricksManager.Phase == 3 && !playerList[0].healthStatus)
            {
                if (HealthyPlayersCount == 1)
                {
                    //Se o próximo player estiver curado, chama essa função
                    if (playerList[1].healthStatus)
                    {
                        ChangePlayerTurn();
                    }
                    else
                    {
                        //Vou sumir de um por um até chegar no player antecendente ao que ta curado
                        StartCoroutine(MovePlayers());
                        StartCoroutine(FadeOutFirstPlayer());
                    }
                }
                else
                {
                    CureMaster();
                }
            }
        }

        if (challengerTricksManager.Phase == 1)
        {
            float percent = (float)SickPlayersCount / playersCount;
            if (percent >= 0.5f)
            {
                if (playersCount != 2)
                {
                    //Vai para a fase dois
                    challengerTricksManager.GoToNextPhase(2);
                }
                else
                {
                    //Vai para a fase dois
                    challengerTricksManager.GoToNextPhase(3);
                }

            }
        }
        else if (challengerTricksManager.Phase == 2)
        {
            if (HealthyPlayersCount == 1)
            {
                //Vai para a fase 3
                challengerTricksManager.GoToNextPhase(3);
            }
        }

        if (challengerTricksManager.FirstPlayerTrickDoneCounter == 3 && endGame == false)
        {
            //Desaparece a caixa de turno
            StartCoroutine(FadeOutTrickBox());
            //Pega o vencedor e mostra na caixa de vencedor
            winBox.SetActive(true);
            StartCoroutine(FadeInWinBox());
            winBox.GetComponentInChildren<Text>().text = "Parabéns!!\n" + playerList[0].name + " venceu";
            //Move o vencedor para o centro o primeiro player
            StartCoroutine(MoveWinner());
            StartCoroutine(FadeOutOtherPlayers());
            endGame = true;
        }

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

        HealthyPlayersCount = playersCount;

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
                player.GetComponentInChildren<Image>().sprite = pColor.healthyImage;
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

    public void CurePlayers()
    {
        foreach (PlayersInfo player in playerList)
        {
            player.healthStatus = true;
        }
    }

    //MUDAR A VEZ DO PLAYER
    public void ChangePlayerTurn()
    {
        StartCoroutine(MovePlayers());
        StartCoroutine(FadeOutFirstPlayer());
        StartCoroutine(FadeOutTrickBox());

        //Ativar Caixa de turno
        turnBox.SetActive(true);
        turnBox.GetComponentInChildren<Text>().text = playerList[1].name + "!!\nSua vez";
        StartCoroutine(FadeInTurnBox());
    }

    public void ContinueButton()
    {
        StartCoroutine(FadeOutPhaseBox());

        //Ativar Caixa de turno
        turnBox.SetActive(true);
        turnBox.GetComponentInChildren<Text>().text = playerList[0].name + "!!\nSua vez";
        StartCoroutine(FadeInTurnBox());
    }

    public bool GetMasterHealthStatus()
    {
        return playerList[0].healthStatus;
    }

    public void InfectMaster()
    {
        //Se o mestre estiver curado
        if (playerList[0].healthStatus)
        {
            playerList[0].healthStatus = false;
            foreach (PlayerColor pColor in playersColor)
            {
                if (pColor.colorName.Equals(playerList[0].color))
                {
                    playerList[0].player.GetComponentInChildren<Image>().sprite = pColor.sickImage;
                    break;
                }
            }

            //A checagem é importante para esse decremente e incremento
            HealthyPlayersCount--;
            SickPlayersCount++;
        }
        
    }

    public void InfectPlayerByColor(string color)
    {
        PlayersInfo aux = null;

        foreach (PlayersInfo player in playerList)
        {
            if (player.color.Equals(color))
            {
                aux = player;
                break;
            }
        }

        //Se o player da cor estiver saudavel então infecte
        if (aux.healthStatus)
        {
            foreach (PlayerColor pColor in playersColor)
            {
                if (pColor.colorName.Equals(aux.color))
                {
                    aux.healthStatus = false;
                    aux.player.GetComponentInChildren<Image>().sprite = pColor.sickImage;
                    break;
                }
            }

            //A checagem é importante para esse decremente e incremento
            HealthyPlayersCount--;
            SickPlayersCount++;
        }
        
    }

    public void CureMaster()
    {
        //Se o player estiver infectado então cure
        if (!playerList[0].healthStatus)
        {
            playerList[0].healthStatus = true;
            foreach (PlayerColor pColor in playersColor)
            {
                if (pColor.colorName.Equals(playerList[0].color))
                {
                    playerList[0].player.GetComponentInChildren<Image>().sprite = pColor.healthyImage;
                    break;
                }
            }

            //A checagem é importante para esse decremente e incremento
            HealthyPlayersCount++;
            SickPlayersCount--;
        }
    }

    public void CurePlayerByColor(string color)
    {
        PlayersInfo aux = null;

        foreach (PlayersInfo player in playerList)
        {
            if (player.color.Equals(color))
            {
                aux = player;
                break;
            }
        }

        //Se o player da cor selecionada estiver infectado então cure
        if (!aux.healthStatus)
        {
            foreach (PlayerColor pColor in playersColor)
            {
                if (pColor.colorName.Equals(aux.color))
                {
                    aux.healthStatus = true;
                    aux.player.GetComponentInChildren<Image>().sprite = pColor.healthyImage;
                    break;
                }
            }

            //A checagem é importante para esse decremente e incremento
            HealthyPlayersCount++;
            SickPlayersCount--;
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

    public void InstantiatePlayersHealth(List<PlayersInfo> playersListAux)
    {
        for (int i = 0; i < playersListAux.Count; i++)
        {
            //Instaciar o objeto na tela dentro do canvas e separar um do outro
            GameObject player = Instantiate(pickPlayerPrefab, choosePlayerBoxTransform);

            foreach(Text textComponent in player.GetComponentsInChildren<Text>())
            {
                //Salvar o nome e a cor no botão
                if(textComponent.gameObject.name.Equals("Nome"))
                {
                    textComponent.text = playersListAux[i].name;
                }
                else if(textComponent.gameObject.name.Equals("Cor"))
                {
                    //Isso daqui vai ser útil para quando for chamar o metodo do botão, ai ele pega a cor salva em si, busca na lista de players essa cor e muda para infectado ou curado
                    textComponent.text = playersListAux[i].color;
                }
                else
                {
                    if (playersListAux[i].healthStatus)
                    {
                        textComponent.text = "Saudável";
                    }
                    else
                    {
                        textComponent.text = "Infectado";
                    }
                }
            }

            if (i != 0)
            {
                //A cada 2 players adiciona -200 no y para descer 1 linha
                player.transform.localPosition = player.transform.localPosition + new Vector3(0, -200 * (i / 2));
            }

            if (i % 2 == 1)
            {
                //Para cada i de valor impar colocar o player do lado direito da tela adicionado 300 no x
                player.transform.localPosition = player.transform.localPosition + new Vector3(300, 0);
            }

            //Muda a imagem do player e desativa seu botão para outro player nao escolher
            foreach (PlayerColor pColor in playersColor)
            {
                if (pColor.colorName.Equals(playersListAux[i].color))
                {
                    //Se for saudavel
                    if (playersListAux[i].healthStatus)
                    {
                        player.GetComponentInChildren<Image>().sprite = pColor.healthyImage;
                    }
                    else //Se não for saudavel
                    {
                        player.GetComponentInChildren<Image>().sprite = pColor.sickImage;
                    }
                    break;
                }
            }
        }
    }

    public void InstantiateSelectedPlayer(string color)
    {
        foreach(PlayersInfo player in playerList)
        {
            if (player.color.Equals(color))
            {
                //Instaciar o objeto na tela dentro do canvas e separar um do outro
                GameObject playerPrefab = Instantiate(pickPlayerPrefab, choosePlayerBoxTransform);

                foreach (Text textComponent in playerPrefab.GetComponentsInChildren<Text>())
                {
                    //Salvar o nome e a cor no botão
                    if (textComponent.gameObject.name.Equals("Nome"))
                    {
                        textComponent.text = player.name;
                    }
                    else if (textComponent.gameObject.name.Equals("Cor"))
                    {
                        //Isso daqui vai ser útil para quando for chamar o metodo do botão, ai ele pega a cor salva em si, busca na lista de players essa cor e muda para infectado ou curado
                        textComponent.text = player.color;
                    }
                }

                //Instancia o player no meio do box de selecionar players
                playerPrefab.transform.localPosition = playerPrefab.transform.localPosition + new Vector3(0, -550);

                //Muda a imagem do player e desativa seu botão para outro player nao escolher
                foreach (PlayerColor pColor in playersColor)
                {
                    if (pColor.colorName.Equals(color))
                    {
                        //Se for saudavel
                        if (player.healthStatus)
                        {
                            playerPrefab.GetComponentInChildren<Image>().sprite = pColor.healthyImage;
                        }
                        else //Se não for saudavel
                        {
                            playerPrefab.GetComponentInChildren<Image>().sprite = pColor.sickImage;
                        }
                        break;
                    }
                }
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
                    player.player.transform.localPosition = Vector3.Lerp(new Vector3(player.x, player.y), new Vector3(player.x - playerSeparator - playerWidth, player.y), t * 2f);
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
            playerList[0].player.transform.localPosition = Vector3.Lerp(new Vector3(playerList[0].x, playerList[0].y), new Vector3(0, playerList[0].y), t * 1.5f);
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
        getTrickButton.GetComponent<Button>().interactable = true;
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
        //mudar
        else
        {
            Destroy(playerList[0].player);
            playersCount--;
            playerList.RemoveAt(0);
        }
    }

    IEnumerator FadeOutTrickBox()
    {
        nextPlayerButton.GetComponent<Button>().interactable = false;
        trickDoneButton.GetComponent<Button>().interactable = false;
        trickNotDoneButton.GetComponent<Button>().interactable = false;
        choosePlayerButton.GetComponent<Button>().interactable = false;

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
        getTrickButton.GetComponent<Button>().interactable = false;
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            turnBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar Caixa de turno
        turnBox.SetActive(false);
    }

    IEnumerator FadeOutPhaseBox()
    {
        continueButton.GetComponent<Button>().interactable = false;
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            phaseBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar Caixa de turno
        phaseBox.SetActive(false);
    }

    IEnumerator FadeOutOtherPlayers()
    {
        int counter = 0;
       
        foreach(PlayersInfo player in playerList)
        {
            if (counter != 0)
            {
                for (float f = 1f; f >= -0.05f; f -= 0.2f)
                {

                    player.player.GetComponent<CanvasGroup>().alpha = f;
                    yield return new WaitForSeconds(0.05f);
                }
            }

            counter++;

        }
    }
}
