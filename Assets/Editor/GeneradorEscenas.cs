// =====================================================================
// GeneradorEscenas.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR que arma todas las escenas del juego con
//              un solo clic, usando las piezas de HerramientasEscena:
//              - MenuPrincipal
//              - SeleccionNivel (mapa con los 20 niveles por mundo)
//              - Nivel01, Nivel02, Nivel03 (Mundo 1: Salar de Uyuni)
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

        HerramientasEscena.ActualizarBuildSettings();

        // Al terminar, dejamos abierto el menú principal para darle Play.
        EditorSceneManager.OpenScene(RutaEscena(DatosJuego.escenaMenuPrincipal));
        Debug.Log("Inti y Killa: escenas generadas (Menú, Selección de nivel y Niveles 1 a 3). ¡Dale Play!");
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
        Scene escena = CrearEscenaVacia();

        HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearFondoUyuni(0f);

        // Suelo de sal y los dos protagonistas como decoración.
        Transform decoracion = HerramientasEscena.CrearGrupo("Decoracion");
        HerramientasEscena.CrearBloque("Suelo de sal", new Vector2(0f, HerramientasEscena.alturaSuelo - 0.5f), new Vector2(32f, 1f), HerramientasEscena.colorSal, HerramientasEscena.imagenSuelo, decoracion);
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

    private static string GenerarNivel01()
    {
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearGestorNivel("Lleguen cada uno a su puerta. Inti no toca el agua y Killa no toca la lava.");
        HerramientasEscena.CrearFondoUyuni(5f);

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        HerramientasEscena.CrearPlataforma("Bloque de sal A", new Vector2(-6.5f, -1.3f), new Vector2(3f, 0.5f), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataformaQuebradiza("Bloque que se rompe", new Vector2(-2f, 0.6f), new Vector2(2.5f, 0.5f), escenario);
        HerramientasEscena.CrearPlataforma("Bloque de sal B", new Vector2(3.5f, 1.4f), new Vector2(4f, 0.5f), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Bloque de sal C", new Vector2(9f, -1.2f), new Vector2(3f, 0.5f), HerramientasEscena.colorSal, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava", TipoZona.Lava, -8.5f, 2.5f, trampas);
        HerramientasEscena.CrearCharco("Agua", TipoZona.Agua, -4f, 2.5f, trampas);
        HerramientasEscena.CrearCristalesDeSal("Cristales de sal", 0.5f, 2f, trampas);
        HerramientasEscena.CrearCharco("Veneno", TipoZona.Veneno, 4f, 3f, trampas);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-8.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-4f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(0.5f, -1.6f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-2f, 1.6f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(3.5f, 2.6f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(9f, -0.2f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(12.2f, suelo), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(14.2f, suelo), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-12f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Nivel 2: Botones y compuertas =================
    // Enseña: uno sostiene un botón para que el otro pase, palanca y elevador.

    private static string GenerarNivel02()
    {
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearGestorNivel("Uno pisa el botón para que el otro pase. La palanca se usa con S o Flecha abajo.");
        HerramientasEscena.CrearFondoUyuni(-6f);

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        // Repisa alta a la derecha, donde están las puertas de salida.
        HerramientasEscena.CrearPlataforma("Repisa de salida", new Vector2(12.75f, 1.5f), new Vector2(5.5f, 0.5f), HerramientasEscena.colorSal, escenario);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");

        // Compuerta en el centro: la abren DOS botones (uno a cada lado).
        PuertaMecanica compuerta = HerramientasEscena.CrearPuertaMecanica("Compuerta central", new Vector2(0f, -1.25f), new Vector2(0.7f, 4.5f), new Vector2(0f, 4.6f), 1, mecanismos);
        List<ObjetoActivable> objetivosCompuerta = new List<ObjetoActivable>();
        objetivosCompuerta.Add(compuerta);
        HerramientasEscena.CrearBotonPresion("Boton izquierdo", new Vector2(-5f, suelo), objetivosCompuerta, mecanismos);
        HerramientasEscena.CrearBotonPresion("Boton derecho", new Vector2(4.5f, suelo), objetivosCompuerta, mecanismos);

        // Elevador que sube a la repisa: se enciende con la palanca.
        PlataformaMovil elevador = HerramientasEscena.CrearPlataformaMovil("Elevador", new Vector2(8.5f, -2.9f), new Vector2(2f, 0.4f), new Vector2(0f, 4.65f), 2.5f, false, mecanismos);
        List<ObjetoActivable> objetivosPalanca = new List<ObjetoActivable>();
        objetivosPalanca.Add(elevador);
        HerramientasEscena.CrearPalanca("Palanca del elevador", new Vector2(6.4f, suelo), objetivosPalanca, mecanismos);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Lava", TipoZona.Lava, -9f, 2.5f, trampas);
        HerramientasEscena.CrearCharco("Agua", TipoZona.Agua, 2f, 1.8f, trampas);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-9f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(2f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(8.5f, 3.7f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(14.6f, 2.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(11.4f, 1.75f), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(13.2f, 1.75f), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-12f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Nivel 3: El primer eclipse =================
    // Dos ríos imposibles de saltar: uno de lava y otro de agua.
    // Solo se cruzan usando los altares para intercambiar poderes.

    private static string GenerarNivel03()
    {
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(0.95f, 0.85f, 0.90f), 1f);
        HerramientasEscena.CrearGestorNivel("Párense cada uno en una placa del mismo altar a la vez para provocar un ECLIPSE.");
        HerramientasEscena.CrearFondoUyuni(1.75f);

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        // Plataforma en la isla del medio y repisa de salida al final.
        HerramientasEscena.CrearPlataforma("Plataforma de la isla", new Vector2(1.75f, -1.2f), new Vector2(2.5f, 0.5f), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Repisa de salida", new Vector2(13.5f, -1.0f), new Vector2(3f, 0.5f), HerramientasEscena.colorSal, escenario);

        // Ríos de 7.5 unidades: demasiado anchos para saltarlos.
        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de lava", TipoZona.Lava, -4.75f, 7.5f, trampas);
        HerramientasEscena.CrearCharco("Rio de agua", TipoZona.Agua, 8.25f, 7.5f, trampas);

        // Altar 1: una placa antes del río de lava y otra en la isla.
        // Altar 2: una placa en la isla y otra después del río de agua.
        Transform altares = HerramientasEscena.CrearGrupo("Altares");
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 1", new Vector2(-10.5f, suelo), new Vector2(0.3f, suelo), colorAltarUno, altares);
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 2", new Vector2(3.2f, suelo), new Vector2(13.5f, suelo), colorAltarDos, altares);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-4.75f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(1.75f, -0.1f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(8.25f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(13.55f, 0.2f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(12.8f, -0.75f), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(14.3f, -0.75f), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13.8f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-12.4f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
