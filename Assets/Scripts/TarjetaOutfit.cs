using UnityEngine;
using UnityEngine.UI;

public class TarjetaRopa : MonoBehaviour
{
    [Header("Identificación")]
    public string idNombre; 
    public RuntimeAnimatorController outfitDeEstaTarjeta;
    
    [Header("Visuales")]
    public GameObject marcoSeleccion; 
    public GameObject iconoCandado; 
    public GameObject iconoRopa; // <--- NUEVO: Aquí arrastraremos la imagen de la camisa

    private ControlArmario manager;
    private bool estaBloqueado = false; 

    void OnEnable()
    {
        if (manager == null) manager = FindFirstObjectByType<ControlArmario>();
        
        if (GameManager.Instance != null)
        {
            bool desbloqueado = GameManager.Instance.EstaDesbloqueado(idNombre);
            
            // --- AQUÍ ESTÁ EL CAMBIO ---
            if (desbloqueado)
            {
                // ESTADO: LIBRE
                if(iconoCandado != null) iconoCandado.SetActive(false); // Adiós candado
                if(iconoRopa != null) iconoRopa.SetActive(true);       // ¡HOLA ROPA!
                estaBloqueado = false;
            }
            else
            {
                // ESTADO: BLOQUEADO
                if(iconoCandado != null) iconoCandado.SetActive(true);  // Hola candado
                if(iconoRopa != null) iconoRopa.SetActive(false);      // Ocultar ropa (para que sea sorpresa)
                estaBloqueado = true;
            }
        }
        
        if(marcoSeleccion != null) marcoSeleccion.SetActive(false);
    }

    public void AlHacerClick()
    {
        if (estaBloqueado) return; 

        if(manager != null)
        {
            manager.SeleccionarTarjeta(this);
        }
    }

    public void EncenderMarco() { if(marcoSeleccion != null) marcoSeleccion.SetActive(true); }
    public void ApagarMarco() { if(marcoSeleccion != null) marcoSeleccion.SetActive(false); }
}