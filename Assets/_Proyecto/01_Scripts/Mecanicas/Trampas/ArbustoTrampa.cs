// =====================================================================
// ArbustoTrampa.cs
// Proyecto: Inti y Killa
// Descripción: Un arbusto de adorno sobre el piso. Cuando alguien se acerca,
//              se sacude (aviso) y de él salta un bicho que muerde a corta
//              distancia; después vuelve a esconderse. La pista: es el único
//              arbusto que tiene dos puntitos brillantes (ojos).
// Uso: objeto con SpriteRenderer (el arbusto) a la altura del piso.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ArbustoTrampa : TrampaActivable
{
    [SerializeField] private float distanciaDeDeteccion = 1.6f;
    [SerializeField] private float segundosDeAviso = 0.4f;
    [SerializeField] private float distanciaDelMordisco = 1.4f;
    [SerializeField] private float segundosMordiendo = 0.5f;
    [SerializeField] private float segundosEscondido = 1.5f;
    [SerializeField] private Vector2 tamanoMordisco = new Vector2(1f, 0.8f);
    [SerializeField] private string mensajeDeMuerte = "El arbusto tenía dientes y mordió a {0}.";

    private Vector3 posicionOriginal;
    private bool estaMordiendo = false;
    private Vector2 centroDelMordisco;

    protected override void Awake()
    {
        base.Awake();
        posicionOriginal = transform.position;
    }

    private void Update()
    {
        if (estaMordiendo == true)
        {
            ControladorJugador mordido = BuscarJugadorEnRectangulo(centroDelMordisco, tamanoMordisco);
            if (mordido != null)
            {
                mordido.Morir(mensajeDeMuerte.Replace("{0}", mordido.NombreJugador));
            }
            return;
        }

        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        Vector2 zona = new Vector2(distanciaDeDeteccion * 2f, 2f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(posicionOriginal, zona);
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        float direccion = 1f;
        if (causante != null && causante.transform.position.x < posicionOriginal.x)
        {
            direccion = -1f;
        }
        StartCoroutine(SacudirYMorder(direccion));
    }

    private IEnumerator SacudirYMorder(float direccion)
    {
        // 1) Aviso: el arbusto se sacude.
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            float temblor = Mathf.Sin(tiempo * 45f) * 0.1f;
            transform.position = posicionOriginal + new Vector3(temblor, 0f, 0f);
            yield return null;
        }

        // 2) El mordisco: se estira hacia el jugador por un momento.
        transform.position = posicionOriginal + new Vector3(direccion * distanciaDelMordisco * 0.5f, 0f, 0f);
        centroDelMordisco = new Vector2(posicionOriginal.x + direccion * distanciaDelMordisco, posicionOriginal.y + 0.2f);
        estaMordiendo = true;
        yield return new WaitForSeconds(segundosMordiendo);

        // 3) Vuelve a ser un arbusto quieto y descansa antes de poder morder otra vez.
        estaMordiendo = false;
        transform.position = posicionOriginal;
        yield return new WaitForSeconds(segundosEscondido);
        Rearmar();
    }
}
