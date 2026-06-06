using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class CofreContrasena : MonoBehaviour
{
    [Header("Identificador Único del Cofre")]
    public string idCofre = "Cofre_Cesur_Contrasena_01"; 

    [Header("Componentes de UI y Puzzle")]
    public UI_TecladoNumerico panelTecladoUI; 
    public FeedbackInteractuable feedback; 
    public Animator animatorCofre;
public AudioSource sonidoAbrir;
    [Header("Explosión de Tesoros")]
    public GameObject[] itemsFlotantes; 
    public float alturaExplosion = 2.5f; 
    public float separacionHorizontal = 1.5f; 
    public float duracionExplosion = 0.6f; 

    [Header("La Recompensa (UI Temporal)")]
    public Sprite spriteCandadoAbierto;
    public GameObject panelRecompensa;
    public Image[] imagenesEnElPanel; 
    public TextMeshProUGUI textoMensajeUI; 
    public string textoMensaje = "¡Has desbloqueado el Arsenal Superior!";

    [Header("Datos Lógicos del Inventario")]
    public ItemArma itemDataArma1; 
    public ItemArma itemDataArma2; 
    public ItemArma itemDataArma3; 
    public Sprite iconoArma1; 
    public Sprite iconoArma2;
    public Sprite iconoArma3;

    private bool estaAbierto = false;
    private bool enRango = false;

   void Start()
    {
        // 1. DESCONGELAR EL TIEMPO (Por si venimos de un error en otra escena)
        Time.timeScale = 1f;

        // 2. APAGADO SEGURO ANTI-ERRORES
        if (itemsFlotantes != null) 
        {
            foreach(GameObject item in itemsFlotantes) 
            {
                if(item != null) item.SetActive(false);
            }
        }
        
        if(panelRecompensa != null) panelRecompensa.SetActive(false);
        if(panelTecladoUI != null) panelTecladoUI.gameObject.SetActive(false);

        // 3. PERSISTENCIA
        if (GameManager.Instance != null && GameManager.Instance.CofreYaEstaAbierto(idCofre))
        {
            estaAbierto = true;
            if (animatorCofre != null) animatorCofre.Play("COFREABIERTO");            
            if (feedback != null) feedback.OcultarPorAccion();
        }
    }

    void Update()
    {
        if (enRango && !estaAbierto && Input.GetKeyDown(KeyCode.E))
        {
            if (panelTecladoUI != null && !panelTecladoUI.gameObject.activeSelf)
            {
                panelTecladoUI.AbrirTeclado();
                if (feedback != null) feedback.OcultarPorAccion();
                
                // ¡CORREGIDO! Mostramos el ratón para poder teclear la contraseña
                if (GestorCursor.Instance != null) GestorCursor.Instance.MostrarCursor();
            }
        }
    }

    public void DesbloquearPorContrasena()
    {
        Debug.Log("1. El teclado ha llamado a DesbloquearPorContrasena()");
        if (estaAbierto) return;

        // ¡CORREGIDO! Ocultamos el ratón para ver la animación de abrir el cofre
        if (GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor();

        StartCoroutine(SecuenciaAbrirCofre());
    }

    IEnumerator SecuenciaAbrirCofre()
    {
        Debug.Log("2. Iniciando Corrutina de Apertura");
        estaAbierto = true;
        
        GameManager.Instance.DesbloquearAtaquesPorArma("GUANTES");
        GameManager.Instance.DesbloquearAtaquesPorArma("CHAKRAM");
        GameManager.Instance.DesbloquearAtaquesPorArma("TRIDENTE");
        
        if (animatorCofre != null) 
        {
            animatorCofre.SetTrigger("estaAbierto");
            Debug.Log("3. Trigger 'Abrir' enviado al Animator");
        }
        else Debug.LogError("ERROR: ¡No has asignado el AnimatorCofre en el Inspector!");
        
        yield return new WaitForSeconds(0.4f); 
        if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }

        Debug.Log("4. Preparando explosión de " + itemsFlotantes.Length + " items");
        if (itemsFlotantes.Length == 0) Debug.LogError("ERROR: ¡El array de Items Flotantes está vacío en el Inspector!");

        Vector3[] posicionesIniciales = new Vector3[itemsFlotantes.Length];
        Vector3[] posicionesFinales = new Vector3[itemsFlotantes.Length];

        for (int i = 0; i < itemsFlotantes.Length; i++)
        {
            if(itemsFlotantes[i] != null) 
            {
                itemsFlotantes[i].SetActive(true);
                posicionesIniciales[i] = itemsFlotantes[i].transform.position;
                float direccionX = 0f;
                if (i == 0) direccionX = -separacionHorizontal;      
                else if (i == 1) direccionX = 0f;                    
                else if (i == 2) direccionX = separacionHorizontal;  
                posicionesFinales[i] = posicionesIniciales[i] + new Vector3(direccionX, alturaExplosion, 0);
            }
        }

        float tiempo = 0;
        while (tiempo < duracionExplosion)
        {
            tiempo += Time.deltaTime;
            float porcentaje = tiempo / duracionExplosion;
            float curvaFrenado = Mathf.Sin(porcentaje * Mathf.PI * 0.5f);

            for (int i = 0; i < itemsFlotantes.Length; i++)
            {
                if(itemsFlotantes[i] != null)
                {
                    itemsFlotantes[i].transform.position = Vector3.Lerp(posicionesIniciales[i], posicionesFinales[i], curvaFrenado);
                }
            }
            yield return null; 
        }

        Debug.Log("5. Explosión terminada, abriendo Panel de Recompensa");
        yield return new WaitForSeconds(0.3f);
        AbrirPanelRecompensa();
    }

    void AbrirPanelRecompensa()
    {
        if(panelRecompensa != null)
        {
            panelRecompensa.SetActive(true);
            
            // Ya lo tenías bien: Mostramos el ratón para pulsar 'Aceptar'
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
            Time.timeScale = 0; // Pausa el juego
        }
        else Debug.LogError("ERROR: No has asignado el Panel Recompensa en el Inspector.");

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

        if (GameManager.Instance != null) GameManager.Instance.GuardarCofreAbierto(idCofre);
    }

    public void CerrarPanelRecompensa()
    {
        Debug.Log("Botón Aceptar pulsado: Cerrando panel y reanudando tiempo.");
        Time.timeScale = 1; // Reanuda el juego
        if(panelRecompensa != null) panelRecompensa.SetActive(false);
        
        // Ya lo tenías bien: Ocultamos el ratón al terminar
        if (GestorCursor.Instance != null) {
            GestorCursor.Instance.OcultarCursor();
        }

        foreach(GameObject item in itemsFlotantes) if(item != null) item.SetActive(false); 
        
        if (NotificacionManager.Instance != null)
        {
            NotificacionManager.Instance.MostrarNotificacion("NUEVAS ARMAS ADQUIRIDAS", spriteCandadoAbierto);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) 
    { 
        if(col.CompareTag("Player")) 
        { 
            enRango = true; 
            if(feedback != null && !estaAbierto && !panelTecladoUI.gameObject.activeSelf) feedback.MostrarPorFinAccion(); 
        } 
    }

    private void OnTriggerExit2D(Collider2D col) 
    { 
        if(col.CompareTag("Player")) 
        { 
            enRango = false; 
            if(feedback != null) feedback.OcultarPorAccion(); 
            if(panelTecladoUI != null) 
            {
                panelTecladoUI.CerrarTeclado();
                
                // ¡CORREGIDO! Si nos alejamos y se cierra el teclado, ocultamos el ratón
                if (GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor();
            } 
        } 
    }
}