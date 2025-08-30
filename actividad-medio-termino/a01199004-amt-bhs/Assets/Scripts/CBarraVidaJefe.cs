using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CBarraVidaJefe : MonoBehaviour
{
    // Referencias de UI.
    public Slider barraVidaJefe;
    public Image fillBarraVida;
    public TextMeshProUGUI textoVidaJefe;

    // Colores de la barra.
    public Color colorVidaCompleta = Color.green;
    public Color colorVidaMedia = Color.orange;
    public Color colorVidaBaja = Color.red;

    // Referencia del jefe.
    public CJefe jefeObjetivo;

    // Configuración de la barra.
    public bool cambiarColorJefe = true;
    public bool mostrarNumeros = true;

    // Variables privadas.
    private SpriteRenderer spriteRendererJefe;
    private Color colorOriginalJefe;

    void Start()
    {
        // Buscar el jefe si no está asignado.
        if (jefeObjetivo == null)
        {
            jefeObjetivo = Object.FindFirstObjectByType<CJefe>();
        }

        if (jefeObjetivo != null)
        {
            // Enterarse del chisme al evento de cambio de vida.
            jefeObjetivo.OnVidaCambiadaJefe += actualizarBarraVida;
            jefeObjetivo.OnJefeMuerto += jefeMuerto;

            // Obtener el SpriteRenderer del jefe para cambiar color.
            spriteRendererJefe = jefeObjetivo.GetComponent<SpriteRenderer>();
            if (spriteRendererJefe != null)
            {
                colorOriginalJefe = spriteRendererJefe.color;
            }

            // Configurar la barra inicial.
            configurarBarraVidaInicial();
        }
        else
        {
            Debug.LogError("No se encontró el jefe para la barra de vida!");
        }
    }

    void configurarBarraVidaInicial()
    {
        if (barraVidaJefe == null || jefeObjetivo == null)
        {
            Debug.LogWarning("Referencias faltantes para configurar la barra de vida");
            return;
        }

        barraVidaJefe.maxValue = jefeObjetivo.vidaMaximaJefe;
        barraVidaJefe.value = jefeObjetivo.vidaActualJefe;

        actualizarBarraVida(jefeObjetivo.vidaActualJefe);

        Debug.Log($"Barra configurada: {jefeObjetivo.vidaActualJefe}/{jefeObjetivo.vidaMaximaJefe}");
    }

    public void actualizarBarraVida(int vidaJefeActual)
    {
        if (barraVidaJefe == null || jefeObjetivo == null) return;

        // Actualizar valor de la barra.
        barraVidaJefe.value = vidaJefeActual;

        // Calcular porcentaje.
        float porcentaje = (float)vidaJefeActual / (float)jefeObjetivo.vidaMaximaJefe;

        // Cambiar color de la barra según la vida.
        cambiarColorBarra(porcentaje);

        // Actualizar texto si.
        if (mostrarNumeros && textoVidaJefe != null)
        {
            textoVidaJefe.text = "Vida del Jefe\n" + vidaJefeActual + " / " + jefeObjetivo.vidaMaximaJefe;
        }
        Debug.Log($"Barra actualizada: {vidaJefeActual}/{jefeObjetivo.vidaMaximaJefe} ({porcentaje:P1})");
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

    void jefeMuerto()
    {
        // Ocultar la barra cuando el jefe muere.
        if (barraVidaJefe != null)
        {
            barraVidaJefe.gameObject.SetActive(false);
        }

        if (textoVidaJefe != null)
        {
            textoVidaJefe.text = "¡Victory!\n Jefe Derrotado, gg.";
        }
    }

    void OnDestroy()
    {
        // Dejar el chisme de los eventos para evitar errores.
        if (jefeObjetivo != null)
        {
            jefeObjetivo.OnVidaCambiadaJefe -= actualizarBarraVida;
            jefeObjetivo.OnJefeMuerto -= jefeMuerto;
        }
    }
}