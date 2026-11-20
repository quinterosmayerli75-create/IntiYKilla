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
    // Convierte el nivel en una "sala que se repite" de 3 vueltas:
    // - Vuelta 1: la sala normal.
    // - Vuelta 2: la misma sala, pero las puertas se cambian de lugar al llegar.
    // - Vuelta 3: las puertas quedan quietas (el jugador ya espera que se cambien)
    //   y aparece una neblina que invierte los controles cerca de la salida.
    private static void ConvertirEnSalaQueSeRepite(Transform trampas, Vector2 centroNeblina, Vector2 tamanoNeblina)
    {
        ConvertirEnSalaQueSeRepite(trampas, centroNeblina, tamanoNeblina, true);
    }

    // cambiarPuertas = false cuando cruzar hasta la otra puerta obligaría a pasar
    // por un peligro imposible de esquivar: la vuelta 2 queda igual a la primera
    // (lo que también confunde: "¿y ahora qué cambió?").
    private static void ConvertirEnSalaQueSeRepite(Transform trampas, Vector2 centroNeblina, Vector2 tamanoNeblina, bool cambiarPuertas)
    {
        string[] mensajes = new string[] { "", "¿Otra vez? Parece la misma sala...", "¿Y otra vez? Ya no confíes en nada." };
        HerramientasEscena.CrearSalaQueSeRepite(3, mensajes);

        if (cambiarPuertas == false)
        {
            ZonaControlesInvertidos soloNeblina = HerramientasEscena.CrearZonaControlesInvertidos("Neblina de la tercera vuelta", centroNeblina, tamanoNeblina, trampas);
            HerramientasEscena.AparecerSoloEnVueltas(soloNeblina.gameObject, new int[] { 2 });
            return;
        }

        CambiarPuertasDeLugar(trampas);
        Transform cambioDePuertas = trampas.Find("Puertas que se cambian");
        if (cambioDePuertas != null)
        {
            HerramientasEscena.AparecerSoloEnVueltas(cambioDePuertas.gameObject, new int[] { 1 });
        }

        ZonaControlesInvertidos neblina = HerramientasEscena.CrearZonaControlesInvertidos("Neblina de la tercera vuelta", centroNeblina, tamanoNeblina, trampas);
        HerramientasEscena.AparecerSoloEnVueltas(neblina.gameObject, new int[] { 2 });
    }

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
