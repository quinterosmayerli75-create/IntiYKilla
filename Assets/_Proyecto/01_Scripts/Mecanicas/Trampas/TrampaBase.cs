// =====================================================================
// TrampaBase.cs
// Proyecto: Inti y Killa
// Descripción: Lo que comparten todas las trampas del juego:
//              - Conoce a los dos jugadores y sabe si tocan una zona.
//              - Puede matar a un jugador con un mensaje gracioso.
//              - Puede encontrar al compañero (trampas de traición).
//              - Opción del Inspector para una pista visual muy sutil.
// Las trampas revisan la posición de los jugadores cada frame en vez de usar
// triggers: así funcionan aunque su collider esté apagado o estén escondidas.
// =====================================================================

using UnityEngine;

public abstract class TrampaBase : MonoBehaviour
{
    [Header("Pista visual (las trampas se ven igual que las piezas normales)")]
    // Activada: aparece una marquita casi invisible, como una grieta o un brillo leve.
    [SerializeField] private bool mostrarPistaSutil = false;
    [SerializeField] private Sprite spriteBase;
    [SerializeField] private Color colorPista = new Color(0f, 0f, 0f, 0.25f);
    [SerializeField] private Vector2 tamanoPista = new Vector2(0.5f, 0.05f);
    [SerializeField] private Vector2 posicionPista = Vector2.zero;

    protected ControladorJugador[] jugadores;

    protected Sprite SpriteBase
    {
        get { return spriteBase; }
    }

    // Lo usa el generador de escenas: es el cuadrado blanco para dibujar castigos y pistas.
    public void ConfigurarSpriteBase(Sprite nuevoSprite)
    {
        spriteBase = nuevoSprite;
    }

    protected virtual void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();

        if (mostrarPistaSutil == true)
        {
            CrearPista();
        }
    }

    private void CrearPista()
    {
        if (spriteBase == null)
        {
            return;
        }

        GameObject pista = new GameObject("Pista sutil");
        pista.transform.SetParent(transform);
        Vector3 escalaPadre = transform.lossyScale;
        pista.transform.localPosition = new Vector3(posicionPista.x / escalaPadre.x, posicionPista.y / escalaPadre.y, 0f);
        pista.transform.localScale = new Vector3(tamanoPista.x / escalaPadre.x, tamanoPista.y / escalaPadre.y, 1f);

        SpriteRenderer dibujo = pista.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteBase;
        dibujo.color = colorPista;
        dibujo.sortingOrder = 5;
    }

    // Devuelve el jugador vivo que toca el rectángulo, o null si no hay ninguno.
    protected ControladorJugador BuscarJugadorEnRectangulo(Vector2 centro, Vector2 tamano)
    {
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador != null && jugador.EstaVivo == true && JugadorTocaRectangulo(jugador, centro, tamano) == true)
            {
                return jugador;
            }
        }

        return null;
    }

    protected bool JugadorTocaRectangulo(ControladorJugador jugador, Vector2 centro, Vector2 tamano)
    {
        Bounds cuerpo = jugador.LimitesCuerpo;
        bool cruzaEnX = cuerpo.max.x > centro.x - tamano.x / 2f && cuerpo.min.x < centro.x + tamano.x / 2f;
        bool cruzaEnY = cuerpo.max.y > centro.y - tamano.y / 2f && cuerpo.min.y < centro.y + tamano.y / 2f;
        return cruzaEnX == true && cruzaEnY == true;
    }

    // El otro jugador. Con esto las trampas de traición castigan al compañero.
    protected ControladorJugador ObtenerCompanero(ControladorJugador jugador)
    {
        foreach (ControladorJugador candidato in jugadores)
        {
            if (candidato != null && candidato != jugador)
            {
                return candidato;
            }
        }

        return null;
    }
}
