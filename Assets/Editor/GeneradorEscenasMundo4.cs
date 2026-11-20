// =====================================================================
// GeneradorEscenasMundo4.cs
// Proyecto: Inti y Killa
// Descripción: Los 5 niveles del Mundo 4 (Trópico). Es otra parte de
//              GeneradorEscenas.
// Medidas de diseño: el salto sube 3.8. La hoja trampolín sube 7.4. Una liana
//              de largo L con 32 grados se aleja 0.53 x L hacia cada lado, y su
//              punta sube 0.15 x L en los extremos.
// =====================================================================

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static partial class GeneradorEscenas
{
    private static readonly Color colorFondoTropico = new Color(0.45f, 0.72f, 0.74f);
    private static readonly Color colorLuzTropico = new Color(1f, 0.98f, 0.90f);

    private static void CrearPisoTropico(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        Vector2 posicion = new Vector2(centroX, alturaPiso - 0.25f);
        HerramientasEscena.CrearPlataforma(nombre, posicion, new Vector2(ancho, 0.5f), HerramientasEscena.colorSueloTropico, padre);
    }

    private static Scene PrepararEscenaTropico(string pista, out Camera camaraPrincipal)
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoTropico);
        HerramientasEscena.CrearLuzGlobal(colorLuzTropico, 1f);
        HerramientasEscena.CrearGestorNivel(pista);
        HerramientasEscena.CrearFondoTropico();
        return escena;
    }

    // ================= Mundo 4, Nivel 1: Ríos y lianas =================
    // Enseña las lianas: un río ancho que Inti solo cruza balanceándose de una a
    // otra. Killa puede cruzar por el agua, y ahí abajo está su gema.

    private static string GenerarMundo4Nivel1()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaTropico("Tocá una liana en el aire para agarrarte, y salta para soltarte. Inti, no pises el río.", out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloTropico);
        CrearPisoTropico("Rama izquierda", -12f, 4f, pisoUnoYungas, escenario);
        CrearPisoTropico("Salida", 13f, 4f, pisoUnoYungas, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio", TipoZona.Agua, 0f, 18f, trampas);
        // TRAMPA: una puerta que se escapa, y una gema regalada con púas abajo.
        HerramientasEscena.CrearPuertaFalsa("Puerta de mentira", TipoElemento.Fuego, new Vector2(10.4f, suelo), new Vector2(0f, 9f), trampas);
        PuasOcultas puasDeLaGema = HerramientasEscena.CrearPuasOcultas("Piso con gema", 13f, 1.5f, suelo, false, trampas);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(puasDeLaGema);

        Transform lianas = HerramientasEscena.CrearGrupo("Lianas");
        HerramientasEscena.CrearLiana("Liana 1", new Vector2(-5f, 5.5f), 6.5f, 3.6f, 0f, lianas);
        HerramientasEscena.CrearLiana("Liana 2", new Vector2(4f, 5.5f), 6.5f, 3.6f, 0.5f, lianas);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Fuego, new Vector2(13f, -2.4f), trampasDeLaGema, gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-5f, -0.3f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(4f, -0.3f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(0f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-12f, 0.9f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(12f, pisoUnoYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(14f, pisoUnoYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-11f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // En el borde de la rama, donde se espera la liana, aparece agua (Inti
        // tiene que esperar un paso más atrás). Donde se aterriza en la salida, una
        // neblina invierte los controles. Y las puertas se cambian de lugar.
        HerramientasEscena.CrearPisoQueSeCalienta("Borde que se inunda", TipoZona.Agua, -10.6f, 1.2f, pisoUnoYungas, trampas);
        HerramientasEscena.CrearZonaControlesInvertidos("Neblina engañosa", new Vector2(11.6f, 1f), new Vector2(1.6f, 2.2f), trampas);

        // PSICOLÓGICA: un arbusto de adorno junto a la pared tiene ojos y muerde.
        HerramientasEscena.CrearArbustoTrampa("Arbusto de la rama", -14.6f, pisoUnoYungas, trampas);

        string ruta = RutaNivel(4, 1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 4, Nivel 2: Hojas trampolín =================
    // Enseña las hojas y los monos, con cooperación cruzada: cada botón abre la
    // reja del lado contrario, así que uno espera mientras pasa el otro.

    private static string GenerarMundo4Nivel2()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaTropico("Las hojas verdes te lanzan muy alto. Cada botón abre la reja de tu compañero: ayúdense.", out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;
        float pisoAlto = 9.25f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloTropico);
        CrearPisoTropico("Rama izquierda", -7.25f, 4.5f, pisoDosYungas, escenario);
        CrearPisoTropico("Rama derecha", 7.25f, 4.5f, pisoDosYungas, escenario);
        CrearPisoTropico("Rama exterior izquierda", -12.5f, 6f, pisoDosYungas, escenario);
        CrearPisoTropico("Rama exterior derecha", 12.5f, 6f, pisoDosYungas, escenario);
        CrearPisoTropico("Copa del arbol", 0f, 22f, pisoAlto, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Veneno izquierdo", TipoZona.Veneno, -10f, 10f, trampas);
        HerramientasEscena.CrearCharco("Veneno derecho", TipoZona.Veneno, 10f, 10f, trampas);
        // TRAMPA: una puerta de Inti en la copa, lejos de la verdadera; se escapa al acercarse.
        HerramientasEscena.CrearPuertaFalsa("Puerta sospechosa", TipoElemento.Fuego, new Vector2(-7f, pisoAlto), new Vector2(0f, 9f), trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2 tamanoReja = new Vector2(0.5f, 4.5f);
        Vector2 hundirse = new Vector2(0f, -5f);
        PuertaMecanica rejaIzquierda = HerramientasEscena.CrearPuertaMecanica("Reja izquierda", new Vector2(-9.5f, 5.25f), tamanoReja, hundirse, 1, mecanismos);
        PuertaMecanica rejaDerecha = HerramientasEscena.CrearPuertaMecanica("Reja derecha", new Vector2(9.5f, 5.25f), tamanoReja, hundirse, 1, mecanismos);
        List<ObjetoActivable> abreDerecha = new List<ObjetoActivable>();
        abreDerecha.Add(rejaDerecha);
        List<ObjetoActivable> abreIzquierda = new List<ObjetoActivable>();
        abreIzquierda.Add(rejaIzquierda);
        HerramientasEscena.CrearBotonPresion("Boton que abre la derecha", new Vector2(-7.5f, pisoDosYungas), abreDerecha, mecanismos);
        HerramientasEscena.CrearBotonPresion("Boton que abre la izquierda", new Vector2(10.5f, pisoDosYungas), abreIzquierda, mecanismos);

        Transform hojas = HerramientasEscena.CrearGrupo("Hojas");
        HerramientasEscena.CrearHojaTrampolin("Hoja izquierda", -4f, 1.6f, suelo, hojas);
        HerramientasEscena.CrearHojaTrampolin("Hoja derecha", 4f, 1.6f, suelo, hojas);
        HerramientasEscena.CrearHojaTrampolin("Hoja exterior izquierda", -12f, 1.6f, pisoDosYungas, hojas);
        HerramientasEscena.CrearHojaTrampolin("Hoja exterior derecha", 12f, 1.6f, pisoDosYungas, hojas);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearMono("Mono", new Vector2(0f, pisoAlto + 0.5f), 7f, 3.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-3.2f, 1.5f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(3.2f, 1.5f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-14.3f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(14.3f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(-6f, 10.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(6f, 10.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-2f, pisoAlto), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(2f, pisoAlto), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-1.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(1.5f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Encima de cada hoja central, una gema que al acercarse huye a la rama de
        // su lado. En la copa, donde se aterriza desde las hojas de afuera, el piso
        // se llena de veneno. Y las puertas se cambian de lugar.
        HerramientasEscena.CrearGemaQueHuye("Gema fuego 4", TipoElemento.Fuego, new Vector2(-4f, 5.5f), new Vector2(-7.25f, 4.1f), gemas);
        HerramientasEscena.CrearGemaQueHuye("Gema agua 4", TipoElemento.Agua, new Vector2(4f, 5.5f), new Vector2(7.25f, 4.1f), gemas);
        HerramientasEscena.CrearPisoQueSeCalienta("Copa que se envenena izquierda", TipoZona.Veneno, -8.5f, 1.2f, pisoAlto, trampas);
        HerramientasEscena.CrearPisoQueSeCalienta("Copa que se envenena derecha", TipoZona.Veneno, 8.5f, 1.2f, pisoAlto, trampas);

        // PSICOLÓGICA: una hoja en el centro, igual a las demás, es una catapulta
        // que lanza contra las púas escondidas bajo la copa.
        HerramientasEscena.CrearTrampolinCatapulta("Hoja del centro", 0f, 1.4f, suelo, pisoAlto - 0.5f, trampas);
        // BUCLE: la sala se repite tres veces y cada vez cambia una regla.
        // (Sin cambio de puertas: cruzar obligaría a pasar por el mono de la copa.)
        ConvertirEnSalaQueSeRepite(trampas, new Vector2(0f, pisoAlto + 1.2f), new Vector2(6f, 2.4f), false);

        string ruta = RutaNivel(4, 2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 4, Nivel 3: Eclipse en la selva =================
    // Eclipse obligatorio: abajo, cada uno solo puede ir por su lado hasta su placa.
    // Las lianas llevan a un charco del elemento contrario: con el poder de
    // siempre, esa rama mata; hay que llegar con los poderes cambiados.

    private static string GenerarMundo4Nivel3()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaTropico("Lleguen a sus placas para provocar el eclipse. Sin el poder del otro, las ramas de arriba son mortales.", out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloTropico);
        CrearPisoTropico("Placa izquierda", -13f, 4f, pisoUnoYungas, escenario);
        CrearPisoTropico("Placa derecha", 13f, 4f, pisoUnoYungas, escenario);
        CrearPisoTropico("Rama de agua", -2.5f, 3f, pisoDosYungas, escenario);
        CrearPisoTropico("Rama de lava", 2.5f, 3f, pisoDosYungas, escenario);
        CrearPisoTropico("Salida", 0f, 4f, pisoTresYungas, escenario);
        CrearPisoTropico("Rama del mono", 0f, 1.5f, 10f, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava de abajo", TipoZona.Lava, -9.5f, 11f, trampas);
        HerramientasEscena.CrearCharco("Agua de abajo", TipoZona.Agua, 9.5f, 11f, trampas);
        HerramientasEscena.CrearCharco("Agua de la rama", TipoZona.Agua, -2.5f, 3f, pisoDosYungas, trampas);
        HerramientasEscena.CrearCharco("Lava de la rama", TipoZona.Lava, 2.5f, 3f, pisoDosYungas, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        HerramientasEscena.CrearAltarEclipse("Altar", new Vector2(-13f, pisoUnoYungas), new Vector2(13f, pisoUnoYungas), colorAltarUno, mecanismos);

        // TRAMPA: la gema regalada del centro suelta un bloque.
        TechoQueCae bloqueDeLaGema = HerramientasEscena.CrearTechoQueCae("Bloque del centro", new Vector2(0f, 2.2f), new Vector2(1.5f, 0.5f), suelo, false, HerramientasEscena.colorSueloTropico, escenario);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(bloqueDeLaGema);

        Transform lianas = HerramientasEscena.CrearGrupo("Lianas");
        HerramientasEscena.CrearLiana("Liana izquierda", new Vector2(-6.5f, 9.5f), 6.5f, 3.6f, 0f, lianas);
        HerramientasEscena.CrearLiana("Liana derecha", new Vector2(6.5f, 9.5f), 6.5f, 3.6f, 0f, lianas);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearSerpiente("Serpiente", -2f, 2f, suelo, 2f, peligros);
        HerramientasEscena.CrearMono("Mono", new Vector2(0f, 10.5f), 3f, 3.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Agua, new Vector2(0f, -2.4f), trampasDeLaGema, gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-9f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(9f, -2.4f), gemas);
        // Se recogen después del eclipse, cuando los poderes están cambiados.
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-6.5f, 3.7f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(6.5f, 3.7f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-1f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(1f, pisoTresYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-3.2f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(3.2f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Donde cada uno aterriza al subir a su placa, el piso se vuelve de lo que
        // lo mata (lejos de la placa, para que esperar ahí sea seguro). Y las
        // puertas se cambian de lugar.
        HerramientasEscena.CrearPisoQueSeCalienta("Piso que se inunda", TipoZona.Agua, -11.6f, 0.8f, pisoUnoYungas, trampas);
        HerramientasEscena.CrearPisoQueSeCalienta("Piso que se calienta", TipoZona.Lava, 11.6f, 0.8f, pisoUnoYungas, trampas);

        string ruta = RutaNivel(4, 3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 4, Nivel 4: Persecución en el río =================
    // La crecida sube desde el principio y obliga a escalar en zigzag sin parar.
    // Aparecen las traiciones: un botón en el camino y una palanca necesaria.

    private static string GenerarMundo4Nivel4()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaTropico("¡El río crece! Suban en zigzag. La reja de arriba se abre con una palanca... y esa palanca no es de fiar.", out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;
        float pisoCuatro = 9.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloTropico);
        CrearPisoTropico("Piso 1", 3f, 16f, pisoUnoYungas, escenario);
        CrearPisoTropico("Piso 2", -3f, 16f, pisoDosYungas, escenario);
        CrearPisoTropico("Piso 3", 3f, 16f, pisoTresYungas, escenario);
        CrearPisoTropico("Piso 4", -3f, 16f, pisoCuatro, escenario);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        // TRAICIÓN: entre la subida y la gema de Killa. Ahí no hay techo y se salta bien.
        HerramientasEscena.CrearBotonTraicion("Boton del piso 1", new Vector2(7.6f, pisoUnoYungas), TipoCastigo.CaeLava, mecanismos);
        // TRAICIÓN: sin esta palanca no se abre la salida.
        PuertaMecanica reja = HerramientasEscena.CrearPuertaMecanica("Reja de la salida", new Vector2(-4f, 11.5f), new Vector2(0.5f, 4f), new Vector2(0f, 6f), 1, mecanismos);
        List<ObjetoActivable> abreReja = new List<ObjetoActivable>();
        abreReja.Add(reja);
        HerramientasEscena.CrearPalancaSospechosa("Palanca del piso 3", new Vector2(9f, pisoTresYungas), abreReja, TipoCastigo.SalenPuas, mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearSerpiente("Serpiente", -2f, 2f, pisoDosYungas, 2.5f, peligros);
        // En una rama alta, para que no estorbe al que camina por el piso 4.
        CrearPisoTropico("Rama del mono", 2f, 1.5f, 11.6f, escenario);
        HerramientasEscena.CrearMono("Mono", new Vector2(2f, 12.05f), 6f, 3.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-2f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(9.8f, 0.9f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(3.5f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(0f, 7.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(5f, 7.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-9.5f, pisoCuatro), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-7f, pisoCuatro), puertas);

        // Se detiene justo debajo del piso de las puertas.
        HerramientasEscena.CrearCrecida(pisoCuatro - 0.6f, 0.6f, 3f);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-11f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // Con el río subiendo: veneno que aparece donde se aterriza en los pisos 2
        // y 3, un bloque invisible en el salto natural al piso 4 (se rodea saltando
        // desde más a la derecha), y puertas que se cambian de lugar.
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearPisoQueSeCalienta("Piso 2 que se envenena", TipoZona.Veneno, 4f, 1.2f, pisoDosYungas, trampas);
        // DESAFÍO: un mono escondido en la copa se deja caer al paso.
        HerramientasEscena.CrearEnemigoQueCaeDelTecho("Mono que cae", new Vector2(-4f, pisoTresYungas + 2.8f), pisoTresYungas, new Color(0.72f, 0.48f, 0.26f), trampas);
        HerramientasEscena.CrearBloqueInvisible("Bloque escondido", new Vector2(5.8f, 8.3f), new Vector2(1f, 0.5f), HerramientasEscena.colorSueloTropico, escenario);

        // PSICOLÓGICA: un cartel de "META" sobre el piso 4, camino a las puertas.
        // Sus letras caen detrás de quien pasa: hay que seguir sin detenerse.
        HerramientasEscena.CrearCartelQueCae("Cartel de meta", "META", new Vector2(-1f, pisoCuatro + 3.4f), pisoCuatro, trampas);

        string ruta = RutaNivel(4, 4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 4, Nivel 5: El corazón del Chapare =================
    // Examen del mundo: hojas sobre el río, eclipse obligatorio, lianas con el
    // poder cambiado, serpiente, mono y traición. Un muro en la salida impide
    // saltar al otro lado para evitar el eclipse.

    private static string GenerarMundo4Nivel5()
    {
        Camera camaraPrincipal;
        Scene escena = PrepararEscenaTropico("El corazón del Chapare: hojas, placas y lianas. Sin eclipse no se llega, y no todo lo que brilla ayuda.", out camaraPrincipal);
        float suelo = HerramientasEscena.alturaSuelo;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSueloTropico);
        CrearPisoTropico("Rama izquierda", -10.5f, 9f, pisoDosYungas, escenario);
        CrearPisoTropico("Rama derecha", 10.5f, 9f, pisoDosYungas, escenario);
        CrearPisoTropico("Salida", 0f, 8f, pisoTresYungas, escenario);
        // Muro central: separa los dos lados de la salida.
        HerramientasEscena.CrearPlataforma("Muro central", new Vector2(0f, 8.25f), new Vector2(0.5f, 4f), HerramientasEscena.colorSueloTropico, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava de abajo", TipoZona.Lava, -9.5f, 11f, trampas);
        HerramientasEscena.CrearCharco("Agua de abajo", TipoZona.Agua, 9.5f, 11f, trampas);
        // Cada mitad de la salida solo se pisa con el poder que da el eclipse.
        HerramientasEscena.CrearCharco("Agua de la salida", TipoZona.Agua, -2.1f, 3.8f, pisoTresYungas, trampas);
        HerramientasEscena.CrearCharco("Lava de la salida", TipoZona.Lava, 2.1f, 3.8f, pisoTresYungas, trampas);

        Transform hojas = HerramientasEscena.CrearGrupo("Hojas");
        HerramientasEscena.CrearHojaTrampolin("Hoja izquierda", -5f, 1.2f, suelo, hojas);
        HerramientasEscena.CrearHojaTrampolin("Hoja derecha", 5f, 1.2f, suelo, hojas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        HerramientasEscena.CrearAltarEclipse("Altar", new Vector2(-13f, pisoDosYungas), new Vector2(13f, pisoDosYungas), colorAltarUno, mecanismos);
        // TRAICIÓN: en la entrada de cada lado de la salida. Quien se suelta tarde
        // de la liana cae justo encima y castiga a su compañero.
        HerramientasEscena.CrearBotonTraicion("Boton de la salida izquierdo", new Vector2(-3.4f, pisoTresYungas), TipoCastigo.SalenPuas, mecanismos);
        HerramientasEscena.CrearBotonTraicion("Boton de la salida derecho", new Vector2(3.4f, pisoTresYungas), TipoCastigo.SalenPuas, mecanismos);

        Transform lianas = HerramientasEscena.CrearGrupo("Lianas");
        HerramientasEscena.CrearLiana("Liana izquierda", new Vector2(-7.6f, 11.5f), 6f, 3.6f, 0f, lianas);
        HerramientasEscena.CrearLiana("Liana derecha", new Vector2(7.6f, 11.5f), 6f, 3.6f, 0.5f, lianas);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearSerpiente("Serpiente", -2f, 2f, suelo, 2f, peligros);
        HerramientasEscena.CrearMono("Mono", new Vector2(0f, 10.75f), 6f, 3.5f, peligros);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-4.2f, 1.5f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(4.2f, 1.5f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-10f, 4.15f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(10f, 4.15f), gemas);
        // Se recogen después del eclipse, cuando los poderes están cambiados.
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(-2f, 6.9f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(2f, 6.9f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-0.8f, pisoTresYungas), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(0.8f, pisoTresYungas), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-3.4f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(3.4f, suelo));
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        // ===== VERSIÓN DIFÍCIL =====
        // De cada rama sale un enemigo del suelo: se salta cuando la liana está
        // lejos, para no quedar colgado en medio del salto.
        // (Las puertas no se cambian: el muro central lo haría imposible.)
        HerramientasEscena.CrearEnemigoSorpresa("Enemigo de la rama izquierda", new Vector2(-10.5f, pisoDosYungas), trampas);
        HerramientasEscena.CrearEnemigoSorpresa("Enemigo de la rama derecha", new Vector2(10.5f, pisoDosYungas), trampas);

        string ruta = RutaNivel(4, 5);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
