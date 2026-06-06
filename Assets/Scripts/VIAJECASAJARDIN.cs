using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ZonaSalida : MonoBehaviour
{
    [Header("A dónde vamos")]
    public string nombreDeLaOtraEscena; 
    
    // ¡NUEVO! La contraseña de la puerta a la que queremos llegar
    public string idPuertaDestino; 

    // ¡NUEVO! Variable global que la próxima escena leerá
    public static string puertaDestinoActual; 

    [Header("Efectos Visuales")]
    public Animator panelTransicion;    
    public float tiempoDeEspera = 1f;   

    private bool activado = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !activado)
        {
            activado = true; 
            
            // ¡La Magia! Guardamos a qué puerta vamos antes de viajar
            puertaDestinoActual = idPuertaDestino; 
            
            StartCoroutine(CambiarEscena());
        }
    }

    IEnumerator CambiarEscena()
    {
        if (panelTransicion != null)
        {
            panelTransicion.Play("Crossfade_Start");
        }
        yield return new WaitForSeconds(tiempoDeEspera);
        SceneManager.LoadScene(nombreDeLaOtraEscena);
    }
}