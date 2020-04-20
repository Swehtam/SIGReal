using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePlayer : MonoBehaviour
{
    private String playerColor;
    private String playerHealth;
    private ChallengerTricksManager challengerTricksManager;

    public void SelectPlayer()
    {
        gameObject.GetComponent<Button>().interactable = false;
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

        challengerTricksManager = GameObject.Find("EventSystem").GetComponent<ChallengerTricksManager>();
        int action = challengerTricksManager.Action;

        if(action == 2 || action == 3)
        {
            challengerTricksManager.InfectOrCurePlayer(playerColor, playerHealth);
        }
        else if (action == 4)
        {
            challengerTricksManager.ShowTrickDoneOrNotButtons(playerColor);
        }
        
    }

}
