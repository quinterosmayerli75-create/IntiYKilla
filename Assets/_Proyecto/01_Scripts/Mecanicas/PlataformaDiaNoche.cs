// =====================================================================
// PlataformaDiaNoche.cs
// Proyecto: Inti y Killa
// Descripción: Plataforma que solo existe de día o solo de noche. Cuando no
//              le toca se ve como un reflejo tenue, para que se pueda planear
//              el camino. Parpadea antes de desaparecer (regla de justicia).
// Uso: bloque con SpriteRenderer + Collider2D. Necesita un CicloDiaNoche
//      en la escena.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PlataformaDiaNoche : MonoBehaviour
{
    [SerializeField] private bool existeDeDia = true;
    [SerializeField] private float transparenciaOculta = 0.22f;
    [SerializeField] private float segundosDeGracia = 0.6f;

    private SpriteRenderer dibujo;
    private Collider2D colisionador;
    private ControladorJugador[] jugadores;
    private Color colorBase;

    public void Configurar(bool nuevoExisteDeDia)
    {
        existeDeDia = nuevoExisteDeDia;
    }

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        colisionador = GetComponent<Collider2D>();
        jugadores = FindObjectsByType<ControladorJugador>();
        colorBase = dibujo.color;
    }

    private void Update()
    {
        CicloDiaNoche ciclo = CicloDiaNoche.Instancia;
        if (ciclo == null)
        {
            return;
        }

        bool leToca = ciclo.EsDeDia == existeDeDia;

        // Sigue un momento después del cambio: así, durante ese instante existen
        // las dos clases de plataforma y se puede saltar de una a la otra.
        bool estaEnGracia = leToca == false && ciclo.SegundosEnFase < segundosDeGracia;
        bool debeExistir = leToca == true || estaEnGracia == true;

        // No aparece encima de un jugador: lo lanzaría por los aires.
        bool puedeEstarSolida = debeExistir == true;
        if (debeExistir == true && colisionador.enabled == false && HayJugadorAdentro() == true)
        {
            puedeEstarSolida = false;
        }
        colisionador.enabled = puedeEstarSolida;

        float transparencia = transparenciaOculta;
        if (puedeEstarSolida == true)
        {
            transparencia = 1f;

            // Parpadea desde que falta poco hasta que termina la gracia.
            bool vaADesaparecer = estaEnGracia == true || ciclo.EstaPorCambiar == true;
            if (vaADesaparecer == true)
            {
                bool mostrarTenue = Mathf.Repeat(Time.time * 5f, 1f) < 0.5f;
                if (mostrarTenue == true)
                {
                    transparencia = 0.45f;
                }
            }
        }

        dibujo.color = new Color(colorBase.r, colorBase.g, colorBase.b, transparencia);
    }

    private bool HayJugadorAdentro()
    {
        Bounds limites = dibujo.bounds;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (limites.Intersects(jugador.LimitesCuerpo) == true)
            {
                return true;
            }
        }
        return false;
    }
}
