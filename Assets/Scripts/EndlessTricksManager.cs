using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class EndlessTricksManager : MonoBehaviour
{
    //Essa variavel determina qual das duas listas de tricks está sendo usada, pois vamos tentar não repetir os tricks até que acabe da lista
    //1 = lista "tricks"
    //2 = lista "backupTricks"
    private int wichList = 1;
    private List<EndlessTricks> tricks = new List<EndlessTricks>();
    private List<EndlessTricks> backupTricks = new List<EndlessTricks>();
    
    //Essa variavel determina qual categoria é, essa variavel tem que ser atualizada por um script de configurações
    //0 = "Drinking Game"
    //1 = "Family Friendly"
    private int category = 0;
    private System.Random random = new System.Random();
    private readonly string textFilePath = "/TricksText/EndlessTricks.txt";

    //Caixas de texto
    public GameObject trickBox;
    public GameObject turnBox;
    //Butões
    public GameObject giveUpButton;
    public GameObject getTrickButton;
    public GameObject otherTrickButton;
    public GameObject nextPlayerButton;

    // Start is called before the first frame update
    void Start()
    {
        LoadTricks();
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

    //Metodo para fazer a caixa de prendas aparecer
    IEnumerator FadeInTrickBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.05f)
        {
            trickBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ChangeTrick()
    {
        if (wichList == 1)
        {
            //Pega um numero aleatório e atualiza o texto no meio da tela
            int num = random.Next(tricks.Count);
            trickBox.GetComponentInChildren<Text>().text = tricks[num].text[category];

            //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
            backupTricks.Add(tricks[num]);
            tricks.RemoveAt(num);

            //Caso a lista fique vazio mudar a variavel do wichList para 2
            if (tricks.Count == 0)
            {
                wichList = 2;
            }
        }
        else
        {
            //Pega um numero aleatório e atualiza o texto no meio da tela
            int num = random.Next(backupTricks.Count);
            trickBox.GetComponentInChildren<Text>().text = backupTricks[num].text[category];

            //Coloca essa prenda para a outra lista, para não repetir prendas até acabar as que estão na lista
            tricks.Add(backupTricks[num]);
            backupTricks.RemoveAt(num);

            //Caso a lista fique vazio mudar a variavel do wichList para 2
            if (backupTricks.Count == 0)
            {
                wichList = 1;
            }
        }
    }

    public void GetTrick()
    {
        StartCoroutine(FadeOutTurnBox());

        //Ativar o trickbox
        trickBox.SetActive(true);
        //Pegar uma prenda e colocar no texto
        ChangeTrick();

        StartCoroutine(FadeInTrickBox());

        giveUpButton.SetActive(false);
        getTrickButton.SetActive(false);
        otherTrickButton.SetActive(true);
        nextPlayerButton.SetActive(true);
    }

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
                    tricks.Add(new EndlessTricks(phrases[0], phrases[1]));
                }
                file.Close();
            }
        }
    }
}

public class EndlessTricks
{
    public string[] text = new string[2];

    public EndlessTricks (string textDG, string textFF)
    {
        text[0] = textDG;
        text[1] = textFF;
    }
}