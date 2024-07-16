using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ControladorAlmacenamiento : MonoBehaviour
{
    public GameObject azureLoadingPrefab;
    public GameObject loadingPrefab;

    void Start()
    {
        // Obtener el tipo de almacenamiento guardado en PlayerPrefs
        string tipoAlmacenamiento = PlayerPrefs.GetString("TipoAlmacenamiento");

        // Verificar el tipo de almacenamiento y activar/desactivar los objetos correspondientes
        if (tipoAlmacenamiento == "cloud")
        {
            if (loadingPrefab != null)
                Destroy(loadingPrefab);
        }
        else if (tipoAlmacenamiento == "local")
        {
            // Activar LoadingDatabase y desactivar AzureLoadingDataBase
            if (azureLoadingPrefab != null)
                Destroy(azureLoadingPrefab);

           
        }
        else
        {
            Debug.LogWarning("Tipo de almacenamiento desconocido: " + tipoAlmacenamiento);
        }
    }
}
