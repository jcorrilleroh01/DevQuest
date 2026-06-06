using UnityEngine;

[CreateAssetMenu(fileName = "NuevoAtaque", menuName = "RPG/Nuevo Ataque")]
public class AtaqueBase : ScriptableObject
{
    public string nombreAtaque; // Ej: "Almohadazo" o "Git Commit"
    public int potencia;
    public Sprite iconoAtaque;
        public bool descubierto; // Si es true, se ve normal. Si es false, sale en negro.
        public string idArmaPertenece;

    public float precision = 100f; // Porcentaje de acierto
}