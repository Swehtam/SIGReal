using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CategoryManager : MonoBehaviour
{
    //Script com o modo e a categoria
    private GameSettingsManager gameSettingsManager;

    // Start is called before the first frame update
    void Start()
    {
        gameSettingsManager = GameObject.Find("GameSettingsController").GetComponent<GameSettingsManager>();
    }

    public void SetCategory(int categoryValue)
    {
        gameSettingsManager.GoToAddPlayersScene(categoryValue);
    }

    public void BackButton()
    {
        gameSettingsManager.GoBackScene("MainScene");
        Destroy(GameObject.Find("GameSettingsController"));
    }
}
