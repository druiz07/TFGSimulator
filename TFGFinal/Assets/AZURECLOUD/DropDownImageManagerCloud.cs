using UnityEngine;
using UnityEngine.UI;

public class DropDownImageManagerCloud : MonoBehaviour
{
    public Dropdown dropdown;
    public  Canvas canvasACT;
    private GameObject currentRawImageInstance;

    private void Start()
    {
        // Deshabilita la interacci�n del Dropdown al inicio
        dropdown.interactable = true;
        // Cargar el primer elemento del Dropdown una vez  completado los Asset Bundles
        dropdown.value = 0;
        // Llama al m�todo OnDropdownValueChanged para instanciar el prefab
        OnDropdownValueChanged(0);

        // Asigna la funci�n al evento OnValueChanged del Dropdown
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);

 
    }

    private void OnDropdownValueChanged(int index)
    {
        // Destruye la instancia actual si existe
        DestroyCurrentInstance();

        // Obtener el nombre de la imagen seg�n la opci�n del dropdown
        string selectedImageName = GetSelectedImageName(index);

        // Obt�n la imagen del ResourceManager (aseg�rate de que las im�genes est�n configuradas como sprites en el Inspector)
        Sprite selectedImage = ResourceManagerSingleton.ResourceManagerInstance.GetImage(selectedImageName);
       // ResourceManagerSingleton.ResourceManagerInstance.setPlanetSpriteImage(selectedImageName     );


        // Instancia el RawImage si se encontr� el sprite
        if (selectedImage != null)
        {
            // Crear un nuevo GameObject con un componente RectTransform y RawImage, y asigna el sprite
            currentRawImageInstance = new GameObject("RawImageInstance");
            RectTransform rectTransform = currentRawImageInstance.AddComponent<RectTransform>();
            // Ajustar la posici�n y el tama�o
            rectTransform.SetParent(canvasACT.transform, false);  // Agrega el RawImage al Canvas
            RawImage rawImage = currentRawImageInstance.AddComponent<RawImage>();
            rawImage.texture = selectedImage.texture;

            // Ajustar lar posici�n y el tama�o 
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(1920, 1080);
            // Ajustar el z-order para que est� detr�s de todo
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, -1);
            // Colocar el RawImage como el primer hijo en la jerarqu�a del Canvas
            rectTransform.SetAsFirstSibling();

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
                return "moonimage";
            
            default:
                return "";
        }
    }

    
}
