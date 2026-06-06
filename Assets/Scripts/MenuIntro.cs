using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuIntro : MonoBehaviour
{
    [Header("Configuración de Escena")]
    public string nombreEscenaHistoria = "EscenaHistoria";

    [Header("Animación del Botón")]
    public Transform botonJugar;
    public float velocidadFlote = 2f;
    public float alturaFlote = 5f;

    private Vector3 posInicial;

    void Awake()
    {
        // Forzamos el ratón incluso antes del Start
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Start()
    {
        // Re-confirmamos la aparición tras un breve instante por si el GestorMouse lo oculta
        StartCoroutine(AsegurarCursor());

        if (botonJugar != null)
        {
            posInicial = botonJugar.localPosition;
        }
    }

    IEnumerator AsegurarCursor()
    {
        // Esperamos 3 frames para dejar que todos los scripts de la escena se inicialicen
        yield return null;
        yield return null;
        yield return null;

        // Intentamos usar tu gestor si existe
        if (GestorCursor.Instance != null) 
        {
            GestorCursor.Instance.MostrarCursor();
        }

        // Orden de emergencia directa de Unity
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Debug.Log("Cursor forzado en la Intro 🖱️");
    }

    void Update()
    {
        if (botonJugar != null)
        {
            float nuevoY = posInicial.y + Mathf.Sin(Time.time * velocidadFlote) * alturaFlote;
            botonJugar.localPosition = new Vector3(posInicial.x, nuevoY, posInicial.z);
        }
    }

    public void CargarHistoria()
    {
        if (GestorCursor.Instance != null) 
        {
            GestorCursor.Instance.OcultarCursor();
        SceneManager.LoadScene(nombreEscenaHistoria);
    }
} }