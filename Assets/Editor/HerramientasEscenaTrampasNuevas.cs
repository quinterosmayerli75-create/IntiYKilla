// =====================================================================
// HerramientasEscenaTrampasNuevas.cs
// Proyecto: Inti y Killa
// Descripción: Piezas de escena de las trampas nuevas (versión difícil del
//              juego): piso que se calienta, puertas que se cambian, gema
//              que huye, plataforma que se voltea, neblina que invierte los
//              controles y flecha que señala el camino equivocado.
//              Es otra parte de HerramientasEscena.
// =====================================================================

using UnityEngine;

public static partial class HerramientasEscena
{
    private static readonly Color colorNeblinaInvertida = new Color(0.62f, 0.30f, 0.92f, 0.12f);

    // alturaPiso: el piso que se calienta. Por arriba no se nota nada hasta pisarlo.
    public static PisoQueSeCalienta CrearPisoQueSeCalienta(string nombre, TipoZona zona, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(centroX, alturaPiso, 0f);

        Color colorZona = colorLava;
        string mensaje = "El piso se volvió lava bajo los pies de {0}.";
        if (zona == TipoZona.Agua)
        {
            colorZona = colorAgua;
            mensaje = "El piso se inundó y {0} se apagó.";
        }
        else if (zona == TipoZona.Veneno)
        {
            colorZona = colorVeneno;
            mensaje = "El piso se llenó de veneno justo cuando pasaba {0}.";
        }

        // Empieza transparente: el componente la enciende al activarse.
        Color colorInvisible = new Color(colorZona.r, colorZona.g, colorZona.b, 0f);
        GameObject capa = CrearDibujoHijo("Capa", objeto.transform, new Vector2(0f, 0.15f), new Vector2(ancho, 0.3f), colorInvisible, 2);

        PisoQueSeCalienta trampa = objeto.AddComponent<PisoQueSeCalienta>();
        trampa.Configurar(zona, ancho, capa.GetComponent<SpriteRenderer>(), colorZona);
        trampa.ConfigurarMensaje(mensaje);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Une dos puertas de salida que ya existen en la escena.
    public static PuertasQueSeCambian CrearPuertasQueSeCambian(string nombre, GameObject puertaUno, GameObject puertaDos, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);

        PuertasQueSeCambian trampa = objeto.AddComponent<PuertasQueSeCambian>();
        trampa.Configurar(puertaUno.transform, puertaDos.transform);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // destino: el lugar adonde huye; debe poder alcanzarse (regla de justicia).
    public static GemaQueHuye CrearGemaQueHuye(string nombre, TipoElemento elemento, Vector2 posicion, Vector2 destino, Transform padre)
    {
        GameObject gema = CrearGema(nombre, elemento, posicion, padre);
        GemaQueHuye trampa = gema.AddComponent<GemaQueHuye>();
        trampa.Configurar(destino);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static PlataformaQueSeVoltea CrearPlataformaQueSeVoltea(string nombre, Vector2 posicion, Vector2 tamano, Color colorRespaldo, Transform padre)
    {
        GameObject plataforma = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);
        PlataformaQueSeVoltea trampa = plataforma.AddComponent<PlataformaQueSeVoltea>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // centro y tamano: el rectángulo de neblina donde los controles van al revés.
    public static ZonaControlesInvertidos CrearZonaControlesInvertidos(string nombre, Vector2 centro, Vector2 tamano, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(centro.x, centro.y, 0f);

        GameObject neblina = CrearDibujoHijo("Neblina", objeto.transform, Vector2.zero, tamano, colorNeblinaInvertida, 6);

        ZonaControlesInvertidos trampa = objeto.AddComponent<ZonaControlesInvertidos>();
        trampa.Configurar(tamano, neblina.GetComponent<SpriteRenderer>());
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Igual que la flecha guía, pero puesta a propósito hacia el lado equivocado.
    public static void CrearFlechaEnganosa(string nombre, Vector2 posicion, bool haciaDerecha, Transform padre)
    {
        CrearFlechaGuia(nombre, posicion, haciaDerecha, padre);
    }
}
