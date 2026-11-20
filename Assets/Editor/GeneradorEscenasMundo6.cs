// =====================================================================
// GeneradorEscenasMundo6.cs
// Proyecto: Inti y Killa
// Descripción: Los 5 niveles del Mundo 6 (Isla del Sol), con el eclipse
//              automático, y el jefe final. Es otra parte de GeneradorEscenas.
// Medidas de diseño: los charcos que obligan a esperar el eclipse miden 7,
//              más que un salto (6): no se pueden saltar, hay que cruzarlos
//              con el poder correcto.
// =====================================================================

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static partial class GeneradorEscenas
{
    private static readonly Color colorFondoIsla = new Color(0.30f, 0.30f, 0.55f);
    private const float segundosEntreEclipses = 9f;

    private static void CrearPisoIsla(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        Vector2 posicion = new Vector2(centroX, alturaPiso - 0.25f);
        HerramientasEscena.CrearPlataforma(nombre, posicion, new Vector2(ancho, 0.5f), HerramientasEscena.colorSueloIsla, padre);
    }

    private static Scene PrepararEscenaIsla(string pista, Color colorLuz, float intensidadLuz, out Camera camaraPrincipal)
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoIsla);
        HerramientasEscena.CrearLuzGlobal(colorLuz, intensidadLuz);
        HerramientasEscena.CrearGestorNivel(pista);
        HerramientasEscena.CrearFondoDeMundo(6);
        return escena;
    }

    // ================= Mundo 6, Nivel 1: Eclipse eterno =================
    // Enseña el eclipse automático: dos pisos con un charco de lava y uno de agua
    // que no se pueden saltar. Cada uno cruza el suyo, y después del eclipse,
    // el otro. Hay que esperar el momento y no quedarse en el charco.

    private static string GenerarMundo6Nivel1()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaIsla("Eclipse eterno: cada 9 segundos cambian los poderes. ¡Nunca se queden en un charco cuando llega la cuenta!", new Color(1f, 0.9f, 0.85f), 1f, out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloIsla);
        CrearPisoIsla("Piso 1", 0.75f, 27.5f, pisoUnoYungas, escenario);
        CrearPisoIsla("Piso 2", -1f, 27f, pisoDosYungas, escenario);
        CrearPisoIsla("Salida", -8.75f, 7.5f, pisoTresYungas, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava del piso 1", TipoZona.Lava, -3.5f, 7f, pisoUnoYungas, trampas);
        HerramientasEscena.CrearCharco("Agua del piso 1", TipoZona.Agua, 6.5f, 7f, pisoUnoYungas, trampas);
        HerramientasEscena.CrearCharco("Agua del piso 2", TipoZona.Agua, 6.5f, 7f, pisoDosYungas, trampas);
        HerramientasEscena.CrearCharco("Lava del piso 2", TipoZona.Lava, -3.5f, 7f, pisoDosYungas, trampas);

        // TRAMPA: una gema regalada abajo; al tocarla cae el bloque pegado al piso 1.
        TechoQueCae bloqueDeLaGema = HerramientasEscena.CrearTechoQueCae("Bloque del techo", new Vector2(10f, pisoUnoYungas - 0.75f), new Vector2(1.5f, 0.5f), suelo, false, HerramientasEscena.colorSueloIsla, escenario);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(bloqueDeLaGema);
        // TRAMPA: en la salida, una puerta de Killa de más; se escapa al acercarse.
        HerramientasEscena.CrearPuertaFalsa("Puerta sospechosa", TipoElemento.Agua, new Vector2(-6.8f, pisoTresYungas), new Vector2(0f, 9f), trampas);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearLlama("Llama", new Vector2(-5.4f, pisoTresYungas + 0.5f), 3f, 4f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Fuego, new Vector2(10f, -2.4f), trampasDeLaGema, gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-3.5f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(6.5f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-3.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(6.5f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-11.5f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-9f, pisoTresYungas), puertas);

        HerramientasEscena.CrearCicloEclipse(segundosEntreEclipses);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-14.6f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-14.6f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Neblina que invierte los controles antes de subir a la salida y un enemigo
        // en el piso 1. (Antes aparecía veneno en el centro del piso 1, pero ocupaba
        // el único lugar seguro para esperar el eclipse: se quitó por justicia.)
        // DESAFÍO: en el piso 2 caen rocas del templo; las sombras avisan.
        HerramientasEscena.CrearLluviaDeRocas("Piedras del templo", 0.5f, 2.5f, pisoDosYungas + 3f, pisoDosYungas, 2, true, trampas);
        HerramientasEscena.CrearZonaControlesInvertidos("Neblina engañosa", new Vector2(-12f, 4.2f), new Vector2(3f, 2.4f), trampas);
        HerramientasEscena.CrearEnemigoSorpresa("Enemigo del piso 1", new Vector2(12f, pisoUnoYungas), trampas);

        // PSICOLÓGICA: una gema de más con anillo gris: es un imán de piedras.
        HerramientasEscena.CrearGemaIman("Gema del templo", TipoElemento.Agua, new Vector2(-12f, pisoDosYungas + 2.4f), trampas);

        string ruta = RutaNivel(6, 1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 6, Nivel 2: Las escalinatas de Yumani =================
    // Subida vertical: cada escalón es un charco. Hay que saltar al siguiente
    // justo cuando la cuenta llega a 1, para caer con el poder ya cambiado.

    private static string GenerarMundo6Nivel2()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaIsla("Las escalinatas de Yumani: cada escalón es un charco. Salten al siguiente cuando la cuenta del eclipse llegue a 1.", new Color(1f, 0.9f, 0.85f), 1f, out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloIsla);
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Veneno izquierdo", TipoZona.Veneno, -11.75f, 7.5f, trampas);
        HerramientasEscena.CrearCharco("Veneno derecho", TipoZona.Veneno, 11.75f, 7.5f, trampas);

        // Escalones de Inti: lava, agua, lava. Los de Killa, al revés.
        float[] alturas = new float[] { pisoUnoYungas, pisoDosYungas, pisoTresYungas };
        float[] posiciones = new float[] { 10f, 6f, 10f };
        for (int indice = 0; indice < 3; indice++)
        {
            TipoZona zonaDeInti = TipoZona.Lava;
            TipoZona zonaDeKilla = TipoZona.Agua;
            if (indice == 1)
            {
                zonaDeInti = TipoZona.Agua;
                zonaDeKilla = TipoZona.Lava;
            }
            CrearPisoIsla("Escalon izquierdo " + (indice + 1), -posiciones[indice], 3f, alturas[indice], escenario);
            CrearPisoIsla("Escalon derecho " + (indice + 1), posiciones[indice], 3f, alturas[indice], escenario);
            HerramientasEscena.CrearCharco("Charco izquierdo " + (indice + 1), zonaDeInti, -posiciones[indice], 3f, alturas[indice], trampas);
            HerramientasEscena.CrearCharco("Charco derecho " + (indice + 1), zonaDeKilla, posiciones[indice], 3f, alturas[indice], trampas);
        }
        CrearPisoIsla("Cima", 0f, 10f, pisoAltoAiquile, escenario);

        // TRAMPA: una gema regalada entre los dos al empezar; al tocarla salen púas debajo.
        PuasOcultas puasDeLaGema = HerramientasEscena.CrearPuasOcultas("Piso con gema", 0f, 1.5f, suelo, false, trampas);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(puasDeLaGema);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        CrearPisoIsla("Roca de la llama", 0f, 1.5f, 11.6f, escenario);
        HerramientasEscena.CrearLlama("Llama", new Vector2(0f, 12.05f), 3f, 4f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Agua, new Vector2(0f, -2.4f), trampasDeLaGema, gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-10f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(10f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(6f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-6f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-4f, pisoAltoAiquile), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(4f, pisoAltoAiquile), puertas);

        HerramientasEscena.CrearCicloEclipse(segundosEntreEclipses);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-2f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(2f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Sobre el último escalón de cada lado, una gema que huye a la cima. Neblina
        // que invierte los controles en el escalón del medio de cada lado. Púas en
        // el centro de la cima (por donde se cruza cuando las puertas se cambian).
        HerramientasEscena.CrearGemaQueHuye("Gema fuego 3", TipoElemento.Fuego, new Vector2(-10f, 7.5f), new Vector2(-3f, 10.5f), gemas);
        HerramientasEscena.CrearGemaQueHuye("Gema agua 3", TipoElemento.Agua, new Vector2(10f, 7.5f), new Vector2(3f, 10.5f), gemas);
        HerramientasEscena.CrearZonaControlesInvertidos("Neblina engañosa izquierda", new Vector2(-6f, 4.2f), new Vector2(3f, 2.4f), trampas);
        HerramientasEscena.CrearZonaControlesInvertidos("Neblina engañosa derecha", new Vector2(6f, 4.2f), new Vector2(3f, 2.4f), trampas);
        HerramientasEscena.CrearPuasOcultas("Cima tranquila", 0f, 1.2f, pisoAltoAiquile, true, trampas);

        string ruta = RutaNivel(6, 2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 6, Nivel 3: El templo del Sol =================
    // Templo a oscuras: los cristales solo existen en la luz de quien tiene el
    // fuego, y con el eclipse esa luz pasa de uno al otro.

    private static string GenerarMundo6Nivel3()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaIsla("El templo del Sol: los cristales solo existen en la luz del fuego... y el fuego cambia de dueño con cada eclipse.", new Color(0.6f, 0.6f, 0.85f), 0.25f, out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloIsla);
        // Escalera de pisos hacia la izquierda; debajo, el pozo se cruza por los cristales.
        CrearPisoIsla("Piso 1", 12.5f, 5f, pisoUnoYungas, escenario);
        CrearPisoIsla("Piso 2", 5f, 8f, pisoDosYungas, escenario);
        CrearPisoIsla("Piso 3", -4f, 8f, pisoTresYungas, escenario);
        CrearPisoIsla("Salida", -13f, 5f, pisoAltoAiquile, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Pozo de veneno", TipoZona.Veneno, 0f, 16f, trampas);
        HerramientasEscena.CrearCharco("Lava del piso 2", TipoZona.Lava, 4f, 2f, pisoDosYungas, trampas);
        HerramientasEscena.CrearCharco("Agua del piso 3", TipoZona.Agua, -4f, 2f, pisoTresYungas, trampas);
        // TRAMPA: antes de la salida, una puerta de Inti de más; se escapa al acercarse.
        HerramientasEscena.CrearPuertaFalsa("Puerta sospechosa", TipoElemento.Fuego, new Vector2(-7f, pisoTresYungas), new Vector2(0f, 9f), trampas);

        // Las llamas están en todos los niveles de la isla (GDD); esta cuida el piso 1.
        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearLlama("Llama", new Vector2(14.2f, pisoUnoYungas + 0.5f), 2.5f, 4f, peligros);

        Transform cristales = HerramientasEscena.CrearGrupo("Cristales de luz");
        Vector2 tamanoCristal = new Vector2(1.6f, 0.4f);
        HerramientasEscena.CrearPlataformaOculta("Cristal 1", new Vector2(-6f, -2.2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal 2", new Vector2(-2f, -2.2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal 3", new Vector2(2f, -2.2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal 4", new Vector2(6f, -2.2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal de la salida", new Vector2(-9.8f, 7.65f), tamanoCristal, cristales);

        Transform faroles = HerramientasEscena.CrearGrupo("Faroles");
        HerramientasEscena.CrearFarol("Farol de la entrada", new Vector2(-12f, 1f), 4f, faroles);
        HerramientasEscena.CrearFarol("Farol de la salida", new Vector2(-13f, 12.3f), 3.5f, faroles);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(12f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(10.8f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(6.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-6.5f, 7.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-14.5f, pisoAltoAiquile), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-12.5f, pisoAltoAiquile), puertas);

        HerramientasEscena.CrearCicloEclipse(segundosEntreEclipses);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-11f, suelo));
        HerramientasEscena.AgregarLuzPersonaje(inti);
        HerramientasEscena.AgregarLuzPersonaje(killa);
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Púas en el borde del pozo, donde se espera la luz (hay que esperar un paso
        // atrás). Veneno donde se aterriza en los pisos 2 y 3, y puertas que se cambian.
        HerramientasEscena.CrearPuasOcultas("Borde tranquilo", -9.2f, 0.8f, suelo, true, trampas);
        HerramientasEscena.CrearPisoQueSeCalienta("Piso 2 que se envenena", TipoZona.Veneno, 8.2f, 1f, pisoDosYungas, trampas);
        HerramientasEscena.CrearPisoQueSeCalienta("Piso 3 que se envenena", TipoZona.Veneno, -0.8f, 1f, pisoTresYungas, trampas);

        // PSICOLÓGICA: púas que parecen mortales al final del piso 1, pero son
        // de goma. Y en el piso 3, púas que caen del techo en fila (con sombra).
        HerramientasEscena.CrearPuasDeGoma("Puas raras", 13.5f, 1.5f, pisoUnoYungas, trampas);
        HerramientasEscena.CrearPuasQueCaenDelTecho("Puas del techo", -1f, -5f, pisoTresYungas + 3.2f, pisoTresYungas, 3, trampas);

        string ruta = RutaNivel(6, 3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 6, Nivel 4: Todo a la vez =================
    // Hojas sobre ríos, una palanca traicionera, lianas y eclipse automático:
    // la salida tiene un charco de cada lado y solo se cruza con el poder justo.

    private static string GenerarMundo6Nivel4()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaIsla("Todo a la vez: hojas, lianas, una palanca sospechosa y el eclipse que no para. Salgan de los ríos antes de la cuenta.", new Color(1f, 0.9f, 0.85f), 1f, out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloIsla);
        CrearPisoIsla("Rama izquierda", -10.5f, 9f, pisoDosYungas, escenario);
        CrearPisoIsla("Rama derecha", 10.5f, 9f, pisoDosYungas, escenario);
        CrearPisoIsla("Salida", 0f, 8f, pisoTresYungas, escenario);
        HerramientasEscena.CrearPlataforma("Muro central", new Vector2(0f, 8.25f), new Vector2(0.5f, 4f), HerramientasEscena.colorSueloIsla, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava de abajo", TipoZona.Lava, -9.5f, 11f, trampas);
        HerramientasEscena.CrearCharco("Agua de abajo", TipoZona.Agua, 9.5f, 11f, trampas);
        HerramientasEscena.CrearCharco("Agua de la salida", TipoZona.Agua, -3f, 2f, pisoTresYungas, trampas);
        HerramientasEscena.CrearCharco("Lava de la salida", TipoZona.Lava, 3f, 2f, pisoTresYungas, trampas);

        Transform hojas = HerramientasEscena.CrearGrupo("Hojas");
        HerramientasEscena.CrearHojaTrampolin("Hoja izquierda", -5f, 1.2f, suelo, hojas);
        HerramientasEscena.CrearHojaTrampolin("Hoja derecha", 5f, 1.2f, suelo, hojas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2 tamanoBarra = new Vector2(0.5f, 4f);
        PuertaMecanica barraIzquierda = HerramientasEscena.CrearPuertaMecanica("Barra izquierda", new Vector2(-4.25f, 8.25f), tamanoBarra, new Vector2(0f, 5f), 1, mecanismos);
        PuertaMecanica barraDerecha = HerramientasEscena.CrearPuertaMecanica("Barra derecha", new Vector2(4.25f, 8.25f), tamanoBarra, new Vector2(0f, 5f), 1, mecanismos);
        List<ObjetoActivable> abreLasBarras = new List<ObjetoActivable>();
        abreLasBarras.Add(barraIzquierda);
        abreLasBarras.Add(barraDerecha);
        // TRAICIÓN: abre la salida, pero le saca púas al compañero.
        HerramientasEscena.CrearPalancaSospechosa("Palanca de la rama", new Vector2(-13f, pisoDosYungas), abreLasBarras, TipoCastigo.SalenPuas, mecanismos);

        Transform lianas = HerramientasEscena.CrearGrupo("Lianas");
        HerramientasEscena.CrearLiana("Liana izquierda", new Vector2(-7.6f, 11.5f), 6f, 3.6f, 0f, lianas);
        HerramientasEscena.CrearLiana("Liana derecha", new Vector2(7.6f, 11.5f), 6f, 3.6f, 0.5f, lianas);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearLlama("Llama", new Vector2(0f, 10.75f), 3f, 4f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-4.2f, 1.5f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(4.2f, 1.5f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-14.3f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(14.3f, 4.15f), gemas);

        // Las puertas están en el piso seco, entre el charco y el muro.
        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-1.1f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(1.1f, pisoTresYungas), puertas);

        HerramientasEscena.CrearCicloEclipse(10f);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-3.4f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(3.4f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Encima de cada hoja, una gema que huye a la rama de su lado. Donde se
        // aterriza en cada rama, el piso se vuelve de lo que mata a quien llega por
        // ahí, y de cada rama sale un enemigo. (Las puertas no se cambian: el muro
        // central lo haría imposible.)
        HerramientasEscena.CrearGemaQueHuye("Gema fuego 3", TipoElemento.Fuego, new Vector2(-5f, 5.5f), new Vector2(-12f, 4.15f), gemas);
        HerramientasEscena.CrearGemaQueHuye("Gema agua 3", TipoElemento.Agua, new Vector2(5f, 5.5f), new Vector2(12f, 4.15f), gemas);
        HerramientasEscena.CrearPisoQueSeCalienta("Rama que se inunda", TipoZona.Agua, -7f, 1.2f, pisoDosYungas, trampas);
        HerramientasEscena.CrearPisoQueSeCalienta("Rama que se calienta", TipoZona.Lava, 7f, 1.2f, pisoDosYungas, trampas);
        HerramientasEscena.CrearEnemigoSorpresa("Enemigo de la rama izquierda", new Vector2(-10f, pisoDosYungas), trampas);
        HerramientasEscena.CrearEnemigoSorpresa("Enemigo de la rama derecha", new Vector2(10f, pisoDosYungas), trampas);

        // BUCLE: la sala se repite tres veces y cada vez cambia una regla.
        ConvertirEnSalaQueSeRepite(trampas, new Vector2(0f, pisoTresYungas + 1.2f), new Vector2(5f, 2.4f));

        string ruta = RutaNivel(6, 4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 6, Nivel 5: El gran final =================
    // El Guardián del Eclipse. Hay que pisar a la vez los dos botones de los
    // extremos, tres veces, esquivando sus bolas de fuego y de agua. Al vencerlo
    // sube un escalón que lleva a las puertas.

    private static string GenerarMundo6Nivel5()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaIsla("¡El Guardián del Eclipse! Pisen a la vez los dos botones de los extremos. Su fuego daña al agua, y su agua al fuego.", new Color(0.9f, 0.8f, 0.95f), 0.9f, out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloIsla);
        CrearPisoIsla("Altar de las puertas", 0f, 6f, pisoDosYungas, escenario);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        // Escalón escondido bajo el suelo: sube cuando vencen al Guardián.
        PuertaMecanica escalon = HerramientasEscena.CrearPuertaMecanica("Escalon del altar", new Vector2(-6.5f, -5f), new Vector2(3f, 0.5f), new Vector2(0f, 4.5f), 1, mecanismos);
        GameObject botonIzquierdo = HerramientasEscena.CrearBotonPresion("Boton izquierdo", new Vector2(-12.5f, suelo), new List<ObjetoActivable>(), mecanismos);
        GameObject botonDerecho = HerramientasEscena.CrearBotonPresion("Boton derecho", new Vector2(12.5f, suelo), new List<ObjetoActivable>(), mecanismos);

        ObjetoActivable[] alSerVencido = new ObjetoActivable[] { escalon };
        HerramientasEscena.CrearGuardian(new Vector2(0f, 8f), HerramientasEscena.BuscarBoton(botonIzquierdo), HerramientasEscena.BuscarBoton(botonDerecho), alSerVencido);

        // Como en todos los niveles, 2 gemas por elemento (GDD). Cada uno tiene las
        // suyas de su lado, en el camino hacia su botón.
        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-4.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-10.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(4.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(10.5f, -2.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-1.5f, pisoDosYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(1.5f, pisoDosYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-2f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(2f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // En el camino hacia cada botón (nunca encima) el piso se llena de veneno,
        // y al final las puertas del altar se cambian de lugar.
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearPisoQueSeCalienta("Camino izquierdo que se envenena", TipoZona.Veneno, -8f, 1f, suelo, trampas);
        HerramientasEscena.CrearPisoQueSeCalienta("Camino derecho que se envenena", TipoZona.Veneno, 8f, 1f, suelo, trampas);
        CambiarPuertasDeLugar(trampas);

        string ruta = RutaNivel(6, 5);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
