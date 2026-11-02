// =====================================================================
// SeleccionNivel.cs
// Proyecto: Inti y Killa
// Descripción: Pantalla del mapa de niveles.
//              - Esc: volver al menú principal.
//              - F1: desbloquear todos los niveles (modo prueba / defensa).
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SeleccionNivel : MonoBehaviour
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
        BotonNivel[] botones = FindObjectsByType<BotonNivel>(FindObjectsSortMode.None);
        foreach (BotonNivel boton in botones)
        {
            boton.ActualizarEstado();
        }
    }
}
