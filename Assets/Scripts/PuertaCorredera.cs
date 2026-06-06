using System.Collections;
using UnityEngine;

public class PuertaCorredera : MonoBehaviour
{
    [Header("Las Mitades de la Puerta")]
    public Transform mitadIzquierda;
    public Transform mitadDerecha;
    public AudioSource sonidoAbrir; // Sonido que se reproduce al abrir o cerrar la puerta

    [Header("Configuración")]
    [Tooltip("Distancia que se moverá CADA puerta hacia su lado")]
    public float distanciaApertura = 1.5f; 
    public float velocidadApertura = 5f;

    // Posiciones de memoria
    private Vector3 posCerradaIzq;
    private Vector3 posCerradaDer;
    private Vector3 posAbiertaIzq;
    private Vector3 posAbiertaDer;

    private Coroutine animacionActual;

    void Start()
    {
        // 1. Memorizamos dónde están cuando están cerradas
        posCerradaIzq = mitadIzquierda.position;
        posCerradaDer = mitadDerecha.position;

        // 2. Calculamos hasta dónde tienen que llegar al abrirse
        posAbiertaIzq = posCerradaIzq + (Vector3.left * distanciaApertura);
        posAbiertaDer = posCerradaDer + (Vector3.right * distanciaApertura);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si es el jugador Y además este objeto sigue encendido en la escena
        if (collision.CompareTag("Player") && gameObject.activeInHierarchy)
        {
            if (animacionActual != null) StopCoroutine(animacionActual);
            animacionActual = StartCoroutine(MoverPuertas(posAbiertaIzq, posAbiertaDer));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si es el jugador Y además este objeto sigue encendido en la escena
        if (collision.CompareTag("Player") && gameObject.activeInHierarchy)
        {
            if (animacionActual != null) StopCoroutine(animacionActual);
            animacionActual = StartCoroutine(MoverPuertas(posCerradaIzq, posCerradaDer));
        }
    }

    IEnumerator MoverPuertas(Vector3 destinoIzq, Vector3 destinoDer)
    {
        if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }
        // Movemos las puertas suavemente hacia el destino (abierto o cerrado)
        while (Vector3.Distance(mitadIzquierda.position, destinoIzq) > 0.01f ||
               Vector3.Distance(mitadDerecha.position, destinoDer) > 0.01f)
        {
            mitadIzquierda.position = Vector3.Lerp(mitadIzquierda.position, destinoIzq, velocidadApertura * Time.deltaTime);
            mitadDerecha.position = Vector3.Lerp(mitadDerecha.position, destinoDer, velocidadApertura * Time.deltaTime);
            yield return null;
        }
    }
}