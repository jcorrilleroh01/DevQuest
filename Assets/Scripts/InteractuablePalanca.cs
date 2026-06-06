using UnityEngine;

public class InteractuablePalanca : MonoBehaviour
{
    [Header("Persistencia")]
    [Tooltip("ID único para que el GameManager recuerde esta palanca")]
    public string idPalanca = "Palanca_Agora_01"; 

    public float radioInteraccion = 2f; 
    public AudioSource sonidoActivar; // Sonido que se reproduce al activar la palanca
    public Transform jugador;           
    private Animator anim;
    
    // Público para que el cofre pueda leerlo
    public bool activada = false; 

    void Start() 
    {
        anim = GetComponent<Animator>();

        // 1. Preguntamos a tu GameManager si este ID ya está en la lista cofresAbiertos
        if (GameManager.Instance != null && GameManager.Instance.CofreYaEstaAbierto(idPalanca))
        {
            activada = true; // La marcamos como activada en el código
            
            if (anim != null) 
            {
                // 2. Forzamos el parámetro interno del Animator para que no intente volver a Idle
                anim.SetBool("estaActivada", true);
                
                // 3. TRUCO DE UNITY: Reproducimos la animación pero le pasamos '1f' para que salte al 100% de la animación (al final) instantáneamente.
                anim.Play("palancabajar", 0, 1f); 
            }
        }
    }

    void Update() 
    {
        // Si ya la bajamos, cortamos el Update aquí para ahorrar rendimiento y evitar fallos
        if (activada) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia < radioInteraccion && Input.GetKeyDown(KeyCode.E)) 
        {
            ActivarPalanca();
        }
    }

    void ActivarPalanca() 
    {
        activada = true;
        if(sonidoActivar != null)
        {
            sonidoActivar.Play();
        }
        
        if (anim != null) 
        {
            anim.SetBool("estaActivada", true);
        }

        // Guardamos el progreso en la lista de tu GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GuardarCofreAbierto(idPalanca);
            Debug.Log("¡Éxito! " + idPalanca + " guardada en el GameManager.");
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radioInteraccion);
    }
}