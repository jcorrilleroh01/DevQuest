using UnityEngine;

public class Sentar : MonoBehaviour
{
    private bool playerEnRango = false; 
    private bool isSitting = false;     

    [Header("Configuración")]
    public GameObject player;                 
    public Animator playerAnimator;           
    public PlayerController playerMovement;   
    public Transform puntoDeAsiento; 

    [Header("Conexión Visual")]
    // 1. VARIABLE NUEVA: Aquí arrastraremos el otro script
    public FeedbackInteractuable feedbackVisual; // <--- NUEVO

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isSitting)
            {
                Levantarse();
            }
            else if (playerEnRango)
            {
                Debug.Log("Entra en la zona");
                Sentarse();
            }
        }
    }

    void Sentarse()
    {
        isSitting = true;

        // 2. ORDEN NUEVA: "¡Oye Feedback, escóndete que me voy a sentar!"
        if (feedbackVisual != null) feedbackVisual.OcultarPorAccion(); // <--- NUEVO

        // Resto de tu lógica...
        if(playerMovement != null) playerMovement.enabled = false;

        if (puntoDeAsiento != null) player.transform.position = puntoDeAsiento.position;
        else player.transform.position = transform.position;

        playerAnimator.SetBool("IsSitting", true);
        Debug.Log("Jaime está trabajando en el TFG 🤓");
    }

    void Levantarse()
    {
        isSitting = false;

        // 3. ORDEN NUEVA: "¡Oye Feedback, ya he terminado, vuelve a salir!"
        if (feedbackVisual != null) feedbackVisual.MostrarPorFinAccion(); // <--- NUEVO

        // Resto de tu lógica...
        if (playerMovement != null) playerMovement.enabled = true;
        playerAnimator.SetBool("IsSitting", false);

        Debug.Log("Jaime se levanta a descansar 😴");
    }

    // TUS TRIGGERS SE QUEDAN IGUAL (Sirven para la lógica interna de poder pulsar F)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerEnRango = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) playerEnRango = false;
    }
}