using UnityEngine;
using TMPro;

public class CContadorTiempo : MonoBehaviour
{
    // Referencias de UI.
    public TextMeshProUGUI textoTiempo;
    public TextMeshProUGUI textoResultado;

    // Configuración de tiempo.
    public float tiempoTotal = 30f;
    public float intervaloCambioPatron = 10f;

    // Variables privadas.
    private float tiempoRestante;
    private float tiempoUltimoCambio;
    private bool juegoTerminado = false;
    private int patronActual = 0;

    // Referencias.
    private CJefe jefeObjetivo;
    private CJugador jugadorObjetivo;

    // Eventos.
    public System.Action OnJuegoTerminado;
    public System.Action<int> OnCambioPatron;

    void Start()
    {
        // Inicializar tiempo.
        tiempoRestante = tiempoTotal;
        tiempoUltimoCambio = 0f;

        // Buscar referencias.
        jefeObjetivo = Object.FindFirstObjectByType<CJefe>();
        jugadorObjetivo = Object.FindFirstObjectByType<CJugador>();

        // Configurar texto inicial.
        if (textoResultado != null)
        {
            textoResultado.gameObject.SetActive(false);
        }

        actualizarTextoTiempo();
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Actualizar tiempo.
        tiempoRestante -= Time.deltaTime;
        tiempoRestante = Mathf.Max(0f, tiempoRestante);

        // Verificar cambio de patrón cada 10 segundos.
        verificarCambioPatron();

        // Actualizar UI.
        actualizarTextoTiempo();

        // Verificar si el tiempo se acabó.
        if (tiempoRestante <= 0f)
        {
            terminarJuego(TipoFinJuego.TiempoAgotado);
        }

        // Verificar si el jefe murió.
        if (jefeObjetivo != null && jefeObjetivo.estadoJefe())
        {
            terminarJuego(TipoFinJuego.JefeVencido);
        }

        // Verificar si el jugador murió.
        if (jugadorObjetivo != null && jugadorObjetivo.estadoJugador())
        {
            terminarJuego(TipoFinJuego.JugadorMuerto);
        }
    }

    public enum TipoFinJuego
    {
        TiempoAgotado,
        JefeVencido,
        JugadorMuerto
    }

    void verificarCambioPatron()
    {
        float tiempoTranscurrido = tiempoTotal - tiempoRestante;

        // Verificar si han pasado 10 segundos desde el último cambio.
        if (tiempoTranscurrido >= tiempoUltimoCambio + intervaloCambioPatron)
        {
            cambiarPatron();
            tiempoUltimoCambio = tiempoTranscurrido;
        }
    }

    void cambiarPatron()
    {
        patronActual = (patronActual + 1) % 3;

        // Notificar al jefe sobre el cambio de patrón.
        OnCambioPatron?.Invoke(patronActual);

        Debug.Log($"Patrón cambiado a: {obtenerNombrePatron(patronActual)}");
    }

    void actualizarTextoTiempo()
    {
        if (textoTiempo == null) return;

        // Formatear tiempo en segundos.
        int segundos = Mathf.FloorToInt(tiempoRestante % 60f);

        // Calcular siguiente cambio de patrón.
        float tiempoTranscurrido = tiempoTotal - tiempoRestante;
        float tiempoParaCambio = intervaloCambioPatron - (tiempoTranscurrido % intervaloCambioPatron);
        int segundosParaCambio = Mathf.FloorToInt(tiempoParaCambio);

        string nombrePatron = obtenerNombrePatron(patronActual);

        textoTiempo.text = $"Tiempo\n{segundos:00}";
    }

    string obtenerNombrePatron(int patron)
    {
        switch (patron)
        {
            case 0: return "Espiral";
            case 1: return "Flor";
            case 2: return "Rayo";
            default: return "Desconocido";
        }
    }

    void terminarJuego(TipoFinJuego tipoFin)
    {
        if (juegoTerminado) return;

        juegoTerminado = true;

        // Pausar el juego.
        Time.timeScale = 0f;

        // Mostrar resultado.
        mostrarResultado(tipoFin);

        // Invocar evento de juego terminado.
        OnJuegoTerminado?.Invoke();

        Debug.Log($"Juego terminado: {tipoFin}");
    }

    void mostrarResultado(TipoFinJuego tipoFin)
    {
        if (textoResultado == null) return;

        textoResultado.gameObject.SetActive(true);

        string mensaje = "";
        switch (tipoFin)
        {
            case TipoFinJuego.TiempoAgotado:
                mensaje = "¡Victory!\n Tiempo Agotado, gg.";
                break;

            case TipoFinJuego.JefeVencido:
                mensaje = "¡Victory!\n Jefe Derrotado, gg.";
                break;

            case TipoFinJuego.JugadorMuerto:
                mensaje = "LLLLL\n NOOB WAJAJAJ\n ¡GAME OVER!.";
                break;
        }
        textoResultado.text = mensaje;
    }

    // Métodos públicos para acceder al estado del juego.
    public float obtenerTiempoRestante()
    {
        return tiempoRestante;
    }

    public int obtenerPatronActual()
    {
        return patronActual;
    }

    public bool estaJuegoTerminado()
    {
        return juegoTerminado;
    }
}