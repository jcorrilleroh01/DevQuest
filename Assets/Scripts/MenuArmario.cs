using UnityEngine;
using System.Collections.Generic; 

public class ControlArmario : MonoBehaviour
{
    [Header("Conexiones Generales")]
    public GameObject panelMenuRopa;
    public Animator animatorJugador;
    public FeedbackInteractuable feedbackVisual;

    [Header("Efectos de Sonido")]
    public AudioSource sonidoAbrir;

    private RuntimeAnimatorController outfitSeleccionadoPendiente;
    private List<TarjetaRopa> todasLasTarjetas = new List<TarjetaRopa>();

    private bool enRango = false;
    private bool menuAbierto = false;

    void Start()
    {
        menuAbierto = false;
        if(panelMenuRopa != null) panelMenuRopa.SetActive(false);
    }

    // 🛡️ SEGURO DE VIDA: Si el jugador viaja de escena con el armario abierto, salvamos el tiempo
    void OnDisable()
    {
        if (menuAbierto)
        {
            Time.timeScale = 1;
            if (GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor();
        }
    }

    void Update()
    {
        if (enRango && Input.GetKeyDown(KeyCode.E)) 
        {
            if (menuAbierto) CerrarMenu();
            else AbrirMenu();
        }
    }

    // --- LÓGICA DE SELECCIÓN VISUAL ---
    public void RegistrarTarjeta(TarjetaRopa tarjeta)
    {
        if (!todasLasTarjetas.Contains(tarjeta)) todasLasTarjetas.Add(tarjeta);
    }

    public void SeleccionarTarjeta(TarjetaRopa tarjetaClickada)
    {
        foreach (TarjetaRopa t in todasLasTarjetas) t.ApagarMarco();
        tarjetaClickada.EncenderMarco();
        outfitSeleccionadoPendiente = tarjetaClickada.outfitDeEstaTarjeta;
    }

    // --- LÓGICA DE BOTONES DE LA UI ---
    public void BotonConfirmarEquipar()
    {
        if (outfitSeleccionadoPendiente != null && animatorJugador != null)
        {
            animatorJugador.runtimeAnimatorController = outfitSeleccionadoPendiente;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.outfitActual = outfitSeleccionadoPendiente;
            }

            Debug.Log("¡Ropa equipada con éxito!");
            // Ejecutamos orden estricta, si falla la física no pasará nada
            CerrarMenu(); 
        }
    }

    public void BotonCerrarMenu()
    {
        CerrarMenu();
    }

    // --- ÓRDENES ESTRICTAS (Sustituyen al peligroso ToggleMenu) ---
    public void AbrirMenu()
    {
        if (menuAbierto) return; // Cortafuegos
        menuAbierto = true;
        
        if (sonidoAbrir != null) sonidoAbrir.Play();
        if(panelMenuRopa != null) panelMenuRopa.SetActive(true);

        Time.timeScale = 0; 
        if (feedbackVisual != null) feedbackVisual.OcultarPorAccion();
        if (GestorCursor.Instance != null) GestorCursor.Instance.MostrarCursor();
        
        todasLasTarjetas.Clear();
        TarjetaRopa[] encontrados = FindObjectsByType<TarjetaRopa>(FindObjectsSortMode.None);
        todasLasTarjetas.AddRange(encontrados);
    }

    public void CerrarMenu()
    {
        if (!menuAbierto) return; // Cortafuegos
        menuAbierto = false;

        if(panelMenuRopa != null) panelMenuRopa.SetActive(false);
        Time.timeScale = 1; 
        if (GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor();

        // Restauramos el feedback pase lo que pase
        if (feedbackVisual != null) 
        {
            feedbackVisual.MostrarPorFinAccion();
            // Si el jugador se movió por error, apagamos solo la tecla flotante (el brillo se queda)
            if (!enRango && feedbackVisual.iconoTecla != null) 
            {
                feedbackVisual.iconoTecla.SetActive(false);
            }
        }
    }
    
    // --- FÍSICAS ---
    private void OnTriggerEnter2D(Collider2D collision) 
    { 
        if (collision.CompareTag("Player")) enRango = true; 
    }
    
    private void OnTriggerExit2D(Collider2D collision) 
    { 
        if (collision.CompareTag("Player")) 
        { 
            enRango = false; 
            if (menuAbierto) CerrarMenu(); 
        } 
    }
}