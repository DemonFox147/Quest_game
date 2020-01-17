using UnityEngine;

public class MiniGamesController : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public GameObject MiniGame;

    public Transform DialogAndNamePanelOriginal;
    public Transform DialogAndNamePanelTemporarily;
    
    public Transform CharactersOriginal;
    public Transform CharactersTemporarily;

    public int CurrentNumberGame;

    public void StartMiniGame(string nameMiniGame, int currentNumberGame = 0)
    {
        CurrentNumberGame = currentNumberGame;

        FindObjectOfType<BackdropText>().SetSkipMode(false);

        GameObject prefab;

        if (MiniGame == null)
        {
            prefab = Resources.Load<GameObject>("MiniGamesPrefabs/" + nameMiniGame);

            if (prefab != null)
            {
                GameObject g = Instantiate(prefab, transform);
                g.name = nameMiniGame;
                MiniGame = g;
                MiniGame.transform.SetAsFirstSibling();
                MiniGame.SetActive(true);
            }
            else
            {
                Debug.Log("<color=red>Мини игра : " + nameMiniGame + " не найдена</color>");
            }
        }
        else
        {
            switch (nameMiniGame)
            {
                case "PotionMaker":
                    {
                        srp.ScenarioParser.InterfacePanelSetActive(false);
                        StartCoroutine(MiniGame.GetComponent<PotionMaker>().IEContinueMiniGame());
                        break;
                    }
                case "ShadowGames":
                    {
                        srp.ScenarioParser.InterfacePanelSetActive(false);
                        MiniGame.GetComponent<ShadowGames>().ContinueMiniGame();
                        break;
                    }
            }
        }
    }

    public void EndMiniGame(string nameMiniGame, bool state)
    {
        FindObjectOfType<BackdropText>().SetSkipMode(false);

        srp.ScenarioParser.MiniGameMode = false;

        srp.ScenarioParser.InterfacePanelSetActive(true);

        if (state) { srp.ScenarioParser.GoNextChapter("chap_Win_MiniGame_" + nameMiniGame, true); }
        else { srp.ScenarioParser.GoNextChapter("chap_Lose_MiniGame_" + nameMiniGame, true); }

        if (MiniGame != null)
        {
            Destroy(MiniGame);
            MiniGame = null;
        }
    }

    public void SetDialogAndNamePanelMiniGame(bool reverse)
    {
        if (!reverse)
        {
            DialogAndNamePanelOriginal.GetChild(0).SetParent(DialogAndNamePanelTemporarily);
            DialogAndNamePanelOriginal.GetChild(0).SetParent(DialogAndNamePanelTemporarily);
        }
        else
        {
            DialogAndNamePanelTemporarily.GetChild(0).SetParent(DialogAndNamePanelOriginal);
            DialogAndNamePanelTemporarily.GetChild(0).SetParent(DialogAndNamePanelOriginal);
        }
    }

    public void SetCharactersMiniGame(bool reverse)
    {
        if (!reverse)
        {
            CharactersOriginal.GetChild(0).SetParent(CharactersTemporarily);
            CharactersOriginal.GetChild(0).SetParent(CharactersTemporarily);
        }
        else
        {
            CharactersTemporarily.GetChild(0).SetParent(CharactersOriginal);
            CharactersTemporarily.GetChild(0).SetParent(CharactersOriginal);
        }
    }

    public void MiniGameChapterRead(string chapter)
    {
        FindObjectOfType<BackdropText>().SetSkipMode(false);

        srp.ScenarioParser.MiniGameMode = false;

        srp.ScenarioParser.GoNextChapter(chapter, true);

        srp.ScenarioParser.InterfacePanelSetActive(true);
    }
}
