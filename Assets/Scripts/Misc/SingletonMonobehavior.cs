using UnityEngine;

public abstract class SingletonMonbehavior<T> : MonoBehaviour where T: MonoBehaviour
{
    private static T Instance;
    public static T instance
    {
        get
        {
            return Instance;
        }
    }
    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
