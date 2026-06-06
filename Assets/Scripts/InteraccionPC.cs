using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class InteraccionPC : MonoBehaviour
{
    [Header("Referencias de Cámara")]
    public GameObject camaraVirtualPC; 
    
    [Header("Referencias de UI")]
    public GameObject ventanaTerminal;
    public TextMeshProUGUI textoTerminal;
    public Button botonCerrarX;

    [Header("Configuración de Animación")]
    public Animator playerAnimator; // Arrastra aquí al objeto de Jaime [cite: 2026-01-20]
    public string parametroSentado = "isSitting"; // El nombre exacto de tu bool en el Animator

    [Header("Configuración de Texto")]
    [TextArea(5, 10)]
    public string mensajeTerminal = "jaime@portfolio:~$ run presentation.sh\n[OK] Cargando proyectos de DAM...\n[OK] Inicializando entornos multiplataforma...\n\n\"Hola. Soy Jaime. Aquí es donde transformo café en código...\"";
    
    private bool jugadorEnPC = false;
    private bool terminalAbierta = false; 
    private Coroutine corrutinaEscribir;

    void Start() {
        if (botonCerrarX != null) {
            botonCerrarX.onClick.AddListener(CerrarTerminalManualmente);
        }
    }

    void Update() {
        if (jugadorEnPC && Input.GetKeyDown(KeyCode.F)) {
            if (terminalAbierta) {
                LimpiarYSalir();
            } else {
                StartCoroutine(SecuenciaApertura());
            }
        }
    }

    IEnumerator SecuenciaApertura() {
        terminalAbierta = true; 
        
        // 1. Activamos la animación de sentado [cite: 2026-01-20]
        if(playerAnimator != null) playerAnimator.SetBool(parametroSentado, true);

        // 2. Zoom de cámara
        if(camaraVirtualPC != null) camaraVirtualPC.SetActive(true); 
        
        // Espera para que se vea la animación antes de que salga la terminal
        yield return new WaitForSeconds(0.8f); 

        if(ventanaTerminal != null) ventanaTerminal.SetActive(true); 
        corrutinaEscribir = StartCoroutine(EfectoEscribir(mensajeTerminal));
    }

    IEnumerator EfectoEscribir(string texto) {
        textoTerminal.text = ""; 
        foreach (char letra in texto.ToCharArray()) {
            textoTerminal.text += letra;
            yield return new WaitForSeconds(0.03f); 
        }

        while (terminalAbierta) {
            textoTerminal.text = texto + " _";
            yield return new WaitForSeconds(0.5f);
            textoTerminal.text = texto + "  ";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void CerrarTerminalManualmente() {
        LimpiarYSalir();
    }

    private void LimpiarYSalir() {
        terminalAbierta = false; 
        if (corrutinaEscribir != null) StopCoroutine(corrutinaEscribir);
        
        // --- AQUÍ JAIME SE LEVANTA ---
        if(playerAnimator != null) playerAnimator.SetBool(parametroSentado, false);
        
        if(camaraVirtualPC != null) camaraVirtualPC.SetActive(false);
        if(ventanaTerminal != null) ventanaTerminal.SetActive(false);
        
        textoTerminal.text = ""; 
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            jugadorEnPC = true;
            // Opcional: Si Jaime aún no tiene el Animator en el script, lo buscamos
            if(playerAnimator == null) playerAnimator = other.GetComponent<Animator>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            jugadorEnPC = false;
            LimpiarYSalir(); 
        }
    }
}