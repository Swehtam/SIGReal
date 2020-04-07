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
    public int phase { get; private set; } = 1;
    private int action;
    
    //Essa variavel determina qual categoria é, essa variavel tem que ser atualizada por um script de configurações
    //0 = "Drinking Game"
    //1 = "Family Friendly"
    private int category = 0;
    private System.Random random = new System.Random();
    private readonly string textFilePath = "/TricksText/ChallengerTricks.txt";
    
    //Caixas de texto
    public GameObject trickBox;
    public GameObject turnBox;
    public GameObject choosePlayerBox;

    //Butões
    public GameObject nextPlayerButton;
    public GameObject trickDoneButton;
    public GameObject trickNotDoneButton;
    public GameObject choosePlayerButton;

    // Start is called before the first frame update
    void Start()
    {
        LoadTricks();
        challengerPlayersList = gameObject.GetComponent<ChallengerPlayersList>();
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
        ChallengerTricks trick;
        if (phase == 1)
        {
            if (phase1Tricks.Count != 0)
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase1Tricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase1Tricks[num].text[category];

                trick = phase1Tricks[num];

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase1BackupTricks.Add(phase1Tricks[num]);
                phase1Tricks.RemoveAt(num);
            }
            else
            {
                //Pega um numero aleatório e atualiza o texto no meio da tela
                int num = random.Next(phase1BackupTricks.Count);
                trickBox.GetComponentInChildren<Text>().text = phase1BackupTricks[num].text[category];

                trick = phase1Tricks[num];

                //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
                phase1Tricks.Add(phase1BackupTricks[num]);
                phase1BackupTricks.RemoveAt(num);
            }

            if(trick.action == 0)
            {
                trickDoneButton.SetActive(true);
                trickNotDoneButton.SetActive(true);
            }
            else if(trick.action == 1)
            {
                challengerPlayersList.InfectMaster();
                nextPlayerButton.SetActive(true);
            }
            else if (trick.action == 2)
            {
                action = 2;
                choosePlayerButton.SetActive(true);
            }
        }
        else if (phase == 2)
        {

        }
        else
        {

        }
    }

    private void NextPayer()
    {
        challengerPlayersList.ChangePlayerTurn();
    }

    public void GetTrick()
    {
        StartCoroutine(FadeOutTurnBox());

        //Ativar o trickbox
        trickBox.SetActive(true);
        //Pegar uma prenda e colocar no texto
        CheckTrickAction();

        StartCoroutine(FadeInTrickBox());
    }

    public void InitialScene()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        Destroy(GameObject.Find("GameSettingsController"));
        Destroy(GameObject.Find("PlayersController"));
    }

    public void RestartMode()
    {
        //Lembrar de colocar categorias dos modos
        SceneManager.LoadScene("ChallengerMode", LoadSceneMode.Single);
    }

    public void NextPlayerButton()
    {
        NextPayer();
        nextPlayerButton.SetActive(false);
    }

    public void TrickDoneOrNotButton(bool done)
    {
        if (!done)
        {
            challengerPlayersList.InfectMaster();
        }
        else
        {
            challengerPlayersList.CureMaster();
        }

        NextPayer();
        trickDoneButton.SetActive(false);
        trickNotDoneButton.SetActive(false);
    }

    public void ChoosePlayerButton()
    {
        choosePlayerBox.SetActive(true);
        choosePlayerButton.SetActive(false);
        StartCoroutine(FadeInChoosePlayerBox());
        StartCoroutine(FadeOutTrickBox());

        if (action == 2)
        {
            //Pega a lista de players que não estão infectados e passar para o metodo de instaciar esses botões na tela
            challengerPlayersList.InstantiatePlayersHealth(challengerPlayersList.GetSpecificPlayers(true));
            choosePlayerBox.GetComponentInChildren<Text>().text = "Infecte um jogador";
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

        StartCoroutine(FadeOutChoosePlayerBox());

        foreach(ChoosePlayer aux in FindObjectsOfType<ChoosePlayer>())
        {
            Destroy(aux.gameObject);
        }

        NextPayer();
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

    //Metodo para fazer a caixa de prendas desaparecer
    IEnumerator FadeOutTrickBox()
    {
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