// =====================================================================
// GestorProgreso.cs
// Proyecto: Inti y Killa
// Descripción: Guarda el avance del jugador con PlayerPrefs (una pequeña
//              "libreta" que Unity guarda en la computadora o navegador):
//              - Hasta qué nivel está desbloqueado.
//              - Cuántas estrellas se ganaron en cada nivel (la mejor marca).
// =====================================================================

using UnityEngine;

public static class GestorProgreso
{
    private const string claveNivelDesbloqueado = "IntiYKilla_NivelDesbloqueado";
    private const string prefijoEstrellas = "IntiYKilla_Estrellas_";

    public static int ObtenerNivelDesbloqueado()
    {
        // Si nunca se guardó nada, el nivel 1 está desbloqueado.
        return PlayerPrefs.GetInt(claveNivelDesbloqueado, 1);
    }

    public static bool EstaDesbloqueado(int numeroNivel)
    {
        return numeroNivel <= ObtenerNivelDesbloqueado();
    }

    public static int ObtenerEstrellas(int numeroNivel)
    {
        return PlayerPrefs.GetInt(prefijoEstrellas + numeroNivel, 0);
    }

    // Se llama al ganar un nivel.
    public static void GuardarResultado(int numeroNivel, int estrellasGanadas)
    {
        if (numeroNivel <= 0)
        {
            return; // No es un nivel del catálogo (por ejemplo, una escena de prueba).
        }

        // Solo se guarda si es mejor que la marca anterior.
        int estrellasAnteriores = ObtenerEstrellas(numeroNivel);
        if (estrellasGanadas > estrellasAnteriores)
        {
            PlayerPrefs.SetInt(prefijoEstrellas + numeroNivel, estrellasGanadas);
        }

        // Ganar un nivel desbloquea el siguiente.
        int siguienteNivel = numeroNivel + 1;
        if (siguienteNivel > ObtenerNivelDesbloqueado())
        {
            PlayerPrefs.SetInt(claveNivelDesbloqueado, siguienteNivel);
        }

        PlayerPrefs.Save();
    }

    // Útil para la defensa o para probar: abre todos los niveles.
    public static void DesbloquearTodo()
    {
        PlayerPrefs.SetInt(claveNivelDesbloqueado, DatosJuego.CantidadDeNiveles());
        PlayerPrefs.Save();
    }

    public static void BorrarProgreso()
    {
        PlayerPrefs.DeleteKey(claveNivelDesbloqueado);

        for (int numero = 1; numero <= DatosJuego.CantidadDeNiveles(); numero++)
        {
            PlayerPrefs.DeleteKey(prefijoEstrellas + numero);
        }

        PlayerPrefs.Save();
    }
}
