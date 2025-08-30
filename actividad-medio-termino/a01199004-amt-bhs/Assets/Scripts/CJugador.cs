using UnityEngine;

public class CJugador : MonoBehaviour
{
    // Parámetros de movimiento.
    public float velocidadMovimientoJugador = 10f;
    public float velocidadMovimientoLento = 4f;
    public bool limitarMovimientoJugador = true;

    // Parámetros de disparo.
    public GameObject balaJugador;
    public Transform puntoDisparoJugador;
    public float velocidadDisparoJugador = 15f;
    public float intervalDisparoJugador = 0.2f;

    // Parámetros de vida.
    public int vidaMaximaJugador = 4500;
    public int vidaActualJugador;

    // Referencias.
    public Camera camaraJuego;

    // Variables privadas.
    private float tiempoUltimoDisparoJugador;
    private Vector2 limitesMovimientoJugador;
    private bool estaMuertoJugador = false;

    // Eventos.
    public System.Action<int> OnVidaCambiadaJugador;
    public System.Action OnJugadorMuerto;

    void Start()
    {
        // Inicializar vida.
        vidaActualJugador = vidaMaximaJugador;

        // Usar cámara princípal.
        camaraJuego = Camera.main;

        // Calcular límites de movimiento basados en la cámara.
        calcularLimitesMovimientoJugador();

        // Punto de disparo asignado.
        puntoDisparoJugador = transform;

        // Notificar vida inicial a la barra
        OnVidaCambiadaJugador?.Invoke(vidaActualJugador);
    }

    void Update()
    {
        if (estaMuertoJugador) return;

        movimientoJugador();
        disparoJugador();
    }

    void movimientoJugador()
    {
        // Obtener input del jugador.
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        // Determinar velocidad (normal o lenta con Shift presionado).
        float velocidadActual = velocidadMovimientoJugador;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocidadActual = velocidadMovimientoLento;
        }

        // Crear vector de movimiento.
        Vector3 movimiento = new Vector3(inputH, inputV, 0f) * velocidadActual * Time.deltaTime;

        // Aplicar movimiento.
        transform.Translate(movimiento);

        // Limitar movimiento dentro de la pantalla.
        Vector2 areaMinima = new Vector2(-10f, -3.5f);
        Vector2 areaMaxima = new Vector2(3.5f, 1f);
        if (limitarMovimientoJugador)
        {
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, areaMinima.x, areaMaxima.x);
            pos.y = Mathf.Clamp(pos.y, areaMinima.y, areaMaxima.y);
            transform.position = pos;
        }
    }

    void disparoJugador()
    {
        // Verificar si puede disparar.
        bool puedeDisparar = Time.time >= tiempoUltimoDisparoJugador + intervalDisparoJugador;

        // Disparar con botón izquierdo del mouse.
        if ((Input.GetKeyDown(KeyCode.Space)) || (Input.GetMouseButtonDown(0)) && puedeDisparar)
        {
            disparar();
            tiempoUltimoDisparoJugador = Time.time;
        }
    }

    void disparar()
    {
        if (balaJugador == null)
        {
            Debug.LogWarning("No hay prefab de bala asignado al jugador");
            return;
        }

        // Crear la bala.
        GameObject bala = Instantiate(balaJugador, puntoDisparoJugador.position, puntoDisparoJugador.rotation);

        // Destruir la bala después de un tiempo para optimizar.
        Destroy(bala, 3f);
    }

    public void recibirPuntos(int cantidad = 1)
    {
        if (estaMuertoJugador) return;

        vidaActualJugador -= cantidad;
        vidaActualJugador = Mathf.Max(0, vidaActualJugador);

        // Notificar cambio de vida.
        OnVidaCambiadaJugador?.Invoke(vidaActualJugador);

        // Verificar si murió.
        if (vidaActualJugador <= 0)
        {
            morirJugador();
        }
    }

    void morirJugador()
    {
        estaMuertoJugador = true;
        OnJugadorMuerto?.Invoke();
        Debug.Log("¡El jugador ha muerto!");

        // Desactivar el objeto.
        gameObject.SetActive(false);
    }

    void calcularLimitesMovimientoJugador()
    {
        if (camaraJuego == null) return;

        // Obtener los límites de la cámara en coordenadas del mundo.
        float distancia = Vector3.Distance(transform.position, camaraJuego.transform.position);
        Vector2 limites = camaraJuego.ViewportToWorldPoint(new Vector3(1, 1, distancia));

        // Ajustar límites considerando el tamaño del jugador.
        float margen = 0.7f;
        limitesMovimientoJugador.x = limites.x - margen;
        limitesMovimientoJugador.y = limites.y - margen;
    }

    // Método para detectar colisiones con balas enemigas.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BalaJefe"))
        {
            recibirPuntos();
            Destroy(other.gameObject);
        }
    }

    // Método para saber si está muerto.
    public bool estadoJugador()
    {
        return estaMuertoJugador;
    }

    // Método para obtener porcentaje de vida.
    public float obtenerPorcentajeVidaJugador()
    {
        return (float)vidaActualJugador / (float)vidaMaximaJugador;
    }
}