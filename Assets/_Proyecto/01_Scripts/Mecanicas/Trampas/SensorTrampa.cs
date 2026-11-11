// =====================================================================
// SensorTrampa.cs
// Proyecto: Inti y Killa
// Descripción: Sensor invisible que dispara una o varias trampas cuando se
//              cumple una condición dentro de su rectángulo:
//              - AlEntrar: alguien entra a la zona.
//              - AlSaltar: alguien salta estando dentro de la zona.
//              - LosDosDentro: los dos jugadores están a la vez en la zona.
//              Con él, cualquier trampa activable (techo, rocas, pared, bloque)
//              se puede colocar en cualquier nivel y conectarse a cualquier
//              situación, sin escribir código nuevo para cada nivel.
// Uso: objeto vacío; el generador le pasa el tamaño y la lista de trampas.
// =====================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CondicionSensor
{
    AlEntrar,
    AlSaltar,
    LosDosDentro
}

public class SensorTrampa : TrampaBase
{
    [SerializeField] private Vector2 tamanoZona = new Vector2(2f, 2f);
    [SerializeField] private CondicionSensor condicion = CondicionSensor.AlEntrar;
    // Tiempo entre que se cumple la condición y que se disparan las trampas.
    [SerializeField] private float segundosDeRetardo = 0f;
    [SerializeField] private List<TrampaActivable> trampasQueDispara = new List<TrampaActivable>();

    // El sensor funciona una sola vez por intento: el nivel se reinicia al morir.
    private bool yaDisparo = false;

    public void Configurar(Vector2 nuevoTamano, CondicionSensor nuevaCondicion, float nuevoRetardo, List<TrampaActivable> nuevasTrampas)
    {
        tamanoZona = nuevoTamano;
        condicion = nuevaCondicion;
        segundosDeRetardo = nuevoRetardo;
        trampasQueDispara = nuevasTrampas;
    }

    private void Update()
    {
        if (yaDisparo == true)
        {
            return;
        }

        ControladorJugador causante = BuscarCausante();
        if (causante != null)
        {
            yaDisparo = true;
            StartCoroutine(DispararConRetardo(causante));
        }
    }

    // Devuelve el jugador que cumplió la condición, o null si todavía nadie.
    private ControladorJugador BuscarCausante()
    {
        Vector2 centro = transform.position;

        if (condicion == CondicionSensor.AlEntrar)
        {
            return BuscarJugadorEnRectangulo(centro, tamanoZona);
        }

        if (condicion == CondicionSensor.AlSaltar)
        {
            foreach (ControladorJugador jugador in jugadores)
            {
                // Velocidad hacia arriba y sin suelo: acaba de saltar (o rebotar).
                bool estaSubiendo = jugador.VelocidadVertical > 2f && jugador.EstaEnElSuelo() == false;
                if (jugador.EstaVivo == true && estaSubiendo == true && JugadorTocaRectangulo(jugador, centro, tamanoZona) == true)
                {
                    return jugador;
                }
            }
            return null;
        }

        // LosDosDentro: hacen falta exactamente los dos jugadores vivos en la zona.
        int jugadoresDentro = 0;
        ControladorJugador ultimoEnEntrar = null;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.EstaVivo == true && JugadorTocaRectangulo(jugador, centro, tamanoZona) == true)
            {
                jugadoresDentro = jugadoresDentro + 1;
                ultimoEnEntrar = jugador;
            }
        }

        if (jugadoresDentro >= 2)
        {
            return ultimoEnEntrar;
        }
        return null;
    }

    private IEnumerator DispararConRetardo(ControladorJugador causante)
    {
        if (segundosDeRetardo > 0f)
        {
            yield return new WaitForSeconds(segundosDeRetardo);
        }

        foreach (TrampaActivable trampa in trampasQueDispara)
        {
            if (trampa != null)
            {
                trampa.Activar(causante);
            }
        }
    }

    // Para revisar en la vista Scene dónde está la zona del sensor.
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.6f);
        Gizmos.DrawWireCube(transform.position, new Vector3(tamanoZona.x, tamanoZona.y, 0f));
    }
}
