using System.Collections;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [HideInInspector]
    public ScriptReferencesPool srp;

    public Animator AnimationPrefab;
    public Animator AnimationStepPrefab;
    public Animator AnimationLoopedPrefab;

    [Space]
    public GameObject AnimationGameObject;
    public GameObject AnimationStepGameObject;
    public GameObject AnimationLoopedGameObject;
    public GameObject AnimationCombinedGameObject;

    void Start()
    {
        if (Singleton<GameState>.Instance.combinAnimation != "")
        {
            StartCombinAnimation(Singleton<GameState>.Instance.combinAnimation);
        }
        else if (Singleton<GameState>.Instance.loopAnimation != "")
        {
            StartLoopAnimation(Singleton<GameState>.Instance.loopAnimation);
        }
        else if (Singleton<GameState>.Instance.stepAnimation != "")
        {
            string[] mas = Singleton<GameState>.Instance.stepAnimation.Split('^');
            if (mas.Length == 2)
            {
                StartStepAnimator(mas[0], mas[1]);
            }
            else if (mas.Length == 3)
            {
                StartStepAnimator(mas[0], mas[1], mas[2]);
            }
        }
    }

    public void StartAnimator(string nameAnimation, string lang = "")
    {
        StopAnimation();

        GameObject prefab = Resources.Load<GameObject>("AnimationPrefabs/Animation/" + nameAnimation);
        if (prefab != null)
        {
            GameObject g = Instantiate(prefab, transform);
            g.name = nameAnimation;
            AnimationGameObject = g;
            Animator anim = g.GetComponent<Animator>();
            AnimationPrefab = anim;
            if (lang == "Rus" || lang == "Eng") { anim.SetBool(lang, true); }
            anim.SetBool("Start", true);
            StartCoroutine(IEEndAnimation());
        }
        else
        {
            Debug.Log("<color=red>Анимация : " + nameAnimation + " не найдена</color>");
        }
    }

    public void StartAnimator(string nameAnimation, string nameSound, float timeStart, string lang = "")
    {
        StartAnimator(nameAnimation, lang);
        StartCoroutine(IEAnimationSound(nameSound, timeStart));
    }

    public void StartStepAnimator(string nameAnimation, string nameAnimLayer_0)
    {
        GameObject prefab = Resources.Load<GameObject>("AnimationPrefabs/StepAnimation/" + nameAnimation);
        if (prefab != null)
        {
            if (AnimationStepGameObject != null && AnimationStepGameObject.name == nameAnimation)
            {
                AnimationStepPrefab.Play(nameAnimLayer_0, 0);
            }
            else
            {
                GameObject g = Instantiate(prefab, transform);
                g.name = nameAnimation;
                AnimationStepGameObject = g;
                Animator anim = g.GetComponent<Animator>();
                AnimationStepPrefab = anim;
                AnimationStepPrefab.Play(nameAnimLayer_0, 0);
            }
            Singleton<GameState>.Instance.stepAnimation = nameAnimation + "^" + nameAnimLayer_0;
        }
        else
        {
            Debug.Log("<color=red>Анимация : " + nameAnimation + " не найдена</color>");
        }
    }

    public void StartStepAnimator(string nameAnimation, string nameAnimLayer_0, string nameAnimLayer_1)
    {
        GameObject prefab = Resources.Load<GameObject>("AnimationPrefabs/StepAnimation/" + nameAnimation);
        if (prefab != null)
        {
            if (AnimationStepGameObject != null && AnimationStepGameObject.name == nameAnimation)
            {
                if (nameAnimLayer_1 == "")
                {
                    AnimationStepPrefab.Play("Stop", 1);
                }
                else
                {
                    AnimationStepPrefab.Play(nameAnimLayer_1, 1);
                }
                AnimationStepPrefab.Play(nameAnimLayer_0, 0);
            }
            else
            {
                GameObject g = Instantiate(prefab, transform);
                g.name = nameAnimation;
                AnimationStepGameObject = g;
                Animator anim = g.GetComponent<Animator>();
                AnimationStepPrefab = anim;
                AnimationStepPrefab.Play(nameAnimLayer_0, 0);
                AnimationStepPrefab.Play(nameAnimLayer_1, 1);
            }
            Singleton<GameState>.Instance.stepAnimation = nameAnimation + "^" + nameAnimLayer_0 + "^" + nameAnimLayer_1;
        }
        else
        {
            Debug.Log("<color=red>Анимация : " + nameAnimation + " не найдена</color>");
        }
    }

    public void StartLoopAnimation(string nameAnimation)
    {
        GameObject prefab = Resources.Load<GameObject>("AnimationPrefabs/LoopedAnimation/" + nameAnimation);
        if (prefab != null)
        {
            GameObject g = Instantiate(prefab, transform);
            g.name = nameAnimation;
            AnimationLoopedGameObject = g;
            Animator anim = g.GetComponent<Animator>();
            AnimationLoopedPrefab = anim;
            anim.SetBool("Start", true);
            Singleton<GameState>.Instance.loopAnimation = nameAnimation;
        }
        else
        {
            Debug.Log("<color=red>Анимация : " + nameAnimation + " не найдена</color>");
        }
    }

    public void StartCombinAnimation(string nameAnimation)
    {
        GameObject prefab = Resources.Load<GameObject>("AnimationPrefabs/CombinedAnimation/" + nameAnimation);
        if (prefab != null)
        {
            GameObject g = Instantiate(prefab, transform);
            g.name = nameAnimation;
            AnimationCombinedGameObject = g;
            Singleton<GameState>.Instance.combinAnimation = nameAnimation;
        }
        else
        {
            Debug.Log("<color=red>Анимация : " + nameAnimation + " не найдена</color>");
        }
    }

    public void StopAnimation()
    {
        if (AnimationPrefab != null)
        {
            AnimationPrefab.StopPlayback();
            AnimationPrefab = new Animator();
        }

        if (AnimationGameObject != null)
        {
            Destroy(AnimationGameObject);
            AnimationGameObject = null;
            srp.MusicController.StopSound();
        }
    }

    public void StopStepAnimation()
    {
        if (AnimationStepPrefab != null)
        {
            AnimationStepPrefab.StopPlayback();
            AnimationStepPrefab = new Animator();
        }

        if (AnimationStepGameObject != null)
        {
            Destroy(AnimationStepGameObject);
            AnimationStepGameObject = null;
            Singleton<GameState>.Instance.stepAnimation = "";
        }
    }

    public void StopLoopedAnimation()
    {
        if (AnimationLoopedPrefab != null)
        {
            AnimationLoopedPrefab.StopPlayback();
            AnimationLoopedPrefab = new Animator();
        }

        if (AnimationLoopedGameObject != null)
        {
            Destroy(AnimationLoopedGameObject);
            AnimationLoopedGameObject = null;
            Singleton<GameState>.Instance.loopAnimation = "";
        }
    }

    public void StopCombinedAnimation()
    {
        if (AnimationCombinedGameObject != null)
        {
            Destroy(AnimationCombinedGameObject);
            AnimationCombinedGameObject = null;
            Singleton<GameState>.Instance.combinAnimation = "";
        }
    }

    private IEnumerator IEAnimationSound(string nameSound, float timeStart)
    {
        yield return new WaitForSeconds(timeStart);
        srp.MusicController.LoadSound(nameSound);
    }

    private IEnumerator IEEndAnimation()
    {
        yield return new WaitForSeconds(AnimationPrefab.GetFloat("Time"));
        srp.ScenarioParser.AnimMode = false;
        if (!srp.ScenarioParser.DeathMode)
        {
            srp.ScenarioParser.InterfacePanelSetActive(true);
        }
        StopAnimation();
        UIClickBlocker.allowClick = true;
    }
}