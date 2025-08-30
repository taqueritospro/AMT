using UnityEngine;
using System.Collections;

public class CJefe : MonoBehaviour
{
    // Parámetros de Vida.
    public int vidaMaximaJefe = 10000;
    public int vidaActualJefe;

    // Parámetros de Movimiento.
    public float velocidadMovimientoJefe = 2f;
    public bool limitarMovimientoJefe = true;
    public Vector2 areaMinJefe = new Vector2(-10f, 1.5f);
    public Vector2 areaMaxJefe = new Vector2(5f, 4.5f);

    // Parámetros de Disparo.
    public GameObject balaJefe;
    public Transform[] puntosDisparoJefe;
    public float intervalDisparoJefe = 1f;
    public float velocidadBalaJefe = 8f;

    // Patrones de Disparo.
    public bool usarPatronEspiral = true;
    public bool usarPatronFlor = true;
    public bool usarPatronRayo = true;

    // Parámetros para ecuaciones matemáticas.
    public int numeroBalasPatron = 12;
    public float velocidadRotacionPatron = 2f;
    public float amplitudPatron = 3f;
    public float frecuenciaPatron = 2f;

    // Referencias.
    public Camera camaraJuego;
    public Transform jugadorTarget;

    // Efectos.
    public GameObject efectoImpactoJefe;
    public GameObject efectoMuerteJefe;

    // Variables privadas.
    private float tiempoUltimoDisparoJefe;
    private bool estaMuertoJefe = false;
    private Vector3 puntoInicialJefe;
    private Vector3 destinoMovimientoJefe;
    private float tiempoCambioDestino = 0f;
    private float intervaloCambioDestino = 0.75f;
    private Vector2 limitesMovimientoJefe;
    private int patronDisparoActual = 0;
    private float tiempoCambioPatron = 0f;
    private float intervaloCambioPatron = 4f;
    private float tiempoPatron = 0f;

    // Eventos.
    public System.Action<int> OnVidaCambiadaJefe;
    public System.Action OnJefeMuerto;

    void Start()
    {
        // Inicializar vida.
        vidaActualJefe = vidaMaximaJefe;

        // Configurar cámara.
        camaraJuego = Camera.main;

        // Guardar posición inicial.
        puntoInicialJefe = transform.position;
        destinoMovimientoJefe = puntoInicialJefe;

        // Calcular límites de movimiento.
        calcularLimitesMovimientoJefe();

        // Configurar puntos de disparo si no están asignados.
        if (puntosDisparoJefe == null || puntosDisparoJefe.Length == 0)
        {
            puntosDisparoJefe = new Transform[] { transform };
        }

        // Notificar vida inicial.
        OnVidaCambiadaJefe?.Invoke(vidaActualJefe);
    }

    void Update()
    {
        if (estaMuertoJefe) return;

        movimientoJefe();
        disparoJefe();
        cambiarPatronDisparo();
        tiempoPatron += Time.deltaTime;
    }

    void movimientoJefe()
    {
        // Cambiar destino periódicamente.
        if (Time.time >= tiempoCambioDestino + intervaloCambioDestino)
        {
            generarNuevoDestino();
            tiempoCambioDestino = Time.time;
        }

        // Mover hacia el destino.
        transform.position = Vector3.MoveTowards(transform.position, destinoMovimientoJefe, velocidadMovimientoJefe * Time.deltaTime);

        // Si llegó al destino, generar uno nuevo.
        if (Vector3.Distance(transform.position, destinoMovimientoJefe) < 0.1f)
        {
            generarNuevoDestino();
        }
    }

    void generarNuevoDestino()
    {
        // Generar posición aleatoria dentro de los límites.
        float nuevaX = Random.Range(areaMinJefe.x, areaMaxJefe.x);
        float nuevaY = Random.Range(areaMinJefe.y, areaMaxJefe.y);

        destinoMovimientoJefe = new Vector3(nuevaX, nuevaY, transform.position.z);

        // Asegurar que está dentro de los límites.
        if (limitarMovimientoJefe)
        {
            destinoMovimientoJefe.x = Mathf.Clamp(destinoMovimientoJefe.x, areaMinJefe.x, areaMaxJefe.x);
            destinoMovimientoJefe.y = Mathf.Clamp(destinoMovimientoJefe.y, areaMinJefe.y, areaMaxJefe.y);
        }
    }

    void disparoJefe()
    {
        // Verificar si puede disparar.
        bool puedeDisparar = Time.time >= tiempoUltimoDisparoJefe + intervalDisparoJefe;

        if (puedeDisparar)
        {
            ejecutarPatronDisparo();
            tiempoUltimoDisparoJefe = Time.time;
        }
    }

    void ejecutarPatronDisparo()
    {
        switch (patronDisparoActual)
        {
            case 0:
                if (usarPatronEspiral) disparoEspiral();
                break;
            case 1:
                if (usarPatronFlor) disparoFlor();
                break;
            case 2:
                if (usarPatronRayo) disparoRayo();
                break;
        }
    }

    // PATRÓN 1: Espiral de Arquímedes.
    void disparoEspiral()
    {
        Transform puntoDisparo = puntosDisparoJefe[0];
        float incrementoAngulo = 360f / numeroBalasPatron;

        for (int i = 0; i < numeroBalasPatron; i++)
        {
            float angulo = i * incrementoAngulo + tiempoPatron * velocidadRotacionPatron * 50f;
            float theta = angulo * Mathf.Deg2Rad;
            float radio = amplitudPatron * theta * 0.15f;

            // Posición en el espiral.
            Vector3 posicionEspiral = new Vector3(
                radio * Mathf.Cos(theta),
                radio * Mathf.Sin(theta),
                0
            );

            // Dirección tangente al espiral.
            Vector3 direccion = new Vector3(
                Mathf.Cos(theta + Mathf.PI / 2),
                Mathf.Sin(theta + Mathf.PI / 2),
                0
            ).normalized;

            crearBalaJefe(puntoDisparo.position, direccion);
        }
    }

    // PATRÓN 2: Epitrocoide (Flor).
    void disparoFlor()
    {
        Transform puntoDisparo = puntosDisparoJefe[0];
        float incrementoT = 2f * Mathf.PI / numeroBalasPatron;
        float R = amplitudPatron * 0.8f; // Radio del círculo fijo.
        float r = amplitudPatron * 0.3f; // Radio del círculo rodante.
        float d = amplitudPatron * 0.5f; // Distancia del centro del círculo rodante al punto.

        for (int i = 0; i < numeroBalasPatron; i++)
        {
            float t = i * incrementoT + tiempoPatron * 1.8f;

            // Ecuación del epitrocoide.
            float factorT = (R + r) / r * t;
            float x = (R + r) * Mathf.Cos(t) - d * Mathf.Cos(factorT);
            float y = (R + r) * Mathf.Sin(t) - d * Mathf.Sin(factorT);

            Vector3 direccion = new Vector3(x, y, 0).normalized;

            crearBalaJefe(puntoDisparo.position, direccion);
        }
    }

    // PATRÓN 3: Rayo (Zigzag).
    void disparoRayo()
    {
        Transform puntoDisparo = puntosDisparoJefe[0];

        // Dirección principal hacia el jugador (si existe) o hacia abajo.
        Vector3 direccionPrincipal = Vector3.down;
        if (jugadorTarget != null)
        {
            direccionPrincipal = (jugadorTarget.position - puntoDisparo.position).normalized;
        }

        // Crear múltiples segmentos de rayo.
        int segmentosRayo = 8;
        float longitudSegmento = amplitudPatron * 0.5f;
        float desviacionMaxima = amplitudPatron * 0.8f;

        Vector3 posicionActual = puntoDisparo.position;
        Vector3 direccionActual = direccionPrincipal;

        for (int i = 0; i < segmentosRayo; i++)
        {
            // Añadir desviación aleatoria al rayo para hacerlo zigzag.
            float desviacionX = Random.Range(-desviacionMaxima, desviacionMaxima);
            float desviacionY = Random.Range(-desviacionMaxima * 0.3f, desviacionMaxima * 0.3f);

            Vector3 desviacion = new Vector3(desviacionX, desviacionY, 0);
            Vector3 direccionSegmento = (direccionPrincipal + desviacion * (0.5f + Mathf.Sin(tiempoPatron * 10f + i) * 0.3f)).normalized;

            // Crear bala en la posición actual con la dirección del segmento.
            crearBalaJefe(posicionActual, direccionSegmento);

            // Calcular siguiente posición.
            posicionActual += direccionSegmento * longitudSegmento;
        }
    }

    void crearBalaJefe(Vector3 posicion, Vector3 direccion)
    {
        if (balaJefe == null) return;

        // Calcular rotación basada en la dirección
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        Quaternion rotacion = Quaternion.AngleAxis(angulo - 90f, Vector3.forward);

        // Crear la bala.
        GameObject bala = Instantiate(balaJefe, posicion, rotacion);

        // Configurar velocidad.
        Rigidbody2D rbBala = bala.GetComponent<Rigidbody2D>();
        if (rbBala != null)
        {
            rbBala.linearVelocity = direccion * velocidadBalaJefe;
        }

        // Destruir después de un tiempo.
        Destroy(bala, 6f);
    }

    void cambiarPatronDisparo()
    {
        if (Time.time >= tiempoCambioPatron + intervaloCambioPatron)
        {
            // Cambiar al siguiente patrón disponible.
            do
            {
                patronDisparoActual = (patronDisparoActual + 1) % 3;
            }
            while (!patronDisponible(patronDisparoActual));

            tiempoCambioPatron = Time.time;
            tiempoPatron = 0f;
            Debug.Log("Cambiando a patrón de disparo: " + patronDisparoActual);
        }
    }

    bool patronDisponible(int patron)
    {
        switch (patron)
        {
            case 0: return usarPatronEspiral;
            case 1: return usarPatronFlor;
            case 2: return usarPatronRayo;
            default: return false;
        }
    }

    public void recibirPuntos(int cantidad)
    {
        if (estaMuertoJefe) return;

        vidaActualJefe -= cantidad;
        vidaActualJefe = Mathf.Max(0, vidaActualJefe);

        // Crear efecto de impacto.
        crearEfectoImpactoJefe();

        // Notificar cambio de vida.
        OnVidaCambiadaJefe?.Invoke(vidaActualJefe);

        // Verificar si murió.
        if (vidaActualJefe <= 0)
        {
            morirJefe();
        }
        else
        {
            // Efecto visual de daño.
            StartCoroutine(efectoGolpe());
        }
    }

    void crearEfectoImpactoJefe()
    {
        if (efectoImpactoJefe != null)
        {
            GameObject efecto = Instantiate(efectoImpactoJefe, transform.position, Quaternion.identity);
            Destroy(efecto, 1f);
        }
    }

    System.Collections.IEnumerator efectoGolpe()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color colorOriginal = sr.color;
            sr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sr.color = colorOriginal;
        }
    }

    void morirJefe()
    {
        estaMuertoJefe = true;

        // Crear efecto de muerte.
        if (efectoMuerteJefe != null)
        {
            GameObject efecto = Instantiate(efectoMuerteJefe, transform.position, Quaternion.identity);
            Destroy(efecto, 2f);
        }

        // Notificar muerte.
        OnJefeMuerto?.Invoke();
        Debug.Log("¡El jefe ha sido derrotado!");

        // Destruir jefe.
        gameObject.SetActive(false);
    }

    void calcularLimitesMovimientoJefe()
    {
        if (camaraJuego == null) return;

        // Obtener límites de la cámara.
        float distancia = Vector3.Distance(transform.position, camaraJuego.transform.position);
        Vector2 limites = camaraJuego.ViewportToWorldPoint(new Vector3(1, 1, distancia));

        // Ajustar límites con margen.
        float margen = 1f;
        limitesMovimientoJefe.x = limites.x - margen;
        limitesMovimientoJefe.y = limites.y - margen;
    }

    // Método para obtener estado del jefe.
    public bool estadoJefe()
    {
        return estaMuertoJefe;
    }

    // Método para obtener porcentaje de vida.
    public float obtenerPorcentajeVidaJefe()
    {
        return (float)vidaActualJefe / (float)vidaMaximaJefe;
    }
}