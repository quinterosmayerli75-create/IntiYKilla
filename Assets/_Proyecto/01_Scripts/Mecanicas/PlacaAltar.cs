// =====================================================================
// PlacaAltar.cs
// Proyecto: Inti y Killa
// Descripción: Una de las dos placas de un altar de eclipse. Solo
//              recuerda qué personaje está parado encima y brilla
//              cuando alguien la pisa. El que decide el eclipse es el
//              script AltarEclipse (el "padre" de las dos placas).
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlacaAltar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer dibujoPlaca;
    [SerializeField] private Color colorVacia = new Color(0.55f, 0.50f, 0.65f);
    [SerializeField] private Color colorOcupada = new Color(1.00f, 0.95f, 0.60f);

    private List<ControladorJugador> jugadoresEncima = new List<ControladorJugador>();

    // El primer personaje que está encima (o null si está vacía).
    public ControladorJugador Ocupante
    {
        get
        {
            foreach (ControladorJugador jugador in jugadoresEncima)
            {
                if (jugador != null && jugador.EstaVivo == true)
                {
                    return jugador;
                }
            }
            return null;
        }
    }

    public void Configurar(SpriteRenderer nuevoDibujo, Color nuevoColorVacia)
    {
        dibujoPlaca = nuevoDibujo;
        colorVacia = nuevoColorVacia;
    }

    private void Start()
    {
        ActualizarDibujo();
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null && jugadoresEncima.Contains(jugador) == false)
        {
            jugadoresEncima.Add(jugador);
            ActualizarDibujo();
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null)
        {
            jugadoresEncima.Remove(jugador);
            ActualizarDibujo();
        }
    }

    private void ActualizarDibujo()
    {
        if (dibujoPlaca == null)
        {
            return;
        }

        if (jugadoresEncima.Count > 0)
        {
            dibujoPlaca.color = colorOcupada;
        }
        else
        {
            dibujoPlaca.color = colorVacia;
        }
    }
}
