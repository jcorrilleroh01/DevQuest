using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq; 

public class EncuentroAleatorio : MonoBehaviour
{
    [Header("Configuración del Radar")]
    public LayerMask capaSueloSeguro; 
    public float probabilidad = 10f; 
    public float distanciaParaCheck = 1.0f; 
    public string nombreEscenaBatalla = "BATALLA"; 

    private Vector2 ultimaPosicion;
    private float contadorDistancia = 0;

    void Start() 
    { 
        ultimaPosicion = transform.position; 
        // Obliga al radar a detectar la zona segura aunque la configuración de Unity esté mal
        Physics2D.queriesHitTriggers = true; 
    }

    void Update()
    {
        float distanciaMovida = Vector2.Distance(transform.position, ultimaPosicion);
        if (distanciaMovida > 0.1f) 
        {
            contadorDistancia += distanciaMovida;
            if (contadorDistancia >= distanciaParaCheck)
            {
                ComprobarTerreno();
                contadorDistancia = 0; 
                ultimaPosicion = transform.position; 
            }
        }
    }

   void ComprobarTerreno()
    {
        // 1. El radar intenta buscar tu Capa Segura
        Collider2D col = Physics2D.OverlapCircle(transform.position, 0.5f, capaSueloSeguro);
        
        if (col != null)
        {
            Debug.Log("🛡️ ZONA SEGURA: El radar de Jaime está tocando -> " + col.gameObject.name);
        }
        else
        {
            // --- MODO DETECTIVE ACTIVADO ---
            Debug.Log("⚠️ PELIGRO: El radar no detecta la capa SueloSeguro. Escaneando el entorno...");
            
            // Hacemos un círculo ciego que detecte TODO sin importar la capa
            Collider2D[] todoLoQueToca = Physics2D.OverlapCircleAll(transform.position, 0.5f);
            
            if (todoLoQueToca.Length == 0) 
            {
                Debug.LogError("🛑 EL RADAR ESTÁ EN EL VACÍO: El círculo azul no está tocando NINGUNA línea verde. O el suelo no tiene colliders, o te has salido del mapa.");
            } 
            else 
            {
                foreach(Collider2D c in todoLoQueToca) 
                {
                    string nombreCapa = LayerMask.LayerToName(c.gameObject.layer);
                    Debug.LogWarning("🔍 El radar está tocando: [" + c.gameObject.name + "] pero su capa es [" + nombreCapa + "]");
                }
            }
            // ---------------------------------

            IntentarCombate();
        }
    }

    void IntentarCombate()
    {
        if (Random.Range(0f, 100f) < probabilidad) IniciarBatallaAleatoria();
    }

    void IniciarBatallaAleatoria()
    {
        if(GameManager.Instance == null) return;

        MonstruoBase[] todos = Resources.LoadAll<MonstruoBase>("Monstruos");
        if (todos.Length > 0)
        {
            List<MonstruoBase> bloqueados = todos.Where(m => !GameManager.Instance.EstaDescubierto(m.nombre)).ToList();
            
            MonstruoBase monstruoElegido;
            if (bloqueados.Count > 0) monstruoElegido = bloqueados[Random.Range(0, bloqueados.Count)];
            else monstruoElegido = todos[Random.Range(0, todos.Length)]; 

            string nombreEscenaActual = SceneManager.GetActiveScene().name;
            GameManager.Instance.GuardarPosicionMundo(transform.position, nombreEscenaActual);
            
            GameManager.Instance.monsterEncontrado = monstruoElegido;
            SceneManager.LoadScene(nombreEscenaBatalla);
        }
    }

    // --- HERRAMIENTA VISUAL PARA EL DEVELOPER ---
    void OnDrawGizmos()
    {
        // Esto dibujará una esfera azul alrededor de tu jugador en la pestaña "Scene"
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}