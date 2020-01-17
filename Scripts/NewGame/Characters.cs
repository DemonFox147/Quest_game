using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class Characters : MonoBehaviour
{
    public CharactersTypes charactersTypes;

    [Serializable]
    public struct CharactersTypes
    {
        public Character[] NormCharacter;
        public Character[] BigCharacter;
        public Character[] IFCharacter;
        public Character[] NormHatCharacter;
        public Character[] NormShadowCharacter;
    }

    [Serializable]
    public struct Character
    {
        public string NameLayer;
        public Image SecondLayer;
        public Image FirstLayer;
    }

    [Space]
    public Transform IFCharacters;

    private Dictionary<string, Character[]> charTypesDict;

    private Dictionary<string, Dictionary<string, XmlNode>> filtersForCharactersByScenes;

    void Start()
    {
        charTypesDict = new Dictionary<string, Character[]>
        {
            { "N",   charactersTypes.NormCharacter },
            { "B",   charactersTypes.BigCharacter },
            { "IF",  charactersTypes.IFCharacter },
            { "NH",  charactersTypes.NormHatCharacter },
            { "NSh", charactersTypes.NormShadowCharacter }
        };

        filtersForCharactersByScenes = Singleton<ToolBox>.Instance.FiltersForCharactersByScenesXML;

        for (int i = 0; i < charTypesDict.Count; i++)
        {
            string type = charTypesDict.ElementAt(i).Key;

            for (int j = 0; j < Singleton<GameState>.Instance.charTypesDict[type].Length; j++)
            {
                ApplyCharacterToTheScene(type, Singleton<GameState>.Instance.charTypesDict[type][j], j);
            }
        }
    }

    public void ApplyCharacterToTheScene(string type, string nameChar, int pos)
    {
        if (nameChar == "" || nameChar == "Zero" || nameChar == null) { return; }

        string[] charDataNew = nameChar.Split('_');
        Sprite currentSprite = Resources.Load<Sprite>("Images/Characters/" + type + "Sprites/" + charDataNew[1] + "/" + nameChar);

        if (currentSprite == null) { Debug.Log("Персонаж " + type + " " + nameChar + " не найден"); return; }

        charTypesDict[type][pos].SecondLayer.sprite = currentSprite;
        charTypesDict[type][pos].SecondLayer.color = new Color(1, 1, 1, 1);
        FilterForCharacterToTheScene(type, nameChar, pos);

        Singleton<GameState>.Instance.charTypesDict[type][pos] = nameChar;
    }

    public void AddCharacterToTheScene(string type, string nameChar, int pos)
    {
        if (nameChar == "" || nameChar == "Zero" || nameChar == null) { return; }

        string[] charDataNew = nameChar.Split('_');

        Sprite currentSprite = Resources.Load<Sprite>("Images/Characters/" + type + "Sprites/" + charDataNew[1] + "/" + nameChar);

        if (currentSprite == null) { Debug.Log("Персонаж " + type + " " + nameChar + " не найден"); return; }

        string[] charDataOld = charTypesDict[type][pos].SecondLayer.sprite.name.Split('_');
        Singleton<GameState>.Instance.charTypesDict[type][pos] = nameChar;

        if (charTypesDict[type][pos].SecondLayer.color.a == 0)
        {
            StartCoroutine(IEShowCharacter(currentSprite, type, pos));
        }
        else if (charDataNew[0] == charDataOld[0] && charDataNew[1] == charDataOld[1])
        {
            StartCoroutine(IEHideAndShowEmotion(currentSprite, type, pos));
        }
        else if (charDataNew[0] == charDataOld[0] && charDataNew[1] != charDataOld[1])
        {
            StartCoroutine(IEHideAndShowCharacter(currentSprite, type, pos));
        }
        else if (charDataNew[0] != charDataOld[0] && charDataNew[1] == charDataOld[1])
        {
            ApplyCharacterToTheScene(type, nameChar, pos);
        }
        else if (charDataNew[0] != charDataOld[0] && charDataNew[1] != charDataOld[1])
        {
            StartCoroutine(IEHideAndShowCharacter(currentSprite, type, pos));
        }
    }

    public void FilterForCharacterToTheScene(string type, string nameChar, int pos)
    {
        if (nameChar == "" || nameChar == "Zero" || nameChar == null) { return; }

        string[] charDataNew = nameChar.Split('_');

        string bg = Singleton<GameState>.Instance.nameBackground;

        if (filtersForCharactersByScenes.ContainsKey(bg))
        {
            if (filtersForCharactersByScenes[bg].ContainsKey(charDataNew[1]))
            {
                float r = float.Parse(filtersForCharactersByScenes[bg][charDataNew[1]].Attributes["r"].Value);
                float g = float.Parse(filtersForCharactersByScenes[bg][charDataNew[1]].Attributes["g"].Value);
                float b = float.Parse(filtersForCharactersByScenes[bg][charDataNew[1]].Attributes["b"].Value);

                charTypesDict[type][pos].SecondLayer.color = new Color(r, g, b);
            }
            else
            {
                charTypesDict[type][pos].SecondLayer.color = new Color(1, 1, 1);
            }
        }
        else
        {
            charTypesDict[type][pos].SecondLayer.color = new Color(1, 1, 1);
            // Debug.Log("Данный фон " + bg + " не используется для фильтра персонажей.");
        }
    }

    private IEnumerator IEShowCharacter(Sprite currentSprite, string type, int pos)
    {
        FilterForCharacterToTheScene(type, currentSprite.name, pos);

        Character character = charTypesDict[type][pos];

        Color color = character.SecondLayer.color;

        character.SecondLayer.sprite = currentSprite;

        for (float f = 0f; f < 1f; f += 0.05f)
        {
            color.a = f;
            character.SecondLayer.color = color;
            yield return new WaitForFixedUpdate();
        }

        color.a = 1;
        character.SecondLayer.color = color;
    }

    private IEnumerator IEHideAndShowCharacter(Sprite currentSprite, string type, int pos)
    {
        FilterForCharacterToTheScene(type, currentSprite.name, pos);

        Character character = charTypesDict[type][pos];

        Color colorSecond = character.SecondLayer.color;
        Color colorFirst = character.FirstLayer.color;

        colorFirst.a = 0f;
        character.FirstLayer.sprite = currentSprite;

        for (float f = 1f; f > 0f; f -= 0.05f)
        {
            colorSecond.a = f;
            colorFirst.a = 1f - f;
            character.SecondLayer.color = colorSecond;
            character.FirstLayer.color = colorFirst;
            yield return new WaitForFixedUpdate();
        }

        colorSecond.a = 1f;
        colorFirst.a = 0f;

        character.SecondLayer.color = colorSecond;
        character.FirstLayer.color = colorFirst;

        character.SecondLayer.sprite = currentSprite;
        character.FirstLayer.sprite = Resources.Load<Sprite>("Images/Characters/Zero");
    }

    private IEnumerator IEHideAndShowEmotion(Sprite fiSprite, string type, int pos)
    {
        FilterForCharacterToTheScene(type, fiSprite.name, pos);

        Character character = charTypesDict[type][pos];

        string[] charDataNew = fiSprite.name.Split('_');

        Sprite secSprite = Resources.Load<Sprite>("Images/Characters/" + type + "Sprites/" + charDataNew[1] + "/" + fiSprite.name);
        character.FirstLayer.sprite = secSprite;

        Color FirstColor = character.SecondLayer.color;
        Color SecondColor = character.SecondLayer.color;

        for (float f = 0f; f < 1f; f += 0.05f)
        {
            FirstColor.a = f;
            character.FirstLayer.color = FirstColor;
            yield return new WaitForFixedUpdate();
        }

        FirstColor.a = 0;
        SecondColor.a = 1;

        character.FirstLayer.color = FirstColor;
        character.SecondLayer.color = SecondColor;

        character.SecondLayer.sprite = fiSprite;
    }

    public void RemoveTheCharacterFromTheScene(string[] types, string[] directions, int[] pos)
    {
        for (int i = 0; i < types.Length; i++)
        {
            string type = types[i];

            if (pos[i] >= 0 && pos[i] < charTypesDict[type].Length)
            {
                Singleton<GameState>.Instance.charTypesDict[type][pos[i]] = "";
                switch (directions[i].Trim())
                {
                    case "L": { StartCoroutine(IEHideCharacter(type, -0.2f, pos[i])); break; }
                    case "R": { StartCoroutine(IEHideCharacter(type, 0.2f, pos[i])); break; }
                    case "M": { StartCoroutine(IEHideCharacter(type, 0f, pos[i])); break; }
                    default: { Debug.Log("<color=red>Направление " + directions[i] + " не найдено</color>"); break; }
                }
            }
            else
            {
                Debug.Log("<color=red>Ошибка попытки убрать персонажа, выход за рамки доступных позиций pos: " + pos[i] + "</color>");
                return;
            }
        }
    }

    private IEnumerator IEHideCharacter(string type, float direction, int pos)
    {
        Character character = charTypesDict[type][pos];

        Color color = character.SecondLayer.color;
        if (color.a != 0)
        {
            for (float f = 1f; f > 0f; f -= 0.05f)
            {
                color.a = f;
                character.SecondLayer.color = color;
                character.SecondLayer.transform.Translate(new Vector3(direction, 0f));
                yield return new WaitForFixedUpdate();
            }
            color.a = 0;
            character.SecondLayer.color = color;

            character.SecondLayer.rectTransform.offsetMin = new Vector2(0, 0);
            character.SecondLayer.rectTransform.offsetMax = new Vector2(0, 0);

            character.SecondLayer.sprite = Resources.Load<Sprite>("Images/Characters/Zero");
            character.FirstLayer.sprite = Resources.Load<Sprite>("Images/Characters/Zero");
        }
    }

    public void MoveCharacterToTheScene(string type, string direction, int pos)
    {
        if (pos >= 0 && pos < charTypesDict[type].Length)
        {
            switch (direction.Trim())
            {
                case "L":
                    {
                        if (pos - 1 < 0) { Debug.Log("Направление " + direction + " недоступно"); break; }
                        StartCoroutine(IEMoveCharacter(type, direction, pos, pos - 1)); break;
                    }
                case "R":
                    {
                        if (pos + 1 >= charTypesDict[type].Length) { Debug.Log("Направление " + direction + " недоступно"); break; }
                        StartCoroutine(IEMoveCharacter(type, direction, pos, pos + 1)); break;
                    }
                default:  { Debug.Log("Направление " + direction + " не найдено"); break; }
            }
        }
        else
        {
            Debug.Log("<color=red>Ошибка попытки переместить персонажа, выход за рамки доступных позиций pos: " + pos + "</color>");
            return;
        }
    }

    private IEnumerator IEMoveCharacter(string type, string direction, int posStart, int posEnd)
    {
        Character characterStart = charTypesDict[type][posStart];
        Character characterEnd = charTypesDict[type][posEnd];

        Animator animatorCharacter = characterStart.SecondLayer.GetComponentInParent<Animator>();

        if (direction == "L")
        {
            animatorCharacter.SetBool("Left", true);
            yield return new WaitForFixedUpdate();
            animatorCharacter.SetBool("Left", false);
        }
        else if (direction == "R")
        {
            animatorCharacter.SetBool("Right", true);
            yield return new WaitForFixedUpdate();
            animatorCharacter.SetBool("Right", false);
        }
        yield return new WaitForSeconds(animatorCharacter.GetFloat("Time"));

        characterEnd.SecondLayer.color = new Color(1, 1, 1, 1);
        characterEnd.SecondLayer.sprite = characterStart.SecondLayer.sprite;
        Singleton<GameState>.Instance.charTypesDict[type][posEnd] = characterStart.SecondLayer.sprite.name;
        FilterForCharacterToTheScene(type, characterStart.SecondLayer.sprite.name, posEnd);

        characterStart.SecondLayer.color = new Color(1, 1, 1, 0);
        characterStart.SecondLayer.sprite = Resources.Load<Sprite>("Images/Characters/Zero");
        Singleton<GameState>.Instance.charTypesDict[type][posStart] = "";

        animatorCharacter.SetBool("Stay", true);
        yield return new WaitForFixedUpdate();
        animatorCharacter.SetBool("Stay", false);
    }

    public void FlinchCharacterToTheScene(string type, string[] directions, int[] pos)
    {
        if (pos.Length > 0 && pos.Length <= charTypesDict[type].Length)
        {
            for (int i = 0; i < pos.Length; i++)
            {
                if (pos[i] >= 0 && pos.Length < charTypesDict[type].Length)
                {
                    switch (directions[i].Trim())
                    {
                        case "H": { StartCoroutine(IEFlinchCharacter(type, "H", pos[i])); break; }
                        case "V": { StartCoroutine(IEFlinchCharacter(type, "V", pos[i])); break; }
                        default:  { Debug.Log("Направление " + directions[i] + " не найдено"); break; }
                    }
                }
                else
                {
                    Debug.Log("<color=red>Ошибка попытки убрать персонажа, выход за рамки доступных позиций pos: " + pos[i] + "</color>");
                    return;
                }
            }
        }
        else
        {
            Debug.Log("<color=red>Ошибка попытки убрать персонажа, выход за рамки кол-ва доступных позиций pos.Lenght = " + pos.Length + "</color>");
            return;
        }
    }

    private IEnumerator IEFlinchCharacter(string type, string direction, int pos)
    {
        Character character = charTypesDict[type][pos];

        if (direction == "H")
        {
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                character.SecondLayer.transform.Translate(new Vector3(-0.2f, 0.0f));
                yield return new WaitForFixedUpdate();
            }

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                character.SecondLayer.transform.Translate(new Vector3(0.2f, 0.0f));
                yield return new WaitForFixedUpdate();
            }
        }
        else if (direction == "V")
        {
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                character.SecondLayer.transform.Translate(new Vector3(0f, -0.2f));
                yield return new WaitForFixedUpdate();
            }

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                character.SecondLayer.transform.Translate(new Vector3(0f, 0.2f));
                yield return new WaitForFixedUpdate();
            }
        }
        character.SecondLayer.rectTransform.offsetMin = new Vector2(0f, 0f);
        character.SecondLayer.rectTransform.offsetMax = new Vector2(0f, 0f);
    }

    public void FlipCharacterToTheScene(string type, int pos, bool flip)
    {
        if (pos >= 0 && pos < charTypesDict[type].Length)
        {
            if (flip)
            {
                charTypesDict[type][pos].FirstLayer.transform.localScale = new Vector3(-1f, 1f, 1f);
                charTypesDict[type][pos].SecondLayer.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                charTypesDict[type][pos].FirstLayer.transform.localScale = new Vector3(1f, 1f, 1f);
                charTypesDict[type][pos].SecondLayer.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
        else
        {
            Debug.Log("<color=red>Ошибка попытки отразить персонажа, выход за рамки доступных позиций pos: " + pos + "</color>");
            return;
        }
    }

    public void InterfaceCharacterToggleSet(bool active)
    {
        for (int i = 0; i < charTypesDict["IF"].Length; i++)
        {
            IFCharacters.GetChild(i).gameObject.SetActive(active);
        }
    }
}