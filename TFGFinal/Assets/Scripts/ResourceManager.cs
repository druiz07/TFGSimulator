using System.Collections.Generic;
using UnityEngine;

//Clase que gestiona los recursos de la simulacion como modelos o imagenes
public class ResourceManager
{
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> imageDictionary = new Dictionary<string, Sprite>();
    private GameObject currentShipPrefab;
    private string planetdecision;
    private float massShip = 0.0f;
    private Dictionary<string, float> sliderValues = new Dictionary<string, float>();
    public void SetPrefab(string prefabName, GameObject prefab)
    {
        if (!prefabDictionary.ContainsKey(prefabName))
        {
            prefabDictionary.Add(prefabName, prefab);
        }
        else
        {
            prefabDictionary[prefabName] = prefab;
        }

    }
    // Método para establecer el valor de un atributo del slider
    public void SetSliderValue(string attributeName, float value)
    {
        if (sliderValues.ContainsKey(attributeName))
        {
            sliderValues[attributeName] = value;
        }
        else
        {
            sliderValues.Add(attributeName, value);
        }
    }

    public int getIndxPlanetDesition()
    {
        switch (planetdecision)
        {
            case "mars": // 
                return 0;
            case "moon": // 
                return 1;
            case "planetx": // PlanetX
                return 2;
            default:
                return 2;
        }

    }
    // Método para obtener el valor de un atributo del slider
    public float GetSliderValue(string attributeName)
    {
        if (sliderValues.ContainsKey(attributeName))
        {
            return sliderValues[attributeName];
        }
        else
        {
            Debug.LogError("No se encontró el atributo del slider: " + attributeName);
            return 0f; //Valor predeterminado
        }
    }
    public void setmassShip(float m)
    {
        massShip = m;
    }
    public float getShipMass()
    {
        return massShip;
    }
    public GameObject GetPrefab(string prefabName)
    {
        if (prefabDictionary.ContainsKey(prefabName))
        {
            return prefabDictionary[prefabName];
        }
        else
        {
            Debug.LogWarning("Prefab with name " + prefabName + " does not exist in the dictionary.");
            return null;
        }
    }

    public void ChangePrefab(string prefabName, GameObject newPrefab)
    {
        if (prefabDictionary.ContainsKey(prefabName))
        {
            prefabDictionary[prefabName] = newPrefab;
        }
        else
        {
            Debug.LogWarning("Prefab with name " + prefabName + " does not exist in the dictionary. Adding it.");
            SetPrefab(prefabName, newPrefab);
        }
    }

    public void DeletePrefab(string prefabName)
    {
        if (prefabDictionary.ContainsKey(prefabName))
        {
            prefabDictionary.Remove(prefabName);
        }
        else
        {
            Debug.LogWarning("Prefab with name " + prefabName + " does not exist in the dictionary.");
        }
    }

    public List<GameObject> GetAllPrefabs()
    {
        return new List<GameObject>(prefabDictionary.Values);
    }

    public void setShipPrefab(GameObject g)
    {
        if (g != null) currentShipPrefab = g;
    }
    public void setPlanetdesition(string decision)
    {
        if (decision != "") planetdecision = decision;
    }
    public string getPlanetdesition()
    {
        return planetdecision;
    }
    public GameObject GetPrefabAtIndex(int index)
    {
        if (index >= 0 && index < prefabDictionary.Count)
        {
            // Obtener el GameObject en la posición del índice
            return new List<GameObject>(prefabDictionary.Values)[index];
        }
        else
        {
            Debug.LogWarning("Index " + index + " is out of range.");
            return null;
        }
    }
    public List<Sprite> GetAllImagesOfSet(string imageSet)
    {
        List<Sprite> images = new List<Sprite>();

        foreach (var entry in imageDictionary)
        {
            if (entry.Key.StartsWith(imageSet))
            {
                images.Add(entry.Value);
            }
        }

        return images;
    }
    public void SetImage(string imageName, Sprite rawImage)
    {
        if (!imageDictionary.ContainsKey(imageName))
        {
            imageDictionary.Add(imageName, rawImage);
        }
        else
        {
            imageDictionary[imageName] = rawImage;
        }
    }

    public Sprite GetImage(string imageName)
    {
        if (imageDictionary.ContainsKey(imageName))
        {
            return imageDictionary[imageName];
        }
        else
        {
            Debug.LogWarning("Image with name " + imageName + " does not exist in the dictionary.");
            return null;
        }
    }

    public void ChangeImage(string imageName, Sprite newRawImage)
    {
        if (imageDictionary.ContainsKey(imageName))
        {
            imageDictionary[imageName] = newRawImage;
        }
        else
        {
            Debug.LogWarning("Image with name " + imageName + " does not exist in the dictionary. Adding it.");
            SetImage(imageName, newRawImage);
        }
    }

    public void DeleteImage(string imageName)
    {
        if (imageDictionary.ContainsKey(imageName))
        {
            imageDictionary.Remove(imageName);
        }
        else
        {
            Debug.LogWarning("Image with name " + imageName + " does not exist in the dictionary.");
        }
    }

    public List<Sprite> GetAllImages()
    {
        return new List<Sprite>(imageDictionary.Values);
    }

    public Sprite GetImageAtIndex(int index)
    {
        if (index >= 0 && index < imageDictionary.Count)
        {
            // Obtener la RawImage en la posición del índice
            return new List<Sprite>(imageDictionary.Values)[index];
        }
        else
        {
            Debug.LogWarning("Index " + index + " is out of range.");
            return null;
        }
    }
}
