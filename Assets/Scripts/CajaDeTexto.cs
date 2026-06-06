using UnityEngine;

public class DialogoNPC : MonoBehaviour
{
    // Arrastraremos aquí el PanelDialogo desde Unity
    public GameObject panelDialogo; 
    
    // Una "bandera" para saber si estamos en la zona
    private bool jugadorCerca;

    void Update()
    {
        // Si el jugador está cerca Y pulsa la tecla E
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            // Si el panel está apagado, lo enciende. Si está encendido, lo apaga.
            bool estadoActual = panelDialogo.activeSelf;
            panelDialogo.SetActive(!estadoActual);
        }
    }

    // Cuando entras en la zona del cartel
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate de que tu PJ tenga el Tag "Player"
        {
            jugadorCerca = true;
            Debug.Log("¡Puedes interactuar!"); // Chivato en consola
        }
    }

    // Cuando sales de la zona
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            panelDialogo.SetActive(false); // Cierra el diálogo si te alejas
        }
    }
}