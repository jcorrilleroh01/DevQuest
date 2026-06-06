using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NotificacionElemento : MonoBehaviour
{
    [Header("Referencias Visuales")]
    public TextMeshProUGUI textoAlerta;
    public Image iconoAlerta;

    private RectTransform rectTransform;
    
    // Configuraciones de movimiento
    private float posFueraX = 400f;
    private float posDentroX = -20f;
    private float posYActual = 0f; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ConfigurarYLanzar(string mensaje, Sprite icono, float alturaInicial)
    {
        textoAlerta.text = mensaje;
        if (icono != null) iconoAlerta.sprite = icono;

        posYActual = alturaInicial; 

        rectTransform.anchoredPosition = new Vector2(posFueraX, alturaInicial);
        
        StartCoroutine(RutinaVida());
    }

    private IEnumerator RutinaVida()
    {
        // 1. Entrada
        yield return StartCoroutine(DeslizarX(posFueraX, posDentroX, 0.4f));
        
        // 2. Espera
        yield return new WaitForSeconds(3f);
        
        // 3. Salida
        yield return StartCoroutine(DeslizarX(posDentroX, posFueraX, 0.4f));
        
        // 4. Destrucción
        NotificacionManager.Instance.RemoverNotificacion(this);
        Destroy(gameObject);
    }

    private IEnumerator DeslizarX(float inicio, float fin, float duracion)
    {
        float tiempo = 0;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime / duracion;
            float x = Mathf.SmoothStep(inicio, fin, tiempo);
            rectTransform.anchoredPosition = new Vector2(x, posYActual);
            yield return null;
        }
    }

    public void DesplazarAbajo(float nuevaY)
    {
        posYActual = nuevaY;
        StartCoroutine(AnimarY(nuevaY));
    }

    private IEnumerator AnimarY(float destinoY)
    {
        float tiempo = 0;
        float inicioY = rectTransform.anchoredPosition.y;
        while (tiempo < 1f)
        {
            tiempo += Time.deltaTime / 0.3f; 
            float y = Mathf.SmoothStep(inicioY, destinoY, tiempo);
            
            // Aquí estaba tu error. Ahora mantiene su X correctamente y actualiza su Y.
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, y);            
            yield return null;
        }
    }
}