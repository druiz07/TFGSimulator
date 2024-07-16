using UnityEngine;

public class ConnectModels : MonoBehaviour
{
    [SerializeField] private GameObject groundObject;
    [SerializeField] private Material marsMaterial;
    [SerializeField] private Material moonMaterial;
    [SerializeField] private Material planetxMaterial;
    [SerializeField] private Material skyboxMars;
    [SerializeField] private Material skyboxMoon;
    [SerializeField] private Material skyboxPlanetX;

    private void Start()
    {
        // Recuperar el nombre del prefab seleccionado desde PlayerPrefs
        if (PlayerPrefs.HasKey("SelectedPrefabName"))
        {
            string selectedPrefabName = PlayerPrefs.GetString("SelectedPrefabName");
           

            // utilizar el nombre del prefab 
            GameObject selectedPrefab = ResourceManagerSingleton.ResourceManagerInstance.GetPrefab(selectedPrefabName);
            if (selectedPrefab != null)
            {
                // Haz que el prefab seleccionado sea hijo de este objeto
                Instantiate(selectedPrefab, transform.position, transform.rotation, transform);

                // Comprobacion de si el nombre del prefab contiene "mars" o "moon" y asigna el material correspondiente al suelo
               ;
                if (ResourceManagerSingleton.ResourceManagerInstance.getPlanetdesition().ToLower().Contains("mars"))
                {
                    SetGroundMaterial(marsMaterial);
                    SetSkybox(skyboxMars);
                }
                else if (ResourceManagerSingleton.ResourceManagerInstance.getPlanetdesition().ToLower().Contains("moon"))
                {
                    SetGroundMaterial(moonMaterial);
                    SetSkybox(skyboxMoon);
                }
                else if (ResourceManagerSingleton.ResourceManagerInstance.getPlanetdesition().ToLower().Contains("planetx"))
                {
                    SetGroundMaterial(planetxMaterial);
                    SetSkybox(skyboxPlanetX);
                }
            }
            else
            {
                Debug.LogWarning("El prefab seleccionado no se encontró en el ResourceManager.");
            }
        }
        else
        {
            Debug.LogWarning("No se ha seleccionado ningún prefab previamente.");
        }
    }

    private void SetGroundMaterial(Material material)
    {
        // Asigna el material al objeto del suelo si está disponible
        if (groundObject != null)
        {
            MeshRenderer groundRenderer = groundObject.GetComponent<MeshRenderer>();
            if (groundRenderer != null)
            {
                groundRenderer.material = material;
            }
            else
            {
                Debug.LogWarning("No se encontró un componente Renderer en el objeto del suelo.");
            }
        }
        else
        {
            Debug.LogWarning("No se ha asignado un objeto de suelo en el inspector.");
        }
    }

    private void SetSkybox(Material material)
    {
        // Cambia el Skybox del juego
        RenderSettings.skybox = material;
    }
}
