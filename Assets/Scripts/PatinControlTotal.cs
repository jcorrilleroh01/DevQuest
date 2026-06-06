using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class DatosVisualesPatin {
    public string idOutfit;      
    public Sprite spriteMontado; 
    public float escalaX = 1f; 
    public float escalaY = 1f; 
}


public class PatinControlTotal : MonoBehaviour {
    [Header("Conexión de Viaje")]
    public string nombreDeLaOtraEscena;
    public string idDeEstePatin;     
    public string idPatinDestino;    
    public bool voltearSoloAJaime = true; 
    
    [Header("Transición")]
    public Animator panelTransicion;
    public float tiempoDeEspera = 1f;
public AudioSource sonidoAbrir; 
    [Header("Objetos Visuales")]
    public GameObject patinAparcado; 
    public GameObject actorDeslizante; 
    public SpriteRenderer spriteJaimeFalso; 
    public Transform puntoFueraPantalla;   
    
    // --- NUEVA VARIABLE ---
    [Tooltip("Punto exacto al lado del patín donde aparecerá el jugador al bajarse")]
    public Transform puntoBajadaJugador; 
    
    public float velocidadPatin = 12f;

    [Header("Configuración de Rotación")]
    [Tooltip("Marca esta casilla si en ESTA escena viaja o entra del revés.")]
    public bool invertirMirada = false;

    [Header("Lista de Outfits")]
    public List<DatosVisualesPatin> listaOutfitsPatin;

    private GameObject jugadorReal;
    private bool enRango = false;
    private bool viajando = false;
    public static string patinDestinoActual = ""; 

    void Start() {
        jugadorReal = GameObject.FindGameObjectWithTag("Player");
        if (actorDeslizante != null) actorDeslizante.SetActive(false);

        if (patinDestinoActual == idDeEstePatin) {
            patinDestinoActual = ""; 
            StartCoroutine(SecuenciaLlegada());
        }
    }

    void Update() {
        if (enRango && !viajando && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(SecuenciaSalida());
        }
    }

    void ConfigurarOutfit() {
        if (GameManager.Instance != null && GameManager.Instance.outfitActual != null) {
            string nombre = GameManager.Instance.outfitActual.name.ToLower();
            var config = listaOutfitsPatin.Find(x => nombre.Contains(x.idOutfit.ToLower()));
            if (config != null && spriteJaimeFalso != null) {
                spriteJaimeFalso.sprite = config.spriteMontado;
                spriteJaimeFalso.flipX = voltearSoloAJaime; 
                spriteJaimeFalso.transform.localScale = new Vector3(config.escalaX, config.escalaY, 1f);
            }
        }
    }

    void AjustarOrientacion(Vector3 puntoDestino) {
        actorDeslizante.transform.rotation = Quaternion.identity;
        float diffX = puntoDestino.x - actorDeslizante.transform.position.x;
        float escalaGrupo = 1f;

        if (diffX > 0.01f) {
            escalaGrupo = invertirMirada ? -1f : 1f;
        } else if (diffX < -0.01f) {
            escalaGrupo = invertirMirada ? 1f : -1f;
        }

        actorDeslizante.transform.localScale = new Vector3(escalaGrupo, 1f, 1f);
    }

    IEnumerator SecuenciaSalida() {
        viajando = true;

        if (jugadorReal == null) {
            jugadorReal = GameObject.FindGameObjectWithTag("Player");
        }

        ConfigurarOutfit();

        actorDeslizante.transform.position = patinAparcado.transform.position; 
        AjustarOrientacion(puntoFueraPantalla.position);

        if (jugadorReal != null) jugadorReal.SetActive(false); 
        if (patinAparcado != null) patinAparcado.SetActive(false); 
        actorDeslizante.SetActive(true); 
if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }
        while (Vector2.Distance(actorDeslizante.transform.position, puntoFueraPantalla.position) > 0.1f) {
             
            actorDeslizante.transform.position = Vector2.MoveTowards(actorDeslizante.transform.position, puntoFueraPantalla.position, velocidadPatin * Time.deltaTime);
            yield return null; 
        }

        if (panelTransicion != null) panelTransicion.Play("Crossfade_Start");
        yield return new WaitForSeconds(tiempoDeEspera);
        
        patinDestinoActual = idPatinDestino; 
        SceneManager.LoadScene(nombreDeLaOtraEscena);
    }

    IEnumerator SecuenciaLlegada() {
        viajando = true; 

        if (jugadorReal == null) {
            jugadorReal = GameObject.FindGameObjectWithTag("Player");
        }

        ConfigurarOutfit();

        if (jugadorReal != null) {
            jugadorReal.transform.position = puntoFueraPantalla.position;
        }
        
        yield return new WaitForEndOfFrame();

        if (jugadorReal != null) jugadorReal.SetActive(false); 
        if (patinAparcado != null) patinAparcado.SetActive(false); 
        
        actorDeslizante.transform.position = puntoFueraPantalla.position;
        actorDeslizante.SetActive(true);
        if (sonidoAbrir != null)
        {
            sonidoAbrir.Play();
        }

        AjustarOrientacion(transform.position);

        while (Vector2.Distance(actorDeslizante.transform.position, transform.position) > 0.1f) {
            actorDeslizante.transform.position = Vector2.MoveTowards(actorDeslizante.transform.position, transform.position, velocidadPatin * Time.deltaTime);
            if (jugadorReal != null) {
                jugadorReal.transform.position = actorDeslizante.transform.position;
            }
            yield return null;
        }

        actorDeslizante.SetActive(false); 
        if (patinAparcado != null) patinAparcado.SetActive(true); 
        
        if (jugadorReal != null) {
            // --- CORRECCIÓN: Usamos el punto de bajada si existe ---
            if (puntoBajadaJugador != null) {
                jugadorReal.transform.position = puntoBajadaJugador.position;
            } else {
                // Respaldo por si se te olvida asignarlo en Unity
                jugadorReal.transform.position = patinAparcado.transform.position; 
            }
            jugadorReal.SetActive(true); 
        }

        viajando = false; 
    }

    private void OnTriggerEnter2D(Collider2D collision) { 
        if (collision.CompareTag("Player")) {
            enRango = true; 
            jugadorReal = collision.gameObject; 
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision) { 
        if (collision.CompareTag("Player")) {
            enRango = false; 
        }
    }
}