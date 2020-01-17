using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class PotionMaker : MonoBehaviour
{
    private MiniGamesController miniGamesController;

    private int currentNumberGame;

    public bool miniGame;

    public bool timerActive;

    public int CurRoundNum;

    public int CurArrowNum;

    public int CountErrorsIngredients;

    public int CountIngredients;

    public int MaxCountErrorsIngredients;

    public string chapterRound;

    public string[] CurRoundDirArrow;

    public Text Timer;

    public Image CurIngredient;

    public Image[] Arrows;

    public GameObject SkipMiniGameButton;

    public GameObject[] Ingredients;

    public Animator Bubbles;

    public Animator TimerAnimation;

    private Dictionary<string, XmlNode> rounds;

    private ControllerSoundButton controllerSoundButton;

    private MusicController musicController;

    void Start()
    {
        controllerSoundButton = FindObjectOfType<ControllerSoundButton>();

        musicController = FindObjectOfType<MusicController>();

        miniGamesController = transform.parent.GetComponent<MiniGamesController>();
        miniGamesController.SetDialogAndNamePanelMiniGame(false);

        currentNumberGame = miniGamesController.CurrentNumberGame;

        miniGame = false;
        timerActive = false;

        CurRoundNum = 0;
        CurArrowNum = 0;
        CountErrorsIngredients = 0;
        MaxCountErrorsIngredients = 3;

        Timer = transform.GetChild(1).GetChild(1).GetComponent<Text>();
        CurIngredient = transform.GetChild(0).GetChild(3).GetComponent<Image>();

        Bubbles = transform.GetChild(0).GetChild(1).GetComponent<Animator>();

        TimerAnimation = transform.GetChild(0).GetChild(2).GetComponent<Animator>();

        SkipMiniGameButton = transform.GetChild(1).GetChild(5).gameObject;

        SkipMiniGameButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/MiniGames/PotionMaker/Sprites/Interface/Skip_" + Singleton<ToolBox>.Instance.GameLanguage);

        Transform arrows = transform.GetChild(1).GetChild(3);

        Arrows = new Image[arrows.childCount];

        for (int i = 0; i < Arrows.Length; i++)
        {
            Arrows[i] = arrows.GetChild(i).GetComponent<Image>();
        }

        Transform ingredients = transform.GetChild(1).GetChild(4);

        Ingredients = new GameObject[ingredients.childCount];

        for (int i = 0; i < Ingredients.Length; i++)
        {
            Ingredients[i] = ingredients.GetChild(i).gameObject;
        }

        CountIngredients = Ingredients.Length;

        TextAsset potionMaker = Resources.Load<TextAsset>("Text/PotionMaker");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(potionMaker.text);

        XmlNodeList ingredientsNodeList = xDoc.DocumentElement.ChildNodes.Item(currentNumberGame).ChildNodes.Item(1).ChildNodes;

        string nameIngredient;

        for (int i = 0; i < Ingredients.Length; i++)
        {
            nameIngredient = ingredientsNodeList.Item(i).Attributes["name"].Value;
            Ingredients[i].transform.GetChild(0).GetComponent<Image>().sprite = 
                Resources.Load<Sprite>("Images/Inventory/" + nameIngredient);
        }

        rounds = new Dictionary<string, XmlNode>();

        XmlNodeList roundsNodeList = xDoc.DocumentElement.ChildNodes.Item(currentNumberGame).ChildNodes.Item(0).ChildNodes;

        foreach (XmlNode round in roundsNodeList)
        {
            rounds.Add(round.Name, round);
        }

        if (Singleton<GameState>.Instance.miniGame != "PotionMaker")
        {
            Singleton<GameState>.Instance.miniGame = "PotionMaker";
        }
        else
        {
            SkipMiniGameButton.SetActive(true);
        }

        musicController.LoadSound("BoilingCauldron");

        StartCoroutine(IEContinueMiniGame());
    }

    void Update()
    {
        if (miniGame && timerActive)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (CurRoundDirArrow[CurArrowNum] == "Left") { NextArrow(true); }
                else { NextArrow(false); }
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (CurRoundDirArrow[CurArrowNum] == "Right") { NextArrow(true); }
                else { NextArrow(false); }
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (CurRoundDirArrow[CurArrowNum] == "Up") { NextArrow(true); }
                else { NextArrow(false); }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (CurRoundDirArrow[CurArrowNum] == "Down") { NextArrow(true); }
                else { NextArrow(false); }
            }
        }
    }

    private IEnumerator IEShowGameElements(Transform t)
    {
        CanvasGroup pt = t.GetComponent<CanvasGroup>();

        for (float f = 0; f < 1; f += 0.02f)
        {
            pt.alpha = f;
            yield return new WaitForFixedUpdate();
        }

        pt.alpha = 1f;
    }

    private IEnumerator IEHideGameElements(Transform t)
    {
        CanvasGroup cg = t.GetComponent<CanvasGroup>();

        for (float f = 1; f > 0; f -= 0.02f)
        {
            cg.alpha = f;
            yield return new WaitForFixedUpdate();
        }

        cg.alpha = 0f;
    }

    private void StartRound()
    {
        miniGame = true;
        timerActive = true;

        CurArrowNum = 0;

        XmlNode currentRound = rounds["Round_" + CurRoundNum];

        int time = int.Parse(currentRound.Attributes["time"].Value);

        StartCoroutine(IEStartTimer(time));

        CurIngredient.sprite = Ingredients[CurRoundNum].transform.GetChild(0).GetComponent<Image>().sprite;

        XmlNodeList arrowsNodeList = currentRound.FirstChild.ChildNodes;

        int countArrow = arrowsNodeList.Count;

        CurRoundDirArrow = new string[countArrow];

        for (int i = 0; i < countArrow; i++)
        {
            XmlNode arrow = arrowsNodeList.Item(i);

            CurRoundDirArrow[i] = arrow.Attributes["dir"].Value;

            Arrows[i].sprite =
                Resources.Load<Sprite>("Images/MiniGames/PotionMaker/Sprites/Arrows/Arrow" + CurRoundDirArrow[i] + "_Norm");

            float xMin = float.Parse(arrow.Attributes["xMin"].Value);
            float xMax = float.Parse(arrow.Attributes["xMax"].Value);
            float yMin = float.Parse(arrow.Attributes["yMin"].Value);
            float yMax = float.Parse(arrow.Attributes["yMax"].Value);

            Arrows[i].GetComponent<RectTransform>().anchorMin = new Vector2(xMin, yMin);
            Arrows[i].GetComponent<RectTransform>().anchorMax = new Vector2(xMax, yMax);
        }

        Arrows[0].sprite =
            Resources.Load<Sprite>("Images/MiniGames/PotionMaker/Sprites/Arrows/Arrow" + CurRoundDirArrow[0] + "_Light");
    }

    public IEnumerator IEContinueMiniGame()
    {
        TimerAnimation.SetBool("Start", true);
        yield return new WaitForSeconds(2f);
        TimerAnimation.SetBool("Start", false);
        yield return new WaitForSeconds(2f);

        StartRound();
        StartCoroutine(IEShowIngredient());
        StartCoroutine(IEShowGameElements(transform.GetChild(1)));
    }

    private void NextArrow(bool state)
    {
        controllerSoundButton.PlaySoundButtonMiniGames("Arrow_" + (CurArrowNum + 1).ToString(), "PotionMaker");

        if (state)
        {
            if (CurArrowNum + 1 >= CurRoundNum + 5)
            {
                Ingredients[CurRoundNum].transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

                CurRoundNum++;

                controllerSoundButton.PlaySoundButtonMiniGames("RoundWin", "PotionMaker");

                if (CurRoundNum >= CountIngredients)
                {
                    StartCoroutine(IEEndGame(true));
                }
                else
                {
                    StartCoroutine(IEHideIngredient());
                    StartCoroutine(IEHideGameElements(transform.GetChild(1)));
                    miniGamesController.MiniGameChapterRead("chap_MiniGame_PotionMaker_Round_" + CurRoundNum);
                }
            }
            else
            {
                Arrows[CurArrowNum].sprite =
                    Resources.Load<Sprite>("Images/MiniGames/PotionMaker/Sprites/Arrows/Arrow" + CurRoundDirArrow[CurArrowNum] + "_Norm");

                CurArrowNum++;

                Arrows[CurArrowNum].sprite =
                    Resources.Load<Sprite>("Images/MiniGames/PotionMaker/Sprites/Arrows/Arrow" + CurRoundDirArrow[CurArrowNum] + "_Light");
            }
        }
        else
        {
            Arrows[CurArrowNum].sprite =
                Resources.Load<Sprite>("Images/MiniGames/PotionMaker/Sprites/Arrows/Arrow" + CurRoundDirArrow[CurArrowNum] + "_Miss");

            Ingredients[CurRoundNum].transform.GetChild(1).gameObject.SetActive(true);

            CountErrorsIngredients++;
            CurRoundNum++;

            controllerSoundButton.PlaySoundButtonMiniGames("RoundLose", "PotionMaker");

            if (CountErrorsIngredients >= MaxCountErrorsIngredients)
            {
                StartCoroutine(IEEndGame(false));
            }
            else if (CurRoundNum >= CountIngredients)
            {
                StartCoroutine(IEEndGame(true));
            }
            else
            {
                StartCoroutine(IEHideIngredient());
                StartCoroutine(IEHideGameElements(transform.GetChild(1)));
                miniGamesController.MiniGameChapterRead("chap_MiniGame_PotionMaker_Round_-" + CurRoundNum);
            }
        }
    }

    private IEnumerator IEHideIngredient()
    {
        miniGame = false;
        timerActive = false;

        Color color = CurIngredient.color;

        for (float f = 1; f > 0; f -= 0.04f)
        {
            color.a = f;
            CurIngredient.color = color;
            yield return new WaitForFixedUpdate();
        }

        color.a = 0;
        CurIngredient.color = color;
    }

    private IEnumerator IEShowIngredient()
    {
        Color color = CurIngredient.color;

        for (float f = 0; f < 1; f += 0.04f)
        {
            color.a = f;
            CurIngredient.color = color;
            yield return new WaitForFixedUpdate();
        }

        color.a = 1;
        CurIngredient.color = color;
    }

    private IEnumerator IEStartTimer(int time)
    {
        int curRoundNum = CurRoundNum;

        Timer.text = time.ToString();

        Timer.color = new Color(255, 212, 0, 255);

        yield return new WaitForSecondsRealtime(1f);

        do
        {
            time--;
            Timer.text = time.ToString();

            if (time <= MaxCountErrorsIngredients)
            {
                Timer.color = Color.red;
            }

            yield return new WaitForSecondsRealtime(1f);
        }
        while (time > 0 && curRoundNum == CurRoundNum && timerActive);

        if (time == 0 && timerActive)
        {
            Ingredients[CurRoundNum].transform.GetChild(1).gameObject.SetActive(true);

            CountErrorsIngredients++;
            CurRoundNum++;

            if (CountErrorsIngredients >= MaxCountErrorsIngredients)
            {
                StartCoroutine(IEEndGame(false));
            }
            else if (CurRoundNum >= CountIngredients)
            {
                StartCoroutine(IEEndGame(true));
            }
            else
            {
                StartCoroutine(IEHideIngredient());
                StartCoroutine(IEHideGameElements(transform.GetChild(1)));
                miniGamesController.MiniGameChapterRead("chap_MiniGame_PotionMaker_Round_-" + CurRoundNum);
            }
        }
    }

    private IEnumerator IEEndGame(bool state)
    {
        miniGame = false;

        miniGamesController.SetDialogAndNamePanelMiniGame(true);

        if (state)
        {
            controllerSoundButton.PlaySoundButtonMiniGames("Win", "PotionMaker");
        }
        else
        {
            controllerSoundButton.PlaySoundButtonMiniGames("Lose", "PotionMaker");
        }

        musicController.StopSound();

        miniGamesController.EndMiniGame("PotionMaker", state);

        CanvasGroup pt = GetComponent<CanvasGroup>();

        for (float f = 1; f > 0; f -= 0.01f)
        {
            pt.alpha = f;
            yield return new WaitForFixedUpdate();
        }

        pt.alpha = 0f;
    }

    public void SkipMiniGame()
    {   
        Singleton<GameState>.Instance.miniGame = "";

        StartCoroutine(IEEndGame(true));
    }
}