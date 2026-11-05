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
using Unity.Cinemachine;
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

    // Carpetas del arte final (imágenes PNG dibujadas a mano).
    private const string carpetaSprites = "Assets/_Proyecto/03_Sprites";
    private const string carpetaSpritesUyuni = "Assets/_Proyecto/03_Sprites/Fondos/Uyuni";
    private const string carpetaSpritesPersonajes = "Assets/_Proyecto/03_Sprites/Personajes";
    private const string carpetaSpritesNiveles = "Assets/_Proyecto/03_Sprites/Niveles";
    private const string carpetaSpritesInterfaz = "Assets/_Proyecto/03_Sprites/Interfaz";
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

    // ================= Arte final (imágenes PNG) =================
    // Cada pieza intenta usar su imagen. Si la imagen todavía no existe, la
    // variable queda en null y la pieza se dibuja con la forma de respaldo
    // (cuadrado o círculo de color), así el juego nunca se rompe.
    public static Sprite imagenCielo;
    public static Sprite imagenCordillera;
    public static Sprite imagenMonticulo;
    public static Sprite imagenSuelo;
    public static Sprite imagenPlataforma;
    public static Sprite imagenLava;
    public static Sprite imagenAgua;
    public static Sprite imagenVeneno;
    public static Sprite imagenCristales;
    public static Sprite imagenGema;
    public static Sprite imagenPuertaSalida;
    public static Sprite imagenBotonPresion;
    public static Sprite imagenPalanca;
    public static Sprite imagenAltar;
    public static Sprite imagenCompuerta;
    public static Sprite imagenInti;
    public static Sprite imagenKilla;
    public static Sprite imagenBotonInterfaz;

    // Todas las imágenes usan la misma escala: 100 píxeles = 1 unidad de Unity.
    // (Ejemplo: un personaje de 80 x 120 píxeles mide lo mismo que su collider.)
    private const float pixelesPorUnidadArte = 100f;

    // Tamaño de los personajes, en unidades. El collider no cambia para que los
    // saltos de los niveles sigan funcionando; el dibujo es más alto para que se
    // aprecien los detalles (la cabeza sobresale del collider hacia arriba).
    private const float anchoColisionPersonaje = 0.8f;
    private const float altoColisionPersonaje = 1.2f;
    private const float altoDibujoPersonaje = 1.6f;
    // Ancho máximo del dibujo: holgado, porque el que manda es el alto.
    private const float anchoMaximoDibujoPersonaje = 1.2f;

    // ================= Bloques del Salar (TilesetSal) =================
    // Son piezas sueltas con borde propio, así que no se repiten con el modo
    // Tiled: cada bloque es un objeto hijo colocado al lado o encima del anterior.
    // Miden unos 230 píxeles; con 200 píxeles por unidad quedan de 1 unidad aprox.
    private const string carpetaTilesetSal = "Assets/_Proyecto/03_Sprites/Niveles/TilesetSal";
    private const float pixelesPorUnidadTileset = 200f;

    // Cuánto se monta un bloque sobre el siguiente, para que no queden huecos
    // entre sus esquinas redondeadas.
    private const float solapeEntreBloques = 0.06f;

    // Las plataformas más angostas que esto usan la pieza pequeña.
    private const float anchoLimitePiezaPequena = 2.3f;

    private static List<Sprite> bloquesSueloSal = new List<Sprite>();
    private static List<Sprite> bloquesParedSal = new List<Sprite>();
    private static Sprite plataformaSalLarga;
    private static Sprite plataformaSalPequena;

    // El generador lo activa en las escenas del Salar y lo apaga en los demás
    // mundos, para que la sal no aparezca, por ejemplo, en las Minas de Potosí.
    public static bool usarTilesetSal = false;

    // Listas para avisar en la Console qué imágenes se encontraron y cuáles faltan.
    private static List<string> imagenesEncontradas = new List<string>();
    private static List<string> imagenesFaltantes = new List<string>();

    // Debe llamarse antes de crear cualquier escena.
    public static void PrepararRecursos()
    {
        CrearCarpetaSiNoExiste("Assets/_Proyecto", "00_Scenes");
        CrearCarpetaSiNoExiste(carpetaEscenas, "Niveles");
        CrearCarpetaSiNoExiste(carpetaSprites, "Temporales");
        CrearCarpetaSiNoExiste(carpetaSprites, "Fondos");
        CrearCarpetaSiNoExiste(carpetaSprites + "/Fondos", "Uyuni");
        CrearCarpetaSiNoExiste(carpetaSprites, "Personajes");
        CrearCarpetaSiNoExiste(carpetaSprites, "Niveles");
        CrearCarpetaSiNoExiste(carpetaSprites, "Interfaz");
        CrearCarpetaSiNoExiste("Assets/_Proyecto", "Materiales");

        spriteCuadrado = ObtenerSpriteBasico("Cuadrado", false);
        spriteCirculo = ObtenerSpriteBasico("Circulo", true);
        materialSinFriccion = ObtenerMaterialSinFriccion();

        CargarArteFinal();
    }

    // Busca cada imagen por su nombre de archivo en su carpeta.
    private static void CargarArteFinal()
    {
        imagenesEncontradas.Clear();
        imagenesFaltantes.Clear();

        imagenCielo = CargarImagenArte(carpetaSpritesUyuni, "Cielo");
        imagenCordillera = CargarImagenArte(carpetaSpritesUyuni, "Cordillera");
        imagenMonticulo = CargarImagenArte(carpetaSpritesUyuni, "Monticulo");

        imagenInti = CargarImagenPersonaje("Inti");
        imagenKilla = CargarImagenPersonaje("Killa");

        imagenSuelo = CargarImagenArte(carpetaSpritesNiveles, "Suelo");
        imagenPlataforma = CargarImagenArte(carpetaSpritesNiveles, "Plataforma");
        imagenLava = CargarImagenArte(carpetaSpritesNiveles, "Lava");
        imagenAgua = CargarImagenArte(carpetaSpritesNiveles, "Agua");
        imagenVeneno = CargarImagenArte(carpetaSpritesNiveles, "Veneno");
        imagenCristales = CargarImagenArte(carpetaSpritesNiveles, "Cristales");
        imagenGema = CargarImagenArte(carpetaSpritesNiveles, "Gema");
        imagenPuertaSalida = CargarImagenArte(carpetaSpritesNiveles, "PuertaSalida");
        imagenBotonPresion = CargarImagenArte(carpetaSpritesNiveles, "Boton");
        imagenPalanca = CargarImagenArte(carpetaSpritesNiveles, "Palanca");
        imagenAltar = CargarImagenArte(carpetaSpritesNiveles, "Altar");
        imagenCompuerta = CargarImagenArte(carpetaSpritesNiveles, "Compuerta");

        imagenBotonInterfaz = CargarImagenArte(carpetaSpritesInterfaz, "Boton");

        // El suelo usa bloques planos: las puntas de cristal se reservan para las
        // trampas, así el jugador no confunde el piso con algo que lo puede matar.
        bloquesSueloSal = new List<Sprite>();
        AgregarBloqueSiExiste(bloquesSueloSal, "BloqueSalPlano");
        AgregarBloqueSiExiste(bloquesSueloSal, "BloqueSalPequeno");

        bloquesParedSal = CargarSerieDeBloques("BloqueSalPared_", 3);
        plataformaSalLarga = CargarImagenArte(carpetaTilesetSal, "PlataformaSalLarga", pixelesPorUnidadTileset);
        plataformaSalPequena = CargarImagenArte(carpetaTilesetSal, "PlataformaSalPequena", pixelesPorUnidadTileset);

        InformarImagenes();
    }

    private static void AgregarBloqueSiExiste(List<Sprite> lista, string nombreArchivo)
    {
        Sprite bloque = CargarImagenArte(carpetaTilesetSal, nombreArchivo, pixelesPorUnidadTileset);
        if (bloque != null)
        {
            lista.Add(bloque);
        }
    }

    // Carga prefijo_1, prefijo_2... y devuelve solo los que existen. Si falta
    // alguno se alterna entre los demás; si faltan todos, la lista queda vacía
    // y la pieza usa el cuadrado de respaldo.
    private static List<Sprite> CargarSerieDeBloques(string prefijo, int cantidad)
    {
        List<Sprite> bloques = new List<Sprite>();

        for (int numero = 1; numero <= cantidad; numero++)
        {
            AgregarBloqueSiExiste(bloques, prefijo + numero);
        }

        return bloques;
    }

    // El dibujo mide más que el collider, pero el jugador sigue con su centro en
    // el centro del collider. Para que los pies no se hundan en el suelo, el pivote
    // del sprite se coloca a la altura de ese centro: lo que sobra crece hacia arriba.
    private static Sprite CargarImagenPersonaje(string nombreArchivo)
    {
        Sprite imagen = CargarImagenArte(carpetaSpritesPersonajes, nombreArchivo);
        if (imagen == null)
        {
            return null;
        }

        string ruta = carpetaSpritesPersonajes + "/" + nombreArchivo + ".png";
        float alturaPivote = (altoColisionPersonaje / 2f) / altoDibujoPersonaje;
        Vector2 pivote = new Vector2(0.5f, alturaPivote);

        TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;
        TextureImporterSettings ajustes = new TextureImporterSettings();
        importador.ReadTextureSettings(ajustes);

        bool pivoteCambio = ajustes.spriteAlignment != (int)SpriteAlignment.Custom || ajustes.spritePivot != pivote;
        if (pivoteCambio == true)
        {
            ajustes.spriteAlignment = (int)SpriteAlignment.Custom;
            ajustes.spritePivot = pivote;
            importador.SetTextureSettings(ajustes);
            importador.SaveAndReimport();
            imagen = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        return imagen;
    }

    private static Sprite CargarImagenArte(string carpeta, string nombreArchivo)
    {
        return CargarImagenArte(carpeta, nombreArchivo, pixelesPorUnidadArte);
    }

    // Devuelve el sprite de la imagen, o null si el archivo no existe.
    private static Sprite CargarImagenArte(string carpeta, string nombreArchivo, float pixelesPorUnidad)
    {
        string ruta = carpeta + "/" + nombreArchivo + ".png";
        if (File.Exists(ruta) == false)
        {
            imagenesFaltantes.Add(ruta);
            return null;
        }

        ConfigurarImportacionArte(ruta, pixelesPorUnidad);

        Sprite imagen = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        if (imagen == null)
        {
            imagenesFaltantes.Add(ruta + " (existe pero no se pudo importar como Sprite)");
            return null;
        }

        imagenesEncontradas.Add(ruta);
        return imagen;
    }

    // Deja la imagen lista para usarse como sprite de arte (sin pérdida de calidad).
    // Solo reimporta si algo cambió, para que generar escenas siga siendo rápido.
    private static void ConfigurarImportacionArte(string ruta, float pixelesPorUnidad)
    {
        TextureImporter importador = AssetImporter.GetAtPath(ruta) as TextureImporter;
        if (importador == null)
        {
            // El archivo es nuevo y Unity todavía no lo conoce: se importa primero.
            AssetDatabase.ImportAsset(ruta);
            importador = AssetImporter.GetAtPath(ruta) as TextureImporter;
            if (importador == null)
            {
                return;
            }
        }

        bool hayCambios = false;

        if (importador.textureType != TextureImporterType.Sprite)
        {
            importador.textureType = TextureImporterType.Sprite;
            hayCambios = true;
        }
        // Cada archivo es un solo dibujo. En modo Multiple, Unity cortaría una
        // imagen con partes separadas (como una fila de cristales) y solo se
        // vería la primera parte.
        if (importador.spriteImportMode != SpriteImportMode.Single)
        {
            importador.spriteImportMode = SpriteImportMode.Single;
            hayCambios = true;
        }
        if (importador.spritePixelsPerUnit != pixelesPorUnidad)
        {
            importador.spritePixelsPerUnit = pixelesPorUnidad;
            hayCambios = true;
        }
        if (importador.filterMode != FilterMode.Bilinear)
        {
            importador.filterMode = FilterMode.Bilinear;
            hayCambios = true;
        }
        if (importador.textureCompression != TextureImporterCompression.Uncompressed)
        {
            importador.textureCompression = TextureImporterCompression.Uncompressed;
            hayCambios = true;
        }
        if (importador.mipmapEnabled == true)
        {
            importador.mipmapEnabled = false;
            hayCambios = true;
        }
        if (importador.alphaIsTransparency == false)
        {
            importador.alphaIsTransparency = true;
            hayCambios = true;
        }

        // El modo "Tiled" del SpriteRenderer necesita la malla rectangular
        // completa (Full Rect); con la malla ajustada (Tight) la imagen no se repite.
        // Los bordes 9-slice que se dibujen en el Sprite Editor NO se tocan.
        TextureImporterSettings ajustes = new TextureImporterSettings();
        importador.ReadTextureSettings(ajustes);
        if (ajustes.spriteMeshType != SpriteMeshType.FullRect)
        {
            ajustes.spriteMeshType = SpriteMeshType.FullRect;
            importador.SetTextureSettings(ajustes);
            hayCambios = true;
        }

        if (hayCambios == true)
        {
            importador.SaveAndReimport();
        }
    }

    // Escribe en la Console la lista de imágenes encontradas y faltantes.
    private static void InformarImagenes()
    {
        string mensaje = "Inti y Killa - imágenes encontradas: " + imagenesEncontradas.Count + ", faltantes: " + imagenesFaltantes.Count;

        foreach (string ruta in imagenesEncontradas)
        {
            mensaje = mensaje + "\n  [OK] " + ruta;
        }
        foreach (string ruta in imagenesFaltantes)
        {
            mensaje = mensaje + "\n  [FALTA, se usa la forma de respaldo] " + ruta;
        }

        Debug.Log(mensaje);
    }

    // ================= Cómo se coloca cada imagen =================

    // Calcula la escala para que la imagen quepa dentro de la caja SIN deformarse.
    private static float CalcularEscalaParaCaber(Sprite imagen, Vector2 caja)
    {
        Vector2 tamanoImagen = imagen.bounds.size;
        float escalaAncho = caja.x / tamanoImagen.x;
        float escalaAlto = caja.y / tamanoImagen.y;
        return Mathf.Min(escalaAncho, escalaAlto);
    }

    // Para piezas que se estiran (suelo, plataformas, líquidos, compuertas):
    // la imagen se escala para que su lado corto coincida con el lado corto de
    // la pieza, y se REPITE a lo largo (modo Tiled), en vez de deformarse.
    // Si la imagen tiene bordes 9-slice, Unity respeta las esquinas al repetir.
    private static void AplicarImagenRepetida(GameObject objeto, Sprite imagen, Vector2 tamano)
    {
        Vector2 tamanoImagen = imagen.bounds.size;
        float escala = tamano.y / tamanoImagen.y;
        if (tamano.y > tamano.x)
        {
            // Pieza vertical (como la compuerta): se ajusta al ancho y se repite hacia arriba.
            escala = tamano.x / tamanoImagen.x;
        }

        objeto.transform.localScale = new Vector3(escala, escala, 1f);

        SpriteRenderer dibujo = objeto.GetComponent<SpriteRenderer>();
        dibujo.sprite = imagen;
        dibujo.drawMode = SpriteDrawMode.Tiled;
        dibujo.tileMode = SpriteTileMode.Continuous;
        dibujo.size = new Vector2(tamano.x / escala, tamano.y / escala);
    }

    // Para piezas de tamaño fijo (gemas, puertas, personajes, palanca...):
    // la imagen se ajusta dentro de la caja sin deformarse.
    private static void AplicarImagenAjustada(GameObject objeto, Sprite imagen, Vector2 caja)
    {
        float escala = CalcularEscalaParaCaber(imagen, caja);
        objeto.transform.localScale = new Vector3(escala, escala, 1f);

        SpriteRenderer dibujo = objeto.GetComponent<SpriteRenderer>();
        dibujo.sprite = imagen;
        dibujo.drawMode = SpriteDrawMode.Simple;
    }

    // El collider se define en unidades del objeto, así que se divide por la
    // escala para que en el mundo mida exactamente lo mismo que antes.
    private static void AjustarCajaDeColision(GameObject objeto, Vector2 tamanoEnMundo)
    {
        BoxCollider2D caja = objeto.GetComponent<BoxCollider2D>();
        if (caja == null)
        {
            return;
        }

        Vector3 escala = objeto.transform.localScale;
        caja.size = new Vector2(tamanoEnMundo.x / escala.x, tamanoEnMundo.y / escala.y);
        caja.offset = Vector2.zero;
    }

    // ================= Objetos básicos de una escena =================

    // Devuelve la cámara para que los niveles puedan agregarle el cerebro de Cinemachine.
    public static Camera CrearCamara(Color colorFondo)
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
        return camara;
    }

    // ================= Cámara cooperativa (Cinemachine) =================
    // Los dos jugadores comparten una sola pantalla, así que la cámara tiene
    // que mirar el punto medio entre ambos y alejarse cuando se separan.
    // Se arma solo con piezas de Cinemachine (sin script propio) para que
    // todo se pueda ajustar desde el Inspector:
    //   - CinemachineBrain en la cámara principal: la "mueve" cada frame.
    //   - CinemachineTargetGroup: grupo con Inti y Killa (su centro es el punto a seguir).
    //   - CinemachineCamera + PositionComposer: sigue al grupo con suavidad.
    //   - CinemachineGroupFraming: hace zoom para que los dos quepan siempre.
    //   - CinemachineConfiner2D: no deja que la cámara muestre fuera del nivel.

    // Zoom = mitad de la altura visible, en unidades.
    // El máximo (8.5) muestra el nivel completo, igual que la cámara fija anterior.
    private const float zoomMinimoCamara = 7f;
    private const float zoomMaximoCamara = 8.5f;

    public static void CrearCamaraCooperativa(Camera camaraPrincipal, Transform jugadorUno, Transform jugadorDos)
    {
        // 1) Cerebro: sin él, las cámaras virtuales no controlan la cámara real.
        camaraPrincipal.gameObject.AddComponent<CinemachineBrain>();

        // 2) Grupo de seguimiento con los dos protagonistas.
        //    El radio es el "espacio libre" que se deja alrededor de cada uno.
        GameObject objetoGrupo = new GameObject("Grupo de jugadores (camara)");
        CinemachineTargetGroup grupo = objetoGrupo.AddComponent<CinemachineTargetGroup>();
        float pesoJugador = 1f;
        float radioJugador = 1.5f;
        grupo.AddMember(jugadorUno, pesoJugador, radioJugador);
        grupo.AddMember(jugadorDos, pesoJugador, radioJugador);

        // 3) Cámara virtual que sigue al grupo.
        GameObject objetoCamaraVirtual = new GameObject("Camara cooperativa");
        objetoCamaraVirtual.transform.position = camaraPrincipal.transform.position;

        CinemachineCamera camaraVirtual = objetoCamaraVirtual.AddComponent<CinemachineCamera>();
        camaraVirtual.Follow = objetoGrupo.transform;

        // El lente empieza con el zoom máximo (todo el nivel a la vista).
        LensSettings lente = camaraVirtual.Lens;
        lente.OrthographicSize = zoomMaximoCamara;
        camaraVirtual.Lens = lente;

        // El compositor lleva la cámara hacia el grupo con un pequeño retraso
        // (damping) para que la imagen no tiemble con cada salto.
        CinemachinePositionComposer compositor = objetoCamaraVirtual.AddComponent<CinemachinePositionComposer>();
        compositor.CameraDistance = 10f;
        compositor.Damping = new Vector3(0.6f, 0.6f, 0f);

        // 4) Zoom automático: solo cambia el tamaño ortográfico (no mueve la
        //    cámara), siempre entre el zoom mínimo y el máximo.
        CinemachineGroupFraming encuadreGrupo = objetoCamaraVirtual.AddComponent<CinemachineGroupFraming>();
        encuadreGrupo.FramingMode = CinemachineGroupFraming.FramingModes.HorizontalAndVertical;
        encuadreGrupo.SizeAdjustment = CinemachineGroupFraming.SizeAdjustmentModes.ZoomOnly;
        encuadreGrupo.FramingSize = 0.8f;
        encuadreGrupo.Damping = 1f;
        encuadreGrupo.OrthoSizeRange = new Vector2(zoomMinimoCamara, zoomMaximoCamara);

        // 5) Límites: un rectángulo del tamaño del nivel.
        //    Es trigger para que nadie choque con él (el jugador ignora los
        //    triggers al revisar si está en el suelo, así que no le afecta).
        GameObject objetoLimites = new GameObject("Limites de la camara");
        PolygonCollider2D formaLimites = objetoLimites.AddComponent<PolygonCollider2D>();
        formaLimites.isTrigger = true;

        float bordeIzquierdo = -16f;
        float bordeDerecho = 16f;
        float bordeInferior = alturaSuelo - 1f;
        float bordeSuperior = bordeInferior + zoomMaximoCamara * 2f;

        Vector2[] esquinas = new Vector2[4];
        esquinas[0] = new Vector2(bordeIzquierdo, bordeInferior);
        esquinas[1] = new Vector2(bordeDerecho, bordeInferior);
        esquinas[2] = new Vector2(bordeDerecho, bordeSuperior);
        esquinas[3] = new Vector2(bordeIzquierdo, bordeSuperior);
        formaLimites.SetPath(0, esquinas);

        CinemachineConfiner2D confinador = objetoCamaraVirtual.AddComponent<CinemachineConfiner2D>();
        confinador.BoundingShape2D = formaLimites;
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
        CrearSuelo(colorSuelo, escenario.transform);
        CrearParedLateral("Pared izquierda", -15.5f, colorSuelo, escenario.transform);
        CrearParedLateral("Pared derecha", 15.5f, colorSuelo, escenario.transform);

        return escenario.transform;
    }

    // Suelo de lado a lado. Con el tileset son bloques planos alternados;
    // sin él, la imagen Suelo.png o el rectángulo de color.
    public static void CrearSuelo(Color colorRespaldo, Transform padre)
    {
        Vector2 centro = new Vector2(0f, alturaSuelo - 0.5f);
        Vector2 tamano = new Vector2(32f, 1f);

        if (usarTilesetSal == false || bloquesSueloSal.Count == 0)
        {
            CrearBloque("Suelo", centro, tamano, colorRespaldo, imagenSuelo, padre);
            return;
        }

        GameObject suelo = CrearSoloColision("Suelo", centro, tamano, padre);

        float bordeDerecho = centro.x + tamano.x / 2f;
        float posicionX = centro.x - tamano.x / 2f;
        int indiceBloque = 0;

        while (posicionX < bordeDerecho)
        {
            // Se alternan las imágenes para que el suelo no se vea repetido.
            Sprite bloque = bloquesSueloSal[indiceBloque % bloquesSueloSal.Count];
            Vector2 tamanoBloque = bloque.bounds.size;

            // Se alinean por arriba, justo donde termina el collider, para que
            // los pies pisen el borde del dibujo aunque los bloques midan distinto.
            Vector2 centroBloque = new Vector2(posicionX + tamanoBloque.x / 2f, alturaSuelo - tamanoBloque.y / 2f);
            CrearDecoracion("Bloque de sal " + (indiceBloque + 1), bloque, centroBloque, tamanoBloque, Color.white, 0, false, suelo.transform);

            posicionX = posicionX + tamanoBloque.x - solapeEntreBloques;
            indiceBloque = indiceBloque + 1;
        }
    }

    // Pared que impide salirse del nivel. Con el tileset se ven los bloques
    // apilados; sin él es invisible, como antes. El collider mide siempre 1 x 16.
    private static void CrearParedLateral(string nombre, float posicionX, Color colorRespaldo, Transform padre)
    {
        Vector2 centro = new Vector2(posicionX, 3f);
        Vector2 tamano = new Vector2(1f, 16f);

        if (usarTilesetSal == false || bloquesParedSal.Count == 0)
        {
            GameObject paredInvisible = CrearBloque(nombre + " invisible", centro, tamano, colorRespaldo, padre);
            paredInvisible.GetComponent<SpriteRenderer>().enabled = false;
            return;
        }

        GameObject pared = CrearSoloColision(nombre, centro, tamano, padre);

        // La pila empieza sobre el suelo y llega hasta el borde superior de la
        // cámara, para que nunca se vea el final de la pared.
        float limiteSuperior = alturaSuelo - 1f + zoomMaximoCamara * 2f;
        float posicionY = alturaSuelo;
        int indiceBloque = 0;

        while (posicionY < limiteSuperior)
        {
            Sprite bloque = bloquesParedSal[indiceBloque % bloquesParedSal.Count];
            Vector2 tamanoBloque = bloque.bounds.size;

            Vector2 centroBloque = new Vector2(posicionX, posicionY + tamanoBloque.y / 2f);
            CrearDecoracion("Bloque de pared " + (indiceBloque + 1), bloque, centroBloque, tamanoBloque, Color.white, 0, false, pared.transform);

            posicionY = posicionY + tamanoBloque.y - solapeEntreBloques;
            indiceBloque = indiceBloque + 1;
        }
    }

    // Muro vertical que separa zonas del nivel, de 1 unidad de ancho. En el Salar
    // se ven los bloques apilados; en los demás mundos, un rectángulo de color.
    public static void CrearMuro(string nombre, float posicionX, float alturaInferior, float alturaSuperior, Color colorRespaldo, Transform padre)
    {
        float altura = alturaSuperior - alturaInferior;
        Vector2 centro = new Vector2(posicionX, alturaInferior + altura / 2f);
        Vector2 tamano = new Vector2(1f, altura);

        if (usarTilesetSal == false || bloquesParedSal.Count == 0)
        {
            CrearBloque(nombre, centro, tamano, colorRespaldo, padre);
            return;
        }

        GameObject muro = CrearSoloColision(nombre, centro, tamano, padre);

        // Los bloques se ajustan al alto exacto del muro para que no sobresalgan.
        int cantidadBloques = Mathf.Max(1, Mathf.RoundToInt(altura / 1f));
        float altoBloque = altura / cantidadBloques;

        for (int indice = 0; indice < cantidadBloques; indice++)
        {
            Sprite bloque = bloquesParedSal[indice % bloquesParedSal.Count];
            Vector2 centroBloque = new Vector2(posicionX, alturaInferior + altoBloque * (indice + 0.5f));
            Vector2 tamanoBloque = new Vector2(1.1f, altoBloque + solapeEntreBloques);
            CrearDecoracion("Bloque de muro " + (indice + 1), bloque, centroBloque, tamanoBloque, Color.white, 0, false, muro.transform);
        }
    }

    // Objeto con collider pero sin dibujo propio: lo que se ve son sus hijos.
    // Tiene escala 1 para que los bloques hijos no se deformen.
    private static GameObject CrearSoloColision(string nombre, Vector2 centro, Vector2 tamano, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(centro.x, centro.y, 0f);

        BoxCollider2D colisionador = objeto.AddComponent<BoxCollider2D>();
        colisionador.size = tamano;
        return objeto;
    }

    // ¿Se pueden armar plataformas con el tileset en esta escena?
    private static bool HayPlataformasDeSal()
    {
        return usarTilesetSal == true && (plataformaSalLarga != null || plataformaSalPequena != null);
    }

    // Plataforma armada con piezas del tileset, una al lado de la otra.
    private static GameObject CrearPlataformaDeSal(string nombre, Vector2 centro, Vector2 tamano, Transform padre)
    {
        GameObject plataforma = CrearSoloColision(nombre, centro, tamano, padre);

        // Elige la pieza según el ancho; si falta una, usa la otra.
        Sprite pieza = plataformaSalLarga;
        if (pieza == null || (tamano.x < anchoLimitePiezaPequena && plataformaSalPequena != null))
        {
            pieza = plataformaSalPequena;
        }

        Vector2 tamanoPieza = pieza.bounds.size;
        float pasoPieza = tamanoPieza.x - solapeEntreBloques;

        // Cuántas piezas caben, y un ajuste leve del ancho para llenar justo la
        // plataforma: así el dibujo coincide con el collider de lado a lado.
        int cantidadPiezas = Mathf.Max(1, Mathf.RoundToInt(tamano.x / pasoPieza));
        float anchoSinAjustar = cantidadPiezas * tamanoPieza.x - (cantidadPiezas - 1) * solapeEntreBloques;
        float ajusteAncho = tamano.x / anchoSinAjustar;

        float anchoFinal = tamanoPieza.x * ajusteAncho;
        float pasoFinal = pasoPieza * ajusteAncho;
        float inicioX = centro.x - tamano.x / 2f + anchoFinal / 2f;

        // La imagen es más alta que el collider: se alinea por arriba para que
        // los pies toquen el borde del dibujo, y lo que sobra cuelga hacia abajo.
        float centroY = centro.y + tamano.y / 2f - tamanoPieza.y / 2f;

        for (int indice = 0; indice < cantidadPiezas; indice++)
        {
            Vector2 centroPieza = new Vector2(inicioX + indice * pasoFinal, centroY);
            Vector2 tamanoFinal = new Vector2(anchoFinal, tamanoPieza.y);
            CrearDecoracion("Pieza " + (indice + 1), pieza, centroPieza, tamanoFinal, Color.white, 0, false, plataforma.transform);
        }

        return plataforma;
    }

    public static Transform CrearGrupo(string nombre)
    {
        GameObject grupo = new GameObject(nombre);
        return grupo.transform;
    }

    // ================= Piezas del nivel =================

    // Bloque sólido de color (se puede pisar). Se usa para las paredes invisibles
    // y para cualquier pieza que no tenga imagen propia.
    public static GameObject CrearBloque(string nombre, Vector2 posicion, Vector2 tamano, Color color, Transform padre)
    {
        return CrearBloque(nombre, posicion, tamano, color, null, padre);
    }

    // Bloque sólido con imagen. Si imagen es null, se dibuja el cuadrado de color.
    // En los dos casos el BoxCollider2D mide exactamente "tamano" en el mundo.
    public static GameObject CrearBloque(string nombre, Vector2 posicion, Vector2 tamano, Color color, Sprite imagen, Transform padre)
    {
        GameObject bloque = new GameObject(nombre);
        bloque.transform.SetParent(padre);
        bloque.transform.position = new Vector3(posicion.x, posicion.y, 0f);

        SpriteRenderer dibujo = bloque.AddComponent<SpriteRenderer>();

        if (imagen != null)
        {
            // Con imagen: se repite a lo largo y se ve con sus colores originales.
            AplicarImagenRepetida(bloque, imagen, tamano);
            dibujo.color = Color.white;
        }
        else
        {
            // Respaldo: cuadrado blanco de 1 x 1 estirado y pintado de color.
            bloque.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);
            dibujo.sprite = spriteCuadrado;
            dibujo.color = color;
        }

        bloque.AddComponent<BoxCollider2D>();
        AjustarCajaDeColision(bloque, tamano);
        return bloque;
    }

    // Plataforma flotante normal (los bloques de sal de cada nivel).
    public static GameObject CrearPlataforma(string nombre, Vector2 posicion, Vector2 tamano, Color colorRespaldo, Transform padre)
    {
        if (HayPlataformasDeSal() == true)
        {
            return CrearPlataformaDeSal(nombre, posicion, tamano, padre);
        }

        return CrearBloque(nombre, posicion, tamano, colorRespaldo, imagenPlataforma, padre);
    }

    // Charco sobre el suelo: centroX y ancho, a la altura del suelo.
    public static void CrearCharco(string nombre, TipoZona tipo, float centroX, float ancho, Transform padre)
    {
        CrearCharco(nombre, tipo, centroX, ancho, alturaSuelo, padre);
    }

    // Charco sobre cualquier piso: alturaPiso es donde los pies tocan la plataforma.
    public static void CrearCharco(string nombre, TipoZona tipo, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        Color color = colorLava;
        Sprite imagen = imagenLava;
        if (tipo == TipoZona.Agua)
        {
            color = colorAgua;
            imagen = imagenAgua;
        }
        else if (tipo == TipoZona.Veneno)
        {
            color = colorVeneno;
            imagen = imagenVeneno;
        }

        Vector2 posicion = new Vector2(centroX, alturaPiso + 0.15f);
        GameObject charco = CrearBloque(nombre, posicion, new Vector2(ancho, 0.3f), color, imagen, padre);
        charco.GetComponent<SpriteRenderer>().sortingOrder = 1;
        charco.GetComponent<BoxCollider2D>().isTrigger = true;

        ZonaElemental zona = charco.AddComponent<ZonaElemental>();
        zona.ConfigurarZona(tipo);
    }

    public static GameObject CrearGema(string nombre, TipoElemento elemento, Vector2 posicion, Transform padre)
    {
        GameObject gema = new GameObject(nombre);
        gema.transform.SetParent(padre);
        gema.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        float tamanoGema = 0.5f;

        SpriteRenderer dibujo = gema.AddComponent<SpriteRenderer>();
        dibujo.sortingOrder = 4;

        // Hay una sola imagen de gema para los dos: se tiñe con el color del
        // elemento (naranja para Inti, azul para Killa) para distinguirlas.
        // Por eso conviene dibujar Gema.png en tonos claros o grises.
        dibujo.color = ColoresElemento.ObtenerColor(elemento);
        if (imagenGema != null)
        {
            AplicarImagenAjustada(gema, imagenGema, new Vector2(tamanoGema, tamanoGema));
        }
        else
        {
            gema.transform.localScale = new Vector3(tamanoGema, tamanoGema, 1f);
            dibujo.sprite = spriteCirculo;
        }

        // El radio en el mundo sigue siendo la mitad del tamaño de la gema.
        CircleCollider2D colisionador = gema.AddComponent<CircleCollider2D>();
        colisionador.isTrigger = true;
        colisionador.offset = Vector2.zero;
        colisionador.radius = (tamanoGema / 2f) / gema.transform.localScale.x;

        Gema componenteGema = gema.AddComponent<Gema>();
        componenteGema.ConfigurarGema(elemento);
        return gema;
    }

    // posicionSuelo = punto donde la puerta toca el piso.
    public static GameObject CrearPuertaSalida(string nombre, TipoElemento elemento, Vector2 posicionSuelo, Transform padre)
    {
        Color colorElemento = ColoresElemento.ObtenerColor(elemento);
        Color colorPuerta = new Color(colorElemento.r, colorElemento.g, colorElemento.b, 0.65f);

        Vector2 tamanoPuerta = new Vector2(1.2f, 2f);
        Vector2 centro = new Vector2(posicionSuelo.x, posicionSuelo.y + 1f);
        GameObject puerta = CrearBloque(nombre, centro, tamanoPuerta, colorPuerta, padre);
        puerta.GetComponent<SpriteRenderer>().sortingOrder = -1;
        puerta.GetComponent<BoxCollider2D>().isTrigger = true;

        // Con imagen: una sola puerta para los dos, teñida con el color del elemento.
        if (imagenPuertaSalida != null)
        {
            AplicarImagenAjustada(puerta, imagenPuertaSalida, tamanoPuerta);
            puerta.GetComponent<SpriteRenderer>().color = colorElemento;
            AjustarCajaDeColision(puerta, tamanoPuerta);
        }

        PuertaSalida componentePuerta = puerta.AddComponent<PuertaSalida>();
        componentePuerta.ConfigurarPuerta(elemento);
        return puerta;
    }

    // posicionSuelo = punto donde el jugador toca el piso al empezar.
    // imagenPersonaje puede ser null: entonces se dibuja el rectángulo de color.
    public static GameObject CrearJugador(string nombre, TipoElemento elemento, Key izquierda, Key derecha, Key saltar, Key habilidad, Vector2 posicionSuelo, Color colorMarca, Sprite imagenPersonaje)
    {
        float anchoJugador = anchoColisionPersonaje;
        float altoJugador = altoColisionPersonaje;
        Vector2 tamanoJugador = new Vector2(anchoJugador, altoJugador);

        GameObject jugador = new GameObject(nombre);
        jugador.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y + altoJugador / 2f + 0.05f, 0f);

        SpriteRenderer dibujo = jugador.AddComponent<SpriteRenderer>();
        dibujo.color = ColoresElemento.ObtenerColor(elemento);
        dibujo.sortingOrder = 10;

        if (imagenPersonaje != null)
        {
            // Con imagen propia se ve con sus colores reales (sin teñir).
            AplicarImagenAjustada(jugador, imagenPersonaje, new Vector2(anchoMaximoDibujoPersonaje, altoDibujoPersonaje));
            dibujo.color = Color.white;
        }
        else
        {
            jugador.transform.localScale = new Vector3(anchoJugador, altoJugador, 1f);
            dibujo.sprite = spriteCuadrado;
        }

        Rigidbody2D cuerpo = jugador.AddComponent<Rigidbody2D>();
        cuerpo.gravityScale = 3f;
        cuerpo.freezeRotation = true;
        cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        cuerpo.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Sin fricción, para que no se queden "pegados" a las paredes.
        BoxCollider2D colisionador = jugador.AddComponent<BoxCollider2D>();
        colisionador.sharedMaterial = materialSinFriccion;
        AjustarCajaDeColision(jugador, tamanoJugador);

        ControladorJugador controlador = jugador.AddComponent<ControladorJugador>();
        controlador.ConfigurarJugador(nombre, elemento, izquierda, derecha, saltar, habilidad);
        controlador.ConfigurarColoresDeLaImagen(imagenPersonaje != null);

        // Marca de identidad sobre la cabeza: solo hace falta en la forma de
        // respaldo, que cambia de color con el eclipse y dejaría de saberse quién
        // es quién. Con imagen propia, cada personaje ya se reconoce por su dibujo.
        if (imagenPersonaje == null)
        {
            GameObject marca = new GameObject("Marca de identidad");
            marca.transform.SetParent(jugador.transform);
            marca.transform.localPosition = new Vector3(0f, 0.72f, 0f);
            marca.transform.localScale = new Vector3(0.35f / anchoJugador, 0.35f / altoJugador, 1f);
            SpriteRenderer dibujoMarca = marca.AddComponent<SpriteRenderer>();
            dibujoMarca.sprite = spriteCirculo;
            dibujoMarca.color = colorMarca;
            dibujoMarca.sortingOrder = 11;
        }

        // Se devuelve para que la cámara cooperativa pueda seguirlo.
        return jugador;
    }

    // Los dos protagonistas en su posición inicial.
    public static GameObject CrearInti(Vector2 posicionSuelo)
    {
        Color colorSolDorado = new Color(1.00f, 0.95f, 0.45f);
        return CrearJugador("Inti", TipoElemento.Fuego, Key.A, Key.D, Key.W, Key.S, posicionSuelo, colorSolDorado, imagenInti);
    }

    public static GameObject CrearKilla(Vector2 posicionSuelo)
    {
        Color colorLunaPlateada = new Color(0.85f, 0.90f, 1.00f);
        return CrearJugador("Killa", TipoElemento.Agua, Key.LeftArrow, Key.RightArrow, Key.UpArrow, Key.DownArrow, posicionSuelo, colorLunaPlateada, imagenKilla);
    }

    // ================= Piezas de las Minas de Potosí =================

    public static readonly Color colorCristalMina = new Color(0.55f, 0.95f, 1.00f);
    private static readonly Color colorParedMina = new Color(0.16f, 0.11f, 0.10f);
    private static readonly Color colorVigaMadera = new Color(0.30f, 0.20f, 0.13f);

    // Le pone a un jugador su luz 2D. Se agrega a los DOS jugadores: el script
    // LuzPersonaje solo la enciende en quien tenga el fuego (cambia con el eclipse).
    // Solo se usa en niveles oscuros; en los niveles con luz de día sobraría.
    public static void AgregarLuzPersonaje(GameObject jugador)
    {
        GameObject objetoLuz = new GameObject("Luz del personaje");
        objetoLuz.transform.SetParent(jugador.transform);
        objetoLuz.transform.localPosition = Vector3.zero;
        // La escala del jugador no debe deformar la luz.
        Vector3 escalaJugador = jugador.transform.localScale;
        objetoLuz.transform.localScale = new Vector3(1f / escalaJugador.x, 1f / escalaJugador.y, 1f);

        Light2D luz = objetoLuz.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Point;
        luz.enabled = false;

        LuzPersonaje componente = jugador.AddComponent<LuzPersonaje>();
        componente.Configurar(luz);
    }

    // Plataforma de cristal que solo se puede pisar dentro de la luz de Inti.
    public static void CrearPlataformaOculta(string nombre, Vector2 posicion, Vector2 tamano, Transform padre)
    {
        GameObject plataforma = CrearBloque(nombre, posicion, tamano, colorCristalMina, imagenPlataforma, padre);

        // Con imagen se tiñe de celeste para que se note que es de cristal.
        plataforma.GetComponent<SpriteRenderer>().color = colorCristalMina;
        plataforma.GetComponent<SpriteRenderer>().sortingOrder = 1;
        plataforma.AddComponent<PlataformaOculta>();
    }

    // Farol colgado: una luz fija y cálida para que la mina no sea negra del todo.
    public static void CrearFarol(string nombre, Vector2 posicion, float radio, Transform padre)
    {
        GameObject farol = new GameObject(nombre);
        farol.transform.SetParent(padre);
        farol.transform.position = new Vector3(posicion.x, posicion.y, 0f);

        Light2D luz = farol.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Point;
        luz.color = new Color(1f, 0.65f, 0.30f);
        luz.intensity = 0.9f;
        luz.pointLightOuterRadius = radio;
        luz.pointLightInnerRadius = radio * 0.2f;

        // La lámpara en sí: un círculo pequeño colgando de un cordel.
        CrearDibujoHijo("Cordel", farol.transform, new Vector2(0f, 0.5f), new Vector2(0.05f, 0.8f), new Color(0.2f, 0.15f, 0.1f), -4);
        GameObject lampara = CrearDibujoHijo("Lampara", farol.transform, Vector2.zero, new Vector2(0.3f, 0.3f), new Color(1f, 0.8f, 0.4f), -3);
        lampara.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
    }

    // Fondo de la mina: pared de roca oscura con vigas de madera que sostienen el techo.
    public static void CrearFondoMina()
    {
        Transform fondo = CrearGrupo("Fondo");

        CrearDecoracion("Pared de la mina", spriteCuadrado, new Vector2(0f, 3.5f), new Vector2(36f, 18f), colorParedMina, -30, false, fondo);
        CrearDecoracion("Techo", spriteCuadrado, new Vector2(0f, 12.2f), new Vector2(36f, 1f), colorVigaMadera, -26, false, fondo);

        // Un marco de vigas (dos postes + travesaño) cada 6 unidades.
        int numero = 1;
        for (float posicionX = -12f; posicionX <= 12f; posicionX = posicionX + 6f)
        {
            float alturaPoste = 12f - alturaSuelo;
            float centroPosteY = alturaSuelo + alturaPoste / 2f;
            CrearDecoracion("Poste " + numero + "a", spriteCuadrado, new Vector2(posicionX - 1.5f, centroPosteY), new Vector2(0.35f, alturaPoste), colorVigaMadera, -27, false, fondo);
            CrearDecoracion("Poste " + numero + "b", spriteCuadrado, new Vector2(posicionX + 1.5f, centroPosteY), new Vector2(0.35f, alturaPoste), colorVigaMadera, -27, false, fondo);
            CrearDecoracion("Travesano " + numero, spriteCuadrado, new Vector2(posicionX, 11.5f), new Vector2(3.6f, 0.35f), colorVigaMadera, -27, false, fondo);
            numero = numero + 1;
        }
    }

    // ================= Trampas =================
    // Se arman con las mismas piezas que los objetos normales: por eso se ven iguales.

    public static SueloFalso CrearSueloFalso(string nombre, Vector2 posicion, Vector2 tamano, Color colorRespaldo, Transform padre)
    {
        GameObject plataforma = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);
        SueloFalso trampa = plataforma.AddComponent<SueloFalso>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static BloqueInvisible CrearBloqueInvisible(string nombre, Vector2 posicion, Vector2 tamano, Color colorRespaldo, Transform padre)
    {
        GameObject plataforma = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);
        BloqueInvisible trampa = plataforma.AddComponent<BloqueInvisible>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // alturaPiso: donde se apoyan. Los dibujos son los mismos cristales que los normales.
    public static PuasOcultas CrearPuasOcultas(string nombre, float centroX, float ancho, float alturaPiso, bool seActivaSola, Transform padre)
    {
        GameObject zona = new GameObject(nombre);
        zona.transform.SetParent(padre);
        zona.transform.position = new Vector3(centroX, alturaPiso, 0f);

        Sprite spritePua = imagenCristales;
        Color colorPua = Color.white;
        if (spritePua == null)
        {
            spritePua = ObtenerSpriteCristal();
            colorPua = colorCristal;
        }

        float anchoPua = 0.5f;
        int cantidadPuas = Mathf.Max(1, Mathf.RoundToInt(ancho / anchoPua));
        for (int indice = 0; indice < cantidadPuas; indice++)
        {
            float posicionX = centroX - ancho / 2f + anchoPua / 2f + indice * anchoPua;
            CrearDecoracion("Pua " + (indice + 1), spritePua, new Vector2(posicionX, alturaPiso + 0.3f), new Vector2(anchoPua, 0.6f), colorPua, 3, false, zona.transform);
        }

        PuasOcultas trampa = zona.AddComponent<PuasOcultas>();
        trampa.Configurar(ancho);
        trampa.ConfigurarDisparo(seActivaSola);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // alturaPiso: hasta dónde cae. La zona que lo dispara es la columna de debajo.
    public static TechoQueCae CrearTechoQueCae(string nombre, Vector2 posicion, Vector2 tamano, float alturaPiso, bool seActivaSola, Color colorRespaldo, Transform padre)
    {
        GameObject bloque = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);
        TechoQueCae trampa = bloque.AddComponent<TechoQueCae>();
        trampa.Configurar(alturaPiso);
        trampa.ConfigurarDisparo(seActivaSola);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static GemaTrampa CrearGemaTrampa(string nombre, TipoElemento elemento, Vector2 posicion, List<TrampaActivable> trampas, Transform padre)
    {
        GameObject gema = CrearGema(nombre, elemento, posicion, padre);

        // Se quita la gema de verdad: esta no debe contar para las estrellas.
        Object.DestroyImmediate(gema.GetComponent<Gema>());
        Object.DestroyImmediate(gema.GetComponent<CircleCollider2D>());

        GemaTrampa trampa = gema.AddComponent<GemaTrampa>();
        trampa.Configurar(trampas);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static PuertaFalsa CrearPuertaFalsa(string nombre, TipoElemento elemento, Vector2 posicionSuelo, Vector2 desplazamientoAlHuir, Transform padre)
    {
        GameObject puerta = CrearPuertaSalida(nombre, elemento, posicionSuelo, padre);

        // Se quita la puerta de verdad: el nivel no debe esperar a nadie aquí.
        Object.DestroyImmediate(puerta.GetComponent<PuertaSalida>());
        Object.DestroyImmediate(puerta.GetComponent<BoxCollider2D>());

        PuertaFalsa trampa = puerta.AddComponent<PuertaFalsa>();
        trampa.Configurar(desplazamientoAlHuir);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static PlataformaQueHuye CrearPlataformaQueHuye(string nombre, Vector2 posicion, Vector2 tamano, Vector2 desplazamientoAlHuir, Color colorRespaldo, Transform padre)
    {
        GameObject plataforma = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);
        Rigidbody2D cuerpo = plataforma.AddComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;

        PlataformaQueHuye trampa = plataforma.AddComponent<PlataformaQueHuye>();
        trampa.Configurar(desplazamientoAlHuir);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static BotonEnganoso CrearBotonEnganoso(string nombre, Vector2 posicionSuelo, List<TrampaActivable> trampas, Transform padre)
    {
        GameObject boton = CrearBotonPresion(nombre, posicionSuelo, new List<ObjetoActivable>(), padre);
        BotonEnganoso trampa = boton.AddComponent<BotonEnganoso>();
        trampa.Configurar(trampas);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // posicionSuelo: el piso del que sale.
    public static EnemigoSorpresa CrearEnemigoSorpresa(string nombre, Vector2 posicionSuelo, Transform padre)
    {
        Vector2 tamano = new Vector2(0.9f, 0.9f);
        GameObject enemigo = new GameObject(nombre);
        enemigo.transform.SetParent(padre);
        enemigo.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y + tamano.y / 2f, 0f);
        enemigo.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);

        SpriteRenderer dibujo = enemigo.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.25f, 0.10f, 0.30f);
        dibujo.sortingOrder = 9;

        EnemigoSorpresa trampa = enemigo.AddComponent<EnemigoSorpresa>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // ----- Trampas de traición: castigan al compañero de quien las usa -----

    public static BotonTraicion CrearBotonTraicion(string nombre, Vector2 posicionSuelo, TipoCastigo castigo, Transform padre)
    {
        GameObject boton = CrearBotonPresion(nombre, posicionSuelo, new List<ObjetoActivable>(), padre);
        BotonTraicion trampa = boton.AddComponent<BotonTraicion>();
        trampa.Configurar(castigo);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static PalancaSospechosa CrearPalancaSospechosa(string nombre, Vector2 posicionSuelo, List<ObjetoActivable> objetivos, TipoCastigo castigo, Transform padre)
    {
        GameObject palanca = CrearPalanca(nombre, posicionSuelo, objetivos, padre);
        PalancaSospechosa trampa = palanca.AddComponent<PalancaSospechosa>();
        trampa.Configurar(castigo);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Une las dos placas de un altar: quien se baja primero castiga al otro.
    public static PlacaVinculada CrearPlacaVinculada(AltarEclipse altar, TipoCastigo castigo)
    {
        PlacaVinculada trampa = altar.gameObject.AddComponent<PlacaVinculada>();
        trampa.Configurar(altar.PlacaUno, altar.PlacaDos, castigo);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Figura quieta de un personaje (sin física ni controles), para el menú principal.
    public static void CrearFiguraPersonaje(string nombre, Sprite imagenPersonaje, Color colorRespaldo, Vector2 centro, Transform padre)
    {
        Vector2 tamanoJugador = new Vector2(0.8f, 1.2f);
        if (imagenPersonaje != null)
        {
            Vector2 cajaDibujo = new Vector2(anchoMaximoDibujoPersonaje, altoDibujoPersonaje);
            Vector2 tamanoSinDeformar = imagenPersonaje.bounds.size * CalcularEscalaParaCaber(imagenPersonaje, cajaDibujo);
            CrearDecoracion(nombre, imagenPersonaje, centro, tamanoSinDeformar, Color.white, 10, false, padre);
        }
        else
        {
            CrearDecoracion(nombre, spriteCuadrado, centro, tamanoJugador, colorRespaldo, 10, false, padre);
        }
    }

    // Púas de sal centradas en centroX, sobre el suelo. Matan a los dos.
    public static void CrearCristalesDeSal(string nombre, float centroX, float ancho, Transform padre)
    {
        CrearCristalesDeSal(nombre, centroX, ancho, alturaSuelo, padre);
    }

    // Púas de sal sobre cualquier piso: alturaPiso es donde se apoyan.
    public static void CrearCristalesDeSal(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        GameObject zona = new GameObject(nombre);
        zona.transform.SetParent(padre);
        zona.transform.position = new Vector3(centroX, alturaPiso + 0.25f, 0f);

        BoxCollider2D colisionador = zona.AddComponent<BoxCollider2D>();
        colisionador.isTrigger = true;
        colisionador.size = new Vector2(ancho, 0.45f);

        ZonaElemental componenteZona = zona.AddComponent<ZonaElemental>();
        componenteZona.ConfigurarZona(TipoZona.CristalesSal);

        // Con imagen: una sola tira de cristales que se repite a lo ancho.
        // Va en un hijo para no cambiar la escala de la zona (ni su collider).
        if (imagenCristales != null)
        {
            Vector2 tamanoTira = new Vector2(ancho, 0.6f);
            GameObject tira = new GameObject("Cristales");
            tira.transform.SetParent(zona.transform);
            tira.transform.position = new Vector3(centroX, alturaPiso + 0.3f, 0f);

            SpriteRenderer dibujoTira = tira.AddComponent<SpriteRenderer>();
            dibujoTira.sortingOrder = 3;
            AplicarImagenRepetida(tira, imagenCristales, tamanoTira);
            return;
        }

        // Respaldo: una fila de triángulos generados por código.
        Sprite spriteCristal = ObtenerSpriteCristal();
        float anchoCristal = 0.5f;
        int cantidadCristales = Mathf.RoundToInt(ancho / anchoCristal);

        for (int indice = 0; indice < cantidadCristales; indice++)
        {
            float posicionX = centroX - ancho / 2f + anchoCristal / 2f + indice * anchoCristal;
            Vector2 centroCristal = new Vector2(posicionX, alturaPiso + 0.3f);
            CrearDecoracion("Cristal " + (indice + 1), spriteCristal, centroCristal, new Vector2(anchoCristal, 0.6f), colorCristal, 3, false, zona.transform);
        }
    }

    public static void CrearPlataformaQuebradiza(string nombre, Vector2 posicion, Vector2 tamano, Transform padre)
    {
        GameObject plataforma = CrearPlataforma(nombre, posicion, tamano, colorSalQuebradiza, padre);

        // Usa la misma imagen que las plataformas normales, pero un poco rojiza
        // para que los jugadores sepan que se va a romper. La forma de respaldo
        // ya tiene su propio color, así que no se toca.
        if (HayPlataformasDeSal() == true || imagenPlataforma != null)
        {
            SpriteRenderer[] dibujos = plataforma.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer dibujo in dibujos)
            {
                dibujo.color = new Color(1f, 0.78f, 0.72f);
            }
        }

        plataforma.AddComponent<PlataformaQuebradiza>();
    }

    // Compuerta de piedra que se abre con botones o palancas.
    public static PuertaMecanica CrearPuertaMecanica(string nombre, Vector2 posicion, Vector2 tamano, Vector2 desplazamientoAlAbrir, int senalesNecesarias, Transform padre)
    {
        GameObject bloque = CrearBloque(nombre, posicion, tamano, colorPiedra, imagenCompuerta, padre);
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
        GameObject bloque = CrearPlataforma(nombre, posicion, tamano, colorMadera, padre);

        Rigidbody2D cuerpo = bloque.AddComponent<Rigidbody2D>();
        cuerpo.bodyType = RigidbodyType2D.Kinematic;

        PlataformaMovil plataforma = bloque.AddComponent<PlataformaMovil>();
        plataforma.Configurar(desplazamiento, velocidad, moverseDesdeElInicio);
        return plataforma;
    }

    // Botón de presión apoyado en el suelo en la posición indicada.
    public static GameObject CrearBotonPresion(string nombre, Vector2 posicionSuelo, List<ObjetoActivable> objetivos, Transform padre)
    {
        GameObject boton = new GameObject(nombre);
        boton.transform.SetParent(padre);
        boton.transform.position = new Vector3(posicionSuelo.x, posicionSuelo.y, 0f);

        // Zona que detecta a los jugadores (un poco más alta que el botón).
        BoxCollider2D zona = boton.AddComponent<BoxCollider2D>();
        zona.isTrigger = true;
        zona.size = new Vector2(1.2f, 0.6f);
        zona.offset = new Vector2(0f, 0.3f);

        GameObject tapa;
        if (imagenBotonPresion != null)
        {
            // Con imagen: el botón completo es una sola imagen apoyada en el suelo.
            // El script BotonPresion la tiñe (amarillo suelto / verde presionado),
            // así que conviene dibujar Boton.png en tonos claros.
            Vector2 cajaBoton = new Vector2(1.6f, 0.4f);
            tapa = CrearDibujoHijo("Boton", boton.transform, new Vector2(0f, cajaBoton.y / 2f), Vector2.one, Color.yellow, 3);
            AplicarImagenAjustada(tapa, imagenBotonPresion, cajaBoton);
        }
        else
        {
            CrearDibujoHijo("Base", boton.transform, new Vector2(0f, 0.05f), new Vector2(1.6f, 0.1f), new Color(0.30f, 0.28f, 0.32f), 2);
            tapa = CrearDibujoHijo("Tapa", boton.transform, new Vector2(0f, 0.18f), new Vector2(1.1f, 0.18f), Color.yellow, 3);
        }

        BotonPresion componente = boton.AddComponent<BotonPresion>();
        componente.Configurar(objetivos, tapa.GetComponent<SpriteRenderer>());
        return boton;
    }

    // Palanca apoyada en el suelo en la posición indicada.
    public static GameObject CrearPalanca(string nombre, Vector2 posicionSuelo, List<ObjetoActivable> objetivos, Transform padre)
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

        // Con imagen: Palanca.png es el mango (dibujado vertical). Gira con el
        // pivote y el script Palanca lo tiñe (rojo apagada / verde encendida).
        if (imagenPalanca != null)
        {
            Vector2 cajaMango = new Vector2(0.4f, 0.9f);
            mango.transform.localPosition = new Vector3(0f, cajaMango.y / 2f, 0f);
            AplicarImagenAjustada(mango, imagenPalanca, cajaMango);
        }

        Palanca componente = palanca.AddComponent<Palanca>();
        componente.Configurar(objetivos, pivote.transform, mango.GetComponent<SpriteRenderer>());
        return palanca;
    }

    // Altar de eclipse: dos placas (pueden estar lejos) unidas por un mismo altar.
    public static AltarEclipse CrearAltarEclipse(string nombre, Vector2 posicionPlacaUno, Vector2 posicionPlacaDos, Color colorAltar, Transform padre)
    {
        GameObject altar = new GameObject(nombre);
        altar.transform.SetParent(padre);

        PlacaAltar placaUno = CrearPlacaAltar("Placa del Sol", posicionPlacaUno, colorAltar, altar.transform);
        PlacaAltar placaDos = CrearPlacaAltar("Placa de la Luna", posicionPlacaDos, colorAltar, altar.transform);

        AltarEclipse componente = altar.AddComponent<AltarEclipse>();
        componente.Configurar(placaUno, placaDos);
        return componente;
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

        GameObject piso;
        if (imagenAltar != null)
        {
            // Con imagen: el altar completo (placa, columna y símbolo) es una
            // sola imagen. PlacaAltar la tiñe con el color de su altar, así los
            // dos altares de un nivel se distinguen, y se ilumina al pisarla.
            Vector2 cajaAltar = new Vector2(1.5f, 2.6f);
            piso = CrearDibujoHijo("Altar", placa.transform, new Vector2(0f, cajaAltar.y / 2f), Vector2.one, colorAltar, -5);
            AplicarImagenAjustada(piso, imagenAltar, cajaAltar);

            PlacaAltar componenteConImagen = placa.AddComponent<PlacaAltar>();
            componenteConImagen.Configurar(piso.GetComponent<SpriteRenderer>(), colorAltar);
            return componenteConImagen;
        }

        piso = CrearDibujoHijo("Piso del altar", placa.transform, new Vector2(0f, 0.07f), new Vector2(1.5f, 0.14f), colorAltar, 2);

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
        Transform fondo = CrearGrupo("Fondo");

        // ----- Cielo y su reflejo en el salar (efecto espejo) -----
        if (imagenCielo != null)
        {
            // El cielo se estira para cubrir todo el fondo: al ser un degradado
            // o nubes suaves, no se nota la deformación. El reflejo es la misma
            // imagen de cabeza y un poco más oscura.
            CrearDecoracion("Cielo", imagenCielo, new Vector2(0f, 4.75f), new Vector2(36f, 16.5f), Color.white, -30, false, fondo);
            CrearDecoracion("Reflejo del cielo", imagenCielo, new Vector2(0f, -7f), new Vector2(36f, 5f), new Color(0.85f, 0.8f, 0.85f), -30, true, fondo);
        }
        else
        {
            Sprite spriteCieloGenerado = ObtenerSpriteCielo("CieloGenerado", false);
            Sprite spriteReflejoGenerado = ObtenerSpriteCielo("ReflejoCieloGenerado", true);
            CrearDecoracion("Cielo", spriteCieloGenerado, new Vector2(0f, 4.75f), new Vector2(36f, 16.5f), Color.white, -30, false, fondo);
            CrearDecoracion("Reflejo del cielo", spriteReflejoGenerado, new Vector2(0f, -7f), new Vector2(36f, 5f), Color.white, -30, false, fondo);
        }

        // El sol no está en la lista de imágenes: sigue siendo un círculo con brillo.
        CrearDecoracion("Brillo del sol", spriteCirculo, new Vector2(posicionSolX, alturaSuelo + 0.3f), new Vector2(7f, 7f), colorBrilloSol, -29, false, fondo);
        CrearDecoracion("Sol", spriteCirculo, new Vector2(posicionSolX, alturaSuelo + 0.3f), new Vector2(2.4f, 2.4f), colorSol, -28, false, fondo);

        // ----- Cordillera y su reflejo -----
        Vector2 centroCordillera = new Vector2(0f, alturaSuelo + 0.55f);
        Vector2 centroReflejoCordillera = new Vector2(0f, alturaSuelo - 1.7f);
        Vector2 tamanoCordillera = new Vector2(36f, 1.4f);

        if (imagenCordillera != null)
        {
            // La imagen se repite a lo ancho (modo Tiled) para no deformar las montañas.
            GameObject cordillera = CrearDecoracion("Cordillera", imagenCordillera, centroCordillera, tamanoCordillera, Color.white, -27, false, fondo);
            AplicarImagenRepetida(cordillera, imagenCordillera, tamanoCordillera);

            GameObject reflejo = CrearDecoracion("Reflejo cordillera", imagenCordillera, centroReflejoCordillera, tamanoCordillera, new Color(1f, 1f, 1f, 0.35f), -27, true, fondo);
            AplicarImagenRepetida(reflejo, imagenCordillera, tamanoCordillera);
        }
        else
        {
            Sprite spriteCordilleraGenerada = ObtenerSpriteCordillera();
            Color colorReflejoCordillera = new Color(colorCordillera.r, colorCordillera.g, colorCordillera.b, 0.35f);
            CrearDecoracion("Cordillera", spriteCordilleraGenerada, centroCordillera, tamanoCordillera, colorCordillera, -27, false, fondo);
            CrearDecoracion("Reflejo cordillera", spriteCordilleraGenerada, centroReflejoCordillera, tamanoCordillera, colorReflejoCordillera, -27, true, fondo);
        }

        // ----- Montículos de sal -----
        if (imagenMonticulo != null)
        {
            CrearMonticulosDeSal(imagenMonticulo, Color.white, true, fondo);
        }
        else
        {
            CrearMonticulosDeSal(ObtenerSpriteMonticulo(), colorMonticulo, false, fondo);
        }
    }

    // sinDeformar = true para la imagen dibujada (respeta su proporción);
    // la forma generada sí se estira porque es solo un cono de color.
    private static void CrearMonticulosDeSal(Sprite spriteMonticulo, Color colorDibujo, bool sinDeformar, Transform padre)
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

            if (sinDeformar == true)
            {
                // Se mantiene la altura y el ancho sale de la proporción de la imagen.
                Vector2 tamanoImagen = spriteMonticulo.bounds.size;
                ancho = altura * (tamanoImagen.x / tamanoImagen.y);
            }

            Vector2 centroMonticulo = new Vector2(posicionX, alturaSuelo + altura / 2f);
            CrearDecoracion("Monticulo de sal " + numero, spriteMonticulo, centroMonticulo, new Vector2(ancho, altura), colorDibujo, -20, false, padre);

            // Su reflejo de cabeza debajo del suelo, más transparente (efecto espejo).
            Color colorReflejo = new Color(colorDibujo.r, colorDibujo.g, colorDibujo.b, 0.35f);
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

        // Si existe Interfaz/Boton.png, el botón usa esa imagen. Los colores de
        // abajo la tiñen, así cada botón conserva su color (naranja, morado...).
        // Si la imagen tiene bordes 9-slice, se estira sin deformar las esquinas.
        if (imagenBotonInterfaz != null)
        {
            imagen.sprite = imagenBotonInterfaz;
            if (imagenBotonInterfaz.border != Vector4.zero)
            {
                imagen.type = Image.Type.Sliced;
            }
            else
            {
                imagen.type = Image.Type.Simple;
            }
        }

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
        AgregarSiExiste(listaEscenas, carpetaEscenas + "/" + DatosJuego.escenaSeleccionMundo + ".unity");
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

    // Las formas generadas por código se guardan en Temporales con otro nombre,
    // para que nunca se confundan con el arte final de Fondos/Uyuni.

    // Degradado vertical del atardecer. Si es reflejo, se invierte (horizonte arriba).
    private static Sprite ObtenerSpriteCielo(string nombre, bool esReflejo)
    {
        string ruta = carpetaSpritesTemporales + "/" + nombre + ".png";
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
        string ruta = carpetaSpritesTemporales + "/MonticuloGenerado.png";
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
        string ruta = carpetaSpritesTemporales + "/CristalGenerado.png";
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
        string ruta = carpetaSpritesTemporales + "/CordilleraGenerada.png";
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
