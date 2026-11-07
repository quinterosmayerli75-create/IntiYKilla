// =====================================================================
// Opciones.cs
// Proyecto: Inti y Killa
// Descripción: Opciones del jugador guardadas con PlayerPrefs: volumen general
//              y pantalla completa. Se aplican solas al abrir el juego.
//              (El volumen separado de música y efectos llegará con el
//              Audio Mixer, cuando estén los sonidos.)
// =====================================================================

using UnityEngine;

public static class Opciones
{
    private const string claveVolumen = "IntiYKilla_Volumen";
    private const string clavePantallaCompleta = "IntiYKilla_PantallaCompleta";

    public static float Volumen
    {
        get { return PlayerPrefs.GetFloat(claveVolumen, 0.8f); }
        set
        {
            float volumenValido = Mathf.Clamp01(value);
            PlayerPrefs.SetFloat(claveVolumen, volumenValido);
            AudioListener.volume = volumenValido;
        }
    }

    public static bool PantallaCompleta
    {
        get { return PlayerPrefs.GetInt(clavePantallaCompleta, 0) == 1; }
        set
        {
            int valor = 0;
            if (value == true)
            {
                valor = 1;
            }
            PlayerPrefs.SetInt(clavePantallaCompleta, valor);
            Screen.fullScreen = value;
        }
    }

    public static void Guardar()
    {
        PlayerPrefs.Save();
    }

    // Unity llama a este método solo, una vez, al empezar el juego.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AplicarAlIniciar()
    {
        AudioListener.volume = Volumen;
    }
}
