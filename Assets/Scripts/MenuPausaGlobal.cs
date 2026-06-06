using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausaGlobal : MonoBehaviour
{
    public static MenuPausaGlobal Instance;
public JugadorStats statsJaime;
    [Header("UI del Menú")]
    [Tooltip("Arrastra aquí el panel visual que contiene los botones")]
    public GameObject panelPausa;

    private bool estaPausado = false;

    void Awake()
    {
        // 1. Convertirlo en un Singleton Global que no se destruye
        if (Instance == null)
        {
            Instance = this;
            // IMPORTANTE: Para que DontDestroyOnLoad funcione bien, 
            // este script debe estar en el objeto raíz (el Canvas completo)
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); // Destruye duplicados si volvemos a la escena inicial
        }
    }

    void Start()
    {
        // Asegurarnos de que empiece apagado y el tiempo corra normal
        if (panelPausa != null) panelPausa.SetActive(false);
        estaPausado = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        // Detectar la tecla ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPausa();
        }
    }

    public void AlternarPausa()
    {
        if (panelPausa == null) return;

        estaPausado = !estaPausado;
        panelPausa.SetActive(estaPausado);

        if (estaPausado)
        {
            // Pausar el juego y mostrar el ratón
            Time.timeScale = 0f;
            if (GestorCursor.Instance != null) GestorCursor.Instance.MostrarCursor();
                        if (InventarioManager.Instance != null && InventarioManager.Instance.objetoHUDCompleto != null) InventarioManager.Instance.objetoHUDCompleto.SetActive(false);

        }
        else
        {
            // Reanudar el juego y ocultar el ratón
            Time.timeScale = 1f;
            if (GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor();
                        if (InventarioManager.Instance != null && InventarioManager.Instance.objetoHUDCompleto != null) InventarioManager.Instance.objetoHUDCompleto.SetActive(true);
        }
    }

    // --- FUNCIONES PARA LOS BOTONES ---

    // Asigna esta función al evento OnClick() de tu botón "Reanudar"
    public void BotonReanudar()
    {
        AlternarPausa();
    }

    // Asigna esta función al evento OnClick() de tu botón "Salir"
    public void BotonSalirYReestablecer()
    {
        Debug.Log("Reestableciendo progreso y saliendo del juego...");

        // 1. REESTABLECER TODO: Borra todo el progreso guardado (Cofres, armas, etc.)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // 2. SALIR DEL JUEGO
        // Esto cierra el juego compilado (.exe o WebGL)
        if (statsJaime != null)
        {
            statsJaime.vidaMax = 20; // O el valor base que hayas decidido
            statsJaime.vidaActual = 20; 
        }

        AtaqueBase[] todosLosAtaques = Resources.LoadAll<AtaqueBase>("Ataques/BUENOS");

        foreach (AtaqueBase ataque in todosLosAtaques)
        {
            // Escribe aquí los NOMBRES EXACTOS de los ataques con los que empiezas el juego
            if (ataque.nombreAtaque == "Almohadazo" || ataque.nombreAtaque == "Sueño Profundo")
            {
                ataque.descubierto = true; // Estos se quedan desbloqueados
            }
            else
            {
                ataque.descubierto = false; // Todos los demás se bloquean
            }
        }
        Debug.Log("Ataques reseteados correctamente.");
        Application.Quit();

        // TRUCO DE DESARROLLADOR: Esto detiene el modo "Play" dentro del editor de Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}