using UnityEngine;
using UnityEngine.UI;

public class DropDownImageManager : MonoBehaviour
{
    public Dropdown dropdown;
    public  Canvas canvasACT;
    private GameObject currentRawImageInstance;
    private const string SelectedPlanet = "SelectedPlanet";

    private void Start()
    {
        // Deshabilitar la interacci�n del Dropdown al inicio
        dropdown.interactable = true;
        // Cargar el primer elemento del Dropdown una vez  completado los Asset Bundles
        dropdown.value = 0;
        // Llama al m�todo OnDropdownValueChanged para instanciar el prefab
        OnDropdownValueChanged(0);

        // Asigna la funci�n al evento OnValueChanged 
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

 
    }

    private void OnDropdownValueChanged(int index)
    {
        // Destruye la instancia actual si existe
        DestroyCurrentInstance();

        // Obt�n el nombre de la imagen seg�n la opci�n seleccionada en el Dropdown
        string selectedImageName = GetSelectedImageName(index);
        string selectedPlanet = GetPlanetDesition(index);
        // Obt�n la imagen del ResourceManager 
        Sprite selectedImage = ResourceManagerSingleton.ResourceManagerInstance.GetImage(selectedImageName);
        ResourceManagerSingleton.ResourceManagerInstance.setPlanetdesition(selectedPlanet);
        // Instancia el RawImage si se encontr� el sprite
        if (selectedImage != null)
        {
            // Crea un nuevo GameObject con un componente RectTransform y RawImage, y asigna el sprite
            currentRawImageInstance = new GameObject("RawImageInstance");
            RectTransform rectTransform = currentRawImageInstance.AddComponent<RectTransform>();
            // Ajusta la posici�n y el tama�o seg�n tus necesidades
            rectTransform.SetParent(canvasACT.transform, false);  // Agrega el RawImage al Canvas
            RawImage rawImage = currentRawImageInstance.AddComponent<RawImage>();
            rawImage.texture = selectedImage.texture;

            // Ajustar la posici�n y el tama�o seg�n tus necesidades
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(1920, 1080);
            // Ajustar el z-order para que est� detr�s de todo
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, -1);
            // Colocar el RawImage como el primer hijo en la jerarqu�a del Canvas
            rectTransform.SetAsFirstSibling();

        }
    }

    private void OnDestroy()
    {
        // Almacenar el nombre del prefab seleccionado antes de destruir el objeto
        string selectedPrefabName = GetSelectedPrefabName(dropdown.value);
        PlayerPrefs.SetString(SelectedPlanet, selectedPrefabName);
        PlayerPrefs.Save();
    }
    private string GetSelectedPrefabName(int index)
    {
        // Asocia el �ndice del Dropdown con el nombre del prefab
        switch (index)
        {
            case 0: 
                return "mars";
            case 1:
                return "moon";
            case 2: 
                return "planetX";
            default:
                return "";
        }
    }
    private void DestroyCurrentInstance()
    {
        // Destruir la instancia actual si existe
        if (currentRawImageInstance != null)
        {
            Destroy(currentRawImageInstance);
        }
    }

    private string GetSelectedImageName(int index)
    {
        // Asociar el �ndice del Dropdown con el nombre del prefab
        switch (index)
        {
            case 0: // vikingLander
                return "marsImage";
            case 1: // Lunar Module
                return "lunarImage";
            case 2: // Lunar Module
                return "planetxImage";
            default:
                return "";
        }
    }
    private string GetPlanetDesition(int index)
    {
        // Asociar el �ndice del Dropdown con el nombre del prefab
        switch (index)
        {
            case 0: // vikingLander
                return "mars";
            case 1: // Lunar Module
                return "moon";
            case 2: // PlanetX
                return "planetx";
            default:
                return "";
        }
    }


}
