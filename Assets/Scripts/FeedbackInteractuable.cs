using UnityEngine;

public class FeedbackInteractuable : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public GameObject iconoTecla;        
    public ParticleSystem fxBrillo;      

    private bool enUso = false;          
    private Collider2D miCollider;

    void Start()
    {
        miCollider = GetComponent<Collider2D>();

        // 1. Aseguramos que las partículas no solo estén activas, sino reproduciéndose
        if (fxBrillo != null) 
        {
            fxBrillo.gameObject.SetActive(true);
            fxBrillo.Play();
        }

        // 2. COMPROBACIÓN INTELIGENTE: ¿Está Jaime ya pisando este trigger al cargar la escena?
        bool jugadorYaEstaDentro = false;
        if (miCollider != null)
        {
           Collider2D[] collidersTocando = new Collider2D[10];
            
            // VERSIÓN ACTUALIZADA PARA UNITY 6: 
            // Usamos la propiedad estática y el nuevo método Overlap directamente
            int cantidad = miCollider.Overlap(ContactFilter2D.noFilter, collidersTocando);
            for (int i = 0; i < cantidad; i++)
            {
                if (collidersTocando[i] != null && collidersTocando[i].CompareTag("Player"))
                {
                    jugadorYaEstaDentro = true;
                    break; // Encontramos a Jaime, dejamos de buscar
                }
            }
        }

        // 3. Encendemos o apagamos la tecla basándonos en la realidad, no a ciegas
        if (iconoTecla != null) 
        {
            iconoTecla.SetActive(jugadorYaEstaDentro && !enUso);
        }
    }

    // --- FUNCIONES PARA LA ACCIÓN ---

    public void OcultarPorAccion()
    {
        enUso = true;
        ApagarTodo();
    }

    public void MostrarPorFinAccion()
    {
        enUso = false;
        
        if (fxBrillo != null) 
        {
            fxBrillo.gameObject.SetActive(true);
            fxBrillo.Play();
        }
        
        if (iconoTecla != null) iconoTecla.SetActive(true);
    }

    // --- CONTROL DE PROXIMIDAD ---

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !enUso)
        {
            if (iconoTecla != null) iconoTecla.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (iconoTecla != null) iconoTecla.SetActive(false);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !enUso && iconoTecla != null && !iconoTecla.activeSelf)
        {
            iconoTecla.SetActive(true);
        }
    }

    // --- AYUDANTE ---
    void ApagarTodo()
    {
        if (iconoTecla != null) iconoTecla.SetActive(false);
        if (fxBrillo != null) 
        {
            fxBrillo.Stop(); // Es más seguro detener el emisor de partículas
            fxBrillo.gameObject.SetActive(false);
        }
    }
}