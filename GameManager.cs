using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public variables
    public int enemiesSpawned = 0;
    public int needToBeat;//how many enemies need to be beaten to pass this wave
    public int defeatedEnemies;//how many enemies have been defeated
    public static int playerCount;

    public GameObject ninja, knight, samurai; //player characters
    public GameObject sp1, sp2;//spawn points for player 1 and 2
    public GameObject mainMenu, charSelect;//menu references
    public GameObject stage; //reference to the stage
    public GameObject eSpawn;//enemy spawn point - this will have a reference to a parent gameObject that holds all the enemy spawn points
    public GameObject[] enemies;//array of enemies
    public Sprite boxTex;
    public Sprite pOneInd, pTwoInd; //player indicators for who is who for when players pick the same character
    public Texture[] player1Controls, player2Controls;
    public Text p1Lives, p2Lives, p1LivesDisp, p2LivesDisp;
    //private variables
    playerController p1, p2;//playerController reference for both players
    bool oneP;//for checking how many players. true = one player, false = two players
    bool onePicking = false, twoPicking = false;//for checking which player picked a character
    bool howToPlay = false;
    bool gameOver = false;
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
        if (stage.activeSelf)//only run this if the stage is active
        {
            p1Lives.text = p1.lives.ToString();
            if (!oneP) { 
                p2Lives.text = p2.lives.ToString();
            }
            print(playerCount);
            if(playerCount <= 0)
            {
                print("GAME IS DONE");
                gameOver = true;
                quitButton();
            }
            if (enemiesSpawned < 3 && defeatedEnemies < needToBeat) //spawn enemies whenever there are less than this number or the max defeated enemies has been reached for this wave
                SpawnEnemy();
            //else
            //spawn boss
        }
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
        playerCount++;
        return temp;
    }
    //for spawning enemies to the stage
    void SpawnEnemy()
    {
        int i = Random.Range(0, enemies.Length - 1);//get a random value where min is 0 and max is the size of enemies-1 (since arrays start at 0)
        Transform[] sArray = eSpawn.GetComponentsInChildren<Transform>();//array of spawn points that will be put into a list
        List<Transform> spawnP = new List<Transform>();//A list will contain the spawn points in the scene
        //copying the array into the list so that we don't have the parent in the list
        for (int k = 0; k < sArray.Length; k++)
        {
            if (sArray[i].gameObject != this.gameObject)
                spawnP.Add(sArray[i]);
        }
        int j = Random.Range(0, sArray.Length - 2) + 1;
        Instantiate(enemies[i], sArray[j]);
        enemiesSpawned++;
    }
    //for when the player selects a character
    public void PickCharacter(string sChar)
    {
        GameObject temp;
        if (onePicking)
        {
            temp = SetupCharacter(sChar, sp1.transform);
            p1 = temp.GetComponent<playerController>();
            p1.p1 = true;
            p1.spawn = sp1;
            p1Lives.text = p1.lives.ToString();
            temp.transform.Find("PlayerIndicator").GetComponent<SpriteRenderer>().sprite = pOneInd;
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
                //p2LivesDisp.enabled = false;
                p2LivesDisp.gameObject.SetActive(false);
                onePicking = false;
                charSelect.SetActive(false);
                stage.SetActive(true);
            }
        }
        else if (twoPicking)
        {
            temp = SetupCharacter(sChar, sp2.transform);
            p2 = temp.GetComponent<playerController>();
            p2.p2 = true;
            p2.spawn = sp2;
            p2Lives.text = p2.lives.ToString();
            temp.transform.Find("PlayerIndicator").GetComponent<SpriteRenderer>().sprite = pTwoInd;
            twoPicking = false;
            charSelect.SetActive(false);
            stage.SetActive(true);
        }
    }

    public void HowToPlay()
    {
        howToPlay = true;
    }

    public void quitButton()
    {
        print("Quitting...");
#if UNITY_EDITOR
        //Application.Quit() doesnt work in editor we use this function instead
        UnityEditor.EditorApplication.isPlaying = false;
#else
    //this will be called when not running in the editor
        Application.Quit();
#endif


        print(Application.isPlaying);

    }

    private void OnGUI()
    {
        if (howToPlay)
        {
            GUI.enabled = true;
            mainMenu.SetActive(false);
            float sWidth = Screen.width;
            float sHeight = Screen.height;
            float sMidW = sWidth / 2;
            float sMidH = sHeight / 2;
            Rect box = new Rect(sMidW - 450, sMidH - 225, sMidW * 1.5f, sMidH * 1.5f);
            Rect textArea = new Rect(box.x + 50, box.y + 50, box.width - 50, box.height - 100);
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            GUIStyle textStyle = new GUIStyle();
            boxStyle.fontSize = 24;
            boxStyle.normal.textColor = Color.white;
            textStyle.fontSize = 18;
            textStyle.normal.textColor = Color.white;
            GUI.Box(box, "How to play", boxStyle);

            GUI.Label(textArea, "\t\t\tPlayer 1:\t\t\t\tPlayer 2:\n\nJump\n\n\n\nMove Left\n\n\n\nMove Right\n\n\n\nAttack", textStyle);

            GUI.DrawTexture(new Rect(box.x + 305, box.y + 90, 30, 32), player1Controls[0]);
            GUI.DrawTexture(new Rect(box.x + 305, box.y + 170, 30, 32), player1Controls[1]);
            GUI.DrawTexture(new Rect(box.x + 305, box.y + 260, 30, 32), player1Controls[2]);
            GUI.DrawTexture(new Rect(box.x + 305, box.y + 340, 30, 32), player1Controls[3]);
            GUI.DrawTexture(new Rect(box.x + 630, box.y + 90, 30, 32), player2Controls[0]);
            GUI.DrawTexture(new Rect(box.x + 630, box.y + 170, 30, 32), player2Controls[1]);
            GUI.DrawTexture(new Rect(box.x + 630, box.y + 260, 30, 32), player2Controls[2]);
            GUI.DrawTexture(new Rect(box.x + 630, box.y + 340, 30, 32), player2Controls[3]);

            if (GUI.Button(new Rect(box.width / 2, box.height - 10, 150, 60), "Back"))
            {
                GUI.enabled = false;
                howToPlay = false;
                mainMenu.SetActive(true);
            }
        }

        if (gameOver)
        {
            //show game over screen
        }
    }
}