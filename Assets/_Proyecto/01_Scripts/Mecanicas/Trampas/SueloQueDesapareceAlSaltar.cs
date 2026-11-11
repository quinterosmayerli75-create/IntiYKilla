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

        // Franja justo encima del piso: ahí están los pies de quien lo pisa
        // y también los de quien acaba de despegar.
        Bounds limites = limitesDelPiso;
        Vector2 centroFranja = new Vector2(limites.center.x, limites.max.y + 0.6f);
        Vector2 tamanoFranja = new Vector2(limites.size.x, 1.2f);

        foreach (ControladorJugador jugador in jugadores)
        {
            bool acabaDeSaltar = jugador.VelocidadVertical > 2f && jugador.EstaEnElSuelo() == false;
            if (jugador.EstaVivo == true && acabaDeSaltar == true && JugadorTocaRectangulo(jugador, centroFranja, tamanoFranja) == true)
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
