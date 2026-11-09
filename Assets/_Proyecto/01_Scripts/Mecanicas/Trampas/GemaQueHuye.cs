// =====================================================================
// GemaQueHuye.cs
// Proyecto: Inti y Killa
// Descripción: Una gema de verdad (sí cuenta para las estrellas) que se
//              escapa a otro lugar cuando su dueño se acerca. Huye una sola
//              vez y se queda en su nuevo lugar, que siempre se puede
//              alcanzar: quien ya la conoce va directo a buscarla allá.
// Uso: se pone en el mismo objeto que la Gema.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Gema))]
public class GemaQueHuye : TrampaBase
{
    [SerializeField] private Vector2 destino = Vector2.zero;
    [SerializeField] private float distanciaDeDeteccion = 2f;
    [SerializeField] private float velocidadHuida = 12f;

    private Gema gema;
    private bool yaHuyo = false;

    public void Configurar(Vector2 nuevoDestino)
    {
        destino = nuevoDestino;
    }

    protected override void Awake()
    {
        base.Awake();
        gema = GetComponent<Gema>();
    }

    private void Update()
    {
        if (yaHuyo == true)
        {
            return;
        }

        foreach (ControladorJugador jugador in jugadores)
        {
            // Solo huye de quien puede recogerla: el otro ni la asusta.
            if (jugador == null || jugador.EstaVivo == false || jugador.Elemento != gema.Elemento)
            {
                continue;
            }

            float distancia = Vector2.Distance(jugador.transform.position, transform.position);
            if (distancia <= distanciaDeDeteccion)
            {
                yaHuyo = true;
                StartCoroutine(Huir());
                return;
            }
        }
    }

    private IEnumerator Huir()
    {
        Vector3 posicionActual = transform.position;
        Vector3 posicionFinal = new Vector3(destino.x, destino.y, posicionActual.z);

        while (Vector3.Distance(posicionActual, posicionFinal) > 0.05f)
        {
            posicionActual = Vector3.MoveTowards(posicionActual, posicionFinal, velocidadHuida * Time.deltaTime);
            gema.CambiarPosicionBase(posicionActual);
            yield return null;
        }

        gema.CambiarPosicionBase(posicionFinal);
    }
}
