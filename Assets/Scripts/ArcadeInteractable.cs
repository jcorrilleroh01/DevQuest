using UnityEngine;
using UnityEngine.UI;

public class ArcadeInteractable : MonoBehaviour
{
    [Header("Configuración de UI")]
    public GameObject panelArcade; 
    public AudioSource sonidoAbrir;
        public FeedbackInteractuable feedbackVisual; 
        public string idLogro;
        public Sprite iconoArcade;

    public Button botonAccion;     
    public Button botonSalir;      
    
    [Header("Datos de la Arcade")]
    public string urlDestino;      
    public bool esCurriculum;      

    // VARIABLE ESTÁTICA: Compartida por todas las instancias del script
    // Mantiene su valor aunque cambies de escena o uses diferentes arcades.
    private static bool logroDesbloqueado = false;

    private bool jugadorCerca = false;

    void Start()
    {
        botonSalir.onClick.AddListener(CerrarPanel);
        
        if (esCurriculum) {
            botonAccion.onClick.AddListener(DescargarPDF);
        } else {
            botonAccion.onClick.AddListener(AbrirGitHub);
        }
    }

    void Update()
    {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E))
        {
            AbrirPanel();
            CheckearLogroPrimerContacto();
        }
    }

    void AbrirPanel()
    {
        if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }
        if (panelArcade != null)
        {
            panelArcade.SetActive(true);
            if (InventarioManager.Instance != null && InventarioManager.Instance.objetoHUDCompleto != null) InventarioManager.Instance.objetoHUDCompleto.SetActive(false);
if (GestorCursor.Instance != null)
            {
                GestorCursor.Instance.MostrarCursor();
            }        }
    }

    void CheckearLogroPrimerContacto()
    {
        // Solo entra aquí si el logro NO ha sido desbloqueado aún
        if (!logroDesbloqueado)
        {
            logroDesbloqueado = true;

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
        PlayerPrefs.SetInt("Arcade_" + idLogro, 1);
        PlayerPrefs.Save();

        Debug.Log("Logro Arcade desbloqueado: " + idLogro);
    }}

    public void CerrarPanel()
    {
        if (panelArcade != null)
        {
            panelArcade.SetActive(false);
if (GestorCursor.Instance != null)
            {
                GestorCursor.Instance.OcultarCursor();      
            }if (InventarioManager.Instance != null && InventarioManager.Instance.objetoHUDCompleto != null) InventarioManager.Instance.objetoHUDCompleto.SetActive(true);  }
    }

    void AbrirGitHub()
    {
        if (!string.IsNullOrEmpty(urlDestino))
            Application.OpenURL(urlDestino);
    }

    void DescargarPDF()
    {
        if (!string.IsNullOrEmpty(urlDestino))
            Application.OpenURL(urlDestino); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) jugadorCerca = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) {
            jugadorCerca = false;
            CerrarPanel();
        }
    }
}