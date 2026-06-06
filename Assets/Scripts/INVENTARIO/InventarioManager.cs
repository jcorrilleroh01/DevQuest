using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventarioManager : MonoBehaviour
{
    public static InventarioManager Instance;

    [System.Serializable]
    public struct OutfitData
    {
        [Tooltip("Pon aquí el nombre EXACTO del archivo de Animator (Ej: Pijama, TrajeTrabajo)")]
        public string nombreOutfit; 
        public Sprite fotoFrontal;  
    }

    [Header("Configuración Automática")]
    public string tagJugador = "Player"; 
    
    [Header("Referencias de la UI")]
    public GameObject panelInventario;
    public Image displayPersonajeOutfit;
    public Transform contenedorCuadricula; 

    [Header("Base de Datos Visual de Outfits")]
    public OutfitData[] listaDeOutfits;
    
    [Header("Panel de Detalles")]
    public Image iconoGrandeDetalle; 
    public TextMeshProUGUI textoTituloItem; 
    public TextMeshProUGUI textoDescripcionTMP;

    [Header("HUD Gameplay")]
    public GameObject objetoHUDCompleto; 
    public GameObject puntoNotificacionHUD; 

    [Header("Sistema de Auto-Ocultar HUD")]
    [Tooltip("Arrastra aquí los paneles (Mapa Global, Armario, Combate, etc.) que deben ocultar el HUD cuando se abran.")]
    public GameObject[] panelesQueOcultanElHUD; // ¡LA NUEVA LISTA!

    [Header("Inventario Dinámico")]
    public List<ItemArma> armasAdquiridas = new List<ItemArma>(); 

    [Header("Sistema de Navegación por Pestañas")]
    public Image imagenFondoPrincipal; 
    public Sprite[] fondosPestanas; 
    public GameObject[] panelesContenido;

    private Image[] slotsImagenes;
    private Button[] slotsBotones;

 void Awake()
    {
        // 1. Si ya existe un inventario, destruimos TODA la jerarquía nueva, no solo este script
        if (Instance != null && Instance != this)
        {
            // 🛡️ IMPORTANTE: Destruimos el objeto raíz (transform.root) 
            // para llevarnos por delante los paneles y HUDs duplicados
            Destroy(transform.root.gameObject); 
            return; 
        }

        Instance = this;

        // 2. Hacemos inmortal a la raíz para que viajen todos los Canvas juntos
        if (transform.root.gameObject.scene.name != "DontDestroyOnLoad")
        {
            DontDestroyOnLoad(transform.root.gameObject);
        }
    }

    void Start()
    {
        if (contenedorCuadricula != null)
        {
            slotsImagenes = contenedorCuadricula.GetComponentsInChildren<Image>(); 
            slotsBotones = contenedorCuadricula.GetComponentsInChildren<Button>();
        }

        LimpiarPanelDetalles();
        if (panelInventario != null) panelInventario.SetActive(false);
        
        if(puntoNotificacionHUD != null) puntoNotificacionHUD.SetActive(false);
        
        // El HUD se encenderá automáticamente en el Update si no hay paneles bloqueando
        
        ActualizarUIInventario();
    }

    void Update()
    {
        // 1. Lógica de abrir/cerrar con la tecla 'I'
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (panelInventario == null) return; 

            bool estaAbriendo = !panelInventario.activeSelf;
            panelInventario.SetActive(estaAbriendo);
            
            if (estaAbriendo)
            {
                if(puntoNotificacionHUD != null) puntoNotificacionHUD.SetActive(false);
                
                // 1. Detectamos y ponemos la foto ANTES de pausar el tiempo
                DetectarOutfitActual(); 
                
                // 2. Actualizamos items
                ActualizarUIInventario();
                
                // 3. Pausamos el juego
                Time.timeScale = 0; 

                if (GestorCursor.Instance != null) GestorCursor.Instance.MostrarCursor();
            }
            else
            {
                Time.timeScale = 1;
                if (GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor();
            }
        }

        // 2. ¡NUEVO! Comprobación constante del estado del HUD
        ComprobarAutoOcultarHUD();
    }

    // --- FUNCIÓN DE VIGILANCIA DEL HUD ---
    void ComprobarAutoOcultarHUD()
    {
        if (objetoHUDCompleto == null) return;

        // Si el inventario propio está abierto, el HUD debe apagarse
        if (panelInventario != null && panelInventario.activeSelf)
        {
            objetoHUDCompleto.SetActive(false);
            return;
        }

        // Revisamos la lista de los demás paneles
        bool debeEstarOculto = false;
        if (panelesQueOcultanElHUD != null && panelesQueOcultanElHUD.Length > 0)
        {
            foreach (GameObject panel in panelesQueOcultanElHUD)
            {
                // Si encontramos UN SOLO panel abierto, marcamos que debe ocultarse y paramos de buscar
                if (panel != null && panel.activeSelf)
                {
                    debeEstarOculto = true;
                    break; 
                }
            }
        }

        // Finalmente, encendemos o apagamos el HUD según lo que hayamos detectado
        objetoHUDCompleto.SetActive(!debeEstarOculto);
    }

    // --- LA MAGIA AUTOMÁTICA DEL OUTFIT ---
    public void DetectarOutfitActual()
    {
        if (displayPersonajeOutfit == null) return;

        GameObject jaimeReal = GameObject.FindGameObjectWithTag(tagJugador);
        
        if (jaimeReal != null)
        {
            Animator animJugador = jaimeReal.GetComponent<Animator>();
            
            if (animJugador != null && animJugador.runtimeAnimatorController != null)
            {
                string nombreControladorActual = animJugador.runtimeAnimatorController.name;
                nombreControladorActual = nombreControladorActual.Replace("(Clone)", "").Trim();

                bool fotoEncontrada = false;

                foreach (OutfitData outfit in listaDeOutfits)
                {
                    if (outfit.nombreOutfit == nombreControladorActual)
                    {
                        displayPersonajeOutfit.sprite = outfit.fotoFrontal;
                        displayPersonajeOutfit.preserveAspect = true;
                        fotoEncontrada = true;
                        break; 
                    }
                }

                if (!fotoEncontrada)
                {
                    Debug.LogWarning("Inventario: Falta configurar la foto para el outfit llamado: " + nombreControladorActual);
                }
            }
        }
    }

    public void AñadirNuevaArma(ItemArma nuevaArma)
    {
        if (nuevaArma != null && !armasAdquiridas.Contains(nuevaArma))
        {
            armasAdquiridas.Add(nuevaArma);
            nuevaArma.desbloqueada = true; 
            if (GestorLogros.Instance != null)
            {
                GestorLogros.Instance.ComprobarLogrosMaestros();
            }
            ActualizarUIInventario(); 
            
            if(puntoNotificacionHUD != null) puntoNotificacionHUD.SetActive(true);
        }
    }

    public void ActualizarUIInventario()
    {
        if (slotsImagenes == null || slotsBotones == null) return; 

        for (int i = 0; i < slotsImagenes.Length; i++)
        {
            if (i < armasAdquiridas.Count)
            {
                slotsImagenes[i].sprite = armasAdquiridas[i].icono;
                slotsImagenes[i].color = Color.white; 
                slotsBotones[i].interactable = true; 
            }
            else
            {
                slotsImagenes[i].sprite = null;
                slotsImagenes[i].color = new Color(0, 0, 0, 0); 
                slotsBotones[i].interactable = false; 
            }
        }
    }

    public void SeleccionarItem(int index)
    {
        if (index < armasAdquiridas.Count)
        {
            if(iconoGrandeDetalle != null) 
            {
                iconoGrandeDetalle.sprite = armasAdquiridas[index].icono;
                iconoGrandeDetalle.color = Color.white; 
            }
            if(textoTituloItem != null) textoTituloItem.text = armasAdquiridas[index].nombre;
            if(textoDescripcionTMP != null) textoDescripcionTMP.text = armasAdquiridas[index].descripcion;
        }
    }

    private void LimpiarPanelDetalles()
    {
        if(iconoGrandeDetalle != null) iconoGrandeDetalle.color = new Color(1, 1, 1, 0); 
        if(textoTituloItem != null) textoTituloItem.text = ""; 
        if(textoDescripcionTMP != null) textoDescripcionTMP.text = ""; 
    }

    public void CambiarPestana(int indice)
    {
        if (imagenFondoPrincipal != null && indice < fondosPestanas.Length)
        {
            imagenFondoPrincipal.sprite = fondosPestanas[indice];
        }

        if (panelesContenido != null)
        {
            for (int i = 0; i < panelesContenido.Length; i++)
            {
                if (panelesContenido[i] != null)
                {
                    panelesContenido[i].SetActive(i == indice);
                }
            }
        }
        
        LimpiarPanelDetalles();
    }
}