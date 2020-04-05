using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettingsManager : MonoBehaviour
{
    //Essa variavel determina qual categoria é, essa variavel tem que ser atualizada por um script de configurações
    //0 = "Drinking Game"
    //1 = "Family Friendly"
    public int category = 0;
    private string mode;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public int GetCategory()
    {
        return category;
    }

    public string GetMode()
    {
        return mode;
    }

    public void ChangeCategory(int newCategory)
    {
        category = newCategory;
    }

    public void GoToCategoryScene(string modeName)
    {
        mode = modeName;
        Debug.Log("Modo: " + mode);
        SceneManager.LoadScene("CategorysScene", LoadSceneMode.Single);
    }

    public void GoToAddPlayersScene(int categoryValue)
    {
        category = categoryValue;
        Debug.Log("Categoria: " + category);
        SceneManager.LoadScene("AddPlayers", LoadSceneMode.Single);
    }

    public void GoBackScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }
}
