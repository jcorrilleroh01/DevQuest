using UnityEngine;

public class CargarOutfit : MonoBehaviour
{
    void Start()
    {
        // 1. Preguntar al Cerebro (GameManager) qué ropa toca
        if (GameManager.Instance != null && GameManager.Instance.outfitActual != null)
        {
            // 2. Vestirse con esa ropa
            GetComponent<Animator>().runtimeAnimatorController = GameManager.Instance.outfitActual;
        }
        else
        {
            // Debug para saber si falla el cerebro
            Debug.LogWarning("⚠️ No encontré outfit en el GameManager, me quedo con lo que llevo.");
        }
    }
}