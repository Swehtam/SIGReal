using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class AddPlayersManager : MonoBehaviour
{
    private string color;
    private PlayersManager playersManager;

    public GameObject playerPrefab;
    public Transform playerListTransform;
    public RectTransform movableComponents;

    //Botões para deixar nao interativeis quando chegar em 10 players
    public InputField nameInput;
    public Button chooseColorButton;
    public Button addPlayerButton;

    //Objeto onde aparece as cores para serem escolhidas
    public GameObject playersColorBox;
    //Sprite para mudar imagem do icone de escolher a cor
    public Sprite huePlayerIcon;

    [Serializable]
    public struct IconColorButton
    {
        public Button iconButton;
        public String colorName;
        public Sprite image;
    }

    public IconColorButton[] colorButton;

    // Start is called before the first frame update
    void Start()
    {
        playersManager = GameObject.Find("PlayersController").GetComponent<PlayersManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playersManager.PlayersCount == 10)
        {
            nameInput.interactable = false;
            chooseColorButton.interactable = false;
            addPlayerButton.interactable = false;
        }
    }
    
    //Adicionar os players na Scene AddPlayers e na lista do PlayersManager
    public void AddPlayer()
    {
        if (playersManager.PlayersCount < 10)
        {
            if (!string.IsNullOrWhiteSpace(nameInput.text))
            {
                if (color != null)
                {
                    //Adiciona os players na lista do Script gerenciador
                    playersManager.AddPlayer(nameInput.text, color, true);
                    //Cria um objeto na Scene de AddPlayers, muda sua posição, sua imagem e seu texto
                    GameObject player = Instantiate(playerPrefab, playerListTransform);
                    player.GetComponentInChildren<Text>().text = nameInput.text;
                    if (playersManager.PlayersCount != 0)
                    {
                        player.transform.localPosition = new Vector3(playersManager.PlayerList[playersManager.PlayersCount - 1].x, playersManager.PlayerList[playersManager.PlayersCount - 1].y - 200);
                    }
                    //Muda a imagem do player e desativa seu botão para outro player nao escolher
                    foreach (IconColorButton button in colorButton)
                    {
                        if (button.colorName.Equals(color))
                        {
                            player.GetComponentInChildren<Image>().sprite = button.image;
                            button.iconButton.gameObject.SetActive(false);
                            break;
                        }
                    }
                    //Seta o x e y do player dentro de sua lista, para ser referenciado durante a Scene AddPlayers
                    playersManager.SetXY(player.transform.localPosition.x, player.transform.localPosition.y);


                    playersManager.IncrementCounter();
                    nameInput.text = "";
                    color = null;
                    chooseColorButton.image.sprite = huePlayerIcon;
                    if (playersManager.PlayersCount > 5)
                    {
                        movableComponents.sizeDelta = new Vector2(1080, 1920 + ((playersManager.PlayersCount - 5) * 200));
                    }
                }
                else
                {
                    //Colocar um aviso pra escolher uma cor pro player
                    Debug.Log("escolha uma cor");
                }
            }
            else
            {
                //Colocar um aviso pra escrever um nome pro player
                Debug.Log("escreva um nome");
            }
        }
        else
        {
            //Colocar um aviso de que o número máximo de Players é de 10
            Debug.Log("full bitch");
        }
    }

    //Ir para a Scene do modo correto
    public void NextScene()
    {
        if (playersManager.PlayersCount > 1)
        {
            //Lembrar de checar qual o modo que o player vai jogar e também a categoria
            SceneManager.LoadScene(playersManager.Mode, LoadSceneMode.Single);
        }
        else
        {
            //Colocar mensagem de que tem adicionar players para jogar
        }

    }

    //Voltar para a Scene de Categoria
    public void BackButton()
    {
        SceneManager.LoadScene("CategorysScene", LoadSceneMode.Single);
        Destroy(playersManager.gameObject);
    }

    public void ChooseColor(string colorName)
    {
        color = colorName;
        foreach (IconColorButton button in colorButton)
        {
            if (button.colorName.Equals(colorName))
            {
                chooseColorButton.image.sprite = button.image;
            }
        }
        StartCoroutine(FadeOutColorBox());
    }

    public void OpenChoosePlayersColor()
    {
        playersColorBox.SetActive(true);
        StartCoroutine(FadeInColorBox());
    }

    public void CloseChoosePlayersColor()
    {
        StartCoroutine(FadeOutColorBox());
    }

    IEnumerator FadeOutColorBox()
    {
        for (float f = 1f; f >= -0.05f; f -= 0.2f)
        {
            playersColorBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }

        //Desativar Caixa de turno
        playersColorBox.SetActive(false);
    }

    //Metodo para fazer a caixa de prendas aparecer
    IEnumerator FadeInColorBox()
    {
        for (float f = 0f; f <= 1.05f; f += 0.2f)
        {
            playersColorBox.GetComponent<CanvasGroup>().alpha = f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
