// =====================================================================
// DatosJuego.cs
// Proyecto: Inti y Killa
// Descripción: "Catálogo" del juego. Guarda en un solo lugar la lista de
//              los 20 niveles y los 6 mundos del GDD: nombre de la escena,
//              título, mundo y tiempo objetivo para ganar la 3ra estrella.
//              Así, si un nivel cambia de nombre, se corrige solo aquí.
// =====================================================================

using System.Collections.Generic;

// Datos de un solo nivel.
public class DatosNivel
{
    public int numero;
    public string nombreEscena;
    public string titulo;
    public int numeroMundo;
    public float tiempoObjetivo;

    public DatosNivel(int nuevoNumero, string nuevoTitulo, int nuevoMundo, float nuevoTiempoObjetivo)
    {
        numero = nuevoNumero;
        titulo = nuevoTitulo;
        numeroMundo = nuevoMundo;
        tiempoObjetivo = nuevoTiempoObjetivo;

        // Nivel 1 -> "Nivel01", Nivel 12 -> "Nivel12"
        nombreEscena = "Nivel" + nuevoNumero.ToString("00");
    }
}

public static class DatosJuego
{
    public const string escenaMenuPrincipal = "MenuPrincipal";
    public const string escenaSeleccionNivel = "SeleccionNivel";

    // Índice 0 = Mundo 1, índice 5 = Mundo 6.
    public static readonly string[] nombresMundos = new string[]
    {
        "Salar de Uyuni",
        "Minas de Potosí",
        "Yungas",
        "Trópico",
        "Aiquile",
        "Isla del Sol"
    };

    private static List<DatosNivel> listaNiveles;

    public static List<DatosNivel> ObtenerTodosLosNiveles()
    {
        // La lista se crea la primera vez que alguien la pide.
        if (listaNiveles == null)
        {
            CrearListaDeNiveles();
        }

        return listaNiveles;
    }

    public static int CantidadDeNiveles()
    {
        return ObtenerTodosLosNiveles().Count;
    }

    // Devuelve el nivel pedido (empezando en 1) o null si no existe.
    public static DatosNivel ObtenerNivel(int numero)
    {
        List<DatosNivel> niveles = ObtenerTodosLosNiveles();

        foreach (DatosNivel nivel in niveles)
        {
            if (nivel.numero == numero)
            {
                return nivel;
            }
        }

        return null;
    }

    // Convierte el nombre de la escena abierta en su número de nivel (0 si no es un nivel).
    public static int ObtenerNumeroPorEscena(string nombreEscena)
    {
        List<DatosNivel> niveles = ObtenerTodosLosNiveles();

        foreach (DatosNivel nivel in niveles)
        {
            if (nivel.nombreEscena == nombreEscena)
            {
                return nivel.numero;
            }
        }

        return 0;
    }

    public static string ObtenerNombreMundo(int numeroMundo)
    {
        int indice = numeroMundo - 1;
        if (indice < 0 || indice >= nombresMundos.Length)
        {
            return "";
        }

        return nombresMundos[indice];
    }

    private static void CrearListaDeNiveles()
    {
        listaNiveles = new List<DatosNivel>();

        // Mundo 1: Salar de Uyuni (tutorial)
        listaNiveles.Add(new DatosNivel(1, "Primeros pasos", 1, 35f));
        listaNiveles.Add(new DatosNivel(2, "Botones y compuertas", 1, 45f));
        listaNiveles.Add(new DatosNivel(3, "El primer eclipse", 1, 70f));

        // Mundo 2: Minas de Potosí
        listaNiveles.Add(new DatosNivel(4, "La luz de Inti", 2, 60f));
        listaNiveles.Add(new DatosNivel(5, "Vagonetas", 2, 60f));
        listaNiveles.Add(new DatosNivel(6, "Cajas pesadas", 2, 70f));

        // Mundo 3: Yungas
        listaNiveles.Add(new DatosNivel(7, "Camino en la niebla", 3, 60f));
        listaNiveles.Add(new DatosNivel(8, "Lluvia en la montaña", 3, 70f));
        listaNiveles.Add(new DatosNivel(9, "Día y noche", 3, 75f));

        // Mundo 4: Trópico
        listaNiveles.Add(new DatosNivel(10, "Ríos y lianas", 4, 70f));
        listaNiveles.Add(new DatosNivel(11, "Hojas trampolín", 4, 70f));
        listaNiveles.Add(new DatosNivel(12, "Eclipse en la selva", 4, 80f));
        listaNiveles.Add(new DatosNivel(13, "Persecución en el río", 4, 60f));

        // Mundo 5: Aiquile, tierra del charango
        listaNiveles.Add(new DatosNivel(14, "Cuerdas de charango", 5, 70f));
        listaNiveles.Add(new DatosNivel(15, "La puerta melódica", 5, 80f));
        listaNiveles.Add(new DatosNivel(16, "Melodía a dúo", 5, 90f));
        listaNiveles.Add(new DatosNivel(17, "Serenata nocturna", 5, 90f));

        // Mundo 6: Isla del Sol
        listaNiveles.Add(new DatosNivel(18, "Eclipse eterno", 6, 90f));
        listaNiveles.Add(new DatosNivel(19, "Todo a la vez", 6, 100f));
        listaNiveles.Add(new DatosNivel(20, "El gran final", 6, 120f));
    }
}
