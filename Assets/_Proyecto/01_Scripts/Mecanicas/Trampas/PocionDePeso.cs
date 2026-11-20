// =====================================================================
// PocionDePeso.cs
// Proyecto: Inti y Killa
// Descripción: Una botella que parece dar fuerza. Quien la toma se vuelve
//              tan pesado que el piso donde está parado se rompe y cae al
//              piso de abajo. La pista: la botella es gris y hunde un poco
//              el piso donde está apoyada (se ve una grieta).
// Uso: objeto con SpriteRenderer; el generador le indica qué piso se rompe
//      (un SueloQueDesapareceAlSaltar que no se activa solo).
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PocionDePeso : TrampaBase
{
    [SerializeField] private Vector2 tamanoParaTomar = new Vector2(0.7f, 0.9f);
    [SerializeField] private TrampaActivable pisoQueSeRompe;

    private SpriteRenderer dibujo;
    private bool yaLaTomaron = false;

    public void Configurar(TrampaActivable nuevoPiso)
    {
        pisoQueSeRompe = nuevoPiso;
    }

    protected override void Awake()
    {
        base.Awake();
        dibujo = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (yaLaTomaron == true)
        {
            return;
        }

        ControladorJugador jugador = BuscarJugadorEnRectangulo(transform.position, tamanoParaTomar);
        if (jugador == null)
        {
            return;
        }

        yaLaTomaron = true;
        dibujo.enabled = false;
        jugador.RecordarTrampa("La botella gris pesaba: el piso agrietado no aguantó.");

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡" + jugador.NombreJugador + " se volvió pesadísimo!", 2f);
        }

        if (pisoQueSeRompe != null)
        {
            pisoQueSeRompe.Activar(jugador);
        }
    }
}
