using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CBarraVidaJugador : MonoBehaviour
{
    // Referencias del UI.
    public Slider barraVidaJugador;
    public Image fillBarraVida;
    public TextMeshProUGUI textoVidaJugador;

    // Colores.
    public Color colorVidaCompleta = Color.green;
    public Color colorVidaMedia = Color.orange;
    public Color colorVidaBaja = Color.red;

    // Configuración.
    public bool mostrarNumeros = true;
    public bool cambiarColores = true;

    // Variables privadas.
    private SpriteRenderer spriteRendererJugador;
    private Color colorOriginalJugador;

    // Referencia del jugador.
    private CJugador jugadorObjetivo;

    void Start()
    {
        // Busca al jugador.
        if (jugadorObjetivo == null)
        {
            jugadorObjetivo = Object.FindFirstObjectByType<CJugador>();
        }

        if (jugadorObjetivo != null)
        {
            // Enterarse del chisme des los eventos del jugador.
            jugadorObjetivo.OnVidaCambiadaJugador += actualizarBarraVida;
            jugadorObjetivo.OnJugadorMuerto += jugadorMuerto;

            spriteRendererJugador = jugadorObjetivo.GetComponent<SpriteRenderer>();
            if (spriteRendererJugador != null)
            {
                colorOriginalJugador = spriteRendererJugador.color;
            }

            // Configurar barra inicial.
            configurarBarraInicial();
        }
        else
        {
            Debug.LogError("No se encontró el jugador!");
        }
    }

    void configurarBarraInicial()
    {
        if (barraVidaJugador == null || jugadorObjetivo == null)
        {
            Debug.LogWarning("Referencias faltantes para configurar la barra de vida");
            return;
        }

        barraVidaJugador.maxValue = jugadorObjetivo.vidaMaximaJugador;
        barraVidaJugador.value = jugadorObjetivo.vidaActualJugador;

        actualizarBarraVida(jugadorObjetivo.vidaActualJugador);

        Debug.Log($"Barra configurada: {jugadorObjetivo.vidaActualJugador}/{jugadorObjetivo.vidaMaximaJugador}");
    }

    public void actualizarBarraVida(int vidaJugadorActual)
    {
        if (barraVidaJugador == null || jugadorObjetivo == null) return;

        // Actualizar valor de la barra.
        barraVidaJugador.value = vidaJugadorActual;

        // Calcular porcentaje.
        float porcentaje = (float)vidaJugadorActual / (float)jugadorObjetivo.vidaMaximaJugador;

        // Cambiar color según la vida.
        cambiarColorBarra(porcentaje);

        // Actualizar texto.
        if (mostrarNumeros && textoVidaJugador != null)
        {
            textoVidaJugador.text = "Vida del Jugador\n" + vidaJugadorActual + " / " + jugadorObjetivo.vidaMaximaJugador;
        }
        Debug.Log($"Barra actualizada: {vidaJugadorActual}/{jugadorObjetivo.vidaMaximaJugador} ({porcentaje:P1})");
    }

    void cambiarColorBarra(float porcentaje)
    {
        if (fillBarraVida == null) return;

        if (porcentaje > 0.7f)
        {
            fillBarraVida.color = colorVidaCompleta;
        }
        else if (porcentaje > 0.2f)
        {
            fillBarraVida.color = colorVidaMedia;
        }
        else
        {
            fillBarraVida.color = colorVidaBaja;
        }
    }

    void jugadorMuerto()
    {
        // Ocultar la barra cuando el jefe muere
        if (barraVidaJugador != null)
        {
            barraVidaJugador.gameObject.SetActive(false);
        }
        // Cambiar texto cuando el jugador muere
        if (textoVidaJugador != null)
        {
            textoVidaJugador.text = "LLLLL\n NOOB WAJAJAJ\n ¡GAME OVER!.";
        }
    }

    void OnDestroy()
    {
        // Desuscribirse de los eventos
        if (jugadorObjetivo != null)
        {
            jugadorObjetivo.OnVidaCambiadaJugador -= actualizarBarraVida;
            jugadorObjetivo.OnJugadorMuerto -= jugadorMuerto;
        }
    }
}