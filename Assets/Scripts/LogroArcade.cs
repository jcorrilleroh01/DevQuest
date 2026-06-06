using UnityEngine;

public class LogroArcade : MonoBehaviour
{
    public bool encontrado = false; // Memoria de la sesión actual

    [Header("Configuración del Logro")]
    public string idLogro = "logro_insert_coin"; // El ID de tu JSON
    public Sprite iconoArcade; // El icono del logro

    [Header("Conexión Visual")]
    public FeedbackInteractuable feedbackVisual; 
    private bool enRango = false;

    void Start()
    {
        // Al arrancar, verificamos si ya se insertó la moneda anteriormente
        if (PlayerPrefs.GetInt("Arcade_" + idLogro, 0) == 1)
        {
            encontrado = true;
        }
    }

    void Update()
    {
        // Si estamos cerca y pulsamos la E
        if (enRango && Input.GetKeyDown(KeyCode.E))
        {
            InsertarMoneda();
        }
    }

    void InsertarMoneda()
    {
        // Si ya lo encontramos antes, no hacemos nada (escudo anti-spam)
        if (encontrado) 
        {
            Debug.Log("Ya jugaste a esta máquina, no quedan más créditos.");
            return;
        }

        // 1. Registramos el logro en el sistema central (PDA)
        if (GestorLogros.Instance != null)
        {
            GestorLogros.Instance.DesbloquearLogro(idLogro);
        }

        // 2. Lanzamos la notificación visual
        if (NotificacionManager.Instance != null)
        {
            NotificacionManager.Instance.MostrarNotificacion("¡INSERT COIN! Has revivido un clásico.", iconoArcade);
        }

        // 3. Ocultamos el brillo o la tecla de interacción
        if (feedbackVisual != null) feedbackVisual.OcultarPorAccion();

        // 4. Guardamos en el disco duro para siempre
        encontrado = true;
        PlayerPrefs.SetInt("Arcade_" + idLogro, 1);
        PlayerPrefs.Save();

        Debug.Log("Logro Arcade desbloqueado: " + idLogro);
    }

    // --- DETECCIÓN DE PROXIMIDAD ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enRango = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            enRango = false;
        }
    }
}