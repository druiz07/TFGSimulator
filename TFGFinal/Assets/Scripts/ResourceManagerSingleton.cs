using UnityEngine;

public class ResourceManagerSingleton : MonoBehaviour
{
    private static ResourceManager resourceManagerInstance;

    public static ResourceManager ResourceManagerInstance
    {
        get
        {
            if (resourceManagerInstance == null)
            {
                // Si la instancia aún no existe, crea un nuevo ResourceManager
                resourceManagerInstance = new ResourceManager();
            }

            return resourceManagerInstance;
        }
    }

    private void Awake()
    {
        // Asegurar que este objeto persista entre escenas
        DontDestroyOnLoad(gameObject);
    }
}