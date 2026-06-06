using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogoManager : MonoBehaviour
{
    public static DialogoManager Instance;

    [Header("Referencias UI")]
    public GameObject panelDialogo;
    public TextMeshProUGUI textoContenido;
    public GameObject flechaContinuar; 

    [Header("Ajustes de Texto")]
    public float velocidadEscritura = 0.05f;
    private Coroutine corrutinaEscritura;

    [Header("Audio")]
    public AudioSource fuenteAudio; 
    public AudioClip sonidoTeclado; // <-- Renombrado para que quede más claro

    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start() {
        CerrarPanel();
        if(flechaContinuar != null) flechaContinuar.SetActive(false);
    }

    public void MostrarTexto(string mensaje) {
        panelDialogo.SetActive(true);
        if(flechaContinuar != null) flechaContinuar.SetActive(false); 
        
        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        corrutinaEscritura = StartCoroutine(EscribirMensaje(mensaje));
    }

    IEnumerator EscribirMensaje(string mensaje) {
        textoContenido.text = ""; 
        
        // --- 1. ARRANCAMOS EL SONIDO DE ESCRIBIR ---
        if (fuenteAudio != null && sonidoTeclado != null) {
            fuenteAudio.clip = sonidoTeclado;
            fuenteAudio.loop = true; // Hacemos que se repita mientras dure el texto
            fuenteAudio.Play();
        }
        
        foreach (char letra in mensaje.ToCharArray()) {
            textoContenido.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
        
        // --- 2. PARAMOS EL SONIDO AL TERMINAR ---
        if (fuenteAudio != null) {
            fuenteAudio.Stop();
        }
        
        if(flechaContinuar != null) flechaContinuar.SetActive(true);
        corrutinaEscritura = null;
    }

    public void CerrarPanel() {
        if (corrutinaEscritura != null) StopCoroutine(corrutinaEscritura);
        
        // --- 3. POR SI ACASO: Paramos el sonido si te alejas antes de que termine ---
        if (fuenteAudio != null) fuenteAudio.Stop(); 
        
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (flechaContinuar != null) flechaContinuar.SetActive(false);
    }
}