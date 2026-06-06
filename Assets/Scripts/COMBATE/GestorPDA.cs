using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq; 

public class GestorPDA : MonoBehaviour
{
    [Header("Configuración Izquierda")]
    public GameObject prefabCelda;
    public Transform contenedorCeldas; 
    public RectTransform marcoSeleccion; 

    [Header("Panel Detalles (Centro/Derecha)")]
    public Image imagenMarcoAzul; 
    public TextMeshProUGUI txtNombre;
    public TextMeshProUGUI txtTipo;
    public TextMeshProUGUI txtVida;
    public TextMeshProUGUI txtAtaque;
    public TextMeshProUGUI txtDefensa;
    public TextMeshProUGUI txtDebilidad;
    public TextMeshProUGUI txtLoreDerecha;

    [Header("Panel Inferior (Marrón)")]
    public Image iconoAbajo;
    public TextMeshProUGUI txtTituloAbajo;
    public TextMeshProUGUI txtDescripcionAbajo;

    private MonstruoBase[] todosLosMonstruos;

    void OnEnable()
    {
        GenerarLista();
    }

    // --- NUEVO: SISTEMA DE AUTOCURACIÓN ---
    void ComprobarMarco()
    {
        // Si Unity ha destruido la referencia al recargar la escena tras la batalla...
        if (marcoSeleccion == null && contenedorCeldas != null && contenedorCeldas.parent != null)
        {
            // Transform.Find es capaz de encontrar objetos aunque estén apagados (SetActve false)
            Transform recuperado = contenedorCeldas.parent.Find("MarcoSeleccion_Definitivo");
            if (recuperado != null) 
            {
                marcoSeleccion = recuperado.GetComponent<RectTransform>();
                Debug.Log("¡Marco de selección recuperado automáticamente tras la batalla!");
            }
        }
    }

    void GenerarLista()
    {
        // 1. SALVAR EL MARCO
        if (marcoSeleccion != null)
        {
            marcoSeleccion.gameObject.SetActive(false);
            if (marcoSeleccion.parent != contenedorCeldas.parent)
                marcoSeleccion.SetParent(contenedorCeldas.parent, true);
        }

        // 2. LIMPIEZA
        foreach (Transform hijo in contenedorCeldas)
        {
            if (marcoSeleccion != null && hijo == marcoSeleccion) continue; 
            Destroy(hijo.gameObject);
        }

        // --- ¡LA CLAVE ESTÁ AQUÍ! SINCRONIZAMOS CON EL GAMEMANAGER ---
        todosLosMonstruos = Resources.LoadAll<MonstruoBase>("Monstruos");
        
        if (GameManager.Instance != null)
        {
            foreach (MonstruoBase m in todosLosMonstruos)
            {
                // Si el GameManager dice que lo tienes, sobrescribimos el archivo para que sea verdad
                if (GameManager.Instance.EstaDescubierto(m.nombre))
                {
                    m.descubierto = true;
                }
            }
        }

        // 3. ORDENACIÓN
        todosLosMonstruos = todosLosMonstruos.OrderByDescending(m => m.descubierto).ToArray();
        
        if (todosLosMonstruos == null || todosLosMonstruos.Length == 0) return; 

        bool primeroSeleccionado = false;

        foreach (MonstruoBase monstruo in todosLosMonstruos)
        {
            GameObject celda = Instantiate(prefabCelda, contenedorCeldas);
            Transform iconoTransform = celda.transform.Find("IconoBicho");
            
            if (iconoTransform == null) continue; 

            Image imgIcono = iconoTransform.GetComponent<Image>();
            
            if (monstruo.spriteFrontal != null) imgIcono.sprite = monstruo.spriteFrontal;

            if (!monstruo.descubierto) imgIcono.color = new Color(0f, 0f, 0f, 1f);
            else imgIcono.color = Color.white; 

            Button btn = celda.GetComponent<Button>();
            btn.onClick.AddListener(() => SeleccionarMonstruo(monstruo, celda.transform));

            if (!primeroSeleccionado && monstruo.descubierto)
            {
                SeleccionarMonstruo(monstruo, celda.transform);
                primeroSeleccionado = true;
            }
        }

        if (!primeroSeleccionado && todosLosMonstruos.Length > 0 && contenedorCeldas.childCount > 0)
        {
            SeleccionarMonstruo(todosLosMonstruos[0], contenedorCeldas.GetChild(0));
        }
    }

    public void SeleccionarMonstruo(MonstruoBase monstruo, Transform celdaTransform)
    {
        ComprobarMarco(); 
        
        if (marcoSeleccion != null)
        {
            // --- NUEVO: POSICIÓN FLOTANTE ---
            // En lugar de meter el marco DENTRO de la celda (lo que causaba su destrucción),
            // lo mantenemos fuera y simplemente copiamos las coordenadas de la celda.
            if (marcoSeleccion.parent != contenedorCeldas.parent)
                marcoSeleccion.SetParent(contenedorCeldas.parent, true);

            // Movemos el marco visualmente a donde está la celda clickeada
            marcoSeleccion.position = celdaTransform.position;
            marcoSeleccion.SetAsLastSibling(); 
            marcoSeleccion.gameObject.SetActive(true);
        }

        // Lógica visual de la PDA
        if (!monstruo.descubierto)
        {
            imagenMarcoAzul.sprite = monstruo.spriteFrontal;
            imagenMarcoAzul.color = new Color(0f, 0f, 0f, 1f); 
            txtNombre.text = "???";
            txtTipo.text = "TIPO: Desconocido";
            txtVida.text = "HP: ???";
            txtAtaque.text = "ATQ: ???";
            txtDefensa.text = "DEF: ???";
            txtDebilidad.text = "???";
            txtLoreDerecha.text = "Datos corruptos. Debes enfrentarte a este error en el código fuente para registrarlo.";

            iconoAbajo.sprite = monstruo.spriteFrontal;
            iconoAbajo.color = new Color(0f, 0f, 0f, 1f);
            txtTituloAbajo.text = "ENTRADA BLOQUEADA";
            txtDescripcionAbajo.text = "Faltan datos en el sistema.";
        }
        else
        {
            imagenMarcoAzul.sprite = monstruo.spriteFrontal;
            imagenMarcoAzul.color = Color.white;
            txtNombre.text = monstruo.nombre;
            txtTipo.text = "TIPO: " + monstruo.tipo;
            txtVida.text = $"VIDA: {monstruo.vidaMax}";
            txtAtaque.text = $"ATQ: {monstruo.ataque}";
            txtDefensa.text = $"DEF: {monstruo.defensa}";
            txtDebilidad.text = monstruo.debilidad;
            txtLoreDerecha.text = monstruo.lore;

            iconoAbajo.sprite = monstruo.spriteFrontal;
            iconoAbajo.color = Color.white;
            txtTituloAbajo.text =  monstruo.nombre.ToUpper();
            txtDescripcionAbajo.text = monstruo.descripcion; 
        }
    }
}