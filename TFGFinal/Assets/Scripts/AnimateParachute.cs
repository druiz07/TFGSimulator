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
    public float velocidadSubida = 5.0f; // Velocidad de subida del paracaídas
    public float alturaMaxima = 40000.0f; // Altura máxima a la que subirá el paracaídas
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
    // Método para mover el paracaídas hacia arriba
    void MoverParacaidas()
    {
        subiendo = true;
    }
    void Update()
    {
        if (canActivate)
        {
            // Comprobamos si se ha presionado la tecla H y la animación no ha sido iniciada aún
            if (Input.GetKey(KeyCode.P) && !animacionIniciada)
            {
                // Activamos el objeto si no está activado
                objetoAScalar.SetActive(true);

                // Iniciamos la animación
                animacionIniciada = true;
                tiempoInicioAnimacion = Time.time;
                AudioManager.Instance.PlaySFX("Parachute");

            }

            // Si la animación ha sido iniciada
            if (animacionIniciada)
            {
                // Calculamos el progreso de la animación
                float progreso = (Time.time - tiempoInicioAnimacion) / duracionAnimacion;
                progreso = Mathf.Clamp01(progreso); // Aseguramos que esté entre 0 y 1

                // Aplicamos la interpolación lineal entre la escala inicial y final
                objetoAScalar.transform.localScale = Vector3.Lerp(escalaInicial, escalaFinal, progreso);

                // Si la animación ha terminado
                if (progreso == 1f)
                {
                    // Invocamos el evento onAnimationComplete
                    onAnimationComplete.Invoke();
                }
            }
            if (subiendo)
            {
                // Calculamos el nuevo valor de posición en Y del paracaídas
                float nuevaAltura = objetoAScalar.transform.position.y + velocidadSubida;

                // Limitamos la altura máxima
                nuevaAltura = Mathf.Min(nuevaAltura, alturaMaxima);

                // Asignamos la nueva posición al paracaídas
                objetoAScalar.transform.position = new Vector3(objetoAScalar.transform.position.x, nuevaAltura, objetoAScalar.transform.position.z);

                // Si hemos alcanzado la altura máxima, detenemos el movimiento y activamos el evento onAnimationComplete
                if (nuevaAltura >= alturaMaxima)
                {
                    subiendo = false;

                }
            }
        }


    }
}
