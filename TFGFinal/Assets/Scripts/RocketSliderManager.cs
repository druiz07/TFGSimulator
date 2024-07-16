using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class RocketSliderManager : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderValueText; 
    public Dropdown dropdown;


    private RocketMass rocketAttributes;
    private string filePath;
    private float offsetWind = 15;
    private float indexChosen = 0;
    private float minimumValue = 95; 

    private void Start()
    {
        filePath = "jsons/rocket_mass"; // Nombre del archivo JSON sin extensión


        LoadRocketAttributes(); // Carga los atributos del Rocket desde el archivo JSON al inicio

        minimumValue = slider.value; // Asigna el valor inicial del slider como el mínimo
        SetValue(minimumValue);
        // Configurar el valor del slider con el valor del atributo del JSON
        if (rocketAttributes != null)
        {
            float defaultValue = GetValueFromIDX(0);
            if (defaultValue != float.MinValue)
            {
                slider.value = defaultValue;
                sliderValueText.text = defaultValue.ToString();
            }
        }
        else
        {
            Debug.LogError("Los atributos del Rocket no se han cargado correctamente.");
        }


        slider.onValueChanged.AddListener(OnSliderChanged);
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

     
    }

    private void LoadRocketAttributes()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath); // Carga el archivo JSON como TextAsset

        // Si el archivo JSON se ha cargado correctamente
        if (jsonFile != null)
        {
            // Deserializa el JSON en la instancia de RocketAttributes
            rocketAttributes = JsonUtility.FromJson<RocketMass>(jsonFile.text);
        }
        else
        {
            Debug.LogError("No se pudo cargar el archivo JSON de atributos.");
        }
    }

    // Método que se llama cuando cambia el valor del slider
    public void OnSliderChanged(float newValue)
    {
        if (rocketAttributes != null)
        {
            SetValue(newValue);
            sliderValueText.text = Math.Round(newValue, 2).ToString();
        }
        else
        {
            Debug.LogError("Los atributos del Rocket no se han cargado correctamente.");
        }
    }

    // Método que se llama cuando cambia la selección del Dropdown
    public void OnDropdownValueChanged(int index)
    {
        indexChosen = index;
        float value = GetValueFromIDX(index);
        slider.value = value;
        sliderValueText.text = value.ToString();
    }

    public void SaveRocketAttributes()
    {
        string directoryPath = Path.Combine(Application.dataPath, "Resources", "jsons");
        filePath = Path.Combine(directoryPath, "rocket_mass.json");
        string jsonContent = JsonUtility.ToJson(rocketAttributes, true);
        PlayerPrefs.SetString("RocketMass", jsonContent);
        PlayerPrefs.Save();
        File.WriteAllText(filePath, jsonContent);
    }

    private float GetValueFromIDX(int idx)
    {
        float value = float.MinValue;

        switch (idx)
        {
            case 0:
                value = rocketAttributes.VikingLander;
                break;
            case 1:
                value = rocketAttributes.LunarModule;
                break;
            default:
                Debug.LogError("El indice del atributo no se reconoce: " + name);
                break;
        }

        return value;
    }

    private void SetValue(float value)
    {
        switch (indexChosen)
        {
            case 0:
                rocketAttributes.VikingLander = value;
                break;
            case 1:
                rocketAttributes.LunarModule = value;
                break;
            default:
                Debug.LogError("El indice de acceso a la nave no se reconoce: " + name);
                break;
        }
        ResourceManagerSingleton.ResourceManagerInstance.setmassShip(value);
        SaveRocketAttributes();
    }

}
