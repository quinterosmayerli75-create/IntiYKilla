// =====================================================================
// PuasDeGoma.cs
// Proyecto: Inti y Killa
// Descripción: Púas que parecen mortales y hacen perder tiempo buscando
//              cómo rodearlas. Si alguien las toca, resultan ser de goma:
//              lo lanzan muy alto, hacia un atajo o una gema escondida.
//              La pista: son de un color apenas más claro y se mueven un
//              poquito, como si fueran blandas.
// Uso: objeto vacío a la altura del piso; el generador dibuja las púas.
// =====================================================================

using UnityEngine;

public class PuasDeGoma : TrampaBase
{
    [SerializeField] private float ancho = 1.5f;
    [SerializeField] private float fuerzaDeRebote = 22f;

    private bool yaSeDescubrio = false;

    public void Configurar(float nuevoAncho, float nuevaFuerza)
    {
        ancho = nuevoAncho;
        fuerzaDeRebote = nuevaFuerza;
    }

    private void Update()
    {
        // Se mueven apenas, como gelatina: la pista de que son blandas.
        float latido = 1f + Mathf.Sin(Time.time * 6f) * 0.06f;
        transform.localScale = new Vector3(1f, latido, 1f);

        Vector2 centro = new Vector2(transform.position.x, transform.position.y + 0.3f);
        Vector2 tamano = new Vector2(ancho, 0.6f);
        foreach (ControladorJugador jugador in jugadores)
        {
            // Solo rebota a quien cae sobre ellas (no a quien sube atravesándolas).
            if (jugador.EstaVivo == true && jugador.VelocidadVertical <= 0.5f && JugadorTocaRectangulo(jugador, centro, tamano) == true)
            {
                jugador.Rebotar(fuerzaDeRebote);
                if (yaSeDescubrio == false && GestorNivel.Instancia != null)
                {
                    yaSeDescubrio = true;
                    GestorNivel.Instancia.MostrarAviso("¡Las púas eran de goma!", 2f);
                }
            }
        }
    }
}
