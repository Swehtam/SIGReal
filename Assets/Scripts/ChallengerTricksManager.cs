using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using UnityEngine.SceneManagement;

public class ChallengerTricksManager : MonoBehaviour
{
    //Se a lista principal de tricks ficar vazia, mudar para a lista de backup
    private List<ChallengerTricks> phase1Tricks = new List<ChallengerTricks>();
    private List<ChallengerTricks> phase1BackupTricks = new List<ChallengerTricks>();
    private List<ChallengerTricks> phase2Tricks = new List<ChallengerTricks>();
    private List<ChallengerTricks> phase2BackupTricks = new List<ChallengerTricks>();
    private List<ChallengerTricks> phase3Tricks = new List<ChallengerTricks>();
    private List<ChallengerTricks> phase3BackupTricks = new List<ChallengerTricks>();
    private ChallengerPlayersList challengerPlayersList;
    private GameSettingsManager gameSettingsManager;
    public int Phase { get; private set; } = 1;
    public int Action { get; private set; }
    public int FirstPlayerTrickDoneCounter { get; private set; } = 0;

    private System.Random random = new System.Random();
    private readonly string textFilePath = "/TricksText/ChallengerTricks.txt";

    //Objeto que mostra quantas vezes o player completou os desafios
    public GameObject tricksDoneCounter;
    
    //Caixas de texto
    public GameObject trickBox;
    public GameObject turnBox;
    public GameObject choosePlayerBox;
    public GameObject phaseBox;

    //Butões
    public GameObject nextPlayerButton;
    public GameObject trickDoneButton;
    public GameObject trickNotDoneButton;
    public GameObject choosePlayerButton;
    public GameObject getTrickButton;
    public GameObject continueButton;

    // Start is called before the first frame update
    void Start()
    {
        LoadTricks();
        challengerPlayersList = gameObject.GetComponent<ChallengerPlayersList>();
        gameSettingsManager = GameObject.Find("GameSettingsController").GetComponent<GameSettingsManager>();
    }

    //Metodo para carregar o arquivo .txt com os tricks do challenger
    private void LoadTricks()
    {
        if (File.Exists(Application.dataPath + textFilePath))
        {
            //Ler o arquivo usando StreamReader. Ler o arquivo linha por linha
            using (StreamReader file = new StreamReader(Application.dataPath + textFilePath))
            {
                string ln;

                while ((ln = file.ReadLine()) != null)
                {
                    string[] phrases = ln.Split(';');
                    int phase = int.Parse(phrases[3]);
                    if(phase == 1)
                    {
                        phase1Tricks.Add(new ChallengerTricks(phrases[0], phrases[1], int.Parse(phrases[2])));
                    }
                    else if (phase == 2)
                    {
                        phase2Tricks.Add(new ChallengerTricks(phrases[0], phrases[1], int.Parse(phrases[2])));
                    }
                    else
                    {
                        phase3Tricks.Add(new ChallengerTricks(phrases[0], phrases[1], int.Parse(phrases[2])));
                    } 
                }
                file.Close();
            }
        }
    }

    private void CheckTrickAction()
    {
        if (Phase == 1)
        {
            if (phase1Tricks.Count != 0)
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase1Tricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase1Tricks[num].text[gameSettingsManager.GetCategory()];

                Action = phase1Tricks[num].action;

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase1BackupTricks.Add(phase1Tricks[num]);
                phase1Tricks.RemoveAt(num);
            }
            else
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase1BackupTricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase1BackupTricks[num].text[gameSettingsManager.GetCategory()];

                Action = phase1BackupTricks[num].action;

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase1Tricks.Add(phase1BackupTricks[num]);
                phase1BackupTricks.RemoveAt(num);
            }
            
        }
        else if (Phase == 2)
        {
            if (phase2Tricks.Count != 0)
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase2Tricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase2Tricks[num].text[gameSettingsManager.GetCategory()];

                Action = phase2Tricks[num].action;

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase2BackupTricks.Add(phase2Tricks[num]);
                phase2Tricks.RemoveAt(num);
            }
            else
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase2BackupTricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase2BackupTricks[num].text[gameSettingsManager.GetCategory()];

                Action = phase2BackupTricks[num].action;

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase2Tricks.Add(phase2BackupTricks[num]);
                phase2BackupTricks.RemoveAt(num);
            }
        }
        else
        {
            tricksDoneCounter.SetActive(true);
            if (phase3Tricks.Count != 0)
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase3Tricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase3Tricks[num].text[gameSettingsManager.GetCategory()];

                Action = phase3Tricks[num].action;

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase3BackupTricks.Add(phase3Tricks[num]);
                phase3Tricks.RemoveAt(num);
            }
            else
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase3BackupTricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase3BackupTricks[num].text[gameSettingsManager.GetCategory()];

                Action = phase3BackupTricks[num].action;

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase3Tricks.Add(phase3BackupTricks[num]);
                phase3BackupTricks.RemoveAt(num);
            }
        }

        if (Action == 0)
        {
            trickDoneButton.SetActive(true);
            trickNotDoneButton.SetActive(true);
        }
        else if (Action == 1)
        {
            nextPlayerButton.SetActive(true);
        }
        else if (Action == 2)
        {
            choosePlayerButton.SetActive(true);
        }
        else if(Action == 3 || Action == 4)
        {
            if(challengerPlayersList.SickPlayersCount == 0)
            {
                CheckTrickAction();
            }
            else
            {
                choosePlayerButton.SetActive(true);
            }
        }
        else if (Action == 8)
        {
            trickDoneButton.SetActive(true);
            trickNotDoneButton.SetActive(true);
        }
    }

    public void GetTrick()
    {
        //Desaparece a caixa de turno
        StartCoroutine(FadeOutTurnBox());

        //Ativar o trickbox
        trickBox.SetActive(true);
        //Pegar uma prenda e colocar no texto
        CheckTrickAction();

        StartCoroutine(FadeInTrickBox());
    }

    private void NextPlayer()
    {
        challengerPlayersList.ChangePlayerTurn();
    }

    //Juntas todas as bases de dados aleatoriamente no phase2BackUpTrick
    private void JoinPhase1And2Tricks()
    {
        System.Random random = new System.Random();
        int total = phase1Tricks.Count;

        //Pega todos os tricks da phase1 aleatoriamente
        for(int i = 0; i < total; i++)
        {
            int num = random.Next(phase1Tricks.Count);
            phase2BackupTricks.Add(phase1Tricks[num]);
            phase1Tricks.RemoveAt(num);
        }

        total = phase1BackupTricks.Count;

        //Pega todos os tricks da phase1 aleatoriamente
        for (int i = 0; i < total; i++)
        {
            int num = random.Next(phase1BackupTricks.Count);
            phase2BackupTricks.Add(phase1BackupTricks[num]);
            phase1BackupTricks.RemoveAt(num);
        }

        total = phase2Tricks.Count;

        //Pega todos os tricks da phase1 aleatoriamente
        for (int i = 0; i < total; i++)
        {
            int num = random.Next(phase2Tricks.Count);
            phase2BackupTricks.Add(phase2Tricks[num]);
            phase2Tricks.RemoveAt(num);
        }
    }
    
    public void InitialScene()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        Destroy(GameObject.Find("GameSettingsController"));
        Destroy(GameObject.Find("PlayersController"));
    }

    public void RestartMode()
    {
        challengerPlayersList.CurePlayers();
        //Lembrar de colocar categorias dos modos
        SceneManager.LoadScene("ChallengerMode", LoadSceneMode.Single);
    }

    public void NextPlayerButton()
    {
        NextPlayer();
        nextPlayerButton.SetActive(false);

        if(Action == 1)
        {
            challengerPlayersList.InfectMaster();
        }
    }

    public void TrickDoneOrNotButton(bool done)
    {
        if(Action == 0)
        {
            if (!done)
            {
                challengerPlayersList.InfectMaster();
            }
            else
            {
                challengerPlayersList.CureMaster();
            }
            NextPlayer();
            trickDoneButton.SetActive(false);
            trickNotDoneButton.SetActive(false);
        }
        else if (Action == 4)
        {
            GameObject selectedPlayer = FindObjectOfType<ChoosePlayer>().gameObject;
            foreach (Text textComponent in selectedPlayer.GetComponentsInChildren<Text>())
            {
                if (textComponent.name.Equals("Cor"))
                {
                    if (!done)
                    {
                        challengerPlayersList.InfectPlayerByColor(textComponent.text);
                    }
                    else
                    {
                        challengerPlayersList.CurePlayerByColor(textComponent.text);
                    }
                }
            }

            StartCoroutine(FadeOutChoosePlayerBox());
            Destroy(selectedPlayer);
            NextPlayer();
            trickDoneButton.SetActive(false);
            trickNotDoneButton.SetActive(false);
        }
        else if (Action == 8)
        {
            if (!done)
            {
                FirstPlayerTrickDoneCounter = 0;
                tricksDoneCounter.GetComponentInChildren<Text>().text = "" + FirstPlayerTrickDoneCounter;
                challengerPlayersList.InfectMaster();
                NextPlayer();
                tricksDoneCounter.SetActive(false);
                trickDoneButton.SetActive(false);
                trickNotDoneButton.SetActive(false);
            }
            else
            {
                FirstPlayerTrickDoneCounter++;
                tricksDoneCounter.GetComponentInChildren<Text>().text = "" + FirstPlayerTrickDoneCounter;
                CheckTrickAction();
            }
        }
    }

    public void ChoosePlayerButton()
    {
        choosePlayerBox.SetActive(true);
        choosePlayerButton.SetActive(false);
        StartCoroutine(FadeInChoosePlayerBox());
        StartCoroutine(FadeOutTrickBox());

        if (Action == 2)
        {
            //Pega a lista de players que não estão infectados e passar para o metodo de instaciar esses botões na tela
            challengerPlayersList.InstantiatePlayersHealth(challengerPlayersList.GetSpecificPlayers(true));
            choosePlayerBox.GetComponentInChildren<Text>().text = "Infecte um jogador";
        }
        else if(Action == 3)
        {
            //Pega a lista de players que estão infectados e passar para o metodo de instaciar esses botões na tela
            challengerPlayersList.InstantiatePlayersHealth(challengerPlayersList.GetSpecificPlayers(false));
            choosePlayerBox.GetComponentInChildren<Text>().text = "Cure um jogador";
        }
        else if (Action == 4)
        {
            //Pega a lista de players que estão infectados e passar para o metodo de instaciar esses botões na tela
            challengerPlayersList.InstantiatePlayersHealth(challengerPlayersList.GetSpecificPlayers(false));
            choosePlayerBox.GetComponentInChildren<Text>().text = "Selecione um Jogador";
        }
    }
    
    public void InfectOrCurePlayer(string color, string health)
    {
        if (health.Equals("Saudável"))
        {
            challengerPlayersList.InfectPlayerByColor(color);
        }
        else
        {
            challengerPlayersList.CurePlayerByColor(color);
        }

        trickBox.SetActive(false);
        StartCoroutine(FadeOutChoosePlayerBox());

        foreach(ChoosePlayer aux in FindObjectsOfType<ChoosePlayer>())
        {
            Destroy(aux.gameObject);
        }

        NextPlayer();
    }

    public void GoToNextPhase(int phaseValue)
    {
        //Desaparece a caixa de turno
        StartCoroutine(FadeOutTurnBox());

        Phase = phaseValue;
        phaseBox.SetActive(true);
        phaseBox.GetComponentInChildren<Text>().text = "Fase " + Phase;
        StartCoroutine(FadeInPhaseBox());

        if(Phase == 2)
        {
            JoinPhase1And2Tricks();
        }
    }

    //Metodo para saber se o player infectado ou não fez a prenda
    public void ShowTrickDoneOrNotButtons(string color)
    {
        //Destroi todos os players que estão na caixa de seleção
        foreach (ChoosePlayer aux in FindObjectsOfType<ChoosePlayer>())
        {
            Destroy(aux.gameObject);
        }

        //Ativa os botãos para dizer que o player selecionado fez ou não a prenda
        trickDoneButton.SetActive(true);
        trickNotDoneButton.SetActive(true);

        choosePlayerBox.GetComponentInChildren<Text>().text = "Jogador selecionado";
        challengerPlayersList.InstantiateSelectedPlayer(color);
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

    //Metodo para fazer a caixa de prendas desaparecer
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

        //Desativar Caixa de turno
        turnBox.SetActive(false);
    }

    //Metodo para fazer a caixa de selecionar jogador desaparecer
    IEnumerator FadeOutChoosePlayerBox()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.05f)
        {
            choosePlayerBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar Caixa de selecionar jogador
        choosePlayerBox.SetActive(false);
    }

    //Metodo para fazer a caixa de prendas aparecer
    IEnumerator FadeInTrickBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            trickBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        nextPlayerButton.GetComponent<Button>().interactable = true;
        trickDoneButton.GetComponent<Button>().interactable = true;
        trickNotDoneButton.GetComponent<Button>().interactable = true;
        choosePlayerButton.GetComponent<Button>().interactable = true;
    }

    //Metodo para fazer a caixa de selecionar jogador aparecer
    IEnumerator FadeInChoosePlayerBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            choosePlayerBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    //Metodo para fazer a caixa de fase aparecer
    IEnumerator FadeInPhaseBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            phaseBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        continueButton.GetComponent<Button>().interactable = true;
    }
}

public class ChallengerTricks
{
    public string[] text = new string[2];
    //Variavel para dizer qual a ação do player
    //phase 1 ---------------
    //0 - Lê a prenda, se fizer não é infectado, mas se não conseguir/fizer é infectado
    //1 - Você foi infectado
    //2 - Escolher um player para ser infectado
    //------------------
    //phase 2---------------------
    //3 - Escolha alguém para curar
    //4 - Escolher infectado, se fizer a prenda é curado, se não fizer não é
    //5 - Escolher alguém de todos para ser curado caso esteja infectado ou infectado caso esteja saudavel e mostrar número aleatorio entre 2 até o numero máximo de pessoas
    //6 - Escolher alguém de todos para ser curado caso esteja infectado ou infectado caso esteja saudavel
    //7 - Escolher entre 1 infectado e 1 curado aleatório
    //----------------------------
    //phase 3---------------------
    //8 - Fez ou não fez?
    //----------------------------
    public int action;

    //No arquivo tem um quarto campo que é referente a qual fase o trick é do jogo, mas não precisa ser adicionado aqui
    //private int phase;

    public ChallengerTricks(string textDG, string textFF, int action)
    {
        text[0] = textDG;
        text[1] = textFF;
        this.action = action;
    }
}