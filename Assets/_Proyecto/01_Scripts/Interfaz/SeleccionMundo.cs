// =====================================================================
// SeleccionMundo.cs
// Proyecto: Inti y Killa
// Descripción: Teclado de la pantalla de mundos.
//              - Izquierda/Derecha o A/D: cambiar de mundo.
//              - Enter: entrar al mundo del centro.
//              - Esc: volver al menú principal.
//              - F1: desbloquear todo (modo prueba / defensa).
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SeleccionMundo : MonoBehaviour
{
    [SerializeField] private CarruselMundos carrusel;

    public void Configurar(CarruselMundos nuevoCarrusel)
    {
        carrusel = nuevoCarrusel;
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
            VolverAlMenu();
        }

        if (teclado.f1Key.wasPressedThisFrame == true)
        {
            GestorProgreso.DesbloquearTodo();
            RefrescarBotones();
        }

        if (teclado.leftArrowKey.wasPressedThisFrame == true || teclado.aKey.wasPressedThisFrame == true)
        {
            carrusel.MoverAnterior();
        }

        if (teclado.rightArrowKey.wasPressedThisFrame == true || teclado.dKey.wasPressedThisFrame == true)
        {
            carrusel.MoverSiguiente();
        }

        if (teclado.enterKey.wasPressedThisFrame == true || teclado.numpadEnterKey.wasPressedThisFrame == true)
        {
            carrusel.AbrirMundoCentral();
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
