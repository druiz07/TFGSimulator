using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraRotate : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float rotationSpeed = 45.0f;

    void Update()
    {
        // Calcula la cantidad de rotación en este frame
        float rotationAmount = rotationSpeed * Time.deltaTime;

        // Aplica la rotación a la cámara alrededor del eje Y
        transform.Rotate(0, rotationAmount, 0, Space.World);
    }
}
