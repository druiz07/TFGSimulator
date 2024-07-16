using UnityEngine;
using UnityEngine.UI;

public class DropDownBundleManagerCloud: MonoBehaviour
{
    public Dropdown dropdown;
    private GameObject currentPrefabInstance;

    private void Start()
    {
        // Deshabilitar la interacción del Dropdown al inicio
        dropdown.interactable = true;
        // Cargar el primer elemento del Dropdown una vez completado los Asset Bundles
        dropdown.value = 0;
        // Llama al método OnDropdownValueChanged para instanciar el prefab
        OnDropdownValueChanged(0);

        // Asignar la función al evento del dropdown
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);


    }

    private void OnDropdownValueChanged(int index)
    {
        // Destruye la instancia actual si existe
        DestroyCurrentInstance();

        // nombre del prefab según la opción seleccionada en el Dropdown
        string selectedPrefabName = GetSelectedPrefabName(index);

        //  prefab del ResourceManager
        GameObject selectedPrefab = ResourceManagerSingleton.ResourceManagerInstance.GetPrefab(selectedPrefabName);
        ResourceManagerSingleton.ResourceManagerInstance.setShipPrefab(selectedPrefab);

        // Instanciar el prefab si se encuentra
        if (selectedPrefab != null)
        {
            if (index == 0)
            {
                Vector3 pos = new Vector3(0, 0, 10);
                currentPrefabInstance = Instantiate(selectedPrefab, pos, Quaternion.identity);
                Vector3 miVector = new Vector3(-180.0f, 0.0f, 0.0f);
                currentPrefabInstance.transform.Rotate(miVector);

            }
            else
            {
                currentPrefabInstance = Instantiate(selectedPrefab, Vector3.zero, Quaternion.identity);
            }
        }
    }

    private void DestroyCurrentInstance()
    {
        // Destruir la instancia si existe
        if (currentPrefabInstance != null)
        {
            Destroy(currentPrefabInstance);
        }
    }

    private string GetSelectedPrefabName(int index)
    {
        // Asociacion del índice del Dropdown con el nombre del prefab
        switch (index)
        {
            case 0: // vikingLander
                return "vikinglander";
            case 1: // Lunar Module
                return "LunarModule";
          //Aqui irian mas opciones
            default:
                return "";
        }
    }


}
