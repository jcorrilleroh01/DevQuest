using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class CofreAgoraDAW : MonoBehaviour
{
    [Header("Identificador Único del Cofre")]
    public string idCofre = "Cofre_Agora_DAW_01"; 

    [Header("Componentes del Cofre")]
    public Animator animatorCofre;
    public FeedbackInteractuable feedback; 
    public AudioSource sonidoAbrir;
    [Header("Animación de los Tesoros")]
    public GameObject[] itemsFlotantes; 
    public float velocidadFlotar = 2f;
    public float alturaFlotar = 1.5f;

    [Header("La Recompensa (UI Temporal)")]
    public GameObject panelRecompensa;
    public Image[] imagenesEnElPanel; 
    public TextMeshProUGUI textoMensajeUI; 

    [Header("Datos Lógicos del Inventario (Moldes)")]
    public ItemArma itemDataArma1; 
    public ItemArma itemDataArma2; 
    public ItemArma itemDataArma3; 

    [Header("Iconos para el Panel UI")]
    public Sprite iconoArma1; 
    public Sprite iconoArma2;
    public Sprite iconoArma3;
    public string textoMensaje = "¡Has desbloqueado el Arsenal DAW!";

    [Header("--- NUEVO: SISTEMA DE BLOQUEO POR PALANCAS ---")]
    public InteractuablePalanca palanca1; // Arrastra aquí la Palanca 1
    public InteractuablePalanca palanca2; // Arrastra aquí la Palanca 2
    public GameObject panelDialogoBloqueado; // Arrastra tu panel de "¡Cofre Bloqueado!"
    public TextMeshProUGUI textoDialogoBloqueado; // (Opcional) Por si quieres cambiar el texto por código
    public float tiempoMensajeBloqueado = 2f; // Cuánto tiempo se muestra el mensaje

    private bool estaAbierto = false;
    private bool enRango = false;
    private bool mostrandoMensajeBloqueo = false; // Evita que se solapen muchos mensajes
    public Sprite spriteCandadoAbierto;

    void Start()
    {
        foreach(GameObject item in itemsFlotantes) if(item != null) item.SetActive(false);
        if(panelRecompensa != null) panelRecompensa.SetActive(false);
        if(panelDialogoBloqueado != null) panelDialogoBloqueado.SetActive(false); // Nos aseguramos de que el diálogo empiece apagado

        if (GameManager.Instance != null && GameManager.Instance.CofreYaEstaAbierto(idCofre))
        {
            estaAbierto = true;
            if (animatorCofre != null) animatorCofre.Play("COFREAGORAFIJO"); 
            if (feedback != null) feedback.OcultarPorAccion();
        }
    }

    void Update()
    {
        // Si el jugador pulsa E estando en rango y el cofre no está abierto...
        if (enRango && !estaAbierto && Input.GetKeyDown(KeyCode.E))
        {
            // COMPROBAMOS LAS PALANCAS
            if (palanca1 != null && palanca2 != null && palanca1.activada && palanca2.activada)
            {
                // Ambas están activadas, abrimos el cofre
                StartCoroutine(SecuenciaAbrirCofre());
                GameManager.Instance.DesbloquearAtaquesPorArma("MAZO");
                                GameManager.Instance.DesbloquearAtaquesPorArma("BACULO");
                GameManager.Instance.DesbloquearAtaquesPorArma("DAGAS");


            }
            else
            {
                // Falta alguna palanca, mostramos el diálogo
                if (!mostrandoMensajeBloqueo)
                {
                    StartCoroutine(MostrarDialogoBloqueado());
                }
            }
        }
    }

    IEnumerator MostrarDialogoBloqueado()
    {
        mostrandoMensajeBloqueo = true;
        
        // Encendemos el panel y ponemos el texto (opcional)
        if (panelDialogoBloqueado != null) 
        {
            panelDialogoBloqueado.SetActive(true);
            if (textoDialogoBloqueado != null) textoDialogoBloqueado.text = "¡Cofre Bloqueado! Parece que necesita energía...";
        }

        // Esperamos unos segundos
        yield return new WaitForSeconds(tiempoMensajeBloqueado);

        // Lo apagamos
        if (panelDialogoBloqueado != null) panelDialogoBloqueado.SetActive(false);
        mostrandoMensajeBloqueo = false;
    }

    IEnumerator SecuenciaAbrirCofre()
    {
        estaAbierto = true;
        if(feedback != null) feedback.OcultarPorAccion();
        if (animatorCofre != null) animatorCofre.SetTrigger("Abrir");
        
        yield return new WaitForSeconds(0.5f);
        if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }

        for (int i = 0; i < itemsFlotantes.Length; i++)
        {
            if(itemsFlotantes[i] != null) itemsFlotantes[i].SetActive(true);
        }

        float tiempo = 0;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime * velocidadFlotar;
            for (int i = 0; i < itemsFlotantes.Length; i++)
            {
                if(itemsFlotantes[i] != null)
                {
                    itemsFlotantes[i].transform.position += Vector3.up * (alturaFlotar * Time.deltaTime);
                }
            }
            yield return null; 
        }

        AbrirPanelRecompensa();
    }

    void AbrirPanelRecompensa()
    {
        if(panelRecompensa != null)
        {
            panelRecompensa.SetActive(true);
            if (GestorCursor.Instance != null) {
            GestorCursor.Instance.MostrarCursor();
        }
            
            if(imagenesEnElPanel.Length >= 3)
            {
                imagenesEnElPanel[0].sprite = iconoArma1;
                imagenesEnElPanel[1].sprite = iconoArma2;
                imagenesEnElPanel[2].sprite = iconoArma3; 
            }
            if(textoMensajeUI != null) textoMensajeUI.text = textoMensaje;
            Time.timeScale = 0; 
        }

        if (itemDataArma1 != null) itemDataArma1.desbloqueada = true;
        if (itemDataArma2 != null) itemDataArma2.desbloqueada = true;
        if (itemDataArma3 != null) itemDataArma3.desbloqueada = true; 

        if (InventarioManager.Instance != null)
        {
            if (itemDataArma1 != null) InventarioManager.Instance.AñadirNuevaArma(itemDataArma1);
            if (itemDataArma2 != null) InventarioManager.Instance.AñadirNuevaArma(itemDataArma2);
            if (itemDataArma3 != null) InventarioManager.Instance.AñadirNuevaArma(itemDataArma3); 
            InventarioManager.Instance.ActualizarUIInventario();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GuardarCofreAbierto(idCofre);
        }
    }

    public void CerrarPanel()
    {
        Time.timeScale = 1; 
        if(panelRecompensa != null) panelRecompensa.SetActive(false);
        if (GestorCursor.Instance != null) {
            GestorCursor.Instance.OcultarCursor();
        }
        foreach(GameObject item in itemsFlotantes) if(item != null) item.SetActive(false); 
                    NotificacionManager.Instance.MostrarNotificacion("NUEVAS ARMAS ADQUIRIDAS", spriteCandadoAbierto);

    }

    private void OnTriggerEnter2D(Collider2D col) { if(col.CompareTag("Player")) enRango = true; }
    private void OnTriggerExit2D(Collider2D col) { if(col.CompareTag("Player")) enRango = false; }
}