// =====================================================================
// PantallaLogros.cs
// Proyecto: Inti y Killa
// Descripción: Ventana del menú principal con la lista de logros: los
//              ganados se ven dorados y los que faltan, grises. También
//              muestra cuántos niveles se pasaron con las 3 vidas intactas.
//              Se cierra con Esc o con el botón "Cerrar".
// =====================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PantallaLogros : MonoBehaviour
{
    private const float altoReferencia = 1080f;

    private bool estaAbierta = false;

    public bool EstaAbierta
    {
        get { return estaAbierta; }
    }

    public void Abrir()
    {
        estaAbierta = true;
    }

    public void Cerrar()
    {
        estaAbierta = false;
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (estaAbierta == true && teclado != null && teclado.escapeKey.wasPressedThisFrame == true)
        {
            Cerrar();
        }
    }

    private int ContarNivelesPerfectos()
    {
        int cantidad = 0;
        for (int numeroMundo = 1; numeroMundo <= DatosJuego.CantidadDeMundos(); numeroMundo++)
        {
            for (int numeroEnMundo = 1; numeroEnMundo <= DatosJuego.nivelesPorMundo; numeroEnMundo++)
            {
                if (GestorLogros.NivelEsPerfecto(numeroMundo, numeroEnMundo) == true)
                {
                    cantidad = cantidad + 1;
                }
            }
        }
        return cantidad;
    }

    private void OnGUI()
    {
        if (estaAbierta == false)
        {
            return;
        }

        // Coordenadas "virtuales" de 1080 de alto, como en el resto del juego.
        float escala = Screen.height / altoReferencia;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(escala, escala, 1f));
        float anchoVirtual = Screen.width / escala;

        List<GestorLogros.DatosLogro> logros = GestorLogros.ObtenerTodos();
        float anchoVentana = 1100f;
        float altoFila = 72f;
        float altoVentana = 230f + logros.Count * altoFila;
        Rect ventana = new Rect((anchoVirtual - anchoVentana) / 2f, (altoReferencia - altoVentana) / 2f, anchoVentana, altoVentana);

        Color colorAnterior = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.9f);
        GUI.DrawTexture(new Rect(0f, 0f, anchoVirtual, altoReferencia), Texture2D.whiteTexture);
        GUI.color = colorAnterior;

        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.label);
        estiloTitulo.fontSize = 56;
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.MiddleCenter;
        estiloTitulo.normal.textColor = new Color(1f, 0.85f, 0.3f);
        GUI.Label(new Rect(ventana.x, ventana.y, anchoVentana, 70f), "LOGROS  (" + GestorLogros.CantidadDesbloqueados() + " de " + logros.Count + ")", estiloTitulo);

        GUIStyle estiloResumen = new GUIStyle(GUI.skin.label);
        estiloResumen.fontSize = 28;
        estiloResumen.alignment = TextAnchor.MiddleCenter;
        estiloResumen.normal.textColor = Color.white;
        string resumen = "Niveles pasados con las 3 vidas intactas: " + ContarNivelesPerfectos() + " de " + (DatosJuego.CantidadDeMundos() * DatosJuego.nivelesPorMundo);
        GUI.Label(new Rect(ventana.x, ventana.y + 70f, anchoVentana, 40f), resumen, estiloResumen);

        GUIStyle estiloNombre = new GUIStyle(GUI.skin.label);
        estiloNombre.fontSize = 32;
        estiloNombre.fontStyle = FontStyle.Bold;
        GUIStyle estiloDescripcion = new GUIStyle(GUI.skin.label);
        estiloDescripcion.fontSize = 24;

        float y = ventana.y + 125f;
        foreach (GestorLogros.DatosLogro logro in logros)
        {
            bool ganado = GestorLogros.LogroDesbloqueado(logro.clave);
            Color colorTexto = new Color(0.55f, 0.55f, 0.55f);
            string marca = "[  ]";
            if (ganado == true)
            {
                colorTexto = new Color(1f, 0.85f, 0.3f);
                marca = "[★]";
            }
            estiloNombre.normal.textColor = colorTexto;
            estiloDescripcion.normal.textColor = colorTexto;
            GUI.Label(new Rect(ventana.x + 40f, y, 120f, 40f), marca, estiloNombre);
            GUI.Label(new Rect(ventana.x + 140f, y, anchoVentana - 180f, 40f), logro.nombre, estiloNombre);
            GUI.Label(new Rect(ventana.x + 140f, y + 36f, anchoVentana - 180f, 32f), logro.descripcion, estiloDescripcion);
            y = y + altoFila;
        }

        GUIStyle estiloBoton = new GUIStyle(GUI.skin.button);
        estiloBoton.fontSize = 32;
        if (GUI.Button(new Rect(ventana.x + anchoVentana / 2f - 150f, ventana.y + altoVentana - 80f, 300f, 64f), "Cerrar (Esc)", estiloBoton) == true)
        {
            Cerrar();
        }
    }
}
