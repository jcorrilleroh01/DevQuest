using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransicionApertura : MonoBehaviour
{
    [Header("Configuración")]
    public RectTransform mascaraIris; // Arrastra aquí el objeto "MascaraIris"
    public float velocidad = 2.0f;    // Qué tan rápido se abre/cierra

    void Start()
    {
        // Empezamos la corrutina de transición nada más cargar la escena
        StartCoroutine(AnimarIris());
    }

    IEnumerator AnimarIris()
    {
        // 1. Esperamos un poquito para que el jugador lea el texto (0.5 segundos)
        yield return new WaitForSeconds(0.5f);

        // 2. Bucle de animación: Encogemos la máscara
        // Mientras la escala sea mayor que 0 (visible)
        float escalaActual = 1.0f;
        
        while (escalaActual > 0)
        {
            escalaActual -= Time.deltaTime * velocidad;
            
            // Aplicamos la nueva escala al círculo
            if(mascaraIris != null)
                mascaraIris.localScale = new Vector3(escalaActual, escalaActual, 1);
            
            yield return null; // Esperar al siguiente frame
        }

        // 3. Cuando termina (escala 0), destruimos todo el telón para liberar memoria
        Destroy(gameObject); 
    }
}