using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{

    public void CloseHelp()
    {
        StartCoroutine(FadeOutHelpBox());
    }

    public void OpenHelp()
    {
        gameObject.SetActive(true);

        Scene scene = SceneManager.GetActiveScene();

        //Descobrir em qual cena tá.
        if (scene.name == "MainScene")
        {
            gameObject.GetComponentInChildren<Text>().text = "Este jogo foi desenvolvido durante a AKOM Quarantine Game Jam 2020, " +
                                                                "o propósito do jogo é divertir você e seus amigos durante a quarentena! " +
                                                                "\nVocê poderá jogar em dois modos: o infinito e o desafio. Em ambos, " +
                                                                "há um mestre que controla a vez do jogo. Basta olhar de quem é a vez e " +
                                                                "seguir as ordens do jogo! No primeiro modo, você e as pessoas ao seu " +
                                                                "redor serão adversários e poderão jogar até cansarem. Ou seja, vocês " +
                                                                "poderão ir retirando os players que forem desistindo de jogar, o vencedor " +
                                                                "é aquele que desistir por último. No segundo modo de jogo, há três fases, " +
                                                                "e ganha aquele que chegar na última parte do jogo sem estar contaminado pelo " +
                                                                "COVID-19. " +
                                                                "\nUm jogo Honey Joojs (Mathews Alves e Rebeca Bivar)!" +
                                                                "\nAgradecemos a todos que colaboraram com esse jogo!";
            
        }
        else if(scene.name == "CategorysScene")
        {
            gameObject.GetComponentInChildren<Text>().text = "Oi, de novo! :D " +
                                                              "\nVocê pode escolher entre duas CATEGORIAS de jogo:" +
                                                              "\n - Jogo de bebidas: nesse modo, as prendas envolverão " +
                                                              "consumo de bebidas alcoólicas, bebam com moderação! Beber" +
                                                              " muito pode baixar sua imunidade, não queremos isso, mas um " +
                                                              "pouquinho faz a gente rir pra caramba, haha. Se beber, não dirija." +
                                                              "\n - Jogo em família: esse aqui é pra quem tá jogando com a criançada, " +
                                                              "ou pra quem não quer ou não gosta de bebidas, haha. Ninguém fica de fora aqui!";
        }
        else if(scene.name == "EndlessMode")
        {
            gameObject.GetComponentInChildren<Text>().text = "Opa! Escolheu o infinito, né? Vamos que vamos! \nRelembrando: " +
                                                              "aqui o segredo é: não desista! Fique até o final, pague todas as " +
                                                              "prendas, e seja o ÚLTIMO a desistir! Assim o fazendo, você será a" +
                                                              " pessoa vencedora! Boa sorte! :D";
        }
        StartCoroutine(FadeInHelpBox());
    }

    IEnumerator FadeOutHelpBox()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.1f)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar o Help
        gameObject.SetActive(false);
 
    }

    //Metodo para fazer o Help desaparecer


    //Metodo para fazer as configurações aparecer
    IEnumerator FadeInHelpBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.1f)
        {
            gameObject.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
