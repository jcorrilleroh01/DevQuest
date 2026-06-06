using UnityEngine;

public class CartelMapaInteractuable : MonoBehaviour
{
    [Header("UI References (Arrastrar desde el Canvas)")]
    // El Panel padre que contiene la imagen y el fondo oscuro
    public GameObject panelMapaUI; 
    // El Animator componente que está en el panelMapaUI
    public Animator animatorPanel;   

    [Header("Interaction Visuals (Ya configurado en el objeto)")]
    // Referencia a tu tecla flotante y brillo ya creados
    public GameObject teclaFlotanteInteract; 

    private bool jugadorCerca = false;
    private bool mapaAbierto = false;
    
    // KeyCode configurable, por defecto la 'E'
    private KeyCode interactionKey = KeyCode.E;

    void Start() {
        // Asegurar que el panel esté oculto al iniciar el juego
        if (panelMapaUI != null) {
            panelMapaUI.SetActive(false);
            // Asegurar que la escala sea 0 para empezar
            panelMapaUI.transform.localScale = Vector3.zero; 
        }
        
        // Asumimos que la tecla flotante empieza desactivada
        if(teclaFlotanteInteract != null) teclaFlotanteInteract.SetActive(false);
    }

    void Update() {
        // Detectar si el jugador presiona E estando cerca
        if (jugadorCerca && Input.GetKeyDown(interactionKey)) {
            if (mapaAbierto) {
                // Si ya está abierto, lo cerramos al pulsar E de nuevo
                CloseMapaWithAnimation();
            } else {
                // Si está cerrado, lo abrimos
                OpenMapaWithAnimation();
            }
        }
    }

    void OpenMapaWithAnimation() {
        if (panelMapaUI != null && animatorPanel != null) {
            mapaAbierto = true;
            panelMapaUI.SetActive(true); // Activar GameObject para ver la animación
            animatorPanel.Play("EXPANDIRMAPA"); // Ejecutar la animación de expansión
            
            // OPCIONAL: Bloquear movimiento del jugador aquí si tienes un GameManager
            // ej: GameManager.Instance.LockPlayerInput(true);
        }
    }

    // Función separada para cerrar, así podemos llamarla si el jugador se aleja
    void CloseMapaWithAnimation() {
        if (panelMapaUI != null && animatorPanel != null && mapaAbierto) {
            mapaAbierto = false;
            animatorPanel.Play("CERRARMAPA"); // Ejecutar la animación de cierre
            
            // OPCIONAL: Desbloquear movimiento del jugador aquí
            // ej: GameManager.Instance.LockPlayerInput(false);
            
            // Usar una corrutina o evento de animación para desactivar el panel
            // DESPUÉS de que la animación termine, para que no desaparezca instantáneamente.
            StartCoroutine(DesactivarPanelDespuesAnimacion(0.5f)); // Reemplaza 0.5f con la duración real de tu anim.
        }
    }

    // Corrutina simple para desactivar el GameObject tras el final de la animación
    System.Collections.IEnumerator DesactivarPanelDespuesAnimacion(float delay) {
        yield return new WaitForSeconds(delay);
        if (!mapaAbierto) { // Verificar que el jugador no lo haya reabierto
            panelMapaUI.SetActive(false);
        }
    }

    // Trigger Logic (Casi idéntica a tu script original, adaptada)
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            jugadorCerca = true;
            // Activar tu tecla flotante y brillo
            if(teclaFlotanteInteract != null) teclaFlotanteInteract.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            jugadorCerca = false;
            // Desactivar tu tecla flotante y brillo
            if(teclaFlotanteInteract != null) teclaFlotanteInteract.SetActive(false);
            
            // Si el mapa estaba abierto, lo cerramos automáticamente si el jugador se aleja
            if(mapaAbierto) {
                CloseMapaWithAnimation();
            }
        }
    }
}