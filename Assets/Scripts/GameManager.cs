using UnityEngine;
using System.Collections.Generic; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("Datos de Personalización")]
    public RuntimeAnimatorController outfitActual;
    public string idPuertaDestino;
    public List<string> outfitsDesbloqueados = new List<string>();
    public string escenaPreBatalla; 

    [Header("Base de Datos del Jugador")]
    public List<string> cofresAbiertos = new List<string>(); 
    public List<string> monstruosDescubiertos = new List<string>(); 
    public Sprite spriteCandadoAbierto;
    public JugadorStats statsJaimeGlobal;

    [Header("Datos de Batalla")]
    public Vector3 posicionJugadorMundo; 
    public bool vieneDeBatalla = false;  
    public MonstruoBase monsterEncontrado; 

    // 🛡️ LA SOLUCIÓN: Una caché global que Unity no puede borrar
    private AtaqueBase[] baseDeDatosAtaques;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if(outfitsDesbloqueados.Count == 0)
            {
                outfitsDesbloqueados.Add("Pijama"); 
            }

            if (statsJaimeGlobal != null)
            {
                int vidaMaxGuardada = PlayerPrefs.GetInt("VidaMaxJaime", 20); 
                statsJaimeGlobal.vidaMax = vidaMaxGuardada;
            }

            // Cargamos la memoria nada más empezar
            CargarAtaquesDesdeMemoria();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DesbloquearOutfit(string nombreOutfit)
    {
        if (!outfitsDesbloqueados.Contains(nombreOutfit))
        {
            outfitsDesbloqueados.Add(nombreOutfit);
            Debug.Log("¡Nuevo outfit desbloqueado: " + nombreOutfit + "!");
        }
    }

    public bool EstaDesbloqueado(string nombreOutfit)
    {
        return outfitsDesbloqueados.Contains(nombreOutfit);
    }

    public void GuardarCofreAbierto(string idCofre)
    {
        if (!cofresAbiertos.Contains(idCofre))
        {
            cofresAbiertos.Add(idCofre);
        }
    }

    public bool CofreYaEstaAbierto(string idCofre)
    {
        return cofresAbiertos.Contains(idCofre);
    }

    public void GuardarPosicionMundo(Vector3 pos, string nombreEscena) 
    {
        posicionJugadorMundo = pos;
        escenaPreBatalla = nombreEscena;
        vieneDeBatalla = true;
    }

    // 🛡️ ACTUALIZADO: Ahora usa la Caché RAM y prohíbe el borrado accidental
    public void DesbloquearAtaquesPorArma(string idBuscado)
    {
        // Si por algún motivo la caché está vacía, la llenamos
        if (baseDeDatosAtaques == null || baseDeDatosAtaques.Length == 0)
        {
            baseDeDatosAtaques = Resources.LoadAll<AtaqueBase>("Ataques/BUENOS");
        }

        int contador = 0;

        foreach (AtaqueBase atq in baseDeDatosAtaques)
        {
            // Seguro anti-roturas por si te dejaste un ID en blanco en Unity
            if (string.IsNullOrEmpty(atq.idArmaPertenece)) continue;

            if (atq.idArmaPertenece.Trim().ToUpper() == idBuscado.Trim().ToUpper()) 
            {
                atq.descubierto = true;
                contador++;
                
                PlayerPrefs.SetInt("ArmaGuardada_" + idBuscado.Trim().ToUpper(), 1);
                
                #if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(atq);
                #endif
                
                Debug.Log("🔓 Ataque desbloqueado en RAM y Disco: " + atq.nombreAtaque + " (ID: " + idBuscado + ")");
            }
        }
        
        PlayerPrefs.Save();

        if(contador == 0) Debug.LogWarning("⚠️ No se encontró ningún ataque con el ID: " + idBuscado);
        else Debug.Log("✅ Se han desbloqueado y guardado " + contador + " ataques para el arma " + idBuscado);
    }

    public void CargarAtaquesDesdeMemoria()
    {
        // Llenamos la caché global para que persista
        if (baseDeDatosAtaques == null || baseDeDatosAtaques.Length == 0)
        {
            baseDeDatosAtaques = Resources.LoadAll<AtaqueBase>("Ataques/BUENOS");
        }
        
        foreach (AtaqueBase atq in baseDeDatosAtaques)
        {
            if (string.IsNullOrEmpty(atq.idArmaPertenece)) continue;

            string idArma = atq.idArmaPertenece.Trim().ToUpper();
            
            // IMPORTANTE: Aquí pone "ALMOHADA" como arma inicial por defecto.
            if (PlayerPrefs.GetInt("ArmaGuardada_" + idArma, 0) == 1 || idArma == "ALMOHADA")
            {
                atq.descubierto = true;
            }
            else
            {
                atq.descubierto = false; 
            }
        }
        Debug.Log("✅ Arsenal cargado desde la memoria local a la RAM interactiva.");
    }

    public void RegistrarMonstruo(string nombreMonstruo)
    {
        if (!monstruosDescubiertos.Contains(nombreMonstruo))
        {
            monstruosDescubiertos.Add(nombreMonstruo);
            Debug.Log("PDA: " + nombreMonstruo + " registrado permanentemente.");
        }
    }

    public bool EstaDescubierto(string nombreMonstruo)
    {
        return monstruosDescubiertos.Contains(nombreMonstruo);
    }

    void Start()
    {
        if(GestorCursor.Instance != null) GestorCursor.Instance.OcultarCursor(); 
        
        if (NotificacionManager.Instance != null)
        {
            NotificacionManager.Instance.MostrarNotificacion("NUEVO LOGRO DESBLOQUEADO", spriteCandadoAbierto);
        }

        if (GestorLogros.Instance != null)
        {
            GestorLogros.Instance.DesbloquearLogro("logro_01_spawn");        
        }
    }
}