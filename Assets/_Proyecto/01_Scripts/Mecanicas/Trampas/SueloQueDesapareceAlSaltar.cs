// =====================================================================
// SueloQueDesapareceAlSaltar.cs
// Proyecto: Inti y Killa
// Descripción: Un tramo de piso firme mientras se camina sobre él, pero que
//              desaparece en cuanto alguien SALTA estando encima: al bajar ya
//              no hay piso. Vuelve a aparecer después de unos segundos.
//              Lo justo: tiene grietas finas (pista sutil) y se puede cruzar
//              caminando sin saltar.
// Uso: plataforma con BoxCollider2D (el dibujo puede estar en piezas hijas).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SueloQueDesapareceAlSaltar : TrampaActivable
{
    [SerializeField] private float segundosSinPiso = 2.5f;
    [SerializeField] private float opacidadDesaparecido = 0.15f;

    private BoxCollider2D colisionador;
    private SpriteRenderer[] dibujos;
    private Color[] coloresOriginales;
    // Se guarda al empezar: con el choque apagado Unity informa tamaño cero.
    private Bounds limitesDelPiso;
    // Si cada jugador estaba parado encima en el cuadro anterior.
    private bool[] estabaParadoEncima;

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        limitesDelPiso = colisionador.bounds;
        dibujos = GetComponentsInChildren<SpriteRenderer>();
        coloresOriginales = new Color[dibujos.Length];
        for (int indice = 0; indice < dibujos.Length; indice++)
        {
            coloresOriginales[indice] = dibujos[indice].color;
        }
    }

    private void Update()
    {
        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        // Franja delgada justo encima del piso: ahí están los pies de quien
        // está parado sobre él.
        Bounds limites = limitesDelPiso;
        Vector2 centroFranja = new Vector2(limites.center.x, limites.max.y + 0.3f);
        Vector2 tamanoFranja = new Vector2(limites.size.x, 0.6f);

        if (estabaParadoEncima == null || estabaParadoEncima.Length != jugadores.Length)
        {
            estabaParadoEncima = new bool[jugadores.Length];
        }

        for (int indice = 0; indice < jugadores.Length; indice++)
        {
            ControladorJugador jugador = jugadores[indice];
            bool estaParado = jugador.EstaVivo == true && jugador.EstaEnElSuelo() == true && JugadorTocaRectangulo(jugador, centroFranja, tamanoFranja) == true;

            // Solo cuenta quien estaba PARADO encima y ahora sube: así no se
            // dispara cuando alguien llega saltando desde un piso de abajo.
            bool acabaDeSaltar = estabaParadoEncima[indice] == true && jugador.VelocidadVertical > 2f && jugador.EstaEnElSuelo() == false;
            estabaParadoEncima[indice] = estaParado;

            if (jugador.EstaVivo == true && acabaDeSaltar == true)
            {
                Activar(jugador);
                return;
            }
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        if (causante != null)
        {
            causante.RecordarTrampa("Ese piso agrietado desaparece si saltas encima. Crúzalo caminando.");
        }
        StartCoroutine(DesaparecerYVolver());
    }

    private IEnumerator DesaparecerYVolver()
    {
        colisionador.enabled = false;
        PintarConOpacidad(opacidadDesaparecido);
        yield return new WaitForSeconds(segundosSinPiso);

        // Solo vuelve cuando nadie está dentro del hueco: así no encierra a nadie.
        while (BuscarJugadorEnRectangulo(limitesDelPiso.center, limitesDelPiso.size) != null)
        {
            yield return null;
        }

        colisionador.enabled = true;
        PintarConOpacidad(1f);
        Rearmar();
    }

    private void PintarConOpacidad(float factor)
    {
        for (int indice = 0; indice < dibujos.Length; indice++)
        {
            Color original = coloresOriginales[indice];
            dibujos[indice].color = new Color(original.r, original.g, original.b, original.a * factor);
        }
    }
}
