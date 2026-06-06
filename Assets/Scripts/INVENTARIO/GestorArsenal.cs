using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; // ¡NUEVO! Es la magia que nos permite ordenar listas

public class GestorArsenal : MonoBehaviour
{
    [Header("Configuración Lista Izquierda")]
    public GameObject prefabBotonArma;
    public Transform contenedorContent; // El objeto "Content" del ScrollView

    [Header("Panel Derecho (Visualización)")]
    public TextMeshProUGUI txtNombreArmaGrande; 
    public Image imagenArmaApoyada; 
    public Image rellenoAtaque; 
    public Image rellenoVelocidad;
    public Image rellenoRango;

    [Header("Panel Inferior")]
    public Image iconoAbajo;
    public TextMeshProUGUI txtTituloAbajo;
    public TextMeshProUGUI txtDescripcionAbajo;

    private ItemArma[] todasLasArmas;

    // ¡NUEVO! OnEnable se ejecuta CADA VEZ que este panel se enciende en pantalla.
    void OnEnable()
    {
        GenerarListaArsenal();
    }

    void GenerarListaArsenal()
    {
        // 1. LIMPIEZA: Destruimos los botones anteriores para no duplicarlos
        foreach (Transform hijo in contenedorContent)
        {
            Destroy(hijo.gameObject);
        }

        // 2. ORDENACIÓN: Cargamos las armas y las ordenamos (las desbloqueadas primero)
        todasLasArmas = Resources.LoadAll<ItemArma>("Armas")
                                 .OrderByDescending(arma => arma.desbloqueada)
                                 .ToArray();

        bool primeraSeleccionada = false;

        // 3. CREACIÓN DE BOTONES
        foreach (ItemArma arma in todasLasArmas)
        {
            GameObject nuevoBoton = Instantiate(prefabBotonArma, contenedorContent);

            Image imgIcono = nuevoBoton.transform.Find("IconoArma").GetComponent<Image>();
            TextMeshProUGUI txtNombre = nuevoBoton.transform.Find("TxtNombre").GetComponent<TextMeshProUGUI>();
            GameObject iconoCandado = nuevoBoton.transform.Find("IconoCandado").gameObject;
            GameObject fondoOscuro = nuevoBoton.transform.Find("FondoOscuro").gameObject;

            if (arma.desbloqueada)
            {
                imgIcono.sprite = arma.icono;
                imgIcono.color = Color.white; // Aseguramos que el color sea normal
                txtNombre.text = arma.nombre;
                iconoCandado.SetActive(false);
                fondoOscuro.SetActive(false);
            }
            else
            {
                imgIcono.sprite = arma.icono;
                imgIcono.color = new Color(0, 0, 0, 1); // Silueta negra
                txtNombre.text = "ARMA DESCONOCIDA";
                iconoCandado.SetActive(true);
                fondoOscuro.SetActive(true);
            }

            Button btn = nuevoBoton.GetComponent<Button>();
            btn.onClick.AddListener(() => MostrarDetallesArma(arma));

            // Autoseleccionar la primera desbloqueada de la lista
            if (!primeraSeleccionada && arma.desbloqueada)
            {
                MostrarDetallesArma(arma);
                primeraSeleccionada = true;
            }
        }

        // ¡Detalle extra! Si entras y no tienes NINGUNA arma desbloqueada, 
        // seleccionamos la primera (bloqueada) para que el panel derecho no se quede vacío/roto.
        if (!primeraSeleccionada && todasLasArmas.Length > 0)
        {
            MostrarDetallesArma(todasLasArmas[0]);
        }
    }

    public void MostrarDetallesArma(ItemArma arma)
    {
        imagenArmaApoyada.rectTransform.anchoredPosition = arma.posicionUI;
        imagenArmaApoyada.rectTransform.localEulerAngles = arma.rotacionUI;
        imagenArmaApoyada.rectTransform.localScale = arma.escalaUI;
        imagenArmaApoyada.rectTransform.sizeDelta = arma.tamanoUI;

        if (!arma.desbloqueada)
        {
            txtNombreArmaGrande.text = "???";
            imagenArmaApoyada.sprite = arma.icono;
            imagenArmaApoyada.color = new Color(0, 0, 0, 1); // Silueta negra
            
            rellenoAtaque.fillAmount = 0f;
            rellenoVelocidad.fillAmount = 0f;
            rellenoRango.fillAmount = 0f;

            iconoAbajo.sprite = arma.icono;
            iconoAbajo.color = new Color(0, 0, 0, 1);
            txtTituloAbajo.text = "ARMA DESCONOCIDA";
            txtDescripcionAbajo.text = "Sigue explorando para encontrar este plano de construcción.";
        }
        else
        {
            txtNombreArmaGrande.text = arma.nombre;
            imagenArmaApoyada.sprite = arma.icono;
            imagenArmaApoyada.color = Color.white; 
            
            rellenoAtaque.fillAmount = arma.ataque / 100f;
            rellenoVelocidad.fillAmount = arma.velocidad / 100f;
            rellenoRango.fillAmount = arma.rango / 100f;

            iconoAbajo.sprite = arma.icono;
            iconoAbajo.color = Color.white;
            txtTituloAbajo.text = arma.nombre.ToUpper();
            txtDescripcionAbajo.text = arma.descripcion;
        }
    }
}