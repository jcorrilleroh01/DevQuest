using UnityEngine;

public class InteractuablePostIt : MonoBehaviour
{
    [Header("UI y Feedback")]
    public GameObject panelPostItUI; // El Canvas grande con el Post-it y la contraseña
    public FeedbackInteractuable feedback; // La tecla 'E' flotante

    private bool enRango = false;
    private bool panelAbierto = false;

    void Start()
    {
        if (panelPostItUI != null) panelPostItUI.SetActive(false);
    }

    void Update()
    {
        if (enRango && Input.GetKeyDown(KeyCode.E))
        {
            if (!panelAbierto) AbrirPanel();
            else CerrarPanel();
        }
    }

    void AbrirPanel()
    {
        panelAbierto = true;
        if (GestorCursor.Instance != null) {
            GestorCursor.Instance.MostrarCursor();
        }
        if (panelPostItUI != null) panelPostItUI.SetActive(true);
        if (feedback != null) feedback.OcultarPorAccion();
        // Opcional: Time.timeScale = 0; si quieres que el juego se pause al leer
    }

    // Vincula este método al evento OnClick() de tu botón de la 'X' en la UI
    public void CerrarPanel()
    {
        if (GestorCursor.Instance != null) {
            GestorCursor.Instance.OcultarCursor();
        }
        panelAbierto = false;
        if (panelPostItUI != null) panelPostItUI.SetActive(false);
        if (feedback != null && enRango) feedback.MostrarPorFinAccion();
        // Opcional: Time.timeScale = 1; si pausaste el juego
    }

    private void OnTriggerEnter2D(Collider2D col) 
    { 
        if(col.CompareTag("Player")) { enRango = true; if(feedback != null && !panelAbierto) feedback.MostrarPorFinAccion(); }
    }
    
    private void OnTriggerExit2D(Collider2D col) 
    { 
        if(col.CompareTag("Player")) { enRango = false; if(feedback != null) feedback.OcultarPorAccion(); CerrarPanel(); }
    }
}