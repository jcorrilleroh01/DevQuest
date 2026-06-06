using UnityEngine;

public class MostrarTecla : MonoBehaviour
{
    [Header("Arrastra aquí el objeto hijo 'Icono_Tecla'")]
    public GameObject iconoTecla;

    // Cuando entras en la zona
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Asegúrate que tu personaje tiene el Tag "Player"
        {
            iconoTecla.SetActive(true); // ¡Muestra la tecla!
        }
    }

    // Cuando sales de la zona
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            iconoTecla.SetActive(false); // ¡Esconde la tecla!
        }
    }
}