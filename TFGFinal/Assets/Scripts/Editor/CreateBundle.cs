using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using RESTClient;
using Azure.StorageServices;
using UnityEngine.Networking;
using System.IO;



public static class CreateBundle
{

    static string assetBundleDirectory;
    static string assetBundleName;

    [MenuItem("Assets/Create Asset Bundles")]
    private static void BuildAssetBundles()
    {
        assetBundleDirectory = EditorUtility.SaveFolderPanel("Guardar AssetBundle", "Assets", "AssetBundles");

        // Verifica si el usuario presionó "Cancelar" en el cuadro de diálogo
        if (assetBundleDirectory == "")
        {
            Debug.Log("Creación de AssetBundle cancelada.");
            return;
        }

        // Construir el AssetBundle
        try
        {
            BuildPipeline.BuildAssetBundles(
                assetBundleDirectory,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget
            );

            Debug.Log("Directorio en el que se ha guardado  " + assetBundleDirectory);

        }
        catch (Exception e)
        {
            Debug.LogWarning(e);
        }
    }

    [MenuItem("Assets/Upload All Asset Bundles")]
    private static void UploadAllBundlesToAzure()
    {
        assetBundleDirectory = EditorUtility.SaveFolderPanel("Elige la carpeta donde se encuentran los assets a subir", "Assets", "AssetBundles");
        string[] bundlePaths = Directory.GetFiles(assetBundleDirectory, "*");

        TemporaryMonoBehaviour tempMonoBehaviour = new GameObject("TemporaryMonoBehaviour").AddComponent<TemporaryMonoBehaviour>();
        tempMonoBehaviour.StartCoroutine(tempMonoBehaviour.UploadAssetBundlesCoroutine(bundlePaths));


    }
    [MenuItem("Assets/Upload All Images")]
    private static void UploadAllImagesToAzure()
    {
        assetBundleDirectory = EditorUtility.SaveFolderPanel("Elige la carpeta donde se encuentran los assets a subir", "Assets", "AssetBundles");
        string[] bundlePaths = Directory.GetFiles(assetBundleDirectory, "*");

        TemporaryMonoBehaviour tempMonoBehaviour = new GameObject("TemporaryMonoBehaviour").AddComponent<TemporaryMonoBehaviour>();
        tempMonoBehaviour.StartCoroutine(tempMonoBehaviour.UploadImageCoroutine(bundlePaths));


    }


    // Clase MonoBehaviour temporal
    public class TemporaryMonoBehaviour : MonoBehaviour
    {
        public IEnumerator UploadAssetBundlesCoroutine(string[] bundlePaths)
        {
            foreach (string bundlePath in bundlePaths)
            {
                // Obtener el nombre del archivo sin extensión
                string assetBundleName = Path.GetFileNameWithoutExtension(bundlePath);

                if (string.IsNullOrEmpty(Path.GetExtension(bundlePath)))
                {
                    if (assetBundleName != "AssetBundles")
                    {
                        // Cargar la configuración desde EditorPrefs
                        string storageAccount = EditorPrefs.GetString("StorageAccount", "");
                        string accessKey = EditorPrefs.GetString("AccessKey", "");
                        string container = EditorPrefs.GetString("Container", "");
                        StorageServiceClient client = StorageServiceClient.Create(storageAccount, accessKey);
                        BlobService blobService = client.GetBlobService();

                        // Lee el archivo del AssetBundle en bytes
                        byte[] assetBundleBytes = File.ReadAllBytes(bundlePath);

                        // Ruta en Azure Storage donde se almacenará el AssetBundle
                        string blobName = assetBundleName;
                        string resourcePath = container + "/" + blobName;

                        Debug.Log("Uploading AssetBundle to Azure: " + resourcePath);


                        yield return StartCoroutine(blobService.PutBlob(response => OnUploadAssetBundleComplete(response), assetBundleBytes, container, assetBundleName, "application/octet-stream"));
                        // Eliminar el archivo local después de cargarlo a Azure
                        File.Delete(bundlePath);
                        // Eliminar archivos .meta y .manifest correspondientes
                        string metaFilePath = bundlePath + ".meta";
                        string manifestFilePath = bundlePath + ".manifest";

                        if (File.Exists(metaFilePath))
                        {
                            File.Delete(metaFilePath);
                        }

                        if (File.Exists(manifestFilePath))
                        {
                            File.Delete(manifestFilePath);
                        }
                    }
                    else
                    {
                        File.Delete(bundlePath);
                        // Eliminar archivos .meta y .manifest correspondientes
                        string metaFilePath = bundlePath + ".meta";
                        string manifestFilePath = bundlePath + ".manifest";

                        if (File.Exists(metaFilePath))
                        {
                            File.Delete(metaFilePath);
                        }

                        if (File.Exists(manifestFilePath))
                        {
                            File.Delete(manifestFilePath);
                        }
                    }
                }
            }
            // Destruir el objeto MonoBehaviour temporal después de la tarea
            DestroyImmediate(gameObject);
        }

        public IEnumerator UploadImageCoroutine(string[] imagePaths)
        {
            foreach (string imagePath in imagePaths)
            {
                // Obtener el nombre del archivo sin extensión
                string imageName = Path.GetFileNameWithoutExtension(imagePath);
                // Obtener la extensión del archivo
                string fileExtension = Path.GetExtension(imagePath);
                if (fileExtension.Equals(".png", StringComparison.OrdinalIgnoreCase))
                {

                    // Cargar la configuración desde EditorPrefs
                    string storageAccount = EditorPrefs.GetString("StorageAccount", "");
                    string accessKey = EditorPrefs.GetString("AccessKey", "");
                    string container = EditorPrefs.GetString("Container", "");
                    StorageServiceClient client = StorageServiceClient.Create(storageAccount, accessKey);
                    BlobService blobService = client.GetBlobService();

                    // Lee el archivo del AssetBundle en bytes
                    byte[] imageBytes = File.ReadAllBytes(imagePath);

                    // Ruta en Azure Storage donde se almacenará  la Imagen
                    string blobName = imageName + ".png";
                    string resourcePath = container + "/" + blobName;

                    Debug.Log("Uploading Images to Azure: " + resourcePath);


                    yield return StartCoroutine(blobService.PutImageBlob(response => OnUploadImageComplete(response), imageBytes, container, blobName, "image/png"));
                    // Eliminar el archivo local después de cargarlo a Azure
                    File.Delete(imagePath);

                }
            }

            // Destruir el objeto MonoBehaviour temporal después de la tarea
            DestroyImmediate(gameObject);
        }

        private static void OnUploadAssetBundleComplete(RestResponse response)
        {
            if (response.IsError)
            {
                Debug.LogError("Failed to upload AssetBundle to Azure: " + response.StatusCode + ", " + response.ErrorMessage);
            }
            else
            {
                Debug.Log("AssetBundle uploaded to Azure successfully: " + response.Url);
            }
        }

        private static void OnUploadImageComplete(RestResponse response)
        {
            if (response.IsError)
            {
                Debug.LogError("Failed to upload Image to Azure: " + response.StatusCode + ", " + response.ErrorMessage);
            }
            else
            {
                Debug.Log("Image uploaded to Azure successfully: " + response.Url);
            }
        }
    }

}




public class AzureStorageConfigWindow : EditorWindow
{

    public static string storageAccount = "";
    public static string accessKey = "";
    public static string container = "";

    [MenuItem("Window/Azure Storage Config")]
    public static void ShowWindow()
    {
        GetWindow<AzureStorageConfigWindow>("Azure Storage Config");
        LoadConfig();  // Cargar la configuración al abrir la ventana
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Ingrese la información de la cuenta de almacenamiento:");

        storageAccount = EditorGUILayout.TextField("Storage Account", storageAccount);
        accessKey = EditorGUILayout.TextField("Access Key", accessKey);
        container = EditorGUILayout.TextField("Container", container);

        if (GUILayout.Button("Aceptar"))
        {
            // Guardar la configuración al hacer clic en "Aceptar"
            SaveConfig();
            Debug.Log($"Storage Account: {storageAccount}, Access Key: {accessKey}, Container: {container}");
            Close();
        }

        if (GUILayout.Button("Cancelar"))
        {
            Close();
        }
    }

    private static void SaveConfig()
    {
        // Guardar la configuración en EditorPrefs
        EditorPrefs.SetString("StorageAccount", storageAccount);
        EditorPrefs.SetString("AccessKey", accessKey);
        EditorPrefs.SetString("Container", container);
    }

    private static void LoadConfig()
    {
        // Cargar la configuración desde EditorPrefs
        storageAccount = EditorPrefs.GetString("StorageAccount", "");
        accessKey = EditorPrefs.GetString("AccessKey", "");
        container = EditorPrefs.GetString("Container", "");
    }
}
public  static class AzureStorageConfigWrapper
{
    public static string[] GetStorageAccount()
    {

        string[] data = { AzureStorageConfigWindow.storageAccount, AzureStorageConfigWindow.accessKey, AzureStorageConfigWindow.container };
        return data;

    }


}

