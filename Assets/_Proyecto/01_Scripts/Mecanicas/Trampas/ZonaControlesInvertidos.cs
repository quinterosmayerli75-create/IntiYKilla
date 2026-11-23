// =====================================================================
// ZonaControlesInvertidos.cs
// Proyecto: Inti y Killa
// Descripción: Una neblina violeta casi transparente. Quien entra en ella
//              tiene la izquierda y la derecha al revés hasta que sale.
//              No mata por sí sola, pero junto a un charco o un borde es
//              muy engañosa. La neblina brilla más cuando hay alguien
//              adentro, para que se entienda qué está pasando.
// Uso: objeto vacío con un hijo SpriteRenderer (la neblina).
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

public class ZonaControlesInvertidos : TrampaBase
{
    [SerializeField] private Vector2 tamanoZona = new Vector2(4f, 3f);
    [SerializeField] private SpriteRenderer neblina;
    [SerializeField] private float opacidadEnReposo = 0.2f;
    [SerializeField] private float opacidadConAlguien = 0.55f;

    // Solo se devuelven los controles a quien esta zona se los invirtió:
    // así dos zonas juntas no se pisan entre sí.
    private List<ControladorJugador> jugadoresInvertidos = new List<ControladorJugador>();

    public void Configurar(Vector2 nuevoTamano, SpriteRenderer nuevaNeblina)
    {
        tamanoZona = nuevoTamano;
        neblina = nuevaNeblina;
    }

    private void Update()
    {
        Vector2 centro = transform.position;

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null)
            {
                continue;
            }

            bool estaDentro = jugador.EstaVivo == true && JugadorTocaRectangulo(jugador, centro, tamanoZona) == true;
            bool yaLoInverti = jugadoresInvertidos.Contains(jugador);

            if (estaDentro == true && yaLoInverti == false)
            {
                jugador.InvertirControles(true);
                jugadoresInvertidos.Add(jugador);

                // Aviso claro: sin él, el jugador no entiende por qué camina al revés.
                if (GestorNivel.Instancia != null)
                {
                    GestorNivel.Instancia.MostrarAviso("¡Neblina mágica! " + jugador.NombreJugador + " tiene izquierda y derecha al revés mientras esté adentro.", 2f);
                }
            }
            else if (estaDentro == false && yaLoInverti == true)
            {
                jugador.InvertirControles(false);
                jugadoresInvertidos.Remove(jugador);
            }
        }

        ActualizarNeblina();
    }

    private void ActualizarNeblina()
    {
        if (neblina == null)
        {
            return;
        }

        float opacidad = opacidadEnReposo;
        if (jugadoresInvertidos.Count > 0)
        {
            opacidad = opacidadConAlguien;
        }

        Color colorActual = neblina.color;
        neblina.color = new Color(colorActual.r, colorActual.g, colorActual.b, opacidad);
    }

    // Si la zona desaparece (por ejemplo al reiniciar), nadie queda al revés.
    private void OnDisable()
    {
        foreach (ControladorJugador jugador in jugadoresInvertidos)
        {
            if (jugador != null)
            {
                jugador.InvertirControles(false);
            }
        }
        jugadoresInvertidos.Clear();
    }
}
