// =====================================================================
// BotonEnganoso.cs
// Proyecto: Inti y Killa
// Descripción: Parece un botón normal, pero al pisarlo activa una trampa.
//              Se pone en un objeto que ya tiene BotonPresion: así se ve y
//              se hunde igual que los demás botones.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BotonPresion))]
public class BotonEnganoso : TrampaBase
{
    [SerializeField] private List<TrampaActivable> trampasQueActiva = new List<TrampaActivable>();
    [SerializeField] private Vector2 tamanoZona = new Vector2(1.2f, 0.6f);
    [SerializeField] private string mensajeDeMuerte = "Ese botón no abría nada. Nunca lo hizo.";

    private bool yaSeUso = false;

    public void Configurar(List<TrampaActivable> nuevasTrampas)
    {
        trampasQueActiva = nuevasTrampas;
    }

    private void Update()
    {
        if (yaSeUso == true)
        {
            return;
        }

        Vector2 centroZona = (Vector2)transform.position + new Vector2(0f, tamanoZona.y / 2f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroZona, tamanoZona);
        if (jugador == null)
        {
            return;
        }

        yaSeUso = true;
        jugador.RecordarTrampa(mensajeDeMuerte);

        foreach (TrampaActivable trampa in trampasQueActiva)
        {
            if (trampa != null)
            {
                trampa.Activar(jugador);
            }
        }
    }
}
