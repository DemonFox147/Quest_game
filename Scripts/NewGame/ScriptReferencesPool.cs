using UnityEngine;

public class ScriptReferencesPool : MonoBehaviour
{
    public AnimatorController AnimatorController
    {
        get;
        private set;
    }

    public Backgrounds Backgrounds
    {
        get;
        private set;
    }

    public Characters Characters
    {
        get;
        private set;
    }

    public DeathEnd DeathEnd
    {
        get;
        private set;
    }

    public GameEnd GameEnd
    {
        get;
        private set;
    }

    public Inventory Inventory
    {
        get;
        private set;
    }

    public MiniGamesController MiniGamesController
    {
        get;
        private set;
    }

    public MiniMenuController MiniMenuController
    {
        get;
        private set;
    }

    public MusicController MusicController
    {
        get;
        private set;
    }

    public NamesHeroes NamesHeroes
    {
        get;
        private set;
    }

    public NotesController NotesController
    {
        get;
        private set;
    }

    public ScenarioParser ScenarioParser
    {
        get;
        private set;
    }

    public SceneGameItemsController SceneGameItemsController
    {
        get;
        private set;
    }

    public SceneTransitionController SceneTransitionController
    {
        get;
        private set;
    }

    public SelectionController SelectionController
    {
        get;
        private set;
    }

    public TaskController TaskController
    {
        get;
        private set;
    }

    public TutorController TutorController
    {
        get;
        private set;
    }

    void Awake()
    {
        ScriptReferencesPool srp = GetComponent<ScriptReferencesPool>();

        AnimatorController = FindObjectOfType<AnimatorController>();
        AnimatorController.srp = srp;
        Backgrounds = FindObjectOfType<Backgrounds>();
        Characters = FindObjectOfType<Characters>();
        DeathEnd = FindObjectOfType<DeathEnd>();
        DeathEnd.srp = srp;
        GameEnd = FindObjectOfType<GameEnd>();
        GameEnd.srp = srp;
        Inventory = FindObjectOfType<Inventory>();
        MiniGamesController = FindObjectOfType<MiniGamesController>();
        MiniGamesController.srp = srp;
        MiniMenuController = FindObjectOfType<MiniMenuController>();
        MusicController = FindObjectOfType<MusicController>();
        NamesHeroes = FindObjectOfType<NamesHeroes>();
        NotesController = FindObjectOfType<NotesController>();
        ScenarioParser = FindObjectOfType<ScenarioParser>();
        ScenarioParser.srp = srp;
        SceneGameItemsController = FindObjectOfType<SceneGameItemsController>();
        SceneGameItemsController.srp = srp;
        SceneTransitionController = FindObjectOfType<SceneTransitionController>();
        SceneTransitionController.srp = srp;
        SelectionController = FindObjectOfType<SelectionController>();
        SelectionController.srp = srp;
        TaskController = FindObjectOfType<TaskController>();
        TaskController.srp = srp;
        TutorController = FindObjectOfType<TutorController>();
    }
}