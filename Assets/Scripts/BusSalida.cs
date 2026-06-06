using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BusSalida : MonoBehaviour
{
    [Header("Conexión con tu Sistema de Viaje")]
    public string nombreDeLaOtraEscena; 
    public string idPuertaDestino;      // Debe ser EXACTO al de la ZonaEntrada destino
    public Animator panelTransicion;    
    public float tiempoDeEspera = 1f;

    [Header("Actores de la Cinemática")]
    public Transform autobus;
    public AudioSource sonidoAbrir;

    public Transform puntoParada;
    [Tooltip("Punto fuera de cámara desde donde vendrá el bus")]
    public Transform puntoInicioBus;    
    public GameObject jugador;

    [Header("Configuración Bus")]
    public float velocidadBus = 10f;

    private bool enRango = false;
    private bool cinematicaIniciada = false;

    void Start()
    {
        // Colocamos el bus fuera de cámara respetando su Z original (así no se entierra en el fondo)
        if (autobus != null && puntoInicioBus != null)
        {
            autobus.position = new Vector3(puntoInicioBus.position.x, puntoInicioBus.position.y, autobus.position.z);
        }
    }

    void Update()
    {
        // Si estamos cerca, pulsamos E y no ha empezado la cinemática...
        if (enRango && !cinematicaIniciada && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SecuenciaSalida());
        }
    }

    IEnumerator SecuenciaSalida()
    {
        cinematicaIniciada = true;
         if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }

        // 1. EL BUS VIENE HACIA LA PARADA (Moverse limpio, sin volteos)
        while (Vector2.Distance(autobus.position, puntoParada.position) > 0.1f)
        
        {
            autobus.position = Vector2.MoveTowards(autobus.position, puntoParada.position, velocidadBus * Time.deltaTime);
            yield return null; 
        }

        // 2. EL BUS APARCA Y EL JUGADOR SE SUBE
        if (jugador != null) jugador.SetActive(false);
        yield return new WaitForSeconds(0.5f);

        // 3. EL BUS SE DA LA VUELTA (FUERA DEL BUCLE, SOLO 1 VEZ)
        Vector3 escala = autobus.localScale;
        escala.x *= -1; 
        autobus.localScale = escala;

        // 4. INICIAMOS EL FUNDIDO A NEGRO
        if (panelTransicion != null)
        {
            panelTransicion.Play("Crossfade_Start");
        }

        // 5. EL BUS SE VA DANDO LA VUELTA MIENTRAS LA PANTALLA SE OSCURECE
        float tiempoFunde = 0f;
        while (tiempoFunde < tiempoDeEspera)
        {
            // El bus vuelve al punto de inicio (fuera de cámara)
            autobus.position = Vector2.MoveTowards(autobus.position, puntoInicioBus.position, velocidadBus * Time.deltaTime);
            tiempoFunde += Time.deltaTime;
            yield return null;
        }

        // 6. GUARDAMOS EL ID Y VIAJAMOS
        ZonaSalida.puertaDestinoActual = idPuertaDestino; 
        SceneManager.LoadScene(nombreDeLaOtraEscena);
    }

    // Aseguramos los sensores
    private void OnTriggerEnter2D(Collider2D collision) { if (collision.CompareTag("Player")) enRango = true; }
    private void OnTriggerStay2D(Collider2D collision)  { if (collision.CompareTag("Player")) enRango = true; }
    private void OnTriggerExit2D(Collider2D collision)  { if (collision.CompareTag("Player")) enRango = false; }
}