// =====================================================================
// GeneradorArteMenus.cs
// Proyecto: Inti y Killa
// Descripción: Script de EDITOR que dibuja por código el arte PROVISIONAL de
//              los menús: un paisaje por mundo, estrella, candado, flecha y
//              rectángulo redondeado. Permite ver las pantallas terminadas antes
//              de tener las imágenes finales. Si un archivo ya existe no se
//              vuelve a dibujar. El arte final se pone en 03_Sprites/Interfaz
//              (ver HerramientasInterfaz) y reemplaza a estas imágenes.
// =====================================================================

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class GeneradorArteMenus
{
    private const string carpeta = "Assets/_Proyecto/03_Sprites/Temporales";
    private const float pixelesPorUnidad = 100f;

    // Una fila de montañas del paisaje. Las alturas van de 0 (abajo) a 1 (arriba).
    private class CapaPaisaje
    {
        public Color colorCima;
        public Color colorBase;
        public float altura;
        public float irregularidad;
        public float frecuencia;
        public float semilla;
        // Montaña puntiaguda opcional, como el Cerro Rico.
        public float picoCentro = 0f;
        public float picoAncho = 1f;
        public float picoAlto = 0f;
        // Parte de la cima pintada de nieve.
        public float nieve = 0f;
        // Neblina que tapa la parte baja de la capa.
        public float neblina = 0f;
        // Cuánto refleja el cielo y las montañas, como el salar mojado o el lago.
        public float reflejo = 0f;

        public CapaPaisaje(Color nuevaCima, Color nuevaBase, float nuevaAltura, float nuevaIrregularidad, float nuevaFrecuencia, float nuevaSemilla)
        {
            colorCima = nuevaCima;
            colorBase = nuevaBase;
            altura = nuevaAltura;
            irregularidad = nuevaIrregularidad;
            frecuencia = nuevaFrecuencia;
            semilla = nuevaSemilla;
        }
    }

    // Todo lo que define el paisaje de un mundo.
    private class Paisaje
    {
        public Color cieloArriba;
        public Color cieloAbajo;
        public Color colorSol = Color.clear;
        public Vector2 posicionSol;
        public float radioSol;
        public bool hayEclipse = false;
        public bool hayEstrellas = false;
        public bool hayLuces = false;
        public List<CapaPaisaje> capas = new List<CapaPaisaje>();
    }

    // ================= Paisajes =================

    public static Sprite ObtenerTarjeta(int numeroMundo)
    {
        return ObtenerPaisaje("TarjetaMundo" + numeroMundo, numeroMundo, 404, 640);
    }

    public static Sprite ObtenerFondo(int numeroMundo)
    {
        return ObtenerPaisaje("FondoMundo" + numeroMundo, numeroMundo, 960, 540);
    }

    private static Sprite ObtenerPaisaje(string nombre, int numeroMundo, int ancho, int alto)
    {
        string ruta = carpeta + "/" + nombre + ".png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        Texture2D textura = PintarPaisaje(CrearPaisaje(numeroMundo), ancho, alto);
        return Guardar(ruta, textura, Vector4.zero);
    }

    private static Paisaje CrearPaisaje(int numeroMundo)
    {
        Paisaje paisaje = new Paisaje();

        if (numeroMundo == 1)
        {
            // Salar de Uyuni: atardecer rosado, cordillera lejana y sal blanca.
            paisaje.cieloArriba = new Color(0.22f, 0.27f, 0.60f);
            paisaje.cieloAbajo = new Color(0.98f, 0.68f, 0.55f);
            paisaje.colorSol = new Color(1f, 0.93f, 0.72f);
            paisaje.posicionSol = new Vector2(0.05f, 0.52f);
            paisaje.radioSol = 0.09f;
            CapaPaisaje cordillera = new CapaPaisaje(new Color(0.45f, 0.38f, 0.62f), new Color(0.62f, 0.45f, 0.62f), 0.43f, 0.05f, 4f, 3f);
            cordillera.nieve = 0.018f;
            paisaje.capas.Add(cordillera);
            CapaPaisaje sal = new CapaPaisaje(new Color(0.98f, 0.92f, 0.90f), new Color(0.74f, 0.80f, 0.94f), 0.38f, 0f, 1f, 0f);
            sal.reflejo = 0.62f;
            paisaje.capas.Add(sal);
        }
        else if (numeroMundo == 2)
        {
            // Minas de Potosí: noche rojiza, el Cerro Rico y las luces del pueblo.
            paisaje.cieloArriba = new Color(0.08f, 0.05f, 0.16f);
            paisaje.cieloAbajo = new Color(0.55f, 0.20f, 0.18f);
            paisaje.hayEstrellas = true;
            paisaje.hayLuces = true;
            CapaPaisaje cerro = new CapaPaisaje(new Color(0.72f, 0.33f, 0.22f), new Color(0.32f, 0.12f, 0.12f), 0.30f, 0.018f, 9f, 5f);
            cerro.picoCentro = 0f;
            cerro.picoAncho = 0.62f;
            cerro.picoAlto = 0.42f;
            paisaje.capas.Add(cerro);
            paisaje.capas.Add(new CapaPaisaje(new Color(0.20f, 0.10f, 0.13f), new Color(0.09f, 0.05f, 0.09f), 0.24f, 0.03f, 5f, 8f));
        }
        else if (numeroMundo == 3)
        {
            // Yungas: montañas verdes una detrás de otra, separadas por neblina.
            paisaje.cieloArriba = new Color(0.42f, 0.70f, 0.82f);
            paisaje.cieloAbajo = new Color(0.92f, 0.95f, 0.88f);
            AgregarCapaConNeblina(paisaje, new Color(0.42f, 0.66f, 0.55f), 0.72f, 0.09f, 3f, 11f);
            AgregarCapaConNeblina(paisaje, new Color(0.26f, 0.56f, 0.38f), 0.54f, 0.10f, 4f, 14f);
            AgregarCapaConNeblina(paisaje, new Color(0.14f, 0.44f, 0.26f), 0.36f, 0.10f, 5f, 17f);
            AgregarCapaConNeblina(paisaje, new Color(0.07f, 0.28f, 0.17f), 0.17f, 0.07f, 8f, 21f);
        }
        else if (numeroMundo == 4)
        {
            // Trópico: selva tupida y un río ancho.
            paisaje.cieloArriba = new Color(0.16f, 0.58f, 0.74f);
            paisaje.cieloAbajo = new Color(0.98f, 0.87f, 0.56f);
            paisaje.colorSol = new Color(1f, 0.96f, 0.72f);
            paisaje.posicionSol = new Vector2(-0.12f, 0.74f);
            paisaje.radioSol = 0.07f;
            paisaje.capas.Add(new CapaPaisaje(new Color(0.30f, 0.60f, 0.36f), new Color(0.18f, 0.42f, 0.28f), 0.52f, 0.035f, 16f, 25f));
            paisaje.capas.Add(new CapaPaisaje(new Color(0.12f, 0.44f, 0.24f), new Color(0.06f, 0.28f, 0.18f), 0.42f, 0.04f, 20f, 29f));
            paisaje.capas.Add(new CapaPaisaje(new Color(0.34f, 0.72f, 0.80f), new Color(0.08f, 0.40f, 0.58f), 0.27f, 0.006f, 6f, 33f));
            paisaje.capas.Add(new CapaPaisaje(new Color(0.05f, 0.26f, 0.14f), new Color(0.02f, 0.14f, 0.08f), 0.09f, 0.05f, 14f, 37f));
        }
        else if (numeroMundo == 5)
        {
            // Aiquile: lomas de tierra cálida bajo un sol grande.
            paisaje.cieloArriba = new Color(0.72f, 0.32f, 0.30f);
            paisaje.cieloAbajo = new Color(1f, 0.78f, 0.40f);
            paisaje.colorSol = new Color(1f, 0.90f, 0.50f);
            paisaje.posicionSol = new Vector2(0.14f, 0.62f);
            paisaje.radioSol = 0.13f;
            paisaje.capas.Add(new CapaPaisaje(new Color(0.84f, 0.56f, 0.30f), new Color(0.70f, 0.42f, 0.24f), 0.46f, 0.07f, 3f, 41f));
            paisaje.capas.Add(new CapaPaisaje(new Color(0.66f, 0.38f, 0.21f), new Color(0.48f, 0.26f, 0.16f), 0.31f, 0.07f, 4f, 44f));
            paisaje.capas.Add(new CapaPaisaje(new Color(0.40f, 0.22f, 0.14f), new Color(0.22f, 0.11f, 0.09f), 0.15f, 0.06f, 6f, 47f));
        }
        else
        {
            // Isla del Sol: el lago Titicaca, la isla, los Andes y un eclipse.
            paisaje.cieloArriba = new Color(0.08f, 0.10f, 0.38f);
            paisaje.cieloAbajo = new Color(0.96f, 0.56f, 0.46f);
            paisaje.colorSol = new Color(1f, 0.86f, 0.50f);
            paisaje.posicionSol = new Vector2(0f, 0.70f);
            paisaje.radioSol = 0.10f;
            paisaje.hayEclipse = true;
            paisaje.hayEstrellas = true;
            CapaPaisaje andes = new CapaPaisaje(new Color(0.38f, 0.36f, 0.58f), new Color(0.56f, 0.44f, 0.60f), 0.44f, 0.06f, 5f, 51f);
            andes.nieve = 0.022f;
            paisaje.capas.Add(andes);
            CapaPaisaje lago = new CapaPaisaje(new Color(0.42f, 0.62f, 0.82f), new Color(0.05f, 0.20f, 0.46f), 0.37f, 0f, 1f, 0f);
            lago.reflejo = 0.5f;
            paisaje.capas.Add(lago);
            CapaPaisaje isla = new CapaPaisaje(new Color(0.34f, 0.40f, 0.22f), new Color(0.13f, 0.20f, 0.17f), 0.17f, 0.02f, 7f, 55f);
            isla.picoCentro = 0.03f;
            isla.picoAncho = 0.75f;
            isla.picoAlto = 0.13f;
            paisaje.capas.Add(isla);
        }

        return paisaje;
    }

    private static void AgregarCapaConNeblina(Paisaje paisaje, Color color, float altura, float irregularidad, float frecuencia, float semilla)
    {
        CapaPaisaje capa = new CapaPaisaje(color, color, altura, irregularidad, frecuencia, semilla);
        capa.neblina = 0.75f;
        paisaje.capas.Add(capa);
    }

    private static Texture2D PintarPaisaje(Paisaje paisaje, int ancho, int alto)
    {
        Texture2D textura = new Texture2D(ancho, alto, TextureFormat.RGB24, false);
        Color[] pixeles = new Color[ancho * alto];

        for (int y = 0; y < alto; y++)
        {
            float alturaPixel = (y + 0.5f) / alto;

            for (int x = 0; x < ancho; x++)
            {
                // Se mide en "alturas de imagen" desde el centro: así la tarjeta y
                // el fondo muestran el mismo paisaje sin estirarlo, solo recortado.
                float posicionX = (x + 0.5f - ancho / 2f) / alto;
                Color color = PintarHastaCapa(paisaje, paisaje.capas.Count, posicionX, alturaPixel, 1f / alto);

                if (paisaje.hayLuces == true)
                {
                    color = PintarLuces(color, posicionX, alturaPixel);
                }

                pixeles[y * ancho + x] = color;
            }
        }

        textura.SetPixels(pixeles);
        return textura;
    }

    // Pinta el cielo y las primeras capas. Una capa con reflejo vuelve a llamar a
    // este método, con la altura volteada, para pintar lo que tiene detrás.
    private static Color PintarHastaCapa(Paisaje paisaje, int cantidadCapas, float posicionX, float alturaPixel, float tamanoPixel)
    {
        Color color = PintarCielo(paisaje, posicionX, alturaPixel, tamanoPixel);

        for (int indice = 0; indice < cantidadCapas; indice++)
        {
            CapaPaisaje capa = paisaje.capas[indice];
            color = PintarCapa(capa, color, posicionX, alturaPixel, tamanoPixel);

            float borde = CalcularBorde(capa, posicionX);
            if (capa.reflejo > 0f && alturaPixel < borde)
            {
                float alturaReflejada = borde * 2f - alturaPixel;
                Color colorReflejado = PintarHastaCapa(paisaje, indice, posicionX, alturaReflejada, tamanoPixel);
                color = Color.Lerp(color, colorReflejado, capa.reflejo);
            }
        }

        return color;
    }

    private static float CalcularBorde(CapaPaisaje capa, float posicionX)
    {
        float ruidoGrande = Mathf.PerlinNoise(posicionX * capa.frecuencia + capa.semilla, capa.semilla) - 0.5f;
        float ruidoFino = Mathf.PerlinNoise(posicionX * capa.frecuencia * 3f + capa.semilla, capa.semilla + 7f) - 0.5f;
        float borde = capa.altura + (ruidoGrande * 2f + ruidoFino * 0.6f) * capa.irregularidad;
        return borde + capa.picoAlto * Mathf.Clamp01(1f - Mathf.Abs(posicionX - capa.picoCentro) / capa.picoAncho);
    }

    private static Color PintarCielo(Paisaje paisaje, float posicionX, float alturaPixel, float tamanoPixel)
    {
        Color color = Color.Lerp(paisaje.cieloAbajo, paisaje.cieloArriba, Mathf.SmoothStep(0.25f, 1f, alturaPixel));

        if (paisaje.hayEstrellas == true && alturaPixel > 0.45f)
        {
            float azar = Mathf.PerlinNoise(posicionX * 170f + 40f, alturaPixel * 170f);
            if (azar > 0.86f)
            {
                color = Color.Lerp(color, Color.white, (azar - 0.86f) * 6f);
            }
        }

        if (paisaje.radioSol > 0f)
        {
            float distancia = Vector2.Distance(new Vector2(posicionX, alturaPixel), paisaje.posicionSol);

            // Resplandor alrededor y disco con el borde suavizado.
            float resplandor = Mathf.Clamp01(1f - distancia / (paisaje.radioSol * 3.2f));
            color = Color.Lerp(color, paisaje.colorSol, resplandor * resplandor * 0.75f);
            color = Color.Lerp(color, paisaje.colorSol, Mathf.Clamp01((paisaje.radioSol - distancia) / tamanoPixel));

            if (paisaje.hayEclipse == true)
            {
                Color colorLuna = new Color(0.07f, 0.07f, 0.17f);
                color = Color.Lerp(color, colorLuna, Mathf.Clamp01((paisaje.radioSol * 0.92f - distancia) / tamanoPixel));
            }
        }

        return color;
    }

    private static Color PintarCapa(CapaPaisaje capa, Color colorDebajo, float posicionX, float alturaPixel, float tamanoPixel)
    {
        float borde = CalcularBorde(capa, posicionX);

        // Borde suavizado de un píxel: sin esto, el perfil se ve dentado.
        float cobertura = Mathf.Clamp01((borde - alturaPixel) / tamanoPixel);
        if (cobertura <= 0f)
        {
            return colorDebajo;
        }

        float profundidad = Mathf.Clamp01((borde - alturaPixel) / Mathf.Max(borde, 0.01f));
        Color color = Color.Lerp(capa.colorCima, capa.colorBase, profundidad);

        // La nieve solo cubre las partes altas y baja de forma dispareja:
        // con un grosor parejo parecería una cinta sobre la montaña.
        float picoRelativo = Mathf.Clamp01((borde - capa.altura) / Mathf.Max(capa.irregularidad, 0.001f) + 0.2f);
        float dientes = Mathf.PerlinNoise(posicionX * 45f, capa.semilla + 3f);
        float grosorNieve = capa.nieve * 3f * picoRelativo * dientes;
        if (borde - alturaPixel < grosorNieve)
        {
            color = new Color(0.97f, 0.96f, 1f);
        }

        if (capa.neblina > 0f)
        {
            color = Color.Lerp(color, new Color(0.92f, 0.96f, 0.92f), profundidad * profundidad * capa.neblina);
        }

        return Color.Lerp(colorDebajo, color, cobertura);
    }

    // Ventanitas del pueblo al pie del Cerro Rico.
    private static Color PintarLuces(Color color, float posicionX, float alturaPixel)
    {
        Color colorLuz = new Color(1f, 0.78f, 0.35f);

        for (int indice = 0; indice < 40; indice++)
        {
            float luzX = Mathf.Sin(indice * 12.9898f) * 0.85f;
            float luzY = 0.04f + Mathf.Abs(Mathf.Sin(indice * 78.233f)) * 0.14f;
            float distancia = Vector2.Distance(new Vector2(posicionX, alturaPixel), new Vector2(luzX, luzY));
            if (distancia < 0.018f)
            {
                color = Color.Lerp(color, colorLuz, Mathf.Clamp01(1f - distancia / 0.018f) * 0.8f);
            }
        }

        return color;
    }

    // ================= Iconos (blancos, se pintan desde la imagen) =================

    // Rectángulo redondeado que se estira sin deformar las esquinas (9-slice).
    public static Sprite ObtenerRedondeado()
    {
        string ruta = carpeta + "/Redondeado.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int tamano = 128;
        float radio = 40f;
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);
        for (int x = 0; x < tamano; x++)
        {
            for (int y = 0; y < tamano; y++)
            {
                Vector2 punto = new Vector2(x + 0.5f, y + 0.5f);
                float distancia = DistanciaRectanguloRedondeado(punto, new Vector2(64f, 64f), new Vector2(63f, 63f), radio);
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(0.5f - distancia)));
            }
        }

        return Guardar(ruta, textura, new Vector4(44f, 44f, 44f, 44f));
    }

    public static Sprite ObtenerEstrella()
    {
        string ruta = carpeta + "/Estrella.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int tamano = 128;
        Vector2[] vertices = CrearVerticesEstrella(new Vector2(64f, 62f), 62f, 28f);
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);
        int muestrasPorLado = 4;

        for (int x = 0; x < tamano; x++)
        {
            for (int y = 0; y < tamano; y++)
            {
                // Varias muestras por píxel para que las puntas no queden dentadas.
                int muestrasDentro = 0;
                for (int muestraX = 0; muestraX < muestrasPorLado; muestraX++)
                {
                    for (int muestraY = 0; muestraY < muestrasPorLado; muestraY++)
                    {
                        Vector2 punto = new Vector2(x + (muestraX + 0.5f) / muestrasPorLado, y + (muestraY + 0.5f) / muestrasPorLado);
                        if (EstaDentroDelPoligono(punto, vertices) == true)
                        {
                            muestrasDentro = muestrasDentro + 1;
                        }
                    }
                }

                float transparencia = (float)muestrasDentro / (muestrasPorLado * muestrasPorLado);
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, transparencia));
            }
        }

        return Guardar(ruta, textura, Vector4.zero);
    }

    public static Sprite ObtenerCandado()
    {
        string ruta = carpeta + "/Candado.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int tamano = 128;
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);
        Vector2 centroAro = new Vector2(64f, 80f);

        for (int x = 0; x < tamano; x++)
        {
            for (int y = 0; y < tamano; y++)
            {
                Vector2 punto = new Vector2(x + 0.5f, y + 0.5f);

                float distanciaCuerpo = DistanciaRectanguloRedondeado(punto, new Vector2(64f, 44f), new Vector2(46f, 38f), 9f);

                // El aro es un anillo, y sus patas bajan hasta el cuerpo.
                float distanciaAro = Mathf.Abs(Vector2.Distance(punto, centroAro) - 27f) - 8f;
                if (punto.y < centroAro.y)
                {
                    distanciaAro = Mathf.Abs(Mathf.Abs(punto.x - 64f) - 27f) - 8f;
                }

                float distanciaCerradura = Vector2.Distance(punto, new Vector2(64f, 48f)) - 11f;
                float forma = Mathf.Max(Mathf.Min(distanciaCuerpo, distanciaAro), -distanciaCerradura);
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(0.5f - forma)));
            }
        }

        return Guardar(ruta, textura, Vector4.zero);
    }

    // Apunta a la izquierda; para la derecha se gira desde la imagen.
    public static Sprite ObtenerFlecha()
    {
        string ruta = carpeta + "/Flecha.png";
        if (File.Exists(ruta) == true)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }

        int tamano = 128;
        Vector2 punta = new Vector2(28f, 64f);
        Texture2D textura = new Texture2D(tamano, tamano, TextureFormat.RGBA32, false);

        for (int x = 0; x < tamano; x++)
        {
            for (int y = 0; y < tamano; y++)
            {
                Vector2 punto = new Vector2(x + 0.5f, y + 0.5f);
                float distanciaAstil = DistanciaASegmento(punto, punta, new Vector2(104f, 64f));
                float distanciaAlaArriba = DistanciaASegmento(punto, punta, new Vector2(64f, 100f));
                float distanciaAlaAbajo = DistanciaASegmento(punto, punta, new Vector2(64f, 28f));
                float forma = Mathf.Min(distanciaAstil, Mathf.Min(distanciaAlaArriba, distanciaAlaAbajo)) - 10f;
                textura.SetPixel(x, y, new Color(1f, 1f, 1f, Mathf.Clamp01(0.5f - forma)));
            }
        }

        return Guardar(ruta, textura, Vector4.zero);
    }

    // ================= Geometría =================

    // Negativo dentro de la forma y positivo fuera, en píxeles.
    private static float DistanciaRectanguloRedondeado(Vector2 punto, Vector2 centro, Vector2 mitad, float radio)
    {
        float distanciaX = Mathf.Abs(punto.x - centro.x) - (mitad.x - radio);
        float distanciaY = Mathf.Abs(punto.y - centro.y) - (mitad.y - radio);
        Vector2 exterior = new Vector2(Mathf.Max(distanciaX, 0f), Mathf.Max(distanciaY, 0f));
        float interior = Mathf.Min(Mathf.Max(distanciaX, distanciaY), 0f);
        return exterior.magnitude + interior - radio;
    }

    private static float DistanciaASegmento(Vector2 punto, Vector2 inicio, Vector2 fin)
    {
        Vector2 segmento = fin - inicio;
        float avance = Mathf.Clamp01(Vector2.Dot(punto - inicio, segmento) / segmento.sqrMagnitude);
        return Vector2.Distance(punto, inicio + segmento * avance);
    }

    private static Vector2[] CrearVerticesEstrella(Vector2 centro, float radioExterior, float radioInterior)
    {
        int cantidadVertices = 10;
        Vector2[] vertices = new Vector2[cantidadVertices];

        for (int indice = 0; indice < cantidadVertices; indice++)
        {
            float radio = radioInterior;
            if (indice % 2 == 0)
            {
                radio = radioExterior;
            }

            // La primera punta mira hacia arriba.
            float angulo = Mathf.PI / 2f + indice * Mathf.PI * 2f / cantidadVertices;
            vertices[indice] = centro + new Vector2(Mathf.Cos(angulo), Mathf.Sin(angulo)) * radio;
        }

        return vertices;
    }

    private static bool EstaDentroDelPoligono(Vector2 punto, Vector2[] vertices)
    {
        bool estaDentro = false;
        int anterior = vertices.Length - 1;

        for (int actual = 0; actual < vertices.Length; actual++)
        {
            bool cruzaEnY = (vertices[actual].y > punto.y) != (vertices[anterior].y > punto.y);
            if (cruzaEnY == true)
            {
                float cruceX = vertices[actual].x + (punto.y - vertices[actual].y) / (vertices[anterior].y - vertices[actual].y) * (vertices[anterior].x - vertices[actual].x);
                if (punto.x < cruceX)
                {
                    estaDentro = !estaDentro;
                }
            }
            anterior = actual;
        }

        return estaDentro;
    }

    // ================= Guardar =================

    private static Sprite Guardar(string ruta, Texture2D textura, Vector4 borde)
    {
        textura.Apply();
        File.WriteAllBytes(ruta, textura.EncodeToPNG());
        Object.DestroyImmediate(textura);

        AssetDatabase.ImportAsset(ruta);
        TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(ruta);
        importador.textureType = TextureImporterType.Sprite;
        importador.spriteImportMode = SpriteImportMode.Single;
        importador.spritePixelsPerUnit = pixelesPorUnidad;
        importador.spriteBorder = borde;
        importador.filterMode = FilterMode.Bilinear;
        importador.wrapMode = TextureWrapMode.Clamp;
        importador.mipmapEnabled = false;
        importador.alphaIsTransparency = true;
        // Sin compresión: comprimir un degradado de cielo lo llena de franjas.
        importador.textureCompression = TextureImporterCompression.Uncompressed;

        // Una imagen estirada por partes necesita la malla completa del sprite.
        TextureImporterSettings ajustes = new TextureImporterSettings();
        importador.ReadTextureSettings(ajustes);
        ajustes.spriteMeshType = SpriteMeshType.FullRect;
        importador.SetTextureSettings(ajustes);

        importador.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
    }
}
