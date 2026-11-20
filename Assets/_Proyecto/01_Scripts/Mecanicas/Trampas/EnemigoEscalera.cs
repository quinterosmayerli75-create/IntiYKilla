// =====================================================================
// EnemigoEscalera.cs
// Proyecto: Inti y Killa
// Descripción: Un monstruo grande que parece muy peligroso (tiene ojos
//              enojados y gruñe moviéndose). En realidad no hace daño: es
//              sólido y la única forma de llegar a una plataforma alta es
//              subirse a su cabeza y saltar desde ahí. Engaña al revés: da
//              miedo acercarse a lo que justamente hay que usar.
// Uso: bloque con BoxCollider2D (el cuerpo); el generador le dibuja los ojos.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemigoEscalera : TrampaBase
{
    [SerializeField] private float alturaDeRespiracion = 0.12f;
    [SerializeField] private float velocidadDeRespiracion = 2.5f;

    private Vector3 posicionOriginal;
    private BoxCollider2D cuerpo;
    private bool yaSeDescubrio = false;

    protected override void Awake()
    {
        base.Awake();
        posicionOriginal = transform.position;
        cuerpo = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // "Respira": sube y baja apenas, para que parezca vivo y amenazante.
        float desplazamiento = Mathf.Sin(Time.time * velocidadDeRespiracion) * alturaDeRespiracion;
        transform.position = posicionOriginal + new Vector3(0f, desplazamiento, 0f);

        if (yaSeDescubrio == true)
        {
            return;
        }

        // La primera vez que alguien se para en su cabeza, se descubre el engaño.
        Bounds limites = cuerpo.bounds;
        Vector2 centroCabeza = new Vector2(limites.center.x, limites.max.y + 0.3f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroCabeza, new Vector2(limites.size.x, 0.6f));
        if (jugador != null && jugador.EstaEnElSuelo() == true)
        {
            yaSeDescubrio = true;
            if (GestorNivel.Instancia != null)
            {
                GestorNivel.Instancia.MostrarAviso("No muerde... ¡era una escalera!", 2f);
            }
        }
    }
}
