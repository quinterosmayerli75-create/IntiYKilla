// =====================================================================
// PuertaSalida.cs
// Proyecto: Inti y Killa
// Descripción: Puerta de salida de un personaje. Cada uno tiene la suya.
//              El nivel se gana cuando los DOS están parados en su
//              puerta al mismo tiempo (lo revisa el GestorNivel).
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuertaSalida : MonoBehaviour
{
    [SerializeField] private TipoElemento elementoRequerido = TipoElemento.Fuego;

    private bool estaOcupada = false;

    public bool EstaOcupada
    {
        get { return estaOcupada; }
    }

    public void ConfigurarPuerta(TipoElemento nuevoElemento)
    {
        elementoRequerido = nuevoElemento;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador == null)
        {
            return;
        }

        if (jugador.Elemento == elementoRequerido)
        {
            estaOcupada = true;

            if (GestorNivel.Instancia != null)
            {
                GestorNivel.Instancia.RevisarVictoria();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador == null)
        {
            return;
        }

        if (jugador.Elemento == elementoRequerido)
        {
            estaOcupada = false;
        }
    }
}
