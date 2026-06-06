using UnityEngine;

public class ControlCama : MonoBehaviour
{
    public bool encontrado = false; 
    
    [Header("Inicio de Escena")]
    [Tooltip("Marca esta casilla para que Jaime aparezca frito en la cama al cargar la escena")]
    public bool empezarDurmiendo = false; 

    [Header("Configuración")]
    public Animator animatorJugador;
    public Transform puntoDeDormir;
    public MonoBehaviour scriptMovimiento;

    [Header("Conexión Visual")]
    public FeedbackInteractuable feedbackVisual; 
    public Sprite spriteCandadoAbierto;
    
    private bool enRango = false;
    private bool estaDurmiendo = false;

    private static bool primeraVezEnElJuego = true;

    void Start()
    {
        if (PlayerPrefs.GetInt("Logro_logro_02_cama", 0) == 1)
        {
            encontrado = true;
        }

        if (empezarDurmiendo && primeraVezEnElJuego)
        {
            estaDurmiendo = true;
            if (puntoDeDormir != null) animatorJugador.transform.position = puntoDeDormir.position;
            if (scriptMovimiento != null) scriptMovimiento.enabled = false;
            
            animatorJugador.SetBool("IsSleep", true);
            
            if (feedbackVisual != null) feedbackVisual.MostrarPorFinAccion();

            primeraVezEnElJuego = false;
        }
    }

    void Update()
    {
        if (estaDurmiendo && Input.GetKeyDown(KeyCode.Q))
        {
            Levantarse();
        }
        else if (!estaDurmiendo && enRango && Input.GetKeyDown(KeyCode.Q))
        {
            Dormir();
        }
    }

    void Dormir()
    {
        estaDurmiendo = true;

        if (puntoDeDormir != null) animatorJugador.transform.position = puntoDeDormir.position;
        if (scriptMovimiento != null) scriptMovimiento.enabled = false;
        animatorJugador.SetBool("IsSleep", true);

        if (feedbackVisual != null) feedbackVisual.MostrarPorFinAccion();

        if (encontrado == false)  
        {
            if (GestorLogros.Instance != null)
            {
                GestorLogros.Instance.DesbloquearLogro("logro_02_cama");
            }
            else
            {
                Debug.LogWarning("🚨 Aviso: No hay GestorLogros en esta escena.");
            }

            if (NotificacionManager.Instance != null)
            {
                NotificacionManager.Instance.MostrarNotificacion("NUEVO LOGRO DESBLOQUEADO", spriteCandadoAbierto);
            }

            encontrado = true; 
            
            PlayerPrefs.SetInt("Logro_logro_02_cama", 1);
            PlayerPrefs.Save();
        }
    }

    void Levantarse()
    {
        estaDurmiendo = false;

        // ¡AQUÍ ESTABA EL ERROR! Hemos borrado el "OcultarPorAccion" para que la tecla siga visible.

        animatorJugador.SetBool("IsSleep", false);
        if (scriptMovimiento != null) scriptMovimiento.enabled = true;
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) enRango = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) enRango = false;
    }
}