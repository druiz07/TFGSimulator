
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using UnityEngine.SceneManagement;
public class ChangeSceneStorage : MonoBehaviour
{

    bool cloud = false;
    bool local = false;
    string almacenamiento;

    public void CambiarEscena(string nombreDeEscenaDestino)
    {
        // Cargar la escena con el nombre proporcionado      

        if ((cloud && local) || (!cloud! && !local))
        {

            Debug.Log("No se puede cambiar de escena porque no está definido un tipo de almacenamiento o están ambos activados.");
        }
        else
        {
            // Guardar el tipo de almacenamiento en PlayerPrefs
            if (local)
            {
                almacenamiento = "local";
            }
            else if (cloud)
            {
                almacenamiento = "cloud";
            }
            PlayerPrefs.SetString("TipoAlmacenamiento", almacenamiento);
            // Cargar la escena con el nombre proporcionado
            SceneManager.LoadScene(nombreDeEscenaDestino);
        }
    }
    public void setLocalStorage()
    {
        local = !local;
    }
    public void setCloudStorage()
    {
        cloud = !cloud;
    }

}

