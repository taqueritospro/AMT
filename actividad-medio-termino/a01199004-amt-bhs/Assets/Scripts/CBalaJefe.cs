using UnityEngine;

public class CBalaJefe : MonoBehaviour
{
    // Parámetros de Bala.
    public float velocidadBalaJefe = 8f;
    public int puntoGolpeBalaJefe = 450;
    public float tiempoVidaBalaJefe = 6f;

    // Comportamiento.
    public bool seguirJugador = false;
    public float fuerzaSeguimiento = 2f;
    public bool rotarHaciaMovimiento = true;

    // Efectos.
    public GameObject efectoImpactoBalaJefe;
    public GameObject efectoDestruccionBalaJefe;
    public bool destruirBalaJefeImpacto = true;

    // Patrón Especial.
    public bool usarPatronEspiral = false;
    public float velocidadRotacion = 180f;
    public bool cambiarVelocidadTiempo = false;
    public AnimationCurve curvaVelocidad = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    // Variables privadas.
    private Rigidbody2D rbBalaJefe;
    private bool balaJefeActivada = false;
    private Transform jugadorTarget;
    private Vector3 direccionInicialBalaJefe;
    private float tiempoCreacionBalaJefe;
    private float velocidadInicialBalaJefe;

    void Start()
    {
        // Sumar al contador.
        CContadorBalas.sumarBalaJefe();

        // Obtener componentes.
        rbBalaJefe = GetComponent<Rigidbody2D>();

        // Buscar al jugador para seguimiento.
        GameObject jugador = GameObject.FindGameObjectWithTag("Jugador");
        if (jugadorTarget)
        {
            jugadorTarget = jugador.transform;
        }

        // Guardar valores iniciales.
        direccionInicialBalaJefe = rbBalaJefe.linearVelocity.normalized;
        tiempoCreacionBalaJefe = Time.time;
        velocidadInicialBalaJefe = velocidadBalaJefe;

        // Marcar como activada.
        balaJefeActivada = true;

        // Destruir después del tiempo de vida.
        Destroy(gameObject, tiempoVidaBalaJefe);
    }

    void Update()
    {
        if (!balaJefeActivada) return;

        // Rotar hacia la dirección de movimiento.
        if (rotarHaciaMovimiento && rbBalaJefe != null)
        {
            Vector2 direccion = rbBalaJefe.linearVelocity;
            if (direccion != Vector2.zero)
            {
                float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angulo - 90f, Vector3.forward);
            }
        }

        // Verificar límites de pantalla.
        verificarLimitesPantalla();
    }

    void verificarLimitesPantalla()
    {
        // Verificar si la bala salió de la pantalla.
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 pantallaPos = cam.WorldToViewportPoint(transform.position);

            // Si está fuera de la pantalla.
            if (pantallaPos.x < -0.1f || pantallaPos.x > 1.1f ||
                pantallaPos.y < -0.1f || pantallaPos.y > 1.1f)
            {
                destruirBalaJefe();
            }
        }
    }

    // Detectar colisiones.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!balaJefeActivada) return;

        // Colisión con jugador.
        if (other.CompareTag("Jugador"))
        {
            // Hacer daño al jugador.
            CJugador jugador = other.GetComponent<CJugador>();
            if (jugador != null)
            {
                jugador.recibirPuntos(puntoGolpeBalaJefe);
            }

            // Crear efecto de impacto.
            crearEfectoImpactoBalaJefe();

            // Destruir la bala.
            if (destruirBalaJefeImpacto)
            {
                destruirBalaJefe();
            }
        }

        // Colisión con balas del jugador.
        if (other.CompareTag("BalaJugador"))
        {
            // Crear efecto de impacto.
            crearEfectoImpactoBalaJefe();

            // Destruir esta bala.
            destruirBalaJefe();
        }

        // Colisión con límites del mapa.
        if (other.CompareTag("LimiteMapa"))
        {
            destruirBalaJefe();
        }
    }

    void crearEfectoImpactoBalaJefe()
    {
        if (efectoImpactoBalaJefe != null)
        {
            GameObject efecto = Instantiate(efectoImpactoBalaJefe, transform.position, Quaternion.identity);
            Destroy(efecto, 1f);
        }
    }

    void crearEfectoDestruccionBalaJefe()
    {
        if (efectoDestruccionBalaJefe != null)
        {
            GameObject efecto = Instantiate(efectoDestruccionBalaJefe, transform.position, Quaternion.identity);
            Destroy(efecto, 1f);
        }
    }

    void destruirBalaJefe()
    {
        if (!balaJefeActivada) return;

        balaJefeActivada = false;

        // Restar del contador.
        CContadorBalas.restarBalaJefe();

        // Crear efecto de destrucción.
        crearEfectoDestruccionBalaJefe();

        // Destruir el objeto.
        Destroy(gameObject);
    }

    // Métodos para usarlos externamente.
    public int obtenerPuntosGolpe()
    {
        return puntoGolpeBalaJefe;
    }

    public bool estaActiva()
    {
        return balaJefeActivada;
    }

    void OnBecameInvisible()
    {
        // Destruir la bala cuando salga de la pantalla.
        if (balaJefeActivada)
        {
            Invoke("destruirBalaJefe", 0.2f);
        }
    }
    void OnDestroy()
    {
        // Restar del contador al destruir
        if (balaJefeActivada)
        {
            CContadorBalas.restarBalaJefe();
        }
    }
}