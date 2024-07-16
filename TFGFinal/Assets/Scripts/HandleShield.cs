using UnityEngine;
using UnityEngine.Events;


public class HandleShield : MonoBehaviour
{
    private Rigidbody shieldRigidbody; // Referencia al Rigidbody del escudo t�rmico
    float upwardForce = 180f; // Ajusta seg�n sea necesario
    [SerializeField] private ParticleSystem upParticles; // Part�culas a activar cuando el objeto sube
    public UnityEvent onShieldDetached; // Evento que se activar� cuando se despegue el escudo t�rmico


    void Start()
    {
        // Obtener o a�adir un Rigidbody al objeto
        shieldRigidbody = GetComponent<Rigidbody>();
        if (shieldRigidbody == null)
        {
            // Si el objeto no tiene Rigidbody, a�adir uno
            shieldRigidbody = gameObject.AddComponent<Rigidbody>();
            shieldRigidbody.isKinematic = true; // Hacer el Rigidbody cinem�tico para que no sea afectado por la f�sica
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (transform.parent != null)
            {
                // Desasociar el objeto hijo del objeto principal
                transform.parent = null;

                // Hacer que el Rigidbody sea din�mico para que pueda moverse
                shieldRigidbody.isKinematic = false;

                // Aplicar una fuerza hacia arriba al objeto para hacerlo subir
              
                shieldRigidbody.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                // Activar las part�culas
                if (upParticles != null)
                {
                    // Instanciar las part�culas
                    Vector3 offsetPos = new Vector3(0, -2.5f, 0);
                    ParticleSystem upParticlesIns = Instantiate(upParticles, transform.position+ offsetPos, Quaternion.identity);
                    // Establecer el objeto escudo t�rmico como padre de las part�culas
                    upParticlesIns.transform.parent = transform;
                    upParticlesIns.transform.localScale *= 4.5f; 
                    // Iniciar las part�culas
                    upParticlesIns.Play();
                }
                else
                {
                    Debug.LogWarning("No se ha asignado un prefab de sistema de part�culas para instanciar.");
                }
                // Activar el evento de desprendimiento del escudo t�rmico
                onShieldDetached.Invoke();
            }
            else
            {
                Debug.LogWarning("El objeto ya est� desasociado.");
            }
        }
    }
}
