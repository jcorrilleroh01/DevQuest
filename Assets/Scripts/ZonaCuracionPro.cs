using UnityEngine;

public class ZonaCuracionPro : MonoBehaviour
{
    [Header("Configuración")]
    public string idZonaUnica;
    public int cantidadCuracion = 10;
    public JugadorStats statsJaime;
    public Sprite spriteCandadoAbierto;
    private BoxCollider2D miCollider;
    private bool yaSeUso = false;

    void Start()
    {
        miCollider = GetComponent<BoxCollider2D>();
        
        if (PlayerPrefs.GetInt("Curado_" + idZonaUnica, 0) == 1)
        {
            yaSeUso = true;
        }
    }

    void Update()
    {
        if (yaSeUso) return;

        Collider2D hit = Physics2D.OverlapBox(miCollider.bounds.center, miCollider.bounds.size, 0);

        if (hit != null && hit.CompareTag("Player"))
        {
            EjecutarCuracion();
        }
    }

    void EjecutarCuracion()
    {
        if (statsJaime != null)
        {
            // --- LA SOLUCIÓN CLAVE ---
            // 1. Aumentamos la vida MÁXIMA permanente del jugador
            statsJaime.vidaMax += cantidadCuracion;
            
            // 2. Le curamos al máximo por haber descubierto el secreto
            statsJaime.vidaActual = statsJaime.vidaMax;

            // 3. Guardamos la nueva vida máxima en el disco duro para que no se pierda al cambiar de escena
            PlayerPrefs.SetInt("VidaMaxJaime", statsJaime.vidaMax);

            yaSeUso = true;
            PlayerPrefs.SetInt("Curado_" + idZonaUnica, 1);
            PlayerPrefs.Save();

            if (NotificacionManager.Instance != null)
            {
                NotificacionManager.Instance.MostrarNotificacion("¡NUEVA ZONA DESCUBIERTA! +10 VIDA MÁXIMA!", spriteCandadoAbierto);
            }
            Debug.Log("<color=green><b>[SISTEMA VIDA]</b></color> Vida máxima aumentada +" + cantidadCuracion + " en " + idZonaUnica);
        }
    }

    [ContextMenu("Resetear esta zona")]
    public void ResetZona()
    {
        PlayerPrefs.SetInt("Curado_" + idZonaUnica, 0);
        yaSeUso = false;
        Debug.Log("Zona reseteada");
    }
}