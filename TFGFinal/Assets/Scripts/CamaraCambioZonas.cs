using UnityEngine;

public class CamaraCambioZonas : MonoBehaviour
{
    public Camera camaraPrincipalOriginal; // Para almacenar la cámara principal original antes de cambiarla
    public GameObject spaceshipgo;
    public Transform spaceshipt;
    public Vector3 posNaveSiBaja;
    public Vector3 posNaveSiSube;
    public Rocket scriptRocket;
    public string zonaCheck;
    private void FixedUpdate()
    {
        checkPos();
    }
    private void checkPos()
    {
        if (spaceshipgo != null && spaceshipt != null)
        {

            if (spaceshipgo.GetComponent<Rigidbody>() != null) // Verificar si el otro objeto tiene un componente Rigidbody
            {
                Rigidbody otherRigidbody = spaceshipgo.GetComponent<Rigidbody>();

                if (spaceshipt.position.y <= this.transform.position.y)
                {
                    if (otherRigidbody.velocity.y < 0 && scriptRocket.ObtenerZonaActual() == zonaCheck)
                    {
                        camaraPrincipalOriginal.transform.position = posNaveSiBaja;
                    }
                 }
                else if (spaceshipt.position.y > this.transform.position.y)
                {
                    if (otherRigidbody.velocity.y > 0 && scriptRocket.ObtenerZonaActual() == zonaCheck)
                    {
                        camaraPrincipalOriginal.transform.position = posNaveSiSube;
                    }
                }
                   
            }
        }
    }
}