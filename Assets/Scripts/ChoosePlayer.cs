using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePlayer : MonoBehaviour
{
    private String playerColor;
    private String playerHealth;
    
    public void SelectPlayer()
    {
        foreach (Text textComponent in gameObject.GetComponentsInChildren<Text>())
        {
            if (textComponent.name.Equals("Cor"))
            {
                playerColor = textComponent.text;
            }
            else if (textComponent.name.Equals("Saúde"))
            {
                playerHealth = textComponent.text;
            }
        }

        GameObject.Find("EventSystem").GetComponent<ChallengerTricksManager>().InfectOrCurePlayer(playerColor, playerHealth);
    }

}
