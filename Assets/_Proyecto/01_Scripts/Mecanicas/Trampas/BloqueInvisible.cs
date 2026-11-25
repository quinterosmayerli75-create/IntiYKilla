// =====================================================================
// BloqueInvisible.cs
// Proyecto: Inti y Killa
// Descripción: No se ve ni se choca con él, hasta que un jugador salta
//              debajo: aparece de golpe y le corta el salto. Después de unos
//              segundos se vuelve a esconder: así nunca deja a nadie
//              atascado, y quien ya lo conoce salta desde otro lugar.
// Uso: plataforma con BoxCollider2D (el dibujo puede estar en piezas hijas).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BloqueInvisible : TrampaActivable
{
    [SerializeField] private float segundosVisible = 3f;

    private BoxCollider2D colisionador;
    private SpriteRenderer[] dibujos;
    private Vector2 centroZona;
    private Vector2 tamanoZona;

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        dibujos = GetComponentsInChildren<SpriteRenderer>();

        // Los límites se leen antes de apagar el collider.
        Bounds limites = colisionador.bounds;
        centroZona = limites.center;
        tamanoZona = limites.size;

        colisionador.enabled = false;
        CambiarVisibilidad(false);
    }

    private void Update()
    {
        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && JugadorSaltaPorDebajo(jugador) == true)
            {
                Activar(jugador);
                return;
            }
        }
    }

    // Solo cuenta si va subiendo y su cabeza entra por la parte de abajo del bloque.
    private bool JugadorSaltaPorDebajo(ControladorJugador jugador)
    {
        bool vaSubiendo = jugador.VelocidadVertical > 0.1f;
        bool estaDebajo = jugador.LimitesCuerpo.center.y < centroZona.y;
        return vaSubiendo == true && estaDebajo == true && JugadorTocaRectangulo(jugador, centroZona, tamanoZona) == true;
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        colisionador.enabled = true;
        CambiarVisibilidad(true);
        StartCoroutine(EsconderseDespues());
    }

    // Vuelve a ser invisible y queda lista para sorprender otra vez.
    private IEnumerator EsconderseDespues()
    {
        yield return new WaitForSeconds(segundosVisible);
        colisionador.enabled = false;
        CambiarVisibilidad(false);
        Rearmar();
    }

    private void CambiarVisibilidad(bool esVisible)
    {
        foreach (SpriteRenderer dibujo in dibujos)
        {
            // La pista sutil se queda visible: es lo único que delata al bloque.
            if (dibujo.gameObject.name != "Pista sutil")
            {
                dibujo.enabled = esVisible;
            }
        }
    }
}
