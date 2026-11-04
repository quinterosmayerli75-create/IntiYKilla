// =====================================================================
// GeneradorEscenas.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR que arma todas las escenas del juego con
//              un solo clic, usando las piezas de HerramientasEscena:
//              - MenuPrincipal
//              - SeleccionNivel (mapa con los 20 niveles por mundo)
//              - Nivel01, Nivel02, Nivel03 (Mundo 1: Salar de Uyuni)
//              - Nivel04 (Mundo 2: Minas de Potosí)
//              Además ordena las escenas en Build Settings.
// Uso: Menú superior de Unity -> Inti y Killa -> Generar todas las escenas
// Ojo: volver a generar una escena la REEMPLAZA (se pierden los cambios
//      hechos a mano en esa escena).
// =====================================================================

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class GeneradorEscenas
{
    private static readonly Color colorCieloFondo = new Color(0.40f, 0.28f, 0.55f);
    private static readonly Color colorBotonPrincipal = new Color(0.95f, 0.50f, 0.15f);
    private static readonly Color colorBotonSecundario = new Color(0.42f, 0.25f, 0.62f);
    private static readonly Color colorBotonSalir = new Color(0.30f, 0.26f, 0.36f);
    private static readonly Color colorAltarUno = new Color(0.95f, 0.80f, 0.35f);
    private static readonly Color colorAltarDos = new Color(0.70f, 0.85f, 1.00f);

    // ================= Menús de Unity =================

    [MenuItem("Inti y Killa/Generar todas las escenas", false, 1)]
    public static void GenerarTodas()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
        {
            return;
        }

        HerramientasEscena.PrepararRecursos();

        GenerarMenuPrincipal();
        GenerarSeleccionNivel();
        GenerarNivel01();
        GenerarNivel02();
        GenerarNivel03();
        GenerarNivel04();

        HerramientasEscena.ActualizarBuildSettings();

        // Al terminar, dejamos abierto el menú principal para darle Play.
        EditorSceneManager.OpenScene(RutaEscena(DatosJuego.escenaMenuPrincipal));
        Debug.Log("Inti y Killa: escenas generadas (Menú, Selección de nivel y Niveles 1 a 4). ¡Dale Play!");
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Menú principal", false, 20)]
    public static void MenuGenerarMenuPrincipal()
    {
        GenerarUnaSola(1);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Selección de nivel", false, 21)]
    public static void MenuGenerarSeleccion()
    {
        GenerarUnaSola(2);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Nivel 1", false, 22)]
    public static void MenuGenerarNivel01()
    {
        GenerarUnaSola(3);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Nivel 2", false, 23)]
    public static void MenuGenerarNivel02()
    {
        GenerarUnaSola(4);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Nivel 3", false, 24)]
    public static void MenuGenerarNivel03()
    {
        GenerarUnaSola(5);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Nivel 4", false, 25)]
    public static void MenuGenerarNivel04()
    {
        GenerarUnaSola(6);
    }

    private static void GenerarUnaSola(int cual)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
        {
            return;
        }

        HerramientasEscena.PrepararRecursos();
        string rutaGenerada = "";

        if (cual == 1)
        {
            rutaGenerada = GenerarMenuPrincipal();
        }
        else if (cual == 2)
        {
            rutaGenerada = GenerarSeleccionNivel();
        }
        else if (cual == 3)
        {
            rutaGenerada = GenerarNivel01();
        }
        else if (cual == 4)
        {
            rutaGenerada = GenerarNivel02();
        }
        else if (cual == 5)
        {
            rutaGenerada = GenerarNivel03();
        }
        else if (cual == 6)
        {
            rutaGenerada = GenerarNivel04();
        }

        HerramientasEscena.ActualizarBuildSettings();
        EditorSceneManager.OpenScene(rutaGenerada);
    }

    // ================= Rutas =================

    private static string RutaEscena(string nombre)
    {
        return HerramientasEscena.carpetaEscenas + "/" + nombre + ".unity";
    }

    private static string RutaNivel(int numero)
    {
        DatosNivel datos = DatosJuego.ObtenerNivel(numero);
        return HerramientasEscena.carpetaNiveles + "/" + datos.nombreEscena + ".unity";
    }

    private static Scene CrearEscenaVacia()
    {
        return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
    }

    // ================= Menú principal =================

    private static string GenerarMenuPrincipal()
    {
        HerramientasEscena.usarTilesetSal = true;
        Scene escena = CrearEscenaVacia();

        HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearFondoUyuni(0f);

        // Suelo de sal y los dos protagonistas como decoración.
        Transform decoracion = HerramientasEscena.CrearGrupo("Decoracion");
        HerramientasEscena.CrearSuelo(HerramientasEscena.colorSal, decoracion);
        HerramientasEscena.CrearFiguraPersonaje("Inti", HerramientasEscena.imagenInti, ColoresElemento.colorFuego, new Vector2(-1.2f, HerramientasEscena.alturaSuelo + 0.6f), decoracion);
        HerramientasEscena.CrearFiguraPersonaje("Killa", HerramientasEscena.imagenKilla, ColoresElemento.colorAgua, new Vector2(1.2f, HerramientasEscena.alturaSuelo + 0.6f), decoracion);

        // ----- Interfaz -----
        Transform canvas = HerramientasEscena.CrearCanvas();
        HerramientasEscena.CrearEventSystem();

        HerramientasEscena.CrearTexto(canvas, "Titulo", "INTI y KILLA", 130, new Vector2(0f, 340f), new Vector2(1400f, 160f), new Color(1f, 0.92f, 0.70f));
        HerramientasEscena.CrearTexto(canvas, "Subtitulo", "Un viaje cooperativo por Bolivia", 42, new Vector2(0f, 235f), new Vector2(1400f, 60f), Color.white);

        GameObject objetoMenu = new GameObject("MenuPrincipal");
        MenuPrincipal menu = objetoMenu.AddComponent<MenuPrincipal>();

        Button botonJugar = HerramientasEscena.CrearBoton(canvas, "Boton Jugar", "JUGAR", new Vector2(0f, 60f), new Vector2(440f, 100f), colorBotonPrincipal, 48);
        Button botonBorrar = HerramientasEscena.CrearBoton(canvas, "Boton Borrar progreso", "Borrar progreso", new Vector2(0f, -60f), new Vector2(440f, 80f), colorBotonSecundario, 34);
        Button botonSalir = HerramientasEscena.CrearBoton(canvas, "Boton Salir", "Salir", new Vector2(0f, -160f), new Vector2(440f, 80f), colorBotonSalir, 34);

        // Conexiones guardadas en la escena (se ven en el Inspector, sección On Click).
        UnityEventTools.AddPersistentListener(botonJugar.onClick, new UnityAction(menu.Jugar));
        UnityEventTools.AddPersistentListener(botonBorrar.onClick, new UnityAction(menu.BorrarProgreso));
        UnityEventTools.AddPersistentListener(botonSalir.onClick, new UnityAction(menu.Salir));

        Text textoMensaje = HerramientasEscena.CrearTexto(canvas, "Mensaje", "", 28, new Vector2(0f, -245f), new Vector2(1200f, 50f), new Color(1f, 1f, 0.8f));
        menu.Configurar(textoMensaje);

        string textoControles = "2 jugadores en el mismo teclado     Inti: A D W + S     Killa: Flechas + Flecha abajo";
        HerramientasEscena.CrearTexto(canvas, "Controles", textoControles, 26, new Vector2(0f, -490f), new Vector2(1800f, 50f), new Color(1f, 1f, 1f, 0.9f));

        string ruta = RutaEscena(DatosJuego.escenaMenuPrincipal);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Selección de nivel =================

    private static string GenerarSeleccionNivel()
    {
        Scene escena = CrearEscenaVacia();

        HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(0.75f, 0.70f, 0.85f), 1f);
        HerramientasEscena.CrearFondoUyuni(9f);

        Transform canvas = HerramientasEscena.CrearCanvas();
        HerramientasEscena.CrearEventSystem();

        GameObject objetoSeleccion = new GameObject("SeleccionNivel");
        SeleccionNivel seleccion = objetoSeleccion.AddComponent<SeleccionNivel>();

        HerramientasEscena.CrearTexto(canvas, "Titulo", "Elige un nivel", 72, new Vector2(0f, 455f), new Vector2(1200f, 100f), new Color(1f, 0.92f, 0.70f));

        Button botonVolver = HerramientasEscena.CrearBoton(canvas, "Boton Volver", "< Volver", new Vector2(-780f, 455f), new Vector2(240f, 70f), colorBotonSalir, 30);
        UnityEventTools.AddPersistentListener(botonVolver.onClick, new UnityAction(seleccion.VolverAlMenu));

        // Una fila por mundo, con sus niveles a la derecha.
        float posicionYFila = 310f;
        for (int numeroMundo = 1; numeroMundo <= DatosJuego.nombresMundos.Length; numeroMundo++)
        {
            string textoMundo = "Mundo " + numeroMundo + ": " + DatosJuego.ObtenerNombreMundo(numeroMundo);
            Text etiqueta = HerramientasEscena.CrearTexto(canvas, "Etiqueta Mundo " + numeroMundo, textoMundo, 34, new Vector2(-560f, posicionYFila), new Vector2(520f, 100f), Color.white);
            etiqueta.alignment = TextAnchor.MiddleRight;

            float posicionXBoton = -170f;
            foreach (DatosNivel nivel in DatosJuego.ObtenerTodosLosNiveles())
            {
                if (nivel.numeroMundo == numeroMundo)
                {
                    CrearBotonDeNivel(canvas, nivel, new Vector2(posicionXBoton, posicionYFila));
                    posicionXBoton = posicionXBoton + 200f;
                }
            }

            posicionYFila = posicionYFila - 125f;
        }

        string textoAyuda = "Esc: volver al menú     F1: desbloquear todos los niveles (modo prueba)";
        HerramientasEscena.CrearTexto(canvas, "Ayuda", textoAyuda, 24, new Vector2(0f, -500f), new Vector2(1600f, 40f), new Color(1f, 1f, 1f, 0.85f));

        string ruta = RutaEscena(DatosJuego.escenaSeleccionNivel);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    private static void CrearBotonDeNivel(Transform canvas, DatosNivel nivel, Vector2 posicion)
    {
        Button boton = HerramientasEscena.CrearBoton(canvas, "Boton Nivel " + nivel.numero, nivel.numero.ToString(), posicion, new Vector2(180f, 105f), colorBotonSecundario, 44);

        // El número va arriba y el estado (estrellas / bloqueado) abajo.
        RectTransform rectanguloNumero = boton.transform.Find("Texto").GetComponent<RectTransform>();
        rectanguloNumero.anchoredPosition = new Vector2(0f, 16f);

        Text textoEstado = HerramientasEscena.CrearTexto(boton.transform, "Estado", "", 24, new Vector2(0f, -30f), new Vector2(180f, 30f), new Color(1f, 0.90f, 0.45f));
        textoEstado.raycastTarget = false;

        BotonNivel botonNivel = boton.gameObject.AddComponent<BotonNivel>();
        botonNivel.Configurar(nivel.numero, textoEstado);
    }

    // ================= Nivel 1: Primeros pasos =================
    // Enseña: moverse, saltar, gemas, lava/agua, púas y un bloque que se rompe.
    // No es un pasillo: son tres pisos que se suben en zigzag. Empiezan juntos en
    // el centro y cada uno se va por su lado, porque el camino del otro lo mata.
    // Se reencuentran en el piso central y ahí se CRUZAN: la puerta de cada uno
    // está arriba, en el lado contrario al que subió.

    private static string GenerarNivel01()
    {
        HerramientasEscena.usarTilesetSal = true;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearGestorNivel("Suban hasta su puerta. Inti va por la izquierda y Killa por la derecha, y arriba se cruzan.");
        HerramientasEscena.CrearFondoUyuni(0f);

        // Altura de cada piso: separados 3.25 unidades, un poco menos que el salto (3.8).
        float pisoUno = -0.25f;
        float pisoDos = 3f;
        float pisoTres = 6.25f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        HerramientasEscena.CrearPlataforma("Piso 1 izquierda", new Vector2(-8f, pisoUno - mitadGrosor), new Vector2(6f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 1 derecha", new Vector2(8f, pisoUno - mitadGrosor), new Vector2(6f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2 central", new Vector2(0f, pisoDos - mitadGrosor), new Vector2(6f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 izquierda", new Vector2(-9f, pisoTres - mitadGrosor), new Vector2(8f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 derecha", new Vector2(9f, pisoTres - mitadGrosor), new Vector2(8f, grosor), HerramientasEscena.colorSal, escenario);

        // Premio arriesgado: dos gemas sobre un bloque que se rompe, justo encima
        // del veneno. Quien se suba tiene que saltar enseguida o cae al charco.
        HerramientasEscena.CrearPlataformaQuebradiza("Bloque que se rompe", new Vector2(0f, pisoTres - mitadGrosor), new Vector2(2f, grosor), escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        // Planta baja: el lado izquierdo es de Inti y el derecho es de Killa.
        HerramientasEscena.CrearCharco("Lava de la entrada", TipoZona.Lava, -5.5f, 5f, trampas);
        HerramientasEscena.CrearCharco("Agua de la entrada", TipoZona.Agua, 5.5f, 5f, trampas);
        // Piso 1: cada uno tiene que saltar lo que al otro no le hace daño.
        HerramientasEscena.CrearCharco("Agua del piso 1", TipoZona.Agua, -8f, 2f, pisoUno, trampas);
        HerramientasEscena.CrearCharco("Lava del piso 1", TipoZona.Lava, 8f, 2f, pisoUno, trampas);
        // Piso 2: lugar del cruce, con veneno que mata a los dos.
        HerramientasEscena.CrearCharco("Veneno del cruce", TipoZona.Veneno, 0f, 2f, pisoDos, trampas);
        // Piso 3: la última prueba antes de cada puerta.
        HerramientasEscena.CrearCristalesDeSal("Cristales izquierda", -8.5f, 2f, pisoTres, trampas);
        HerramientasEscena.CrearCristalesDeSal("Cristales derecha", 8.5f, 2f, pisoTres, trampas);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-5.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(5.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-0.6f, 7.2f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(0.6f, 7.2f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(8.5f, 8.6f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(-8.5f, 8.6f), gemas);

        // Cada puerta está en el lado contrario al que subió cada personaje.
        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(11.5f, pisoTres), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-11.5f, pisoTres), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-1.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(1.5f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Nivel 2: Botones y compuertas =================
    // Enseña a turnarse: nadie avanza solo. Cada uno empieza en su lado de un muro
    // y el botón que abre la compuerta de uno está en el lado del otro.
    // Arriba, la salida solo se abre cuando LOS DOS encendieron su palanca.

    private static string GenerarNivel02()
    {
        HerramientasEscena.usarTilesetSal = true;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearGestorNivel("El botón de cada uno abre la compuerta del otro. Arriba, los dos deben encender su palanca (S o Flecha abajo).");
        HerramientasEscena.CrearFondoUyuni(0f);

        float pisoUno = -0.25f;
        float pisoDos = 3f;
        float pisoTres = 6.25f;
        float pisoSalida = 9.5f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        // El muro central separa a los dos jugadores hasta el piso 2.
        HerramientasEscena.CrearMuro("Muro central", 0f, suelo, pisoDos - grosor, HerramientasEscena.colorSal, escenario);
        // El piso 1 llega hasta la pared: solo se sube por el hueco cercano al muro,
        // que queda DETRÁS de la compuerta.
        HerramientasEscena.CrearPlataforma("Piso 1 izquierda", new Vector2(-10f, pisoUno - mitadGrosor), new Vector2(10f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 1 derecha", new Vector2(10f, pisoUno - mitadGrosor), new Vector2(10f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2 central", new Vector2(0f, pisoDos - mitadGrosor), new Vector2(7f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 izquierda", new Vector2(-9f, pisoTres - mitadGrosor), new Vector2(8f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 derecha", new Vector2(9f, pisoTres - mitadGrosor), new Vector2(8f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso de salida", new Vector2(0f, pisoSalida - mitadGrosor), new Vector2(7f, grosor), HerramientasEscena.colorSal, escenario);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");

        // Compuertas de la planta baja: se abren hundiéndose en el suelo, porque
        // subiendo atravesarían el piso 1 y estorbarían arriba.
        Vector2 tamanoCompuerta = new Vector2(0.7f, 2.75f);
        Vector2 hundirse = new Vector2(0f, -2.85f);
        float centroCompuertaY = suelo + tamanoCompuerta.y / 2f;
        PuertaMecanica compuertaInti = HerramientasEscena.CrearPuertaMecanica("Compuerta de Inti", new Vector2(-8f, centroCompuertaY), tamanoCompuerta, hundirse, 1, mecanismos);
        PuertaMecanica compuertaKilla = HerramientasEscena.CrearPuertaMecanica("Compuerta de Killa", new Vector2(8f, centroCompuertaY), tamanoCompuerta, hundirse, 1, mecanismos);
        compuertaInti.GetComponent<SpriteRenderer>().sortingOrder = -1;
        compuertaKilla.GetComponent<SpriteRenderer>().sortingOrder = -1;

        // Killa abre desde abajo el paso de Inti; Inti, ya en el piso 1, abre el de Killa.
        List<ObjetoActivable> abreCompuertaInti = new List<ObjetoActivable>();
        abreCompuertaInti.Add(compuertaInti);
        HerramientasEscena.CrearBotonPresion("Boton de Killa", new Vector2(11.5f, suelo), abreCompuertaInti, mecanismos);

        List<ObjetoActivable> abreCompuertaKilla = new List<ObjetoActivable>();
        abreCompuertaKilla.Add(compuertaKilla);
        HerramientasEscena.CrearBotonPresion("Boton de Inti", new Vector2(-10f, pisoUno), abreCompuertaKilla, mecanismos);

        // Salida: dos compuertas altas, cada una a un lado del piso de salida. Son
        // tan altas que no se pueden saltar, y necesitan las DOS palancas encendidas.
        Vector2 tamanoCierre = new Vector2(0.7f, 4.5f);
        Vector2 subir = new Vector2(0f, 4.6f);
        float centroCierreY = pisoSalida + tamanoCierre.y / 2f;
        PuertaMecanica cierreIzquierdo = HerramientasEscena.CrearPuertaMecanica("Cierre izquierdo", new Vector2(-3.15f, centroCierreY), tamanoCierre, subir, 2, mecanismos);
        PuertaMecanica cierreDerecho = HerramientasEscena.CrearPuertaMecanica("Cierre derecho", new Vector2(3.15f, centroCierreY), tamanoCierre, subir, 2, mecanismos);

        List<ObjetoActivable> abreSalida = new List<ObjetoActivable>();
        abreSalida.Add(cierreIzquierdo);
        abreSalida.Add(cierreDerecho);
        HerramientasEscena.CrearPalanca("Palanca izquierda", new Vector2(-11.5f, pisoTres), abreSalida, mecanismos);
        HerramientasEscena.CrearPalanca("Palanca derecha", new Vector2(11.5f, pisoTres), abreSalida, mecanismos);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        // Bajo techo el salto es corto: estos charcos piden un salto preciso.
        HerramientasEscena.CrearCharco("Agua de la planta baja", TipoZona.Agua, -6f, 1.4f, trampas);
        HerramientasEscena.CrearCharco("Lava de la planta baja", TipoZona.Lava, 6f, 1.4f, trampas);
        HerramientasEscena.CrearCharco("Lava del piso 3", TipoZona.Lava, -9f, 3f, pisoTres, trampas);
        HerramientasEscena.CrearCharco("Agua del piso 3", TipoZona.Agua, 9f, 3f, pisoTres, trampas);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-6f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(6f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-9f, 6.75f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(9f, 6.75f), gemas);

        // Cada puerta queda del lado contrario: también se cruzan al final.
        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(1.2f, pisoSalida), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-1.2f, pisoSalida), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-14f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(14f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Nivel 3: El primer eclipse =================
    // Cuatro pisos que se suben en zigzag. Tres ríos imposibles de saltar: lava,
    // agua y lava. Quien puede cruzar va primero y se para en la placa del otro
    // lado; el eclipse intercambia los poderes y entonces cruza el compañero.
    // Después de tres eclipses los poderes quedan cambiados: cada uno entra por
    // la puerta del poder que tiene en ese momento.

    private static string GenerarNivel03()
    {
        HerramientasEscena.usarTilesetSal = true;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(0.95f, 0.85f, 0.90f), 1f);
        HerramientasEscena.CrearGestorNivel("Párense cada uno en una placa del mismo color para provocar un ECLIPSE. Al final, cada uno entra por la puerta del poder que tenga.");
        HerramientasEscena.CrearFondoUyuni(0f);

        float pisoUno = -0.25f;
        float pisoDos = 3f;
        float pisoTres = 6.25f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        // Cada piso deja un solo hueco para subir, alternando de lado (el zigzag).
        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        HerramientasEscena.CrearPlataforma("Piso 1", new Vector2(-1.5f, pisoUno - mitadGrosor), new Vector2(27f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2", new Vector2(2f, pisoDos - mitadGrosor), new Vector2(26f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3", new Vector2(-1.75f, pisoTres - mitadGrosor), new Vector2(26.5f, grosor), HerramientasEscena.colorSal, escenario);

        // Ríos de 16 unidades: demasiado anchos para saltarlos.
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de lava de abajo", TipoZona.Lava, 0f, 16f, trampas);
        HerramientasEscena.CrearCharco("Rio de agua", TipoZona.Agua, 0f, 16f, pisoUno, trampas);
        HerramientasEscena.CrearCharco("Rio de lava de arriba", TipoZona.Lava, 0f, 16f, pisoDos, trampas);
        // Último piso: prueba de saltos con los poderes ya cambiados.
        HerramientasEscena.CrearCharco("Agua final", TipoZona.Agua, 6f, 2f, pisoTres, trampas);
        HerramientasEscena.CrearCharco("Lava final", TipoZona.Lava, 2f, 2f, pisoTres, trampas);
        HerramientasEscena.CrearCharco("Veneno final", TipoZona.Veneno, -2f, 2f, pisoTres, trampas);
        HerramientasEscena.CrearCristalesDeSal("Cristales finales", -6f, 2f, pisoTres, trampas);

        Transform altares = HerramientasEscena.CrearGrupo("Altares");
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 1", new Vector2(-12f, suelo), new Vector2(12f, suelo), colorAltarUno, altares);
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 2", new Vector2(10f, pisoUno), new Vector2(-10f, pisoUno), colorAltarDos, altares);
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 3", new Vector2(-9.5f, pisoDos), new Vector2(10f, pisoDos), colorAltarUno, altares);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(0f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(0f, 0.85f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(0f, 4.1f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(-6f, 8.6f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-2f, 8.6f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-10f, pisoTres), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-12.5f, pisoTres), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-10.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-13.5f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Nivel 4: La luz de Inti (Minas de Potosí) =================
    // La mina está a oscuras y se recorre subiendo. Los cristales solo se pisan
    // dentro de la luz de Inti, así que él alumbra el camino de Killa:
    // 1) Cruzan juntos el veneno por cristales, con Inti delante manteniendo S.
    // 2) Killa sube sobre la lava mientras Inti alumbra desde abajo. El último
    //    escalón está fuera de la luz normal: Inti tiene que mantener S.
    // 3) Killa, ya arriba, enciende la palanca que abre el paso de Inti.
    // 4) En el piso alto cruzan otro río de veneno, y otra vez dependen de la luz.

    private static readonly Color colorFondoMina = new Color(0.07f, 0.05f, 0.05f);
    private static readonly Color colorSueloMina = new Color(0.40f, 0.32f, 0.28f);

    private static string GenerarNivel04()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoMina);
        // Luz ambiental muy baja: lo que no toque una luz queda casi negro.
        HerramientasEscena.CrearLuzGlobal(new Color(0.55f, 0.60f, 0.85f), 0.18f);
        HerramientasEscena.CrearGestorNivel("La mina está oscura. Los cristales solo se pisan con la luz de Inti. Mantén S para iluminar más lejos.");
        HerramientasEscena.CrearFondoMina();

        float pisoMedio = 1.2f;
        float pisoAlto = 4.45f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(colorSueloMina);
        HerramientasEscena.CrearPlataforma("Repisa media", new Vector2(11.25f, pisoMedio - mitadGrosor), new Vector2(2.5f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso alto", new Vector2(-3f, pisoAlto - mitadGrosor), new Vector2(24f, grosor), colorSueloMina, escenario);

        // Faroles fijos: marcan el inicio, el descanso del medio y la salida.
        Transform faroles = HerramientasEscena.CrearGrupo("Faroles");
        HerramientasEscena.CrearFarol("Farol de la entrada", new Vector2(-13f, 1.5f), 4f, faroles);
        HerramientasEscena.CrearFarol("Farol del medio", new Vector2(0.5f, 1.5f), 3.5f, faroles);
        HerramientasEscena.CrearFarol("Farol de la salida", new Vector2(-12f, 8.5f), 4f, faroles);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de veneno", TipoZona.Veneno, -6f, 8f, trampas);
        HerramientasEscena.CrearCharco("Lago de lava", TipoZona.Lava, 6.5f, 7f, trampas);
        HerramientasEscena.CrearCharco("Rio de veneno alto", TipoZona.Veneno, -2f, 8f, pisoAlto, trampas);

        Transform cristales = HerramientasEscena.CrearGrupo("Cristales de luz");
        Vector2 tamanoCristal = new Vector2(1.4f, 0.4f);
        HerramientasEscena.CrearPlataformaOculta("Cristal veneno 1", new Vector2(-8.5f, -2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal veneno 2", new Vector2(-6f, -2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal veneno 3", new Vector2(-3.5f, -2f), tamanoCristal, cristales);

        // Escalera sobre la lava. El escalón 3 está a más de 3.5 unidades de Inti
        // (en el suelo): solo aparece si mantiene S y la luz crece a 5.5.
        Vector2 tamanoEscalon = new Vector2(1.6f, 0.4f);
        HerramientasEscena.CrearPlataformaOculta("Escalon de cristal 1", new Vector2(4.5f, -2f), tamanoEscalon, cristales);
        HerramientasEscena.CrearPlataformaOculta("Escalon de cristal 2", new Vector2(6.5f, -0.2f), tamanoEscalon, cristales);
        HerramientasEscena.CrearPlataformaOculta("Escalon de cristal 3", new Vector2(8.5f, 1f), tamanoEscalon, cristales);

        // Pozo por donde sube Inti cuando Killa le abre la compuerta.
        HerramientasEscena.CrearPlataformaOculta("Cristal del pozo", new Vector2(14f, -1.5f), tamanoEscalon, cristales);

        // Cristales sobre el río alto.
        float alturaCristalAlto = pisoAlto + 1.5f;
        HerramientasEscena.CrearPlataformaOculta("Cristal alto 1", new Vector2(0f, alturaCristalAlto), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal alto 2", new Vector2(-2f, alturaCristalAlto), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal alto 3", new Vector2(-4f, alturaCristalAlto), tamanoCristal, cristales);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        // Llega hasta la repisa, para que Inti no pueda pasar por encima. Se hunde al abrir.
        Vector2 tamanoCompuerta = new Vector2(0.7f, pisoMedio - grosor - suelo);
        PuertaMecanica compuerta = HerramientasEscena.CrearPuertaMecanica("Compuerta del pozo", new Vector2(10.65f, suelo + tamanoCompuerta.y / 2f), tamanoCompuerta, new Vector2(0f, -tamanoCompuerta.y - 0.1f), 1, mecanismos);
        compuerta.GetComponent<SpriteRenderer>().sortingOrder = -1;

        List<ObjetoActivable> abreCompuerta = new List<ObjetoActivable>();
        abreCompuerta.Add(compuerta);
        HerramientasEscena.CrearPalanca("Palanca de la repisa", new Vector2(11f, pisoMedio), abreCompuerta, mecanismos);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(6.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-2f, 6.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(6.5f, 0.7f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(11.9f, 2.1f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-13.2f, pisoAlto), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-10.5f, pisoAlto), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-11.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-13.5f, suelo));

        // Los dos llevan la luz; solo se enciende en quien tenga el fuego.
        HerramientasEscena.AgregarLuzPersonaje(inti);
        HerramientasEscena.AgregarLuzPersonaje(killa);

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
