// =====================================================================
// HerramientasEscena.cs
// Proyecto: Inti y Killa
// Descripción: "Caja de herramientas" del generador de escenas.
//              Tiene un método para crear cada pieza del juego (suelo,
//              charcos, gemas, puertas, jugadores, botones, palancas,
//              altares, fondo del Salar, botones de interfaz...).
//              El GeneradorEscenas usa estas piezas como bloques de Lego
//              para armar cada nivel.
// Ubicación obligatoria: Assets/Editor/ (solo funciona dentro de Unity).
// =====================================================================

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public static class HerramientasEscena
{
    // ================= Rutas =================
    public const string carpetaEscenas = "Assets/_Proyecto/00_Scenes";
    public const string carpetaNiveles = "Assets/_Proyecto/00_Scenes/Niveles";
    private const string carpetaSpritesTemporales = "Assets/_Proyecto/03_Sprites/Temporales";
    private const string carpetaSpritesUyuni = "Assets/_Proyecto/03_Sprites/Fondos/Uyuni";
    private const string rutaMaterialFisico = "Assets/_Proyecto/Materiales/SinFriccion.physicsMaterial2D";

    // Altura de la superficie del suelo en todos los niveles.
    public const float alturaSuelo = -3.5f;

    // ================= Colores =================
    public static readonly Color colorSal = new Color(0.98f, 0.94f, 0.92f);
    public static readonly Color colorSalQuebradiza = new Color(0.86f, 0.74f, 0.72f);
    public static readonly Color colorPiedra = new Color(0.45f, 0.38f, 0.48f);
    public static readonly Color colorLava = new Color(1.00f, 0.30f, 0.05f);
    public static readonly Color colorAgua = new Color(0.00f, 0.25f, 0.80f);
    public static readonly Color colorVeneno = new Color(0.35f, 0.85f, 0.20f);
    public static readonly Color colorCristal = new Color(0.90f, 0.95f, 1.00f);
    private static readonly Color colorMonticulo = new Color(1.00f, 0.92f, 0.86f);
    private static readonly Color colorCordillera = new Color(0.30f, 0.18f, 0.34f);
    private static readonly Color colorSol = new Color(1.00f, 0.88f, 0.55f, 0.95f);
    private static readonly Color colorBrilloSol = new Color(1.00f, 0.65f, 0.35f, 0.30f);

    // ================= Sprites base =================
    // Se guardan aquí para no buscarlos en disco cada vez.
    public static Sprite spriteCuadrado;
    public static Sprite spriteCirculo;
    public static PhysicsMaterial2D materialSinFriccion;

    // Debe llamarse antes de crear cualquier escena.
    public static void PrepararRecursos()
    {
        CrearCarpetaSiNoExiste("Assets/_Proyecto", "00_Scenes");
        CrearCarpetaSiNoExiste(carpetaEscenas, "Niveles");
        CrearCarpetaSiNoExiste("Assets/_Proyecto/03_Sprites", "Temporales");
        CrearCarpetaSiNoExiste("Assets/_Proyecto/03_Sprites", "Fondos");
        CrearCarpetaSiNoExiste("Assets/_Proyecto/03_Sprites/Fondos", "Uyuni");
        CrearCarpetaSiNoExiste("Assets/_Proyecto", "Materiales");

        spriteCuadrado = ObtenerSpriteBasico("Cuadrado", false);
        spriteCirculo = ObtenerSpriteBasico("Circulo", true);
        materialSinFriccion = ObtenerMaterialSinFriccion();
    }

    // ================= Objetos básicos de una escena =================

    public static void CrearCamara(Color colorFondo)
    {
        GameObject objetoCamara = new GameObject("Main Camera");
        objetoCamara.tag = "MainCamera";
        objetoCamara.transform.position = new Vector3(0f, 0.5f, -10f);

        Camera camara = objetoCamara.AddComponent<Camera>();
        camara.orthographic = true;
        camara.orthographicSize = 8.5f;
        camara.clearFlags = CameraClearFlags.SolidColor;
        camara.backgroundColor = colorFondo;

        objetoCamara.AddComponent<AudioListener>();
    }

    public static void CrearLuzGlobal(Color colorLuz, float intensidad)
    {
        // En URP 2D los sprites necesitan luz para verse (si no, salen negros).
        GameObject objetoLuz = new GameObject("Luz global 2D");
        Light2D luz = objetoLuz.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Global;
        luz.color = colorLuz;
        luz.intensity = intensidad;
    }

    // La pista es el texto de ayuda que aparece al empezar el nivel.
    public static void CrearGestorNivel(string pista)
    {
        GameObject objetoGestor = new GameObject("GestorNivel");
        GestorNivel gestor = objetoGestor.AddComponent<GestorNivel>();
        gestor.ConfigurarPista(pista);
    }

    // Suelo de lado a lado + paredes invisibles para no salirse de la pantalla.
    public static Transform CrearSueloYParedes(Color colorSuelo)
    {
        GameObject escenario = new GameObject("Escenario");
        CrearBloque("Suelo", new Vector2(0f, alturaSuelo - 0.5f), new Vector2(32f, 1f), colorSuelo, escenario.transform);

        GameObject paredIzquierda = CrearBloque("Pared invisible izquierda", new Vector2(-15.5f, 3f), new Vector2(1f, 16f), colorSuelo, escenario.transform);
        paredIzquierda.GetComponent<SpriteRenderer>().enabled = false;

        GameObject paredDerecha = CrearBloque("Pared invisible derecha", new Vector2(15.5f, 3f), new Vector2(1f, 16f), colorSuelo, escenario.transform);
        paredDerecha.GetComponent<SpriteRenderer>().enabled = false;

        return escenario.transform;
    }

    public static Transform CrearGrupo(string nombre)
    {
        GameObject grupo = new GameObject(nombre);
        return grupo.transform;
    }

    // ================= Piezas del nivel =================

    // Bloque sólido (se puede pisar).
    public static GameObject CrearBloque(string nombre, Vector2 posicion, Vector2 tamano, Color color, Transform padre)
    {
        GameObject bloque = new GameObject(nombre);
        bloque.transform.SetParent(padre);
        bloque.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        bloque.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);

        SpriteRenderer dibujo = bloque.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCuadrado;
        dibujo.color = color;

        bloque.AddComponent<BoxCollider2D>();
        return bloque;
    }

    // Charco sobre el suelo: centroX y ancho, siempre a la altura del suelo.
    public static void CrearCharco(string nombre, TipoZona tipo, float centroX, float ancho, Transform padre)
    {
        Color color = colorLava;
        if (tipo == TipoZona.Agua)
        {
            color = colorAgua;
        }
        else if (tipo == TipoZona.Veneno)
        {
            color = colorVeneno;
        }

        Vector2 posicion = new Vector2(centroX, alturaSuelo + 0.15f);
        GameObject charco = CrearBloque(nombre, posicion, new Vector2(ancho, 0.3f), color, padre);
        charco.GetComponent<SpriteRenderer>().sortingOrder = 1;
        charco.GetComponent<BoxCollider2D>().isTrigger = true;

        ZonaElemental zona = charco.AddComponent<ZonaElemental>();
        zona.ConfigurarZona(tipo);
    }

    public static void CrearGema(string nombre, TipoElemento elemento, Vector2 posicion, Transform padre)
    {
        GameObject gema = new GameObject(nombre);
        gema.transform.SetParent(padre);
        gema.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        gema.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        SpriteRenderer dibujo = gema.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = ColoresElemento.ObtenerColor(elemento);
        dibujo.sortingOrder = 4;

        CircleCollider2D colisionador = gema.AddComponent<CircleCollider2D>();
        colisionador.isTrigger = true;

        Gema componenteGema = gema.AddComponent<Gema>();
        componenteGema.ConfigurarGema(elemento);
    }

    // posicionSuelo = punto donde la puerta toca el piso.
    public static void CrearPuertaSalida(string nombre, TipoElemento elemento, Vector2 posicionSuelo, Transform padre)
    {
        Color colorElemento = ColoresElemento.ObtenerColor(elemento);
        Color colorPuerta = new Color(colorElemento.r, colorElemento.g, colorElemento.b, 0.65f);

        Vector2 centro = new Vector2(posicionSuelo.x, posicionSuelo.y + 1f);
        GameObject puerta = CrearBloque(nombre, centro, new Vector2(1.2f, 2f), colorPuerta, padre);
        puerta.GetComponent<SpriteRenderer>().sortingOrder = -1;
        puerta.GetComponent<BoxCollider2D>().isTrigger = true;

        PuertaSalida componentePuerta = puerta.AddComponent<PuertaSalida>();
        componentePuerta.ConfigurarPuerta(elemento);
    }

    // posicionSuelo = punto donde el jugador toca el piso al empezar.
    public static void CrearJugador(string nombre, TipoElemento elemento, Key izquierda, Key derecha, Key saltar, Key habilidad, Vector2 posicionSuelo, Color colorMarca)
    {
        float anchoJugador = 0.8f;
        float altoJugador = 1.2f;

        GameObject jugador = new GameObject(nombre);
        jugador.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y + altoJugador / 2f + 0.05f, 0f);
        jugador.transform.localScale = new Vector3(anchoJugador, altoJugador, 1f);

        SpriteRenderer dibujo = jugador.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCuadrado;
        dibujo.color = ColoresElemento.ObtenerColor(elemento);
        dibujo.sortingOrder = 10;

        Rigidbody2D cuerpo = jugador.AddComponent<Rigidbody2D>();
        cuerpo.gravityScale = 3f;
        cuerpo.freezeRotation = true;
        cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        cuerpo.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Sin fricción, para que no se queden "pegados" a las paredes.
        BoxCollider2D colisionador = jugador.AddComponent<BoxCollider2D>();
        colisionador.sharedMaterial = materialSinFriccion;

        ControladorJugador controlador = jugador.AddComponent<ControladorJugador>();
        controlador.ConfigurarJugador(nombre, elemento, izquierda, derecha, saltar, habilidad);

        // Marca de identidad sobre la cabeza: NO cambia con el eclipse,
        // así siempre se sabe quién es Inti y quién es Killa.
        GameObject marca = new GameObject("Marca de identidad");
        marca.transform.SetParent(jugador.transform);
        marca.transform.localPosition = new Vector3(0f, 0.72f, 0f);
        marca.transform.localScale = new Vector3(0.35f / anchoJugador, 0.35f / altoJugador, 1f);
        SpriteRenderer dibujoMarca = marca.AddComponent<SpriteRenderer>();
        dibujoMarca.sprite = spriteCirculo;
        dibujoMarca.color = colorMarca;
        dibujoMarca.sortingOrder = 11;
    }

    // Los dos protagonistas en su posición inicial.
    public static void CrearInti(Vector2 posicionSuelo)
    {
        Color colorSolDorado = new Color(1.00f, 0.95f, 0.45f);
        CrearJugador("Inti", TipoElemento.Fuego, Key.A, Key.D, Key.W, Key.S, posicionSuelo, colorSolDorado);
    }

    public static void CrearKilla(Vector2 posicionSuelo)
    {
        Color colorLunaPlateada = new Color(0.85f, 0.90f, 1.00f);
        CrearJugador("Killa", TipoElemento.Agua, Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow, posicionSuelo, colorLunaPlateada);
    }

    // Púas de sal centradas en centroX, sobre el suelo. Matan a los dos.
    public static void CrearCristalesDeSal(string nombre, float centroX, float ancho, Transform padre)
    {
        GameObject zona = new GameObject(nombre);
        zona.transform.SetParent(padre);
        zona.transform.position = new Vector3(centroX, alturaSuelo + 0.25f, 0f);

        BoxCollider2D colisionador = zona.AddComponent<BoxCollider2D>();
        colisionador.isTrigger = true;
        colisionador.size = new Vector2(ancho, 0.45f);

        ZonaElemental componenteZona = zona.AddComponent<ZonaElemental>();
        componenteZona.ConfigurarZona(TipoZona.CristalesSal);

        Sprite spriteCristal = ObtenerSpriteCristal();
        float anchoCristal = 0.5f;
        int cantidadCristales = Mathf.RoundToInt(ancho / anchoCristal);

        for (int indice = 0; indice < cantidadCristales; indice++)
        {
            float posicionX = centroX - ancho / 2f + anchoCristal / 2f + indice * anchoCristal;
            Vector2 centroCristal = new Vector2(posicionX, alturaSuelo + 0.3f);
            CrearDecoracion("Cristal " + (indice + 1), spriteCristal, centroCristal, new Vector2(anchoCristal, 0.6f), colorCristal, 3, false, zona.transform);
        }
    }

    public static void CrearPlataformaQuebradiza(string nombre, Vector2 posicion, Vector2 tamano, Transform padre)
    {
        GameObject plataforma = CrearBloque(nombre, posicion, tamano, colorSalQuebradiza, padre);
        plataforma.AddComponent<PlataformaQuebradiza>();
    }

    // Compuerta de piedra que se abre con botones o palancas.
    public static PuertaMecanica CrearPuertaMecanica(string nombre, Vector2 posicion, Vector2 tamano, Vector2 desplazamientoAlAbrir, int senalesNecesarias, Transform padre)
    {
        GameObject bloque = CrearBloque(nombre, posicion, tamano, colorPiedra, padre);
        bloque.GetComponent<SpriteRenderer>().sortingOrder = 2;

        Rigidbody2D cuerpo = bloque.AddComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;

        PuertaMecanica puerta = bloque.AddComponent<PuertaMecanica>();
        puerta.Configurar(desplazamientoAlAbrir, 6f);
        puerta.ConfigurarSenalesNecesarias(senalesNecesarias);
        return puerta;
    }

    public static PlataformaMovil CrearPlataformaMovil(string nombre, Vector2 posicion, Vector2 tamano, Vector2 desplazamiento, float velocidad, bool moverseDesdeElInicio, Transform padre)
    {
        Color colorMadera = new Color(0.62f, 0.45f, 0.30f);
        GameObject bloque = CrearBloque(nombre, posicion, tamano, colorMadera, padre);

        Rigidbody2D cuerpo = bloque.AddComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;

        PlataformaMovil plataforma = bloque.AddComponent<PlataformaMovil>();
        plataforma.Configurar(desplazamiento, velocidad, moverseDesdeElInicio);
        return plataforma;
    }

    // Botón de presión apoyado en el suelo en la posición indicada.
    public static void CrearBotonPresion(string nombre, Vector2 posicionSuelo, List<ObjetoActivable> objetivos, Transform padre)
    {
        GameObject boton = new GameObject(nombre);
        boton.transform.SetParent(padre);
        boton.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y, 0f);

        // Zona que detecta a los jugadores (un poco más alta que el botón).
        BoxCollider2D zona = boton.AddComponent<BoxCollider2D>();
        zona.isTrigger = true;
        zona.size = new Vector2(1.2f, 0.6f);
        zona.offset = new Vector2(0f, 0.3f);

        CrearDibujoHijo("Base", boton.transform, new Vector2(0f, 0.05f), new Vector2(1.6f, 0.1f), new Color(0.30f, 0.28f, 0.32f), 2);
        GameObject tapa = CrearDibujoHijo("Tapa", boton.transform, new Vector2(0f, 0.18f), new Vector2(1.1f, 0.18f), Color.yellow, 3);

        BotonPresion componente = boton.AddComponent<BotonPresion>();
        componente.Configurar(objetivos, tapa.GetComponent<SpriteRenderer>());
    }

    // Palanca apoyada en el suelo en la posición indicada.
    public static void CrearPalanca(string nombre, Vector2 posicionSuelo, List<ObjetoActivable> objetivos, Transform padre)
    {
        GameObject palanca = new GameObject(nombre);
        palanca.transform.SetParent(padre);
        palanca.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y, 0f);

        BoxCollider2D zona = palanca.AddComponent<BoxCollider2D>();
        zona.isTrigger = true;
        zona.size = new Vector2(1.6f, 1.4f);
        zona.offset = new Vector2(0f, 0.7f);

        CrearDibujoHijo("Base", palanca.transform, new Vector2(0f, 0.12f), new Vector2(0.9f, 0.24f), new Color(0.30f, 0.28f, 0.32f), 3);

        // El pivote gira y lleva el mango, como una bisagra.
        GameObject pivote = new GameObject("Pivote");
        pivote.transform.SetParent(palanca.transform);
        pivote.transform.localPosition = new Vector3(0f, 0.2f, 0f);

        GameObject mango = CrearDibujoHijo("Mango", pivote.transform, new Vector2(0f, 0.4f), new Vector2(0.14f, 0.8f), Color.red, 2);

        Palanca componente = palanca.AddComponent<Palanca>();
        componente.Configurar(objetivos, pivote.transform, mango.GetComponent<SpriteRenderer>());
    }

    // Altar de eclipse: dos placas (pueden estar lejos) unidas por un mismo altar.
    public static void CrearAltarEclipse(string nombre, Vector2 posicionPlacaUno, Vector2 posicionPlacaDos, Color colorAltar, Transform padre)
    {
        GameObject altar = new GameObject(nombre);
        altar.transform.SetParent(padre);

        PlacaAltar placaUno = CrearPlacaAltar("Placa del Sol", posicionPlacaUno, colorAltar, altar.transform);
        PlacaAltar placaDos = CrearPlacaAltar("Placa de la Luna", posicionPlacaDos, colorAltar, altar.transform);

        AltarEclipse componente = altar.AddComponent<AltarEclipse>();
        componente.Configurar(placaUno, placaDos);
    }

    private static PlacaAltar CrearPlacaAltar(string nombre, Vector2 posicionSuelo, Color colorAltar, Transform padre)
    {
        GameObject placa = new GameObject(nombre);
        placa.transform.SetParent(padre);
        placa.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y, 0f);

        BoxCollider2D zona = placa.AddComponent<BoxCollider2D>();
        zona.isTrigger = true;
        zona.size = new Vector2(1.2f, 0.8f);
        zona.offset = new Vector2(0f, 0.4f);

        GameObject piso = CrearDibujoHijo("Piso del altar", placa.transform, new Vector2(0f, 0.07f), new Vector2(1.5f, 0.14f), colorAltar, 2);

        // Símbolo flotante (sol/luna) para que se vea desde lejos.
        GameObject simbolo = CrearDibujoHijo("Simbolo", placa.transform, new Vector2(0f, 2.3f), new Vector2(0.6f, 0.6f), colorAltar, -5);
        simbolo.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
        CrearDibujoHijo("Columna", placa.transform, new Vector2(0f, 1.1f), new Vector2(0.12f, 2f), new Color(colorAltar.r, colorAltar.g, colorAltar.b, 0.5f), -6);

        PlacaAltar componente = placa.AddComponent<PlacaAltar>();
        componente.Configurar(piso.GetComponent<SpriteRenderer>(), colorAltar);
        return componente;
    }

    // Un hijo solo visual con un cuadrado de color.
    private static GameObject CrearDibujoHijo(string nombre, Transform padre, Vector2 posicionLocal, Vector2 tamano, Color color, int ordenDeDibujo)
    {
        GameObject hijo = new GameObject(nombre);
        hijo.transform.SetParent(padre);
        hijo.transform.localPosition = new Vector3(posicionLocal.x, posicionLocal.y, 0f);
        hijo.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);

        SpriteRenderer dibujo = hijo.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCuadrado;
        dibujo.color = color;
        dibujo.sortingOrder = ordenDeDibujo;
        return hijo;
    }

    // Objeto solo visual (sin colisión), ajustado al tamaño pedido en unidades.
    public static GameObject CrearDecoracion(string nombre, Sprite sprite, Vector2 centro, Vector2 tamano, Color color, int ordenDeDibujo, bool voltearVertical, Transform padre)
    {
        GameObject decoracion = new GameObject(nombre);
        decoracion.transform.SetParent(padre);
        decoracion.transform.position = new Vector3(centro.x, centro.y, 0f);

        Vector3 tamanoOriginal = sprite.bounds.size;
        decoracion.transform.localScale = new Vector3(tamano.x / tamanoOriginal.x, tamano.y / tamanoOriginal.y, 1f);

        SpriteRenderer dibujo = decoracion.AddComponent<SpriteRenderer>();
        dibujo.sprite = sprite;
        dibujo.color = color;
        dibujo.sortingOrder = ordenDeDibujo;
        dibujo.flipY = voltearVertical;

        return decoracion;
    }

    // ================= Fondo del Salar de Uyuni =================

    // posicionSolX permite que el sol cambie de lugar entre niveles.
    public static void CrearFondoUyuni(float posicionSolX)
    {
        Sprite spriteCielo = ObtenerSpriteCielo("CieloAtardecer", false);
        Sprite spriteReflejo = ObtenerSpriteCielo("ReflejoAtardecer", true);
        Sprite spriteMonticulo = ObtenerSpriteMonticulo();
        Sprite spriteCordillera = ObtenerSpriteCordillera();

        Transform fondo = CrearGrupo("Fondo");

        CrearDecoracion("Cielo", spriteCielo, new Vector2(0f, 3f), new Vector2(36f, 13f), Color.white, -30, false, fondo);
        CrearDecoracion("Reflejo del cielo", spriteReflejo, new Vector2(0f, -7f), new Vector2(36f, 5f), Color.white, -30, false, fondo);
        CrearDecoracion("Brillo del sol", spriteCirculo, new Vector2(posicionSolX, alturaSuelo + 0.3f), new Vector2(7f, 7f), colorBrilloSol, -29, false, fondo);
        CrearDecoracion("Sol", spriteCirculo, new Vector2(posicionSolX, alturaSuelo + 0.3f), new Vector2(2.4f, 2.4f), colorSol, -28, false, fondo);
        CrearDecoracion("Cordillera", spriteCordillera, new Vector2(0f, alturaSuelo + 0.55f), new Vector2(36f, 1.4f), colorCordillera, -27, false, fondo);

        Color colorReflejoCordillera = new Color(colorCordillera.r, colorCordillera.g, colorCordillera.b, 0.35f);
        CrearDecoracion("Reflejo cordillera", spriteCordillera, new Vector2(0f, alturaSuelo - 1.7f), new Vector2(36f, 1.4f), colorReflejoCordillera, -27, true, fondo);

        CrearMonticulosDeSal(spriteMonticulo, fondo);
    }

    private static void CrearMonticulosDeSal(Sprite spriteMonticulo, Transform padre)
    {
        // Cada dato: posición X y altura del montículo (los pequeños parecen lejanos).
        List<Vector2> listaMonticulos = new List<Vector2>();
        listaMonticulos.Add(new Vector2(-13.0f, 0.7f));
        listaMonticulos.Add(new Vector2(-11.2f, 0.5f));
        listaMonticulos.Add(new Vector2(-9.5f, 0.9f));
        listaMonticulos.Add(new Vector2(-6.8f, 0.6f));
        listaMonticulos.Add(new Vector2(-5.0f, 1.1f));
        listaMonticulos.Add(new Vector2(-2.6f, 0.5f));
        listaMonticulos.Add(new Vector2(-0.8f, 0.8f));
        listaMonticulos.Add(new Vector2(1.8f, 0.6f));
        listaMonticulos.Add(new Vector2(7.5f, 0.9f));
        listaMonticulos.Add(new Vector2(9.8f, 0.5f));
        listaMonticulos.Add(new Vector2(11.5f, 1.2f));
        listaMonticulos.Add(new Vector2(13.6f, 0.7f));

        int numero = 1;
        foreach (Vector2 datos in listaMonticulos)
        {
            float posicionX = datos.x;
            float altura = datos.y;
            float ancho = altura * 2.2f;

            Vector2 centroMonticulo = new Vector2(posicionX, alturaSuelo + altura / 2f);
            CrearDecoracion("Monticulo de sal " + numero, spriteMonticulo, centroMonticulo, new Vector2(ancho, altura), colorMonticulo, -20, false, padre);

            // Su reflejo de cabeza debajo del suelo, más transparente (efecto espejo).
            Color colorReflejo = new Color(colorMonticulo.r, colorMonticulo.g, colorMonticulo.b, 0.35f);
            Vector2 centroReflejo = new Vector2(posicionX, alturaSuelo - 1f - altura / 2f);
            CrearDecoracion("Reflejo monticulo " + numero, spriteMonticulo, centroReflejo, new Vector2(ancho, altura), colorReflejo, -20, true, padre);

            numero = numero + 1;
        }
    }

    // ================= Interfaz (uGUI) =================

    private static Font fuenteInterfaz;

    private static Font ObtenerFuente()
    {
        if (fuenteInterfaz == null)
        {
            fuenteInterfaz = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        return fuenteInterfaz;
    }

    public static Transform CrearCanvas()
    {
        GameObject objetoCanvas = new GameObject("Canvas");
        Canvas canvas = objetoCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Escala la interfaz según el tamaño de la pantalla (diseñada en 1920x1080).
        CanvasScaler escalador = objetoCanvas.AddComponent<CanvasScaler>();
        escalador.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920f, 1080f);
        escalador.matchWidthOrHeight = 0.5f;

        objetoCanvas.AddComponent<GraphicRaycaster>();
        return objetoCanvas.transform;
    }

    // El EventSystem permite hacer clic en los botones (con el Input System nuevo).
    public static void CrearEventSystem()
    {
        GameObject objetoEventos = new GameObject("EventSystem");
        objetoEventos.AddComponent<EventSystem>();
        objetoEventos.AddComponent<InputSystemUIInputModule>();
    }

    public static Text CrearTexto(Transform padre, string nombre, string contenido, int tamanoLetra, Vector2 posicion, Vector2 tamanoCaja, Color color)
    {
        GameObject objetoTexto = new GameObject(nombre, typeof(RectTransform));
        objetoTexto.transform.SetParent(padre, false);

        RectTransform rectangulo = objetoTexto.GetComponent<RectTransform>();
        rectangulo.anchoredPosition = posicion;
        rectangulo.sizeDelta = tamanoCaja;

        Text texto = objetoTexto.AddComponent<Text>();
        texto.font = ObtenerFuente();
        texto.text = contenido;
        texto.fontSize = tamanoLetra;
        texto.color = color;
        texto.alignment = TextAnchor.MiddleCenter;
        texto.horizontalOverflow = HorizontalWrapMode.Overflow;
        texto.verticalOverflow = VerticalWrapMode.Overflow;

        // Sombra suave para que se lea sobre el cielo.
        Shadow sombra = objetoTexto.AddComponent<Shadow>();
        sombra.effectColor = new Color(0f, 0f, 0f, 0.55f);
        sombra.effectDistance = new Vector2(3f, -3f);

        return texto;
    }

    public static Button CrearBoton(Transform padre, string nombre, string contenido, Vector2 posicion, Vector2 tamano, Color colorFondo, int tamanoLetra)
    {
        GameObject objetoBoton = new GameObject(nombre, typeof(RectTransform));
        objetoBoton.transform.SetParent(padre, false);

        RectTransform rectangulo = objetoBoton.GetComponent<RectTransform>();
        rectangulo.anchoredPosition = posicion;
        rectangulo.sizeDelta = tamano;

        Image imagen = objetoBoton.AddComponent<Image>();
        imagen.color = Color.white;

        Button boton = objetoBoton.AddComponent<Button>();
        boton.targetGraphic = imagen;

        // Colores del botón: normal, al pasar el mouse, al presionar y desactivado.
        ColorBlock colores = boton.colors;
        colores.normalColor = colorFondo;
        colores.highlightedColor = Color.Lerp(colorFondo, Color.white, 0.25f);
        colores.selectedColor = Color.Lerp(colorFondo, Color.white, 0.15f);
        colores.pressedColor = Color.Lerp(colorFondo, Color.black, 0.2f);
        colores.disabledColor = new Color(0.35f, 0.35f, 0.40f, 0.75f);
        boton.colors = colores;

        Text texto = CrearTexto(objetoBoton.transform, "Texto", contenido, tamanoLetra, Vector2.zero, tamano, Color.white);
        texto.raycastTarget = false;

        return boton;
    }

    // ================= Build Settings =================

    // Pone en Build Settings todas las escenas del juego que existan, en orden.
    public static void ActualizarBuildSettings()
    {
        List<EditorBuildSettingsScene> listaEscenas = new List<EditorBuildSettingsScene>();

        AgregarSiExiste(listaEscenas, carpetaEscenas + "/" + DatosJuego.escenaMenuPrincipal + ".unity");
        AgregarSiExiste(listaEscenas, carpetaEscenas + "/" + DatosJuego.escenaSeleccionNivel + ".unity");

        foreach (DatosNivel nivel in DatosJuego.ObtenerTodosLosNiveles())
        {
            AgregarSiExiste(listaEscenas, carpetaNiveles + "/" + nivel.nombreEscena + ".unity");
        }

        EditorBuildSettings.scenes = listaEscenas.ToArray();
    }

    private static void AgregarSiExiste(List<EditorBuildSettingsScene> lista, string ruta)
    {
        if (File.Exists(ruta) == true)
        {
            lista.Add(new EditorBuildSettingsScene(ruta, true));
        }
    }

    // ================= Recursos generados por código =================

    public static void CrearCarpetaSiNoExiste(string rutaPadre, string nombreCarpeta)
    {
        string rutaCompleta = rutaPadre + "/" + nombreCarpeta;
        if (AssetDatabase.IsValidFolder(rutaCompleta) == false)
        {
            AssetDatabase.CreateFolder(rutaPadre, nombreCarpeta);
        }
    }

    private static PhysicsMaterial2D ObtenerMaterialSinFriccion()
    {
        PhysicsMaterial2D materialExistente = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(rutaMaterialFisico);
        if (materialExistente != null)
        {
            return materialExistente;
        }

        PhysicsMaterial2D materialNuevo = new PhysicsMaterial2D("SinFriccion");
        materialNuevo.friction = 0f;
        materialNuevo.bounciness = 0f;
        AssetDatabase.CreateAsset(materialNuevo, rutaMaterialFisico);
        AssetDatabase.SaveAssets();
        return materialNuevo;
    }

    // Cuadrado o círculo blanco de 64x64; se pinta de color en el SpriteRenderer.
    private static Sprite ObtenerSpriteBasico(string nombre, bool esCirculo)
    {
        string ruta = carpetaSpritesTemporales + "/" + nombre + ".png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int tamano = 64;
        float radio = tamano / 2f;
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);

        for (int x = 0; x < tamano; x++)
        {
            for (int y = 0; y < tamano; y++)
            {
                Color colorPixel = Color.white;
                if (esCirculo == true)
                {
                    float distancia = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(radio, radio));
                    if (distancia > radio)
                    {
                        colorPixel = Color.clear;
                    }
                }
                textura.SetPixel(x, y, colorPixel);
            }
        }

        return GuardarComoSprite(ruta, textura);
    }

    // Degradado vertical del atardecer. Si es reflejo, se invierte (horizonte arriba).
    private static Sprite ObtenerSpriteCielo(string nombre, bool esReflejo)
    {
        string ruta = carpetaSpritesUyuni + "/" + nombre + ".png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int ancho = 8;
        int alto = 256;
        Texture2D textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);

        for (int y = 0; y < alto; y++)
        {
            // t = 0 es el horizonte y t = 1 es lo más alto del cielo.
            float t = (float)y / (alto - 1);
            if (esReflejo == true)
            {
                t = 1f - t;
            }

            Color colorFila = ColorDelAtardecer(t);
            if (esReflejo == true)
            {
                // El reflejo del salar es un poco más oscuro que el cielo real.
                colorFila = new Color(colorFila.r * 0.85f, colorFila.g * 0.8f, colorFila.b * 0.85f, 1f);
            }

            for (int x = 0; x < ancho; x++)
            {
                textura.SetPixel(x, y, colorFila);
            }
        }

        return GuardarComoSprite(ruta, textura);
    }

    // Colores clave del atardecer, de abajo (horizonte) hacia arriba.
    private static Color ColorDelAtardecer(float t)
    {
        float[] posiciones = new float[] { 0f, 0.15f, 0.40f, 0.70f, 1f };
        Color[] colores = new Color[]
        {
            new Color(1.00f, 0.82f, 0.45f), // amarillo del horizonte
            new Color(0.98f, 0.55f, 0.25f), // naranja
            new Color(0.78f, 0.36f, 0.45f), // rosado
            new Color(0.40f, 0.28f, 0.55f), // morado
            new Color(0.16f, 0.18f, 0.42f)  // azul noche
        };

        for (int indice = 0; indice < posiciones.Length - 1; indice++)
        {
            if (t <= posiciones[indice + 1])
            {
                float tramo = (t - posiciones[indice]) / (posiciones[indice + 1] - posiciones[indice]);
                return Color.Lerp(colores[indice], colores[indice + 1], tramo);
            }
        }

        return colores[colores.Length - 1];
    }

    // Montículo de sal: un cono con punta redondeada, con un lado en sombra.
    private static Sprite ObtenerSpriteMonticulo()
    {
        string ruta = carpetaSpritesUyuni + "/MonticuloSal.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int ancho = 128;
        int alto = 64;
        Texture2D textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);

        for (int x = 0; x < ancho; x++)
        {
            float u = (x + 0.5f) / (ancho / 2f) - 1f; // de -1 (izquierda) a 1 (derecha)
            float alturaPerfil = 1f - Mathf.Pow(Mathf.Abs(u), 1.4f);

            for (int y = 0; y < alto; y++)
            {
                float v = (y + 0.5f) / alto;
                Color colorPixel = Color.clear;

                if (v <= alturaPerfil)
                {
                    float brillo = 1f;
                    if (u > 0f)
                    {
                        brillo = 0.82f; // lado en sombra
                    }
                    brillo = brillo * (0.9f + 0.1f * v);
                    colorPixel = new Color(brillo, brillo, brillo, 1f);
                }

                textura.SetPixel(x, y, colorPixel);
            }
        }

        return GuardarComoSprite(ruta, textura);
    }

    // Cristal de sal: un triángulo puntiagudo con una cara más oscura.
    private static Sprite ObtenerSpriteCristal()
    {
        string ruta = carpetaSpritesUyuni + "/CristalSal.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int tamano = 64;
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);

        for (int x = 0; x < tamano; x++)
        {
            float u = (x + 0.5f) / (tamano / 2f) - 1f;

            for (int y = 0; y < tamano; y++)
            {
                float v = (y + 0.5f) / tamano;
                Color colorPixel = Color.clear;

                if (v <= 1f - Mathf.Abs(u))
                {
                    float brillo = 1f;
                    if (u > 0f)
                    {
                        brillo = 0.78f;
                    }
                    colorPixel = new Color(brillo, brillo, brillo, 1f);
                }

                textura.SetPixel(x, y, colorPixel);
            }
        }

        return GuardarComoSprite(ruta, textura);
    }

    // Silueta de montañas lejanas usando ondas (senos) sumadas.
    private static Sprite ObtenerSpriteCordillera()
    {
        string ruta = carpetaSpritesUyuni + "/Cordillera.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int ancho = 512;
        int alto = 64;
        Texture2D textura = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);

        for (int x = 0; x < ancho; x++)
        {
            float alturaMontana = 0.45f
                + 0.25f * Mathf.Sin(x * 0.03f)
                + 0.15f * Mathf.Sin(x * 0.11f + 1f)
                + 0.08f * Mathf.Sin(x * 0.37f + 2f);
            alturaMontana = Mathf.Clamp(alturaMontana, 0.1f, 1f);

            for (int y = 0; y < alto; y++)
            {
                float v = (y + 0.5f) / alto;
                Color colorPixel = Color.clear;
                if (v <= alturaMontana)
                {
                    colorPixel = Color.white;
                }
                textura.SetPixel(x, y, colorPixel);
            }
        }

        return GuardarComoSprite(ruta, textura);
    }

    // Guarda la textura como PNG en el proyecto y la importa como Sprite.
    private static Sprite GuardarComoSprite(string ruta, Texture2D textura)
    {
        textura.Apply();
        File.WriteAllBytes(ruta, textura.EncodeToPNG());
        Object.DestroyImmediate(textura);

        AssetDatabase.ImportAsset(ruta);
        TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(ruta);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.spritePixelsPerUnit = 64;
        importador.filterMode = FilterMode.Bilinear;
        importador.wrapMode = TextureWrapMode.Clamp;
        importador.mipmapEnabled = false;
        importador.alphaIsTransparency = true;
        importador.textureCompression = TextureImporterCompression.Uncompressed;
        importador.SaveAndReimport();

        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }
}
