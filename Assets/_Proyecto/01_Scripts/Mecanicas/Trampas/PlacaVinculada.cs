// =====================================================================
// PlacaVinculada.cs
// Proyecto: Inti y Killa
// Descripción: Une dos placas de altar. Cuando los dos están parados en
//              ellas, si uno se baja, el que se queda recibe un castigo.
//              Se puede poner sobre un altar de eclipse: después de cambiar
//              de poderes, el primero en irse traiciona al otro.
// =====================================================================

using UnityEngine;

public class PlacaVinculada : TrampaBase
{
    [SerializeField] private PlacaAltar placaUno;
    [SerializeField] private PlacaAltar placaDos;
    [SerializeField] private TipoCastigo castigo = TipoCastigo.SalenPuas;
    // Tiempo que tiene el compañero para bajarse también (regla de justicia).
    [SerializeField] private float tiempoDeAviso = 0.8f;

    private ControladorJugador ocupanteUno = null;
    private ControladorJugador ocupanteDos = null;
    private bool estaArmada = false;
    private bool yaSeUso = false;

    public void Configurar(PlacaAltar nuevaPlacaUno, PlacaAltar nuevaPlacaDos, TipoCastigo nuevoCastigo)
    {
        placaUno = nuevaPlacaUno;
        placaDos = nuevaPlacaDos;
        castigo = nuevoCastigo;
    }

    private void Update()
    {
        if (yaSeUso == true || placaUno == null || placaDos == null)
        {
            return;
        }

        ControladorJugador encimaDeUno = placaUno.Ocupante;
        ControladorJugador encimaDeDos = placaDos.Ocupante;

        if (estaArmada == false)
        {
            // Se arma cuando cada placa tiene a un jugador distinto.
            if (encimaDeUno != null && encimaDeDos != null && encimaDeUno != encimaDeDos)
            {
                estaArmada = true;
                ocupanteUno = encimaDeUno;
                ocupanteDos = encimaDeDos;
            }
            return;
        }

        if (encimaDeUno == null && encimaDeDos != null)
        {
            Disparar(ocupanteUno, encimaDeDos);
        }
        else if (encimaDeDos == null && encimaDeUno != null)
        {
            Disparar(ocupanteDos, encimaDeUno);
        }
    }

    private void Disparar(ControladorJugador quienSeBajo, ControladorJugador quienSeQuedo)
    {
        yaSeUso = true;
        CastigoTraicion.Aplicar(quienSeBajo, quienSeQuedo, castigo, tiempoDeAviso, SpriteBase);
    }
}
