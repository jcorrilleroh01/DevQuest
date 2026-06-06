using System;
using System.Collections.Generic;
using UnityEngine;

public class GestorLogros : MonoBehaviour
{
    public static GestorLogros Instance;

    [Header("Archivos y Datos")]
    public TextAsset archivoJson;
    public Sprite iconoCandadoDesbloqueado;

    [System.Serializable]
    public class LogroData {
        public string id; public string titulo; public string descripcion;
        public string icono; public bool desbloqueado; public string padreId;
        public float x; public float y;
    }

    [System.Serializable]
    public class ListaLogros { public LogroData[] logros; }

    public Dictionary<string, LogroData> diccionarioLogros = new Dictionary<string, LogroData>();
    public ListaLogros listaLogrosCache;

    public Action OnLogrosActualizados;

    void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this; 
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject); 
            return; 
        }

        // 🛡️ CHIVATO 1: Comprobamos si falta el JSON
        if (archivoJson == null)
        {
            Debug.LogError("🚨 GESTOR LOGROS: ¡Te has olvidado de arrastrar tu archivo 'datos_logros.json' en el inspector del Gestor!");
            return;
        }

        CargarDatosInternos();
    }

    public void CargarDatosInternos()
    {
        diccionarioLogros.Clear();
        listaLogrosCache = JsonUtility.FromJson<ListaLogros>(archivoJson.text);

        if (listaLogrosCache == null || listaLogrosCache.logros == null)
        {
            Debug.LogError("🚨 GESTOR LOGROS: El archivo JSON no se ha podido leer bien.");
            return;
        }

        foreach (LogroData logro in listaLogrosCache.logros)
        {
            if (PlayerPrefs.GetInt("Logro_" + logro.id, 0) == 1) logro.desbloqueado = true;
            diccionarioLogros.Add(logro.id, logro);
        }
        
        Debug.Log("✅ CEREBRO OK: Datos de logros cargados en la memoria invisible.");
    }

    public void DesbloquearLogro(string idLogro)
    {
        if (diccionarioLogros.ContainsKey(idLogro))
        {
            LogroData logro = diccionarioLogros[idLogro];
            if (logro.desbloqueado) return; 

            logro.desbloqueado = true;
            PlayerPrefs.SetInt("Logro_" + idLogro, 1);
            PlayerPrefs.Save();

            OnLogrosActualizados?.Invoke();
            ComprobarLogrosMaestros();
            Debug.Log("✅ Logro " + idLogro + " desbloqueado exitosamente.");
        }
    }

    // --- LOGROS MAESTROS ---
    public void ComprobarLogrosMaestros()
    {
        CheckEdificiosMaster("logro_11_fullstack");
        CheckMonstruosMaster("logro_08_pda");
        CheckArmasMaster("logro_09_armas");
        CheckLogroPlatino("logro_12_platino");
    }

    private void CheckEdificiosMaster(string idMaster)
    {
        if (!diccionarioLogros.ContainsKey(idMaster) || diccionarioLogros[idMaster].desbloqueado) return;
        bool tieneLaboral = diccionarioLogros.ContainsKey("logro_03_laboral") && diccionarioLogros["logro_03_laboral"].desbloqueado;
        bool tieneAgora   = diccionarioLogros.ContainsKey("logro_05_agora") && diccionarioLogros["logro_05_agora"].desbloqueado;
        bool tieneCesur   = diccionarioLogros.ContainsKey("logro_07_cesur") && diccionarioLogros["logro_07_cesur"].desbloqueado;

        if (tieneLaboral && tieneAgora && tieneCesur) DesbloquearLogroMaestroConNotificacion(idMaster, "FULLSTACK! LOGRO DESBLOQUEADO");
    }

    private void CheckArmasMaster(string idMaster)
    {
        if (!diccionarioLogros.ContainsKey(idMaster) || diccionarioLogros[idMaster].desbloqueado) return;
        int armas = 0;
        if (InventarioManager.Instance != null)
        {
            foreach (var arma in InventarioManager.Instance.armasAdquiridas) { if (arma != null && arma.desbloqueada) armas++; }
        }
        if (armas >= 9) DesbloquearLogroMaestroConNotificacion(idMaster, "NUEVO LOGRO DESBLOQUEADO!");
    }

    private void CheckLogroPlatino(string idPlatino)
    {
        if (!diccionarioLogros.ContainsKey(idPlatino) || diccionarioLogros[idPlatino].desbloqueado) return;
        int conseguidos = 0;
        foreach (var par in diccionarioLogros) { if (par.Value.desbloqueado && par.Key != idPlatino) conseguidos++; }
        if (conseguidos >= 11) DesbloquearLogroMaestroConNotificacion(idPlatino, "DESBLOQUEASTE EL LOGRO PLATINO!");
    }

    private void CheckMonstruosMaster(string idMaster)
    {
        if (!diccionarioLogros.ContainsKey(idMaster) || diccionarioLogros[idMaster].desbloqueado) return;
        MonstruoBase[] todos = Resources.LoadAll<MonstruoBase>("Monstruos");
        bool completado = true;
        foreach (var m in todos) { if (!m.descubierto) completado = false; }
        if (completado && todos.Length > 0) DesbloquearLogroMaestroConNotificacion(idMaster, "NUEVO LOGRO DESBLOQUEADO!");
    }

    private void DesbloquearLogroMaestroConNotificacion(string id, string msg)
    {
        DesbloquearLogro(id);
        if (NotificacionManager.Instance != null) NotificacionManager.Instance.MostrarNotificacion(msg, iconoCandadoDesbloqueado);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F12))
        {
            PlayerPrefs.DeleteAll(); PlayerPrefs.Save();
            CargarDatosInternos(); OnLogrosActualizados?.Invoke();
            Debug.Log("⚠️ ¡MEMORIA BORRADA!");
        }
    }
}