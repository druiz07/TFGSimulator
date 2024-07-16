using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class SliderManager : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderValueText; // Cambiar el tipo de texto a TextMeshProUGUI
    public Dropdown dropdown;
    public Dropdown dropdownOpt;
    public string attributeName; // Nombre del atributo en la clase RocketAttributes
    private RocketAttributes rocketAttributes;
    private string filePath;
    private float offsetWind = 25;
    private float minWindVal = 5;

    private void Start()
    {
        filePath = "jsons/rocket_attributes"; // Nombre del archivo JSON sin extensión

        LoadRocketAttributes(); // Carga los atributos del Rocket desde el archivo JSON al inicio

        // Configurar  el valor del slider con el valor del atributo del JSON
        if (rocketAttributes != null)
        {
            float defaultValue = GetValueFromAttributeNameWind(attributeName);
            if (defaultValue != float.MinValue)
            {
                slider.value = defaultValue;
                sliderValueText.text = defaultValue.ToString();
            }
            else
            {
                Debug.LogError("El atributo " + attributeName + " no se encuentra en los atributos del Rocket.");
            }
        }
        else
        {
            Debug.LogError("Los atributos del Rocket no se han cargado correctamente.");
        }

        if (dropdownOpt == null || (dropdownOpt.value ==0 && (attributeName=="thrustForce" || attributeName == "fuelAmount"))) slider.gameObject.SetActive(false); // Desactiva el slider por defecto
        else slider.gameObject.SetActive(true);

        slider.onValueChanged.AddListener(OnSliderChanged);
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        // Guardar los valores por defecto en el ResourceManagerSingleton
        SaveDefaultValues();
    }

    private void LoadRocketAttributes()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath); // Carga el archivo JSON como TextAsset

        // Si el archivo JSON se ha cargado correctamente
        if (jsonFile != null)
        {
            // Deserializa el JSON en la instancia de RocketAttributes
            rocketAttributes = JsonUtility.FromJson<RocketAttributes>(jsonFile.text);
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
            SetValueToAttributeName(attributeName, newValue);
            sliderValueText.text = Math.Round(newValue, 2).ToString();
            ResourceManagerSingleton.ResourceManagerInstance.SetSliderValue(attributeName, newValue);
        }
        else
        {
            Debug.LogError("Los atributos del Rocket no se han cargado correctamente.");
        }
    }

    // Método que se llama cuando cambia la selección del Dropdown
    public void OnDropdownValueChanged(int index)
    {


        if (index == dropdown.options.Count - 1) // Si se selecciona la última opción del Dropdown
        {
            slider.gameObject.SetActive(true); // Activa el slider

        }
        else
        {
            slider.gameObject.SetActive(false); // Desactiva el slider


        }
    }


    private void SaveDefaultValues()
    {
        // Guarda los valores por defecto en el ResourceManagerSingleton
        if (rocketAttributes != null)
        {
            float defaultValue = GetValueFromAttributeName(attributeName);
            ResourceManagerSingleton.ResourceManagerInstance.SetSliderValue(attributeName, defaultValue);
        }
    }
    public void SaveRocketAttributes()
    {



        string directoryPath = Path.Combine(Application.dataPath, "Resources", "jsons");
        filePath = Path.Combine(directoryPath, "rocket_attributes.json");
        string jsonContent = JsonUtility.ToJson(rocketAttributes, true);
        PlayerPrefs.SetString("RocketAttributes", jsonContent);
        PlayerPrefs.Save();
        File.WriteAllText(filePath, jsonContent);
    }

    private float GetValueFromAttributeName(string name)
    {
        float value = float.MinValue;

        switch (name)
        {
            case "PlanetGravity":
                value = (float)Math.Round(rocketAttributes.PlanetGravity, 2); ;
                break;
            case "scaleForceUser":
                value = (float)Math.Round(rocketAttributes.scaleForceUser, 3);
                break;
            case "windForce":
                value = rocketAttributes.windForce + offsetWind;
                break;
            case "fuelConsumptionAmount":
                value = rocketAttributes.fuelConsumptionAmount;
                break;
            case "maxVelocityforLanding":
                value = rocketAttributes.maxVelocityforLanding;
                break;
            case "thrustForce":
                value = rocketAttributes.thrustForce;
                break;
            case "fuelAmount":
                value = rocketAttributes.fuelAmount;
                break;

            default:
                Debug.LogError("El nombre del atributo no se reconoce: " + name);
                break;
        }

        return value;
    }
    private float GetValueFromAttributeNameWind(string name)
    {
        float value = float.MinValue;

        switch (name)
        {
            case "PlanetGravity":
                value = (float)Math.Round(rocketAttributes.PlanetGravity, 2); ;
                break;
            case "scaleForceUser":
                value = (float)Math.Round(rocketAttributes.scaleForceUser, 3);
                break;
            case "windForce":
                value = minWindVal;
                break;
            case "fuelConsumptionAmount":
                value = rocketAttributes.fuelConsumptionAmount;
                break;
            case "maxVelocityforLanding":
                value = rocketAttributes.maxVelocityforLanding;
                break;
            case "thrustForce":
                value = rocketAttributes.thrustForce;
                break;
            case "fuelAmount":
                value = rocketAttributes.fuelAmount;
                break;

            default:
                Debug.LogError("El nombre del atributo no se reconoce: " + name);
                break;
        }

        return value;
    }

    private void SetValueToAttributeName(string name, float value)
    {
        switch (name)
        {
            case "PlanetGravity":
                rocketAttributes.PlanetGravity = (float)Math.Round(value, 2);
                break;
            case "scaleForceUser":
                rocketAttributes.scaleForceUser = (float)Math.Round(value, 2);
                break;
            case "windForce":
                if (value > 0) rocketAttributes.windForce = value + offsetWind;
                else rocketAttributes.windForce = 0;
                break;
            case "fuelConsumptionAmount":
                rocketAttributes.fuelConsumptionAmount = value;
                break;
            case "maxVelocityforLanding":
                rocketAttributes.maxVelocityforLanding = value;
                break;
            case "thrustForce":
                rocketAttributes.thrustForce = value;
                break;
            case "fuelAmount":
                rocketAttributes.fuelAmount = value;
                break;
            default:
                Debug.LogError("El nombre del atributo no se reconoce: " + name);
                break;
        }
        SaveRocketAttributes();

    }


}
