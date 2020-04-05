using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SettingsBox : MonoBehaviour
{
    //Script com o modo e a categoria
    private GameSettingsManager gameSettingsManager;
    private GameObject teste;

    public GameObject DrinkingGameButton;
    public GameObject FamilyFriendlyButton;
    public GameObject MainMenuButton;
    public GameObject CategoryChange;

    // Start is called before the first frame update
    void Awake()
    {
        gameSettingsManager = GameObject.Find("GameSettingsController").GetComponent<GameSettingsManager>();
    }

    public void CloseSetting()
    {
        StartCoroutine(FadeOutSettingsBox());
    }

    public void OpenSetting()
    {
        gameObject.SetActive(true);

        Scene scene = SceneManager.GetActiveScene();
        //Nas cenas do Menu principal, no de escolher categoria e no de adcionar players não vai aparecer os botãos de ir para o Menu principal e mudar categoria
        if (scene.name == "EndlessMode" || scene.name == "ChallengerMode")
        {
            MainMenuButton.SetActive(true);
            CategoryChange.SetActive(true);

            if (gameSettingsManager == null)
            {
                Debug.Log("wtf");
            }
            int category = gameSettingsManager.category;
            if(category == 0)
            {
                DrinkingGameButton.SetActive(true);
            }
            else
            {
                FamilyFriendlyButton.SetActive(true);
            }
        }

        StartCoroutine(FadeInSettingsBox());
    }

    public void ChangeCategory()
    {
        int category = gameSettingsManager.GetCategory();
        //Categoria é Drinking Game, então mudar para Family Friendly
        if (category == 0)
        {
            gameSettingsManager.ChangeCategory(1);
            DrinkingGameButton.SetActive(false);
            FamilyFriendlyButton.SetActive(true);
        }
        else //Mudar categoria de Family Friendly para Drinking Game
        {
            gameSettingsManager.ChangeCategory(0);
            DrinkingGameButton.SetActive(true);
            FamilyFriendlyButton.SetActive(false);
        }
    }

    public void MainScene()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    //Metodo para fazer as configurações desaparecer
    IEnumerator FadeOutSettingsBox()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.1f)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar as Configurações
        gameObject.SetActive(false);

        Scene scene = SceneManager.GetActiveScene();
        //Nas cenas do Menu principal, no de escolher categoria e no de adcionar players não vai aparecer os botãos de ir para o Menu principal e mudar categoria
        if (scene.name == "EndlessMode" || scene.name == "ChallengerMode")
        {
            MainMenuButton.SetActive(false);
            CategoryChange.SetActive(false);

            int category = gameSettingsManager.GetCategory();
            if (category == 0)
            {
                DrinkingGameButton.SetActive(false);
            }
            else
            {
                FamilyFriendlyButton.SetActive(false);
            }
        }
    }

    //Metodo para fazer as configurações aparecer
    IEnumerator FadeInSettingsBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.1f)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
