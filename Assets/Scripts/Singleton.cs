using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
	private static T instance;
	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindFirstObjectByType<T>(); // try to find existing object in scene
				if (instance == null) // not in scene yet, create
				{
					GameObject obj = new GameObject(typeof(T).Name);
					instance = obj.AddComponent<T>();
				}
				DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}
	}

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
	}

	protected virtual void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}
}
