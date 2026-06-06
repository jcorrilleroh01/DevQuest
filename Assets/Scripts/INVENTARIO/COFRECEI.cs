using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class CofreTripleSorpresa : MonoBehaviour
{
    [Header("Identificador Único del Cofre")]
    [Tooltip("Pon un nombre único. Ej: Cofre_Lab_01")]
    public string idCofre = "Cofre_CEI_Principal"; 

    [Header("Componentes del Cofre")]
    public Animator animatorCofre;
    public FeedbackInteractuable feedback; 
    
    [Header("Animación de los Tesoros")]
    public GameObject[] itemsFlotantes; 
    public float velocidadFlotar = 2f;
    public float alturaFlotar = 1.5f;

    [Header("La Recompensa (UI Temporal)")]
    public GameObject panelRecompensa;
    public Image[] imagenesEnElPanel; 
    public TextMeshProUGUI textoMensajeUI; 
public AudioSource sonidoAbrir;
    [Header("Datos Lógicos del Inventario")]
    public ItemArma itemDataArma1; 
    public ItemArma itemDataArma2; 
    public string idOutfitADesbloquear = "Varsity"; 

    public Sprite iconoArma1; 
    public Sprite iconoArma2;
    public Sprite iconoOutfit;
    public string textoMensaje = "¡Has obtenido nuevo equipamiento academico!";
public Sprite spriteCandadoAbierto;
public Sprite spriteCandadoAbierto2;

    private bool estaAbierto = false;
    private bool enRango = false;

    void Start()
    {
        foreach(GameObject item in itemsFlotantes) if(item != null) item.SetActive(false);
        if(panelRecompensa != null) panelRecompensa.SetActive(false);

        // --- CORREGIDO: Usamos el nombre exacto de tu estado fijo ---
        if (GameManager.Instance != null && GameManager.Instance.CofreYaEstaAbierto(idCofre))
        {
            estaAbierto = true;
            if (animatorCofre != null) animatorCofre.Play("COFRECCEIABIERTO"); 
            if (feedback != null) feedback.OcultarPorAccion();
        }
    }

    void Update()
    {
        if (enRango && !estaAbierto && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SecuenciaAbrirCofre());
        }
    }

    IEnumerator SecuenciaAbrirCofre()
    {
        if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }
        estaAbierto = true;
        GameManager.Instance.DesbloquearAtaquesPorArma("LLAVE");
                                GameManager.Instance.DesbloquearAtaquesPorArma("BASTON");
        if(feedback != null) feedback.OcultarPorAccion();
        
        // --- CORREGIDO: Forzamos la animación de abrir por nombre directo (Play) ---
        // Usamos Play en lugar de SetTrigger para no depender de la pestaña Parameters
animatorCofre.SetTrigger("Abrir");
        yield return new WaitForSeconds(0.2f);

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
                imagenesEnElPanel[2].sprite = iconoOutfit;
            }
            if(textoMensajeUI != null) textoMensajeUI.text = textoMensaje;
            Time.timeScale = 0; 
        }

        if (itemDataArma1 != null) itemDataArma1.desbloqueada = true;
        if (itemDataArma2 != null) itemDataArma2.desbloqueada = true;

        if (InventarioManager.Instance != null)
        {
            InventarioManager.Instance.ActualizarUIInventario();
            if (itemDataArma1 != null) InventarioManager.Instance.AñadirNuevaArma(itemDataArma1);
            if (itemDataArma2 != null) InventarioManager.Instance.AñadirNuevaArma(itemDataArma2);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DesbloquearOutfit(idOutfitADesbloquear);
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
        if (NotificacionManager.Instance != null)
        {
            NotificacionManager.Instance.MostrarNotificacion("NUEVO OUTFIT LISTO PARA EQUIPAR", spriteCandadoAbierto);
                        NotificacionManager.Instance.MostrarNotificacion("NUEVAS ARMAS ADQUIRIDAS", spriteCandadoAbierto2);

        }
    }

    private void OnTriggerEnter2D(Collider2D col) { if(col.CompareTag("Player")) enRango = true; }
    private void OnTriggerExit2D(Collider2D col) { if(col.CompareTag("Player")) enRango = false; }
}