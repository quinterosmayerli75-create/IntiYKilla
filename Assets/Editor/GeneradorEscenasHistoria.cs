// =====================================================================
// GeneradorEscenasHistoria.cs
// Proyecto: Inti y Killa
// Descripción: Escenas Intro (antes del menú) y Final (después del Guardián):
//              un paisaje oscurecido y la historia contada en páginas.
//              Es otra parte de GeneradorEscenas.
// =====================================================================

using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static partial class GeneradorEscenas
{
    private static string GenerarIntro()
    {
        string[] paginas = new string[]
        {
            "Hace mucho tiempo, Inti, el Sol, y Killa, la Luna, cuidaban juntos el equilibrio entre el día y la noche.",
            "Pero un eclipse sin fin rompió ese equilibrio, y las gemas de luz se esparcieron por toda Bolivia.",
            "Desde el Salar de Uyuni hasta la Isla del Sol, Inti y Killa deberán recuperarlas... juntos.",
            "Un consejo antes de empezar: no confíes en nada... ni en tu compañero."
        };
        return GenerarPantallaHistoria(DatosJuego.escenaIntro, 1, paginas, DatosJuego.escenaMenuPrincipal);
    }

    private static string GenerarFinal()
    {
        string[] paginas = new string[]
        {
            "El Guardián del Eclipse fue vencido, y el sol volvió a salir sobre el lago Titicaca.",
            "Inti y Killa devolvieron el equilibrio entre el día y la noche. Ninguno lo habría logrado solo.",
            "Estrellas conseguidas: {estrellas}.",
            "Inti y Killa\nUn juego de Mayerli Quinteros Silva\n\n¡Gracias por jugar!"
        };
        return GenerarPantallaHistoria(DatosJuego.escenaFinal, 6, paginas, DatosJuego.escenaMenuPrincipal);
    }

    private static string GenerarPantallaHistoria(string nombreEscena, int mundoDelFondo, string[] paginas, string escenaSiguiente)
    {
        Scene escena = CrearEscenaVacia();
        HerramientasEscena.CrearCamara(Color.black);
        Transform canvas = HerramientasEscena.CrearCanvas();

        // El paisaje se oscurece para que el texto se lea bien.
        Image fondo = HerramientasInterfaz.CrearImagen(canvas, "Fondo", HerramientasInterfaz.ObtenerImagenFondo(mundoDelFondo), new Color(0.45f, 0.45f, 0.55f), Vector2.zero, Vector2.zero);
        AspectRatioFitter ajuste = fondo.gameObject.AddComponent<AspectRatioFitter>();
        ajuste.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        ajuste.aspectRatio = 16f / 9f;

        Image panel = HerramientasInterfaz.CrearImagen(canvas, "Panel", null, new Color(0f, 0f, 0f, 0.6f), new Vector2(0f, 0f), new Vector2(1500f, 420f));
        Text textoPagina = HerramientasEscena.CrearTexto(panel.transform, "Texto", "", 54, Vector2.zero, new Vector2(1400f, 380f), new Color(1f, 0.95f, 0.85f));
        textoPagina.horizontalOverflow = HorizontalWrapMode.Wrap;
        textoPagina.raycastTarget = false;

        Text textoAyuda = HerramientasEscena.CrearTexto(canvas, "Ayuda", "", 28, new Vector2(0f, -470f), new Vector2(1600f, 50f), new Color(1f, 1f, 1f, 0.85f));
        textoAyuda.raycastTarget = false;

        GameObject objeto = new GameObject("Historia");
        PantallaHistoria historia = objeto.AddComponent<PantallaHistoria>();
        historia.Configurar(textoPagina, textoAyuda, paginas, escenaSiguiente);

        string ruta = RutaEscena(nombreEscena);
        EditorSceneManager.SaveScene(escena, ruta);
        return ruta;
    }
}
