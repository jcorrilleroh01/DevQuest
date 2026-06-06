using UnityEngine;
using System.Collections.Generic;

public class NotificacionManager : MonoBehaviour
{
    public static NotificacionManager Instance;
public AudioSource sonidoAbrir; // Sonido que se reproduce al mostrar una nueva notificación
    [Header("Configuración")]
    public GameObject alertaPrefab; 
    public Transform contenedor;    
    
    public float separacionY = 110f; 
    public float margenSuperior = -50f; 

    private List<NotificacionElemento> alertasActivas = new List<NotificacionElemento>();

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else 
        { 
            Destroy(gameObject); 
        }
    }

    public void MostrarNotificacion(string mensaje, Sprite iconoOpcional = null)
    {
        if(sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }
        // 1. Desplazamos las antiguas hacia abajo
        for (int i = 0; i < alertasActivas.Count; i++)
        {
            float nuevaPosY = margenSuperior - ((i + 1) * separacionY);
            alertasActivas[i].DesplazarAbajo(nuevaPosY);
        }

        // 2. Creamos la nueva
        GameObject nuevaAlerta = Instantiate(alertaPrefab, contenedor);
        NotificacionElemento scriptElemento = nuevaAlerta.GetComponent<NotificacionElemento>();
        
        // 3. La añadimos a la lista y la lanzamos
        alertasActivas.Insert(0, scriptElemento);
        scriptElemento.ConfigurarYLanzar(mensaje, iconoOpcional, margenSuperior);    
        if (InventarioManager.Instance != null && InventarioManager.Instance.puntoNotificacionHUD != null)
        {
            InventarioManager.Instance.puntoNotificacionHUD.SetActive(true);
        }
    }

    public void RemoverNotificacion(NotificacionElemento elemento)
    {
        if (alertasActivas.Contains(elemento))
        {
            alertasActivas.Remove(elemento);
        }
    }
}