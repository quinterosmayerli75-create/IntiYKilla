// =====================================================================
// GeneradorEscenasMundo5.cs
// Proyecto: Inti y Killa
// Descripción: Los 5 niveles del Mundo 5 (Aiquile, tierra del charango).
//              Es otra parte de GeneradorEscenas.
// Medidas de diseño: una cuerda lanza igual que una hoja (sube 7.4). Quien
//              llega caminando rebota en el borde de la cuerda, a unos 1.1 de
//              su centro: por eso sobre ese punto no puede haber un piso.
//              Notas: 0 rojo, 1 naranja, 2 amarillo, 3 verde, 4 celeste.
// =====================================================================

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static partial class GeneradorEscenas
{
    private static readonly Color colorFondoAiquile = new Color(0.86f, 0.55f, 0.40f);
    private const float pisoAltoAiquile = 9.5f;

    private static void CrearPisoAiquile(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        Vector2 posicion = new Vector2(centroX, alturaPiso - 0.25f);
        HerramientasEscena.CrearPlataforma(nombre, posicion, new Vector2(ancho, 0.5f), HerramientasEscena.colorSueloAiquile, padre);
    }

    private static Scene PrepararEscenaAiquile(string pista, out Camera camaraPrincipal, out Light2D luz)
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoAiquile);
        luz = HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.95f, 0.85f), 1f);
        HerramientasEscena.CrearGestorNivel(pista);
        HerramientasEscena.CrearFondoDeMundo(5);
        return escena;
    }

    // ================= Mundo 5, Nivel 1: Cuerdas de charango =================
    // Enseña las cuerdas: lanzan muy alto. Inti sube por la izquierda (lava) y
    // Killa por la derecha (agua), cuerda por cuerda, hasta la cima.

    private static string GenerarMundo5Nivel1()
    {
        Camera camaraPrincipal;
        Light2D luz;
        Scene escena = PrepararEscenaAiquile("Las cuerdas del charango te lanzan muy alto. Inti sube por la izquierda y Killa por la derecha.", out camaraPrincipal, out luz);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloAiquile);
        CrearPisoAiquile("Loma izquierda", -13f, 4f, pisoDosYungas, escenario);
        CrearPisoAiquile("Loma derecha", 13f, 4f, pisoDosYungas, escenario);
        CrearPisoAiquile("Escalon izquierdo", -6f, 3f, pisoDosYungas, escenario);
        CrearPisoAiquile("Escalon derecho", 6f, 3f, pisoDosYungas, escenario);
        CrearPisoAiquile("Cima", 0f, 10f, pisoAltoAiquile, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava", TipoZona.Lava, -9.5f, 11f, trampas);
        HerramientasEscena.CrearCharco("Agua", TipoZona.Agua, 9.5f, 11f, trampas);
        PuasOcultas puasDeLaGema = HerramientasEscena.CrearPuasOcultas("Piso con gema", 0f, 1.2f, pisoAltoAiquile, false, trampas);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(puasDeLaGema);

        Transform cuerdas = HerramientasEscena.CrearGrupo("Cuerdas");
        HerramientasEscena.CrearCuerda("Cuerda izquierda baja", -10f, suelo, 0, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda derecha baja", 10f, suelo, 4, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda izquierda alta", -5.3f, pisoDosYungas, 1, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda derecha alta", 5.3f, pisoDosYungas, 3, MomentoCuerda.Siempre, cuerdas);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Agua, new Vector2(0f, 10.6f), trampasDeLaGema, gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-7f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(7f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-13.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(13.5f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-2.5f, pisoAltoAiquile), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(2.5f, pisoAltoAiquile), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-2f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(2f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(5, 1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 5, Nivel 2: La puerta melódica =================
    // Enseña la puerta melódica: las luces de arriba dicen qué notas tocar y en
    // qué orden. Del otro lado, una piedra rueda hacia la reja.

    private static string GenerarMundo5Nivel2()
    {
        Camera camaraPrincipal;
        Light2D luz;
        Scene escena = PrepararEscenaAiquile("Las luces de la reja muestran la melodía: toquen esas cuerdas, en ese orden.", out camaraPrincipal, out luz);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloAiquile);
        CrearPisoAiquile("Escalon", 6f, 2f, pisoUnoYungas, escenario);
        CrearPisoAiquile("Salida", 12f, 6f, pisoDosYungas, escenario);

        Transform cuerdas = HerramientasEscena.CrearGrupo("Cuerdas");
        for (int nota = 0; nota <= 4; nota++)
        {
            HerramientasEscena.CrearCuerda("Cuerda " + nota, -13f + nota * 3f, suelo, nota, MomentoCuerda.Siempre, cuerdas);
        }

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2[] barras = new Vector2[] { new Vector2(2.5f, 1.375f) };
        HerramientasEscena.CrearPuertaMelodica("Reja melodica", new int[] { 1, 4, 2 }, new Vector2(2.5f, 7.2f), barras, new Vector2(0.5f, 9.75f), new Vector2(0f, -10f), mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearPiedraRodante("Piedra", 14.4f, suelo, -10.5f, 7f, 2.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-11.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-5.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(6f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(10f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(12f, pisoDosYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(14f, pisoDosYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-14.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-14.5f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(5, 2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // Arma la mitad izquierda y la derecha con un muro en medio y la salida
    // encima, entre dos barras. Lo comparten los niveles 5-3 y 5-5.
    private static void CrearEscenarioDuo(Transform escenario, Transform trampas)
    {
        HerramientasEscena.CrearPlataforma("Muro central", new Vector2(0f, -0.25f), new Vector2(0.5f, 6.5f), HerramientasEscena.colorSueloAiquile, escenario);
        CrearPisoAiquile("Salida", 0f, 8f, pisoDosYungas, escenario);
        HerramientasEscena.CrearCharco("Lava", TipoZona.Lava, -7.6f, 15.2f, trampas);
        HerramientasEscena.CrearCharco("Agua", TipoZona.Agua, 7.6f, 15.2f, trampas);
    }

    // ================= Mundo 5, Nivel 3: Melodía a dúo =================
    // Cada uno tiene sus cuerdas, a un lado del muro, y la melodía va pasando de
    // un lado al otro: hay que turnarse.

    private static string GenerarMundo5Nivel3()
    {
        Camera camaraPrincipal;
        Light2D luz;
        Scene escena = PrepararEscenaAiquile("Melodía a dúo: cada uno tiene sus cuerdas. Túrnense siguiendo las luces de arriba.", out camaraPrincipal, out luz);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloAiquile);
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        CrearEscenarioDuo(escenario, trampas);

        Transform cuerdas = HerramientasEscena.CrearGrupo("Cuerdas");
        HerramientasEscena.CrearCuerda("Cuerda 0", -12f, suelo, 0, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 1", -9f, suelo, 1, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 2", -5.3f, suelo, 2, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 3", 5.3f, suelo, 3, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 4", 9f, suelo, 4, MomentoCuerda.Siempre, cuerdas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2[] barras = new Vector2[] { new Vector2(-4.25f, 5.25f), new Vector2(4.25f, 5.25f) };
        HerramientasEscena.CrearPuertaMelodica("Barras melodicas", new int[] { 0, 3, 1, 4, 2 }, new Vector2(0f, 9.5f), barras, new Vector2(0.5f, 4.5f), new Vector2(0f, 6f), mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearAvispa("Avispa", new Vector2(0f, 8f), new Vector2(0f, 6f), new Vector2(8f, 6f), 2.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-14f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(14f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-3f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(3f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-1.5f, pisoDosYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(1.5f, pisoDosYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-2f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(2f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(5, 3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 5, Nivel 4: Serenata nocturna =================
    // Algunas cuerdas solo suenan de día y otras solo de noche: la melodía obliga
    // a esperar el momento justo. Después, un botón y una palanca traicioneros.

    private static string GenerarMundo5Nivel4()
    {
        Camera camaraPrincipal;
        Light2D luz;
        Scene escena = PrepararEscenaAiquile("Serenata nocturna: algunas cuerdas suenan solo de día y otras solo de noche. Del otro lado... cuidado con lo que tocan.", out camaraPrincipal, out luz);
        HerramientasEscena.CrearCicloDiaNoche(luz);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloAiquile);
        CrearPisoAiquile("Escalon", 6.5f, 2f, pisoUnoYungas, escenario);
        CrearPisoAiquile("Salida", 12f, 6f, pisoDosYungas, escenario);

        Transform cuerdas = HerramientasEscena.CrearGrupo("Cuerdas");
        HerramientasEscena.CrearCuerda("Cuerda 0", -13f, suelo, 0, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 1", -10f, suelo, 1, MomentoCuerda.SoloDeNoche, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 2", -7f, suelo, 2, MomentoCuerda.SoloDeDia, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 3", -4f, suelo, 3, MomentoCuerda.SoloDeNoche, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 4", -1f, suelo, 4, MomentoCuerda.Siempre, cuerdas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2[] barras = new Vector2[] { new Vector2(2.5f, 1.375f) };
        HerramientasEscena.CrearPuertaMelodica("Reja melodica", new int[] { 2, 3, 0, 1 }, new Vector2(2.5f, 7.2f), barras, new Vector2(0.5f, 9.75f), new Vector2(0f, -10f), mecanismos);

        // TRAICIÓN: entre la reja y el escalón. Se puede saltar.
        HerramientasEscena.CrearBotonTraicion("Boton del camino", new Vector2(4.4f, suelo), TipoCastigo.CaeLava, mecanismos);
        // TRAICIÓN: la reja de las puertas se abre con esta palanca.
        PuertaMecanica reja = HerramientasEscena.CrearPuertaMecanica("Reja de las puertas", new Vector2(10.6f, 5f), new Vector2(0.5f, 4f), new Vector2(0f, 5f), 1, mecanismos);
        HerramientasEscena.CrearPalancaSospechosa("Palanca del escalon", new Vector2(6.5f, pisoUnoYungas), HerramientasEscena.ListaCon(reja), TipoCastigo.SalenPuas, mecanismos);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-11.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-2.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(9.6f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(11f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(12f, pisoDosYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(13.5f, pisoDosYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-14.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-14.5f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(5, 4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 5, Nivel 5: El gran concierto =================
    // Examen del mundo: melodía larga a dúo, con cuerdas de día y de noche, una
    // avispa que cuida una gema, y botones traicioneros junto al muro.

    private static string GenerarMundo5Nivel5()
    {
        Camera camaraPrincipal;
        Light2D luz;
        Scene escena = PrepararEscenaAiquile("El gran concierto: una melodía larga, a dúo, de día y de noche. Que no se les escape ninguna nota.", out camaraPrincipal, out luz);
        HerramientasEscena.CrearCicloDiaNoche(luz);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloAiquile);
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        CrearEscenarioDuo(escenario, trampas);

        Transform cuerdas = HerramientasEscena.CrearGrupo("Cuerdas");
        HerramientasEscena.CrearCuerda("Cuerda 0", -12f, suelo, 0, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 1", -9f, suelo, 1, MomentoCuerda.SoloDeNoche, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 2", -5.3f, suelo, 2, MomentoCuerda.Siempre, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 3", 5.3f, suelo, 3, MomentoCuerda.SoloDeDia, cuerdas);
        HerramientasEscena.CrearCuerda("Cuerda 4", 9f, suelo, 4, MomentoCuerda.Siempre, cuerdas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2[] barras = new Vector2[] { new Vector2(-4.25f, 5.25f), new Vector2(4.25f, 5.25f) };
        HerramientasEscena.CrearPuertaMelodica("Barras melodicas", new int[] { 0, 3, 1, 4, 2, 3 }, new Vector2(0f, 9.5f), barras, new Vector2(0.5f, 4.5f), new Vector2(0f, 6f), mecanismos);
        // TRAICIÓN: junto al muro, donde empieza cada uno. Hay que saltarlos.
        HerramientasEscena.CrearBotonTraicion("Boton de Inti", new Vector2(-2.3f, suelo), TipoCastigo.SalenPuas, mecanismos);
        HerramientasEscena.CrearBotonTraicion("Boton de Killa", new Vector2(2.3f, suelo), TipoCastigo.SalenPuas, mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearAvispa("Avispa", new Vector2(14f, 4f), new Vector2(13f, 0.5f), new Vector2(5f, 9f), 1.6f, peligros);
        HerramientasEscena.CrearAvispa("Avispa de la salida", new Vector2(0f, 8f), new Vector2(0f, 6f), new Vector2(8f, 6f), 2.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-14f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(14f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-3f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(3f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-1.5f, pisoDosYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(1.5f, pisoDosYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-0.7f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(0.7f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(5, 5);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
