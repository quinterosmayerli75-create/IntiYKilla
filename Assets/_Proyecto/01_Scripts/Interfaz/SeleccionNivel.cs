// =====================================================================
// SeleccionNivel.cs
// Proyecto: Inti y Killa
// Descripción: Pantalla con los 5 niveles del mundo elegido. Es una sola
//              escena para todos los mundos: al abrirse lee cuál se eligió
//              y muestra su paisaje, su título y sus estrellas.
//              - Esc o la flecha: regresar a la pantalla de mundos.
//              - F1: desbloquear todo (modo prueba / defensa).
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SeleccionNivel : MonoBehaviour
{
    [SerializeField] private Text textoTitulo;
    [SerializeField] private Text textoEstrellas;
    [SerializeField] private FondoMundo fondo;

    public void Configurar(Text nuevoTextoTitulo, Text nuevoTextoEstrellas, FondoMundo nuevoFondo)
    {
        textoTitulo = nuevoTextoTitulo;
        textoEstrellas = nuevoTextoEstrellas;
        fondo = nuevoFondo;
    }

    private void Start()
    {
        int numeroMundo = GestorProgreso.MundoElegido;
        fondo.Mostrar(numeroMundo, false);
        textoTitulo.text = "Mundo " + numeroMundo + ": " + DatosJuego.ObtenerNombreMundo(numeroMundo);
        textoEstrellas.text = GestorProgreso.ObtenerEstrellasDelMundo(numeroMundo) + "/" + GestorProgreso.ObtenerEstrellasMaximasPorMundo();
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            return;
        }

        if (teclado.escapeKey.wasPressedThisFrame == true)
        {
            VolverAMundos();
        }

        if (teclado.f1Key.wasPressedThisFrame == true)
        {
            GestorProgreso.DesbloquearTodo();
            RefrescarBotones();
        }
    }

    public void VolverAMundos()
    {
        SceneManager.LoadScene(DatosJuego.escenaSeleccionMundo);
    }

    private void RefrescarBotones()
    {
        BotonNivel[] botones = FindObjectsByType<BotonNivel>();
        foreach (BotonNivel boton in botones)
        {
            boton.ActualizarEstado();
        }
    }
}
