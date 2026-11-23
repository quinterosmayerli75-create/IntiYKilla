// =====================================================================
// MenuPrincipal.cs
// Proyecto: Inti y Killa
// Descripción: Acciones de los botones del menú principal.
//              Los botones se conectan a estos métodos desde el
//              generador de escenas (o a mano en el Inspector: On Click).
//              Atajo de teclado: Enter = Jugar.
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private Text textoMensaje;

    public void Configurar(Text nuevoTextoMensaje)
    {
        textoMensaje = nuevoTextoMensaje;
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null)
        {
            return;
        }

        bool presionoEnter = teclado.enterKey.wasPressedThisFrame || teclado.numpadEnterKey.wasPressedThisFrame;
        if (presionoEnter == true)
        {
            Jugar();
        }
    }

    // La historia de los hermanos se cuenta la primera vez que se juega en cada
    // sesión (se puede saltar con Esc); después se va directo a los mundos.
    private static bool yaSeContoLaHistoria = false;

    public void Jugar()
    {
        if (yaSeContoLaHistoria == false && Application.CanStreamedLevelBeLoaded(DatosJuego.escenaIntro) == true)
        {
            yaSeContoLaHistoria = true;
            SceneManager.LoadScene(DatosJuego.escenaIntro);
            return;
        }

        SceneManager.LoadScene(DatosJuego.escenaSeleccionMundo);
    }

    public void BorrarProgreso()
    {
        GestorProgreso.BorrarProgreso();

        if (textoMensaje != null)
        {
            textoMensaje.text = "Progreso borrado. ¡A empezar de nuevo!";
        }
    }

    public void Salir()
    {
        // En el editor, Application.Quit no hace nada; por eso detenemos el Play.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
