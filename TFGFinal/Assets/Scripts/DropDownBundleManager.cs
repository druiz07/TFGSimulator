using UnityEngine;
using UnityEngine.UI;

public class DropDownBundleManager : MonoBehaviour
{
    public Dropdown dropdown;
    private GameObject currentPrefabInstance;
    private const string SelectedPrefabNameKey = "SelectedPrefabName";
    private float scaleoffset = 20.0f;

    private void Start()
    {
        // Deshabilitar la interacci�n del Dropdown al inicio
        dropdown.interactable = true;
        // Cargar el primer elemento del Dropdown una vez que se han completado los Asset Bundles
        dropdown.value = 0;
        // Llamar al m�todo OnDropdownValueChanged para instanciar el prefab
        OnDropdownValueChanged(0);

        // Asigna la funci�n al evento OnValueChanged
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);


    }

    private void Update()
    {
        // Si hay un objeto instanciado, rotarlo continuamente
        if (currentPrefabInstance != null)
        {
            currentPrefabInstance.transform.Rotate(Vector3.up * 50f * Time.deltaTime);
        }
    }
    private void OnDropdownValueChanged(int index)
    {
        // Destruye la instancia actual si existe
        DestroyCurrentInstance();

        // Obt�n el nombre del prefab seg�n la opci�n seleccionada en el Dropdown
        string selectedPrefabName = GetSelectedPrefabName(index);

        // Obt�n el prefab del ResourceManager
        GameObject selectedPrefab = ResourceManagerSingleton.ResourceManagerInstance.GetPrefab(selectedPrefabName);
        ResourceManagerSingleton.ResourceManagerInstance.setShipPrefab(selectedPrefab);

        // Instancia el prefab si se encontr�
 
        if (selectedPrefab != null)
        {
            Vector3 scale = Vector3.one * scaleoffset;
            Quaternion rotation = Quaternion.identity; // Sin rotaci�n inicial

            Vector3 position = Vector3.zero;
            position.x += scaleoffset / 4;
            // Instancia el prefab con la escala, rotaci�n y posici�n definidas
            currentPrefabInstance = Instantiate(selectedPrefab, position, rotation);
            currentPrefabInstance.transform.localScale = currentPrefabInstance.transform.localScale + scale; // Aplicar escala
        }
    }

    private void OnDestroy()
    {
        // Almacenar el nombre del prefab seleccionado antes de destruir el objeto
        string selectedPrefabName = GetSelectedPrefabName(dropdown.value);
        PlayerPrefs.SetString(SelectedPrefabNameKey, selectedPrefabName);
        PlayerPrefs.Save();
    }
    private void DestroyCurrentInstance()
    {
        // Destruye la instancia actual si existe
        if (currentPrefabInstance != null)
        {
            Destroy(currentPrefabInstance);
        }
    }

    private string GetSelectedPrefabName(int index)
    {
        // Asocia el �ndice del Dropdown con el nombre del prefab
        switch (index)
        {
            case 0: // vikingLander
                return "VikingLander";
            case 1: // Lunar Module
                return "LunarModule";
            
            default:
                return "";
        }
    }


}
