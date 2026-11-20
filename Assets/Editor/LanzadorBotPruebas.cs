// =====================================================================
// LanzadorBotPruebas.cs
// Proyecto: Inti y Killa
// Descripción: Pone en marcha el bot de pruebas (BotPruebas): abre el menú
//              principal y entra en Play con el bot encendido.
//              - Desde el menú "Inti y Killa > Pruebas", dentro del editor.
//              - Desde la consola: Herramientas/correr_bot.ps1 llama a
//                LanzadorBotPruebas.Ejecutar en modo batch.
// =====================================================================

using System;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class LanzadorBotPruebas
{
    private const string escenaInicial = "Assets/_Proyecto/00_Scenes/MenuPrincipal.unity";

    // Modo batch: vuelve a generar todas las escenas desde los generadores
    // (lo mismo que el menú "Generar todas las escenas") y guarda.
    public static void GenerarEscenas()
    {
        GeneradorEscenas.GenerarTodas();
        AssetDatabase.SaveAssets();
    }

    // Modo batch: las variables PRUEBA_BOT y SOLO_NIVEL ya vienen de la consola.
    public static void Ejecutar()
    {
        EditorSceneManager.OpenScene(escenaInicial);
        EditorApplication.isPlaying = true;
    }

    [MenuItem("Inti y Killa/Pruebas/Bot: sistema y Mundos 5 y 6", false, 200)]
    private static void ProbarSistemaYMundosFinales()
    {
        ProbarDesdeElMenu("");
    }

    [MenuItem("Inti y Killa/Pruebas/Bot: Mundos 1 a 4", false, 201)]
    private static void ProbarPrimerosMundos()
    {
        ProbarDesdeElMenu("todos");
    }

    [MenuItem("Inti y Killa/Pruebas/Bot: piezas del desafío", false, 202)]
    private static void ProbarPiezasDelDesafio()
    {
        ProbarDesdeElMenu("desafio");
    }

    [MenuItem("Inti y Killa/Pruebas/Bot: reinicio de los 30 niveles", false, 203)]
    private static void ProbarReinicios()
    {
        ProbarDesdeElMenu("reinicio");
    }

    // El bot lee estas variables al empezar el Play, en este mismo proceso.
    private static void ProbarDesdeElMenu(string modo)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
        {
            return;
        }

        Environment.SetEnvironmentVariable("PRUEBA_BOT", "1");
        Environment.SetEnvironmentVariable("SOLO_NIVEL", modo);
        EditorApplication.playModeStateChanged += ApagarAlSalir;
        Ejecutar();
    }

    // Al salir del Play se apaga el bot, para que jugar normal no lo encienda.
    private static void ApagarAlSalir(PlayModeStateChange estado)
    {
        if (estado == PlayModeStateChange.EnteredEditMode)
        {
            Environment.SetEnvironmentVariable("PRUEBA_BOT", null);
            Environment.SetEnvironmentVariable("SOLO_NIVEL", null);
            EditorApplication.playModeStateChanged -= ApagarAlSalir;
        }
    }
}
