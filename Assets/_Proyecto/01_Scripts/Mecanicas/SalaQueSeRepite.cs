// =====================================================================
// SalaQueSeRepite.cs
// Proyecto: Inti y Killa
// Descripción: El "bucle" psicológico. Cuando los dos llegan a sus puertas,
//              el nivel NO termina: la pantalla parpadea, aparece un mensaje
//              ("¿Otra vez?") y la misma sala vuelve a empezar, pero con una
//              regla cambiada (las piezas con VariantePorVuelta cambian).
//              Recién al completar todas las vueltas se gana el nivel.
//              El número de vuelta se guarda aunque alguien muera: morir no
//              hace perder las vueltas ya ganadas (regla de justicia).
// Uso: un solo objeto en la escena, creado por el generador.
// =====================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SalaQueSeRepite : MonoBehaviour
{
    // Total de vueltas: con 3, se juega la sala tres veces.
    [SerializeField] private int vueltasTotales = 3;
    [SerializeField] private float segundosAntesDeRepetir = 1.6f;
    // Lo que se lee al empezar cada vuelta (la primera usa la pista del nivel).
    [SerializeField] private string[] mensajesDeCadaVuelta = new string[] { "", "¿Otra vez? Parece la misma sala...", "¿Y otra vez? Ya no confíes en nada." };

    private static SalaQueSeRepite instancia;

    // La vuelta vive entre recargas de la misma escena; al entrar a otra
    // escena se vuelve a empezar desde la primera.
    private static int vueltaGuardada = 0;
    private static string escenaDeLaVuelta = "";

    private bool cambiandoDeVuelta = false;
    private float tiempoDelCambio = 0f;

    public static SalaQueSeRepite Instancia
    {
        get { return instancia; }
    }

    // Vuelta en la que se está jugando (0 = la primera).
    public static int VueltaActual
    {
        get
        {
            if (escenaDeLaVuelta != SceneManager.GetActiveScene().name)
            {
                return 0;
            }
            return vueltaGuardada;
        }
    }

    // Al quedarse sin vidas, la sala vuelve a empezar desde la primera vuelta.
    public static void VolverALaPrimeraVuelta()
    {
        vueltaGuardada = 0;
    }

    public int VueltasTotales
    {
        get { return vueltasTotales; }
    }

    public void Configurar(int nuevasVueltas, string[] nuevosMensajes)
    {
        vueltasTotales = nuevasVueltas;
        mensajesDeCadaVuelta = nuevosMensajes;
    }

    private void Awake()
    {
        instancia = this;

        // Si se entra al nivel desde el mapa, empieza en la primera vuelta.
        string nombreEscena = SceneManager.GetActiveScene().name;
        if (escenaDeLaVuelta != nombreEscena)
        {
            escenaDeLaVuelta = nombreEscena;
            vueltaGuardada = 0;
        }
    }

    private System.Collections.IEnumerator Start()
    {
        // Se espera un cuadro: así este mensaje va después de la pista del nivel.
        yield return null;

        // Al empezar una vuelta que no es la primera se muestra su mensaje.
        int vuelta = VueltaActual;
        if (vuelta > 0 && vuelta < mensajesDeCadaVuelta.Length && GestorNivel.Instancia != null)
        {
            string texto = "Vuelta " + (vuelta + 1) + " de " + vueltasTotales + ": " + mensajesDeCadaVuelta[vuelta];
            GestorNivel.Instancia.MostrarAviso(texto, 3.5f);
        }
    }

    private void OnDestroy()
    {
        if (instancia == this)
        {
            instancia = null;
        }
    }

    // El GestorNivel pregunta esto cuando los dos están en sus puertas.
    // Devuelve true si todavía quedan vueltas: entonces la victoria se
    // "atrapa" y la sala se repite en vez de terminar el nivel.
    public bool AtrapaLaVictoria()
    {
        if (cambiandoDeVuelta == true)
        {
            return true;
        }

        if (VueltaActual + 1 >= vueltasTotales)
        {
            // Última vuelta: la próxima vez que se entre, empieza de cero.
            escenaDeLaVuelta = "";
            return false;
        }

        cambiandoDeVuelta = true;
        tiempoDelCambio = 0f;
        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.MostrarAviso("...", segundosAntesDeRepetir);
        }
        return true;
    }

    private void Update()
    {
        if (cambiandoDeVuelta == false)
        {
            return;
        }

        tiempoDelCambio = tiempoDelCambio + Time.deltaTime;
        if (tiempoDelCambio >= segundosAntesDeRepetir)
        {
            cambiandoDeVuelta = false;
            vueltaGuardada = vueltaGuardada + 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // Mientras cambia de vuelta, la pantalla parpadea en blanco: así se nota
    // que "algo pasó" aunque la sala se vea igual.
    private void OnGUI()
    {
        if (cambiandoDeVuelta == false)
        {
            return;
        }

        float intensidad = Mathf.PingPong(tiempoDelCambio * 2.5f, 1f) * 0.6f;
        Color colorAnterior = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, intensidad);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = colorAnterior;
    }
}
