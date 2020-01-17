using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class ShadowGames : MonoBehaviour
{
    private MiniGamesController miniGamesController;

    public bool miniGame;

    public int CurRoundNum;
    
    public int CountCorrectAnswer;

    public string[] Answers;

    public Image CurShadow;

    public Image[] AnswerOptions;

    private Dictionary<string, XmlNode> rounds;

    void Start()
    {
        miniGame = false;

        miniGamesController = transform.parent.GetComponent<MiniGamesController>();
        miniGamesController.SetDialogAndNamePanelMiniGame(false);
        miniGamesController.SetCharactersMiniGame(false);

        Transform answerOptions = transform.GetChild(1).GetChild(0);

        AnswerOptions = new Image[answerOptions.childCount];
        
        for(int i = 0; i < AnswerOptions.Length; i++)
        {
            AnswerOptions[i] = answerOptions.GetChild(i).GetComponent<Image>();
        }

        CurShadow = transform.GetChild(1).GetChild(1).GetComponent<Image>();

        TextAsset shadowGames = Resources.Load<TextAsset>("Text/ShadowGames");
        XmlDocument xDoc = new XmlDocument();
        xDoc.LoadXml(shadowGames.text);
        XmlElement root = xDoc.DocumentElement;

        XmlNodeList answers = root.ChildNodes.Item(0).LastChild.ChildNodes;

        Answers = new string[answers.Count];

        for (int i = 0; i < Answers.Length; i++)
        {
            Answers[i] = answers.Item(i).Attributes["name"].Value;
        }

        rounds = new Dictionary<string, XmlNode>();

        XmlNodeList roundsNodeList = root.ChildNodes.Item(0).FirstChild.ChildNodes;

        foreach (XmlNode round in roundsNodeList)
        {
            rounds.Add(round.Name, round);
        }

        StartCoroutine(IEShowGameElements(transform));

        StartRound();

        StartCoroutine(IEShowGameElements(transform.GetChild(1), 2f));
    }

    private IEnumerator IEShowGameElements(Transform t, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup pt = t.GetComponent<CanvasGroup>();

        for (float f = 0; f < 1; f += 0.05f)
        {
            pt.alpha = f;
            yield return new WaitForFixedUpdate();
        }

        pt.alpha = 1f;
    }

    private IEnumerator IEHideGameElements(Transform t, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);

        CanvasGroup cg = t.GetComponent<CanvasGroup>();

        for (float f = 1; f > 0; f -= 0.05f)
        {
            cg.alpha = f;
            yield return new WaitForFixedUpdate();
        }

        cg.alpha = 0f;
    }

    private void StartRound()
    {
        CurShadow.sprite = Resources.Load<Sprite>("Images/MiniGames/ShadowGames/Sprites/Shadows/" + Answers[CurRoundNum]);

        XmlNode currentRound = rounds["Round_" + CurRoundNum];

        XmlNodeList answerOprionNodeList = currentRound.ChildNodes;

        for (int i = 0; i < answerOprionNodeList.Count; i++)
        {
            string name = answerOprionNodeList.Item(i).Attributes["name"].Value;

            string lang = Singleton<ToolBox>.Instance.GameLanguage;

            AnswerOptions[i].sprite = Resources.Load<Sprite>("Images/MiniGames/ShadowGames/Sprites/АnswerOptions/" + lang + "/" + name);
        }

        miniGame = true;
    }

    public void ContinueMiniGame()
    {
        StartRound();
        StartCoroutine(IEShowGameElements(transform.GetChild(1)));
    }

    public void NextRound(int numAnswerOption)
    {
        if (!miniGame || transform.GetChild(1).GetComponent<CanvasGroup>().alpha != 1f)
        {
            return;
        }

        if (Answers[CurRoundNum] == AnswerOptions[numAnswerOption - 1].sprite.name)
        {
            CountCorrectAnswer++;
        }

        CurRoundNum++;

        if (CurRoundNum >= 4)
        {
            StartCoroutine(IEEndGame());
        }
        else
        {
            miniGame = false;
            StartCoroutine(IEHideGameElements(transform.GetChild(1)));
            miniGamesController.MiniGameChapterRead("chap_MiniGame_ShadowGames_Round_" + CurRoundNum + "_Answer_" + numAnswerOption);
        }
    }

    private IEnumerator IEEndGame()
    {
        miniGame = false;

        miniGamesController.SetDialogAndNamePanelMiniGame(true);
        miniGamesController.SetCharactersMiniGame(true);

        if (CountCorrectAnswer != 1)
        {
            miniGamesController.EndMiniGame("ShadowGames_" + CountCorrectAnswer, true);
        }
        else
        {
            miniGamesController.EndMiniGame("ShadowGames_" + CountCorrectAnswer, false);
        }


        CanvasGroup pt = GetComponent<CanvasGroup>();

        for (float f = 1; f > 0; f -= 0.02f)
        {
            pt.alpha = f;
            yield return new WaitForFixedUpdate();
        }

        pt.alpha = 0f;
    }
}