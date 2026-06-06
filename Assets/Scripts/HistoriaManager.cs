using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class HistoriaManager : MonoBehaviour
{
    [Header("Referencias")]
    public TextMeshProUGUI textoHistoria;
    public AudioSource sonidoEscritura; 
    
    [Tooltip("El objeto vacío que contiene SOLO LAS IMÁGENES de las teclas")]
    public GameObject contenedorImagenesControles; 

    [Header("1. Páginas de Historia Normal")]
    [TextArea(3, 6)]
    public string[] paginasDeHistoria; 

    [Header("2. Página de Controles")]
    [TextArea(3, 6)]
    public string textoControles = "CONTROLES:\n\n(Aquí puedes hacer espacios para que quepan tus imágenes flotantes)\n\n[M] para abrir el mapa de viaje rápido.\n[I] para abrir el inventario.\n\nY más botones para interactuar con el entorno que descubrirás.";

    [Header("3. Página Final")]
    [TextArea(3, 6)]
    public string textoFinalSonido = "Por favor, sube el volumen para disfrutar al máximo de la experiencia.\n\n¡Disfruta del viaje!";

    [Header("Tiempos y Velocidades")]
    public float velocidadEscritura = 0.04f;
    public float tiempoEsperaEntrePaginas = 5f; 
    public float tiempoEsperaEnControles = 8f; // Algo más de tiempo para ver los controles

    [Header("Destino")]
    public string nombreEscenaSpawn = "Spawn";

    private bool forzarTextoCompleto = false;

    void Start()
    {
        textoHistoria.text = "";
        
        // Asegurarnos de que las imágenes de teclas empiecen apagadas
        if(contenedorImagenesControles != null)
        {
            contenedorImagenesControles.SetActive(false);
        }
        
        StartCoroutine(DirectorDeHistoria());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            forzarTextoCompleto = true;
        }
    }

    IEnumerator DirectorDeHistoria()
    {
        // 1. Mostrar páginas de historia
        for (int i = 0; i < paginasDeHistoria.Length; i++)
        {
            yield return StartCoroutine(ReproducirPagina(paginasDeHistoria[i], tiempoEsperaEntrePaginas));
        }

        // 2. Mostrar Controles (Encendemos las imágenes flotantes)
        if(contenedorImagenesControles != null) contenedorImagenesControles.SetActive(true);
        yield return StartCoroutine(ReproducirPagina(textoControles, tiempoEsperaEnControles));
        if(contenedorImagenesControles != null) contenedorImagenesControles.SetActive(false); // Las apagamos al terminar

        // 3. Mostrar Mensaje de Sonido
        yield return StartCoroutine(ReproducirPagina(textoFinalSonido, tiempoEsperaEntrePaginas));

        // 4. Ir al juego
        SceneManager.LoadScene(nombreEscenaSpawn);
    }

    IEnumerator ReproducirPagina(string mensaje, float tiempoEspera)
    {
        forzarTextoCompleto = false;
        textoHistoria.text = "";
        
        if (sonidoEscritura != null) sonidoEscritura.Play();
        
        foreach (char letra in mensaje.ToCharArray())
        {
            if (forzarTextoCompleto)
            {
                textoHistoria.text = mensaje;
                break;
            }

            textoHistoria.text += letra;
            yield return new WaitForSeconds(velocidadEscritura);
        }
        
        if (sonidoEscritura != null) sonidoEscritura.Stop();

        // Pausa de lectura
        float tiempoEsperado = 0f;
        forzarTextoCompleto = false; 
        
        while (tiempoEsperado < tiempoEspera)
        {
            if (forzarTextoCompleto) break; // Permite saltar la pausa
            tiempoEsperado += Time.deltaTime;
            yield return null;
        }
    }
}