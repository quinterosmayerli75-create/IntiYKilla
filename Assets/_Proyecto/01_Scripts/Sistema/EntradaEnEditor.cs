// =====================================================================
// EntradaEnEditor.cs
// Proyecto: Inti y Killa
// Descripción: Solo dentro del Editor de Unity. Por defecto, el teclado
//              llega al juego únicamente si la ventana "Game" tiene el foco:
//              basta con hacer clic en otra ventana (Hierarchy, Inspector, la
//              grabadora de pantalla...) para que las teclas "dejen de
//              funcionar". Este script hace que el teclado siempre llegue al
//              juego mientras se está jugando en el Editor.
//              En el juego exportado (WebGL) no hace nada.
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;

public static class EntradaEnEditor
{
#if UNITY_EDITOR
    // Unity llama a este método solo, al empezar cada partida en el Editor.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void HacerQueElTecladoSiempreLlegue()
    {
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
    }
#endif
}
