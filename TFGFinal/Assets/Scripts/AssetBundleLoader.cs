using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityGoogleDrive;


public class AssetBundleLoader : MonoBehaviour
{
    [Header("Asset Bundle Loading")]
    [SerializeField] private List<string> assetBundleNames = new List<string>(); // Lista de nombres de Asset Bundles
    [SerializeField] private List<string> prefabNames = new List<string>(); // Lista de nombres de prefabs correspondientes
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    [SerializeField] private GameObject[] prefabs; // Prefabs como variables públicas si no estamos en el editor


    public GameObject loadingBarGO;
    private Slider loadingBar;



    void Start()
    {
        string tipoAlmacenamiento = PlayerPrefs.GetString("TipoAlmacenamiento");
        if (tipoAlmacenamiento == "local")
        {
            CreateLoadingBar();


            // Asociar los prefabs como variables públicas solo en tiempo de ejecución
            int i = 0;
            float progressIncrement = 1f / prefabNames.Count;
            foreach (GameObject prefab in prefabs)
            {
                string prefabName = prefab.name;
                prefabDictionary.Add(prefabName, prefab);
                ResourceManagerSingleton.ResourceManagerInstance.SetPrefab(prefabName, prefab);
                loadingBar.value = (i + 1) * progressIncrement;
                i++;
            }
        }
        else Destroy(this);



    }


    private void CreateLoadingBar()
    {
        // Crear un objeto GameObject para el Slider
        loadingBar = loadingBarGO.GetComponent<Slider>();

        // Configurar las propiedades del Slider
        loadingBar.minValue = 0;
        loadingBar.maxValue = 1;
        loadingBar.value = 0;
    }
}
