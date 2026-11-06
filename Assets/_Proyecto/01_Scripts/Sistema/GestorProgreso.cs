// =====================================================================
// GestorProgreso.cs
// Proyecto: Inti y Killa
// Descripción: Guarda el avance del jugador con PlayerPrefs (una pequeña
//              "libreta" que Unity guarda en la computadora o navegador):
//              - Hasta qué mundo está desbloqueado.
//              - Hasta qué nivel está desbloqueado en cada mundo.
//              - Cuántas estrellas se ganaron en cada nivel (la mejor marca).
//              - El récord de menos muertes de cada nivel.
// Reglas:     - Al inicio solo está abierto el Mundo 1, Nivel 1.
//              - Ganar un nivel desbloquea el siguiente de su mundo.
//              - Un mundo se abre SOLO al ganar el nivel 5 del anterior.
// =====================================================================

using UnityEngine;

public static class GestorProgreso
{
    // MODO PRUEBA: con true, todos los mundos y niveles están abiertos para poder
    // probarlos. ANTES DE PUBLICAR hay que ponerlo en false (está en el CHECKLIST).
    public const bool modoPruebaTodoAbierto = true;

    private const string claveMundosDesbloqueados = "IntiYKilla_MundosDesbloqueados";
    private const string prefijoNivelesDesbloqueados = "IntiYKilla_NivelesDesbloqueados_Mundo";
    private const string prefijoEstrellas = "IntiYKilla_Estrellas_";
    private const string prefijoRecordMuertes = "IntiYKilla_RecordMuertes_";

    // Mundo que se está viendo en la pantalla SeleccionNivel. No se guarda en
    // PlayerPrefs porque solo sirve para pasar el dato de una escena a otra.
    private static int mundoElegido = 1;

    public static int MundoElegido
    {
        get { return mundoElegido; }
        set { mundoElegido = value; }
    }

    // ================= Mundos =================

    public static int ObtenerMundosDesbloqueados()
    {
        // Si nunca se guardó nada, solo está abierto el Mundo 1.
        return PlayerPrefs.GetInt(claveMundosDesbloqueados, 1);
    }

    public static bool MundoEstaDesbloqueado(int numeroMundo)
    {
        if (modoPruebaTodoAbierto == true)
        {
            return true;
        }

        return numeroMundo <= ObtenerMundosDesbloqueados();
    }

    // ================= Niveles =================

    private static string ClaveNivelesDesbloqueados(int numeroMundo)
    {
        return prefijoNivelesDesbloqueados + numeroMundo;
    }

    private static string ClaveEstrellas(int numeroMundo, int numeroEnMundo)
    {
        return prefijoEstrellas + "Mundo" + numeroMundo + "_Nivel" + numeroEnMundo;
    }

    public static int ObtenerNivelesDesbloqueados(int numeroMundo)
    {
        // En un mundo recién abierto, el nivel 1 ya se puede jugar.
        return PlayerPrefs.GetInt(ClaveNivelesDesbloqueados(numeroMundo), 1);
    }

    // Un nivel solo está abierto si también lo está su mundo.
    public static bool NivelEstaDesbloqueado(int numeroMundo, int numeroEnMundo)
    {
        if (modoPruebaTodoAbierto == true)
        {
            return true;
        }

        if (MundoEstaDesbloqueado(numeroMundo) == false)
        {
            return false;
        }

        return numeroEnMundo <= ObtenerNivelesDesbloqueados(numeroMundo);
    }

    // ================= Estrellas =================

    public static int ObtenerEstrellas(int numeroMundo, int numeroEnMundo)
    {
        return PlayerPrefs.GetInt(ClaveEstrellas(numeroMundo, numeroEnMundo), 0);
    }

    // Suma las estrellas de los 5 niveles (lo que muestra el botón del mundo).
    public static int ObtenerEstrellasDelMundo(int numeroMundo)
    {
        int total = 0;

        for (int numeroEnMundo = 1; numeroEnMundo <= DatosJuego.nivelesPorMundo; numeroEnMundo++)
        {
            total = total + ObtenerEstrellas(numeroMundo, numeroEnMundo);
        }

        return total;
    }

    public static int ObtenerEstrellasMaximasPorMundo()
    {
        return DatosJuego.nivelesPorMundo * DatosJuego.estrellasPorNivel;
    }

    // ================= Récord de muertes =================

    private static string ClaveRecordMuertes(int numeroMundo, int numeroEnMundo)
    {
        return prefijoRecordMuertes + "Mundo" + numeroMundo + "_Nivel" + numeroEnMundo;
    }

    // Menos muertes con las que se terminó el nivel, o -1 si nunca se terminó.
    public static int ObtenerRecordMuertes(int numeroMundo, int numeroEnMundo)
    {
        return PlayerPrefs.GetInt(ClaveRecordMuertes(numeroMundo, numeroEnMundo), -1);
    }

    public static void GuardarRecordMuertes(int numeroMundo, int numeroEnMundo, int muertes)
    {
        int recordAnterior = ObtenerRecordMuertes(numeroMundo, numeroEnMundo);
        if (recordAnterior < 0 || muertes < recordAnterior)
        {
            PlayerPrefs.SetInt(ClaveRecordMuertes(numeroMundo, numeroEnMundo), muertes);
            PlayerPrefs.Save();
        }
    }

    // ================= Guardar =================

    // Se llama al ganar un nivel.
    public static void GuardarResultado(int numeroMundo, int numeroEnMundo, int estrellasGanadas)
    {
        // Solo se guarda si es mejor que la marca anterior.
        int estrellasAnteriores = ObtenerEstrellas(numeroMundo, numeroEnMundo);
        if (estrellasGanadas > estrellasAnteriores)
        {
            PlayerPrefs.SetInt(ClaveEstrellas(numeroMundo, numeroEnMundo), estrellasGanadas);
        }

        if (numeroEnMundo < DatosJuego.nivelesPorMundo)
        {
            // Dentro del mundo: se abre el siguiente nivel.
            int siguienteNivel = numeroEnMundo + 1;
            if (siguienteNivel > ObtenerNivelesDesbloqueados(numeroMundo))
            {
                PlayerPrefs.SetInt(ClaveNivelesDesbloqueados(numeroMundo), siguienteNivel);
            }
        }
        else
        {
            // Último nivel del mundo: se abre el mundo siguiente, si existe.
            int siguienteMundo = numeroMundo + 1;
            bool existeSiguienteMundo = siguienteMundo <= DatosJuego.CantidadDeMundos();
            if (existeSiguienteMundo == true && siguienteMundo > ObtenerMundosDesbloqueados())
            {
                PlayerPrefs.SetInt(claveMundosDesbloqueados, siguienteMundo);
            }
        }

        PlayerPrefs.Save();
    }

    // Útil para la defensa o para probar: abre todos los mundos y niveles.
    public static void DesbloquearTodo()
    {
        PlayerPrefs.SetInt(claveMundosDesbloqueados, DatosJuego.CantidadDeMundos());

        for (int numeroMundo = 1; numeroMundo <= DatosJuego.CantidadDeMundos(); numeroMundo++)
        {
            PlayerPrefs.SetInt(ClaveNivelesDesbloqueados(numeroMundo), DatosJuego.nivelesPorMundo);
        }

        PlayerPrefs.Save();
    }

    public static void BorrarProgreso()
    {
        PlayerPrefs.DeleteKey(claveMundosDesbloqueados);

        for (int numeroMundo = 1; numeroMundo <= DatosJuego.CantidadDeMundos(); numeroMundo++)
        {
            PlayerPrefs.DeleteKey(ClaveNivelesDesbloqueados(numeroMundo));

            for (int numeroEnMundo = 1; numeroEnMundo <= DatosJuego.nivelesPorMundo; numeroEnMundo++)
            {
                PlayerPrefs.DeleteKey(ClaveEstrellas(numeroMundo, numeroEnMundo));
                PlayerPrefs.DeleteKey(ClaveRecordMuertes(numeroMundo, numeroEnMundo));
            }
        }

        PlayerPrefs.Save();
    }
}
