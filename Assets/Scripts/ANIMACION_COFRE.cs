using UnityEngine;
using System.Collections;
using TMPro; 
using UnityEngine.UI;

public class CofreSorpresa : MonoBehaviour
{
    [Header("NUEVO: Identificador Único del Cofre")]
    public string idCofre = "Cofre_Cuarto_Ropa"; 
[Header("Efectos de Sonido")]
    public AudioSource sonidoAbrir;
    [Header("Componentes del Cofre")]
    public Animator animatorCofre; 
    public FeedbackInteractuable feedback; 
    public Sprite spriteCandadoAbierto;
    [Header("La Animación del Tesoro")]
    public GameObject itemFlotante; 
    public float velocidadFlotar = 2f;
    public float alturaFlotar = 1.5f;

    [Header("La Recompensa (UI)")]
    public GameObject panelRecompensa; 
    public Image imagenEnElPanel; 
    public TextMeshProUGUI textoNombreOutfit; 

    [Header("Datos del Outfit")]
    public RuntimeAnimatorController outfitADesbloquear; 
    public Sprite iconoDelOutfit; 
    public string nombreDelOutfit = "Ropa de calle";
    public string idParaDesbloquear = "RopaCalle";

    private bool estaAbierto = false;
    private bool enRango = false;

    void Start()
    {
        if(itemFlotante != null) itemFlotante.SetActive(false);
        if(panelRecompensa != null) panelRecompensa.SetActive(false);

        // --- NUEVO: PREGUNTAR AL GAMEMANAGER ---
        if (GameManager.Instance != null && GameManager.Instance.CofreYaEstaAbierto(idCofre))
        {
            estaAbierto = true;

            if (animatorCofre != null) animatorCofre.Play("COFRESPAWNABIERTO"); // O "Open", según se llame tu estado final
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

        if(feedback != null) feedback.OcultarPorAccion();
        if(animatorCofre != null) animatorCofre.SetTrigger("Abrir");

        yield return new WaitForSeconds(0.2f);

        if(itemFlotante != null)
        {
            itemFlotante.SetActive(true);
            itemFlotante.GetComponent<SpriteRenderer>().sprite = iconoDelOutfit;

            Vector3 posInicial = itemFlotante.transform.position;
            Vector3 posFinal = posInicial + Vector3.up * alturaFlotar;
            float tiempo = 0;

            while (tiempo < 1f)
            {
                tiempo += Time.deltaTime * velocidadFlotar;
                itemFlotante.transform.position = Vector3.Lerp(posInicial, posFinal, tiempo);
                yield return null; 
            }
        }

        AbrirPanelRecompensa();
    }

   void AbrirPanelRecompensa()
    {
        if(panelRecompensa != null)
        {
            panelRecompensa.SetActive(true);
            GestorCursor.Instance.MostrarCursor();
            if(imagenEnElPanel != null) imagenEnElPanel.sprite = iconoDelOutfit;
            if(textoNombreOutfit != null) textoNombreOutfit.text = "¡Obtuviste: " + nombreDelOutfit + "!";
            
            // Aquí congelamos el juego
            Time.timeScale = 0;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.DesbloquearOutfit(idParaDesbloquear);
            GameManager.Instance.GuardarCofreAbierto(idCofre);
            // ¡Hemos quitado la notificación de aquí!
        }
    }

    public void CerrarPanel()
    {
        // 1. El tiempo vuelve a fluir
        Time.timeScale = 1; 
        
        // 2. Cerramos el panel central gigante
        if(panelRecompensa != null) panelRecompensa.SetActive(false);
        if(itemFlotante != null) itemFlotante.SetActive(false); 

        // 3. ¡AHORA SÍ! Lanzamos la notificación deslizante limpia y sin interrupciones
        if (NotificacionManager.Instance != null)
        {
            NotificacionManager.Instance.MostrarNotificacion("NUEVO OUTFIT LISTO PARA EQUIPAR", spriteCandadoAbierto);
        }
        GestorCursor.Instance.OcultarCursor();
    }

    private void OnTriggerEnter2D(Collider2D col) { if(col.CompareTag("Player")) enRango = true; }
    private void OnTriggerExit2D(Collider2D col) { if(col.CompareTag("Player")) enRango = false; }
}