
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using UnityEngine.SceneManagement;
public class ChangeScene : MonoBehaviour
{


    // Método para cambiar de escena
    public void CambiarEscena(string nombreDeEscenaDestino)
    {
        // Cargar la escena con el nombre proporcionado



        string directoryPath = Path.Combine(Application.dataPath, "Resources", "jsons");
        string filePath = Path.Combine(directoryPath, "rocket_attributes.json");
        if (File.Exists(filePath))
        {
            string[] fileLines = File.ReadAllLines(filePath);
            foreach (string line in fileLines)
            {
                Debug.Log(line);
            }
        }
        else
        {
            Debug.LogError("El archivo JSON no se encontró después de guardarlo.");
        }

        SceneManager.LoadScene(nombreDeEscenaDestino);
    }
    public void CambiarEscenaSINDEBUG(string nombreDeEscenaDestino)
    {
        // Cargar la escena con el nombre proporcionado

        SceneManager.LoadScene(nombreDeEscenaDestino);
    }
}

