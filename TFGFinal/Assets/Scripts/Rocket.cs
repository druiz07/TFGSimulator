using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Necesario para usar UI elements en Unity
using System.IO;
using UnityEngine.Events;

public class Rocket : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float thrustForce;
    [SerializeField]
    private float scaleForceUser;
    [SerializeField]
    private float groundRayDistance;
    [SerializeField]
    private float PlanetGravity;
    [SerializeField]
    private float offsetGravityRED = 1.8f;
    [SerializeField]
    private float windForce;
    [SerializeField]
    private float propulsorActivationHeight;
    [SerializeField]
    private float maxVelocityforLanding;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Transform upPosAtmospheere;
    [SerializeField]
    private ParticleSystem explosionParticles; // 
    [SerializeField]
    private ParticleSystem thrustParticles; // 
    [SerializeField]
    private Text infoText; // Objeto de texto para mostrar la velocidad
    [SerializeField]
    private Text warningText; // Objeto de texto para mostrar la velocidad
    [SerializeField]
    private float windMinTime;
    [SerializeField]
    private float windMaxTime;
    [SerializeField]
    private Collider floor;
    [SerializeField]
    private float parachuteThrustMultiplier; // Multiplicador de la fuerza del propulsor cuando el paracaídas está activado
    [SerializeField]
    private float parachuteGravityReduction; // Reducción de la gravedad cuando el paracaídas está activado

    private float lastPlanet = 2;

    [SerializeField]
    private float massPercentageDecrease; //Porcentaje de reduccion de peso segun gastas combustible
    [SerializeField]
    private float fuelConsumptionAmount; // Cantidad de combustible consumido por cada uso del propulsor
    private RocketAttributes rocketAttributes;
    private RocketMass rocketMass;

    private float scalewindforce = 6.0f;
    private bool hasLandedSuccessfully = false; // Variable para controlar si la nave ha aterrizado exitosamente


    // Variables para las posiciones de las cámaras
    // Variables para las posiciones de las cámaras
    [SerializeField] private Transform ZONA1;
    [SerializeField] private Transform ZONA2;

    [SerializeField]
    private float temperatureIncreaseRate = 0.5f; // Tasa de aumento de temperatura
    [SerializeField] private float temperatureDecreaseAmount; // Cantidad de temperatura que disminuye cuando se despega el escudo térmico
    float tempMult = 10.0f;

    private float currentTemperature = 150f; // Temperatura inicial en grados Celsius

    public UnityEvent onSuccessfulLanding; // Evento que se activará cuando el objeto aterrice exitosamente


    private float updateInterval = 1f; // Intervalo de actualización 
    private float timeSinceLastUpdate = 0f;
    public GameObject locationOfThrust;

    ParticleSystem propulsorPart = null;
    private float fuelAmount = 1000f; // Cantidad inicial de combustible
    private float initialMass; // Masa inicial del objeto



    private float initialTemperature = 0f;
    private float maxTemperature = 0f;



    private bool noForce = false;
    private bool hasCollided = false;
    private bool haspressedThrust = false;
    private bool hasParachute = false;
    private bool useofThrust = true;
    private float velocityActual = 0;

    [SerializeField] private HandleShield shieldScript; // Referencia al script HandleShield


    private Vector3 thrustDir = Vector3.zero;
    // bool propulsorActive;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialMass = rb.mass;
        // Carga los atributos del Rocket desde el archivo JSON al inicio
        SetPlanetTemperature(); // Llamar a la función para ajustar la temperatura inicial y máxima según el planeta
        LoadRocketAttributes();
        // Suscribirse al evento de desprendimiento del escudo térmico
        if (shieldScript != null)
        {
            shieldScript.onShieldDetached.AddListener(DecreaseTemperature);
        }


    }
    private void SetPlanetTemperature()
    {
        string currentPlanet = ResourceManagerSingleton.ResourceManagerInstance.getPlanetdesition();
        switch (currentPlanet)
        {
            case "mars":
                initialTemperature = -100f;
                maxTemperature = 150f;
                tempMult = 75.0f;
                temperatureDecreaseAmount = 65.0f;
                break;
            case "moon":
                initialTemperature = -50f;
                temperatureDecreaseAmount = 40.0f;
                maxTemperature = 90.0f;
                tempMult = 25.0f;
                break;
            default:
                initialTemperature = 0f;
                maxTemperature = 350f;
                temperatureDecreaseAmount = 85.0f;
                tempMult = 45.0f;
                break;
        }
        currentTemperature = initialTemperature; // Establecer la temperatura inicial al inicio
    }
    private void DecreaseTemperature()
    {
        // Disminuir la temperatura de la nave
        currentTemperature -= temperatureDecreaseAmount;



        // Actualizar el texto con la información de la temperatura
        UpdateVelocityText();

        // Verificar si la temperatura ha alcanzado el límite máximo y destruir la nave si no hay escudo térmico
        if (currentTemperature >= maxTemperature && !hasParachute)
        {
            AudioManager.Instance.PlaySFX("ExplosionShip");
            ShowWarningMessage("La nave ha sufrido daños críticos debido al sobrecalentamiento.");
            CreateExplosion();
        }
    }
    private void AssignAttributeValues()
    {
        if (rocketAttributes != null)
        {
            // Asignar los valores de los sliders a las variables correspondientes
            rocketAttributes.maxVelocityforLanding = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("maxVelocityforLanding");
            rocketAttributes.PlanetGravity = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("PlanetGravity");
            rocketAttributes.scaleForceUser = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("scaleForceUser");
            rocketAttributes.windForce = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("windForce");
            rocketAttributes.fuelConsumptionAmount = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("fuelConsumptionAmount");
            rocketAttributes.thrustForce = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("thrustForce");


            maxVelocityforLanding = rocketAttributes.maxVelocityforLanding;
            PlanetGravity = rocketAttributes.PlanetGravity;
            windForce = rocketAttributes.windForce;
            fuelConsumptionAmount = rocketAttributes.fuelConsumptionAmount;
            scaleForceUser = rocketAttributes.scaleForceUser;

        }
    }

    private void LoadRocketAttributes()
    {



        // Cargar los atributos del cohete desde PlayerPrefs
        string jsonContent = PlayerPrefs.GetString("RocketAttributes");
        if (!string.IsNullOrEmpty(jsonContent))
        {
            rocketAttributes = JsonUtility.FromJson<RocketAttributes>(jsonContent);
        }
        else
        {
            // Si no hay datos guardados, crear nuevos atributos de cohete
            rocketAttributes = new RocketAttributes();
        }
        // Ajusta las variables del script con los atributos cargados
        if (rocketAttributes != null && ResourceManagerSingleton.ResourceManagerInstance.getIndxPlanetDesition() == lastPlanet)
        {
            thrustForce = rocketAttributes.thrustForce;
            scaleForceUser = rocketAttributes.scaleForceUser;
            groundRayDistance = rocketAttributes.groundRayDistance;
            PlanetGravity = rocketAttributes.PlanetGravity;
            windForce = rocketAttributes.windForce;
            propulsorActivationHeight = rocketAttributes.propulsorActivationHeight;
            maxVelocityforLanding = rocketAttributes.maxVelocityforLanding;
            windMinTime = rocketAttributes.windMinTime;
            windMaxTime = rocketAttributes.windMaxTime;
            parachuteThrustMultiplier = rocketAttributes.parachuteThrustMultiplier;
            parachuteGravityReduction = rocketAttributes.parachuteGravityReduction;
            massPercentageDecrease = rocketAttributes.massPercentageDecrease;
            fuelConsumptionAmount = rocketAttributes.fuelConsumptionAmount;
            rocketAttributes.fuelAmount = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("fuelAmount");
            fuelAmount = rocketAttributes.fuelAmount;
        }
        else
        {
            // Lee el archivo JSON correspondiente y actualiza los atributos del cohete
            string fileName = "rocket_attributes_" + ResourceManagerSingleton.ResourceManagerInstance.getPlanetdesition() + ".json";
            string filePath = Path.Combine(Application.dataPath, "Resources", "jsons", fileName);

            if (File.Exists(filePath))
            {
                // Lee el contenido del archivo JSON
                string jsonContentFromFile = File.ReadAllText(filePath);

                // Actualiza los atributos del cohete con el contenido del archivo JSON
                rocketAttributes = JsonUtility.FromJson<RocketAttributes>(jsonContentFromFile);


                //IGUALAR LOS ATRIBUTOS
                rocketAttributes.thrustForce = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("thrustForce");
                rocketAttributes.fuelAmount = ResourceManagerSingleton.ResourceManagerInstance.GetSliderValue("fuelAmount");
                fuelAmount = rocketAttributes.fuelAmount;
                thrustForce = rocketAttributes.thrustForce;
                scaleForceUser = rocketAttributes.scaleForceUser;
                groundRayDistance = rocketAttributes.groundRayDistance;
                PlanetGravity = rocketAttributes.PlanetGravity;
                windForce = rocketAttributes.windForce;
                propulsorActivationHeight = rocketAttributes.propulsorActivationHeight;
                maxVelocityforLanding = rocketAttributes.maxVelocityforLanding;
                windMinTime = rocketAttributes.windMinTime;
                windMaxTime = rocketAttributes.windMaxTime;
                parachuteThrustMultiplier = rocketAttributes.parachuteThrustMultiplier;
                parachuteGravityReduction = rocketAttributes.parachuteGravityReduction;
                massPercentageDecrease = rocketAttributes.massPercentageDecrease;
                fuelConsumptionAmount = rocketAttributes.fuelConsumptionAmount;
            }
            else
            {
                Debug.LogError("No se encontró el archivo JSON: " + fileName);
            }
        }


        asignMass();
        if (ResourceManagerSingleton.ResourceManagerInstance.getIndxPlanetDesition() == lastPlanet) AssignAttributeValues();


    }
    void asignMass()
    {
        rb.mass = ResourceManagerSingleton.ResourceManagerInstance.getShipMass();
        Debug.Log(rb.mass + "\n");
    }
    private void FixedUpdate()
    {
        // Restringir la actualización del texto al intervalo especificado
        timeSinceLastUpdate += Time.fixedDeltaTime;
   
        if (!hasLandedSuccessfully)
        {
            // Incrementar la temperatura en función de la velocidad
            currentTemperature += (temperatureIncreaseRate * Time.fixedDeltaTime) * tempMult;

            if (currentTemperature >= maxTemperature)
            {
                AudioManager.Instance.PlaySFX("ExplosionShip");
                ShowWarningMessage("La nave ha sufrido daños críticos debido al sobrecalentamiento.");
                CreateExplosion();
                return; // Detener la ejecución del FixedUpdate para evitar que se realicen más cálculos innecesarios
            }
        }
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateVelocityText();
            timeSinceLastUpdate = 0f; // Reiniciar el contador
        }

        if (!noForce)
        {
            ApplyGravity();
            ApplyWindForce();
            HandleUserControl();

            if (transform.position.y <= propulsorActivationHeight)
            {
                HandleManualThrust();
            }
        }


        if (!noForce)
        {
            // Actualizar el texto de la velocidad
            UpdateVelocityText();
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.up, out hit, groundRayDistance))
            {
                if (hit.collider == floor)
                {

                    velocityActual = rb.velocity.y;

                }
            }
        }
        if (propulsorPart != null)
        {
            propulsorPart.transform.position = locationOfThrust.transform.position;
        }

        // Verificar si el objeto se sale del campo de visión de la cámara
        if (mainCamera != null)
        {
            Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);
            Vector3 viewportCenter = new Vector3(0.5f, 0.5f, 0); // Centro de la pantalla en coordenadas de vista
            if (viewportPosition.x < 0)
            {
                // El objeto se ha salido por la izquierda
                AudioManager.Instance.PlaySFX("ExplosionShip");
                ShowWarningMessage("Nave fuera de límites: ¡Aterriza dentro del área designada!");

                if (propulsorPart != null)
                {
                    Destroy(propulsorPart.gameObject);
                    propulsorPart = null; // Limpiar la referencia
                }

            }
            else if (viewportPosition.x > 1)
            {
                if (propulsorPart != null)
                {
                    Destroy(propulsorPart.gameObject);
                    propulsorPart = null; // Limpiar la referencia
                }
                // El objeto se ha salido por la derecha
                ShowWarningMessage("Nave fuera de límites: ¡Aterriza dentro del área designada!");
            }
            else if (this.transform.position.y > upPosAtmospheere.position.y + 20)
            {
                if (propulsorPart != null)
                {
                    Destroy(propulsorPart.gameObject);
                    propulsorPart = null; // Limpiar la referencia
                }
                // El objeto se ha salido por arriba
                ShowWarningMessage("Nave fuera de límites: ¡Te has elevado demasiado, aterriza en el área designada!");
            }
        }


    }

    private void ShowWarningMessage(string message)
    {
        // Mostrar el mensaje en el objeto de texto asociado
        warningText.text = message;

        // Destruir la nave
        Destroy(gameObject);
    }
    private void ApplyGravity()
    {
        float effectiveGravity = PlanetGravity / offsetGravityRED;

        if (hasParachute)
        {
            // Calculating the reduced gravity with parachute
            float parachuteReductionFactor = parachuteGravityReduction / 100f;
            float reducedGravity = Mathf.Abs(effectiveGravity) * (1 - parachuteReductionFactor);

            // Applying the reduced gravity force
            Vector3 gravityForce = Vector3.down * reducedGravity * rb.mass;
            rb.AddForce(gravityForce, ForceMode.Acceleration);
        }
        else
        {
            // Applying regular gravity force
            Vector3 gravityForce = Vector3.down * Mathf.Abs(effectiveGravity) * (rb.mass);
            rb.AddForce(gravityForce, ForceMode.Acceleration);
        }
    }


    private void ApplyWindForce()
    {
        if (windForce > 0)
        {
            Vector2 currentWindDirection = Random.insideUnitCircle.normalized; // Dirección inicial del viento (2D)
            float currentWindIntensity = Random.Range(windForce / 1.25f, windForce * scalewindforce); // Intensidad inicial del viento
            if (!hasCollided)
            {
                Vector2 targetWindDirection = Random.insideUnitCircle.normalized; // Dirección objetivo del viento (2D)
                float targetWindIntensity = Random.Range(0f, windForce); // Intensidad objetivo del viento

                float duration = Random.Range(windMinTime, windMaxTime); // Duración de la transición del viento
                float elapsedTime = 0f;

                float turbulenceOffsetX = Random.Range(0f, 100f); // Offset para la turbulencia en X
                float turbulenceOffsetY = Random.Range(0f, 100f); // Offset para la turbulencia en Y

                while (elapsedTime < duration)
                {
                    // Interpolar suavemente entre la dirección e intensidad actual y la dirección e intensidad objetivo
                    currentWindDirection = Vector2.Lerp(currentWindDirection, targetWindDirection, elapsedTime / duration);
                    currentWindIntensity = Mathf.Lerp(currentWindIntensity, targetWindIntensity, elapsedTime / duration);

                    // Añadir turbulencia al viento de manera más constante y menos agresiva
                    float turbulenceX = Mathf.PerlinNoise(turbulenceOffsetX + Time.time * 0.5f, turbulenceOffsetY) * 2f - 1f; // Valores entre -1 y 1
                    float turbulenceY = Mathf.PerlinNoise(turbulenceOffsetX, turbulenceOffsetY + Time.time * 0.5f) * 2f - 1f; // Valores entre -1 y 1
                    float turbulence = (turbulenceX + turbulenceY) * 0.5f; // Promedio de turbulencia en X e Y
                    currentWindIntensity += turbulence * 0.25f; // Ajusta la intensidad de la turbulencia

                    // Aplicar fuerza de viento con la dirección e intensidad interpoladas
                    Vector3 windForceVector = new Vector3(currentWindDirection.x, 0, 0f) * currentWindIntensity; // Convertir a 3D (X, Y, 0)
                    rb.AddForce(windForceVector, ForceMode.Acceleration);

                    elapsedTime += Time.deltaTime;
                }
            }


        }

    }

    // Método para obtener la zona actual
    public string ObtenerZonaActual()
    {

        if (transform.position.y > ZONA2.position.y && transform.position.y > ZONA1.position.y)
        {
            return "Zona 1";
        }
        else if (transform.position.y > ZONA2.position.y && transform.position.y <= ZONA1.position.y)
        {
            return "Zona 2";
        }
        else if (transform.position.y <= ZONA2.position.y && transform.position.y <= ZONA1.position.y)
        {
            return "Zona 3";
        }
        return "Desconocida";
    }

    private void StopOnGround()
    {
        // Obtener la velocidad en el eje Y y redondear a dos decimales
        float velocityY = Mathf.Round(rb.velocity.y * 100f) / 100f;
        string zonaActual = ObtenerZonaActual(); // Obtener la zona actual
                                                 // Construir el texto que se mostrará en la pantalla

        string velocityInfo = "Velocidad (M/S):   " + Mathf.Abs(velocityY) + "\n" +
                              "Velocidad Maxima de aterrizaje al planeta:   " + maxVelocityforLanding + " \n" +
                              "Gravedad del Planeta:   " + PlanetGravity + " m/s²\n" +
                              "Dirección del Viento:   " + GetWindDirection() + "\n" +
                              "Fuerza del propulsor:   " + thrustForce + "\n" +
                              "Cantidad de Combustible:   " + (fuelAmount > 0 ? fuelAmount.ToString() : "¡Sin Combustible!") + "\n" +
                              "Masa del Objeto:   " + rb.mass + " TON" + "\n" +
                              "Zona Actual:   " + zonaActual;

        // Actualizar el texto en el objeto UI
        infoText.text = velocityInfo;
        rb.Sleep();
        noForce = true;
        // Mostrar el mensaje en el objeto de texto asociado
        warningText.text = "¡Aterrizaje Exitoso!";
        if (!hasLandedSuccessfully)
        {

            AudioManager.Instance.PlaySFX("LandingConfirmed");
        }
        hasLandedSuccessfully = true;
        // Activar el evento de aterrizaje exitoso
        onSuccessfulLanding.Invoke();
    }

    private void HandleUserControl()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 userControlDir = new Vector3(horizontalInput, 0, 0).normalized;
        userControlDir = transform.TransformVector(userControlDir);
        float finalThrustForce = thrustForce;
        if (hasParachute)
        {
            finalThrustForce *= parachuteThrustMultiplier; // Reducir la fuerza del propulsor si el paracaídas está activado
        }
        if (scaleForceUser > 0) rb.AddForce((userControlDir * finalThrustForce) * scaleForceUser, ForceMode.Acceleration);
    }

    private void HandleManualThrust()
    {


        if (!hasParachute || useofThrust)
        {
            if (Input.GetKey(KeyCode.Space) && fuelAmount > 0)
            {
                haspressedThrust = true;

                rb.AddForce(Vector3.up * thrustForce, ForceMode.Impulse);
                fuelAmount -= fuelConsumptionAmount; // Restar combustible
                UpdateMass(); // Actualizar la masa del objeto
                if (propulsorPart == null && explosionParticles != null)
                {
                    propulsorPart = Instantiate(thrustParticles, locationOfThrust.transform.position, Quaternion.identity);
                    propulsorPart.Play(); // Activar las partículas del propulsor
                    AudioManager.Instance.PlaySFX("Propulsor");
                }
            }
            else
            {

                if (propulsorPart != null)
                {

                    Destroy(propulsorPart.gameObject);
                    propulsorPart = null; // Limpiar la referencia
                }
            }
        }
        else
        {
            // Verificar si el jugador sigue presionando el botón mientras no está usando el propulsor
            if (Input.GetKey(KeyCode.Space))
            {

                if (propulsorPart != null)
                {
                    Destroy(propulsorPart.gameObject);
                    propulsorPart = null; // Limpiar la referencia
                }


            }
        }


    }
    private void CreateExplosion()
    {
        if (explosionParticles != null)
        {
          
            Vector3 pos = transform.position;
          
            if (!haspressedThrust) pos.y += 75;
            else pos.y += 30;
            ParticleSystem explosion = Instantiate(explosionParticles, pos, Quaternion.identity);
            Destroy(explosion.gameObject, explosion.main.duration);
            if (propulsorPart != null)
            {
               
                Destroy(propulsorPart.gameObject);
            }
        }


        Destroy(gameObject); // Destruir la nave después de la explosión
    }
    private void OnDrawGizmos()
    {
        DrawDebugGizmos();
    }

    private void UpdateVelocityText()
    {
        // Obtener la velocidad en el eje Y y redondear a dos decimales
        float velocityY = Mathf.Round(rb.velocity.y * 100f) / 100f;

        string zonaActual = ObtenerZonaActual(); // Obtener la zona actual
                                                 // Construir el texto que se mostrará en la pantalla

        string velocityInfo = "Velocidad (M/S):   " + Mathf.Abs(velocityY) + "\n" +
                              "Velocidad Maxima de aterrizaje al planeta:   " + maxVelocityforLanding + " \n" +
                              "Gravedad del Planeta:   " + PlanetGravity + " m/s²\n" +
                              "Dirección del Viento:   " + GetWindDirection() + "\n" +
                              "Fuerza del propulsor:   " + thrustForce + "\n" +
                              "Cantidad de Combustible:   " + (fuelAmount > 0 ? fuelAmount.ToString() : "¡Sin Combustible!") + "\n" +
                              "Masa del Objeto:   " + rb.mass + " TON" + "\n" +
                              "Zona Actual:   " + zonaActual + "\n" +
                              "Temperatura:   " + currentTemperature.ToString("F1") + "°C\n" +
                              "Temperatura Máxima Antes de Sobrecalentarse:   " + maxTemperature + "°C";

        // Actualizar el texto en el objeto UI
        infoText.text = velocityInfo;
    }


    private string GetWindDirection()
    {
        // Obtener la dirección del viento en base a la velocidad actual del viento
        Vector2 windDirection = rb.velocity.normalized;

        // Determinar la dirección del viento en función de la velocidad
        if (windDirection.x > 0)
        {
            return "Derecha";
        }
        else if (windDirection.x < 0)
        {
            return "Izquierda";
        }
        else
        {
            return "Ninguna (Viento Calmado)";
        }
    }
    private void DrawDebugGizmos()
    {
        // Visualizar el rayo cast
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position - transform.up * groundRayDistance);

        // Visualizar la dirección de la gravedad
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 2);

        // Visualizar la dirección del viento
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(1f, 0f, 0f).normalized * 2);
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            if (!noForce && Mathf.Abs(velocityActual) >= maxVelocityforLanding) // Aterriza bruscamente si la velocidad es alta
            {
                AudioManager.Instance.PlaySFX("ExplosionShip");
                ShowWarningMessage("La nave ha sufrido daños críticos debido al impacto contra el suelo.");
                CreateExplosion(); 

                hasCollided = true;
                if (propulsorPart != null)
                {
                    // Mostrar el mensaje indicando que la nave se ha destruido al chocar contra el suelo

                    Destroy(propulsorPart.gameObject);
                    propulsorPart = null; // Limpiar la referencia
                }

            }
            else StopOnGround();

        }
    }
    private void UpdateMass()
    {
        // Actualizar la masa del objeto basada en el combustible restante
        rb.mass = rb.mass - (initialMass * massPercentageDecrease / 100.0f);

    }
    public void changeHasParachute()
    {
        hasParachute = true;
    }
    public void noUseOFThrust()
    {
        useofThrust = false;
    }
    public void ReleaseAndMoveUp(Transform childTransform)
    {
        // Desasociar el objeto hijo del objeto principal
        childTransform.parent = null;

        // Calcular una posición hacia arriba desde la posición actual del objeto hijo
        Vector3 targetPosition = childTransform.position + Vector3.up * 2f; 

        // Mover el objeto hijo hacia la nueva posición
        childTransform.position = targetPosition;
    }
}
