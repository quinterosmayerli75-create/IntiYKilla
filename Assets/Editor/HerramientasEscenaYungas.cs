// =====================================================================
// HerramientasEscenaYungas.cs
// Proyecto: Inti y Killa
// Descripción: Piezas de escena del Mundo 3 (Yungas): fondo, niebla, lluvia,
//              día y noche, charcos que se congelan, cóndor y piedras rodantes.
//              Es la otra parte de HerramientasEscena; está separada para que
//              las piezas de cada mundo sean fáciles de encontrar.
// =====================================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static partial class HerramientasEscena
{
    public static readonly Color colorSueloYungas = new Color(0.30f, 0.23f, 0.16f);
    private static readonly Color colorTechoHojas = new Color(0.86f, 0.68f, 0.32f);
    private static readonly Color colorCharcoCongelable = new Color(0.30f, 0.72f, 1f);
    private static readonly Color colorPlataformaDia = new Color(1f, 0.76f, 0.28f);
    private static readonly Color colorPlataformaNoche = new Color(0.46f, 0.44f, 0.98f);

    private static Material ObtenerMaterialSinLuces()
    {
        return AssetDatabase.LoadAssetAtPath<Material>(rutaMaterialSinLuces);
    }

    // Paisaje de las montañas; se reemplaza solo con Interfaz/FondoMundo3.png.
    public static void CrearFondoYungas()
    {
        Transform fondo = CrearGrupo("Fondo");
        Sprite paisaje = HerramientasInterfaz.ObtenerImagenFondo(3);
        // Un poco oscurecido: así las plataformas se distinguen del paisaje.
        CrearDecoracion("Paisaje", paisaje, new Vector2(0f, 4f), new Vector2(36f, 20.25f), colorPaisajeDelNivel, -30, false, fondo);
    }

    public static void CrearNiebla()
    {
        GameObject objeto = new GameObject("Niebla");
        Niebla niebla = objeto.AddComponent<Niebla>();
        niebla.Configurar(ObtenerMaterialSinLuces());
    }

    public static void CrearLluvia()
    {
        GameObject objeto = new GameObject("Lluvia");
        Lluvia lluvia = objeto.AddComponent<Lluvia>();
        lluvia.Configurar(spriteCuadrado, ObtenerMaterialSinLuces());
    }

    // El sol y la luna cruzan el cielo, detrás del nivel y delante del paisaje.
    public static void CrearCicloDiaNoche(Light2D luzGlobal)
    {
        GameObject objeto = new GameObject("Ciclo de dia y noche");
        Vector2 tamanoAstro = new Vector2(2.2f, 2.2f);
        GameObject sol = CrearDecoracion("Sol", spriteDisco, new Vector2(-15f, 6f), tamanoAstro, new Color(1f, 0.84f, 0.35f), -20, false, objeto.transform);
        GameObject luna = CrearDecoracion("Luna", spriteDisco, new Vector2(-15f, 6f), tamanoAstro, new Color(0.88f, 0.92f, 1f), -20, false, objeto.transform);

        CicloDiaNoche ciclo = objeto.AddComponent<CicloDiaNoche>();
        ciclo.Configurar(luzGlobal, sol.transform, luna.transform);
    }

    // Dorada si existe de día, azul si existe de noche.
    public static void CrearPlataformaDiaNoche(string nombre, Vector2 posicion, Vector2 tamano, bool existeDeDia, Transform padre)
    {
        Color color = colorPlataformaNoche;
        if (existeDeDia == true)
        {
            color = colorPlataformaDia;
        }

        // Con el bloque dibujado del mundo, teñido: cálido de día y azul de noche.
        Sprite bloqueDelMundo = ObtenerBloqueDelMundo();
        GameObject plataforma = CrearBloque(nombre, posicion, tamano, color, bloqueDelMundo, padre);
        if (bloqueDelMundo != null)
        {
            Color tinte = new Color(0.55f, 0.72f, 1f);
            if (existeDeDia == true)
            {
                tinte = new Color(1f, 0.88f, 0.5f);
            }
            plataforma.GetComponent<SpriteRenderer>().color = tinte;
        }
        plataforma.GetComponent<SpriteRenderer>().sortingOrder = 1;
        PlataformaDiaNoche componente = plataforma.AddComponent<PlataformaDiaNoche>();
        componente.Configurar(existeDeDia);
    }

    // Techo de paja, de color claro para distinguirlo de los pisos: protege de
    // la lluvia y también se puede pisar.
    public static void CrearTecho(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        CrearBloque(nombre, new Vector2(centroX, alturaPiso - 0.25f), new Vector2(ancho, 0.5f), colorTechoHojas, padre);
    }

    // Un celeste más claro que el agua común: esta sí se puede congelar.
    public static void CrearCharcoCongelable(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        GameObject charco = CrearCharco(nombre, TipoZona.Agua, centroX, ancho, alturaPiso, padre);
        charco.GetComponent<SpriteRenderer>().color = colorCharcoCongelable;
        charco.AddComponent<AguaCongelable>();
    }

    // Cóndor: mientras no haya arte es un óvalo oscuro.
    public static void CrearCondor(string nombre, Vector2 posicion, float rangoHorizontal, float rangoVertical, Transform padre)
    {
        GameObject condor = new GameObject(nombre);
        condor.transform.SetParent(padre);
        condor.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        condor.transform.localScale = new Vector3(1.3f, 0.7f, 1f);

        SpriteRenderer dibujo = condor.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.12f, 0.11f, 0.16f);
        dibujo.sortingOrder = 8;

        EnemigoEnPicada enemigo = condor.AddComponent<EnemigoEnPicada>();
        enemigo.Configurar(rangoHorizontal, rangoVertical);
        enemigo.ConfigurarMensaje("Un cóndor se llevó a {0} de paseo.");
    }

    // distancia: positivo rueda a la derecha; negativo, a la izquierda.
    public static void CrearPiedraRodante(string nombre, float inicioX, float alturaPiso, float distancia, float velocidad, float segundosDeEspera, Transform padre)
    {
        float diametro = 0.9f;
        GameObject piedra = new GameObject(nombre);
        piedra.transform.SetParent(padre);
        piedra.transform.position = new Vector3(inicioX, alturaPiso + diametro / 2f, 0f);
        piedra.transform.localScale = new Vector3(diametro, diametro, 1f);

        SpriteRenderer dibujo = piedra.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.55f, 0.50f, 0.45f);
        dibujo.sortingOrder = 7;

        // Una marca oscura para que se note que va girando.
        CrearDibujoHijo("Marca", piedra.transform, new Vector2(0.2f, 0f), new Vector2(0.3f, 0.12f), new Color(0.25f, 0.22f, 0.20f), 8);

        PiedraRodante componente = piedra.AddComponent<PiedraRodante>();
        // Ruedan un 25 % más rápido que lo indicado: así presionan más (pedido del 23 nov).
        componente.Configurar(distancia, velocidad * 1.25f, segundosDeEspera);
    }
}
