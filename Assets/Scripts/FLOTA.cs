using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [Header("Configuración del Flote")]
    public float velocidad = 2f;    // Qué tan rápido sube y baja
    public float altura = 0.1f;     // Qué tanto se mueve (distancia)

    private Vector3 posInicial;

    void Start()
    {
        // Guardamos dónde empieza para moverlo relativo a este punto
        posInicial = transform.position;
    }

    void Update()
    {
        // MAGIA MATEMÁTICA: La función SIN (Seno) crea una onda suave entre -1 y 1
        // Lo multiplicamos por la altura para que sea sutil
        float nuevoY = Mathf.Sin(Time.time * velocidad) * altura;

        // Aplicamos la posición
        transform.position = posInicial + new Vector3(0, nuevoY, 0);
    }
}