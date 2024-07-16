using RESTClient;
using Azure.StorageServices;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEditor;
using System;

using System.IO;

public class ImageLoaderCloud : MonoBehaviour
{

    [Header("Image Loading")]
    [SerializeField]
    private List<string> imageNames = new List<string>(); // Puedes cambiar esto directamente en el Inspector

    private static StorageServiceClient client;
    private static BlobService blobService;
    public delegate void ImageLoadCompleteEvent();
    public static event ImageLoadCompleteEvent OnImagesLoaded;
    public GameObject loadingBarGO;
    private Slider loadingBar = null;
    RawImage targetRawImage;

    [SerializeField]
    string storageAccount = "";
    [SerializeField]
    string accessKey = "";
    [SerializeField]
    string container = "";
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

            client = StorageServiceClient.Create(storageAccount, accessKey);
            blobService = client.GetBlobService();
            CreateLoadingBar();
            // Llamada  a la función para cargar la imagen  al inicio
            StartCoroutine(LoadImageFromAzureBlob());
        }
        else Destroy(this);
    }

    private IEnumerator LoadImageFromAzureBlob()
    {
        float progressIncrement = 1f / imageNames.Count;

        for (int i = 0; i < imageNames.Count; i++)
        {
            string blobName = imageNames[i] + ".png";
            string resourcePath = container + "/" + blobName;

            Debug.Log("Loading: " + resourcePath);

            yield return StartCoroutine(blobService.GetBlob(GetImageBlobComplete, resourcePath));

            // Actualizar la barra de carga
            loadingBar.value = (i + 1) * progressIncrement;
        }
        if (OnImagesLoaded != null)
        {
            OnImagesLoaded.Invoke();
        }

    }

    private void GetImageBlobComplete(IRestResponse<byte[]> response)
    {
        if (response.IsError)
        {
            Debug.LogError("Failed to load image: " + response.StatusCode + ", " + response.ErrorMessage);
        }
        else
        {
            Debug.Log("Loaded image: " + response.Url);

            // Cargar la imagen en la RawImage de Unity
            Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
            texture.LoadImage(response.Data);

            // Obtener el nombre de la imagen desde la URL
            string imageName = GetImageNameFromUrl(response.Url);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

            // Agregar la nueva RawImage al ResourceManager
            ResourceManagerSingleton.ResourceManagerInstance.SetImage(imageName, sprite);
            // Destruir el objeto GameObject después de asignarlo al ResourceManager
           


        }
    }

    // Método para extraer el nombre de la imagen desde la URL.
    private string GetImageNameFromUrl(string url)
    {
        int lastSlashIndex = url.LastIndexOf('/');
        if (lastSlashIndex != -1 && lastSlashIndex < url.Length - 1)
        {
            // Obtener el nombre de la imagen eliminando todo lo que está a la derecha de la última barra
            return url.Substring(lastSlashIndex + 1).Replace(".png", ""); // Eliminar la extensión para obtener el nombre
        }
        else
        {
            Debug.LogWarning("Invalid URL format: " + url);
            return null;
        }
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
