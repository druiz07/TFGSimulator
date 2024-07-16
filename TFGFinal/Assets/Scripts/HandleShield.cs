using UnityEngine;
using UnityEngine.Events;


public class HandleShield : MonoBehaviour
{
    private Rigidbody shieldRigidbody; // Referencia al Rigidbody del escudo térmico
    float upwardForce = 180f; // Ajusta según sea necesario
    [SerializeField] private ParticleSystem upParticles; // Partículas a activar cuando el objeto sube
    public UnityEvent onShieldDetached; // Evento que se activará cuando se despegue el escudo térmico


    void Start()
    {
        // Obtener o añadir un Rigidbody al objeto
        shieldRigidbody = GetComponent<Rigidbody>();
        if (shieldRigidbody == null)
        {
            // Si el objeto no tiene Rigidbody, añadir uno
            shieldRigidbody = gameObject.AddComponent<Rigidbody>();
            shieldRigidbody.isKinematic = true; // Hacer el Rigidbody cinemático para que no sea afectado por la física
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

                // Hacer que el Rigidbody sea dinámico para que pueda moverse
                shieldRigidbody.isKinematic = false;

                // Aplicar una fuerza hacia arriba al objeto para hacerlo subir
              
                shieldRigidbody.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                // Activar las partículas
                if (upParticles != null)
                {
                    // Instanciar las partículas
                    Vector3 offsetPos = new Vector3(0, -2.5f, 0);
                    ParticleSystem upParticlesIns = Instantiate(upParticles, transform.position+ offsetPos, Quaternion.identity);
                    // Establecer el objeto escudo térmico como padre de las partículas
                    upParticlesIns.transform.parent = transform;
                    upParticlesIns.transform.localScale *= 4.5f; 
                    // Iniciar las partículas
                    upParticlesIns.Play();
                }
                else
                {
                    Debug.LogWarning("No se ha asignado un prefab de sistema de partículas para instanciar.");
                }
                // Activar el evento de desprendimiento del escudo térmico
                onShieldDetached.Invoke();
            }
            else
            {
                Debug.LogWarning("El objeto ya está desasociado.");
            }
        }
    }
}
