// =====================================================================
// BotonPresion.cs
// Proyecto: Inti y Killa
// Descripción: Botón en el suelo. Mientras un personaje esté parado
//              encima, manda una señal encendida a sus objetivos
//              (compuertas, elevadores...). Al bajarse, la apaga.
//              Es la mecánica clave de cooperación: uno sostiene el
//              botón para que el otro pase.
// Uso: objeto con BoxCollider2D "Is Trigger" + un hijo "Tapa" visual.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BotonPresion : MonoBehaviour
{
    [SerializeField] private List<ObjetoActivable> objetivos = new List<ObjetoActivable>();
    [SerializeField] private SpriteRenderer dibujoTapa;
    [SerializeField] private Color colorSuelto = new Color(0.95f, 0.75f, 0.20f);
    [SerializeField] private Color colorPresionado = new Color(0.35f, 0.85f, 0.35f);

    private List<ControladorJugador> jugadoresEncima = new List<ControladorJugador>();
    private bool estaPresionado = false;
    private Vector3 posicionTapaSuelta;

    // Lo usa el Guardián del Eclipse para saber si los dos botones están pisados.
    public bool EstaPresionado
    {
        get { return estaPresionado; }
    }

    public void Configurar(List<ObjetoActivable> nuevosObjetivos, SpriteRenderer nuevaTapa)
    {
        objetivos = nuevosObjetivos;
        dibujoTapa = nuevaTapa;
    }

    private void Start()
    {
        if (dibujoTapa != null)
        {
            posicionTapaSuelta = dibujoTapa.transform.localPosition;
            dibujoTapa.color = colorSuelto;
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null && jugadoresEncima.Contains(jugador) == false)
        {
            jugadoresEncima.Add(jugador);
            ActualizarEstado();
        }
    }

    private void OnTriggerExit2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador != null)
        {
            jugadoresEncima.Remove(jugador);
            ActualizarEstado();
        }
    }

    private void ActualizarEstado()
    {
        bool nuevoEstado = jugadoresEncima.Count > 0;
        if (nuevoEstado == estaPresionado)
        {
            return; // No cambió nada (por ejemplo, se subió el segundo jugador).
        }

        estaPresionado = nuevoEstado;

        foreach (ObjetoActivable objetivo in objetivos)
        {
            if (objetivo != null)
            {
                objetivo.RecibirSenal(estaPresionado);
            }
        }

        ActualizarDibujo();
    }

    private void ActualizarDibujo()
    {
        if (dibujoTapa == null)
        {
            return;
        }

        if (estaPresionado == true)
        {
            // La tapa se hunde un poquito y se pone verde.
            dibujoTapa.transform.localPosition = new Vector3(posicionTapaSuelta.x, posicionTapaSuelta.y - 0.1f, posicionTapaSuelta.z);
            dibujoTapa.color = colorPresionado;
        }
        else
        {
            dibujoTapa.transform.localPosition = posicionTapaSuelta;
            dibujoTapa.color = colorSuelto;
        }
    }
}
