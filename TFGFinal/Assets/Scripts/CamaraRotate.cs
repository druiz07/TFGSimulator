using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraRotate : MonoBehaviour
{
    // Velocidad de rotaci�n en grados por segundo
    public float rotationSpeed = 45.0f;

    void Update()
    {
        // Calcula la cantidad de rotaci�n en este frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Aplica la rotaci�n a la c�mara alrededor del eje Y
        transform.Rotate(0, rotationAmount, 0, Space.World);
    }
}
