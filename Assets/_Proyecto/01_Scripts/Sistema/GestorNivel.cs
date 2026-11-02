// =====================================================================
// GestorNivel.cs
// Proyecto: Inti y Killa
// Descripción: El "árbitro" del nivel:
//              - Cuenta las gemas y mide el tiempo.
//              - Reinicia el nivel cuando alguien muere.
//              - Declara la victoria cuando los dos están en su puerta,
//                calcula las estrellas y guarda el progreso.
//              - Teclas: R reiniciar, Esc volver al mapa,
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

    [SerializeField] private float tiempoEsperaReinicio = 1.2f;

    [Header("Pista que aparece al empezar el nivel")]
    [TextArea]
    [SerializeField] private string pistaInicial = "";
    [SerializeField] private float duracionPista = 6f;

    // Datos del nivel actual (se leen de DatosJuego según el nombre de la escena).
    private int numeroNivel = 0;
    private string tituloNivel = "Nivel de prueba";
    private float tiempoObjetivo = 60f;

    // Gemas: totales del nivel y recogidas.
    private int totalGemasFuego = 0;
    private int totalGemasAgua = 0;
    private int gemasFuegoRecogidas = 0;
    private int gemasAguaRecogidas = 0;

    private float cronometro = 0f;
    private bool nivelTerminado = false;
    private bool reinicioEnCurso = false;
    private int estrellasGanadas = 0;

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

        if (pistaInicial != "")
        {
            MostrarAviso(pistaInicial, duracionPista);
        }
    }

    private void CargarDatosDelNivel()
    {
        string nombreEscena = SceneManager.GetActiveScene().name;
        numeroNivel = DatosJuego.ObtenerNumeroPorEscena(nombreEscena);

        DatosNivel datos = DatosJuego.ObtenerNivel(numeroNivel);
        if (datos != null)
        {
            string nombreMundo = DatosJuego.ObtenerNombreMundo(datos.numeroMundo);
            tituloNivel = "Nivel " + datos.numero + " - " + nombreMundo + ": " + datos.titulo;
            tiempoObjetivo = datos.tiempoObjetivo;
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

        if (teclado.rKey.wasPressedThisFrame == true)
        {
            ReiniciarNivel();
        }

        if (teclado.escapeKey.wasPressedThisFrame == true)
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

    public void JugadorMurio(ControladorJugador jugador, string motivo)
    {
        if (nivelTerminado == true || reinicioEnCurso == true)
        {
            return;
        }

        // Regla del GDD: si uno muere, el nivel se reinicia para los dos.
        reinicioEnCurso = true;
        mensajeCentral = jugador.NombreJugador + " " + motivo + "... ¡otra vez!";
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

        TerminarNivel();
    }

    private void TerminarNivel()
    {
        nivelTerminado = true;
        estrellasGanadas = CalcularEstrellas();
        GestorProgreso.GuardarResultado(numeroNivel, estrellasGanadas);

        foreach (ControladorJugador jugador in listaJugadores)
        {
            jugador.DetenerMovimiento();
        }

        mensajeCentral = "¡Nivel completado!";
    }

    // 1 estrella por terminar, +1 por todas las gemas, +1 por llegar a tiempo.
    private int CalcularEstrellas()
    {
        int estrellas = 1;

        bool recogioTodasLasGemas = gemasFuegoRecogidas >= totalGemasFuego && gemasAguaRecogidas >= totalGemasAgua;
        if (recogioTodasLasGemas == true)
        {
            estrellas = estrellas + 1;
        }

        bool llegoATiempo = cronometro <= tiempoObjetivo;
        if (llegoATiempo == true)
        {
            estrellas = estrellas + 1;
        }

        return estrellas;
    }

    public void ReiniciarNivel()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    public void VolverAlMapa()
    {
        if (Application.CanStreamedLevelBeLoaded(DatosJuego.escenaSeleccionNivel) == true)
        {
            SceneManager.LoadScene(DatosJuego.escenaSeleccionNivel);
        }
    }

    public void IrAlSiguienteNivel()
    {
        DatosNivel siguiente = DatosJuego.ObtenerNivel(numeroNivel + 1);

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

    // HUD temporal: luego se reemplaza por la interfaz definitiva.
    private void OnGUI()
    {
        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 22;
        estiloTexto.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 15, 1000, 30), tituloNivel, estiloTexto);
        string textoGemas = "Gemas de fuego: " + gemasFuegoRecogidas + "/" + totalGemasFuego + "     Gemas de agua: " + gemasAguaRecogidas + "/" + totalGemasAgua;
        GUI.Label(new Rect(20, 45, 1000, 30), textoGemas, estiloTexto);
        GUI.Label(new Rect(20, 75, 1000, 30), "Tiempo: " + cronometro.ToString("0.0") + " s   (objetivo: " + tiempoObjetivo.ToString("0") + " s)", estiloTexto);

        GUIStyle estiloAyuda = new GUIStyle(GUI.skin.label);
        estiloAyuda.fontSize = 16;
        estiloAyuda.normal.textColor = new Color(1f, 1f, 1f, 0.85f);
        string textoAyuda = "Inti: A/D mover, W saltar, S usar     Killa: Flechas mover, Arriba saltar, Abajo usar     R: reiniciar     Esc: mapa";
        GUI.Label(new Rect(20, Screen.height - 35, 1400, 30), textoAyuda, estiloAyuda);

        GUIStyle estiloCentral = new GUIStyle(GUI.skin.label);
        estiloCentral.fontSize = 34;
        estiloCentral.fontStyle = FontStyle.Bold;
        estiloCentral.alignment = TextAnchor.MiddleCenter;
        estiloCentral.normal.textColor = new Color(1f, 0.85f, 0.3f);

        if (tiempoRestanteAviso > 0f && avisoTemporal != "")
        {
            GUI.Label(new Rect(0, Screen.height * 0.25f, Screen.width, 60), avisoTemporal, estiloCentral);
        }

        if (mensajeCentral != "")
        {
            GUI.Label(new Rect(0, Screen.height / 2f - 60f, Screen.width, 60), mensajeCentral, estiloCentral);
        }

        if (nivelTerminado == true)
        {
            GUIStyle estiloEstrellas = new GUIStyle(estiloCentral);
            estiloEstrellas.fontSize = 56;
            GUI.Label(new Rect(0, Screen.height / 2f, Screen.width, 70), TextoEstrellas(estrellasGanadas), estiloEstrellas);

            GUIStyle estiloOpciones = new GUIStyle(estiloAyuda);
            estiloOpciones.fontSize = 22;
            estiloOpciones.alignment = TextAnchor.MiddleCenter;
            string textoOpciones = "Tiempo: " + cronometro.ToString("0.0") + " s     Enter: siguiente nivel     R: repetir     Esc: mapa";
            GUI.Label(new Rect(0, Screen.height / 2f + 80f, Screen.width, 40), textoOpciones, estiloOpciones);
        }
    }
}
