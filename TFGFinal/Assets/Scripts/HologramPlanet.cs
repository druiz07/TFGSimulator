using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MaterialKeyValuePair
{
    public int key;
    public Material value;
}

public class HologramPlanet : MonoBehaviour
{
    public Dropdown dropdown;
    public Button infoButton;
    public Text infoText;

    public List<MaterialKeyValuePair> materialesPlanetas = new List<MaterialKeyValuePair>();
    [SerializeField]
    private float quantityRotation = 50.0f;

    private bool rotate;
    private string[] planetInfo = {
        "Marte es un planeta rocoso con una gravedad similar a la tierra.\n" +
        "El control de dirección es mínimo, hay leves vientos que sacuden la nave.\n" +
        "La temperatura inicial es de -100 °C, la máxima de 150 °C antes de que la nave se sobrecaliente.\n" +
        "Desprende el escudo térmico para disminuir la temperatura de la nave.\n" +
        "Usa el paracaídas para un mayor control en la nave y disminución de la velocidad.",

        "Luna , el satelite natural de la tierra , muy facil perder la orbita dada su baja gravedad.\n" +
        "El control de dirección hace muy posible perder la direccion de la nave dada la poca fuerza externa y nulos vientos.\n" +
        "La temperatura inicial es de -50 °C, la máxima de 90 °C antes de que la nave se sobrecaliente.\n" +
        "Desprende el escudo térmico para disminuir la temperatura de la nave.\n" +
        "No existe paracaidas y solo se disminuye la velocidad con el uso del propulsor.",

        "Configura este planeta con las características que quieras.\n" +
        "La temperatura inicial es de 0 °C, la máxima de 350 °C antes de que la nave se sobrecaliente.\n" +
        "Desprende el escudo térmico para disminuir la temperatura de la nave.\n" +
        "Usa el paracaídas para un mayor control en la nave y disminución de la velocidad."
    };

    private string currentInfo = ""; // Almacena el texto actual que se está mostrando
    private string targetInfo = ""; // Almacena el texto completo que se desea mostrar
    private int currentIndex = 0; // Índice de la letra actual que se está mostrando
    private float typingSpeed = 0.05f; // Velocidad a la que se mostrarán las letras 

    void Start()
    {
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        infoButton.onClick.AddListener(ShowInfo);
        infoText.gameObject.SetActive(false); 

        // Mostrar información por defecto para el planeta Marte al inicio
        OnDropdownValueChanged(0);
    }

    private void ShowInfo()
    {
        infoText.gameObject.SetActive(!infoText.gameObject.activeSelf); 
    }

    private void OnDropdownValueChanged(int index)
    {
        rotate = false;
        // Verificar si el material asociado al índice está en la lista
        foreach (var kvp in materialesPlanetas)
        {
            if (kvp.key == index)
            {
                // Aplicar el material asociado al objeto
                GetComponent<Renderer>().material = kvp.value;

                // Almacenar el texto completo del planeta seleccionado
                targetInfo = planetInfo[index];

                // mostrar el texto letra por letra
                StartCoroutine(TypeText());
                return;
            }
        }
        // Si no hay material asociado, muestra un mensaje de advertencia
        Debug.LogWarning("No se encontró material para el índice " + index);
    }

    // Método para mostrar el texto letra por letra
    private System.Collections.IEnumerator TypeText()
    {
        // Reinicia las variables
        currentIndex = 0;
        currentInfo = "";

        // Mientras no hayas mostrado todo el texto
        while (currentIndex < targetInfo.Length)
        {
            // Agrega la siguiente letra al texto actual
            currentInfo += targetInfo[currentIndex];
            infoText.text = currentInfo;

            // Aumenta el índice para mostrar la siguiente letra
            currentIndex++;

            // Espera un tiempo antes de mostrar la siguiente letra
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void Update()
    {
        if (rotate) transform.Rotate(Vector3.up * quantityRotation * Time.deltaTime);
        else
        {
            //Devolvemos la rotacion a su punto inicial 
            transform.rotation = Quaternion.identity;
            rotate = true;
        }
    }
}
