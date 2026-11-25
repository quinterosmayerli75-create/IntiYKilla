// =====================================================================
// GestorNivel.cs
// Proyecto: Inti y Killa
// Descripción: El "árbitro" del nivel:
//              - Cuenta las gemas y mide el tiempo.
//              - Reinicia el nivel cuando alguien muere.
//              - Declara la victoria cuando los dos están en su puerta,
//                calcula las estrellas y guarda el progreso.
//              - Teclas: R reiniciar, H ver la pista (Esc pausa, en MenuPausa),
//                Enter pasar al siguiente nivel (después de ganar).
//              Muestra un HUD temporal (OnGUI) mientras se diseña la
//              interfaz definitiva.
// Uso: un solo objeto "GestorNivel" por escena.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GestorNivel : MonoBehaviour
{
    // Instancia única para que cualquier script lo encuentre fácilmente.
    private static GestorNivel instancia;

    public static GestorNivel Instancia
    {
        get { return instancia; }
    }

    // Reinicio casi instantáneo: morir es parte del juego y no debe aburrir.
    [SerializeField] private float tiempoEsperaReinicio = 0.7f;
    [SerializeField] private float duracionMensajeDeMuerte = 2.5f;

    [Header("Pista que aparece al empezar el nivel")]
    [TextArea]
    [SerializeField] private string pistaInicial = "";
    [SerializeField] private float duracionPista = 8f;

    // Datos del nivel actual (se leen de DatosJuego según el nombre de la escena).
    // Queda en null si la escena no está en el catálogo (por ejemplo, una de prueba).
    private DatosNivel datosNivel = null;
    private string tituloNivel = "Nivel de prueba";
    private int muertesPermitidas = 5;

    // Son estáticos porque la escena se recarga al morir y borraría lo demás:
    // las muertes y el último mensaje deben sobrevivir a ese reinicio.
    private static int muertesDelIntento = 0;

    // Tres vidas por nivel: al perder la tercera, el nivel empieza de cero
    // (con 3 vidas otra vez y, si es una sala que se repite, desde la vuelta 1).
    private const int vidasPorNivel = 3;
    private static int vidasRestantes = vidasPorNivel;

    // En el nivel final, al perder las 3 vidas se vuelve al comienzo del mundo.
    private bool volverAlDestinoAlReiniciar = false;
    // Al perder las 3 vidas en los niveles 2 a 5 de un mundo, se vuelve al nivel 1 de ese mundo.
    private bool volverAlNivelUnoAlReiniciar = false;
    private static string escenaDelContador = "";
    private static string mensajeDeLaUltimaMuerte = "";

    // Gemas: totales del nivel y recogidas.
    private int totalGemasFuego = 0;
    private int totalGemasAgua = 0;
    private int gemasFuegoRecogidas = 0;
    private int gemasAguaRecogidas = 0;

    private float cronometro = 0f;
    private bool nivelTerminado = false;
    private bool nivelPerfecto = false;
    private List<string> logrosNuevos = new List<string>();
    private bool reinicioEnCurso = false;
    private int estrellasGanadas = 0;

    // True si se acaba de ganar el último nivel del mundo.
    private bool mundoCompletado = false;
    [SerializeField] private float segundosMensajeMundoCompletado = 4f;

    private string mensajeCentral = "";
    private string avisoTemporal = "";
    private float tiempoRestanteAviso = 0f;

    private List<PuertaSalida> listaPuertas = new List<PuertaSalida>();
    private List<ControladorJugador> listaJugadores = new List<ControladorJugador>();

    private void Awake()
    {
        instancia = this;
    }

    public void ConfigurarPista(string nuevaPista)
    {
        pistaInicial = nuevaPista;
    }

    private void Start()
    {
        CargarDatosDelNivel();
        BuscarObjetosDeLaEscena();

        // Si se entra al nivel desde afuera, el contador empieza de cero.
        string nombreEscena = SceneManager.GetActiveScene().name;
        if (nombreEscena != escenaDelContador)
        {
            escenaDelContador = nombreEscena;
            muertesDelIntento = 0;
            vidasRestantes = vidasPorNivel;
            mensajeDeLaUltimaMuerte = "";
        }

        // El reinicio es tan rápido que el mensaje se muestra ya en el nuevo intento.
        if (mensajeDeLaUltimaMuerte != "")
        {
            MostrarAviso(mensajeDeLaUltimaMuerte, duracionMensajeDeMuerte);
            mensajeDeLaUltimaMuerte = "";
        }
        else if (pistaInicial != "" && muertesDelIntento == 0)
        {
            MostrarAviso(pistaInicial, duracionPista);
        }
    }

    private void CargarDatosDelNivel()
    {
        string nombreEscena = SceneManager.GetActiveScene().name;
        datosNivel = DatosJuego.ObtenerNivelPorEscena(nombreEscena);

        if (datosNivel != null)
        {
            string nombreMundo = DatosJuego.ObtenerNombreMundo(datosNivel.numeroMundo);
            tituloNivel = "Mundo " + datosNivel.numeroMundo + " - Nivel " + datosNivel.numeroEnMundo + " (" + nombreMundo + "): " + datosNivel.titulo;
            muertesPermitidas = datosNivel.muertesPermitidas;

            // Así, al volver al mapa se muestran los niveles de este mundo.
            GestorProgreso.MundoElegido = datosNivel.numeroMundo;
        }
    }

    private void BuscarObjetosDeLaEscena()
    {
        PuertaSalida[] puertasEncontradas = FindObjectsByType<PuertaSalida>(FindObjectsSortMode.None);
        foreach (PuertaSalida puerta in puertasEncontradas)
        {
            listaPuertas.Add(puerta);
        }

        ControladorJugador[] jugadoresEncontrados = FindObjectsByType<ControladorJugador>(FindObjectsSortMode.None);
        foreach (ControladorJugador jugador in jugadoresEncontrados)
        {
            listaJugadores.Add(jugador);
        }

        // Contamos las gemas al inicio para saber si se recogieron todas.
        Gema[] gemasEncontradas = FindObjectsByType<Gema>(FindObjectsSortMode.None);
        foreach (Gema gema in gemasEncontradas)
        {
            if (gema.Elemento == TipoElemento.Fuego)
            {
                totalGemasFuego = totalGemasFuego + 1;
            }
            else
            {
                totalGemasAgua = totalGemasAgua + 1;
            }
        }
    }

    private void Update()
    {
        if (nivelTerminado == false && reinicioEnCurso == false)
        {
            cronometro = cronometro + Time.deltaTime;
            RevisarVictoria();
        }

        if (tiempoRestanteAviso > 0f)
        {
            tiempoRestanteAviso = tiempoRestanteAviso - Time.deltaTime;
        }

        LeerTeclasDelNivel();
    }

    private void LeerTeclasDelNivel()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            return;
        }

        // La pista solo sale al empezar: con H se puede volver a leer.
        if (teclado.hKey.wasPressedThisFrame == true && pistaInicial != "")
        {
            MostrarAviso(pistaInicial, duracionPista);
        }

        if (MenuPausa.EstaPausado == true)
        {
            return;
        }

        if (teclado.rKey.wasPressedThisFrame == true)
        {
            ReiniciarNivel();
        }

        if (teclado.escapeKey.wasPressedThisFrame == true && nivelTerminado == true)
        {
            VolverAlMapa();
        }

        bool presionoEnter = teclado.enterKey.wasPressedThisFrame || teclado.numpadEnterKey.wasPressedThisFrame;
        if (nivelTerminado == true && presionoEnter == true)
        {
            IrAlSiguienteNivel();
        }
    }

    public void RegistrarGema(TipoElemento elemento)
    {
        if (elemento == TipoElemento.Fuego)
        {
            gemasFuegoRecogidas = gemasFuegoRecogidas + 1;
        }
        else
        {
            gemasAguaRecogidas = gemasAguaRecogidas + 1;
        }
    }

    // Muestra un mensaje corto en pantalla (por ejemplo, al activar un eclipse).
    public void MostrarAviso(string texto, float segundos)
    {
        avisoTemporal = texto;
        tiempoRestanteAviso = segundos;
    }

    public void JugadorMurio(string mensajeDeMuerte)
    {
        if (nivelTerminado == true || reinicioEnCurso == true)
        {
            return;
        }

        // Regla del GDD: si uno muere, el nivel se reinicia para los dos.
        reinicioEnCurso = true;
        muertesDelIntento = muertesDelIntento + 1;
        mensajeCentral = mensajeDeMuerte;
        mensajeDeLaUltimaMuerte = mensajeDeMuerte;
        vidasRestantes = vidasRestantes - 1;

        // La pantalla se oscurece mientras se espera: así se nota el reinicio.
        // Nivel final: solo al perder las 3 vidas se vuelve al comienzo del mundo.
        if (VolverAlPerder.Instancia != null && vidasRestantes <= 0)
        {
            vidasRestantes = vidasPorNivel;
            volverAlDestinoAlReiniciar = true;
            TransicionReinicio.Oscurecer(tiempoEsperaReinicio, VolverAlPerder.Instancia.Mensaje);
        }
        else if (vidasRestantes <= 0 && datosNivel != null && datosNivel.numeroEnMundo > 1)
        {
            // Sin vidas: se vuelve a empezar el mundo desde su nivel 1.
            vidasRestantes = vidasPorNivel;
            SalaQueSeRepite.VolverALaPrimeraVuelta();
            volverAlNivelUnoAlReiniciar = true;
            TransicionReinicio.Oscurecer(tiempoEsperaReinicio, "¡Sin vidas! Vuelven al nivel 1 del mundo");
        }
        else if (vidasRestantes <= 0)
        {
            // Sin vidas en el nivel 1: el nivel empieza de cero, también las vueltas de la sala.
            vidasRestantes = vidasPorNivel;
            SalaQueSeRepite.VolverALaPrimeraVuelta();
            TransicionReinicio.Oscurecer(tiempoEsperaReinicio, "¡Sin vidas! El nivel empieza de cero");
        }
        else
        {
            TransicionReinicio.Oscurecer(tiempoEsperaReinicio, "Te quedan " + vidasRestantes + " vidas");
        }
        Invoke(nameof(ReiniciarNivel), tiempoEsperaReinicio);
    }

    // Se gana cuando TODAS las puertas tienen a su personaje adentro.
    private void RevisarVictoria()
    {
        if (listaPuertas.Count == 0)
        {
            return;
        }

        foreach (PuertaSalida puerta in listaPuertas)
        {
            if (puerta.EstaOcupada == false)
            {
                return; // Todavía falta alguien.
            }
        }

        // Sala que se repite: si quedan vueltas, la sala vuelve a empezar
        // con una regla cambiada en vez de terminar el nivel.
        if (SalaQueSeRepite.Instancia != null && SalaQueSeRepite.Instancia.AtrapaLaVictoria() == true)
        {
            return;
        }

        TerminarNivel();
    }

    private void TerminarNivel()
    {
        nivelTerminado = true;
        estrellasGanadas = CalcularEstrellas();

        // Si repiten el nivel después de ganarlo, las muertes empiezan de cero.
        escenaDelContador = "";

        if (datosNivel != null)
        {
            GestorProgreso.GuardarResultado(datosNivel.numeroMundo, datosNivel.numeroEnMundo, estrellasGanadas);
            GestorProgreso.GuardarRecordMuertes(datosNivel.numeroMundo, datosNivel.numeroEnMundo, muertesDelIntento);
            // Logros: el nivel cuenta como perfecto si no se perdió ninguna vida.
            nivelPerfecto = vidasRestantes == vidasPorNivel;
            logrosNuevos = GestorLogros.RegistrarVictoria(datosNivel.numeroMundo, datosNivel.numeroEnMundo, nivelPerfecto);
            mundoCompletado = datosNivel.numeroEnMundo == DatosJuego.nivelesPorMundo;
        }

        foreach (ControladorJugador jugador in listaJugadores)
        {
            jugador.DetenerMovimiento();
        }

        if (mundoCompletado == true)
        {
            // Se deja ver el mensaje y las estrellas antes de volver a los mundos.
            mensajeCentral = "¡Mundo completado!";
            if (EsElUltimoNivelDelJuego() == true)
            {
                mensajeCentral = "¡Vencieron al eclipse!";
            }
            Invoke(nameof(VolverAMundos), segundosMensajeMundoCompletado);
        }
        else
        {
            mensajeCentral = "¡Nivel completado!";
        }
    }

    // 1 estrella por terminar, +1 por todas las gemas, +1 por morir pocas veces.
    private int CalcularEstrellas()
    {
        int estrellas = 1;

        bool recogioTodasLasGemas = gemasFuegoRecogidas >= totalGemasFuego && gemasAguaRecogidas >= totalGemasAgua;
        if (recogioTodasLasGemas == true)
        {
            estrellas = estrellas + 1;
        }

        bool murioPocasVeces = muertesDelIntento <= muertesPermitidas;
        if (murioPocasVeces == true)
        {
            estrellas = estrellas + 1;
        }

        return estrellas;
    }

    public bool NivelTerminado
    {
        get { return nivelTerminado; }
    }

    public void ReiniciarNivel()
    {
        // Nivel final: al perder las 3 vidas no se repite, se vuelve al 6-1.
        if (VolverAlPerder.Instancia != null && volverAlDestinoAlReiniciar == true)
        {
            volverAlDestinoAlReiniciar = false;
            SceneManager.LoadScene(VolverAlPerder.Instancia.EscenaDestino);
            return;
        }

        if (volverAlNivelUnoAlReiniciar == true && datosNivel != null)
        {
            volverAlNivelUnoAlReiniciar = false;
            string escenaNivelUno = "Mundo" + datosNivel.numeroMundo + "_Nivel1";
            if (Application.CanStreamedLevelBeLoaded(escenaNivelUno) == true)
            {
                SceneManager.LoadScene(escenaNivelUno);
                return;
            }
        }

        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    public void VolverAlMapa()
    {
        escenaDelContador = "";

        if (Application.CanStreamedLevelBeLoaded(DatosJuego.escenaSeleccionNivel) == true)
        {
            SceneManager.LoadScene(DatosJuego.escenaSeleccionNivel);
        }
    }

    private bool EsElUltimoNivelDelJuego()
    {
        return datosNivel != null && datosNivel.numeroMundo == DatosJuego.CantidadDeMundos() && datosNivel.numeroEnMundo == DatosJuego.nivelesPorMundo;
    }

    public void VolverAMundos()
    {
        escenaDelContador = "";
        // Después del último nivel del juego viene la escena final.
        if (EsElUltimoNivelDelJuego() == true && Application.CanStreamedLevelBeLoaded(DatosJuego.escenaFinal) == true)
        {
            SceneManager.LoadScene(DatosJuego.escenaFinal);
            return;
        }
        SceneManager.LoadScene(DatosJuego.escenaSeleccionMundo);
    }

    public void IrAlSiguienteNivel()
    {
        if (mundoCompletado == true)
        {
            VolverAMundos();
            return;
        }

        DatosNivel siguiente = null;
        if (datosNivel != null)
        {
            siguiente = DatosJuego.ObtenerNivel(datosNivel.numeroMundo, datosNivel.numeroEnMundo + 1);
        }

        if (siguiente != null && Application.CanStreamedLevelBeLoaded(siguiente.nombreEscena) == true)
        {
            SceneManager.LoadScene(siguiente.nombreEscena);
        }
        else
        {
            // Si el siguiente nivel todavía no existe, volvemos al mapa.
            VolverAlMapa();
        }
    }

    private string TextoEstrellas(int cantidad)
    {
        string texto = "";
        for (int indice = 1; indice <= 3; indice++)
        {
            if (indice <= cantidad)
            {
                texto = texto + "★";
            }
            else
            {
                texto = texto + "☆";
            }
        }
        return texto;
    }

    // Muertes de este intento, las permitidas para la estrella y el récord guardado.
    private string ObtenerTextoMuertes()
    {
        string texto = "Vidas: " + vidasRestantes + " de " + vidasPorNivel + "     Muertes: " + muertesDelIntento + "   (estrella con " + muertesPermitidas + " o menos)";

        if (datosNivel != null)
        {
            int record = GestorProgreso.ObtenerRecordMuertes(datosNivel.numeroMundo, datosNivel.numeroEnMundo);
            if (record >= 0)
            {
                texto = texto + "   Récord: " + record;
            }
        }

        return texto;
    }

    // Resolución para la que se diseñó el HUD. En otras pantallas todo se
    // escala en proporción, así el texto no se corta ni queda diminuto
    // (importante en WebGL, donde cada jugador tiene un tamaño de ventana distinto).
    private const float altoReferenciaHud = 1080f;
    private const float margenHud = 30f;

    // HUD temporal: luego se reemplaza por la interfaz definitiva.
    // Pistas y avisos: el texto puede ser largo, así que baja a varias líneas y
    // lleva un panel oscuro para que se lea sobre cualquier parte del nivel.
    private void DibujarAviso(float anchoVirtual, float altoVirtual)
    {
        GUIStyle estiloAviso = new GUIStyle(GUI.skin.label);
        estiloAviso.fontSize = 36;
        estiloAviso.fontStyle = FontStyle.Bold;
        estiloAviso.alignment = TextAnchor.MiddleCenter;
        estiloAviso.normal.textColor = new Color(1f, 0.88f, 0.4f);
        estiloAviso.wordWrap = true;

        float anchoAviso = Mathf.Min(anchoVirtual - 120f, 1200f);
        float altoTexto = estiloAviso.CalcHeight(new GUIContent(avisoTemporal), anchoAviso);
        float relleno = 16f;
        float posicionX = (anchoVirtual - anchoAviso) / 2f;
        float posicionY = altoVirtual * 0.22f;

        Color colorAnterior = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.85f);
        GUI.DrawTexture(new Rect(posicionX - relleno, posicionY - relleno, anchoAviso + relleno * 2f, altoTexto + relleno * 2f), Texture2D.whiteTexture);
        GUI.color = colorAnterior;
        GUI.Label(new Rect(posicionX, posicionY, anchoAviso, altoTexto), avisoTemporal, estiloAviso);
    }

    // Ventana de victoria: estrellas, gemas, muertes y tiempo, y qué teclas usar.
    private void DibujarVentanaVictoria(float anchoVirtual, float altoVirtual, GUIStyle estiloCentral, GUIStyle estiloAyuda)
    {
        float anchoVentana = 900f;
        // La ventana crece si hay que mostrar el premio de las vidas o logros nuevos.
        float altoExtra = 0f;
        if (nivelPerfecto == true)
        {
            altoExtra = 50f + logrosNuevos.Count * 45f;
        }
        float altoVentana = 330f + altoExtra;
        Rect ventana = new Rect((anchoVirtual - anchoVentana) / 2f, altoVirtual / 2f - 120f, anchoVentana, altoVentana);
        Color colorAnterior = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.8f);
        GUI.DrawTexture(ventana, Texture2D.whiteTexture);
        GUI.color = colorAnterior;

        GUI.Label(new Rect(ventana.x, ventana.y + 10f, anchoVentana, 80f), mensajeCentral, estiloCentral);

        GUIStyle estiloEstrellas = new GUIStyle(estiloCentral);
        estiloEstrellas.fontSize = 80;
        GUI.Label(new Rect(ventana.x, ventana.y + 85f, anchoVentana, 100f), TextoEstrellas(estrellasGanadas), estiloEstrellas);

        GUIStyle estiloDatos = new GUIStyle(estiloAyuda);
        estiloDatos.fontSize = 30;
        int gemasRecogidas = gemasFuegoRecogidas + gemasAguaRecogidas;
        int gemasTotales = totalGemasFuego + totalGemasAgua;
        string textoDatos = "Gemas: " + gemasRecogidas + "/" + gemasTotales + "     Muertes: " + muertesDelIntento + "     Tiempo: " + cronometro.ToString("0") + " s";
        GUI.Label(new Rect(ventana.x, ventana.y + 195f, anchoVentana, 50f), textoDatos, estiloDatos);

        string textoOpciones = "Enter: siguiente nivel     R: repetir     Esc: mapa";
        if (mundoCompletado == true)
        {
            textoOpciones = "Enter: continuar";
        }
        if (nivelPerfecto == true)
        {
            GUIStyle estiloLogro = new GUIStyle(estiloDatos);
            estiloLogro.normal.textColor = new Color(1f, 0.85f, 0.3f);
            estiloLogro.fontStyle = FontStyle.Bold;
            GUI.Label(new Rect(ventana.x, ventana.y + 245f, anchoVentana, 45f), "¡Nivel perfecto: las 3 vidas intactas!", estiloLogro);
            for (int indice = 0; indice < logrosNuevos.Count; indice++)
            {
                GUI.Label(new Rect(ventana.x, ventana.y + 290f + indice * 45f, anchoVentana, 45f), "Logro desbloqueado: " + logrosNuevos[indice], estiloLogro);
            }
        }
        GUI.Label(new Rect(ventana.x, ventana.y + 255f + altoExtra, anchoVentana, 50f), textoOpciones, estiloDatos);
    }

    private void OnGUI()
    {
        // Todo lo que se dibuja abajo usa coordenadas "virtuales" de 1080 de alto;
        // la matriz de GUI las convierte al tamaño real de la pantalla.
        float escala = Screen.height / altoReferenciaHud;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(escala, escala, 1f));
        float anchoVirtual = Screen.width / escala;
        float altoVirtual = altoReferenciaHud;
        float anchoTexto = anchoVirtual - margenHud * 2f;

        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 32;
        estiloTexto.normal.textColor = Color.white;
        estiloTexto.wordWrap = false;

        GUI.Label(new Rect(margenHud, margenHud, anchoTexto, 44), tituloNivel, estiloTexto);
        string textoGemas = "Gemas de fuego: " + gemasFuegoRecogidas + "/" + totalGemasFuego + "     Gemas de agua: " + gemasAguaRecogidas + "/" + totalGemasAgua;
        GUI.Label(new Rect(margenHud, margenHud + 44, anchoTexto, 44), textoGemas, estiloTexto);
        GUI.Label(new Rect(margenHud, margenHud + 88, anchoTexto, 44), ObtenerTextoMuertes(), estiloTexto);

        // La ayuda de controles va centrada abajo, con margen para que no toque el borde.
        GUIStyle estiloAyuda = new GUIStyle(GUI.skin.label);
        estiloAyuda.fontSize = 24;
        estiloAyuda.alignment = TextAnchor.MiddleCenter;
        estiloAyuda.normal.textColor = Color.white;
        estiloAyuda.wordWrap = false;
        string textoAyuda = "Inti: A/D mover, W saltar, S usar     Killa: Flechas mover, Arriba saltar, Abajo usar     H: ver pista     R: reiniciar     Esc: pausa";

        // Franja oscura detrás del texto: sin ella, el blanco se pierde sobre el suelo claro.
        float altoFranja = 44f;
        Rect franja = new Rect(0f, altoVirtual - altoFranja, anchoVirtual, altoFranja);
        Color colorAnterior = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.88f);
        GUI.DrawTexture(franja, Texture2D.whiteTexture);
        GUI.color = colorAnterior;
        GUI.Label(franja, textoAyuda, estiloAyuda);

        GUIStyle estiloCentral = new GUIStyle(GUI.skin.label);
        estiloCentral.fontSize = 48;
        estiloCentral.fontStyle = FontStyle.Bold;
        estiloCentral.alignment = TextAnchor.MiddleCenter;
        estiloCentral.normal.textColor = new Color(1f, 0.85f, 0.3f);

        if (tiempoRestanteAviso > 0f && avisoTemporal != "")
        {
            DibujarAviso(anchoVirtual, altoVirtual);
        }

        // Al ganar, el mensaje va dentro de la ventana de victoria.
        if (mensajeCentral != "" && nivelTerminado == false)
        {
            GUI.Label(new Rect(0, altoVirtual / 2f - 80f, anchoVirtual, 80), mensajeCentral, estiloCentral);
        }

        if (nivelTerminado == true)
        {
            DibujarVentanaVictoria(anchoVirtual, altoVirtual, estiloCentral, estiloAyuda);
        }
    }
}
