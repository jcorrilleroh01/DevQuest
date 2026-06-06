using UnityEngine;

public class ObjetoInteractuable : MonoBehaviour
{
    [Header("Configuración")]
    public string nombreObjeto;
    [TextArea(3, 10)]
    public string contenidoInfo; 
    public GameObject teclaFlotante; 

    private bool jugadorCerca = false;

    void Start() {
        if(teclaFlotante != null) teclaFlotante.SetActive(false);
    }

    void Update() {
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E)) { 
            Debug.Log("Mostrando info de: " + nombreObjeto);
            
            // Verificamos si existe el manager para evitar errores en consola
            if(DialogoManager.Instance != null) {
                DialogoManager.Instance.MostrarTexto(contenidoInfo);
            } else {
                Debug.LogWarning("🚨 No se encuentra DialogoManager en la escena.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            jugadorCerca = true;
            if(teclaFlotante != null) teclaFlotante.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            jugadorCerca = false;
            if(teclaFlotante != null) teclaFlotante.SetActive(false);
            
            if(DialogoManager.Instance != null) {
                DialogoManager.Instance.CerrarPanel();
            }
        }
    }
}