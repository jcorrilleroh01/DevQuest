using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BotonAtaqueUI : MonoBehaviour
{
    [Header("Componentes del Prefab")]
    public TextMeshProUGUI txtNombre;
    public Image imgIconoArma;
    public GameObject fondoOscuro; // El objeto 'FondoOscuro'
    public GameObject iconoCandado; // El objeto 'IconoCandado'
    public Button miBoton;

    // Esta es la función que rellena los datos
    public void ConfigurarBoton(AtaqueBase datos, bool estaDesbloqueado)
    {
        // 1. Ponemos el nombre del ataque
        if (txtNombre != null) 
        {
            txtNombre.text = estaDesbloqueado ? datos.nombreAtaque : "???"; 
        }

        // 2. Ponemos el icono del arma
        if (imgIconoArma != null)
        {
            imgIconoArma.sprite = datos.iconoAtaque;
            // Si está bloqueado, lo ponemos gris o negro
            imgIconoArma.color = estaDesbloqueado ? Color.white : Color.black;
        }

        // 3. Gestionamos el candado y el fondo oscuro
        if (fondoOscuro != null) fondoOscuro.SetActive(!estaDesbloqueado);
        if (iconoCandado != null) iconoCandado.SetActive(!estaDesbloqueado);

        // 4. Bloqueamos el clic si no está descubierto
        if (miBoton != null) miBoton.interactable = estaDesbloqueado;
    }
}