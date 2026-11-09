// =====================================================================
// GeneradorEscenasDificultad.cs
// Proyecto: Inti y Killa
// Descripción: Ayudantes de la versión difícil de los niveles. Cada nivel
//              agrega sus trampas extra en un bloque "VERSIÓN DIFÍCIL" al
//              final de su generador; aquí están las piezas que repiten.
//              Es otra parte de GeneradorEscenas.
// =====================================================================

using UnityEngine;

public static partial class GeneradorEscenas
{
    // Une las dos puertas de salida del nivel que se está armando: se cambian
    // de lugar la primera vez que alguien se acerca.
    private static void CambiarPuertasDeLugar(Transform trampas)
    {
        GameObject puertaFuego = GameObject.Find("Puerta fuego");
        GameObject puertaAgua = GameObject.Find("Puerta agua");
        if (puertaFuego == null || puertaAgua == null)
        {
            Debug.LogWarning("Inti y Killa: no se encontraron las dos puertas para cambiarlas de lugar.");
            return;
        }

        HerramientasEscena.CrearPuertasQueSeCambian("Puertas que se cambian", puertaFuego, puertaAgua, trampas);
    }
}
