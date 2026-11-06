// =====================================================================
// GeneradorEscenasMundo3.cs
// Proyecto: Inti y Killa
// Descripción: Los 5 niveles del Mundo 3 (Yungas). Es la otra parte de
//              GeneradorEscenas; está separada porque cada mundo es largo.
// Medidas de diseño: el salto sube 3.8 y llega a 6 de distancia, pero bajo un
//              techo a 2.75 solo llega a 2.6. Por eso, cuando un camino pasa
//              por debajo de otro, se separan 6.5 o los huecos son cortos.
// =====================================================================

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public static partial class GeneradorEscenas
{
    private static readonly Color colorFondoYungas = new Color(0.60f, 0.80f, 0.84f);
    private static readonly Color colorLuzYungas = new Color(1f, 0.97f, 0.90f);
    private const float pisoUnoYungas = -0.25f;
    private const float pisoDosYungas = 3f;
    private const float pisoTresYungas = 6.25f;

    // alturaPiso es donde quedan los pies: el bloque va justo debajo.
    private static void CrearPisoYungas(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        Vector2 posicion = new Vector2(centroX, alturaPiso - 0.25f);
        HerramientasEscena.CrearPlataforma(nombre, posicion, new Vector2(ancho, 0.5f), HerramientasEscena.colorSueloYungas, padre);
    }

    private static Vector2 AlturaDeBloque(float centroX, float alturaPiso)
    {
        return new Vector2(centroX, alturaPiso - 0.25f);
    }

    // ================= Mundo 3, Nivel 1: Camino en la niebla =================
    // Enseña la niebla: cada uno ve solo cerca de sí mismo. Es un camino angosto
    // sobre veneno, de ida por abajo y de vuelta por arriba. Abajo, cada uno pisa
    // el lado del charco que no le hace daño. Arriba hay un tramo falso.

    private static string GenerarMundo3Nivel1()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoYungas);
        HerramientasEscena.CrearLuzGlobal(colorLuzYungas, 1f);
        HerramientasEscena.CrearGestorNivel("La niebla solo deja ver de cerca. Avancen con calma... y no pisen sin mirar.");
        HerramientasEscena.CrearFondoYungas();

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloYungas);

        // Camino de ida, sobre el río de veneno.
        CrearPisoYungas("Ida 1", -7f, 3f, pisoUnoYungas, escenario);
        CrearPisoYungas("Ida 2", -1f, 3f, pisoUnoYungas, escenario);
        CrearPisoYungas("Ida 3", 5f, 3f, pisoUnoYungas, escenario);
        CrearPisoYungas("Ida 4", 12f, 4f, pisoUnoYungas, escenario);

        // Escalón para cambiar de camino.
        CrearPisoYungas("Escalon", 13.75f, 1.5f, pisoDosYungas, escenario);

        // Camino de vuelta, por arriba y hacia la izquierda.
        CrearPisoYungas("Vuelta 1", 9.5f, 5f, pisoTresYungas, escenario);
        CrearPisoYungas("Vuelta 2", 4.5f, 3f, pisoTresYungas, escenario);
        CrearPisoYungas("Vuelta 3", -2.05f, 1.9f, pisoTresYungas, escenario);
        CrearPisoYungas("Vuelta 4", -7f, 3f, pisoTresYungas, escenario);
        CrearPisoYungas("Salida", -12f, 5f, pisoTresYungas, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de veneno", TipoZona.Veneno, 3.75f, 23.5f, trampas);
        HerramientasEscena.CrearCharco("Lava del camino", TipoZona.Lava, -1.75f, 1.5f, pisoUnoYungas, trampas);
        HerramientasEscena.CrearCharco("Agua del camino", TipoZona.Agua, 4.25f, 1.5f, pisoUnoYungas, trampas);
        // TRAMPA: en la niebla parece la continuación del tramo siguiente.
        HerramientasEscena.CrearSueloFalso("Tramo de niebla", AlturaDeBloque(-0.55f, pisoTresYungas), new Vector2(1.1f, 0.5f), HerramientasEscena.colorSueloYungas, escenario);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-1.75f, 1f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(4.25f, 1f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(10f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(4.5f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(-2f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(-7f, 7.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-13.5f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-11f, pisoTresYungas), puertas);

        HerramientasEscena.CrearNiebla();

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-11.5f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3, 1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 3, Nivel 2: Lluvia en la montaña =================
    // Enseña la lluvia, y que Killa puede congelar el agua. La lluvia solo apaga a
    // quien tiene el fuego, así que Inti cruza entre techos cuando está seco, y
    // Killa, que puede mojarse, le congela el charco y le ayuda con la compuerta.
    // Cuando suben, un cóndor vigila desde arriba: quien se detiene ahí, lo paga.

    private static string GenerarMundo3Nivel2()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoYungas);
        HerramientasEscena.CrearLuzGlobal(new Color(0.85f, 0.90f, 0.95f), 0.9f);
        HerramientasEscena.CrearGestorNivel("Inti, la lluvia te apaga: cruza entre techos. Killa, congela el charco con Flecha abajo.");
        HerramientasEscena.CrearFondoYungas();

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloYungas);

        HerramientasEscena.CrearTecho("Techo de la entrada", -11f, 4f, pisoUnoYungas, escenario);
        HerramientasEscena.CrearTecho("Techo del charco", -1.5f, 3f, pisoUnoYungas, escenario);
        HerramientasEscena.CrearTecho("Techo del fondo", 7.5f, 3f, pisoUnoYungas, escenario);

        CrearPisoYungas("Alero", 4.5f, 3f, pisoDosYungas, escenario);
        // Llega hasta la compuerta y tapa el hueco: no se puede pasar por arriba.
        CrearPisoYungas("Piso 2", -1.25f, 5.5f, pisoDosYungas, escenario);
        HerramientasEscena.CrearTecho("Techo del medio", -1f, 3f, pisoTresYungas, escenario);
        CrearPisoYungas("Salida", -9f, 8f, pisoTresYungas, escenario);
        HerramientasEscena.CrearTecho("Techo de las puertas", -10f, 8f, 9.5f, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        // Más ancho que un salto: Inti no puede cruzarlo sin que Killa lo congele.
        HerramientasEscena.CrearCharcoCongelable("Charco", -6.25f, 6.5f, suelo, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        PuertaMecanica compuerta = HerramientasEscena.CrearPuertaMecanica("Compuerta", new Vector2(1.75f, -0.5f), new Vector2(0.5f, 6f), new Vector2(0f, -6.5f), 1, mecanismos);
        List<ObjetoActivable> abreCompuerta = new List<ObjetoActivable>();
        abreCompuerta.Add(compuerta);
        // Uno aprieta de este lado para que pase el otro, que aprieta del otro lado.
        HerramientasEscena.CrearBotonPresion("Boton izquierdo", new Vector2(0.7f, suelo), abreCompuerta, mecanismos);
        HerramientasEscena.CrearBotonPresion("Boton derecho", new Vector2(3.4f, suelo), abreCompuerta, mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearCondor("Condor", new Vector2(4.5f, 10.8f), 3.5f, 8.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-6.25f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-1.5f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(8.5f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(4.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(-6f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(-1f, 4.15f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-11.5f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-8.5f, pisoTresYungas), puertas);

        HerramientasEscena.CrearLluvia();

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-12f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-10f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3, 2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 3, Nivel 3: Día y noche =================
    // Enseña el día y la noche, con eclipse obligatorio. Inti sube por la izquierda
    // (lava y plataformas de día) y Killa por la derecha (agua y plataformas de
    // noche), hasta sus placas. Más arriba, cada lado tiene el charco contrario:
    // no se puede seguir sin haber intercambiado los poderes.

    private static string GenerarMundo3Nivel3()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;
        Vector2 tamanoBloque = new Vector2(2f, 0.5f);

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoYungas);
        Light2D luz = HerramientasEscena.CrearLuzGlobal(colorLuzYungas, 1f);
        HerramientasEscena.CrearGestorNivel("Las plataformas doradas existen de día y las azules de noche. Arriba necesitarán el poder del otro: párense en las placas.");
        HerramientasEscena.CrearFondoYungas();
        HerramientasEscena.CrearCicloDiaNoche(luz);

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloYungas);

        HerramientasEscena.CrearPlataformaDiaNoche("Bloque de dia", AlturaDeBloque(-7.75f, pisoUnoYungas), tamanoBloque, true, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Bloque de noche", AlturaDeBloque(7.75f, pisoUnoYungas), tamanoBloque, false, escenario);
        CrearPisoYungas("Placa izquierda", -13f, 5f, pisoUnoYungas, escenario);
        CrearPisoYungas("Placa derecha", 13f, 5f, pisoUnoYungas, escenario);
        CrearPisoYungas("Mirador", 0f, 6f, pisoUnoYungas, escenario);
        CrearPisoYungas("Ladera izquierda", -9.5f, 4f, pisoDosYungas, escenario);
        CrearPisoYungas("Ladera derecha", 9.5f, 4f, pisoDosYungas, escenario);
        float alturaEscalon = 4.6f;
        HerramientasEscena.CrearPlataformaDiaNoche("Escalon de noche", AlturaDeBloque(-5.5f, alturaEscalon), tamanoBloque, false, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Escalon de dia", AlturaDeBloque(5.5f, alturaEscalon), tamanoBloque, true, escenario);
        CrearPisoYungas("Salida", 0f, 6f, pisoTresYungas, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava de abajo", TipoZona.Lava, -9f, 9f, trampas);
        HerramientasEscena.CrearCharco("Agua de abajo", TipoZona.Agua, 9f, 9f, trampas);
        // Cada ladera solo se cruza con el poder que se consigue con el eclipse.
        HerramientasEscena.CrearCharco("Agua de la ladera", TipoZona.Agua, -9.5f, 4f, pisoDosYungas, trampas);
        HerramientasEscena.CrearCharco("Lava de la ladera", TipoZona.Lava, 9.5f, 4f, pisoDosYungas, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        HerramientasEscena.CrearAltarEclipse("Altar", new Vector2(-13f, pisoUnoYungas), new Vector2(13f, pisoUnoYungas), colorAltarUno, mecanismos);

        // TRAMPA: una puerta que parece la salida, y una gema regalada en el mirador.
        HerramientasEscena.CrearPuertaFalsa("Puerta de mentira", TipoElemento.Agua, new Vector2(-9.5f, pisoDosYungas), new Vector2(0f, 4f), trampas);
        TechoQueCae bloqueDeLaGema = HerramientasEscena.CrearTechoQueCae("Bloque del mirador", new Vector2(0f, 3.2f), new Vector2(1.5f, 0.5f), pisoUnoYungas, false, HerramientasEscena.colorSueloYungas, escenario);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(bloqueDeLaGema);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        // Pasa por donde empiezan: hay que salir de ahí pronto.
        HerramientasEscena.CrearPiedraRodante("Piedra", 3.4f, suelo, -6.8f, 7f, 3f, peligros);
        HerramientasEscena.CrearCondor("Condor", new Vector2(-10f, 11.5f), 2f, 8.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Fuego, new Vector2(0f, 0.9f), trampasDeLaGema, gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-9f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(9f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-2f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(2f, 0.9f), gemas);

        // Después del eclipse, Inti tiene el agua y Killa el fuego.
        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-1.5f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(1.5f, pisoTresYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-1.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(1.5f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3, 3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 3, Nivel 4: Puente entre nubes =================
    // Niebla más plataformas de día y de noche: hay que saltar justo cuando cambia
    // la luz, viendo solo el siguiente bloque. Aparecen las traiciones: un botón en
    // el camino y una palanca necesaria que le saca púas al compañero.

    private static string GenerarMundo3Nivel4()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;
        Vector2 tamanoBloque = new Vector2(2.5f, 0.5f);

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoYungas);
        Light2D luz = HerramientasEscena.CrearLuzGlobal(colorLuzYungas, 1f);
        HerramientasEscena.CrearGestorNivel("Un puente entre nubes: salten justo cuando cambie la luz, antes de que su bloque desaparezca. Y si tu compañero toca algo... corre.");
        HerramientasEscena.CrearFondoYungas();
        HerramientasEscena.CrearCicloDiaNoche(luz);

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloYungas);

        CrearPisoYungas("Inicio del puente", -10.5f, 3f, pisoUnoYungas, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Puente 1", AlturaDeBloque(-5.5f, pisoUnoYungas), tamanoBloque, true, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Puente 2", AlturaDeBloque(0f, pisoUnoYungas), tamanoBloque, false, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Puente 3", AlturaDeBloque(5.5f, pisoUnoYungas), tamanoBloque, true, escenario);
        CrearPisoYungas("Descanso", 11.5f, 5f, pisoUnoYungas, escenario);
        CrearPisoYungas("Escalon", 13.75f, 1.5f, pisoDosYungas, escenario);
        CrearPisoYungas("Mirador", 9.5f, 5f, pisoTresYungas, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Puente 4", AlturaDeBloque(3.5f, pisoTresYungas), tamanoBloque, false, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Puente 5", AlturaDeBloque(-2f, pisoTresYungas), tamanoBloque, true, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Puente 6", AlturaDeBloque(-7.5f, pisoTresYungas), tamanoBloque, false, escenario);
        CrearPisoYungas("Salida", -12f, 5f, pisoTresYungas, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de veneno", TipoZona.Veneno, 3.25f, 24.5f, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        // TRAICIÓN: en medio del descanso. Se puede saltar.
        HerramientasEscena.CrearBotonTraicion("Boton del descanso", new Vector2(10.3f, pisoUnoYungas), TipoCastigo.CaeLava, mecanismos);
        // TRAICIÓN: sin esta palanca no se abre la salida, pero castiga al compañero.
        PuertaMecanica reja = HerramientasEscena.CrearPuertaMecanica("Reja de la salida", new Vector2(-9.75f, 8.75f), new Vector2(0.5f, 5f), new Vector2(0f, 6f), 1, mecanismos);
        List<ObjetoActivable> abreReja = new List<ObjetoActivable>();
        abreReja.Add(reja);
        HerramientasEscena.CrearPalancaSospechosa("Palanca del mirador", new Vector2(10f, pisoTresYungas), abreReja, TipoCastigo.SalenPuas, mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearCondor("Condor", new Vector2(12f, 11.5f), 3f, 7f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-5.5f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(0f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(3.5f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-2f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(5.5f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(-7.5f, 7.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-13.5f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-11.5f, pisoTresYungas), puertas);

        HerramientasEscena.CrearNiebla();

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-14.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-13f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3, 4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 3, Nivel 5: El camino de la muerte =================
    // Examen del mundo: niebla, lluvia, charco que se congela, plataformas de día
    // y de noche, piedras rodantes, cóndor y traiciones. Los bloques van bajo el
    // camino de arriba: ahí no llueve, pero el techo acorta el salto.

    private static string GenerarMundo3Nivel5()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;
        Vector2 tamanoBloque = new Vector2(3f, 0.5f);

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoYungas);
        Light2D luz = HerramientasEscena.CrearLuzGlobal(colorLuzYungas, 1f);
        HerramientasEscena.CrearGestorNivel("El camino de la muerte: niebla, lluvia, piedras y un cóndor. Y tu compañero tampoco es de fiar.");
        HerramientasEscena.CrearFondoYungas();
        HerramientasEscena.CrearCicloDiaNoche(luz);

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloYungas);

        // Planta baja, de derecha a izquierda.
        HerramientasEscena.CrearTecho("Techo de la entrada", 10.5f, 4f, pisoUnoYungas, escenario);
        HerramientasEscena.CrearTecho("Techo del charco", -1.5f, 3f, pisoUnoYungas, escenario);
        CrearPisoYungas("Terraza", -10.2f, 5.6f, pisoUnoYungas, escenario);

        // Bloques bajo el camino de arriba, de izquierda a derecha.
        HerramientasEscena.CrearPlataformaDiaNoche("Bloque 1", AlturaDeBloque(-4.5f, pisoDosYungas), tamanoBloque, true, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Bloque 2", AlturaDeBloque(0.5f, pisoDosYungas), tamanoBloque, false, escenario);
        HerramientasEscena.CrearPlataformaDiaNoche("Bloque 3", AlturaDeBloque(5.5f, pisoDosYungas), tamanoBloque, true, escenario);
        CrearPisoYungas("Camino medio", 11.5f, 6f, pisoDosYungas, escenario);
        HerramientasEscena.CrearTecho("Techo del camino medio", 13.75f, 2.5f, pisoTresYungas, escenario);

        // Camino de arriba, de derecha a izquierda.
        CrearPisoYungas("Camino alto", 4.625f, 8.75f, pisoTresYungas, escenario);
        CrearPisoYungas("Salida", -8f, 12f, pisoTresYungas, escenario);
        HerramientasEscena.CrearTecho("Techo de las puertas", -12f, 5f, 9.5f, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharcoCongelable("Charco", 3.75f, 6.5f, suelo, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        // TRAICIÓN: justo antes de subir a la terraza.
        HerramientasEscena.CrearBotonTraicion("Boton de la subida", new Vector2(-4.9f, suelo), TipoCastigo.CaeLava, mecanismos);
        // TRAICIÓN: abre la reja de la salida, pero le saca púas al compañero.
        PuertaMecanica reja = HerramientasEscena.CrearPuertaMecanica("Reja de la salida", new Vector2(-8f, 8.75f), new Vector2(0.5f, 5f), new Vector2(0f, 6f), 1, mecanismos);
        List<ObjetoActivable> abreReja = new List<ObjetoActivable>();
        abreReja.Add(reja);
        HerramientasEscena.CrearPalancaSospechosa("Palanca del camino", new Vector2(13f, pisoDosYungas), abreReja, TipoCastigo.SalenPuas, mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearPiedraRodante("Piedra de abajo", -14f, suelo, 8.8f, 7f, 2f, peligros);
        HerramientasEscena.CrearPiedraRodante("Piedra del medio", 14.4f, pisoDosYungas, -4.5f, 7f, 3f, peligros);
        HerramientasEscena.CrearPiedraRodante("Piedra de arriba", 8.85f, pisoTresYungas, -8.45f, 7f, 3f, peligros);
        HerramientasEscena.CrearCondor("Condor", new Vector2(-4f, 11.5f), 2.5f, 6f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(3.75f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(6f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-4.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(0.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(11.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(3.75f, 7.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-13f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-11f, pisoTresYungas), puertas);

        HerramientasEscena.CrearLluvia();
        HerramientasEscena.CrearNiebla();

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(11f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(9.5f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3, 5);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
