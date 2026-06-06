using UnityEngine;
using System.Collections.Generic; // <--- NECESARIO PARA LISTAS

[CreateAssetMenu(fileName = "NuevoMonstruo", menuName = "RPG/Nuevo Monstruo")]
public class MonstruoBase : ScriptableObject
{
    [Header("Identidad")]
    public string nombre; 
        public string tipo; 
                public string debilidad; 

[Header("Estado de la PDA")]
    public bool descubierto; // Si es true, se ve normal. Si es false, sale en negro.
    [TextArea] public string descripcion; 
        [TextArea] public string lore; 

    public Sprite spriteFrontal; 

    [Header("Stats Base")]
    public int vidaMax;
    public int ataque;
    public int defensa;
    
    [Header("Ataques Conocidos")]
    public List<AtaqueBase> listaAtaques; // <--- ESTO ES LO NUEVO
}