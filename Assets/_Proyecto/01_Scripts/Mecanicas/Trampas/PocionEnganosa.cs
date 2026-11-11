// =====================================================================
// PocionEnganosa.cs
// Proyecto: Inti y Killa
// Descripción: Una botellita brillante que parece un premio. Quien la toma
//              queda mareado unos segundos: izquierda y derecha se invierten.
//              Es una trampa psicológica: no mata por sí sola, pero hace
//              caminar al revés justo antes de un salto o de un charco.
//              Se ve que "burbujea" (crece y se achica): esa es la pista.
// Uso: objeto con SpriteRenderer (el generador le pone el dibujo).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PocionEnganosa : TrampaBase
{
    [SerializeField] private Vector2 tamanoParaTomar = new Vector2(0.7f, 0.9f);
    [SerializeField] private float segundosMareado = 4f;
    [SerializeField] private float velocidadBurbujeo = 5f;

    private SpriteRenderer dibujo;
    private Vector3 escalaOriginal;
    private bool yaLaTomaron = false;
    private ControladorJugador jugadorMareado;

    protected override void Awake()
    {
        base.Awake();
        dibujo = GetComponent<SpriteRenderer>();
        escalaOriginal = transform.localScale;
    }

    private void Update()
    {
        if (yaLaTomaron == true)
        {
            return;
        }

        // El burbujeo: la botella late como si tuviera algo vivo adentro.
        float latido = 1f + Mathf.Sin(Time.time * velocidadBurbujeo) * 0.08f;
        transform.localScale = new Vector3(escalaOriginal.x * latido, escalaOriginal.y * latido, escalaOriginal.z);

        ControladorJugador jugador = BuscarJugadorEnRectangulo(transform.position, tamanoParaTomar);
        if (jugador != null)
        {
            yaLaTomaron = true;
            StartCoroutine(Marear(jugador));
        }
    }

    private IEnumerator Marear(ControladorJugador jugador)
    {
        dibujo.enabled = false;
        jugadorMareado = jugador;
        jugador.InvertirControles(true);
        jugador.RecordarTrampa("La botellita que burbujeaba invertía los controles.");

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("¡" + jugador.NombreJugador + " está mareado! Controles al revés por " + segundosMareado + " segundos.", 2.5f);
        }

        yield return new WaitForSeconds(segundosMareado);
        jugador.InvertirControles(false);
        jugadorMareado = null;
    }

    // Si el nivel se reinicia a mitad del mareo, nadie queda con los controles al revés.
    private void OnDisable()
    {
        if (jugadorMareado != null)
        {
            jugadorMareado.InvertirControles(false);
            jugadorMareado = null;
        }
    }
}
