using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using RESTClient;
using Azure.StorageServices;
using System;
using System.IO;


public class AssetBundleLoaderCloud : MonoBehaviour
{
    string storageAccount="";
    string accessKey=   "";
    string container = "";

    [Header("Asset Bundle Loading")]
    [SerializeField]
    private List<string> assetBundleNames = new List<string>(); // Lista de nombres de Asset Bundles
    [SerializeField]
    private List<string> prefabNames = new List<string>(); // Lista de nombres de prefabs correspondientes
    private static StorageServiceClient client;
    private static BlobService blobService;
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    public delegate void AssetBundleLoadCompleteEvent();
    public static event AssetBundleLoadCompleteEvent OnAssetBundleLoadComplete;
    public GameObject loadingBarGO;
    private Slider loadingBar;

    void Awake()
    {
        string tipoAlmacenamiento = PlayerPrefs.GetString("TipoAlmacenamiento");
        // Cargar la configuración desde EditorPrefs
        storageAccount = EditorPrefs.GetString("StorageAccount", "");
        accessKey = EditorPrefs.GetString("AccessKey", "");
        container = EditorPrefs.GetString("Container", "");

        // Verificar el tipo de almacenamiento y activar/desactivar los objetos correspondientes
        if (tipoAlmacenamiento == "cloud")
        {
            if (string.IsNullOrEmpty(storageAccount) || string.IsNullOrEmpty(accessKey))
            {
                Debug.LogError("Storage account and access key are required. Enter storage account and access key in Unity Editor");
            }


            CreateLoadingBar();
            client = StorageServiceClient.Create(storageAccount, accessKey);
            blobService = client.GetBlobService();
            //  cargar los Asset Bundles autom�ticamente al inicio
            StartCoroutine(LoadAssetBundlesFromAzure());
        }
        else Destroy(this);
    }

    private IEnumerator LoadAssetBundlesFromAzure()
    {
        float progressIncrement = 1f / assetBundleNames.Count;
        for (int i = 0; i < assetBundleNames.Count; i++)
        {
            string assetBundleName = assetBundleNames[i];
            string blobName = assetBundleName;
            string resourcePath = container + "/" + blobName;

            Debug.Log("Loading Asset Bundle: " + resourcePath);

            yield return StartCoroutine(blobService.GetBlob(response => GetAssetBundleComplete(response, prefabNames[i]), resourcePath));
            // Actualizar la barra de carga
            loadingBar.value = (i + 1) * progressIncrement;
        }

        if (OnAssetBundleLoadComplete != null)
        {
            OnAssetBundleLoadComplete.Invoke();
        }

    }

    private void GetAssetBundleComplete(IRestResponse<byte[]> response, string prefabName)
    {
        if (response.IsError)
        {
            Debug.LogError("Failed to load Asset Bundle: " + response.StatusCode + ", " + response.ErrorMessage);
            return;
        }

        Debug.Log("Loaded Asset Bundle: " + response.Url);

        // Cargar el Asset Bundle desde los bytes descargados
        AssetBundle assetBundle = AssetBundle.LoadFromMemory(response.Data);

        if (assetBundle == null)
        {
            Debug.LogError("Failed to load Asset Bundle from memory");
            return;
        }

        GameObject loadedPrefab = assetBundle.LoadAsset<GameObject>(prefabName);

        if (loadedPrefab != null)
        {
            // Agregar el objeto cargado al diccionario con el nombre del prefab como clave
            if (!prefabDictionary.ContainsKey(prefabName))
            {
                prefabDictionary.Add(prefabName, loadedPrefab);
               
                ResourceManagerSingleton.ResourceManagerInstance.SetPrefab(prefabName, loadedPrefab);
            }
            else
            {
                Debug.LogWarning("Prefab with name " + prefabName + " already exists in the dictionary.");
            }
        }
        else
        {
            Debug.LogError("Failed to load prefab: " + prefabName);
        }



        // liberar el Asset Bundle cuando se haya  terminado de usar
        assetBundle.Unload(false);
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