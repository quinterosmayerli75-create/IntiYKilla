// =====================================================================
// HerramientasEscenaTrampasDesafio.cs
// Proyecto: Inti y Killa
// Descripción: Piezas de escena de las trampas del "desafío" (la versión más
//              engañosa del juego): sensor que dispara trampas, lluvia de
//              rocas, poción engañosa, suelo que desaparece al saltar, pared
//              aplastadora, bloque que cierra el regreso, suelo en contra y
//              enemigo que cae del techo.
//              Es otra parte de HerramientasEscena.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

public static partial class HerramientasEscena
{
    // Sensor invisible: cuando se cumple la condición dentro del rectángulo,
    // activa todas las trampas de la lista (que deben tener seActivaSola = false).
    public static SensorTrampa CrearSensorTrampa(string nombre, Vector2 centro, Vector2 tamano, CondicionSensor condicion, float retardo, List<TrampaActivable> trampas, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(centro.x, centro.y, 0f);

        SensorTrampa sensor = objeto.AddComponent<SensorTrampa>();
        sensor.Configurar(tamano, condicion, retardo, trampas);
        sensor.ConfigurarSpriteBase(spriteCuadrado);
        return sensor;
    }

    // Rocas que caen en fila entre inicioX y finX; cada una avisa con su sombra.
    public static LluviaDeRocas CrearLluviaDeRocas(string nombre, float inicioX, float finX, float alturaTecho, float alturaPiso, int cantidad, bool seActivaSola, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3((inicioX + finX) / 2f, alturaTecho, 0f);

        LluviaDeRocas trampa = objeto.AddComponent<LluviaDeRocas>();
        trampa.Configurar(inicioX, finX, alturaTecho, alturaPiso, cantidad);
        trampa.ConfigurarDisparo(seActivaSola);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Botellita morada que late. Parece un premio; marea al que la toma.
    public static PocionEnganosa CrearPocionEnganosa(string nombre, Vector2 posicion, Transform padre)
    {
        GameObject pocion = new GameObject(nombre);
        pocion.transform.SetParent(padre);
        pocion.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        pocion.transform.localScale = new Vector3(0.5f, 0.6f, 1f);

        SpriteRenderer dibujo = pocion.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.75f, 0.35f, 1f);
        dibujo.sortingOrder = 8;

        // El cuello de la botella, para que se lea como "poción" y no como gema.
        GameObject cuello = CrearDibujoHijo("Cuello", pocion.transform, new Vector2(0f, 0.6f), new Vector2(0.35f, 0.4f), new Color(0.85f, 0.75f, 1f), 8);
        cuello.name = "Cuello de la pocion";

        PocionEnganosa trampa = pocion.AddComponent<PocionEnganosa>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Plataforma con grietas finas: desaparece si alguien salta estando encima.
    public static SueloQueDesapareceAlSaltar CrearSueloQueDesapareceAlSaltar(string nombre, Vector2 posicion, Vector2 tamano, Color colorRespaldo, Transform padre)
    {
        GameObject plataforma = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);

        // Las grietas son la pista: tres rayitas oscuras sobre la superficie.
        Color colorGrieta = new Color(0f, 0f, 0f, 0.35f);
        float alturaSuperficie = posicion.y + tamano.y / 2f - 0.06f;
        CrearMarcaEnMundo("Grieta 1", plataforma.transform, new Vector2(posicion.x - tamano.x * 0.3f, alturaSuperficie), new Vector2(0.35f, 0.05f), colorGrieta);
        CrearMarcaEnMundo("Grieta 2", plataforma.transform, new Vector2(posicion.x, alturaSuperficie), new Vector2(0.25f, 0.05f), colorGrieta);
        CrearMarcaEnMundo("Grieta 3", plataforma.transform, new Vector2(posicion.x + tamano.x * 0.3f, alturaSuperficie), new Vector2(0.3f, 0.05f), colorGrieta);

        SueloQueDesapareceAlSaltar trampa = plataforma.AddComponent<SueloQueDesapareceAlSaltar>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Pared que avanza "recorrido" cuando alguien entra en su camino.
    public static ParedAplastadora CrearParedAplastadora(string nombre, Vector2 posicion, Vector2 tamano, Vector2 recorrido, bool seActivaSola, Color colorRespaldo, Transform padre)
    {
        GameObject pared = CrearBloque(nombre, posicion, tamano, colorRespaldo, padre);

        // Rayas de peligro muy tenues en la cara que avanza: una pista para quien mira.
        float signoX = Mathf.Sign(recorrido.x);
        float signoY = Mathf.Sign(recorrido.y);
        Color colorRaya = new Color(0.55f, 0.1f, 0.05f, 0.45f);
        if (Mathf.Abs(recorrido.x) >= Mathf.Abs(recorrido.y))
        {
            Vector2 cara = new Vector2(posicion.x + signoX * (tamano.x / 2f - 0.05f), posicion.y);
            CrearMarcaEnMundo("Cara peligrosa", pared.transform, cara, new Vector2(0.1f, tamano.y * 0.9f), colorRaya);
        }
        else
        {
            Vector2 cara = new Vector2(posicion.x, posicion.y + signoY * (tamano.y / 2f - 0.05f));
            CrearMarcaEnMundo("Cara peligrosa", pared.transform, cara, new Vector2(tamano.x * 0.9f, 0.1f), colorRaya);
        }

        ParedAplastadora trampa = pared.AddComponent<ParedAplastadora>();
        trampa.Configurar(recorrido);
        trampa.ConfigurarDisparo(seActivaSola);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Bloque escondido que sube y tapa el camino de vuelta. Lo dispara un sensor.
    public static BloqueQueCierraElRegreso CrearBloqueQueCierraElRegreso(string nombre, Vector2 posicion, Vector2 tamano, Color colorRespaldo, Transform padre)
    {
        GameObject bloque = CrearPlataforma(nombre, posicion, tamano, colorRespaldo, padre);
        BloqueQueCierraElRegreso trampa = bloque.AddComponent<BloqueQueCierraElRegreso>();
        trampa.ConfigurarDisparo(false);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Tramo de piso que arrastra. velocidad negativa: hacia la izquierda.
    public static SueloEnContra CrearSueloEnContra(string nombre, float centroX, float ancho, float alturaPiso, float velocidad, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(centroX, alturaPiso, 0f);

        // Una franja oscura sobre el piso y marcas claras que corren encima.
        CrearDibujoHijo("Franja", objeto.transform, new Vector2(0f, 0.06f), new Vector2(ancho, 0.12f), new Color(0.15f, 0.12f, 0.1f, 0.55f), 2);
        int cantidadDeMarcas = Mathf.Max(2, Mathf.RoundToInt(ancho / 0.8f));
        Transform[] marcas = new Transform[cantidadDeMarcas];
        for (int indice = 0; indice < cantidadDeMarcas; indice++)
        {
            float posicionX = -ancho / 2f + ancho * indice / cantidadDeMarcas;
            GameObject marca = CrearDibujoHijo("Marca " + (indice + 1), objeto.transform, new Vector2(posicionX, 0.06f), new Vector2(0.2f, 0.1f), new Color(1f, 0.9f, 0.6f, 0.8f), 3);
            marcas[indice] = marca.transform;
        }

        SueloEnContra trampa = objeto.AddComponent<SueloEnContra>();
        trampa.Configurar(ancho, velocidad, marcas);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Bicho pegado al techo; al pasar debajo se deja caer y corre un tramo.
    public static EnemigoQueCaeDelTecho CrearEnemigoQueCaeDelTecho(string nombre, Vector2 posicion, float alturaPiso, Color color, Transform padre)
    {
        GameObject enemigo = new GameObject(nombre);
        enemigo.transform.SetParent(padre);
        enemigo.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        enemigo.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        SpriteRenderer dibujo = enemigo.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = color;
        dibujo.sortingOrder = 8;

        EnemigoQueCaeDelTecho trampa = enemigo.AddComponent<EnemigoQueCaeDelTecho>();
        trampa.Configurar(alturaPiso);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Dibujo hijo colocado y medido en coordenadas del mundo, aunque el padre
    // esté estirado (los bloques de color usan la escala para su tamaño).
    private static GameObject CrearMarcaEnMundo(string nombre, Transform padre, Vector2 posicionMundo, Vector2 tamanoMundo, Color color)
    {
        GameObject marca = new GameObject(nombre);
        marca.transform.SetParent(padre);
        marca.transform.position = new Vector3(posicionMundo.x, posicionMundo.y, 0f);
        Vector3 escalaPadre = padre.lossyScale;
        marca.transform.localScale = new Vector3(tamanoMundo.x / escalaPadre.x, tamanoMundo.y / escalaPadre.y, 1f);

        SpriteRenderer dibujo = marca.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCuadrado;
        dibujo.color = color;
        dibujo.sortingOrder = 6;
        return marca;
    }
}
