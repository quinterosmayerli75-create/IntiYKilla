// =====================================================================
// MensajesDeMuerte.cs
// Proyecto: Inti y Killa
// Descripción: Los mensajes graciosos que aparecen al morir, según la causa.
//              Están juntos para poder cambiarlos o traducirlos fácilmente.
//              Las trampas tienen su propio mensaje en el Inspector.
// =====================================================================

using UnityEngine;

public static class MensajesDeMuerte
{
    private static readonly string[] mensajesLava = new string[]
    {
        " creyó que la lava era jugo de naranja.",
        " tocó la lava. Estaba caliente, sí.",
        "Spoiler: la lava quema a {0}."
    };

    private static readonly string[] mensajesAgua = new string[]
    {
        " se apagó en el agua. Pssss...",
        " olvidó que el Sol no sabe nadar.",
        "A {0} no le tocaba bañarse hoy."
    };

    private static readonly string[] mensajesVeneno = new string[]
    {
        " probó el veneno. No lo recomienda.",
        "El veneno verde no era limonada, {0}.",
        " confundió el veneno con refresco."
    };

    private static readonly string[] mensajesCristales = new string[]
    {
        " abrazó los cristales de sal. Mala idea.",
        "Esos cristales no eran decoración, {0}."
    };

    private static readonly string[] mensajesVacio = new string[]
    {
        " buscó el suelo. No lo encontró.",
        " se fue de viaje al vacío."
    };

    public static string DeZona(TipoZona zona, string nombreJugador)
    {
        if (zona == TipoZona.Lava)
        {
            return Elegir(mensajesLava, nombreJugador);
        }

        if (zona == TipoZona.Agua)
        {
            return Elegir(mensajesAgua, nombreJugador);
        }

        if (zona == TipoZona.Veneno)
        {
            return Elegir(mensajesVeneno, nombreJugador);
        }

        return Elegir(mensajesCristales, nombreJugador);
    }

    public static string DeVacio(string nombreJugador)
    {
        return Elegir(mensajesVacio, nombreJugador);
    }

    public static string DeTraicion(string nombreTraidor, string nombreVictima)
    {
        return "¡" + nombreTraidor + " traicionó a " + nombreVictima + "!";
    }

    // Los mensajes con {0} llevan el nombre adentro; los demás empiezan después del nombre.
    private static string Elegir(string[] opciones, string nombreJugador)
    {
        string mensaje = opciones[Random.Range(0, opciones.Length)];

        if (mensaje.Contains("{0}") == true)
        {
            return mensaje.Replace("{0}", nombreJugador);
        }

        return nombreJugador + mensaje;
    }
}
