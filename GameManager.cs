using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject ninja, knight, samurai; //player characters
    public GameObject sp1, sp2;//spawn points for player 1 and 2
    public GameObject mainMenu, charSelect;//menu references
    public GameObject stage; //reference to the stage

    public Sprite pOneInd, pTwoInd; //player indicators for who is who for when players pick the same character
    bool oneP;//for checking how many players. true = one player, false = two players
    bool onePicking = false, twoPicking = false;//for checking which player picked a character



    // Start is called before the first frame update
    void Start()
    {
        //checking if everything is set active and inactive as it should be
        if (stage.activeSelf)
            stage.SetActive(false);
        if (!mainMenu.activeSelf)
            mainMenu.SetActive(true);
        if (charSelect.activeSelf == true)
            charSelect.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    //function for when player selects 1 or 2 players then switched menu screens
    public void PlayerSelect(bool singlePlay)
    {
        oneP = singlePlay;
        mainMenu.SetActive(false);
        charSelect.SetActive(true);
        onePicking = true;
    }
    
    // function to create a character for the players
    GameObject SetupCharacter(string sChar, Transform spawnPoint)
    {
        GameObject temp = null; ;
        switch (sChar)
        {
            case "Knight":
                temp = Instantiate(knight, spawnPoint);
                break;
            case "Samurai":
                temp = Instantiate(samurai, spawnPoint);
                break;
            case "Ninja":
                temp = Instantiate(ninja, spawnPoint);
                break;
            case "":
                print("have you checked that you have typed something in");
                temp = null;
                break;
        }
        temp.transform.parent = stage.transform;
        return temp;
    }

    //for when the player selects a character
    public void PickCharacter(string sChar)
    {
        GameObject temp;
        if (onePicking)
        {
            temp = SetupCharacter(sChar, sp1.transform);
            temp.GetComponent<playerController>().p1 = true;
            print("Player 1 picked the " + temp.name);
            //if 2 players was selected then set onePicking to false and twoPicking to true to let second player pick their character
            if (!oneP)
            {
                onePicking = false;
                twoPicking = true;
                //changing the instructions to tell the second player to select a character
                charSelect.transform.Find("Instructions").GetComponent<Text>().text = "Player 2 select a Character";
            }
            else
            {
                onePicking = false;
                charSelect.SetActive(false);
                stage.SetActive(true);
            }
        }
        else if (twoPicking)
        {
            temp = SetupCharacter(sChar, sp2.transform);
            temp.GetComponent<playerController>().p2 = true;
            print("Player 2 picked the "+ temp.name);
            twoPicking = false;
            charSelect.SetActive(false);
            stage.SetActive(true);
        }
    }
}