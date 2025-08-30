using UnityEngine;
using TMPro;

public class CContadorBalas : MonoBehaviour
{
    // Texto en UI.
    public TextMeshProUGUI textoContador;

    // Contadores.
    public static int balasJefe = 0;
    public static int balasJugador = 0;

    void Update()
    {
        // Actualizar texto en UI cada frame
        if (textoContador != null)
        {
            textoContador.text = $"Balas de jefe\n{balasJefe}\n\n Balas de jugador\n{balasJugador}\n\n Total de balas\n{balasJefe + balasJugador}";
        }
    }

    // Métodos estáticos para sumar y restar
    public static void sumarBalaJefe()
    {
        balasJefe++;
    }

    public static void restarBalaJefe()
    {
        balasJefe--;
        if (balasJefe < 0) balasJefe = 0;
    }

    public static void sumarBalaJugador()
    {
        balasJugador++;
    }

    public static void restarBalaJugador()
    {
        balasJugador--;
        if (balasJugador < 0) balasJugador = 0;
    }
}