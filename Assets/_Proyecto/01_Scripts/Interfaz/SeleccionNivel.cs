// =====================================================================
// SeleccionNivel.cs
// Proyecto: Inti y Killa
// Descripción: Pantalla con los 5 niveles del mundo elegido. Es una sola
//              escena para todos los mundos: al abrirse lee cuál se eligió
//              y cada botón muestra el nivel que le corresponde.
//              - Esc o "Volver": regresar a la pantalla de mundos.
//              - F1: desbloquear todo (modo prueba / defensa).
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SeleccionNivel : MonoBehaviour
{
    [SerializeField] private Text textoTitulo;

    public void Configurar(Text nuevoTextoTitulo)
    {
        textoTitulo = nuevoTextoTitulo;
    }

    private void Start()
    {
        if (textoTitulo != null)
        {
            int numeroMundo = GestorProgreso.MundoElegido;
            textoTitulo.text = "Mundo " + numeroMundo + ": " + DatosJuego.ObtenerNombreMundo(numeroMundo);
        }
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
