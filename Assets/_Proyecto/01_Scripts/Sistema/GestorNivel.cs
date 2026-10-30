// =====================================================================
// GestorNivel.cs
// Proyecto: Inti y Killa
// Descripción: El "árbitro" del nivel. Cuenta las gemas, mide el tiempo,
//              reinicia el nivel cuando alguien muere y declara la
//              victoria cuando los dos están en su puerta.
//              Muestra un HUD temporal (OnGUI) mientras se diseña la
//              interfaz definitiva con uGUI.
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

    [SerializeField] private string nombreNivel = "Nivel 1 - Salar de Uyuni";
    [SerializeField] private float tiempoEsperaReinicio = 1.2f;

    private int gemasDeInti = 0;
    private int gemasDeKilla = 0;
    private float cronometro = 0f;
    private bool nivelTerminado = false;
    private bool reinicioEnCurso = false;
    private string mensajeCentral = "";

    private List<PuertaSalida> listaPuertas = new List<PuertaSalida>();
    private List<ControladorJugador> listaJugadores = new List<ControladorJugador>();

    private void Awake()
    {
        instancia = this;
    }

    private void Start()
    {
        // Busca todas las puertas y jugadores que hay en la escena.
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
    }

    private void Update()
    {
        if (nivelTerminado == false && reinicioEnCurso == false)
        {
            cronometro = cronometro + Time.deltaTime;
        }

        // Tecla R: reiniciar el nivel en cualquier momento.
        Keyboard teclado = Keyboard.current;
        if (teclado != null && teclado.rKey.wasPressedThisFrame == true)
        {
            ReiniciarNivel();
        }
    }

    public void RegistrarGema(TipoElemento elemento)
    {
        if (elemento == TipoElemento.Fuego)
        {
            gemasDeInti = gemasDeInti + 1;
        }
        else
        {
            gemasDeKilla = gemasDeKilla + 1;
        }
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

    public void RevisarVictoria()
    {
        if (nivelTerminado == true || reinicioEnCurso == true)
        {
            return;
        }

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

        nivelTerminado = true;
        mensajeCentral = "¡Nivel completado en " + cronometro.ToString("0.0") + " s!  (R para jugar de nuevo)";

        foreach (ControladorJugador jugador in listaJugadores)
        {
            jugador.DetenerMovimiento();
        }
    }

    public void ReiniciarNivel()
    {
        Scene escenaActual = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escenaActual.name);
    }

    // HUD temporal: luego se reemplaza por la interfaz definitiva.
    private void OnGUI()
    {
        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 22;
        estiloTexto.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 15, 600, 30), nombreNivel, estiloTexto);
        GUI.Label(new Rect(20, 45, 600, 30), "Gemas Inti: " + gemasDeInti + "    Gemas Killa: " + gemasDeKilla, estiloTexto);
        GUI.Label(new Rect(20, 75, 600, 30), "Tiempo: " + cronometro.ToString("0.0") + " s", estiloTexto);

        GUIStyle estiloAyuda = new GUIStyle(GUI.skin.label);
        estiloAyuda.fontSize = 16;
        estiloAyuda.normal.textColor = new Color(1f, 1f, 1f, 0.8f);
        string textoAyuda = "Inti: A / D mover, W saltar     Killa: Flechas mover, Arriba saltar     R: reiniciar";
        GUI.Label(new Rect(20, Screen.height - 35, 900, 30), textoAyuda, estiloAyuda);

        if (mensajeCentral != "")
        {
            GUIStyle estiloCentral = new GUIStyle(GUI.skin.label);
            estiloCentral.fontSize = 34;
            estiloCentral.fontStyle = FontStyle.Bold;
            estiloCentral.alignment = TextAnchor.MiddleCenter;
            estiloCentral.normal.textColor = new Color(1f, 0.85f, 0.3f);
            GUI.Label(new Rect(0, Screen.height / 2f - 40f, Screen.width, 80), mensajeCentral, estiloCentral);
        }
    }
}
