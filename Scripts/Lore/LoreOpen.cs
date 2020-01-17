using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

public class LoreOpen : MonoBehaviour
{
    public Dictionary<string, Dictionary<string, XmlNode>> LoreDictionaryXML;

    public Dictionary<string, List<string>> CharactersDictionatyActive;
    public Dictionary<string, List<string>> CreaturesDictionatyActive;
    public Dictionary<string, List<string>> WorldDictionatyActive;

    void Start()
    {
        UpdateLoreLang();
    }

    public void UpdateLoreLang()
    {
        LoreDictionaryXML = Singleton<ToolBox>.Instance.LoreDictionaryXML;

        CharactersDictionatyActive = new Dictionary<string, List<string>>();
        CreaturesDictionatyActive = new Dictionary<string, List<string>>();
        WorldDictionatyActive = new Dictionary<string, List<string>>();

        if (PlayerPrefs.GetString("saveBW_openedLoreCharacters") == "Zero")
        {
            string text = string.Empty;

            string[] keys = LoreDictionaryXML["Characters"].Select(kv => kv.Key).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                if (i != keys.Length - 1)
                {
                    text += keys[i] + "^" + "-1|";
                }
                else
                {
                    text += keys[i] + "^" + "-1";
                }

                CharactersDictionatyActive.Add(keys[i], new List<string> { "-1" });
            }

            PlayerPrefs.SetString("saveBW_openedLoreCharacters", text);
        }
        else
        {
            string[] charactersValue = PlayerPrefs.GetString("saveBW_openedLoreCharacters").Split('|');

            for (int i = 0; i < charactersValue.Length; i++)
            {
                int indexOf = charactersValue[i].IndexOf('^');

                string character = charactersValue[i].Substring(0, indexOf);
                List<string> unlock = charactersValue[i].Substring(indexOf + 1, charactersValue[i].Length - 1 - indexOf).Split(',').ToList();

                CharactersDictionatyActive.Add(character, unlock);
            }
        }

        if (PlayerPrefs.GetString("saveBW_openedLoreCreatures") == "Zero")
        {
            string text = string.Empty;

            string[] keys = LoreDictionaryXML["Creatures"].Select(kv => kv.Key).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                if (i != keys.Length - 1)
                {
                    text += keys[i] + "^" + "-1|";
                }
                else
                {
                    text += keys[i] + "^" + "-1";
                }

                CreaturesDictionatyActive.Add(keys[i], new List<string> { "-1" });
            }

            PlayerPrefs.SetString("saveBW_openedLoreCreatures", text);
        }
        else
        {
            string[] CreaturesValue = PlayerPrefs.GetString("saveBW_openedLoreCreatures").Split('|');

            for (int i = 0; i < CreaturesValue.Length; i++)
            {
                int indexOf = CreaturesValue[i].IndexOf('^');

                string monster = CreaturesValue[i].Substring(0, indexOf);
                List<string> unlock = CreaturesValue[i].Substring(indexOf + 1, CreaturesValue[i].Length - 1 - indexOf).Split(',').ToList();

                CreaturesDictionatyActive.Add(monster, unlock);
            }
        }

        if (PlayerPrefs.GetString("saveBW_openedLoreWorld") == "Zero")
        {
            string text = string.Empty;

            string[] keys = LoreDictionaryXML["World"].Select(kv => kv.Key).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                if (i != keys.Length - 1)
                {
                    text += keys[i] + "^" + "-1|";
                }
                else
                {
                    text += keys[i] + "^" + "-1";
                }

                WorldDictionatyActive.Add(keys[i], new List<string> { "-1" });
            }

            PlayerPrefs.SetString("saveBW_openedLoreWorld", text);
        }
        else
        {
            string[] worldValue = PlayerPrefs.GetString("saveBW_openedLoreWorld").Split('|');

            for (int i = 0; i < worldValue.Length; i++)
            {
                int indexOf = worldValue[i].IndexOf('^');

                string world = worldValue[i].Substring(0, indexOf);
                List<string> unlock = worldValue[i].Substring(indexOf + 1, worldValue[i].Length - 1 - indexOf).Split(',').ToList();

                WorldDictionatyActive.Add(world, unlock);
            }
        }
    }

    public void UnlockLore(string name, string category, string unlockLevel)
    {
        if (category == "Characters")
        {
            if (!CharactersDictionatyActive.ContainsKey(name))
            {
                Debug.Log("В Лоре в категории Characters имя " + name + " для изменения не найдено");
                return;
            }

            if (CharactersDictionatyActive[name].Contains<string>("-1"))
            {
                CharactersDictionatyActive[name].Remove("-1");
            }

            if (!CharactersDictionatyActive[name].Contains<string>(unlockLevel))
            {
                CharactersDictionatyActive[name].Add(unlockLevel);
                SaveLoreData(category);
                StartCoroutine(FindObjectOfType<Inventory>().IEShowLore());

                string loreNewInfo = "";

                if (PlayerPrefs.GetString("saveBW_LoreNewInfo") == "")
                {
                    loreNewInfo = name;
                }
                else
                {
                    loreNewInfo = PlayerPrefs.GetString("saveBW_LoreNewInfo") + "," + name;
                }

                PlayerPrefs.SetString("saveBW_LoreNewInfo", loreNewInfo);
            }
            else
            {
                Debug.Log("В категории: " + category + " имя: " + name + " уровень доступа: " + unlockLevel + " уже открыт.");
            }
        }
        else if (category == "Creatures")
        {
            if (!CreaturesDictionatyActive.ContainsKey(name))
            {
                Debug.Log("В Лоре в категории Creatures имя " + name + " для изменения не найдено");
                return;
            }

            if (CreaturesDictionatyActive[name].Contains<string>("-1"))
            {
                CreaturesDictionatyActive[name].Remove("-1");
            }

            if (!CreaturesDictionatyActive[name].Contains<string>(unlockLevel))
            {
                CreaturesDictionatyActive[name].Add(unlockLevel);
                SaveLoreData(category);
                StartCoroutine(FindObjectOfType<Inventory>().IEShowLore());

                string loreNewInfo = "";

                if (PlayerPrefs.GetString("saveBW_LoreNewInfo") == "")
                {
                    loreNewInfo = name;
                }
                else
                {
                    loreNewInfo = PlayerPrefs.GetString("saveBW_LoreNewInfo") + "," + name;
                }

                PlayerPrefs.SetString("saveBW_LoreNewInfo", loreNewInfo);
            }
            else
            {
                Debug.Log("В категории: " + category + " имя: " + name + " уровень доступа: " + unlockLevel + " уже открыт.");
            }
        }
        else if (category == "World")
        {
            if (!WorldDictionatyActive.ContainsKey(name))
            {
                Debug.Log("В Лоре в категории World имя " + name + " для изменения не найдено");
                return;
            }

            if (WorldDictionatyActive[name].Contains<string>("-1"))
            {
                WorldDictionatyActive[name].Remove("-1");
            }

            if (!WorldDictionatyActive[name].Contains<string>(unlockLevel))
            {
                WorldDictionatyActive[name].Add(unlockLevel);
                SaveLoreData(category);
                StartCoroutine(FindObjectOfType<Inventory>().IEShowLore());

                string loreNewInfo = "";

                if (PlayerPrefs.GetString("saveBW_LoreNewInfo") == "")
                {
                    loreNewInfo = name;
                }
                else
                {
                    loreNewInfo = PlayerPrefs.GetString("saveBW_LoreNewInfo") + "," + name;
                }

                PlayerPrefs.SetString("saveBW_LoreNewInfo", loreNewInfo);
            }
            else
            {
                Debug.Log("В категории: " + category + " имя: " + name + " уровень доступа: " + unlockLevel + " уже открыт.");
            }
        }
        else
        {
            Debug.Log("Категория " + category + " в Лоре отсутствует.");
            return;
        }
    }

    private void SaveLoreData(string category)
    {
        if (category == "Characters")
        {
            string text = string.Empty;

            string[] keys = CharactersDictionatyActive.Select(kv => kv.Key).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                string value = string.Join(",", CharactersDictionatyActive[keys[i]].ToArray());

                if (i != keys.Length - 1)
                {
                    text += keys[i] + "^" + value + "|";
                }
                else
                {
                    text += keys[i] + "^" + value;
                }
            }

            PlayerPrefs.SetString("saveBW_openedLoreCharacters", text);
        }
        else if (category == "Creatures")
        {
            string text = string.Empty;

            string[] keys = CreaturesDictionatyActive.Select(kv => kv.Key).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                string value = string.Join(",", CreaturesDictionatyActive[keys[i]].ToArray());

                if (i != keys.Length - 1)
                {
                    text += keys[i] + "^" + value + "|";
                }
                else
                {
                    text += keys[i] + "^" + value;
                }
            }

            PlayerPrefs.SetString("saveBW_openedLoreCreatures", text);
        }
        else if (category == "World")
        {
            string text = string.Empty;

            string[] keys = WorldDictionatyActive.Select(kv => kv.Key).ToArray();

            for (int i = 0; i < keys.Length; i++)
            {
                string value = string.Join(",", WorldDictionatyActive[keys[i]].ToArray());

                if (i != keys.Length - 1)
                {
                    text += keys[i] + "^" + value + "|";
                }
                else
                {
                    text += keys[i] + "^" + value;
                }
            }

            PlayerPrefs.SetString("saveBW_openedLoreWorld", text);
        }
    }
}