using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using System.Linq;

public class BatallaManager : MonoBehaviour
{
    public AudioSource audioIntroBatalla; 
    [Header("Referencias UI Paneles")]
    public GameObject panelOpcionesPrincipales; 
    public GameObject panelListaAtaques;        
    public Transform contenedorAtaques;         
    public GameObject botonAtaquePrefab;        
    public Sprite spriteCara2;
    public AudioSource audioGolpe; 
    public AudioSource audioDerrota; 
    public AudioSource audioDerrota2; 
    
    [Header("Referencias UI Batalla")]
    public TextMeshProUGUI textoAnuncio; 
    public Sprite spriteCara;
    public TextMeshProUGUI textoNombreMonstruo; 
    private bool esTurnoJugador = true;
    public Sprite spriteCandadoAbierto;
    
    [Header("Referencias Visuales")]
    public Image imagenJugador;
    public Image imagenMonstruo;
    public Slider sliderVidaJugador;
    public Slider sliderVidaMonstruo;
    public TextMeshProUGUI textoVidaJugador;
    public TextMeshProUGUI textoVidaMonstruo;
    public bool encontrado = false;
    
    [Header("Sprites y Datos")]
    public List<DatosVisualesCombate> listaOutfitsCombate;
    public JugadorStats statsJaime;
    private MonstruoBase monstruoActual;
    private int vidaActualMonstruo;
    private bool eraMonstruoNuevo = false; 

    void Start() {
        if (GestorCursor.Instance != null) {
            GestorCursor.Instance.MostrarCursor();
        }
        if (audioIntroBatalla != null) {
            audioIntroBatalla.Play();
        }
        ConfigurarEscena();
        
        panelOpcionesPrincipales.SetActive(true);
        panelListaAtaques.SetActive(false);
        GenerarBotonesAtaque();
    }

    void ConfigurarEscena() {
        if (GameManager.Instance != null) {
            if (GameManager.Instance.outfitActual != null) {
                string nombreOutfit = GameManager.Instance.outfitActual.name.ToLower();
                var outfitVisual = listaOutfitsCombate.Find(x => nombreOutfit.Contains(x.idOutfit.ToLower()));
                if (outfitVisual != null) imagenJugador.sprite = outfitVisual.spriteIdle;
            }

            if (GameManager.Instance.monsterEncontrado != null) {
                monstruoActual = GameManager.Instance.monsterEncontrado;
                
                if (!monstruoActual.descubierto) {
                    eraMonstruoNuevo = true; 
                    GameManager.Instance.RegistrarMonstruo(monstruoActual.nombre);
                    monstruoActual.descubierto = true; 
                    Debug.Log("¡Avistamiento! " + monstruoActual.nombre + " registrado en la PDA.");
                }
                else
                {
                    eraMonstruoNuevo = false; 
                }

                vidaActualMonstruo = monstruoActual.vidaMax;
                imagenMonstruo.sprite = monstruoActual.spriteFrontal;
                if(textoNombreMonstruo != null) textoNombreMonstruo.text = monstruoActual.nombre;
                textoAnuncio.text = "¡Un " + monstruoActual.nombre + " salvaje apareció!";
                sliderVidaMonstruo.maxValue = monstruoActual.vidaMax;
                sliderVidaMonstruo.value = vidaActualMonstruo;
                textoVidaMonstruo.text = vidaActualMonstruo + " / " + monstruoActual.vidaMax;
            }
        }

        if (statsJaime != null) {
            // 🛡️ SOLUCIÓN: Curamos a Jaime al 100% siempre al inicio de la batalla
            statsJaime.vidaActual = statsJaime.vidaMax;

            sliderVidaJugador.maxValue = statsJaime.vidaMax;
            sliderVidaJugador.value = statsJaime.vidaActual;
            textoVidaJugador.text = statsJaime.vidaActual + " / " + statsJaime.vidaMax;
        }
    }

    // --- FUNCIONES DE BOTONES PRINCIPALES ---

    public void OnBotonAbrirAtaques() {
        if (!esTurnoJugador) return;
        panelOpcionesPrincipales.SetActive(false);
        panelListaAtaques.SetActive(true);
    }

    public void OnBotonCerrarAtaques() {
        panelListaAtaques.SetActive(false);
        panelOpcionesPrincipales.SetActive(true);
    }

    public void OnBotonHuir() {
        if (!esTurnoJugador) return;
        textoAnuncio.text = "¡Has escapado de la batalla!";
        panelOpcionesPrincipales.SetActive(false);
        StartCoroutine(HuirCoroutine());
    }

    IEnumerator HuirCoroutine() {
        yield return new WaitForSeconds(1.5f);
        VolverAlMapa();
    }

    // --- LÓGICA DE COMBATE ---

    public void OnSeleccionarAtaque(AtaqueBase ataque) {
        panelListaAtaques.SetActive(false);
        StartCoroutine(SecuenciaCombate(ataque));
    }

    IEnumerator SecuenciaCombate(AtaqueBase ataque) {
        esTurnoJugador = false;
        panelOpcionesPrincipales.SetActive(false);
        
        textoAnuncio.text = "¡Jaime usa " + ataque.nombreAtaque + "!";
        yield return StartCoroutine(AnimacionEmbestida(imagenJugador.rectTransform, true));
        
        vidaActualMonstruo -= ataque.potencia;
        vidaActualMonstruo = Mathf.Clamp(vidaActualMonstruo, 0, monstruoActual.vidaMax);
        StartCoroutine(EfectoParpadeo(imagenMonstruo));
        yield return StartCoroutine(ActualizarBarraVida(sliderVidaMonstruo, textoVidaMonstruo, vidaActualMonstruo, monstruoActual.vidaMax));

        // --- AQUÍ COMPROBAMOS SI EL MONSTRUO MUERE ---
        // --- AQUÍ COMPROBAMOS SI EL MONSTRUO MUERE ---
            if (vidaActualMonstruo <= 0) {
                if (audioDerrota2 != null) {
                    audioDerrota2.Play();
                }
                textoAnuncio.text = "¡El " + monstruoActual.nombre + " ha sido derrotado!";
                
                // 🛡️ CORTAFUEGOS: Solo marcamos el logro como completado si la Rama de Logros existe
                if (PlayerPrefs.GetInt("PrimerMonstruoDerrotado", 0) == 0)
                {
                    bool logroAnotado = false; // Variable de control

                    if (GestorLogros.Instance != null)
                    {
                        // IMPORTANTE: Revisa que tu logro se llame EXACTAMENTE "logro_04_combat" en Unity
                        GestorLogros.Instance.DesbloquearLogro("logro_04_combat");
                        logroAnotado = true; // Confirmamos que la rama de logros lo ha escuchado
                    }
                    else
                    {
                        Debug.LogError("🚨 ERROR: No hay GestorLogros en esta escena. El logro no se guardará en la rama.");
                    }

                    if (NotificacionManager.Instance != null)
                    {
                        NotificacionManager.Instance.MostrarNotificacion("NUEVO LOGRO DESBLOQUEADO!", spriteCara2);
                    }

                    // Solo si la rama de logros lo ha recibido bien, bloqueamos la variable para no repetir
                    if (logroAnotado)
                    {
                        PlayerPrefs.SetInt("PrimerMonstruoDerrotado", 1);
                        PlayerPrefs.Save();
                    }
                }

                yield return new WaitForSeconds(2f);
                VolverAlMapa(); 
                yield break;
            }

        yield return new WaitForSeconds(1f);
        AtaqueBase atqEnemigo = monstruoActual.listaAtaques[Random.Range(0, monstruoActual.listaAtaques.Count)];
        textoAnuncio.text = "¡" + monstruoActual.nombre + " usa " + atqEnemigo.nombreAtaque + "!";
        yield return StartCoroutine(AnimacionEmbestida(imagenMonstruo.rectTransform, false));

        statsJaime.vidaActual -= atqEnemigo.potencia;
        statsJaime.vidaActual = Mathf.Clamp(statsJaime.vidaActual, 0, statsJaime.vidaMax);
        StartCoroutine(EfectoParpadeo(imagenJugador));
        yield return StartCoroutine(ActualizarBarraVida(sliderVidaJugador, textoVidaJugador, statsJaime.vidaActual, statsJaime.vidaMax));

        if (statsJaime.vidaActual <= 0) {
            textoAnuncio.text = "¡Jaime se ha debilitado!";
            if (audioDerrota != null) {
                audioDerrota.Play();
            }
            yield return new WaitForSeconds(2f);
            statsJaime.vidaActual = statsJaime.vidaMax;
            VolverAlMapa(); 
            yield break;
        }

        yield return new WaitForSeconds(1f);
        panelOpcionesPrincipales.SetActive(true); 
        esTurnoJugador = true;
    }

    // --- SISTEMA DE LISTADO DINÁMICO ---

    void GenerarBotonesAtaque() {
        if (contenedorAtaques == null || statsJaime == null) return;

        foreach (Transform hijo in contenedorAtaques) Destroy(hijo.gameObject);

        List<AtaqueBase> todosLosAtaques = Resources.LoadAll<AtaqueBase>("Ataques/BUENOS").ToList();

        var listaOrdenada = todosLosAtaques
            .OrderByDescending(atq => atq.descubierto) 
            .ThenBy(atq => atq.idArmaPertenece)
            .ToList();

        foreach (AtaqueBase ataque in listaOrdenada) {
            GameObject nuevoBoton = Instantiate(botonAtaquePrefab, contenedorAtaques);
            BotonAtaqueUI scriptUi = nuevoBoton.GetComponent<BotonAtaqueUI>();
            
            if (scriptUi != null) {
                bool estaDesbloqueado = ataque.descubierto;
                scriptUi.ConfigurarBoton(ataque, estaDesbloqueado);

                if (estaDesbloqueado) {
                    nuevoBoton.GetComponent<Button>().onClick.AddListener(() => OnSeleccionarAtaque(ataque));
                }
            }
        }
    }

    // --- UTILIDADES ---

    IEnumerator AnimacionEmbestida(RectTransform tr, bool haciaDerecha) {
        Vector3 posOriginal = tr.anchoredPosition;
        Vector3 objetivo = posOriginal + (haciaDerecha ? Vector3.right : Vector3.left) * 150f;
        float t = 0;

        while(t < 1f) { 
            t += Time.deltaTime * 12f; 
            tr.anchoredPosition = Vector3.Lerp(posOriginal, objetivo, t); 
            yield return null; 
        }

        if (audioGolpe != null) {
            audioGolpe.PlayOneShot(audioGolpe.clip); 
        }

        yield return new WaitForSeconds(0.05f);

        t = 0;
        while(t < 1f) { 
            t += Time.deltaTime * 10f; 
            tr.anchoredPosition = Vector3.Lerp(objetivo, posOriginal, t); 
            yield return null; 
        }
        
        tr.anchoredPosition = posOriginal;
    }

    IEnumerator EfectoParpadeo(Image img) {
        for(int i=0; i<3; i++) { img.color = Color.red; yield return new WaitForSeconds(0.1f); img.color = Color.white; yield return new WaitForSeconds(0.1f); }
    }

    IEnumerator ActualizarBarraVida(Slider slider, TextMeshProUGUI texto, int valorObjetivo, int valorMax) {
        float velocidad = 0.5f; float valorInicial = slider.value; float tiempo = 0;
        while (tiempo < 1f) { tiempo += Time.deltaTime / velocidad; slider.value = Mathf.Lerp(valorInicial, (float)valorObjetivo, tiempo); texto.text = Mathf.RoundToInt(slider.value) + " / " + valorMax; yield return null; }
    }

   void VolverAlMapa()
    {
        if (GestorCursor.Instance != null) {
            GestorCursor.Instance.OcultarCursor();
        }
        string escenaDestino = "JARDIN"; 
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.escenaPreBatalla)) {
            escenaDestino = GameManager.Instance.escenaPreBatalla;
        }

        if (eraMonstruoNuevo)
        {
            // 🛡️ CORRECCIÓN: Añadido seguro anti-crashes para que el juego no se congele si falta el Manager
            if (NotificacionManager.Instance != null)
            {
                NotificacionManager.Instance.MostrarNotificacion("NUEVO MONSTRUO REGISTRADO", spriteCara);
            }
            
            if (GestorLogros.Instance != null)
            {
                GestorLogros.Instance.ComprobarLogrosMaestros();
            }
        }

        SceneManager.LoadScene(escenaDestino);
    }
}