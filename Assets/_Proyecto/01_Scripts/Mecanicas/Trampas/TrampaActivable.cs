// =====================================================================
// TrampaActivable.cs
// Proyecto: Inti y Killa
// Descripción: Trampa que puede dispararse sola o por orden de otra pieza
//              (GemaTrampa, BotonEnganoso). Cada trampa concreta decide qué
//              hace en AlActivarse.
// =====================================================================

using UnityEngine;

public abstract class TrampaActivable : TrampaBase
{
    [Header("Disparo")]
    // Desactivado: la trampa espera a que otra pieza la active.
    [SerializeField] private bool seActivaSola = true;

    private bool yaActivada = false;

    protected bool SeActivaSola
    {
        get { return seActivaSola; }
    }

    protected bool YaActivada
    {
        get { return yaActivada; }
    }

    public void ConfigurarDisparo(bool nuevoSeActivaSola)
    {
        seActivaSola = nuevoSeActivaSola;
    }

    public void Activar(ControladorJugador causante)
    {
        if (yaActivada == true)
        {
            return;
        }

        yaActivada = true;
        AlActivarse(causante);
    }

    // Las trampas que vuelven a su lugar llaman a esto para poder dispararse otra vez.
    protected void Rearmar()
    {
        yaActivada = false;
    }

    protected abstract void AlActivarse(ControladorJugador causante);
}
