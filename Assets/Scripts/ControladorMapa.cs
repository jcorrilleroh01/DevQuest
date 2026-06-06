using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControladorMapa : MonoBehaviour
{
    [Header("Referencias de Paneles")]
    public GameObject hudMinimapa; 
    public RectTransform panelMapaGlobal; 

    [Header("Configuración Mapa Local (HUD)")]
    public Image imagenMapaHUD; 
    
    [System.Serializable]
    public struct DatosMapa
    {
        public string nombreEscena;
        public Sprite fotoDelMapa;
        public bool permitirMapa; // ¡NUEVO! Casilla para activar/desactivar el mapa aquí
    }
    public DatosMapa[] mapasLocales; 

    [Header("Animación del Mapa Global")]
    public float velocidadApertura = 15f;
    [Header("Efectos de Sonido")]
    public AudioSource audioViajeSalida;  // El "Fium" de irse
    public AudioSource audioViajeLlegada; // El "Fium" de aparecer
    private bool viajeRapidoEnCurso = false; // Para saber si venimos del mapa
    private bool mapaAbierto = false;
    private bool mapaPermitidoAqui = true; // Control interno del script
    private Vector3 escalaCerrado = Vector3.zero; 
    private Vector3 escalaAbierto = Vector3.one;  
public static ControladorMapa Instance;

void Awake()
{
    if (Instance == null)
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    else
    {
        Destroy(gameObject);
    }
}
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ActualizarFotoLocal(); 
        
        mapaAbierto = false;
        if(panelMapaGlobal != null)
        {
            panelMapaGlobal.localScale = escalaCerrado;
            panelMapaGlobal.gameObject.SetActive(false);
        }
        Time.timeScale = 1; 
        // --- NUEVO: Sonido de llegada ---
        if (viajeRapidoEnCurso)
        {
            if (audioViajeLlegada != null) 
            {
                audioViajeLlegada.Play();
            }
            viajeRapidoEnCurso = false; // Apagamos el interruptor
        }
        // --------------------------------
    }

    void Start()
{
    ActualizarFotoLocal();
    
    // Aseguramos que al nacer en el Spawn, el mapa esté apagado
    mapaAbierto = false; 
    if(panelMapaGlobal != null)
    {
        panelMapaGlobal.localScale = escalaCerrado;
        panelMapaGlobal.gameObject.SetActive(false);
    }
}

    void Update()
    {
        // ¡NUEVO! Solo funciona la tecla M si el mapa está permitido en esta escena
        if (mapaPermitidoAqui && Input.GetKeyDown(KeyCode.M))
        {
            mapaAbierto = !mapaAbierto;

            if (mapaAbierto)
            {
                panelMapaGlobal.gameObject.SetActive(true);
                if (GestorCursor.Instance != null) {
            GestorCursor.Instance.MostrarCursor();
        }
                if(hudMinimapa != null) hudMinimapa.SetActive(false);
                Time.timeScale = 0; 
            }
            else
            {
                
                if(hudMinimapa != null) hudMinimapa.SetActive(true);
                Time.timeScale = 1; if (GestorCursor.Instance != null) {
            GestorCursor.Instance.OcultarCursor();
        }
            }
        }

        if (mapaAbierto)
        {
            panelMapaGlobal.localScale = Vector3.Lerp(panelMapaGlobal.localScale, escalaAbierto, Time.unscaledDeltaTime * velocidadApertura);
        }
        else if (panelMapaGlobal.gameObject.activeSelf)
        {
            panelMapaGlobal.localScale = Vector3.Lerp(panelMapaGlobal.localScale, escalaCerrado, Time.unscaledDeltaTime * velocidadApertura);
            if (panelMapaGlobal.localScale.x < 0.05f)
            {
                panelMapaGlobal.gameObject.SetActive(false);
            }
        }
    }

    void ActualizarFotoLocal()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        
        // Por defecto lo apagamos, por si entras a una escena que no está en la lista
        mapaPermitidoAqui = false; 

        foreach (DatosMapa mapa in mapasLocales)
        {
            if (mapa.nombreEscena == escenaActual)
            {
                if (imagenMapaHUD != null) imagenMapaHUD.sprite = mapa.fotoDelMapa;
                mapaPermitidoAqui = mapa.permitirMapa; // Leemos si tiene permiso
                break;
            }
        }

        // Encendemos o apagamos el HUD de la esquina según el permiso
        if (hudMinimapa != null)
        {
            hudMinimapa.SetActive(mapaPermitidoAqui);
        }
    }

    // Modificamos para que acepte el nombre de la escena Y el ID del punto de llegada
// Ahora la función pide el nombre del archivo de la escena Y el ID de la puerta de llegada
public void ViajarAEscena(string datosCombinados)
{
    // TRUCO PRO: Los botones de Unity a veces dan problemas con 2 strings, 
    // así que vamos a usar un formato "Escena:ID" (ej: JARDIN:LlegadaDesdeSpawn)
    string[] partes = datosCombinados.Split(':');
    string nombreEscena = partes[0];
    string idPuntoLlegada = partes.Length > 1 ? partes[1] : "";

    Time.timeScale = 1; 
if (GestorCursor.Instance != null) 
        {
            GestorCursor.Instance.OcultarCursor();
        }
    if (GameManager.Instance != null)
    {
        // Guardamos el punto de aterrizaje en el "cerebro" inmortal
        GameManager.Instance.idPuertaDestino = idPuntoLlegada;
    }
    // --- NUEVO: Sonido de salida ---
        if (audioViajeSalida != null)
        {
            audioViajeSalida.Play();
        }
        
        // Avisamos de que el siguiente cambio de escena es por viaje rápido
        viajeRapidoEnCurso = true;

    SceneManager.LoadScene(nombreEscena); 
}
}