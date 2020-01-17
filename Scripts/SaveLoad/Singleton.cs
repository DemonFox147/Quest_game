using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static object _lock = new object();

    private static bool applicationIsQuitting = false;

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                return (T)((object)null);
            }
            object @lock = _lock;
            T instance;
            lock (@lock)
            {
                if (_instance == null)
                {
                    _instance = (T)FindObjectOfType(typeof(T));
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        instance = _instance;
                        return instance;
                    }
                    if (_instance == null)
                    {
                        GameObject gameObject = new GameObject();
                        _instance = gameObject.AddComponent<T>();
                        gameObject.name = "(singleton) " + typeof(T).ToString();
                        DontDestroyOnLoad(gameObject);
                    }
                }
                instance = _instance;
            }
            return instance;
        }
    }

    public void OnDestroy()
    {
        applicationIsQuitting = true;
    }
}