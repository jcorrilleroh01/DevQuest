using UnityEngine;

public class LogroDevRoom : MonoBehaviour
{
    // Memoria temporal de la sesión
    public bool encontrado = false; 

    [Header("Configuración del Logro Secreto")]
    [Tooltip("El ID exacto de tu JSON, ej: logro_feature")]
    public string idLogro = "logro_feature"; 
    
    [Tooltip("El icono de error, un bicho o el de Unity que generamos")]
    public Sprite iconoLogro; 

    void Start()
    {
        // 1. Al cargar la escena, preguntamos al disco duro si ya pisamos este suelo en el pasado
        if (PlayerPrefs.GetInt("Secreto_" + idLogro, 0) == 1)
        {
            encontrado = true; // Si es 1, cerramos el candado desde el principio
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 2. Si entra el Player y el candado está abierto (!encontrado)
        if (collision.CompareTag("Player") && !encontrado)
        {
            encontrado = true; // Cerramos el candado en la RAM al instante
            DesbloquearYNotificar();
        }
    }

    private void DesbloquearYNotificar()
    {
        // 3. Registramos el logro en tu PDA
        if (GestorLogros.Instance != null)
        {
            GestorLogros.Instance.DesbloquearLogro(idLogro);
        }

        // 4. Lanzamos la notificación con el chiste de programadores
        if (NotificacionManager.Instance != null)
        {
            NotificacionManager.Instance.MostrarNotificacion("WOW! HAS DESCUBIERTO UN LOGRO SECRETO!", iconoLogro);
        }

        // 5. Guardamos a fuego en el disco duro que ya descubriste este secreto
        PlayerPrefs.SetInt("Secreto_" + idLogro, 1);
        PlayerPrefs.Save();

        Debug.Log("¡Logro secreto de la Dev Room desbloqueado de forma permanente!");
    }
}