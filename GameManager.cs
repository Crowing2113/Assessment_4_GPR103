using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //TODO:
    //Fix spawning - make sure enemies don't spawn in the same place
    //Make new sprites for the rest of the characters


    //public variables
    public int enemiesSpawned = 0;
    public int needToBeat;//how many enemies need to be beaten to pass this wave
    public int defeatedEnemies;//how many enemies have been defeated
    public static int playerCount;
    public static int score = 0;

    public GameObject ninja, knight, samurai; //player characters
    public GameObject sp1, sp2;//spawn points for player 1 and 2
    public GameObject mainMenu, charSelect, highScoreScreen, gameOverScreen, pauseScreen;//screen references
    public GameObject askQuit; //reference to the asking to quit pop up
    public GameObject stage; //reference to the stage
    public GameObject eSpawn;//enemy spawn point - this will have a reference to a parent gameObject that holds all the enemy spawn points
    public GameObject[] enemies;//array of enemies
    public GameObject p1Container, p2Container, scoreContainer; //conainters for the p1 and p2 lives display . and container for the score display
    public GameObject p1HealthBar, p2HealthBar;
    public Sprite boxTex;
    public Sprite pOneInd, pTwoInd; //player indicators for who is who for when players pick the same character
    public Texture[] player1Controls, player2Controls;
    public Text p1Lives, p2Lives, p1LivesDisp, p2LivesDisp, scoreDisp, highScoreText, highScoreNames, gameOverScore;
    public InputField highScoreNameField;
    public List<GameObject> enemiesInLevel; //a list of enemeis in the level, this used for clean up
    List<int> highScores = new List<int>();
    List<string> highScoresNames = new List<string>();
    //private variables

    playerController p1, p2;//playerController reference for both players
    bool oneP;//for checking how many players. true = one player, false = two players
    bool onePicking = false, twoPicking = false;//for checking which player picked a character
    bool howToPlay = false;
    bool paused = false;
    //bool quitCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        
        
        //setting everything active and inactive jsut to make sure
        pauseScreen.SetActive(false);
        askQuit.SetActive(false);
        stage.SetActive(false);
        mainMenu.SetActive(true);
        highScoreScreen.SetActive(false);
        charSelect.SetActive(false);
        p1HealthBar.SetActive(false);
        p2HealthBar.SetActive(false);
        scoreContainer.SetActive(false);
        p1Container.SetActive(false);
        p2Container.SetActive(false);
        for (int i = 0; i < 10; i++)
        {
            if (PlayerPrefs.HasKey("highScore_" + i) && PlayerPrefs.HasKey("highScoreName_" + i))
            {
                highScores.Add(PlayerPrefs.GetInt("highScore_" + i));
                highScoresNames.Add(PlayerPrefs.GetString("highScoreName_" + i));
                print("highScores[" + i + "]" + highScores[i] + "highScoresNames[" + i + "]" + highScoresNames[i]);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        //only run this if the stage is active
        if (stage.activeSelf)
        {
            scoreDisp.text = score.ToString();//to display score and keep it updated every frame
            p1Lives.text = p1.lives.ToString();//to display player 1 lives and keep it updated
            p1HealthBar.GetComponent<Image>().fillAmount = (float)p1.hp / p1.startHp;//to display player 1 health and keep it updated
            if (!oneP)
            { //if it's 2 player then also display player 2 lives and health
                p2HealthBar.GetComponent<Image>().fillAmount = (float)p2.hp / p2.startHp;
                p2Lives.text = p2.lives.ToString();
            }
            if (playerCount <= 0)
            {
                gameOverScreen.SetActive(true);
                Time.timeScale = 0;

            }
            if (enemiesSpawned < playerCount * 2 && defeatedEnemies < needToBeat) //spawn enemies whenever there are less than this number or the max defeated enemies has been reached for this wave
                SpawnEnemy();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (paused)
                    ContinueButton();
                else
                    PauseGame();
            }
        }
        if (gameOverScreen.activeSelf)
        {
            gameOverScreen.SetActive(true);
            gameOverScore.text = score.ToString();
        }
    }
    //sort through a list of highscores to have them in ascending order
    public void SortHighScore()
    {
        bool sorted = false;
        while (!sorted)
        {
            sorted = true;
            //starting at 1 so we can compare if the previous entry
            for (int i = 1; i < highScores.Count; i++)
            {
                //if the current score is less than the previous one then swap
                if (highScores[i] > highScores[i - 1])
                {
                    int temp = highScores[i];
                    string tempName = highScoresNames[i];
                    highScores[i] = highScores[i - 1];
                    highScoresNames[i] = highScoresNames[i - 1];
                    highScores[i - 1] = temp;
                    highScoresNames[i - 1] = tempName;
                    sorted = false;
                }
            }
        }
    }
    public void HowToPlay()
    {
        howToPlay = true;
    }

    //function for when player selects 1 or 2 players then switched menu screens
    public void PlayerSelect(bool singlePlay)
    {
        oneP = singlePlay;
        mainMenu.SetActive(false);
        charSelect.SetActive(true);
        onePicking = true;
        charSelect.transform.Find("Instructions").GetComponent<Text>().text = "Player 1 select a Character";
    }
    public void EnterHighScoreName()
    {

        highScoresNames.Add(highScoreNameField.text);
        highScores.Add(score);
        for (int i = 0; i < highScores.Count; i++)
        {
            PlayerPrefs.SetInt("highScore_" + i, highScores[i]);
            PlayerPrefs.SetString("highScoreName_" + i, highScoresNames[i]);
        }
        ClearLevel();
        CleanupUI();
        SortHighScore();
        print("GAME IS DONE");
        PlayerPrefs.Save();
        
        stage.SetActive(false);
        gameOverScreen.SetActive(false);
        mainMenu.SetActive(true);
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
    //for when the player selects a character
    public void PickCharacter(string sChar)
    {
        charSelect.transform.Find("Instructions").GetComponent<Text>().text = "Player 1 select a Character";
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
                onePicking = false;
                StartGame();
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
            StartGame();
        }

    }
    //for spawning enemies to the stage
    void SpawnEnemy()
    {
        int i = Random.Range(0, enemies.Length);//get a random value where min is 0 and max is the size of enemies-1 (since arrays start at 0)
        Transform[] sArray = eSpawn.GetComponentsInChildren<Transform>();//array of spawn points that will be put into a list
        int j = Random.Range(0, sArray.Length - 2) + 1;
        print("sArray length = " + sArray.Length);
        enemiesInLevel.Add(Instantiate(enemies[i], sArray[j]));
        enemiesSpawned++;
    }
    void StartGame()
    {
        charSelect.SetActive(false);
        stage.SetActive(true);
        gameOverScreen.SetActive(false);
        //setting all the UI elements to be visible
        p1Container.SetActive(true);
        p1HealthBar.SetActive(true);
        if (!oneP)
        {
            p2Container.SetActive(true);
            p2HealthBar.SetActive(true);
        }
        scoreContainer.SetActive(true);
    }
    //clear the UI
    void CleanupUI()
    {
        if (p1HealthBar.activeSelf)
            p1HealthBar.SetActive(false);
        if (p1Container.activeSelf)
            p1Container.SetActive(false);
        if (p2HealthBar.activeSelf)
            p2HealthBar.SetActive(false);
        if (p2Container.activeSelf)
            p2Container.SetActive(false);
        if (scoreContainer.activeSelf)
            scoreContainer.SetActive(false);
    }
    //Clearing level
    void ClearLevel()
    {
        //cleaing all the enemies in the level
        foreach (GameObject go in enemiesInLevel)
            Destroy(go);
        //clearing the eneminesInLevel array
        enemiesInLevel.Clear();
        if (p1 != null)
            Destroy(p1.gameObject);
        if (p2 != null)
            Destroy(p2.gameObject);
        
    }
    public void HighScore()
    {
        highScoreScreen.SetActive(true);
        mainMenu.SetActive(false);
        for (int i = 0; i < highScores.Count; i++)
        {
            highScoreText.text += highScores[i].ToString() + "\n";
            highScoreNames.text += highScoresNames[i] + "\n";
        }
    }
    public void PauseGame()
    {
        paused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0;
    }

    public void ContinueButton()
    {
        paused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1;

    }
    public void CheckIfQuit()
    {
        askQuit.SetActive(true);
    }
    //When the player hits yes when being asked to quit
    public void YesQuit()
    {
        askQuit.SetActive(false);
        BackToMenu();
    }
    //When the player hits no when being asked to quit
    public void NoQuit()
    {
        askQuit.SetActive(false);
    }
    //going back to menu from level or other screens
    public void BackToMenu()
    {
        if (pauseScreen.activeSelf)
        {
            pauseScreen.SetActive(false);
            stage.SetActive(false);
            ClearLevel();
            CleanupUI();
            Time.timeScale = 1;
        }
        mainMenu.SetActive(true);
        highScoreScreen.SetActive(false);
        charSelect.SetActive(false);
    }
    public void quitGame()
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
        //drawing the how to play screen
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
    }
}