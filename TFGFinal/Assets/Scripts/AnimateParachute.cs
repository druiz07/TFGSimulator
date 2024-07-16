using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimateParachute : MonoBehaviour
{
    public GameObject objetoAScalar;
    public float duracionAnimacion = 2.5f;
    public UnityEvent onAnimationComplete;


    private Vector3 escalaInicial;
    private Vector3 escalaFinal;
    private bool animacionIniciada = false;
    private float tiempoInicioAnimacion;
    private bool subiendo = false;
    public float velocidadSubida = 5.0f; // Velocidad de subida del paraca�das
    public float alturaMaxima = 40000.0f; // Altura m�xima a la que subir� el paraca�das
    bool canActivate = true;

    void Start()
    {
        // Guardamos la escala inicial del objeto
        escalaInicial = objetoAScalar.transform.localScale;
        objetoAScalar.SetActive(false);
        // Calculamos la escala final deseada (igualar x, y, z a la escala y inicial)
        escalaFinal = new Vector3(escalaInicial.y, escalaInicial.y, escalaInicial.y);
        // Suscribirse al evento de aterrizaje exitoso
        FindObjectOfType<Rocket>().onSuccessfulLanding.AddListener(MoverParacaidas);

        if (ResourceManagerSingleton.ResourceManagerInstance.getPlanetdesition().ToLower() == "moon") canActivate = false;

    }
    // M�todo para mover el paraca�das hacia arriba
    void MoverParacaidas()
    {
        subiendo = true;
    }
    void Update()
    {
        if (canActivate)
        {
            // Comprobamos si se ha presionado la tecla H y la animaci�n no ha sido iniciada a�n
            if (Input.GetKey(KeyCode.P) && !animacionIniciada)
            {
                // Activamos el objeto si no est� activado
                objetoAScalar.SetActive(true);

                // Iniciamos la animaci�n
                animacionIniciada = true;
                tiempoInicioAnimacion = Time.time;
                AudioManager.Instance.PlaySFX("Parachute");

            }

            // Si la animaci�n ha sido iniciada
            if (animacionIniciada)
            {
                // Calculamos el progreso de la animaci�n
                float progreso = (Time.time - tiempoInicioAnimacion) / duracionAnimacion;
                progreso = Mathf.Clamp01(progreso); // Aseguramos que est� entre 0 y 1

                // Aplicamos la interpolaci�n lineal entre la escala inicial y final
                objetoAScalar.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);

                // Si la animaci�n ha terminado
                if (progreso == 1f)
                {
                    // Invocamos el evento onAnimationComplete
                    onAnimationComplete.Invoke();
                }
            }
            if (subiendo)
            {
                // Calculamos el nuevo valor de posici�n en Y del paraca�das
                float nuevaAltura = objetoAScalar.transform.position.y + velocidadSubida;

                // Limitamos la altura m�xima
                nuevaAltura = Mathf.Min(nuevaAltura, alturaMaxima);

                // Asignamos la nueva posici�n al paraca�das
                objetoAScalar.transform.position = new Vector3(objetoAScalar.transform.position.x, nuevaAltura, objetoAScalar.transform.position.z);

                // Si hemos alcanzado la altura m�xima, detenemos el movimiento y activamos el evento onAnimationComplete
                if (nuevaAltura >= alturaMaxima)
                {
                    subiendo = false;

                }
            }
        }


    }
}
