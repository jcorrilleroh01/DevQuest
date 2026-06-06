using UnityEngine;

// Esta línea define que esto es un componente de Unity
public class PlayerController : MonoBehaviour
{
    // --- VARIABLES (CONFIGURACIÓN) ---
    // "public" significa que podrás ver y cambiar este número desde el Inspector de Unity sin volver al código.
    // "float" significa que es un número con decimales.
    [Header("Configuración de Movimiento")]
    public float velocidad = 5f; 

    // Estas son privadas porque no necesitamos tocarlas desde Unity, las calcula el código.
    private Rigidbody2D rb;      // Referencia al cuerpo físico
    private Vector2 movimiento;  // Guardará la dirección (X, Y)
    private Animator animator;
    // --- START: PREPARACIÓN ---
    // Esto se ejecuta en el milisegundo 0 del juego.
    void Start()
    {
        // LECCIÓN: El script necesita conectar con el cuerpo físico para empujarlo.
        // GetComponent busca en el MISMO objeto (Jaime) el componente Rigidbody2D.
        rb = GetComponent<Rigidbody2D>();
        animator= GetComponent<Animator>();

        // Si el GameManager dice que volvemos de una pelea...
    if (GameManager.Instance != null && GameManager.Instance.vieneDeBatalla)
    {
        // Nos teletransportamos al sitio donde estábamos
        transform.position = GameManager.Instance.posicionJugadorMundo;
        GameManager.Instance.vieneDeBatalla = false; // Apagamos el aviso
    }
    }

    // --- UPDATE: EL OÍDO (INPUTS) ---
    // Esto se ejecuta en cada frame. Aquí solo ESCUCHAMOS al teclado.
    void Update()
    {
        // "Input.GetAxisRaw" devuelve:
        //  1 si pulsas Derecha/Arriba
        // -1 si pulsas Izquierda/Abajo
        //  0 si no pulsas nada
        
        movimiento.x = Input.GetAxisRaw("Horizontal");
        movimiento.y = Input.GetAxisRaw("Vertical");
        

        // "Normalize" es un truco matemático:
        // Si pulsas Arriba+Derecha, la velocidad sería 1.4 (hipotenusa). 
        // Esto lo corrige a 1 para que no corras más rápido en diagonal.
        movimiento.Normalize();
    }

    // --- FIXED UPDATE: EL MÚSCULO (FÍSICAS) ---
    // Este Update especial se sincroniza con el motor de físicas de Unity.
    // SIEMPRE que muevas cosas con Rigidbody, hazlo aquí, no en Update normal.
    void FixedUpdate()
    {
        // La fórmula mágica del movimiento:
        // Posición Nueva = Posición Actual + (Dirección * Velocidad * Tiempo)
        rb.MovePosition(rb.position + movimiento * velocidad * Time.fixedDeltaTime);


        // --- INICIO DEL CÓDIGO DE ANIMACIÓN ---

// 1. Si nos estamos moviendo, actualizamos la dirección (Memoria)
if (movimiento != Vector2.zero) 
{
    animator.SetFloat("Horizontal", movimiento.x);
    animator.SetFloat("Vertical", movimiento.y);
}

// 2. Siempre actualizamos la velocidad (para saber si estamos quietos o no)
animator.SetFloat("Velocidad", movimiento.sqrMagnitude);

// --- FIN DEL CÓDIGO ---
        
    }
}