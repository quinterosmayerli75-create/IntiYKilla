// =====================================================================
// ColoresMundos.cs
// Proyecto: Inti y Killa
// Descripción: Color que identifica a cada mundo en los menús. Está en un
//              solo lugar para que la tarjeta del mundo y los cuadros de sus
//              niveles siempre combinen.
// =====================================================================

using UnityEngine;

public static class ColoresMundos
{
    // Índice 0 = Mundo 1, índice 5 = Mundo 6.
    private static readonly Color[] colores = new Color[]
    {
        new Color(0.30f, 0.56f, 0.84f),
        new Color(0.72f, 0.30f, 0.20f),
        new Color(0.22f, 0.58f, 0.36f),
        new Color(0.08f, 0.56f, 0.60f),
        new Color(0.85f, 0.50f, 0.18f),
        new Color(0.36f, 0.30f, 0.76f)
    };

    public static Color ObtenerColor(int numeroMundo)
    {
        int indice = Mathf.Clamp(numeroMundo - 1, 0, colores.Length - 1);
        return colores[indice];
    }
}
