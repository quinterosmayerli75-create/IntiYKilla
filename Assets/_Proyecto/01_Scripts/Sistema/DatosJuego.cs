// =====================================================================
// DatosJuego.cs
// Proyecto: Inti y Killa
// Descripción: "Catálogo" del juego. Guarda en un solo lugar los 6 mundos
//              y sus 5 niveles (30 en total): nombre de la escena, título,
//              mundo, posición dentro del mundo y las muertes permitidas para
//              ganar la 3ra estrella (más altas en los mundos más tramposos).
//              Así, si un nivel cambia de nombre, se corrige solo aquí.
// =====================================================================

using System.Collections.Generic;

// Datos de un solo nivel.
public class DatosNivel
{
    public int numeroMundo;
    public int numeroEnMundo;
    public string nombreEscena;
    public string titulo;
    public int muertesPermitidas;

    public DatosNivel(int nuevoMundo, int nuevoNumeroEnMundo, string nuevoTitulo, int nuevasMuertesPermitidas)
    {
        numeroMundo = nuevoMundo;
        numeroEnMundo = nuevoNumeroEnMundo;
        titulo = nuevoTitulo;
        muertesPermitidas = nuevasMuertesPermitidas;

        // Mundo 1, nivel 2 -> "Mundo1_Nivel2"
        nombreEscena = "Mundo" + nuevoMundo + "_Nivel" + nuevoNumeroEnMundo;
    }
}

public static class DatosJuego
{
    public const string escenaMenuPrincipal = "MenuPrincipal";
    public const string escenaSeleccionMundo = "SeleccionMundo";
    public const string escenaSeleccionNivel = "SeleccionNivel";

    public const int nivelesPorMundo = 5;
    public const int estrellasPorNivel = 3;

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

    public static int CantidadDeMundos()
    {
        return nombresMundos.Length;
    }

    // Devuelve el nivel pedido (los dos números empiezan en 1) o null si no existe.
    public static DatosNivel ObtenerNivel(int numeroMundo, int numeroEnMundo)
    {
        List<DatosNivel> niveles = ObtenerTodosLosNiveles();

        foreach (DatosNivel nivel in niveles)
        {
            if (nivel.numeroMundo == numeroMundo && nivel.numeroEnMundo == numeroEnMundo)
            {
                return nivel;
            }
        }

        return null;
    }

    // Convierte el nombre de la escena abierta en sus datos (null si no es un nivel).
    public static DatosNivel ObtenerNivelPorEscena(string nombreEscena)
    {
        List<DatosNivel> niveles = ObtenerTodosLosNiveles();

        foreach (DatosNivel nivel in niveles)
        {
            if (nivel.nombreEscena == nombreEscena)
            {
                return nivel;
            }
        }

        return null;
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

    private static void AgregarNivel(int numeroMundo, int numeroEnMundo, string titulo, int muertesPermitidas)
    {
        listaNiveles.Add(new DatosNivel(numeroMundo, numeroEnMundo, titulo, muertesPermitidas));
    }

    private static void CrearListaDeNiveles()
    {
        listaNiveles = new List<DatosNivel>();

        // Mundo 1: Salar de Uyuni (tutorial)
        AgregarNivel(1, 1, "Primeros pasos", 3);
        AgregarNivel(1, 2, "Botones y compuertas", 3);
        AgregarNivel(1, 3, "El primer eclipse", 4);
        AgregarNivel(1, 4, "Espejo de sal", 5);
        AgregarNivel(1, 5, "Travesía del salar", 6);

        // Mundo 2: Minas de Potosí
        AgregarNivel(2, 1, "La luz de Inti", 6);
        AgregarNivel(2, 2, "Vagonetas", 7);
        AgregarNivel(2, 3, "Cajas pesadas", 8);
        AgregarNivel(2, 4, "Galerías oscuras", 9);
        AgregarNivel(2, 5, "El corazón del Cerro Rico", 10);

        // Mundo 3: Yungas
        AgregarNivel(3, 1, "Camino en la niebla", 8);
        AgregarNivel(3, 2, "Lluvia en la montaña", 9);
        AgregarNivel(3, 3, "Día y noche", 10);
        AgregarNivel(3, 4, "Puente entre nubes", 11);
        AgregarNivel(3, 5, "El camino de la muerte", 12);

        // Mundo 4: Trópico
        AgregarNivel(4, 1, "Ríos y lianas", 10);
        AgregarNivel(4, 2, "Hojas trampolín", 11);
        AgregarNivel(4, 3, "Eclipse en la selva", 12);
        AgregarNivel(4, 4, "Persecución en el río", 13);
        AgregarNivel(4, 5, "El corazón del Chapare", 14);

        // Mundo 5: Aiquile, tierra del charango
        AgregarNivel(5, 1, "Cuerdas de charango", 12);
        AgregarNivel(5, 2, "La puerta melódica", 13);
        AgregarNivel(5, 3, "Melodía a dúo", 14);
        AgregarNivel(5, 4, "Serenata nocturna", 15);
        AgregarNivel(5, 5, "El gran concierto", 16);

        // Mundo 6: Isla del Sol
        AgregarNivel(6, 1, "Eclipse eterno", 15);
        AgregarNivel(6, 2, "Las escalinatas de Yumani", 16);
        AgregarNivel(6, 3, "El templo del Sol", 18);
        AgregarNivel(6, 4, "Todo a la vez", 20);
        AgregarNivel(6, 5, "El gran final", 25);
    }
}
