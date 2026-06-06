using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UILogrosPanel : MonoBehaviour
{
    [Header("Prefabs y Contenedores UI")]
    public GameObject prefabNodoLogro;
    public GameObject prefabLineaRama;
    public RectTransform contenedorMapa;
    
    [Header("Scroll de la Ventana")]
    [Tooltip("Arrastra aquí tu Scroll View para que al abrir se vaya al principio")]
    public ScrollRect scrollMapa; // 🛡️ EL NUEVO CONTROLADOR DE SCROLL

    [Header("Panel de Detalles")]
    public Image iconoDetalle;
    public TextMeshProUGUI tituloDetalleTMP;
    public TextMeshProUGUI descripcionDetalleTMP;

    private Button primerBotonLogro;

    void OnEnable()
    {
        if (GestorLogros.Instance != null)
        {
            GestorLogros.Instance.OnLogrosActualizados -= GenerarArbolVisual; 
            GestorLogros.Instance.OnLogrosActualizados += GenerarArbolVisual;
            GenerarArbolVisual();
        }
    }

    void OnDisable()
    {
        if (GestorLogros.Instance != null) GestorLogros.Instance.OnLogrosActualizados -= GenerarArbolVisual;
    }

    public void GenerarArbolVisual()
    {
        if (GestorLogros.Instance == null) return;

        foreach (Transform hijo in contenedorMapa) Destroy(hijo.gameObject);

        // Dibujamos Líneas
        foreach (GestorLogros.LogroData logro in GestorLogros.Instance.listaLogrosCache.logros)
        {
            if (!string.IsNullOrEmpty(logro.padreId) && GestorLogros.Instance.diccionarioLogros.ContainsKey(logro.padreId))
            {
                DibujarLinea(GestorLogros.Instance.diccionarioLogros[logro.padreId], logro);
            }
        }

        // Dibujamos Botones
        bool esElPrimero = true;
        foreach (GestorLogros.LogroData logro in GestorLogros.Instance.listaLogrosCache.logros)
        {
            Button btnGenerado = InstanciarNodo(logro);
            if (esElPrimero && btnGenerado != null)
            {
                primerBotonLogro = btnGenerado;
                esElPrimero = false;
            }
        }

        if (primerBotonLogro != null) StartCoroutine(FocusConRetraso());
    }

    IEnumerator FocusConRetraso()
    {
        // 1. Esperamos a que termine el frame actual
        yield return new WaitForEndOfFrame();
        
        // 2. 🛡️ EL TRUCO: Obligamos a Unity a recalcular los tamaños de los botones AHORA MISMO
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(contenedorMapa);
        
        // 3. Esperamos un frame extra para asegurarnos de que la matemática se aplicó
        yield return new WaitForEndOfFrame();

        // 4. Ahora el ScrollRect sí sabe cuánto mide y obedece perfectamente
        if (scrollMapa != null)
        {
            // Apagamos la inercia un segundo para que el salto sea instantáneo
            Vector2 velocidadAntigua = scrollMapa.velocity;
            scrollMapa.velocity = Vector2.zero;
            
            scrollMapa.horizontalNormalizedPosition = 0f; // 0 = Izquierda del todo
            scrollMapa.verticalNormalizedPosition = 1f;   // 1 = Arriba del todo
            
            scrollMapa.velocity = velocidadAntigua;
        }

        // 5. Clicamos visual y lógicamente el primer botón
        if (primerBotonLogro != null)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(primerBotonLogro.gameObject);
            }
            primerBotonLogro.onClick.Invoke();
        }
    }

    void DibujarLinea(GestorLogros.LogroData padre, GestorLogros.LogroData hijo)
    {
        GameObject lineaObj = Instantiate(prefabLineaRama);
        lineaObj.transform.SetParent(contenedorMapa, false);
        RectTransform rtLinea = lineaObj.GetComponent<RectTransform>();
        rtLinea.localScale = Vector3.one;
        Vector2 direccion = new Vector2(hijo.x, hijo.y) - new Vector2(padre.x, padre.y);
        rtLinea.sizeDelta = new Vector2(direccion.magnitude, 40f);
        rtLinea.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg);
        rtLinea.anchoredPosition = new Vector2(padre.x, padre.y) + (direccion / 2);
    }

    Button InstanciarNodo(GestorLogros.LogroData logro)
    {
        GameObject nodoObj = Instantiate(prefabNodoLogro);
        nodoObj.transform.SetParent(contenedorMapa, false);
        RectTransform rtNodo = nodoObj.GetComponent<RectTransform>();
        rtNodo.localScale = Vector3.one;
        rtNodo.anchoredPosition = new Vector2(logro.x, logro.y);

        Sprite spriteIcono = Resources.Load<Sprite>("Logros/" + logro.icono);
        Image imagenNodo = nodoObj.GetComponent<Image>();
        if (spriteIcono != null) imagenNodo.sprite = spriteIcono;
        imagenNodo.color = logro.desbloqueado ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f); 

        // 🛡️ PROTECCIÓN ANTI-SOBREESCRITURA (Para asegurar que no seleccione el último)
        GestorLogros.LogroData logroLocal = logro;
        Sprite spriteLocal = spriteIcono;

        Button botonNodo = nodoObj.GetComponent<Button>();
        botonNodo.onClick.AddListener(() => MostrarDetalles(logroLocal, spriteLocal));
        return botonNodo; 
    }

    public void MostrarDetalles(GestorLogros.LogroData logro, Sprite sprite)
    {
        if (sprite != null && iconoDetalle != null) {
            iconoDetalle.sprite = sprite;
            iconoDetalle.color = logro.desbloqueado ? Color.white : new Color(0.3f, 0.3f, 0.3f, 1f);
        }
        if (tituloDetalleTMP != null) tituloDetalleTMP.text = logro.titulo;
        if (descripcionDetalleTMP != null) descripcionDetalleTMP.text = logro.desbloqueado ? logro.descripcion : "???\n\n(Sigue jugando para descubrirlo).";
    }
}