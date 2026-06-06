using UnityEngine;
using System.Collections;

public class MemoriaPosicion : MonoBehaviour
{
    void Start()
    {
        // Comprobamos si el GameManager tiene guardada una posición de batalla
        if (GameManager.Instance != null && GameManager.Instance.vieneDeBatalla)
        {
            // Usamos una Corrutina para ganar la "pelea" contra el script de la puerta
            StartCoroutine(TeletransportePostBatalla());
        }
    }

    IEnumerator TeletransportePostBatalla()
    {
        // 1. Esperamos a que termine el frame actual. 
        // Esto permite que los scripts de entrada de casa terminen de posicionarte.
        yield return new WaitForEndOfFrame();
        
        // 2. ¡ZAS! Sobrescribimos la posición con la que guardamos en la hierba
        transform.position = GameManager.Instance.posicionJugadorMundo;
        
        // 3. Muy importante: Apagamos el interruptor en el GameManager
        // para que si entras a una casa normalmente, NO te teletransporte a la hierba.
        GameManager.Instance.vieneDeBatalla = false;
        
        Debug.Log("Retorno de batalla: Posición restaurada en " + transform.position);
    }
}