// =====================================================================
// PuertaSalida.cs
// Proyecto: Inti y Killa
// Descripción: Puerta de salida de un elemento (fuego o agua).
//              Está "ocupada" cuando adentro hay un personaje que TIENE
//              ese elemento en este momento (importante por el eclipse).
//              El GestorNivel declara la victoria cuando todas las
//              puertas están ocupadas a la vez.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PuertaSalida : MonoBehaviour
{
    [SerializeField] private TipoElemento elementoRequerido = TipoElemento.Fuego;

    private List<ControladorJugador> jugadoresDentro = new List<ControladorJugador>();

    public bool EstaOcupada
    {
        get
        {
            foreach (ControladorJugador jugador in jugadoresDentro)
            {
                if (jugador != null && jugador.Elemento == elementoRequerido)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public void ConfigurarPuerta(TipoElemento nuevoElemento)
    {
        elementoRequerido = nuevoElemento;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null && jugadoresDentro.Contains(jugador) == false)
        {
            jugadoresDentro.Add(jugador);
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null)
        {
            jugadoresDentro.Remove(jugador);
        }
    }
}
