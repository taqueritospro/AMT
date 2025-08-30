using UnityEngine;

public class CBalaJugador : MonoBehaviour
{
    // Parámetros de Bala.
    public float velocidadBalaJugador = 15f;
    public int puntoGolpeBalaJugador = 143;
    public float tiempoVidaBalaJugador = 3f;

    // Efectos.
    public GameObject efectoImpactoBalaJugador;
    public bool destruirBalaJugadorImpacto = true;

    // Variables privadas.
    private Rigidbody2D rbBalaJugador;
    private bool balaJugadorActivada = false;

    void Start()
    {
        // Sumar al contador.
        CContadorBalas.sumarBalaJugador();

        // Obtener el Rigidbody2D.
        rbBalaJugador = GetComponent<Rigidbody2D>();

        // Configurar el Rigidbody2D.
        rbBalaJugador.gravityScale = 0f;

        // Dar velocidad inicial (hacia arriba).
        rbBalaJugador.linearVelocity = Vector2.up * velocidadBalaJugador;

        // Marcar como activada.
        balaJugadorActivada = true;

        // Destruir después del tiempo especificado.
        Destroy(gameObject, tiempoVidaBalaJugador);
    }

    void Update()
    {
        // Verificar si la bala salió de la pantalla por arriba.
        verificarLimitesPantalla();
    }

    void verificarLimitesPantalla()
    {
        // Si la bala sale de la pantalla, destruirla.
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 pantallaPos = cam.WorldToViewportPoint(transform.position);
            if (pantallaPos.x < -0.1f || pantallaPos.x > 1.1f ||
                pantallaPos.y < -0.1f || pantallaPos.y > 1.1f)
            {
                destruirBalaJugador();
            }
        }
    }

    // Detectar colisiones.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!balaJugadorActivada) return;

        // Colisión con jefe.
        if (other.CompareTag("Jefe"))
        {
            // Hacer daño al jefe.
            CJefe jefe = other.GetComponent<CJefe>();
            if (jefe != null)
            {
                jefe.recibirPuntos(puntoGolpeBalaJugador);
            }

            // Crear efecto de impacto.
            crearEfectoImpactoBalaJugador();

            // Destruir la bala Jugador.
            if (destruirBalaJugadorImpacto)
            {
                destruirBalaJugador();
            }
        }

        // Colisión con balas del jefe.
        if (other.CompareTag("BalaJefe"))
        {
            crearEfectoImpactoBalaJugador();
            destruirBalaJugador();
        }

        // Colisión con límites del mapa.
        if (other.CompareTag("LimiteMapa"))
        {
            destruirBalaJugador();
        }
    }

    void crearEfectoImpactoBalaJugador()
    {
        if (efectoImpactoBalaJugador != null)
        {
            GameObject efectoBalaJugador = Instantiate(efectoImpactoBalaJugador, transform.position, Quaternion.identity);

            // Destruir el efecto.
            Destroy(efectoBalaJugador, 1f);
        }
    }

    void destruirBalaJugador()
    {
        if (!balaJugadorActivada) return;
        balaJugadorActivada = false;
        CContadorBalas.restarBalaJugador();
        Destroy(gameObject);
    }

    // Método para obtener el daño.
    public int obtenerPuntosGolpe()
    {
        return puntoGolpeBalaJugador;
    }

    void OnBecameInvisible()
    {
        // Destruir la bala cuando salga de la pantalla.
        if (balaJugadorActivada)
        {
            Invoke("destruirBalaJugador", 0.1f);
        }
    }
    void OnDestroy()
    {
        // Restar del contador al destruir.
        if (balaJugadorActivada)
        {
            CContadorBalas.restarBalaJugador();
        }
    }
}