// =====================================================================
// PalancaSospechosa.cs
// Proyecto: Inti y Killa
// Descripción: Es una palanca de verdad (hace lo que promete), pero la
//              primera vez que alguien la usa castiga al compañero.
//              Se pone en un objeto que ya tiene Palanca.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Palanca))]
public class PalancaSospechosa : TrampaBase
{
    [SerializeField] private TipoCastigo castigo = TipoCastigo.SeAbreElSuelo;
    // Tiempo que tiene el compañero para moverse (regla de justicia).
    [SerializeField] private float tiempoDeAviso = 0.8f;

    private Palanca palanca;
    private bool yaSeUso = false;

    public void Configurar(TipoCastigo nuevoCastigo)
    {
        castigo = nuevoCastigo;
    }

    protected override void Awake()
    {
        base.Awake();
        palanca = GetComponent<Palanca>();
    }

    private void Update()
    {
        if (yaSeUso == true)
        {
            return;
        }

        ControladorJugador quienLaUso = palanca.UltimoJugadorQueLaUso;
        if (quienLaUso != null)
        {
            yaSeUso = true;
            CastigoTraicion.Aplicar(quienLaUso, ObtenerCompanero(quienLaUso), castigo, tiempoDeAviso, SpriteBase);
        }
    }
}
