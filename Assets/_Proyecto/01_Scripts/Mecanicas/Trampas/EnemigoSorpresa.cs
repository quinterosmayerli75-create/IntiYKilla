// =====================================================================
// EnemigoSorpresa.cs
// Proyecto: Inti y Killa
// Descripción: Sale del suelo cuando alguien se acerca y corre hacia él. No
//              puede alcanzar a quien sigue corriendo, y desaparece después
//              de un tramo corto: nunca se queda tapando el camino.
// Uso: objeto con SpriteRenderer, colocado a la altura del piso.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class EnemigoSorpresa : TrampaActivable
{
    [SerializeField] private Vector2 tamano = new Vector2(0.9f, 0.9f);
    [SerializeField] private float distanciaDeDeteccion = 1.5f;
    [SerializeField] private float tiempoParaSalir = 0.3f;
    [SerializeField] private float velocidad = 3f;
    [SerializeField] private float distanciaARecorrer = 2f;
    [SerializeField] private string mensajeDeMuerte = "¡Sorpresa! Ahí abajo vivía alguien.";

    private SpriteRenderer dibujo;
    private bool estaAfuera = false;

    protected override void Awake()
    {
        base.Awake();
        dibujo = GetComponent<SpriteRenderer>();
        dibujo.enabled = false;
    }

    private void Update()
    {
        if (estaAfuera == true)
        {
            ControladorJugador atrapado = BuscarJugadorEnRectangulo(transform.position, tamano);
            if (atrapado != null)
            {
                atrapado.Morir(mensajeDeMuerte);
            }
            return;
        }

        if (SeActivaSola == true && YaActivada == false)
        {
            Vector2 zona = new Vector2(distanciaDeDeteccion * 2f, 2f);
            ControladorJugador jugador = BuscarJugadorEnRectangulo(transform.position, zona);
            if (jugador != null)
            {
                Activar(jugador);
            }
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(SalirYCorrer(causante));
    }

    private IEnumerator SalirYCorrer(ControladorJugador causante)
    {
        // Sale despacio: ese momento es el aviso para escapar.
        dibujo.enabled = true;
        Vector3 posicionFinal = transform.position;
        Vector3 posicionEscondida = posicionFinal + new Vector3(0f, -tamano.y, 0f);
        float tiempo = 0f;
        while (tiempo < tiempoParaSalir)
        {
            tiempo = tiempo + Time.deltaTime;
            transform.position = Vector3.Lerp(posicionEscondida, posicionFinal, tiempo / tiempoParaSalir);
            yield return null;
        }
        transform.position = posicionFinal;
        estaAfuera = true;

        float direccion = 1f;
        if (causante != null && causante.LimitesCuerpo.center.x < transform.position.x)
        {
            direccion = -1f;
        }

        float recorrido = 0f;
        while (recorrido < distanciaARecorrer)
        {
            float paso = velocidad * Time.deltaTime;
            recorrido = recorrido + paso;
            transform.position = transform.position + new Vector3(direccion * paso, 0f, 0f);
            yield return null;
        }

        estaAfuera = false;
        dibujo.enabled = false;
    }
}
