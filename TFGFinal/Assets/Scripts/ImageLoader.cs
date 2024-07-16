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

public class ImageLoader : MonoBehaviour
{

    [Header("Image Loading")]
    [SerializeField]
    private List<string> imageNames = new List<string>();

    public delegate void ImageLoadCompleteEvent();
    public static event ImageLoadCompleteEvent OnImagesLoaded;
    public GameObject loadingBarGO;
    private Slider loadingBar = null;
    RawImage targetRawImage;


    [SerializeField]
    private string folderPath = "Assets/Images"; // Carpeta local donde se guardará el AssetBundle

    public Image[] imageArrayLocal;


    void Awake()
    {
        string tipoAlmacenamiento = PlayerPrefs.GetString("TipoAlmacenamiento");
        if (tipoAlmacenamiento == "local")
        {
            CreateLoadingBar();
            LoadImages();
        }
        else Destroy(this);



    }


    void LoadImages()
    {
        // Obtener todos los archivos dentro de la carpeta de imágenes
        string[] files = Directory.GetFiles(folderPath, "*.png");

        // Recorrer todos los archivos
        int i = 0;
        float progressIncrement = 1f / files.Length;
        foreach (string file in files)
        {
            // Cargar la imagen desde el archivo
            Texture2D texture = LoadTextureFromFile(file);
            if (texture != null)
            {
                // Crear un Sprite a partir de la textura
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // Obtener el nombre del archivo (sin la extensión) como clave
                string imageName = Path.GetFileNameWithoutExtension(file);

                // Verificar si ya existe en el diccionario
                if (imageNames.Contains(imageName))
                {
                    // Agregar el nombre de la imagen a la lista
                    imageNames.Add(imageName);
                    // Llamar al método SetImage del ResourceManagerSingleton
                    ResourceManagerSingleton.ResourceManagerInstance.SetImage(imageName, sprite);
                }
                else
                {
                    
                    Debug.LogWarning("No existe la Imagen con ese nombre" + imageName);
                }
            }
            // Actualizar la barra de carga
            loadingBar.value = (i + 1) * progressIncrement;
            i++;
        }
    }

    Texture2D LoadTextureFromFile(string filePath)
    {
        // Cargar la imagen como un array de bytes
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        if (texture.LoadImage(fileData)) // Esta función convierte los datos de la imagen en formato Texture2D
        {
            return texture;
        }
        else
        {
            Debug.LogError("No se pudo cargar la imagen: " + filePath);
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
