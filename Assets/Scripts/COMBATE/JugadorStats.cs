using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StatsJaime", menuName = "RPG/Stats del Jugador")]
public class JugadorStats : ScriptableObject
{
    [Header("Estado Actual")]
    public int nivel = 1;
    public int vidaActual;
    public int vidaMax = 100;
    
    [Header("Ataques Aprendidos")]
    public List<AtaqueBase> misAtaques; // <--- AQUÍ irán Almohadazo y Debugging
}