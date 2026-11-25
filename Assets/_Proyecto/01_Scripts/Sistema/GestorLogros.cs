// =====================================================================
// GestorLogros.cs
// Proyecto: Inti y Killa
// Descripción: Logros del juego. Premian pasar los niveles con las 3 vidas
//              intactas (sin morir ni una vez en ese intento):
//              - "Sin un rasguño": el primer nivel perfecto.
//              - Un logro por cada mundo con sus 5 niveles perfectos.
//              - "Leyenda del Sol y la Luna": los 30 niveles perfectos.
//              Se guarda con PlayerPrefs, igual que el resto del progreso.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

public static class GestorLogros
{
    private const string prefijoNivelPerfecto = "IntiYKilla_Perfecto_";
    private const string prefijoLogro = "IntiYKilla_Logro_";

    public const string logroSinRasguno = "SinRasguno";
    public const string logroLeyenda = "Leyenda";
    private const string prefijoLogroMundo = "MundoPerfecto";

    // Un logro para mostrar en pantalla: su clave, su nombre y cómo se gana.
    public class DatosLogro
    {
        public string clave;
        public string nombre;
        public string descripcion;

        public DatosLogro(string nuevaClave, string nuevoNombre, string nuevaDescripcion)
        {
            clave = nuevaClave;
            nombre = nuevoNombre;
            descripcion = nuevaDescripcion;
        }
    }

    // La lista completa, en el orden en que se muestran.
    public static List<DatosLogro> ObtenerTodos()
    {
        List<DatosLogro> lista = new List<DatosLogro>();
        lista.Add(new DatosLogro(logroSinRasguno, "Sin un rasguño", "Pasa un nivel con las 3 vidas intactas."));
        for (int numeroMundo = 1; numeroMundo <= DatosJuego.CantidadDeMundos(); numeroMundo++)
        {
            string nombreMundo = DatosJuego.ObtenerNombreMundo(numeroMundo);
            lista.Add(new DatosLogro(ClaveLogroMundo(numeroMundo), nombreMundo + " perfecto", "Pasa los 5 niveles de " + nombreMundo + " con las 3 vidas intactas."));
        }
        lista.Add(new DatosLogro(logroLeyenda, "Leyenda del Sol y la Luna", "Pasa los 30 niveles con las 3 vidas intactas."));
        return lista;
    }

    private static string ClaveLogroMundo(int numeroMundo)
    {
        return prefijoLogroMundo + numeroMundo;
    }

    private static string ClaveNivelPerfecto(int numeroMundo, int numeroEnMundo)
    {
        return prefijoNivelPerfecto + numeroMundo + "_" + numeroEnMundo;
    }

    public static bool NivelEsPerfecto(int numeroMundo, int numeroEnMundo)
    {
        return PlayerPrefs.GetInt(ClaveNivelPerfecto(numeroMundo, numeroEnMundo), 0) == 1;
    }

    public static bool LogroDesbloqueado(string clave)
    {
        return PlayerPrefs.GetInt(prefijoLogro + clave, 0) == 1;
    }

    public static int CantidadDesbloqueados()
    {
        int cantidad = 0;
        foreach (DatosLogro logro in ObtenerTodos())
        {
            if (LogroDesbloqueado(logro.clave) == true)
            {
                cantidad = cantidad + 1;
            }
        }
        return cantidad;
    }

    // Se llama al ganar un nivel. Devuelve los nombres de los logros que se
    // acaban de ganar (lista vacía si no hubo ninguno nuevo).
    public static List<string> RegistrarVictoria(int numeroMundo, int numeroEnMundo, bool vidasIntactas)
    {
        List<string> nuevos = new List<string>();
        if (vidasIntactas == false)
        {
            return nuevos;
        }

        PlayerPrefs.SetInt(ClaveNivelPerfecto(numeroMundo, numeroEnMundo), 1);

        DesbloquearSiFalta(logroSinRasguno, nuevos);

        if (MundoEsPerfecto(numeroMundo) == true)
        {
            DesbloquearSiFalta(ClaveLogroMundo(numeroMundo), nuevos);
        }

        bool todosPerfectos = true;
        for (int mundo = 1; mundo <= DatosJuego.CantidadDeMundos(); mundo++)
        {
            if (MundoEsPerfecto(mundo) == false)
            {
                todosPerfectos = false;
            }
        }
        if (todosPerfectos == true)
        {
            DesbloquearSiFalta(logroLeyenda, nuevos);
        }

        PlayerPrefs.Save();
        return nuevos;
    }

    private static bool MundoEsPerfecto(int numeroMundo)
    {
        for (int numeroEnMundo = 1; numeroEnMundo <= DatosJuego.nivelesPorMundo; numeroEnMundo++)
        {
            if (NivelEsPerfecto(numeroMundo, numeroEnMundo) == false)
            {
                return false;
            }
        }
        return true;
    }

    // Solo cuenta como "nuevo" la primera vez: así el aviso no se repite.
    private static void DesbloquearSiFalta(string clave, List<string> nuevos)
    {
        if (LogroDesbloqueado(clave) == true)
        {
            return;
        }
        PlayerPrefs.SetInt(prefijoLogro + clave, 1);
        foreach (DatosLogro logro in ObtenerTodos())
        {
            if (logro.clave == clave)
            {
                nuevos.Add(logro.nombre);
            }
        }
    }

    public static void BorrarLogros()
    {
        for (int numeroMundo = 1; numeroMundo <= DatosJuego.CantidadDeMundos(); numeroMundo++)
        {
            for (int numeroEnMundo = 1; numeroEnMundo <= DatosJuego.nivelesPorMundo; numeroEnMundo++)
            {
                PlayerPrefs.DeleteKey(ClaveNivelPerfecto(numeroMundo, numeroEnMundo));
            }
        }
        foreach (DatosLogro logro in ObtenerTodos())
        {
            PlayerPrefs.DeleteKey(prefijoLogro + logro.clave);
        }
        PlayerPrefs.Save();
    }
}
