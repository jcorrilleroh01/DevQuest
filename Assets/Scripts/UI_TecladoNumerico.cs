using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class UI_TecladoNumerico : MonoBehaviour
{
    [Header("Configuración de Seguridad")]
    public string contrasenaCorrecta = "2026"; // Pon aquí la que hayas escrito en el Post-it
    public CofreContrasena cofreVinculado; // Arrastra tu cofre aquí para avisarle de que lo abrimos
public AudioSource sonidoAbrir;
public AudioSource sonidoAbrir2;
public AudioSource sonidoOK;
    [Header("Elementos de Interfaz")]
    public TextMeshProUGUI textoPantallaNumeros; 
    public Image imagenFondoPanel; 
    public Color colorError = Color.red;

    private Color colorOriginal;

    void Start()
    {
        if (imagenFondoPanel != null) colorOriginal = imagenFondoPanel.color;
        if (textoPantallaNumeros != null) textoPantallaNumeros.text = "";
    }

    // En Unity, ve a los botones del 0 al 9, añade OnClick(), arrastra este script y 
    // en el parámetro String escribe el número correspondiente a ese botón.
    public void BotonNumeroPulsado(string numero)
    {
       if(sonidoAbrir2 != null)
        {
            sonidoAbrir2.Play();
        }
        // Limitamos a 6 caracteres para que no se salga de la pantalla
        if (textoPantallaNumeros.text.Length < 6) 
        {
            textoPantallaNumeros.text += numero;
        }
    }

    // Vincula esto al OnClick() del botón DELETE
    public void BotonBorrar()
    {
        if (textoPantallaNumeros.text.Length > 0)
        {
            textoPantallaNumeros.text = textoPantallaNumeros.text.Substring(0, textoPantallaNumeros.text.Length - 1);
        }
    }

    // Vincula esto al OnClick() del botón ENTER
    public void BotonEnter()
    {
        if (textoPantallaNumeros.text == contrasenaCorrecta)
        {
            // ¡Contraseña correcta!
            textoPantallaNumeros.text = "OK";
                if(sonidoAbrir != null)
        {
            sonidoOK.Play();
        }
            if (cofreVinculado != null) cofreVinculado.DesbloquearPorContrasena();
            gameObject.SetActive(false); // Cerramos el teclado
        }
        else
        {
            // Contraseña incorrecta
            StartCoroutine(EfectoErrorVisual());
        }
    }

    IEnumerator EfectoErrorVisual()
    {
        if(sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }
        textoPantallaNumeros.text = "ERROR";
        if (imagenFondoPanel != null) imagenFondoPanel.color = colorError;
        
        yield return new WaitForSeconds(0.8f);
        
        textoPantallaNumeros.text = "";
        if (imagenFondoPanel != null) imagenFondoPanel.color = colorOriginal;
    }

    // Lo usará el cofre para abrir este panel limpio
    public void AbrirTeclado()
    {
        gameObject.SetActive(true);
        textoPantallaNumeros.text = "";
        if (imagenFondoPanel != null) imagenFondoPanel.color = colorOriginal;
    }
    
    // Vincula esto a un botón de "Cerrar" o "X" del teclado por si el jugador quiere salir
    public void CerrarTeclado()
    {
        gameObject.SetActive(false);
    }
}