// =====================================================================
// HerramientasEscenaAiquileIsla.cs
// Proyecto: Inti y Killa
// Descripción: Piezas de escena de los Mundos 5 (Aiquile) y 6 (Isla del Sol):
//              cuerdas de charango, puertas melódicas, avispas, llamas, eclipse
//              automático y el Guardián del Eclipse.
//              Es otra parte de HerramientasEscena.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

public static partial class HerramientasEscena
{
    private static readonly Color colorPaisajeDelNivel = new Color(0.72f, 0.72f, 0.78f);
    public static readonly Color colorSueloAiquile = new Color(0.46f, 0.28f, 0.16f);
    public static readonly Color colorSueloIsla = new Color(0.40f, 0.36f, 0.30f);

    // Paisaje del mundo; se reemplaza solo con Interfaz/FondoMundoN.png.
    public static void CrearFondoDeMundo(int numeroMundo)
    {
        Transform fondo = CrearGrupo("Fondo");
        Sprite paisaje = HerramientasInterfaz.ObtenerImagenFondo(numeroMundo);
        // Un poco oscurecido: así las plataformas se distinguen del paisaje.
        CrearDecoracion("Paisaje", paisaje, new Vector2(0f, 4f), new Vector2(36f, 20.25f), colorPaisajeDelNivel, -30, false, fondo);
    }

    // Cuerda tendida entre dos clavijas, del color de su nota.
    public static void CrearCuerda(string nombre, float centroX, float alturaPiso, int nota, MomentoCuerda momento, Transform padre)
    {
        float ancho = 1.4f;
        GameObject cuerda = new GameObject(nombre);
        cuerda.transform.SetParent(padre);
        cuerda.transform.position = new Vector3(centroX, alturaPiso + 0.2f, 0f);
        cuerda.transform.localScale = new Vector3(ancho, 0.14f, 1f);

        SpriteRenderer dibujo = cuerda.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCuadrado;
        dibujo.color = CuerdaCharango.ObtenerColorNota(nota);
        dibujo.sortingOrder = 2;

        Color colorClavija = new Color(0.35f, 0.22f, 0.12f);
        CrearDecoracion(nombre + " clavija 1", spriteCuadrado, new Vector2(centroX - ancho / 2f - 0.1f, alturaPiso + 0.2f), new Vector2(0.15f, 0.4f), colorClavija, 2, false, padre);
        CrearDecoracion(nombre + " clavija 2", spriteCuadrado, new Vector2(centroX + ancho / 2f + 0.1f, alturaPiso + 0.2f), new Vector2(0.15f, 0.4f), colorClavija, 2, false, padre);

        CuerdaCharango componente = cuerda.AddComponent<CuerdaCharango>();
        componente.Configurar(nota, momento, new Vector2(ancho, 0.45f));
    }

    // Barras que se abren con la melodía, y arriba una luz por nota.
    public static PuertaMelodica CrearPuertaMelodica(string nombre, int[] melodia, Vector2 centroLuces, Vector2[] centrosBarras, Vector2 tamanoBarra, Vector2 desplazamiento, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);

        Rigidbody2D[] barras = new Rigidbody2D[centrosBarras.Length];
        for (int indice = 0; indice < centrosBarras.Length; indice++)
        {
            GameObject barra = CrearBloque(nombre + " barra " + (indice + 1), centrosBarras[indice], tamanoBarra, new Color(0.55f, 0.40f, 0.20f), objeto.transform);
            barra.GetComponent<SpriteRenderer>().sortingOrder = 2;
            barras[indice] = barra.AddComponent<Rigidbody2D>();
            barras[indice].bodyType = RigidbodyType2D.Kinematic;
        }

        float separacion = 0.75f;
        float inicioX = centroLuces.x - separacion * (melodia.Length - 1) / 2f;

        // Panel oscuro detrás de las luces, para que se lean sobre cualquier fondo.
        Vector2 tamanoPanel = new Vector2(separacion * melodia.Length + 0.3f, 0.95f);
        CrearDecoracion(nombre + " panel", spriteCuadrado, centroLuces, tamanoPanel, new Color(0.10f, 0.06f, 0.08f, 0.9f), 3, false, objeto.transform);
        SpriteRenderer[] luces = new SpriteRenderer[melodia.Length];
        for (int indice = 0; indice < melodia.Length; indice++)
        {
            Vector2 posicion = new Vector2(inicioX + indice * separacion, centroLuces.y);
            GameObject luz = CrearDecoracion(nombre + " luz " + (indice + 1), spriteCirculo, posicion, new Vector2(0.55f, 0.55f), CuerdaCharango.ObtenerColorNota(melodia[indice]), 4, false, objeto.transform);
            luces[indice] = luz.GetComponent<SpriteRenderer>();
        }

        PuertaMelodica puerta = objeto.AddComponent<PuertaMelodica>();
        puerta.Configurar(melodia, luces, barras, desplazamiento);
        return puerta;
    }

    // Avispa: mientras no haya arte es un círculo amarillo pequeño.
    public static void CrearAvispa(string nombre, Vector2 posicion, Vector2 centroZona, Vector2 tamanoZona, float velocidad, Transform padre)
    {
        GameObject avispa = new GameObject(nombre);
        avispa.transform.SetParent(padre);
        avispa.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        avispa.transform.localScale = new Vector3(0.6f, 0.5f, 1f);

        SpriteRenderer dibujo = avispa.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(1f, 0.85f, 0.10f);
        dibujo.sortingOrder = 8;

        EnemigoPerseguidor perseguidor = avispa.AddComponent<EnemigoPerseguidor>();
        perseguidor.Configurar(centroZona, tamanoZona, velocidad);
        perseguidor.ConfigurarMensaje("Una avispa picó a {0}. ¡Auch!");
    }

    // Llama: mientras no haya arte es un óvalo blanco.
    public static void CrearLlama(string nombre, Vector2 posicion, float alcance, float segundosEntreLanzamientos, Transform padre)
    {
        GameObject llama = new GameObject(nombre);
        llama.transform.SetParent(padre);
        llama.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        llama.transform.localScale = new Vector3(0.8f, 1f, 1f);

        SpriteRenderer dibujo = llama.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.95f, 0.92f, 0.85f);
        dibujo.sortingOrder = 8;

        LanzadorProyectiles lanzador = llama.AddComponent<LanzadorProyectiles>();
        lanzador.Configurar(alcance, segundosEntreLanzamientos, spriteCirculo);
        lanzador.ConfigurarProyectil(new Color(0.85f, 0.95f, 0.80f), 0.35f, "La llama escupió a {0}. ¡Qué asco!");
        lanzador.ConfigurarMensaje("{0} se acercó demasiado a la llama.");
    }

    public static void CrearCicloEclipse(float segundosEntreEclipses)
    {
        GameObject objeto = new GameObject("Eclipse automatico");
        CicloEclipse ciclo = objeto.AddComponent<CicloEclipse>();
        ciclo.Configurar(segundosEntreEclipses, spriteDisco, spriteResplandor);
    }

    // El Guardián: un disco oscuro con una corona de luz, como un eclipse con ojos.
    public static GuardianEclipse CrearGuardian(Vector2 posicion, BotonPresion botonUno, BotonPresion botonDos, ObjetoActivable[] alSerVencido)
    {
        GameObject guardian = new GameObject("Guardian del Eclipse");
        guardian.transform.position = new Vector3(posicion.x, posicion.y, 0f);

        SpriteRenderer dibujo = guardian.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteDisco;
        dibujo.color = new Color(0.12f, 0.10f, 0.22f);
        dibujo.sortingOrder = 8;
        guardian.transform.localScale = new Vector3(2.2f / spriteDisco.bounds.size.x, 2.2f / spriteDisco.bounds.size.y, 1f);

        GameObject corona = CrearDecoracion("Corona", spriteResplandor, posicion, new Vector2(4.2f, 4.2f), new Color(1f, 0.8f, 0.4f, 0.8f), 7, false, guardian.transform);
        corona.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        GameObject ojoIzquierdo = CrearDecoracion("Ojo izquierdo", spriteCirculo, posicion + new Vector2(-0.4f, 0.2f), new Vector2(0.35f, 0.35f), new Color(1f, 0.3f, 0.2f), 9, false, guardian.transform);
        GameObject ojoDerecho = CrearDecoracion("Ojo derecho", spriteCirculo, posicion + new Vector2(0.4f, 0.2f), new Vector2(0.35f, 0.35f), new Color(1f, 0.3f, 0.2f), 9, false, guardian.transform);
        ojoIzquierdo.transform.position = new Vector3(posicion.x - 0.4f, posicion.y + 0.2f, 0f);
        ojoDerecho.transform.position = new Vector3(posicion.x + 0.4f, posicion.y + 0.2f, 0f);

        GuardianEclipse componente = guardian.AddComponent<GuardianEclipse>();
        componente.Configurar(botonUno, botonDos, alSerVencido, spriteCirculo);
        return componente;
    }

    public static BotonPresion BuscarBoton(GameObject objeto)
    {
        return objeto.GetComponent<BotonPresion>();
    }

    public static List<ObjetoActivable> ListaCon(ObjetoActivable objeto)
    {
        List<ObjetoActivable> lista = new List<ObjetoActivable>();
        lista.Add(objeto);
        return lista;
    }
}
