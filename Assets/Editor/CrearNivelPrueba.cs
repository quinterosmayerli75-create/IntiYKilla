// =====================================================================
// CrearNivelPrueba.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR que arma automáticamente un nivel de
//              prueba jugable (prototipo con cuadrados de colores) para
//              probar las mecánicas antes de tener el arte final.
// Ubicación obligatoria: Assets/Editor/CrearNivelPrueba.cs
// Uso: Menú superior de Unity -> Inti y Killa -> Crear nivel de prueba
// =====================================================================

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class CrearNivelPrueba
{
    private const string carpetaSprites = "Assets/_Proyecto/03_Sprites/Temporales";
    private const string rutaEscena = "Assets/_Proyecto/00_Scenes/Niveles/Nivel01_Prueba.unity";
    private const string rutaMaterialFisico = "Assets/_Proyecto/Materiales/SinFriccion.physicsMaterial2D";

    // Colores del Salar de Uyuni (prototipo).
    private static readonly Color colorCielo = new Color(0.55f, 0.75f, 0.95f);
    private static readonly Color colorSuelo = new Color(0.93f, 0.93f, 0.90f);
    private static readonly Color colorPlataforma = new Color(0.80f, 0.82f, 0.85f);
    private static readonly Color colorLava = new Color(1.00f, 0.35f, 0.10f);
    private static readonly Color colorAgua = new Color(0.00f, 0.25f, 0.80f);
    private static readonly Color colorVeneno = new Color(0.35f, 0.85f, 0.20f);
    private static readonly Color colorInti = new Color(1.00f, 0.60f, 0.10f);
    private static readonly Color colorKilla = new Color(0.20f, 0.35f, 1.00f);

    [MenuItem("Inti y Killa/Crear nivel de prueba")]
    public static void CrearNivel()
    {
        // Si hay cambios sin guardar en la escena abierta, pregunta primero.
        bool usuarioAcepto = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        if (usuarioAcepto == false)
        {
            return;
        }

        CrearCarpetaSiNoExiste("Assets/_Proyecto/03_Sprites", "Temporales");
        Sprite spriteCuadrado = CrearSpriteTemporal("Cuadrado", false);
        Sprite spriteCirculo = CrearSpriteTemporal("Circulo", true);
        PhysicsMaterial2D materialSinFriccion = CrearMaterialSinFriccion();

        Scene escenaNueva = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CrearCamara();
        CrearLuzGlobal();

        GameObject gestor = new GameObject("GestorNivel");
        gestor.AddComponent<GestorNivel>();

        // ---------------- Escenario ----------------
        GameObject escenario = new GameObject("Escenario");
        CrearBloque("Suelo", new Vector2(0f, -4f), new Vector2(32f, 1f), colorSuelo, spriteCuadrado, escenario.transform);
        CrearBloque("ParedIzquierda", new Vector2(-15.5f, 1f), new Vector2(1f, 11f), colorPlataforma, spriteCuadrado, escenario.transform);
        CrearBloque("ParedDerecha", new Vector2(15.5f, 1f), new Vector2(1f, 11f), colorPlataforma, spriteCuadrado, escenario.transform);
        CrearBloque("Plataforma1", new Vector2(-2f, -1.5f), new Vector2(4f, 0.5f), colorPlataforma, spriteCuadrado, escenario.transform);
        CrearBloque("Plataforma2", new Vector2(4.5f, 0.8f), new Vector2(5f, 0.5f), colorPlataforma, spriteCuadrado, escenario.transform);
        CrearBloque("Plataforma3", new Vector2(10.5f, -1.2f), new Vector2(3f, 0.5f), colorPlataforma, spriteCuadrado, escenario.transform);

        // ---------------- Charcos ----------------
        GameObject charcos = new GameObject("Charcos");
        CrearCharco("Lava", TipoZona.Lava, new Vector2(-6f, -3.35f), 2.5f, colorLava, spriteCuadrado, charcos.transform);
        CrearCharco("Agua", TipoZona.Agua, new Vector2(1f, -3.35f), 2.5f, colorAgua, spriteCuadrado, charcos.transform);
        CrearCharco("Veneno", TipoZona.Veneno, new Vector2(7.5f, -3.35f), 2.5f, colorVeneno, spriteCuadrado, charcos.transform);

        // ---------------- Gemas ----------------
        GameObject gemas = new GameObject("Gemas");
        CrearGema("GemaInti_1", TipoElemento.Fuego, new Vector2(-6f, -2.4f), colorInti, spriteCirculo, gemas.transform);
        CrearGema("GemaKilla_1", TipoElemento.Agua, new Vector2(1f, -2.4f), colorKilla, spriteCirculo, gemas.transform);
        CrearGema("GemaInti_2", TipoElemento.Fuego, new Vector2(3f, 1.8f), colorInti, spriteCirculo, gemas.transform);
        CrearGema("GemaKilla_2", TipoElemento.Agua, new Vector2(6f, 1.8f), colorKilla, spriteCirculo, gemas.transform);

        // ---------------- Puertas ----------------
        GameObject puertas = new GameObject("Puertas");
        CrearPuerta("PuertaInti", TipoElemento.Fuego, new Vector2(12.2f, -2.5f), colorInti, spriteCuadrado, puertas.transform);
        CrearPuerta("PuertaKilla", TipoElemento.Agua, new Vector2(14.2f, -2.5f), colorKilla, spriteCuadrado, puertas.transform);

        // ---------------- Jugadores ----------------
        CrearJugador("Inti", TipoElemento.Fuego, Key.A, Key.D, Key.W, new Vector2(-13f, -2.6f), colorInti, spriteCuadrado, materialSinFriccion);
        CrearJugador("Killa", TipoElemento.Agua, Key.LeftArrow, Key.RightArrow, Key.UpArrow, new Vector2(-11.5f, -2.6f), colorKilla, spriteCuadrado, materialSinFriccion);

        EditorSceneManager.SaveScene(escenaNueva, rutaEscena);
        AgregarEscenaABuildSettings(rutaEscena);

        Debug.Log("Inti y Killa: nivel de prueba creado en " + rutaEscena + ". Presiona Play para jugar.");
    }

    // ================= Creación de objetos =================

    private static void CrearCamara()
    {
        GameObject objetoCamara = new GameObject("Main Camera");
        objetoCamara.tag = "MainCamera";
        objetoCamara.transform.position = new Vector3(0f, 0.5f, -10f);

        Camera camara = objetoCamara.AddComponent<Camera>();
        camara.orthographic = true;
        camara.orthographicSize = 8.5f;
        camara.clearFlags = CameraClearFlags.SolidColor;
        camara.backgroundColor = colorCielo;

        objetoCamara.AddComponent<AudioListener>();
    }

    private static void CrearLuzGlobal()
    {
        // En URP 2D los sprites necesitan luz para verse (si no, salen negros).
        GameObject objetoLuz = new GameObject("Luz Global 2D");
        Light2D luz = objetoLuz.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Global;
        luz.intensity = 1f;
    }

    private static GameObject CrearBloque(string nombre, Vector2 posicion, Vector2 tamano, Color color, Sprite sprite, Transform padre)
    {
        GameObject bloque = new GameObject(nombre);
        bloque.transform.SetParent(padre);
        bloque.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        bloque.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);

        SpriteRenderer dibujo = bloque.AddComponent<SpriteRenderer>();
        dibujo.sprite = sprite;
        dibujo.color = color;

        bloque.AddComponent<BoxCollider2D>();
        return bloque;
    }

    private static void CrearCharco(string nombre, TipoZona tipo, Vector2 posicion, float ancho, Color color, Sprite sprite, Transform padre)
    {
        GameObject charco = CrearBloque(nombre, posicion, new Vector2(ancho, 0.3f), color, sprite, padre);
        charco.GetComponent<SpriteRenderer>().sortingOrder = 1;
        charco.GetComponent<BoxCollider2D>().isTrigger = true;

        ZonaElemental zona = charco.AddComponent<ZonaElemental>();
        zona.ConfigurarZona(tipo);
    }

    private static void CrearGema(string nombre, TipoElemento elemento, Vector2 posicion, Color color, Sprite sprite, Transform padre)
    {
        GameObject gema = new GameObject(nombre);
        gema.transform.SetParent(padre);
        gema.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        gema.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

        SpriteRenderer dibujo = gema.AddComponent<SpriteRenderer>();
        dibujo.sprite = sprite;
        dibujo.color = color;
        dibujo.sortingOrder = 2;

        CircleCollider2D colisionador = gema.AddComponent<CircleCollider2D>();
        colisionador.isTrigger = true;

        Gema componenteGema = gema.AddComponent<Gema>();
        componenteGema.ConfigurarGema(elemento);
    }

    private static void CrearPuerta(string nombre, TipoElemento elemento, Vector2 posicion, Color color, Sprite sprite, Transform padre)
    {
        GameObject puerta = CrearBloque(nombre, posicion, new Vector2(1.2f, 2f), new Color(color.r, color.g, color.b, 0.65f), sprite, padre);
        puerta.GetComponent<SpriteRenderer>().sortingOrder = -1;
        puerta.GetComponent<BoxCollider2D>().isTrigger = true;

        PuertaSalida componentePuerta = puerta.AddComponent<PuertaSalida>();
        componentePuerta.ConfigurarPuerta(elemento);
    }

    private static void CrearJugador(string nombre, TipoElemento elemento, Key izquierda, Key derecha, Key saltar, Vector2 posicion, Color color, Sprite sprite, PhysicsMaterial2D material)
    {
        GameObject jugador = new GameObject(nombre);
        jugador.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        jugador.transform.localScale = new Vector3(0.8f, 1.2f, 1f);

        SpriteRenderer dibujo = jugador.AddComponent<SpriteRenderer>();
        dibujo.sprite = sprite;
        dibujo.color = color;
        dibujo.sortingOrder = 5;

        Rigidbody2D cuerpo = jugador.AddComponent<Rigidbody2D>();
        cuerpo.gravityScale = 3f;
        cuerpo.freezeRotation = true;
        cuerpo.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        cuerpo.interpolation = RigidbodyInterpolation2D.Interpolate;

        // Sin fricción, para que no se queden "pegados" a las paredes.
        BoxCollider2D colisionador = jugador.AddComponent<BoxCollider2D>();
        colisionador.sharedMaterial = material;

        ControladorJugador controlador = jugador.AddComponent<ControladorJugador>();
        controlador.ConfigurarJugador(nombre, elemento, izquierda, derecha, saltar);
        EditorUtility.SetDirty(controlador);
    }

    // ================= Recursos temporales =================

    // Dibuja por código un cuadrado o un círculo blanco y lo guarda como PNG.
    // Se pinta de color en el SpriteRenderer, así un solo sprite sirve para todo.
    private static Sprite CrearSpriteTemporal(string nombre, bool esCirculo)
    {
        string rutaArchivo = carpetaSprites + "/" + nombre + ".png";

        if (File.Exists(rutaArchivo) == false)
        {
            int tamanoPixeles = 64;
            Texture2D textura = new Texture2D(tamanoPixeles, tamanoPixeles, TextureFormat.RGBA32, false);
            float radio = tamanoPixeles / 2f;

            for (int x = 0; x < tamanoPixeles; x++)
            {
                for (int y = 0; y < tamanoPixeles; y++)
                {
                    Color colorPixel = Color.white;

                    if (esCirculo == true)
                    {
                        float distanciaAlCentro = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(radio, radio));
                        if (distanciaAlCentro > radio)
                        {
                            colorPixel = Color.clear;
                        }
                    }

                    textura.SetPixel(x, y, colorPixel);
                }
            }

            textura.Apply();
            File.WriteAllBytes(rutaArchivo, textura.EncodeToPNG());
            Object.DestroyImmediate(textura);
            AssetDatabase.ImportAsset(rutaArchivo);

            TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(rutaArchivo);
            importador.textureType = TextureImporterType.Sprite;
            importador.spriteImportMode = SpriteImportMode.Single;
            importador.spritePixelsPerUnit = tamanoPixeles;
            importador.SaveAndReimport();
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(rutaArchivo);
    }

    private static PhysicsMaterial2D CrearMaterialSinFriccion()
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

    private static void CrearCarpetaSiNoExiste(string rutaPadre, string nombreCarpeta)
    {
        string rutaCompleta = rutaPadre + "/" + nombreCarpeta;
        if (AssetDatabase.IsValidFolder(rutaCompleta) == false)
        {
            AssetDatabase.CreateFolder(rutaPadre, nombreCarpeta);
        }
    }

    // Agrega la escena a File -> Build Profiles para poder reiniciarla con SceneManager.
    private static void AgregarEscenaABuildSettings(string ruta)
    {
        List<EditorBuildSettingsScene> listaEscenas = new List<EditorBuildSettingsScene>();

        foreach (EditorBuildSettingsScene escena in EditorBuildSettings.scenes)
        {
            if (escena.path == ruta)
            {
                return; // Ya estaba agregada.
            }
            listaEscenas.Add(escena);
        }

        listaEscenas.Add(new EditorBuildSettingsScene(ruta, true));
        EditorBuildSettings.scenes = listaEscenas.ToArray();
    }
}
