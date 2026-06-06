using UnityEngine;
using System.Collections;

public class LogroEntradaEdificio : MonoBehaviour
{
    private bool encontrado = false; 
    private Collider2D miCollider;

    public string idLogro;
    public Sprite iconoLogro; 

    void Start()
    {
        miCollider = GetComponent<Collider2D>();
        if (PlayerPrefs.GetInt("Logro_" + idLogro, 0) == 1) encontrado = true;
        
        if (!encontrado) StartCoroutine(EscanearSpawnInicial());
    }

    IEnumerator EscanearSpawnInicial()
    {
        yield return new WaitForSeconds(0.5f);
        if (encontrado || miCollider == null) yield break;

        Collider2D[] collidersTocando = new Collider2D[10];
        int cantidad = miCollider.Overlap(ContactFilter2D.noFilter, collidersTocando);
        
        for (int i = 0; i < cantidad; i++)
        {
            if (collidersTocando[i] != null && collidersTocando[i].CompareTag("Player"))
            {
                EjecutarDesbloqueo();
                break;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !encontrado) EjecutarDesbloqueo();
    }

    private void EjecutarDesbloqueo()
    {
        if (encontrado) return; 
        encontrado = true; 

        if (GestorLogros.Instance != null) GestorLogros.Instance.DesbloquearLogro(idLogro);
        if (NotificacionManager.Instance != null) NotificacionManager.Instance.MostrarNotificacion("¡HAS VISITADO UN NUEVO LUGAR!", iconoLogro);
    }
}