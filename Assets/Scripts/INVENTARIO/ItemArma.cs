using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "RPG/Nueva Arma")]
public class ItemArma : ScriptableObject
{

    [Header("Ajustes Visuales en Inventario")]
    public Vector2 posicionUI = Vector2.zero;
    public Vector3 rotacionUI = Vector3.zero;
    public Vector3 escalaUI = Vector3.one; // Por defecto a 1,1,1
    public Vector2 tamanoUI = new Vector2(150, 150); // ¡NUEVO! Controla Width (x) y Height (y)
    [Header("Identidad")]
    public string nombre;
    [TextArea] public string descripcion;
    public Sprite icono;
    public bool desbloqueada;

    [Header("Estadísticas (Valores del 0 al 100)")]
    [Range(0, 100)] public float ataque;
    [Range(0, 100)] public float velocidad;
    [Range(0, 100)] public float rango;
}