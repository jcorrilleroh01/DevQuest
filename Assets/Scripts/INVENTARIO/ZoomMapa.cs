using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomMapa : MonoBehaviour, IScrollHandler
{
    [Header("Configuración de Zoom")]
    public float velocidadZoom = 0.1f;
    public float zoomMinimo = 0.5f; // Cuánto te puedes alejar (50% del tamaño)
    public float zoomMaximo = 2.0f; // Cuánto te puedes acercar (200% del tamaño)

    [Header("El objeto a hacer zoom")]
    public RectTransform contenidoMapa;

    public void OnScroll(PointerEventData eventData)
    {
        // eventData.scrollDelta.y nos dice si la rueda va hacia arriba (positivo) o abajo (negativo)
        float direccionRueda = eventData.scrollDelta.y;

        // Calculamos la nueva escala
        Vector3 nuevaEscala = contenidoMapa.localScale + Vector3.one * direccionRueda * velocidadZoom;

        // Limitamos para que no se haga infinito ni se dé la vuelta (zoom negativo)
        nuevaEscala.x = Mathf.Clamp(nuevaEscala.x, zoomMinimo, zoomMaximo);
        nuevaEscala.y = Mathf.Clamp(nuevaEscala.y, zoomMinimo, zoomMaximo);
        nuevaEscala.z = 1f; // El eje Z no se toca en UI 2D

        // Aplicamos el tamaño
        contenidoMapa.localScale = nuevaEscala;
    }
}