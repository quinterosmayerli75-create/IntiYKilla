// =====================================================================
// PantallaHistoria.cs
// Proyecto: Inti y Killa
// Descripción: Cuenta la historia en páginas de texto que aparecen de a poco.
//              Sirve para la Intro (antes del menú) y para el Final (después de
//              vencer al Guardián). En el texto, {estrellas} se reemplaza por el
//              total de estrellas ganadas.
//              - Enter, Espacio o clic: página siguiente.
//              - Esc: saltar toda la historia.
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PantallaHistoria : MonoBehaviour
{
    [SerializeField] private Text textoPagina;
    [SerializeField] private Text textoAyuda;
    [TextArea]
    [SerializeField] private string[] paginas;
    [SerializeField] private string escenaSiguiente = "MenuPrincipal";
    [SerializeField] private float segundosDeAparicion = 0.8f;

    private int paginaActual = 0;
    private float tiempoEnPagina = 0f;

    public void Configurar(Text nuevoTextoPagina, Text nuevoTextoAyuda, string[] nuevasPaginas, string nuevaEscenaSiguiente)
    {
        textoPagina = nuevoTextoPagina;
        textoAyuda = nuevoTextoAyuda;
        paginas = nuevasPaginas;
        escenaSiguiente = nuevaEscenaSiguiente;
    }

    private void Start()
    {
        MostrarPagina(0);
    }

    private void Update()
    {
        tiempoEnPagina = tiempoEnPagina + Time.unscaledDeltaTime;
        float transparencia = Mathf.Clamp01(tiempoEnPagina / segundosDeAparicion);
        Color color = textoPagina.color;
        textoPagina.color = new Color(color.r, color.g, color.b, transparencia);

        Keyboard teclado = Keyboard.current;
        Mouse raton = Mouse.current;
        bool quiereAvanzar = false;
        bool quiereSaltar = false;

        if (teclado != null)
        {
            quiereAvanzar = teclado.enterKey.wasPressedThisFrame || teclado.spaceKey.wasPressedThisFrame || teclado.numpadEnterKey.wasPressedThisFrame;
            quiereSaltar = teclado.escapeKey.wasPressedThisFrame;
        }
        if (raton != null && raton.leftButton.wasPressedThisFrame == true)
        {
            quiereAvanzar = true;
        }

        if (quiereSaltar == true)
        {
            Terminar();
        }
        else if (quiereAvanzar == true)
        {
            Avanzar();
        }
    }

    public void Avanzar()
    {
        // Si la página todavía está apareciendo, el primer toque la completa.
        if (tiempoEnPagina < segundosDeAparicion)
        {
            tiempoEnPagina = segundosDeAparicion;
            return;
        }

        if (paginaActual + 1 < paginas.Length)
        {
            MostrarPagina(paginaActual + 1);
        }
        else
        {
            Terminar();
        }
    }

    private void MostrarPagina(int numero)
    {
        paginaActual = numero;
        tiempoEnPagina = 0f;
        textoPagina.text = ReemplazarDatos(paginas[numero]);

        string ayuda = "Enter: seguir     Esc: saltar";
        if (paginaActual == paginas.Length - 1)
        {
            ayuda = "Enter: continuar";
        }
        textoAyuda.text = ayuda;
    }

    private string ReemplazarDatos(string texto)
    {
        int estrellas = 0;
        int estrellasPosibles = 0;
        for (int numeroMundo = 1; numeroMundo <= DatosJuego.CantidadDeMundos(); numeroMundo++)
        {
            estrellas = estrellas + GestorProgreso.ObtenerEstrellasDelMundo(numeroMundo);
            estrellasPosibles = estrellasPosibles + GestorProgreso.ObtenerEstrellasMaximasPorMundo();
        }
        return texto.Replace("{estrellas}", estrellas + " de " + estrellasPosibles);
    }

    private void Terminar()
    {
        SceneManager.LoadScene(escenaSiguiente);
    }
}
