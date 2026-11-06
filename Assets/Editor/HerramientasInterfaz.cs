// =====================================================================
// HerramientasInterfaz.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR con las piezas de los menús: fondo por mundo,
//              tarjeta de mundo, cuadro de nivel, flechas y franja de ayuda.
//              Cada imagen se busca primero en 03_Sprites/Interfaz con su nombre:
//                TarjetaMundo1..6, FondoMundo1..6, Estrella, Candado, Flecha
//              Si no está, se usa la provisional de GeneradorArteMenus.
// =====================================================================

using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class HerramientasInterfaz
{
    private static readonly Color colorPanelOscuro = new Color(0.03f, 0.02f, 0.07f, 0.92f);
    private static readonly Color colorTitulo = new Color(1f, 0.92f, 0.70f);
    private static readonly Color colorEstrella = new Color(1f, 0.82f, 0.20f);

    // ================= Imágenes =================

    private static Sprite ObtenerImagenTarjeta(int numeroMundo)
    {
        Sprite arte = HerramientasEscena.CargarArteInterfaz("TarjetaMundo" + numeroMundo);
        if (arte != null)
        {
            return arte;
        }
        return GeneradorArteMenus.ObtenerTarjeta(numeroMundo);
    }

    private static Sprite ObtenerImagenFondo(int numeroMundo)
    {
        Sprite arte = HerramientasEscena.CargarArteInterfaz("FondoMundo" + numeroMundo);
        if (arte != null)
        {
            return arte;
        }
        return GeneradorArteMenus.ObtenerFondo(numeroMundo);
    }

    private static Sprite ObtenerImagenEstrella()
    {
        Sprite arte = HerramientasEscena.CargarArteInterfaz("Estrella");
        if (arte != null)
        {
            return arte;
        }
        return GeneradorArteMenus.ObtenerEstrella();
    }

    private static Sprite ObtenerImagenCandado()
    {
        Sprite arte = HerramientasEscena.CargarArteInterfaz("Candado");
        if (arte != null)
        {
            return arte;
        }
        return GeneradorArteMenus.ObtenerCandado();
    }

    private static Sprite ObtenerImagenFlecha()
    {
        Sprite arte = HerramientasEscena.CargarArteInterfaz("Flecha");
        if (arte != null)
        {
            return arte;
        }
        return GeneradorArteMenus.ObtenerFlecha();
    }

    // ================= Piezas básicas =================

    // Imagen decorativa: no recibe clics para no tapar a los botones de abajo.
    public static Image CrearImagen(Transform padre, string nombre, Sprite sprite, Color color, Vector2 posicion, Vector2 tamano)
    {
        GameObject objeto = new GameObject(nombre, typeof(RectTransform));
        objeto.transform.SetParent(padre, false);

        RectTransform rectangulo = objeto.GetComponent<RectTransform>();
        rectangulo.anchoredPosition = posicion;
        rectangulo.sizeDelta = tamano;

        Image imagen = objeto.AddComponent<UnityEngine.UI.Image>();
        imagen.sprite = sprite;
        imagen.color = color;
        imagen.raycastTarget = false;
        if (sprite != null && sprite.border != Vector4.zero)
        {
            imagen.type = Image.Type.Sliced;
        }
        return imagen;
    }

    private static void EstirarSobrePadre(RectTransform rectangulo, float margen)
    {
        rectangulo.anchorMin = Vector2.zero;
        rectangulo.anchorMax = Vector2.one;
        rectangulo.pivot = new Vector2(0.5f, 0.5f);
        rectangulo.offsetMin = new Vector2(margen, margen);
        rectangulo.offsetMax = new Vector2(-margen, -margen);
    }

    private static GameObject CrearGrupo(Transform padre, string nombre)
    {
        GameObject grupo = new GameObject(nombre, typeof(RectTransform));
        grupo.transform.SetParent(padre, false);
        EstirarSobrePadre(grupo.GetComponent<RectTransform>(), 0f);
        return grupo;
    }

    private static Text CrearTextoFuerte(Transform padre, string nombre, string contenido, int tamanoLetra, Vector2 posicion, Vector2 tamanoCaja, Color color)
    {
        Text texto = HerramientasEscena.CrearTexto(padre, nombre, contenido, tamanoLetra, posicion, tamanoCaja, color);
        texto.fontStyle = FontStyle.Bold;
        texto.raycastTarget = false;

        // El contorno mantiene la letra legible sobre cualquier parte del paisaje.
        Outline contorno = texto.gameObject.AddComponent<Outline>();
        contorno.effectColor = new Color(0.08f, 0.05f, 0.12f, 0.9f);
        contorno.effectDistance = new Vector2(3f, -3f);
        return texto;
    }

    public static Text CrearTitulo(Transform canvas, string contenido, int tamanoLetra, Vector2 posicion)
    {
        return CrearTextoFuerte(canvas, "Titulo", contenido, tamanoLetra, posicion, new Vector2(1500f, 130f), colorTitulo);
    }

    // Los colores cambian poco porque el dibujo es lo importante. Desactivado no
    // cambia nada: las piezas bloqueadas ya se ven apagadas por su candado.
    private static void ConfigurarBoton(Button boton, Graphic dibujoQueCambia)
    {
        boton.targetGraphic = dibujoQueCambia;

        ColorBlock colores = boton.colors;
        colores.normalColor = new Color(0.93f, 0.93f, 0.93f);
        colores.highlightedColor = Color.white;
        colores.selectedColor = new Color(0.93f, 0.93f, 0.93f);
        colores.pressedColor = new Color(0.72f, 0.72f, 0.72f);
        colores.disabledColor = Color.white;
        boton.colors = colores;

        // Sin navegación por teclado: las flechas ya mueven el carrusel.
        Navigation navegacion = new Navigation();
        navegacion.mode = Navigation.Mode.None;
        boton.navigation = navegacion;
    }

    // ================= Fondo y ayuda =================

    public static FondoMundo CrearFondoMundo(Transform canvas)
    {
        GameObject objeto = CrearGrupo(canvas, "Fondo del mundo");
        Image imagenAnterior = CrearImagenFondoCompleta(objeto.transform, "Paisaje anterior");
        Image imagenActual = CrearImagenFondoCompleta(objeto.transform, "Paisaje actual");

        int cantidadMundos = DatosJuego.CantidadDeMundos();
        Sprite[] fondos = new Sprite[cantidadMundos];
        for (int indice = 0; indice < cantidadMundos; indice++)
        {
            fondos[indice] = ObtenerImagenFondo(indice + 1);
        }

        imagenAnterior.sprite = fondos[0];
        imagenActual.sprite = fondos[0];

        FondoMundo fondo = objeto.AddComponent<FondoMundo>();
        fondo.Configurar(imagenAnterior, imagenActual, fondos);
        return fondo;
    }

    private static Image CrearImagenFondoCompleta(Transform padre, string nombre)
    {
        Image imagen = CrearImagen(padre, nombre, null, Color.white, Vector2.zero, Vector2.zero);

        // Cubre toda la pantalla sin deformarse, sea cual sea su proporción.
        AspectRatioFitter ajuste = imagen.gameObject.AddComponent<AspectRatioFitter>();
        ajuste.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        ajuste.aspectRatio = 16f / 9f;
        return imagen;
    }

    public static void CrearFranjaAyuda(Transform canvas, string contenido)
    {
        Image franja = CrearImagen(canvas, "Franja de ayuda", null, new Color(0f, 0f, 0f, 0.88f), Vector2.zero, Vector2.zero);
        RectTransform rectangulo = franja.rectTransform;
        rectangulo.anchorMin = Vector2.zero;
        rectangulo.anchorMax = new Vector2(1f, 0f);
        rectangulo.pivot = new Vector2(0.5f, 0f);
        rectangulo.anchoredPosition = Vector2.zero;
        rectangulo.sizeDelta = new Vector2(0f, 50f);

        Text texto = HerramientasEscena.CrearTexto(franja.transform, "Texto", contenido, 26, Vector2.zero, new Vector2(1800f, 50f), Color.white);
        texto.raycastTarget = false;
    }

    // ================= Flechas =================

    public static Button CrearFlecha(Transform padre, string nombre, Vector2 ancla, Vector2 posicion, float tamano, bool haciaDerecha, Color color, UnityAction accion)
    {
        Image circulo = CrearImagen(padre, nombre, HerramientasEscena.spriteDisco, color, posicion, new Vector2(tamano, tamano));
        circulo.raycastTarget = true;

        RectTransform rectangulo = circulo.rectTransform;
        rectangulo.anchorMin = ancla;
        rectangulo.anchorMax = ancla;
        rectangulo.pivot = new Vector2(0.5f, 0.5f);
        rectangulo.anchoredPosition = posicion;

        Image icono = CrearImagen(circulo.transform, "Icono", ObtenerImagenFlecha(), Color.white, Vector2.zero, new Vector2(tamano * 0.62f, tamano * 0.62f));
        if (haciaDerecha == true)
        {
            icono.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        }

        Button boton = circulo.gameObject.AddComponent<Button>();
        ConfigurarBoton(boton, circulo);
        circulo.gameObject.AddComponent<EscalaAlPasarMouse>();
        UnityEventTools.AddPersistentListener(boton.onClick, accion);
        return boton;
    }

    // Flecha redonda para volver a la pantalla anterior, abajo a la izquierda.
    public static Button CrearFlechaVolver(Transform canvas, UnityAction accion)
    {
        return CrearFlecha(canvas, "Boton Volver", Vector2.zero, new Vector2(100f, 120f), 120f, false, new Color(0.30f, 0.26f, 0.36f), accion);
    }

    // ================= Tarjeta de mundo =================

    public static RectTransform CrearTarjetaMundo(Transform contenido, int numeroMundo)
    {
        Color colorMundo = ColoresMundos.ObtenerColor(numeroMundo);
        Sprite redondeado = GeneradorArteMenus.ObtenerRedondeado();
        Vector2 tamanoTarjeta = new Vector2(420f, 660f);

        // El marco es lo que recibe el clic.
        Image marco = CrearImagen(contenido, "Tarjeta Mundo " + numeroMundo, redondeado, Color.Lerp(colorMundo, Color.white, 0.25f), Vector2.zero, tamanoTarjeta);
        marco.raycastTarget = true;
        RectTransform rectangulo = marco.rectTransform;
        rectangulo.anchorMin = new Vector2(0f, 0.5f);
        rectangulo.anchorMax = new Vector2(0f, 0.5f);
        rectangulo.anchoredPosition = new Vector2(0f, -5f);

        // El recorte redondea las esquinas del dibujo y de lo que va encima.
        Image recorte = CrearImagen(marco.transform, "Recorte", redondeado, Color.white, Vector2.zero, Vector2.zero);
        EstirarSobrePadre(recorte.rectTransform, 10f);
        Mask mascara = recorte.gameObject.AddComponent<Mask>();
        mascara.showMaskGraphic = false;

        Image ilustracion = CrearImagen(recorte.transform, "Ilustracion", ObtenerImagenTarjeta(numeroMundo), Color.white, Vector2.zero, Vector2.zero);
        EstirarSobrePadre(ilustracion.rectTransform, 0f);

        // Candado y oscurecido: tapan el dibujo, pero no el panel con el nombre.
        GameObject grupoBloqueado = CrearGrupo(recorte.transform, "Bloqueado");
        Image capaOscura = CrearImagen(grupoBloqueado.transform, "Capa oscura", null, new Color(0.04f, 0.04f, 0.10f, 0.62f), Vector2.zero, Vector2.zero);
        EstirarSobrePadre(capaOscura.rectTransform, 0f);
        CrearImagen(grupoBloqueado.transform, "Candado", ObtenerImagenCandado(), new Color(0.90f, 0.90f, 0.95f), new Vector2(0f, 70f), new Vector2(170f, 170f));

        // Panel inferior con el nombre y las estrellas.
        Image panel = CrearImagen(recorte.transform, "Panel", null, colorPanelOscuro, Vector2.zero, Vector2.zero);
        RectTransform rectanguloPanel = panel.rectTransform;
        rectanguloPanel.anchorMin = Vector2.zero;
        rectanguloPanel.anchorMax = new Vector2(1f, 0f);
        rectanguloPanel.pivot = new Vector2(0.5f, 0f);
        rectanguloPanel.anchoredPosition = Vector2.zero;
        rectanguloPanel.sizeDelta = new Vector2(0f, 160f);

        CrearTextoFuerte(panel.transform, "Nombre", DatosJuego.ObtenerNombreMundo(numeroMundo), 42, new Vector2(0f, 38f), new Vector2(390f, 60f), Color.white);

        GameObject grupoEstrellas = CrearGrupo(panel.transform, "Estrellas");
        CrearImagen(grupoEstrellas.transform, "Estrella", ObtenerImagenEstrella(), colorEstrella, new Vector2(-62f, -36f), new Vector2(58f, 58f));
        Text textoEstrellas = CrearTextoFuerte(grupoEstrellas.transform, "Cantidad", "0/15", 44, new Vector2(30f, -36f), new Vector2(130f, 60f), Color.white);

        // Ocupa el lugar de las estrellas mientras el mundo está bloqueado.
        Text textoBloqueado = CrearTextoFuerte(panel.transform, "Requisito", "", 34, new Vector2(0f, -36f), new Vector2(390f, 50f), new Color(1f, 0.88f, 0.55f));

        // Insignia con el número del mundo.
        Image insignia = CrearImagen(marco.transform, "Insignia", HerramientasEscena.spriteDisco, colorMundo, new Vector2(-145f, 265f), new Vector2(96f, 96f));
        CrearTextoFuerte(insignia.transform, "Numero", numeroMundo.ToString(), 56, Vector2.zero, new Vector2(96f, 96f), Color.white);

        Button boton = marco.gameObject.AddComponent<Button>();
        ConfigurarBoton(boton, ilustracion);

        BotonMundo botonMundo = marco.gameObject.AddComponent<BotonMundo>();
        botonMundo.Configurar(numeroMundo, textoEstrellas, grupoEstrellas, grupoBloqueado, textoBloqueado);
        return rectangulo;
    }

    // ================= Cuadro de nivel =================

    public static void CrearCuadroNivel(Transform canvas, int numeroEnMundo, Vector2 posicion)
    {
        Sprite redondeado = GeneradorArteMenus.ObtenerRedondeado();
        Vector2 tamano = new Vector2(300f, 300f);

        GameObject raiz = new GameObject("Nivel " + numeroEnMundo, typeof(RectTransform));
        raiz.transform.SetParent(canvas, false);
        raiz.GetComponent<RectTransform>().anchoredPosition = posicion;

        Image cuadro = CrearImagen(raiz.transform, "Cuadro", redondeado, Color.white, Vector2.zero, tamano);
        cuadro.raycastTarget = true;

        // Un borde claro separa el cuadro del paisaje, sea del color que sea.
        Image interior = CrearImagen(cuadro.transform, "Brillo", redondeado, new Color(1f, 1f, 1f, 0.07f), Vector2.zero, Vector2.zero);
        EstirarSobrePadre(interior.rectTransform, 12f);

        Text textoNumero = CrearTextoFuerte(cuadro.transform, "Numero", numeroEnMundo.ToString(), 130, new Vector2(0f, 30f), new Vector2(280f, 170f), Color.white);
        Text textoPronto = CrearTextoFuerte(cuadro.transform, "Pronto", "Pronto", 54, Vector2.zero, new Vector2(280f, 80f), new Color(0.88f, 0.88f, 0.92f));
        Image candado = CrearImagen(cuadro.transform, "Candado", ObtenerImagenCandado(), new Color(0.85f, 0.85f, 0.90f), Vector2.zero, new Vector2(140f, 140f));

        // Tres estrellas; la del medio un poco más alta y grande.
        Sprite spriteEstrella = ObtenerImagenEstrella();
        Image[] estrellas = new Image[DatosJuego.estrellasPorNivel];
        estrellas[0] = CrearImagen(cuadro.transform, "Estrella 1", spriteEstrella, Color.white, new Vector2(-78f, -92f), new Vector2(68f, 68f));
        estrellas[1] = CrearImagen(cuadro.transform, "Estrella 2", spriteEstrella, Color.white, new Vector2(0f, -84f), new Vector2(80f, 80f));
        estrellas[2] = CrearImagen(cuadro.transform, "Estrella 3", spriteEstrella, Color.white, new Vector2(78f, -92f), new Vector2(68f, 68f));

        // El título va debajo, sobre un panel para que se lea encima del paisaje.
        Image panelTitulo = CrearImagen(raiz.transform, "Panel del titulo", redondeado, colorPanelOscuro, new Vector2(0f, -210f), new Vector2(300f, 100f));
        panelTitulo.pixelsPerUnitMultiplier = 2f;
        Text textoTitulo = HerramientasEscena.CrearTexto(panelTitulo.transform, "Titulo", "", 30, Vector2.zero, new Vector2(280f, 90f), Color.white);
        textoTitulo.horizontalOverflow = HorizontalWrapMode.Wrap;
        textoTitulo.raycastTarget = false;

        Button boton = cuadro.gameObject.AddComponent<Button>();
        ConfigurarBoton(boton, cuadro);
        cuadro.gameObject.AddComponent<EscalaAlPasarMouse>();

        BotonNivel botonNivel = cuadro.gameObject.AddComponent<BotonNivel>();
        botonNivel.Configurar(numeroEnMundo, cuadro, textoNumero, textoTitulo, textoPronto, candado.gameObject, estrellas);
    }

    // Fila de una estrella y su cantidad, centrada en la posición dada.
    public static Text CrearContadorEstrellas(Transform canvas, Vector2 posicion)
    {
        CrearImagen(canvas, "Estrella total", ObtenerImagenEstrella(), colorEstrella, posicion + new Vector2(-70f, 0f), new Vector2(64f, 64f));
        return CrearTextoFuerte(canvas, "Estrellas del mundo", "0/15", 50, posicion + new Vector2(28f, 0f), new Vector2(150f, 70f), Color.white);
    }
}
