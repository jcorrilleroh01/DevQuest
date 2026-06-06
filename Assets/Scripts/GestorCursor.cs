using UnityEngine;

public class GestorCursor : MonoBehaviour
{
    // Esto nos permite llamar a GestorCursor.Instance desde CUALQUIER script
    public static GestorCursor Instance;

    [Header("Configuración Visual")]
    public Texture2D texturaCursor;
    
    [Tooltip("El punto exacto de la imagen que hace clic. (0,0) es la esquina superior izquierda.")]
    public Vector2 puntoDeAnclaje = Vector2.zero; 

    void Awake()
    {
        // Configuramos el Singleton para que este script sobreviva a los cambios de escena
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else 
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Al arrancar el juego, le ponemos el dibujo personalizado y lo escondemos
        Cursor.SetCursor(texturaCursor, puntoDeAnclaje, CursorMode.Auto);
        OcultarCursor();
    }

    // Llama a esto cuando abras un panel, el PC, la Arcade o la Batalla
    public void MostrarCursor()
    {
        Cursor.visible = true;
    }

    // Llama a esto cuando el jugador vuelva a tener el control de caminar
    public void OcultarCursor()
    {
        Cursor.visible = false;
    }
}