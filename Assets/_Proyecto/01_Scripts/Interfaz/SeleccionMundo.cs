// =====================================================================
// SeleccionMundo.cs
// Proyecto: Inti y Killa
// Descripción: Pantalla con los 6 mundos del juego.
//              - Esc: volver al menú principal.
//              - F1: desbloquear todo (modo prueba / defensa).
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SeleccionMundo : MonoBehaviour
{
    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            return;
        }

        if (teclado.escapeKey.wasPressedThisFrame == true)
        {
            VolverAlMenu();
        }

        if (teclado.f1Key.wasPressedThisFrame == true)
        {
            GestorProgreso.DesbloquearTodo();
            RefrescarBotones();
        }
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene(DatosJuego.escenaMenuPrincipal);
    }

    private void RefrescarBotones()
    {
        BotonMundo[] botones = FindObjectsByType<BotonMundo>();
        foreach (BotonMundo boton in botones)
        {
            boton.ActualizarEstado();
        }
    }
}
