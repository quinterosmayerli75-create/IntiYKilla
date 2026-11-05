// =====================================================================
// Palanca.cs
// Proyecto: Inti y Killa
// Descripción: Palanca que se usa con la tecla de habilidad
//              (Inti: S, Killa: flecha abajo) estando cerca de ella.
//              A diferencia del botón, se queda activada aunque el
//              jugador se vaya: es como un interruptor de luz.
// Uso: objeto con BoxCollider2D "Is Trigger" + un hijo "Pivote" que gira.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Palanca : MonoBehaviour
{
    [SerializeField] private List<ObjetoActivable> objetivos = new List<ObjetoActivable>();
    [SerializeField] private Transform pivoteMango;
    [SerializeField] private SpriteRenderer dibujoMango;
    [SerializeField] private float anguloApagada = 35f;
    [SerializeField] private float anguloEncendida = -35f;
    [SerializeField] private Color colorApagada = new Color(0.85f, 0.25f, 0.25f);
    [SerializeField] private Color colorEncendida = new Color(0.35f, 0.85f, 0.35f);

    private List<ControladorJugador> jugadoresCerca = new List<ControladorJugador>();
    private bool estaEncendida = false;
    private ControladorJugador ultimoJugadorQueLaUso = null;

    // Lo consulta PalancaSospechosa para saber a quién culpar.
    public ControladorJugador UltimoJugadorQueLaUso
    {
        get { return ultimoJugadorQueLaUso; }
    }

    public void Configurar(List<ObjetoActivable> nuevosObjetivos, Transform nuevoPivote, SpriteRenderer nuevoMango)
    {
        objetivos = nuevosObjetivos;
        pivoteMango = nuevoPivote;
        dibujoMango = nuevoMango;
    }

    private void Start()
    {
        ActualizarDibujo();
    }

    private void Update()
    {
        // Si alguno de los jugadores cercanos presiona su tecla de habilidad, se alterna.
        foreach (ControladorJugador jugador in jugadoresCerca)
        {
            if (jugador != null && jugador.PresionoHabilidadEsteFrame() == true)
            {
                ultimoJugadorQueLaUso = jugador;
                Alternar();
                break; // Solo un cambio por frame, aunque los dos presionen a la vez.
            }
        }
    }

    private void Alternar()
    {
        estaEncendida = !estaEncendida;

        foreach (ObjetoActivable objetivo in objetivos)
        {
            if (objetivo != null)
            {
                objetivo.RecibirSenal(estaEncendida);
            }
        }

        ActualizarDibujo();
    }

    private void ActualizarDibujo()
    {
        float angulo = anguloApagada;
        Color color = colorApagada;

        if (estaEncendida == true)
        {
            angulo = anguloEncendida;
            color = colorEncendida;
        }

        if (pivoteMango != null)
        {
            pivoteMango.localRotation = Quaternion.Euler(0f, 0f, angulo);
        }

        if (dibujoMango != null)
        {
            dibujoMango.color = color;
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null && jugadoresCerca.Contains(jugador) == false)
        {
            jugadoresCerca.Add(jugador);
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null)
        {
            jugadoresCerca.Remove(jugador);
        }
    }
}
