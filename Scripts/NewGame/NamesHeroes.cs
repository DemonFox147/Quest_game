using UnityEngine;
using UnityEngine.UI;

public class NamesHeroes : MonoBehaviour
{
    private Image nameHero;

    void Awake()
    {
        nameHero = GetComponent<Image>();
        CurSpriteNameHero(Singleton<ToolBox>.Instance.GameState.nameHero);
    }

    public void CurSpriteNameHero(string name)
    {
        if (name == "Zero")
        {
            nameHero.sprite = Resources.Load<Sprite>("Images/NamesHeroes/Zero");
            Singleton<ToolBox>.Instance.GameState.nameHero = name;
            return;
        }

        Sprite sprite = Resources.Load<Sprite>("Images/NamesHeroes/" +
            Singleton<ToolBox>.Instance.GameLanguage + "/" + name);

        if (sprite == null)
        {
            Debug.Log("<color=red>Имя персонажа " + name + " не найдено</color>");
        }
        else
        {
            nameHero.sprite = sprite;
            Singleton<ToolBox>.Instance.GameState.nameHero = name;
        }
    }
}