// =====================================================================
// TipoElemento.cs
// Proyecto: Inti y Killa
// Descripción: Lista de los elementos que puede tener un personaje y
//              el color que representa a cada uno.
//              Inti nace con Fuego y Killa con Agua. El altar de eclipse
//              puede intercambiarlos.
// =====================================================================

using UnityEngine;

public enum TipoElemento
{
    Fuego, // puede pisar lava, muere con el agua
    Agua   // puede pisar agua, muere con la lava
}

// Colores oficiales de cada elemento (personajes, gemas y puertas).
public static class ColoresElemento
{
    public static readonly Color colorFuego = new Color(1.00f, 0.60f, 0.10f); // naranja sol
    public static readonly Color colorAgua = new Color(0.45f, 0.15f, 0.80f);  // morado luna

    public static Color ObtenerColor(TipoElemento elemento)
    {
        if (elemento == TipoElemento.Fuego)
        {
            return colorFuego;
        }

        return colorAgua;
    }
}
