// =====================================================================
// HerramientasEscenaTropico.cs
// Proyecto: Inti y Killa
// Descripción: Piezas de escena del Mundo 4 (Trópico): fondo, lianas, hojas
//              trampolín, monos, serpientes y la crecida del río. También la
//              flecha guía que señala hacia dónde hay que hacer algo.
//              Es otra parte de HerramientasEscena.
// =====================================================================

using UnityEngine;

public static partial class HerramientasEscena
{
    public static readonly Color colorSueloTropico = new Color(0.24f, 0.17f, 0.10f);

    public static void CrearFondoTropico()
    {
        Transform fondo = CrearGrupo("Fondo");
        Sprite paisaje = HerramientasInterfaz.ObtenerImagenFondo(4);
        CrearDecoracion("Paisaje", paisaje, new Vector2(0f, 4f), new Vector2(36f, 20.25f), Color.white, -30, false, fondo);
    }

    // Flecha pintada en el fondo. No hace nada: solo muestra hacia dónde ir.
    public static void CrearFlechaGuia(string nombre, Vector2 posicion, bool haciaDerecha, Transform padre)
    {
        Sprite flecha = GeneradorArteMenus.ObtenerFlecha();
        Color colorGuia = new Color(1f, 0.85f, 0.25f, 0.8f);
        GameObject objeto = CrearDecoracion(nombre, flecha, posicion, new Vector2(1.1f, 1.1f), colorGuia, -5, false, padre);

        // El dibujo apunta a la izquierda: se voltea para la derecha.
        if (haciaDerecha == true)
        {
            Vector3 escala = objeto.transform.localScale;
            objeto.transform.localScale = new Vector3(-Mathf.Abs(escala.x), escala.y, escala.z);
        }
    }

    // anclaje: punto de donde cuelga. fase 0 y 0.5 se balancean al revés.
    public static void CrearLiana(string nombre, Vector2 anclaje, float largo, float segundosPorVuelta, float fase, Transform padre)
    {
        GameObject liana = new GameObject(nombre);
        liana.transform.SetParent(padre);
        liana.transform.position = new Vector3(anclaje.x, anclaje.y, 0f);

        Color colorCuerda = new Color(0.30f, 0.52f, 0.18f);
        CrearDibujoHijo("Cuerda", liana.transform, new Vector2(0f, -largo / 2f), new Vector2(0.16f, largo), colorCuerda, 3);
        GameObject nudo = CrearDibujoHijo("Nudo", liana.transform, new Vector2(0f, -largo), new Vector2(0.4f, 0.4f), new Color(0.42f, 0.66f, 0.24f), 3);
        nudo.GetComponent<SpriteRenderer>().sprite = spriteCirculo;

        Liana componente = liana.AddComponent<Liana>();
        componente.Configurar(largo, segundosPorVuelta, fase);
    }

    // alturaPiso: el piso sobre el que se apoya.
    public static void CrearHojaTrampolin(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        Vector2 tamano = new Vector2(ancho, 0.35f);
        GameObject hoja = new GameObject(nombre);
        hoja.transform.SetParent(padre);
        hoja.transform.position = new Vector3(centroX, alturaPiso + tamano.y / 2f, 0f);
        hoja.transform.localScale = new Vector3(tamano.x, tamano.y, 1f);

        SpriteRenderer dibujo = hoja.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.40f, 0.95f, 0.30f);
        dibujo.sortingOrder = 2;

        HojaTrampolin componente = hoja.AddComponent<HojaTrampolin>();
        componente.Configurar(tamano);
    }

    // Mono: mientras no haya arte es un círculo marrón claro.
    public static void CrearMono(string nombre, Vector2 posicion, float alcance, float segundosEntreLanzamientos, Transform padre)
    {
        GameObject mono = new GameObject(nombre);
        mono.transform.SetParent(padre);
        mono.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        mono.transform.localScale = new Vector3(0.9f, 0.9f, 1f);

        SpriteRenderer dibujo = mono.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.72f, 0.48f, 0.26f);
        dibujo.sortingOrder = 8;

        LanzadorProyectiles lanzador = mono.AddComponent<LanzadorProyectiles>();
        lanzador.Configurar(alcance, segundosEntreLanzamientos, spriteCirculo);
        lanzador.ConfigurarMensaje("Un mono atrapó a {0} por la oreja.");
    }

    // Serpiente: mientras no haya arte es un óvalo largo y bajo.
    public static void CrearSerpiente(string nombre, float limiteIzquierdo, float limiteDerecho, float alturaPiso, float velocidad, Transform padre)
    {
        GameObject serpiente = new GameObject(nombre);
        serpiente.transform.SetParent(padre);
        serpiente.transform.position = new Vector3(limiteIzquierdo, alturaPiso + 0.25f, 0f);
        serpiente.transform.localScale = new Vector3(1.3f, 0.5f, 1f);

        SpriteRenderer dibujo = serpiente.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.72f, 0.85f, 0.15f);
        dibujo.sortingOrder = 8;

        EnemigoPatrulla patrulla = serpiente.AddComponent<EnemigoPatrulla>();
        patrulla.Configurar(limiteIzquierdo, limiteDerecho, velocidad);
        patrulla.ConfigurarMensaje("Una serpiente mordió a {0} en el tobillo.");
    }

    public static void CrearCrecida(float alturaFinal, float velocidad, float segundosDeAviso)
    {
        GameObject crecida = new GameObject("Crecida del rio");
        SpriteRenderer dibujo = crecida.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCuadrado;
        dibujo.color = new Color(0.45f, 0.30f, 0.14f, 0.88f);
        dibujo.sortingOrder = 13;

        CrecidaRio componente = crecida.AddComponent<CrecidaRio>();
        componente.Configurar(alturaFinal, velocidad, segundosDeAviso);
    }
}
