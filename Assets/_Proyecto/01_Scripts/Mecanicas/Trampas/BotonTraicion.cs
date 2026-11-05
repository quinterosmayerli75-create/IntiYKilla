// =====================================================================
// BotonTraicion.cs
// Proyecto: Inti y Killa
// Descripción: Parece un botón normal, pero castiga al COMPAÑERO de quien
//              lo pisa: le cae lava, le salen púas o se le abre el suelo.
//              Se pone en un objeto que ya tiene BotonPresion.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(BotonPresion))]
public class BotonTraicion : TrampaBase
{
    [SerializeField] private TipoCastigo castigo = TipoCastigo.SalenPuas;
    // Tiempo que tiene el compañero para moverse (regla de justicia).
    [SerializeField] private float tiempoDeAviso = 0.8f;
    [SerializeField] private Vector2 tamanoZona = new Vector2(1.2f, 0.6f);

    private bool yaSeUso = false;

    public void Configurar(TipoCastigo nuevoCastigo)
    {
        castigo = nuevoCastigo;
    }

    private void Update()
    {
        if (yaSeUso == true)
        {
            return;
        }

        Vector2 centroZona = (Vector2)transform.position + new Vector2(0f, tamanoZona.y / 2f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroZona, tamanoZona);
        if (jugador != null)
        {
            yaSeUso = true;
            CastigoTraicion.Aplicar(jugador, ObtenerCompanero(jugador), castigo, tiempoDeAviso, SpriteBase);
        }
    }
}
