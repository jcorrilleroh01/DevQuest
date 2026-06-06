using UnityEngine;

public class ZonaEntrada : MonoBehaviour
{
    [Header("Identificador de esta entrada")]
    public string idDeEstaPuerta;

    void Start()
    {
        // 1. Preguntamos si el jugador quería venir a ESTA puerta
        if (ZonaSalida.puertaDestinoActual == idDeEstaPuerta)
        {
            // 2. Buscamos al jugador en la escena nueva
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");

            if (jugador != null)
            {
                // 3. Teletransportamos al jugador a la posición de esta entrada
                jugador.transform.position = transform.position;
            }
            else
            {
                Debug.LogWarning("¡No he encontrado al jugador! ¿Tiene la etiqueta 'Player' puesta?");
            }
        }
    }
}