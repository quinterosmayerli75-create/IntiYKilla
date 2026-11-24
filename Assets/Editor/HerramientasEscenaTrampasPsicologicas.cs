// =====================================================================
// HerramientasEscenaTrampasPsicologicas.cs
// Proyecto: Inti y Killa
// Descripción: Piezas de escena de las trampas "psicológicas": la sala que
//              se repite, objetos de ayuda disfrazados (trampolín catapulta,
//              gema imán, poción de peso), fondo que cobra vida (nube,
//              arbusto, antorcha, cartel) y mecánicas que cambian de rol
//              (arena movediza, púas de goma, enemigo escalera, roca gigante).
//              Es otra parte de HerramientasEscena.
// =====================================================================

using UnityEngine;

public static partial class HerramientasEscena
{
    // ---------- Sala que se repite ----------

    public static SalaQueSeRepite CrearSalaQueSeRepite(int vueltas, string[] mensajes)
    {
        GameObject objeto = new GameObject("Sala que se repite");
        SalaQueSeRepite sala = objeto.AddComponent<SalaQueSeRepite>();
        sala.Configurar(vueltas, mensajes);
        return sala;
    }

    // El objeto solo existe en las vueltas indicadas (0 = la primera).
    public static void AparecerSoloEnVueltas(GameObject objeto, int[] vueltas)
    {
        VariantePorVuelta variante = objeto.AddComponent<VariantePorVuelta>();
        variante.Configurar(vueltas);
    }

    // ---------- Presión de tiempo y castigo del final ----------

    public static TiempoLimite CrearTiempoLimite(float segundos)
    {
        GameObject objeto = new GameObject("Tiempo limite");
        TiempoLimite tiempo = objeto.AddComponent<TiempoLimite>();
        tiempo.Configurar(segundos);
        return tiempo;
    }

    // Al perder en este nivel se vuelve a otra escena en vez de repetirlo.
    public static VolverAlPerder CrearVolverAlPerder(string escenaDestino, string mensaje)
    {
        GameObject objeto = new GameObject("Volver al perder");
        VolverAlPerder volver = objeto.AddComponent<VolverAlPerder>();
        volver.Configurar(escenaDestino, mensaje);
        return volver;
    }

    // ---------- Objetos de ayuda disfrazados ----------

    // Igual a una hoja trampolín, pero lanza contra las púas del techo. La pista:
    // tiene una vena roja apenas visible.
    public static void CrearTrampolinCatapulta(string nombre, float centroX, float ancho, float alturaPiso, float alturaTecho, Transform padre)
    {
        CrearHojaTrampolin(nombre, centroX, ancho, alturaPiso, padre);
        GameObject hoja = padre.Find(nombre).gameObject;
        hoja.GetComponent<HojaTrampolin>().ConfigurarFuerza(30f);
        CrearMarcaEnMundo("Vena roja", hoja.transform, new Vector2(centroX, alturaPiso + 0.2f), new Vector2(ancho * 0.5f, 0.05f), new Color(0.8f, 0.15f, 0.1f, 0.6f));

        CrearPuasFijas(nombre + " - puas del techo", new Vector2(centroX, alturaTecho), ancho + 1.5f, true, padre);
    }

    // haciaAbajo: púas colgando del techo (true) o paradas en el piso (false).
    public static PuasFijas CrearPuasFijas(string nombre, Vector2 posicion, float ancho, bool haciaAbajo, Transform padre)
    {
        GameObject zona = new GameObject(nombre);
        zona.transform.SetParent(padre);
        zona.transform.position = new Vector3(posicion.x, posicion.y, 0f);

        Sprite spritePua = imagenCristales;
        Color colorPua = Color.white;
        if (spritePua == null)
        {
            spritePua = ObtenerSpriteCristal();
            colorPua = new Color(0.85f, 0.85f, 0.9f);
        }

        float anchoPua = 0.5f;
        float desplazamientoY = 0.3f;
        if (haciaAbajo == true)
        {
            desplazamientoY = -0.3f;
        }
        int cantidad = Mathf.Max(1, Mathf.RoundToInt(ancho / anchoPua));
        for (int indice = 0; indice < cantidad; indice++)
        {
            float posicionX = posicion.x - ancho / 2f + anchoPua / 2f + indice * anchoPua;
            CrearDecoracion("Pua " + (indice + 1), spritePua, new Vector2(posicionX, posicion.y + desplazamientoY), new Vector2(anchoPua, 0.6f), colorPua, 3, haciaAbajo, zona.transform);
        }

        PuasFijas puas = zona.AddComponent<PuasFijas>();
        puas.Configurar(new Vector2(ancho, 0.6f), "{0} salió volando contra las púas.");
        puas.ConfigurarSpriteBase(spriteCuadrado);
        return puas;
    }

    // La gema imán: brilla como las demás, pero tiene un anillo gris que late.
    public static GemaIman CrearGemaIman(string nombre, TipoElemento elemento, Vector2 posicion, Transform padre)
    {
        GameObject gema = CrearGema(nombre, elemento, posicion, padre);
        Object.DestroyImmediate(gema.GetComponent<Gema>());
        Object.DestroyImmediate(gema.GetComponent<CircleCollider2D>());

        GameObject anillo = CrearMarcaEnMundo("Anillo gris", gema.transform, posicion, new Vector2(0.9f, 0.9f), new Color(0.5f, 0.5f, 0.55f, 0.35f));
        anillo.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
        anillo.GetComponent<SpriteRenderer>().sortingOrder = 4;

        GemaIman trampa = gema.AddComponent<GemaIman>();
        trampa.ConfigurarSpriteBase(spriteCirculo);
        return trampa;
    }

    // Poción gris sobre un piso agrietado: al tomarla, ese piso se rompe.
    public static void CrearPocionDePeso(string nombre, Vector2 posicionPiso, Vector2 tamanoPiso, Color colorPiso, Transform padre)
    {
        SueloQueDesapareceAlSaltar piso = CrearSueloQueDesapareceAlSaltar(nombre + " - piso agrietado", posicionPiso, tamanoPiso, colorPiso, padre);
        piso.ConfigurarDisparo(false);

        GameObject pocion = new GameObject(nombre);
        pocion.transform.SetParent(padre);
        float alturaSuperficie = posicionPiso.y + tamanoPiso.y / 2f;
        pocion.transform.position = new Vector3(posicionPiso.x, alturaSuperficie + 0.45f, 0f);
        pocion.transform.localScale = new Vector3(0.55f, 0.65f, 1f);
        SpriteRenderer dibujo = pocion.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.55f, 0.55f, 0.6f);
        dibujo.sortingOrder = 8;
        CrearDibujoHijo("Cuello", pocion.transform, new Vector2(0f, 0.6f), new Vector2(0.35f, 0.4f), new Color(0.75f, 0.75f, 0.8f), 8);

        PocionDePeso trampa = pocion.AddComponent<PocionDePeso>();
        trampa.Configurar(piso);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
    }

    // ---------- Fondo que cobra vida ----------

    public static NubeAsesina CrearNubeAsesina(string nombre, Vector2 posicion, float alturaPiso, Transform padre)
    {
        GameObject nube = new GameObject(nombre);
        nube.transform.SetParent(padre);
        nube.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        nube.transform.localScale = new Vector3(2.4f, 1f, 1f);
        SpriteRenderer dibujo = nube.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.95f, 0.95f, 1f, 0.9f);
        dibujo.sortingOrder = -2;

        NubeAsesina trampa = nube.AddComponent<NubeAsesina>();
        trampa.Configurar(alturaPiso);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Arbusto de adorno con dos puntitos brillantes: la pista de que tiene ojos.
    public static ArbustoTrampa CrearArbustoTrampa(string nombre, float centroX, float alturaPiso, Transform padre)
    {
        GameObject arbusto = new GameObject(nombre);
        arbusto.transform.SetParent(padre);
        arbusto.transform.position = new Vector3(centroX, alturaPiso + 0.4f, 0f);
        arbusto.transform.localScale = new Vector3(1.3f, 0.8f, 1f);
        SpriteRenderer dibujo = arbusto.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(0.22f, 0.55f, 0.2f);
        dibujo.sortingOrder = 4;

        GameObject ojoUno = CrearMarcaEnMundo("Ojo 1", arbusto.transform, new Vector2(centroX - 0.18f, alturaPiso + 0.5f), new Vector2(0.08f, 0.08f), new Color(1f, 0.9f, 0.3f));
        GameObject ojoDos = CrearMarcaEnMundo("Ojo 2", arbusto.transform, new Vector2(centroX + 0.18f, alturaPiso + 0.5f), new Vector2(0.08f, 0.08f), new Color(1f, 0.9f, 0.3f));
        ojoUno.GetComponent<SpriteRenderer>().sortingOrder = 5;
        ojoDos.GetComponent<SpriteRenderer>().sortingOrder = 5;

        ArbustoTrampa trampa = arbusto.AddComponent<ArbustoTrampa>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Antorcha de la pared: parece luz del fondo, pero escupe bolas de fuego.
    public static void CrearAntorchaQueDispara(string nombre, Vector2 posicion, float alcance, float segundosEntreDisparos, Transform padre)
    {
        GameObject antorcha = new GameObject(nombre);
        antorcha.transform.SetParent(padre);
        antorcha.transform.position = new Vector3(posicion.x, posicion.y, 0f);
        antorcha.transform.localScale = new Vector3(0.45f, 0.6f, 1f);
        SpriteRenderer dibujo = antorcha.AddComponent<SpriteRenderer>();
        dibujo.sprite = spriteCirculo;
        dibujo.color = new Color(1f, 0.65f, 0.15f);
        dibujo.sortingOrder = 6;
        CrearDibujoHijo("Palo", antorcha.transform, new Vector2(0f, -0.9f), new Vector2(0.3f, 1.2f), new Color(0.4f, 0.25f, 0.12f), 5);

        LanzadorProyectiles lanzador = antorcha.AddComponent<LanzadorProyectiles>();
        lanzador.Configurar(alcance, segundosEntreDisparos, spriteCirculo);
        lanzador.ConfigurarProyectil(new Color(1f, 0.45f, 0.1f), 0.4f, "La antorcha no era adorno: una bola de fuego alcanzó a {0}.");
        lanzador.ConfigurarMensaje("{0} tocó la antorcha encendida.");
    }

    // Letrero con letras sueltas que caen cuando alguien pasa por debajo.
    public static CartelQueCae CrearCartelQueCae(string nombre, string texto, Vector2 posicion, float alturaPiso, Transform padre)
    {
        GameObject cartel = new GameObject(nombre);
        cartel.transform.SetParent(padre);
        cartel.transform.position = new Vector3(posicion.x, posicion.y, 0f);

        float separacion = 0.9f;
        float inicioX = posicion.x - (texto.Length - 1) * separacion / 2f;
        Transform[] letras = new Transform[texto.Length];
        Font fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        for (int indice = 0; indice < texto.Length; indice++)
        {
            // Cada letra es un bloque de madera con su letra encima.
            GameObject letra = new GameObject("Letra " + texto[indice]);
            letra.transform.SetParent(cartel.transform);
            letra.transform.position = new Vector3(inicioX + indice * separacion, posicion.y, 0f);
            GameObject bloque = CrearDibujoHijo("Bloque", letra.transform, Vector2.zero, new Vector2(0.8f, 0.8f), new Color(0.75f, 0.55f, 0.3f), 6);
            bloque.name = "Bloque de la letra";

            GameObject textoObjeto = new GameObject("Texto");
            textoObjeto.transform.SetParent(letra.transform);
            textoObjeto.transform.localPosition = new Vector3(0f, 0f, -0.1f);
            TextMesh malla = textoObjeto.AddComponent<TextMesh>();
            malla.text = texto[indice].ToString();
            malla.font = fuente;
            malla.characterSize = 0.12f;
            malla.fontSize = 48;
            malla.anchor = TextAnchor.MiddleCenter;
            malla.color = new Color(0.2f, 0.1f, 0.05f);
            MeshRenderer renderizador = textoObjeto.GetComponent<MeshRenderer>();
            renderizador.sharedMaterial = fuente.material;
            renderizador.sortingOrder = 7;

            letras[indice] = letra.transform;
        }

        CartelQueCae trampa = cartel.AddComponent<CartelQueCae>();
        trampa.Configurar(letras, alturaPiso);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // ---------- Mecánicas que cambian de rol ----------

    // Parece un charco de agua, pero tiene puntitos de arena: es arena movediza.
    public static ArenaMovediza CrearArenaMovediza(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        objeto.transform.position = new Vector3(centroX, alturaPiso, 0f);

        CrearDibujoHijo("Charco", objeto.transform, new Vector2(0f, 0.15f), new Vector2(ancho, 0.3f), new Color(0.25f, 0.45f, 0.75f, 0.9f), 2);
        int cantidadPuntos = Mathf.Max(2, Mathf.RoundToInt(ancho * 2f));
        for (int indice = 0; indice < cantidadPuntos; indice++)
        {
            float posicionX = -ancho / 2f + ancho * (indice + 0.5f) / cantidadPuntos;
            CrearDibujoHijo("Arena " + (indice + 1), objeto.transform, new Vector2(posicionX, 0.2f), new Vector2(0.06f, 0.06f), new Color(0.85f, 0.75f, 0.45f, 0.8f), 3);
        }

        ArenaMovediza trampa = objeto.AddComponent<ArenaMovediza>();
        trampa.Configurar(ancho);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Púas que parecen mortales pero son de goma: lanzan hacia arriba.
    public static PuasDeGoma CrearPuasDeGoma(string nombre, float centroX, float ancho, float alturaPiso, Transform padre)
    {
        GameObject zona = new GameObject(nombre);
        zona.transform.SetParent(padre);
        zona.transform.position = new Vector3(centroX, alturaPiso, 0f);

        Sprite spritePua = imagenCristales;
        Color colorPua = new Color(0.95f, 0.95f, 1f);
        if (spritePua == null)
        {
            spritePua = ObtenerSpriteCristal();
            colorPua = new Color(0.95f, 0.9f, 1f);
        }
        float anchoPua = 0.5f;
        int cantidad = Mathf.Max(1, Mathf.RoundToInt(ancho / anchoPua));
        for (int indice = 0; indice < cantidad; indice++)
        {
            float posicionX = centroX - ancho / 2f + anchoPua / 2f + indice * anchoPua;
            CrearDecoracion("Pua " + (indice + 1), spritePua, new Vector2(posicionX, alturaPiso + 0.3f), new Vector2(anchoPua, 0.6f), colorPua, 3, false, zona.transform);
        }

        PuasDeGoma trampa = zona.AddComponent<PuasDeGoma>();
        trampa.Configurar(ancho, 22f);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Monstruo grande y sólido con ojos enojados: no hace daño, sirve de escalón.
    public static EnemigoEscalera CrearEnemigoEscalera(string nombre, float centroX, float alturaPiso, Vector2 tamano, Color color, Transform padre)
    {
        GameObject cuerpo = CrearBloque(nombre, new Vector2(centroX, alturaPiso + tamano.y / 2f), tamano, color, padre);
        float alturaOjos = alturaPiso + tamano.y * 0.7f;
        CrearMarcaEnMundo("Ojo izquierdo", cuerpo.transform, new Vector2(centroX - tamano.x * 0.2f, alturaOjos), new Vector2(0.25f, 0.25f), Color.white);
        CrearMarcaEnMundo("Ojo derecho", cuerpo.transform, new Vector2(centroX + tamano.x * 0.2f, alturaOjos), new Vector2(0.25f, 0.25f), Color.white);
        CrearMarcaEnMundo("Ceja izquierda", cuerpo.transform, new Vector2(centroX - tamano.x * 0.2f, alturaOjos + 0.22f), new Vector2(0.35f, 0.07f), Color.black);
        CrearMarcaEnMundo("Ceja derecha", cuerpo.transform, new Vector2(centroX + tamano.x * 0.2f, alturaOjos + 0.22f), new Vector2(0.35f, 0.07f), Color.black);
        CrearMarcaEnMundo("Boca", cuerpo.transform, new Vector2(centroX, alturaPiso + tamano.y * 0.35f), new Vector2(tamano.x * 0.5f, 0.1f), new Color(0.3f, 0f, 0f));

        EnemigoEscalera trampa = cuerpo.AddComponent<EnemigoEscalera>();
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    public static RocaGiganteQueRueda CrearRocaGigante(string nombre, float inicioX, float finX, float alturaPiso, float diametro, Transform padre)
    {
        GameObject objeto = new GameObject(nombre);
        objeto.transform.SetParent(padre);
        RocaGiganteQueRueda trampa = objeto.AddComponent<RocaGiganteQueRueda>();
        trampa.Configurar(inicioX, finX, alturaPiso, diametro);
        trampa.ConfigurarDisparo(false);
        trampa.ConfigurarSpriteBase(spriteCirculo);
        return trampa;
    }

    // Púas que caen del techo en fila (la misma lluvia de rocas, con otro aspecto).
    public static LluviaDeRocas CrearPuasQueCaenDelTecho(string nombre, float inicioX, float finX, float alturaTecho, float alturaPiso, int cantidad, Transform padre)
    {
        LluviaDeRocas trampa = CrearLluviaDeRocas(nombre, inicioX, finX, alturaTecho, alturaPiso, cantidad, true, padre);
        trampa.ConfigurarAspecto(new Color(0.85f, 0.85f, 0.92f), 0.6f, "Las púas del techo cayeron sobre {0}. ¡Miren las sombras!");
        // Una púa cada 0,6 s: la "ola" avanza más lento que quien sigue caminando.
        trampa.ConfigurarRitmo(0.6f);
        return trampa;
    }

    // ----- Trampas al estilo de los juegos "troll" de plataformas -----

    // Bloque dorado con un "?": invita a golpearlo de cabeza, pero suelta un
    // bichito que camina hacia "direccion" (1 derecha, -1 izquierda).
    public static BloqueSorpresa CrearBloqueSorpresa(string nombre, Vector2 posicion, float direccion, Transform padre)
    {
        GameObject bloque = CrearBloque(nombre, posicion, new Vector2(0.9f, 0.9f), new Color(0.95f, 0.72f, 0.18f), padre);
        bloque.GetComponent<SpriteRenderer>().sortingOrder = 4;

        // Borde oscuro para que se lea como un bloque y no como una moneda.
        CrearDibujoHijo("Borde", bloque.transform, Vector2.zero, new Vector2(1.08f, 1.08f), new Color(0.45f, 0.28f, 0.05f), 3);

        Font fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GameObject textoObjeto = new GameObject("Signo");
        textoObjeto.transform.SetParent(bloque.transform);
        textoObjeto.transform.localPosition = new Vector3(0f, 0f, -0.1f);
        textoObjeto.transform.localScale = Vector3.one;
        TextMesh malla = textoObjeto.AddComponent<TextMesh>();
        malla.text = "?";
        malla.font = fuente;
        malla.characterSize = 0.14f;
        malla.fontSize = 48;
        malla.fontStyle = FontStyle.Bold;
        malla.anchor = TextAnchor.MiddleCenter;
        malla.color = new Color(0.4f, 0.2f, 0.02f);
        MeshRenderer renderizador = textoObjeto.GetComponent<MeshRenderer>();
        renderizador.sharedMaterial = fuente.material;
        renderizador.sortingOrder = 5;

        BloqueSorpresa trampa = bloque.AddComponent<BloqueSorpresa>();
        trampa.Configurar(direccion, 7f, spriteCirculo);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }

    // Criatura que vive en un charco o un hueco y salta cuando hay alguien cerca.
    // alturaSuperficie: el borde de arriba del charco.
    public static CriaturaQueSalta CrearCriaturaQueSalta(string nombre, float centroX, float alturaSuperficie, TipoZona tipoCharco, float alturaDelSalto, Transform padre)
    {
        GameObject criatura = new GameObject(nombre);
        criatura.transform.SetParent(padre);
        criatura.transform.position = new Vector3(centroX, alturaSuperficie, 0f);

        // El color dice de qué charco sale: rojo de lava, azul de agua, verde de veneno.
        Color colorCuerpo = new Color(0.45f, 0.2f, 0.55f);
        if (tipoCharco == TipoZona.Lava)
        {
            colorCuerpo = new Color(0.85f, 0.25f, 0.05f);
        }
        else if (tipoCharco == TipoZona.Agua)
        {
            colorCuerpo = new Color(0.1f, 0.3f, 0.75f);
        }
        else if (tipoCharco == TipoZona.Veneno)
        {
            colorCuerpo = new Color(0.25f, 0.6f, 0.15f);
        }

        GameObject cuerpo = CrearDibujoHijo("Criatura", criatura.transform, Vector2.zero, new Vector2(0.8f, 0.8f), colorCuerpo, 8);
        cuerpo.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
        GameObject ojoUno = CrearDibujoHijo("Ojo 1", cuerpo.transform, new Vector2(-0.15f, 0.12f), new Vector2(0.18f, 0.24f), Color.white, 9);
        GameObject ojoDos = CrearDibujoHijo("Ojo 2", cuerpo.transform, new Vector2(0.15f, 0.12f), new Vector2(0.18f, 0.24f), Color.white, 9);
        ojoUno.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
        ojoDos.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
        // Aleta de arriba: lo hace ver como un pez y no como una gema.
        CrearDibujoHijo("Aleta", cuerpo.transform, new Vector2(0f, 0.45f), new Vector2(0.18f, 0.25f), colorCuerpo * 0.8f, 7);

        GameObject burbujas = new GameObject("Burbujas");
        burbujas.transform.SetParent(criatura.transform);
        burbujas.transform.localPosition = Vector3.zero;
        for (int indice = 0; indice < 3; indice++)
        {
            float x = -0.35f + indice * 0.35f;
            float tamano = 0.18f + 0.06f * (indice % 2);
            GameObject burbuja = CrearDibujoHijo("Burbuja " + indice, burbujas.transform, new Vector2(x, 0.15f + 0.1f * (indice % 2)), new Vector2(tamano, tamano), new Color(1f, 1f, 1f, 0.85f), 9);
            burbuja.GetComponent<SpriteRenderer>().sprite = spriteCirculo;
        }

        CriaturaQueSalta trampa = criatura.AddComponent<CriaturaQueSalta>();
        trampa.Configurar(alturaDelSalto, 4.5f);
        trampa.ConfigurarSpriteBase(spriteCuadrado);
        return trampa;
    }
}
