// =====================================================================
// VariantePorVuelta.cs
// Proyecto: Inti y Killa
// Descripción: Hace que un objeto exista solo en algunas vueltas de la sala
//              que se repite. Con esto la misma sala cambia una regla en cada
//              vuelta: un piso que antes era seguro ahora no está, aparece una
//              trampa nueva, la gema buena pasa al otro lado, etc.
// Uso: se agrega a cualquier objeto; el generador indica en qué vueltas
//      aparece (0 = la primera vuelta).
// =====================================================================

using UnityEngine;

public class VariantePorVuelta : MonoBehaviour
{
    [SerializeField] private int[] vueltasEnQueAparece = new int[] { 0 };

    public void Configurar(int[] nuevasVueltas)
    {
        vueltasEnQueAparece = nuevasVueltas;
    }

    private void Awake()
    {
        int vuelta = SalaQueSeRepite.VueltaActual;
        bool aparece = false;
        foreach (int vueltaPermitida in vueltasEnQueAparece)
        {
            if (vueltaPermitida == vuelta)
            {
                aparece = true;
            }
        }

        // Se apaga antes del Start de todos: así ni las puertas ni las gemas
        // de otra vuelta cuentan para ganar el nivel.
        if (aparece == false)
        {
            gameObject.SetActive(false);
        }
    }
}
