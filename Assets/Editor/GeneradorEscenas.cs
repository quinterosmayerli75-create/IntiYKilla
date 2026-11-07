// =====================================================================
// GeneradorEscenas.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR que arma todas las escenas del juego con
//              un solo clic, usando las piezas de HerramientasEscena:
//              - MenuPrincipal
//              - SeleccionMundo (los 6 mundos)
//              - SeleccionNivel (los 5 niveles del mundo elegido)
//              - Mundo1_Nivel1 a Mundo1_Nivel5 (Salar de Uyuni)
//              - Mundo2_Nivel1 a Mundo2_Nivel5 (Minas de Potosí)
//              - Mundo3_Nivel1 a Mundo3_Nivel5 (Yungas), en GeneradorEscenasMundo3
//              - Mundo4_Nivel1 a Mundo4_Nivel5 (Trópico), en GeneradorEscenasMundo4
//              - Mundos 5 y 6, en GeneradorEscenasMundo5 y GeneradorEscenasMundo6
//              - Intro y Final, en GeneradorEscenasHistoria
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

public static partial class GeneradorEscenas
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
        GenerarSeleccionMundo();
        GenerarSeleccionNivel();
        GenerarMundo1Nivel1();
        GenerarMundo1Nivel2();
        GenerarMundo1Nivel3();
        GenerarMundo1Nivel4();
        GenerarMundo1Nivel5();
        GenerarMundo2Nivel1();
        GenerarMundo2Nivel2();
        GenerarMundo2Nivel3();
        GenerarMundo2Nivel4();
        GenerarMundo2Nivel5();
        GenerarMundo3Nivel1();
        GenerarMundo3Nivel2();
        GenerarMundo3Nivel3();
        GenerarMundo3Nivel4();
        GenerarMundo3Nivel5();
        GenerarMundo4Nivel1();
        GenerarMundo4Nivel2();
        GenerarMundo4Nivel3();
        GenerarMundo4Nivel4();
        GenerarMundo4Nivel5();
        GenerarMundo5Nivel1();
        GenerarMundo5Nivel2();
        GenerarMundo5Nivel3();
        GenerarMundo5Nivel4();
        GenerarMundo5Nivel5();
        GenerarMundo6Nivel1();
        GenerarMundo6Nivel2();
        GenerarMundo6Nivel3();
        GenerarMundo6Nivel4();
        GenerarMundo6Nivel5();
        GenerarIntro();
        GenerarFinal();

        HerramientasEscena.ActualizarBuildSettings();

        // Al terminar, dejamos abierto el menú principal para darle Play.
        EditorSceneManager.OpenScene(RutaEscena(DatosJuego.escenaMenuPrincipal));
        Debug.Log("Inti y Killa: escenas generadas (todo el juego: Intro, menús, 30 niveles y Final). ¡Dale Play!");
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Menú principal", false, 20)]
    public static void MenuGenerarMenuPrincipal()
    {
        GenerarUnaSola(1);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Selección de mundo", false, 21)]
    public static void MenuGenerarSeleccionMundo()
    {
        GenerarUnaSola(7);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Selección de nivel", false, 22)]
    public static void MenuGenerarSeleccion()
    {
        GenerarUnaSola(2);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 1 - Nivel 1", false, 23)]
    public static void MenuGenerarMundo1Nivel1()
    {
        GenerarUnaSola(3);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 1 - Nivel 2", false, 24)]
    public static void MenuGenerarMundo1Nivel2()
    {
        GenerarUnaSola(4);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 1 - Nivel 3", false, 25)]
    public static void MenuGenerarMundo1Nivel3()
    {
        GenerarUnaSola(5);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 1 - Nivel 4", false, 26)]
    public static void MenuGenerarMundo1Nivel4()
    {
        GenerarUnaSola(8);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 1 - Nivel 5", false, 27)]
    public static void MenuGenerarMundo1Nivel5()
    {
        GenerarUnaSola(9);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 2 - Nivel 2", false, 29)]
    public static void MenuGenerarMundo2Nivel2()
    {
        GenerarUnaSola(10);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 2 - Nivel 3", false, 30)]
    public static void MenuGenerarMundo2Nivel3()
    {
        GenerarUnaSola(11);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 2 - Nivel 4", false, 31)]
    public static void MenuGenerarMundo2Nivel4()
    {
        GenerarUnaSola(12);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 2 - Nivel 5", false, 32)]
    public static void MenuGenerarMundo2Nivel5()
    {
        GenerarUnaSola(13);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 2 - Nivel 1", false, 28)]
    public static void MenuGenerarMundo2Nivel1()
    {
        GenerarUnaSola(6);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 3 - Nivel 1", false, 33)]
    public static void MenuGenerarMundo3Nivel1()
    {
        GenerarUnaSola(14);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 3 - Nivel 2", false, 34)]
    public static void MenuGenerarMundo3Nivel2()
    {
        GenerarUnaSola(15);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 3 - Nivel 3", false, 35)]
    public static void MenuGenerarMundo3Nivel3()
    {
        GenerarUnaSola(16);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 3 - Nivel 4", false, 36)]
    public static void MenuGenerarMundo3Nivel4()
    {
        GenerarUnaSola(17);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 3 - Nivel 5", false, 37)]
    public static void MenuGenerarMundo3Nivel5()
    {
        GenerarUnaSola(18);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 4 - Nivel 1", false, 38)]
    public static void MenuGenerarMundo4Nivel1()
    {
        GenerarUnaSola(19);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 4 - Nivel 2", false, 39)]
    public static void MenuGenerarMundo4Nivel2()
    {
        GenerarUnaSola(20);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 4 - Nivel 3", false, 40)]
    public static void MenuGenerarMundo4Nivel3()
    {
        GenerarUnaSola(21);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 4 - Nivel 4", false, 41)]
    public static void MenuGenerarMundo4Nivel4()
    {
        GenerarUnaSola(22);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 4 - Nivel 5", false, 42)]
    public static void MenuGenerarMundo4Nivel5()
    {
        GenerarUnaSola(23);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 5 - Nivel 1", false, 43)]
    public static void MenuGenerarMundo5Nivel1()
    {
        GenerarUnaSola(24);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 5 - Nivel 2", false, 44)]
    public static void MenuGenerarMundo5Nivel2()
    {
        GenerarUnaSola(25);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 5 - Nivel 3", false, 45)]
    public static void MenuGenerarMundo5Nivel3()
    {
        GenerarUnaSola(26);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 5 - Nivel 4", false, 46)]
    public static void MenuGenerarMundo5Nivel4()
    {
        GenerarUnaSola(27);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 5 - Nivel 5", false, 47)]
    public static void MenuGenerarMundo5Nivel5()
    {
        GenerarUnaSola(28);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 6 - Nivel 1", false, 48)]
    public static void MenuGenerarMundo6Nivel1()
    {
        GenerarUnaSola(29);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 6 - Nivel 2", false, 49)]
    public static void MenuGenerarMundo6Nivel2()
    {
        GenerarUnaSola(30);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 6 - Nivel 3", false, 50)]
    public static void MenuGenerarMundo6Nivel3()
    {
        GenerarUnaSola(31);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 6 - Nivel 4", false, 51)]
    public static void MenuGenerarMundo6Nivel4()
    {
        GenerarUnaSola(32);
    }

    [MenuItem("Inti y Killa/Escenas sueltas/Mundo 6 - Nivel 5", false, 52)]
    public static void MenuGenerarMundo6Nivel5()
    {
        GenerarUnaSola(33);
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
            rutaGenerada = GenerarMundo1Nivel1();
        }
        else if (cual == 4)
        {
            rutaGenerada = GenerarMundo1Nivel2();
        }
        else if (cual == 5)
        {
            rutaGenerada = GenerarMundo1Nivel3();
        }
        else if (cual == 6)
        {
            rutaGenerada = GenerarMundo2Nivel1();
        }
        else if (cual == 7)
        {
            rutaGenerada = GenerarSeleccionMundo();
        }
        else if (cual == 8)
        {
            rutaGenerada = GenerarMundo1Nivel4();
        }
        else if (cual == 10)
        {
            rutaGenerada = GenerarMundo2Nivel2();
        }
        else if (cual == 11)
        {
            rutaGenerada = GenerarMundo2Nivel3();
        }
        else if (cual == 12)
        {
            rutaGenerada = GenerarMundo2Nivel4();
        }
        else if (cual == 13)
        {
            rutaGenerada = GenerarMundo2Nivel5();
        }
        else if (cual == 9)
        {
            rutaGenerada = GenerarMundo1Nivel5();
        }
        else if (cual == 14)
        {
            rutaGenerada = GenerarMundo3Nivel1();
        }
        else if (cual == 15)
        {
            rutaGenerada = GenerarMundo3Nivel2();
        }
        else if (cual == 16)
        {
            rutaGenerada = GenerarMundo3Nivel3();
        }
        else if (cual == 17)
        {
            rutaGenerada = GenerarMundo3Nivel4();
        }
        else if (cual == 18)
        {
            rutaGenerada = GenerarMundo3Nivel5();
        }
        else if (cual == 19)
        {
            rutaGenerada = GenerarMundo4Nivel1();
        }
        else if (cual == 20)
        {
            rutaGenerada = GenerarMundo4Nivel2();
        }
        else if (cual == 21)
        {
            rutaGenerada = GenerarMundo4Nivel3();
        }
        else if (cual == 22)
        {
            rutaGenerada = GenerarMundo4Nivel4();
        }
        else if (cual == 23)
        {
            rutaGenerada = GenerarMundo4Nivel5();
        }
        else if (cual == 24)
        {
            rutaGenerada = GenerarMundo5Nivel1();
        }
        else if (cual == 25)
        {
            rutaGenerada = GenerarMundo5Nivel2();
        }
        else if (cual == 26)
        {
            rutaGenerada = GenerarMundo5Nivel3();
        }
        else if (cual == 27)
        {
            rutaGenerada = GenerarMundo5Nivel4();
        }
        else if (cual == 28)
        {
            rutaGenerada = GenerarMundo5Nivel5();
        }
        else if (cual == 29)
        {
            rutaGenerada = GenerarMundo6Nivel1();
        }
        else if (cual == 30)
        {
            rutaGenerada = GenerarMundo6Nivel2();
        }
        else if (cual == 31)
        {
            rutaGenerada = GenerarMundo6Nivel3();
        }
        else if (cual == 32)
        {
            rutaGenerada = GenerarMundo6Nivel4();
        }
        else if (cual == 33)
        {
            rutaGenerada = GenerarMundo6Nivel5();
        }

        HerramientasEscena.ActualizarBuildSettings();
        EditorSceneManager.OpenScene(rutaGenerada);
    }

    // ================= Rutas =================

    private static string RutaEscena(string nombre)
    {
        return HerramientasEscena.carpetaEscenas + "/" + nombre + ".unity";
    }

    private static string RutaNivel(int numeroMundo, int numeroEnMundo)
    {
        DatosNivel datos = DatosJuego.ObtenerNivel(numeroMundo, numeroEnMundo);
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
        HerramientasEscena.CrearFiguraPersonaje("Inti", HerramientasEscena.imagenInti, ColoresElemento.colorFuego, new Vector2(-1.6f, HerramientasEscena.alturaSuelo + 0.6f), decoracion);
        HerramientasEscena.CrearFiguraPersonaje("Killa", HerramientasEscena.imagenKilla, ColoresElemento.colorAgua, new Vector2(1.6f, HerramientasEscena.alturaSuelo + 0.6f), decoracion);

        // ----- Interfaz -----
        Transform canvas = HerramientasEscena.CrearCanvas();
        HerramientasEscena.CrearEventSystem();

        // El eclipse, que es la historia del juego, detrás del título.
        HerramientasInterfaz.CrearImagen(canvas, "Corona", HerramientasEscena.spriteResplandor, new Color(1f, 0.82f, 0.50f, 0.85f), new Vector2(0f, 335f), new Vector2(640f, 640f));
        HerramientasInterfaz.CrearImagen(canvas, "Luna", HerramientasEscena.spriteDisco, new Color(0.10f, 0.09f, 0.20f), new Vector2(0f, 335f), new Vector2(240f, 240f));

        HerramientasInterfaz.CrearTitulo(canvas, "INTI y KILLA", 150, new Vector2(0f, 310f));
        Text subtitulo = HerramientasEscena.CrearTexto(canvas, "Subtitulo", "No confíes en nada... ni en tu compañero", 44, new Vector2(0f, 200f), new Vector2(1400f, 60f), Color.white);
        subtitulo.raycastTarget = false;

        GameObject objetoMenu = new GameObject("MenuPrincipal");
        MenuPrincipal menu = objetoMenu.AddComponent<MenuPrincipal>();

        Button botonJugar = HerramientasEscena.CrearBoton(canvas, "Boton Jugar", "JUGAR", new Vector2(0f, 20f), new Vector2(560f, 180f), colorBotonPrincipal, 92);
        botonJugar.transform.Find("Texto").GetComponent<Text>().fontStyle = FontStyle.Bold;
        botonJugar.gameObject.AddComponent<EscalaAlPasarMouse>();
        Button botonBorrar = HerramientasEscena.CrearBoton(canvas, "Boton Borrar progreso", "Borrar progreso", new Vector2(680f, -420f), new Vector2(380f, 80f), colorBotonSecundario, 34);
        Button botonSalir = HerramientasEscena.CrearBoton(canvas, "Boton Salir", "Salir", new Vector2(-680f, -420f), new Vector2(380f, 80f), colorBotonSalir, 34);

        // Conexiones guardadas en la escena (se ven en el Inspector, sección On Click).
        UnityEventTools.AddPersistentListener(botonJugar.onClick, new UnityAction(menu.Jugar));
        UnityEventTools.AddPersistentListener(botonBorrar.onClick, new UnityAction(menu.BorrarProgreso));
        UnityEventTools.AddPersistentListener(botonSalir.onClick, new UnityAction(menu.Salir));

        Text textoMensaje = HerramientasEscena.CrearTexto(canvas, "Mensaje", "", 30, new Vector2(0f, -120f), new Vector2(1200f, 50f), new Color(1f, 1f, 0.8f));
        textoMensaje.raycastTarget = false;
        menu.Configurar(textoMensaje);

        HerramientasInterfaz.CrearFranjaAyuda(canvas, "2 jugadores en el mismo teclado     Inti: A D W + S     Killa: Flechas + Flecha abajo");

        string ruta = RutaEscena(DatosJuego.escenaMenuPrincipal);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Selección de mundo =================
    // Tarjetas altas que se pasan hacia los lados, con el paisaje del mundo de
    // fondo. Se ve la tarjeta central y parte de las vecinas, para que se note
    // que hay más mundos a los costados.

    private static string GenerarSeleccionMundo()
    {
        Scene escena = CrearEscenaVacia();

        HerramientasEscena.CrearCamara(colorCieloFondo);
        Transform canvas = HerramientasEscena.CrearCanvas();
        HerramientasEscena.CrearEventSystem();

        FondoMundo fondo = HerramientasInterfaz.CrearFondoMundo(canvas);

        // ----- Carrusel: ScrollRect > Ventana (con máscara) > Contenido > Tarjetas -----
        GameObject objetoCarrusel = new GameObject("Carrusel de mundos", typeof(RectTransform));
        objetoCarrusel.transform.SetParent(canvas, false);
        RectTransform rectanguloCarrusel = objetoCarrusel.GetComponent<RectTransform>();
        rectanguloCarrusel.anchorMin = Vector2.zero;
        rectanguloCarrusel.anchorMax = Vector2.one;
        rectanguloCarrusel.offsetMin = Vector2.zero;
        rectanguloCarrusel.offsetMax = Vector2.zero;

        // La ventana también recibe el arrastre sobre el espacio entre tarjetas.
        Image imagenVentana = HerramientasInterfaz.CrearImagen(objetoCarrusel.transform, "Ventana", null, Color.white, Vector2.zero, Vector2.zero);
        imagenVentana.raycastTarget = true;
        RectTransform ventana = imagenVentana.rectTransform;
        ventana.anchorMin = Vector2.zero;
        ventana.anchorMax = Vector2.one;
        ventana.offsetMin = Vector2.zero;
        ventana.offsetMax = Vector2.zero;
        Mask mascaraVentana = imagenVentana.gameObject.AddComponent<Mask>();
        mascaraVentana.showMaskGraphic = false;

        GameObject objetoContenido = new GameObject("Contenido", typeof(RectTransform));
        objetoContenido.transform.SetParent(ventana, false);
        RectTransform contenido = objetoContenido.GetComponent<RectTransform>();
        contenido.anchorMin = Vector2.zero;
        contenido.anchorMax = new Vector2(0f, 1f);
        contenido.pivot = new Vector2(0f, 0.5f);
        contenido.anchoredPosition = Vector2.zero;
        contenido.sizeDelta = Vector2.zero;

        ScrollRect desplazamiento = objetoCarrusel.AddComponent<ScrollRect>();
        desplazamiento.content = contenido;
        desplazamiento.viewport = ventana;
        desplazamiento.horizontal = true;
        desplazamiento.vertical = false;
        desplazamiento.movementType = ScrollRect.MovementType.Elastic;
        // Sin inercia ni rueda: el carrusel mismo acomoda la tarjeta al soltar.
        desplazamiento.inertia = false;
        desplazamiento.scrollSensitivity = 0f;

        int cantidadMundos = DatosJuego.CantidadDeMundos();
        RectTransform[] tarjetas = new RectTransform[cantidadMundos];
        for (int indice = 0; indice < cantidadMundos; indice++)
        {
            tarjetas[indice] = HerramientasInterfaz.CrearTarjetaMundo(contenido, indice + 1);
        }

        CarruselMundos carrusel = objetoCarrusel.AddComponent<CarruselMundos>();

        // ----- Encima del carrusel -----
        HerramientasInterfaz.CrearTitulo(canvas, "Elige un mundo", 80, new Vector2(0f, 445f));

        Button flechaAnterior = HerramientasInterfaz.CrearFlecha(canvas, "Flecha anterior", new Vector2(0f, 0.5f), new Vector2(110f, 0f), 130f, false, new Color(0.14f, 0.11f, 0.22f, 0.85f), new UnityAction(carrusel.MoverAnterior));
        Button flechaSiguiente = HerramientasInterfaz.CrearFlecha(canvas, "Flecha siguiente", new Vector2(1f, 0.5f), new Vector2(-110f, 0f), 130f, true, new Color(0.14f, 0.11f, 0.22f, 0.85f), new UnityAction(carrusel.MoverSiguiente));

        // Un punto por mundo, bajo las tarjetas.
        Image[] puntos = new Image[cantidadMundos];
        float separacionPuntos = 38f;
        float inicioPuntos = -separacionPuntos * (cantidadMundos - 1) / 2f;
        for (int indice = 0; indice < cantidadMundos; indice++)
        {
            Vector2 posicionPunto = new Vector2(inicioPuntos + indice * separacionPuntos, -372f);
            puntos[indice] = HerramientasInterfaz.CrearImagen(canvas, "Punto " + (indice + 1), HerramientasEscena.spriteDisco, Color.white, posicionPunto, new Vector2(22f, 22f));
        }

        carrusel.Configurar(tarjetas, puntos, fondo, flechaAnterior, flechaSiguiente);

        GameObject objetoSeleccion = new GameObject("SeleccionMundo");
        SeleccionMundo seleccion = objetoSeleccion.AddComponent<SeleccionMundo>();
        seleccion.Configurar(carrusel);

        HerramientasInterfaz.CrearFlechaVolver(canvas, new UnityAction(seleccion.VolverAlMenu));
        HerramientasInterfaz.CrearFranjaAyuda(canvas, "Flechas o arrastrar: cambiar de mundo     Enter o clic: entrar     Esc: volver");

        string ruta = RutaEscena(DatosJuego.escenaSeleccionMundo);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Selección de nivel =================
    // Una sola escena para los 6 mundos: el paisaje, el título y los cuadros se
    // llenan al abrirla, según el mundo elegido en la pantalla anterior.

    private static string GenerarSeleccionNivel()
    {
        Scene escena = CrearEscenaVacia();

        HerramientasEscena.CrearCamara(colorCieloFondo);
        Transform canvas = HerramientasEscena.CrearCanvas();
        HerramientasEscena.CrearEventSystem();

        FondoMundo fondo = HerramientasInterfaz.CrearFondoMundo(canvas);

        GameObject objetoSeleccion = new GameObject("SeleccionNivel");
        SeleccionNivel seleccion = objetoSeleccion.AddComponent<SeleccionNivel>();

        Text textoTitulo = HerramientasInterfaz.CrearTitulo(canvas, "Mundo", 76, new Vector2(0f, 420f));
        Text textoEstrellas = HerramientasInterfaz.CrearContadorEstrellas(canvas, new Vector2(0f, 335f));
        seleccion.Configurar(textoTitulo, textoEstrellas, fondo);

        float separacionX = 345f;
        float inicioX = -separacionX * (DatosJuego.nivelesPorMundo - 1) / 2f;
        for (int numeroEnMundo = 1; numeroEnMundo <= DatosJuego.nivelesPorMundo; numeroEnMundo++)
        {
            Vector2 posicion = new Vector2(inicioX + (numeroEnMundo - 1) * separacionX, 10f);
            HerramientasInterfaz.CrearCuadroNivel(canvas, numeroEnMundo, posicion);
        }

        HerramientasInterfaz.CrearFlechaVolver(canvas, new UnityAction(seleccion.VolverAMundos));
        HerramientasInterfaz.CrearFranjaAyuda(canvas, "Clic en un nivel para jugar     Esc: volver a los mundos");

        string ruta = RutaEscena(DatosJuego.escenaSeleccionNivel);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 1, Nivel 1: Primeros pasos =================
    // Enseña: moverse, saltar, gemas, lava/agua, púas y un bloque que se rompe.
    // No es un pasillo: son tres pisos que se suben en zigzag. Empiezan juntos en
    // el centro y cada uno se va por su lado, porque el camino del otro lo mata.
    // Se reencuentran en el piso central y ahí se CRUZAN: la puerta de cada uno
    // está arriba, en el lado contrario al que subió.

    private static string GenerarMundo1Nivel1()
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
        // TRAMPA: el tramo justo después de los cristales es falso. Quien aterriza
        // ahí cae un piso; quien ya lo sabe salta más lejos, hasta la puerta.
        HerramientasEscena.CrearPlataforma("Piso 3 derecha", new Vector2(7.45f, pisoTres - mitadGrosor), new Vector2(4.9f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearSueloFalso("Tramo de suelo", new Vector2(10.45f, pisoTres - mitadGrosor), new Vector2(1.1f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 derecha final", new Vector2(12f, pisoTres - mitadGrosor), new Vector2(2f, grosor), HerramientasEscena.colorSal, escenario);

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

        // TRAMPA: una gema regalada junto a Killa. Al tocarla cae el bloque de arriba;
        // quien no se detiene se salva, y quien ya lo sabe la salta.
        TechoQueCae bloqueDeLaGema = HerramientasEscena.CrearTechoQueCae("Bloque del techo", new Vector2(2.3f, pisoDos - grosor - mitadGrosor), new Vector2(1.5f, grosor), suelo, false, HerramientasEscena.colorSal, escenario);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(bloqueDeLaGema);
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Agua, new Vector2(3f, -2.4f), trampasDeLaGema, gemas);
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

        string ruta = RutaNivel(1, 1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 1, Nivel 2: Botones y compuertas =================
    // Enseña a turnarse: nadie avanza solo. Cada uno empieza en su lado de un muro
    // y el botón que abre la compuerta de uno está en el lado del otro.
    // Arriba, la salida solo se abre cuando LOS DOS encendieron su palanca.

    private static string GenerarMundo1Nivel2()
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

        // TRAMPA: un botón en el camino de Inti que parece abrir su compuerta, pero
        // suelta el bloque de arriba. Quien sigue corriendo se salva.
        TechoQueCae bloqueDelBoton = HerramientasEscena.CrearTechoQueCae("Bloque del techo", new Vector2(-11f, pisoUno - grosor - mitadGrosor), new Vector2(1.5f, grosor), suelo, false, HerramientasEscena.colorSal, escenario);
        List<TrampaActivable> trampasDelBoton = new List<TrampaActivable>();
        trampasDelBoton.Add(bloqueDelBoton);
        HerramientasEscena.CrearBotonEnganoso("Boton de Inti falso", new Vector2(-11f, suelo), trampasDelBoton, mecanismos);

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

        // TRAMPA: una "salida" al lado de la palanca, que sale volando si te acercas.
        HerramientasEscena.CrearPuertaFalsa("Puerta sospechosa", TipoElemento.Fuego, new Vector2(12.6f, pisoTres), new Vector2(0f, 9f), mecanismos);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        // Bajo techo el salto es corto (unas 2.6 unidades). Con 1 de ancho hay margen
        // para saltar corriendo: con más, el salto exige demasiada precisión.
        HerramientasEscena.CrearCharco("Agua de la planta baja", TipoZona.Agua, -6f, 1f, trampas);
        HerramientasEscena.CrearCharco("Lava de la planta baja", TipoZona.Lava, 6f, 1f, trampas);
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

        string ruta = RutaNivel(1, 2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 1, Nivel 3: El primer eclipse =================
    // Cuatro pisos que se suben en zigzag. Tres ríos imposibles de saltar: lava,
    // agua y lava. Quien puede cruzar va primero y se para en la placa del otro
    // lado; el eclipse intercambia los poderes y entonces cruza el compañero.
    // Después de tres eclipses los poderes quedan cambiados: cada uno entra por
    // la puerta del poder que tiene en ese momento.

    private static string GenerarMundo1Nivel3()
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

        // TRAMPA: púas escondidas justo antes de las puertas. Se pueden saltar.
        HerramientasEscena.CrearPuasOcultas("Piso tranquilo", -8.7f, 1.2f, pisoTres, true, trampas);

        // TRAMPA: un bloque del techo cae sobre el primer río. Quien no se detiene pasa.
        HerramientasEscena.CrearTechoQueCae("Bloque del techo", new Vector2(-5f, pisoUno - grosor - mitadGrosor), new Vector2(1.5f, grosor), suelo, true, HerramientasEscena.colorSal, escenario);

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

        string ruta = RutaNivel(1, 3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 1, Nivel 4: Espejo de sal =================
    // Repaso de botones y eclipse juntos. El nivel es simétrico, como el reflejo
    // del salar, y hay que cambiar de poderes para poder siquiera empezar:
    // 1) Cada uno está junto a un río que lo mata: el eclipse los deja pasar.
    // 2) Del otro lado, el botón de cada uno abre la compuerta del compañero.
    // 3) Arriba hay un río de lava: solo cruza quien tiene el fuego (Killa), y la
    //    placa del otro altar está en lo alto, encima de la lava. Al pisarla, el
    //    fuego vuelve a Inti y entonces él también puede cruzar.
    // Terminan con sus poderes originales: dos eclipses se cancelan, como un espejo.

    private static string GenerarMundo1Nivel4()
    {
        HerramientasEscena.usarTilesetSal = true;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(1f, 0.93f, 0.86f), 1f);
        HerramientasEscena.CrearGestorNivel("Cada río mata a quien está a su lado. Provoquen un eclipse para cruzarlos, y ayúdense con los botones.");
        HerramientasEscena.CrearFondoUyuni(0f);

        float pisoUno = -0.25f;
        float pisoDos = 3f;
        float pisoTres = 6.25f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        HerramientasEscena.CrearMuro("Muro central", 0f, suelo, pisoUno - grosor, HerramientasEscena.colorSal, escenario);
        // El piso 1 deja un hueco en cada extremo, DETRÁS de cada compuerta.
        HerramientasEscena.CrearPlataforma("Piso 1", new Vector2(0f, pisoUno - mitadGrosor), new Vector2(25f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2", new Vector2(0f, pisoDos - mitadGrosor), new Vector2(21f, grosor), HerramientasEscena.colorSal, escenario);
        // Queda tan lejos de las orillas que solo se alcanza saltando desde la lava.
        HerramientasEscena.CrearPlataforma("Piso alto", new Vector2(0f, pisoTres - mitadGrosor), new Vector2(7f, grosor), HerramientasEscena.colorSal, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        // Bajo el techo el salto es corto: estos ríos no se pueden saltar.
        HerramientasEscena.CrearCharco("Rio de agua", TipoZona.Agua, -6.5f, 5f, trampas);
        HerramientasEscena.CrearCharco("Rio de lava", TipoZona.Lava, 6.5f, 5f, trampas);
        // Veneno que protege dos gemas opcionales: pide un salto preciso bajo techo.
        HerramientasEscena.CrearCharco("Veneno izquierdo", TipoZona.Veneno, -7.3f, 1f, pisoUno, trampas);
        HerramientasEscena.CrearCharco("Veneno derecho", TipoZona.Veneno, 7.3f, 1f, pisoUno, trampas);
        HerramientasEscena.CrearCharco("Gran rio de lava", TipoZona.Lava, 0f, 18f, pisoDos, trampas);

        // Remolinos de sal: soplan hacia afuera y dificultan el salto al piso alto.
        // No matan: quien tenga el fuego cae sobre la lava sin daño y lo intenta de nuevo.
        HerramientasEscena.CrearRemolinoViento("Remolino izquierdo", new Vector2(-5.25f, 5.75f), new Vector2(3.5f, 5.5f), -3f, trampas);
        HerramientasEscena.CrearRemolinoViento("Remolino derecho", new Vector2(5.25f, 5.75f), new Vector2(3.5f, 5.5f), 3f, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        Vector2 tamanoCompuerta = new Vector2(0.7f, 2.75f);
        Vector2 hundirse = new Vector2(0f, -2.85f);
        float centroCompuertaY = suelo + tamanoCompuerta.y / 2f;
        PuertaMecanica compuertaIzquierda = HerramientasEscena.CrearPuertaMecanica("Compuerta izquierda", new Vector2(-11.5f, centroCompuertaY), tamanoCompuerta, hundirse, 1, mecanismos);
        PuertaMecanica compuertaDerecha = HerramientasEscena.CrearPuertaMecanica("Compuerta derecha", new Vector2(11.5f, centroCompuertaY), tamanoCompuerta, hundirse, 1, mecanismos);
        compuertaIzquierda.GetComponent<SpriteRenderer>().sortingOrder = -1;
        compuertaDerecha.GetComponent<SpriteRenderer>().sortingOrder = -1;

        // Inti le abre el paso a Killa, y Killa, ya del otro lado, se lo abre a Inti.
        List<ObjetoActivable> abreDerecha = new List<ObjetoActivable>();
        abreDerecha.Add(compuertaDerecha);
        HerramientasEscena.CrearBotonPresion("Boton izquierdo", new Vector2(-10f, suelo), abreDerecha, mecanismos);

        List<ObjetoActivable> abreIzquierda = new List<ObjetoActivable>();
        abreIzquierda.Add(compuertaIzquierda);
        HerramientasEscena.CrearBotonPresion("Boton derecho", new Vector2(13.5f, suelo), abreIzquierda, mecanismos);

        Transform altares = HerramientasEscena.CrearGrupo("Altares");
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 1", new Vector2(-2.7f, suelo), new Vector2(2.7f, suelo), colorAltarUno, altares);
        AltarEclipse altarDeArriba = HerramientasEscena.CrearAltarEclipse("Altar de eclipse 2", new Vector2(-10f, pisoDos), new Vector2(0f, pisoTres), colorAltarDos, altares);

        // TRAICIÓN: después del eclipse, el primero que se baja le saca púas al otro.
        HerramientasEscena.CrearPlacaVinculada(altarDeArriba, TipoCastigo.SalenPuas);

        // TRAICIÓN: para llegar a su gema, Killa tiene que pisar este botón, y a Inti le
        // cae lava. Con el aviso, Inti tiene tiempo de quitarse. Se puede saltar subiendo
        // directo al piso 2, pero entonces se pierde la gema.
        HerramientasEscena.CrearBotonTraicion("Boton del piso 1", new Vector2(10.6f, pisoUno), TipoCastigo.CaeLava, altares);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-6.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(6.5f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-5f, 0.5f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(5f, 0.5f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(6f, 3.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(0f, 7.2f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-2.4f, pisoTres), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(2.4f, pisoTres), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-1f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(1f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(1, 4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 1, Nivel 5: Travesía del salar =================
    // Examen del mundo: un solo camino largo en zigzag que usa todo lo aprendido.
    // 1) Planta baja: cruzar el veneno por bloques que se rompen, de uno en uno.
    // 2) Piso 1: uno sostiene un botón abajo para que el otro llegue a una palanca
    //    que deja la compuerta abierta. Luego, río de lava y eclipse.
    // 3) Piso 2: río de agua y otro eclipse.
    // 4) Piso 3: uno sostiene otro botón mientras el compañero salta veneno y
    //    cristales hasta la palanca que abre el camino a las puertas.

    private static string GenerarMundo1Nivel5()
    {
        HerramientasEscena.usarTilesetSal = true;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorCieloFondo);
        HerramientasEscena.CrearLuzGlobal(new Color(0.95f, 0.85f, 0.90f), 1f);
        HerramientasEscena.CrearGestorNivel("La última prueba del Salar: bloques que se rompen, botones, palancas y eclipses. ¡Cuiden al compañero!");
        HerramientasEscena.CrearFondoUyuni(0f);

        float pisoUno = -0.25f;
        float pisoDos = 3f;
        float pisoTres = 6.25f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        // Cada piso deja un hueco para subir, alternando de lado (el zigzag).
        Transform escenario = HerramientasEscena.CrearSueloYParedes(HerramientasEscena.colorSal);
        HerramientasEscena.CrearPlataforma("Piso 1", new Vector2(-2f, pisoUno - mitadGrosor), new Vector2(26f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2", new Vector2(1.5f, pisoDos - mitadGrosor), new Vector2(27f, grosor), HerramientasEscena.colorSal, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3", new Vector2(0f, pisoTres - mitadGrosor), new Vector2(24f, grosor), HerramientasEscena.colorSal, escenario);

        // Bloques bajos, porque el piso 1 hace de techo: hay que cruzarlos corriendo.
        Vector2 tamanoBloque = new Vector2(2f, grosor);
        float alturaBloque = suelo + 0.9f - mitadGrosor;
        HerramientasEscena.CrearPlataformaQuebradiza("Bloque que se rompe 1", new Vector2(-4.5f, alturaBloque), tamanoBloque, escenario);
        HerramientasEscena.CrearPlataformaQuebradiza("Bloque que se rompe 2", new Vector2(-1.5f, alturaBloque), tamanoBloque, escenario);
        HerramientasEscena.CrearPlataformaQuebradiza("Bloque que se rompe 3", new Vector2(1.5f, alturaBloque), tamanoBloque, escenario);
        HerramientasEscena.CrearPlataformaQuebradiza("Bloque que se rompe 4", new Vector2(4.5f, alturaBloque), tamanoBloque, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Veneno de abajo", TipoZona.Veneno, 0f, 12f, trampas);
        HerramientasEscena.CrearCharco("Rio de lava", TipoZona.Lava, -3f, 10f, pisoUno, trampas);
        HerramientasEscena.CrearCharco("Rio de agua", TipoZona.Agua, 0f, 12f, pisoDos, trampas);
        HerramientasEscena.CrearCharco("Veneno de arriba", TipoZona.Veneno, 6f, 2f, pisoTres, trampas);
        HerramientasEscena.CrearCristalesDeSal("Cristales de arriba", 1f, 2f, pisoTres, trampas);

        // Remolinos de sal: uno empuja hacia el otro lado del veneno y puede hacer que
        // te pases; el otro sopla en contra sobre los cristales. El de contra es más
        // suave (2) para que el salto todavía alcance, y termina antes del lugar donde
        // se aterriza: así nunca empuja hacia los cristales a quien ya cruzó.
        HerramientasEscena.CrearRemolinoViento("Remolino del veneno", new Vector2(6.25f, 8.25f), new Vector2(3.5f, 4f), -3f, trampas);
        HerramientasEscena.CrearRemolinoViento("Remolino de los cristales", new Vector2(1.5f, 7.75f), new Vector2(2.4f, 3f), 2f, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");

        // Compuerta del piso 1: se hunde, y la abre un botón en la planta baja.
        // Una palanca al otro lado la deja abierta para que suba el compañero.
        Vector2 tamanoCompuerta = new Vector2(0.7f, 2.75f);
        PuertaMecanica compuertaPisoUno = HerramientasEscena.CrearPuertaMecanica("Compuerta del piso 1", new Vector2(7f, pisoUno + tamanoCompuerta.y / 2f), tamanoCompuerta, new Vector2(0f, -2.85f), 1, mecanismos);
        compuertaPisoUno.GetComponent<SpriteRenderer>().sortingOrder = -1;

        List<ObjetoActivable> abrePisoUno = new List<ObjetoActivable>();
        abrePisoUno.Add(compuertaPisoUno);
        HerramientasEscena.CrearBotonPresion("Boton de abajo", new Vector2(8.5f, suelo), abrePisoUno, mecanismos);
        // TRAICIÓN: la palanca sí abre la compuerta, pero le abre el suelo a quien
        // sostiene el botón. Con el aviso le alcanza para moverse.
        HerramientasEscena.CrearPalancaSospechosa("Palanca del piso 1", new Vector2(5.7f, pisoUno), abrePisoUno, TipoCastigo.SeAbreElSuelo, mecanismos);

        // Compuerta final: tan alta que no se puede saltar. Funciona igual.
        Vector2 tamanoCierre = new Vector2(0.7f, 4.5f);
        PuertaMecanica compuertaFinal = HerramientasEscena.CrearPuertaMecanica("Compuerta final", new Vector2(-4f, pisoTres + tamanoCierre.y / 2f), tamanoCierre, new Vector2(0f, 4.6f), 1, mecanismos);

        List<ObjetoActivable> abreFinal = new List<ObjetoActivable>();
        abreFinal.Add(compuertaFinal);
        HerramientasEscena.CrearBotonPresion("Boton de arriba", new Vector2(10.2f, pisoTres), abreFinal, mecanismos);

        // TRAICIÓN: junto al veneno de arriba, un botón que le saca púas a quien sostiene
        // la compuerta. Está donde no hay techo, para que siempre se pueda saltar.
        HerramientasEscena.CrearBotonTraicion("Boton del piso 3", new Vector2(7.9f, pisoTres), TipoCastigo.SalenPuas, mecanismos);
        HerramientasEscena.CrearPalanca("Palanca final", new Vector2(-6.5f, pisoTres), abreFinal, mecanismos);

        Transform altares = HerramientasEscena.CrearGrupo("Altares");
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 1", new Vector2(3.5f, pisoUno), new Vector2(-10f, pisoUno), colorAltarUno, altares);
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse 2", new Vector2(-9f, pisoDos), new Vector2(8f, pisoDos), colorAltarDos, altares);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-1.5f, -2f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(1.5f, -2f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-3f, 0.5f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(0f, 3.9f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(6f, 10f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(1f, 9.6f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-9f, pisoTres), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-11f, pisoTres), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-10f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-12f, suelo));

        // La cámara se crea al final porque necesita a los dos jugadores.
        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(1, 5);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 2, Nivel 1: La luz de Inti (Minas de Potosí) =================
    // La mina está a oscuras y se recorre subiendo. Los cristales solo se pisan
    // dentro de la luz de Inti, así que él alumbra el camino de Killa:
    // 1) Cruzan juntos el veneno por cristales, con Inti delante manteniendo S.
    // 2) Killa sube sobre la lava mientras Inti alumbra desde abajo. El último
    //    escalón está fuera de la luz normal: Inti tiene que mantener S.
    // 3) Killa, ya arriba, enciende la palanca que abre el paso de Inti.
    // 4) En el piso alto cruzan otro río de veneno, y otra vez dependen de la luz.

    private static readonly Color colorFondoMina = new Color(0.07f, 0.05f, 0.05f);
    private static readonly Color colorSueloMina = new Color(0.68f, 0.52f, 0.38f);

    private static string GenerarMundo2Nivel1()
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

        // TRAMPA: bloque invisible en el salto natural hacia el piso alto. Se queda
        // ahí, pero se rodea saltando desde más lejos.
        HerramientasEscena.CrearBloqueInvisible("Bloque escondido", new Vector2(9.9f, 2.65f), new Vector2(1f, grosor), colorSueloMina, escenario);

        // TRAMPA: algo sale del suelo antes de la lava y persigue a quien pasa.
        HerramientasEscena.CrearEnemigoSorpresa("Habitante de la mina", new Vector2(2.6f, suelo), mecanismos);

        // TRAMPA: la última gema, justo antes de las puertas, saca púas del piso.
        PuasOcultas puasDeLaGema = HerramientasEscena.CrearPuasOcultas("Piso tranquilo", -9.2f, 1.5f, pisoAlto, false, trampas);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(puasDeLaGema);
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Fuego, new Vector2(-9.2f, 5.5f), trampasDeLaGema, gemas);

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

        string ruta = RutaNivel(2, 1);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 2, Nivel 2: Vagonetas =================
    // Enseña la vagoneta con cooperación: solo avanza mientras alguien pisa un
    // botón, así que cada uno lleva al otro a través del veneno. Arriba empiezan
    // los peligros de la mina: rocas que tiemblan antes de caer y un murciélago.

    private static string GenerarMundo2Nivel2()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoMina);
        HerramientasEscena.CrearLuzGlobal(new Color(0.85f, 0.80f, 0.85f), 0.65f);
        HerramientasEscena.CrearGestorNivel("La vagoneta solo avanza mientras alguien pisa un botón. Crucen por turnos. Las rocas tiemblan antes de caer.");
        HerramientasEscena.CrearFondoMina();

        float pisoUno = -0.25f;
        float pisoDos = 3f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(colorSueloMina);
        HerramientasEscena.CrearPlataforma("Piso 1", new Vector2(7.5f, pisoUno - mitadGrosor), new Vector2(7f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2", new Vector2(-4f, pisoDos - mitadGrosor), new Vector2(18f, grosor), colorSueloMina, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de veneno", TipoZona.Veneno, 0f, 20f, trampas);
        HerramientasEscena.CrearCharco("Lava del piso 2", TipoZona.Lava, -8f, 1.6f, pisoDos, trampas);
        HerramientasEscena.CrearCharco("Agua del piso 2", TipoZona.Agua, -2f, 1.6f, pisoDos, trampas);

        // TRAMPA suave: la "salida" a la derecha del piso 1 se escapa al acercarse.
        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        HerramientasEscena.CrearPuertaFalsa("Puerta sospechosa", TipoElemento.Agua, new Vector2(10.3f, pisoUno), new Vector2(0f, 9f), mecanismos);

        // La vagoneta va de lado a lado; cualquiera de los dos botones la mueve.
        PlataformaMovil vagoneta = HerramientasEscena.CrearVagoneta("Vagoneta", new Vector2(-8.8f, -2.5f), 17.6f, 4f, false, mecanismos);
        List<ObjetoActivable> mueveVagoneta = new List<ObjetoActivable>();
        mueveVagoneta.Add(vagoneta);
        HerramientasEscena.CrearBotonPresion("Boton izquierdo", new Vector2(-13.5f, suelo), mueveVagoneta, mecanismos);
        HerramientasEscena.CrearBotonPresion("Boton derecho", new Vector2(13.5f, suelo), mueveVagoneta, mecanismos);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearRocaQueCae("Roca del piso 1", new Vector2(8.5f, 2.6f), pisoUno, 0.8f, peligros);
        HerramientasEscena.CrearMurcielago("Murcielago", new Vector2(-5f, 5.4f), new Vector2(2.5f, 1f), 5f, peligros);

        Transform faroles = HerramientasEscena.CrearGrupo("Faroles");
        HerramientasEscena.CrearFarol("Farol de la entrada", new Vector2(-12f, 1.5f), 4f, faroles);
        HerramientasEscena.CrearFarol("Farol de la salida", new Vector2(-11f, 8f), 4f, faroles);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-8f, 4.1f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-2f, 4.1f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(0f, -1.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(4f, -1.4f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-12.2f, pisoDos), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-10.4f, pisoDos), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-12.2f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-10.8f, suelo));

        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(2, 2);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 2, Nivel 3: Cajas pesadas =================
    // La caja pesa tanto que hacen falta los dos para moverla: es el escalón hacia
    // el piso alto, que no se alcanza de un salto. Arriba, un río que obliga a usar
    // el eclipse, con trampas sorpresa, rocas y un murciélago.

    private static string GenerarMundo2Nivel3()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoMina);
        HerramientasEscena.CrearLuzGlobal(new Color(0.85f, 0.80f, 0.85f), 0.65f);
        HerramientasEscena.CrearGestorNivel("Párense los dos a la izquierda de la caja y empújenla juntos hasta la marca amarilla. Súbanse a ella y lleguen a las placas de arriba.");
        HerramientasEscena.CrearFondoMina();

        // 4.7 sobre el suelo: más que el salto (3.8). Sin la caja (1.2) no se llega.
        float pisoAlto = 1.2f;
        float pisoFinal = 4.45f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(colorSueloMina);
        HerramientasEscena.CrearPlataforma("Piso alto", new Vector2(11.25f, pisoAlto - mitadGrosor), new Vector2(7.5f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso final", new Vector2(-1.25f, pisoFinal - mitadGrosor), new Vector2(27.5f, grosor), colorSueloMina, escenario);

        // La caja se detiene junto al borde del piso alto: nunca se atasca sin solución.
        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        HerramientasEscena.CrearCajaEmpujable("Caja", new Vector2(-4f, suelo), -9f, 6.4f, mecanismos);
        // Flechas y marca en el piso: muestran adónde hay que llevar la caja.
        HerramientasEscena.CrearFlechaGuia("Flecha de la caja 1", new Vector2(0f, -1.6f), true, mecanismos);
        HerramientasEscena.CrearFlechaGuia("Flecha de la caja 2", new Vector2(3f, -1.6f), true, mecanismos);
        HerramientasEscena.CrearDecoracion("Marca de la caja", HerramientasEscena.spriteCuadrado, new Vector2(6.4f, suelo + 0.04f), new Vector2(1.4f, 0.08f), new Color(1f, 0.85f, 0.25f), 1, false, mecanismos);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de lava", TipoZona.Lava, -2f, 14f, pisoFinal, trampas);

        // TRAMPA: un tramo del piso alto es falso (se ve venir saltando).
        HerramientasEscena.CrearPuasOcultas("Piso tranquilo", 12f, 1f, pisoAlto, true, trampas);

        // TRAMPA: una gema regalada en el piso final, junto a la placa.
        PuasOcultas puasDeLaGema = HerramientasEscena.CrearPuasOcultas("Piso con gema", 8f, 1.5f, pisoFinal, false, trampas);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(puasDeLaGema);
        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Agua, new Vector2(8f, 5.5f), trampasDeLaGema, gemas);

        Transform altares = HerramientasEscena.CrearGrupo("Altares");
        HerramientasEscena.CrearAltarEclipse("Altar de eclipse", new Vector2(6.2f, pisoFinal), new Vector2(-11f, pisoFinal), colorAltarUno, altares);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearRocaQueCae("Roca del piso alto", new Vector2(9.5f, 3.6f), pisoAlto, 0.8f, peligros);
        HerramientasEscena.CrearMurcielago("Murcielago", new Vector2(0f, 7f), new Vector2(3f, 1f), 6f, peligros);

        Transform faroles = HerramientasEscena.CrearGrupo("Faroles");
        HerramientasEscena.CrearFarol("Farol de la caja", new Vector2(-6f, 1.5f), 4f, faroles);
        HerramientasEscena.CrearFarol("Farol de la salida", new Vector2(-12f, 9f), 4f, faroles);

        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(-2f, 5.6f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-6f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(1f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(10.5f, 2.4f), gemas);

        // Tras el eclipse los poderes quedan cambiados: entran por la puerta del poder que tengan.
        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-9.6f, pisoFinal), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-13f, pisoFinal), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-11f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-13f, suelo));

        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(2, 3);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 2, Nivel 4: Galerías oscuras =================
    // Oscuridad total y traición: la palanca que enciende la vagoneta castiga al
    // compañero, y en el piso de arriba hay un botón que le tira lava. Inti sigue
    // siendo el único que alumbra los cristales.

    private static string GenerarMundo2Nivel4()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoMina);
        HerramientasEscena.CrearLuzGlobal(new Color(0.55f, 0.60f, 0.85f), 0.22f);
        HerramientasEscena.CrearGestorNivel("Galerías a oscuras. Enciendan la vagoneta... y no confíen demasiado en esa palanca.");
        HerramientasEscena.CrearFondoMina();

        float pisoUno = 1.2f;
        float pisoDos = 4.45f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(colorSueloMina);
        HerramientasEscena.CrearPlataforma("Piso 1", new Vector2(9f, pisoUno - mitadGrosor), new Vector2(6f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2", new Vector2(-3f, pisoDos - mitadGrosor), new Vector2(23f, grosor), colorSueloMina, escenario);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        HerramientasEscena.CrearCharco("Rio de veneno", TipoZona.Veneno, 0f, 18f, trampas);
        HerramientasEscena.CrearCharco("Veneno alto", TipoZona.Veneno, -1f, 8f, pisoDos, trampas);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        PlataformaMovil vagoneta = HerramientasEscena.CrearVagoneta("Vagoneta", new Vector2(-7.8f, -2.5f), 15.6f, 4f, false, mecanismos);
        List<ObjetoActivable> mueveVagoneta = new List<ObjetoActivable>();
        mueveVagoneta.Add(vagoneta);

        // TRAICIÓN: enciende la vagoneta de verdad, pero le saca púas al compañero.
        HerramientasEscena.CrearPalancaSospechosa("Palanca de la vagoneta", new Vector2(-11f, suelo), mueveVagoneta, TipoCastigo.SalenPuas, mecanismos);
        // TRAICIÓN: en el camino del piso 2. Se puede saltar, porque no hay techo.
        HerramientasEscena.CrearBotonTraicion("Boton del piso 2", new Vector2(5f, pisoDos), TipoCastigo.CaeLava, mecanismos);

        // Cristal para subir al piso 1, y cristales sobre el veneno alto.
        Transform cristales = HerramientasEscena.CrearGrupo("Cristales de luz");
        Vector2 tamanoCristal = new Vector2(1.6f, 0.4f);
        HerramientasEscena.CrearPlataformaOculta("Cristal del pozo", new Vector2(13.5f, -1.7f), tamanoCristal, cristales);
        float alturaCristal = pisoDos + 1.5f;
        HerramientasEscena.CrearPlataformaOculta("Cristal alto 1", new Vector2(2f, alturaCristal), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal alto 2", new Vector2(-1f, alturaCristal), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal alto 3", new Vector2(-4f, alturaCristal), tamanoCristal, cristales);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearRocaQueCae("Roca del piso 1", new Vector2(10f, 3.5f), pisoUno, 0.8f, peligros);
        HerramientasEscena.CrearMurcielago("Murcielago", new Vector2(-9f, 7.1f), new Vector2(2f, 1f), 5f, peligros);

        Transform faroles = HerramientasEscena.CrearGrupo("Faroles");
        HerramientasEscena.CrearFarol("Farol de la entrada", new Vector2(-12f, 1.5f), 4f, faroles);
        HerramientasEscena.CrearFarol("Farol del pozo", new Vector2(13f, 3f), 3.5f, faroles);
        HerramientasEscena.CrearFarol("Farol de la salida", new Vector2(-12f, 9f), 4f, faroles);

        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(0f, -1.4f), gemas);
        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(4f, -1.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-1f, 7f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(2f, 7f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(-13f, pisoDos), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(-11f, pisoDos), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-13.5f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-9.8f, suelo));
        HerramientasEscena.AgregarLuzPersonaje(inti);
        HerramientasEscena.AgregarLuzPersonaje(killa);

        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(2, 4);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }

    // ================= Mundo 2, Nivel 5: El corazón del Cerro Rico =================
    // Examen del mundo y el nivel más tramposo: caja entre los dos, vagoneta encendida
    // con una palanca traicionera, cristales de luz, rocas, murciélagos y trampas.

    private static string GenerarMundo2Nivel5()
    {
        HerramientasEscena.usarTilesetSal = false;
        Scene escena = CrearEscenaVacia();
        float suelo = HerramientasEscena.alturaSuelo;

        Camera camaraPrincipal = HerramientasEscena.CrearCamara(colorFondoMina);
        HerramientasEscena.CrearLuzGlobal(new Color(0.60f, 0.60f, 0.80f), 0.32f);
        HerramientasEscena.CrearGestorNivel("El corazón del Cerro Rico: nada es lo que parece. Inti, adelántate y alumbra quieto: si saltas, tu luz se aleja de los cristales.");
        HerramientasEscena.CrearFondoMina();

        float pisoUno = 1.2f;
        float pisoDos = 4.45f;
        float pisoTres = 7.7f;
        float mitadGrosor = 0.25f;
        float grosor = 0.5f;

        Transform escenario = HerramientasEscena.CrearSueloYParedes(colorSueloMina);
        HerramientasEscena.CrearPlataforma("Piso 1", new Vector2(11f, pisoUno - mitadGrosor), new Vector2(8f, grosor), colorSueloMina, escenario);
        // Orillas del piso 2: en medio solo se cruza en vagoneta.
        HerramientasEscena.CrearPlataforma("Piso 2 derecha", new Vector2(10f, pisoDos - mitadGrosor), new Vector2(4f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso 2 izquierda", new Vector2(-12f, pisoDos - mitadGrosor), new Vector2(6f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 inicio", new Vector2(-7.25f, pisoTres - mitadGrosor), new Vector2(4.5f, grosor), colorSueloMina, escenario);
        HerramientasEscena.CrearPlataforma("Piso 3 salida", new Vector2(8.5f, pisoTres - mitadGrosor), new Vector2(9f, grosor), colorSueloMina, escenario);

        Transform mecanismos = HerramientasEscena.CrearGrupo("Mecanismos");
        HerramientasEscena.CrearCajaEmpujable("Caja", new Vector2(-6f, suelo), -9f, 5.2f, mecanismos);
        // Flechas y marca en el piso: muestran adónde hay que llevar la caja.
        HerramientasEscena.CrearFlechaGuia("Flecha de la caja 1", new Vector2(-2f, -1.6f), true, mecanismos);
        HerramientasEscena.CrearFlechaGuia("Flecha de la caja 2", new Vector2(1f, -1.6f), true, mecanismos);
        HerramientasEscena.CrearDecoracion("Marca de la caja", HerramientasEscena.spriteCuadrado, new Vector2(5.2f, suelo + 0.04f), new Vector2(1.4f, 0.08f), new Color(1f, 0.85f, 0.25f), 1, false, mecanismos);

        // TRAICIÓN: la palanca enciende la vagoneta y le abre el suelo al compañero.
        PlataformaMovil vagoneta = HerramientasEscena.CrearVagoneta("Vagoneta", new Vector2(6.8f, 3.9f), -14.6f, 4f, false, mecanismos);
        List<ObjetoActivable> mueveVagoneta = new List<ObjetoActivable>();
        mueveVagoneta.Add(vagoneta);
        HerramientasEscena.CrearPalancaSospechosa("Palanca de la vagoneta", new Vector2(10f, pisoDos), mueveVagoneta, TipoCastigo.SeAbreElSuelo, mecanismos);
        // TRAICIÓN: antes de las puertas. Se puede saltar, porque no hay techo.
        HerramientasEscena.CrearBotonTraicion("Boton de la salida", new Vector2(7.2f, pisoTres), TipoCastigo.SalenPuas, mecanismos);

        // Cristales de luz entre los dos tramos del piso 3.
        Transform cristales = HerramientasEscena.CrearGrupo("Cristales de luz");
        Vector2 tamanoCristal = new Vector2(1.6f, 0.4f);
        HerramientasEscena.CrearPlataformaOculta("Cristal 1", new Vector2(-2.8f, pisoTres - 0.2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal 2", new Vector2(0f, pisoTres - 0.2f), tamanoCristal, cristales);
        HerramientasEscena.CrearPlataformaOculta("Cristal 3", new Vector2(2.6f, pisoTres - 0.2f), tamanoCristal, cristales);

        Transform trampas = HerramientasEscena.CrearGrupo("Trampas");
        // TRAMPA: púas en el piso 1, antes del salto al piso 2.
        HerramientasEscena.CrearPuasOcultas("Piso tranquilo", 11f, 1.5f, pisoUno, true, trampas);
        // TRAMPA: gema regalada antes de la salida, con un bloque arriba.
        TechoQueCae bloqueDeLaGema = HerramientasEscena.CrearTechoQueCae("Bloque del techo", new Vector2(9.5f, 10.5f), new Vector2(1.5f, grosor), pisoTres, false, colorSueloMina, escenario);
        List<TrampaActivable> trampasDeLaGema = new List<TrampaActivable>();
        trampasDeLaGema.Add(bloqueDeLaGema);
        Transform gemas = HerramientasEscena.CrearGrupo("Gemas");
        HerramientasEscena.CrearGemaTrampa("Gema regalada", TipoElemento.Fuego, new Vector2(9.5f, 8.75f), trampasDeLaGema, gemas);

        Transform peligros = HerramientasEscena.CrearGrupo("Peligros");
        HerramientasEscena.CrearRocaQueCae("Roca del piso 2", new Vector2(-12f, 7f), pisoDos, 0.8f, peligros);
        HerramientasEscena.CrearRocaQueCae("Roca del piso 1", new Vector2(13.5f, 3.6f), pisoUno, 0.8f, peligros);
        HerramientasEscena.CrearMurcielago("Murcielago de abajo", new Vector2(-1f, 0.5f), new Vector2(3f, 0.8f), 6f, peligros);
        HerramientasEscena.CrearMurcielago("Murcielago de arriba", new Vector2(-7f, 10.2f), new Vector2(2f, 0.8f), 5f, peligros);

        Transform faroles = HerramientasEscena.CrearGrupo("Faroles");
        HerramientasEscena.CrearFarol("Farol de la caja", new Vector2(-7f, 1.5f), 4f, faroles);
        HerramientasEscena.CrearFarol("Farol del piso 2", new Vector2(11f, 7f), 3.5f, faroles);
        HerramientasEscena.CrearFarol("Farol de la salida", new Vector2(11f, 11.5f), 3.5f, faroles);

        HerramientasEscena.CrearGema("Gema agua 1", TipoElemento.Agua, new Vector2(-1f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 1", TipoElemento.Fuego, new Vector2(2f, -2.4f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 2", TipoElemento.Fuego, new Vector2(-1f, 5.2f), gemas);
        HerramientasEscena.CrearGema("Gema agua 2", TipoElemento.Agua, new Vector2(-4f, 5.2f), gemas);
        HerramientasEscena.CrearGema("Gema fuego 3", TipoElemento.Fuego, new Vector2(0f, 8.5f), gemas);
        HerramientasEscena.CrearGema("Gema agua 3", TipoElemento.Agua, new Vector2(2.6f, 8.5f), gemas);

        Transform puertas = HerramientasEscena.CrearGrupo("Puertas");
        HerramientasEscena.CrearPuertaSalida("Puerta fuego", TipoElemento.Fuego, new Vector2(12f, pisoTres), puertas);
        HerramientasEscena.CrearPuertaSalida("Puerta agua", TipoElemento.Agua, new Vector2(10.9f, pisoTres), puertas);

        GameObject inti = HerramientasEscena.CrearInti(new Vector2(-9f, suelo));
        GameObject killa = HerramientasEscena.CrearKilla(new Vector2(-11f, suelo));
        HerramientasEscena.AgregarLuzPersonaje(inti);
        HerramientasEscena.AgregarLuzPersonaje(killa);

        HerramientasEscena.CrearCamaraCooperativa(camaraPrincipal, inti.transform, killa.transform);

        string ruta = RutaNivel(2, 5);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
