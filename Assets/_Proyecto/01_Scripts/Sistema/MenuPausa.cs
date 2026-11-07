// =====================================================================
// MenuPausa.cs
// Proyecto: Inti y Killa
// Descripción: Pausa del nivel, con Esc. Detiene el tiempo del juego y muestra:
//              Continuar, Reiniciar, Opciones y Salir al mapa. En Opciones se
//              cambia el volumen y la pantalla completa.
//              Se dibuja con OnGUI, igual que el HUD temporal del nivel.
// Uso: en el mismo objeto que el GestorNivel.
// =====================================================================

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GestorNivel))]
public class MenuPausa : MonoBehaviour
{
    private const float altoReferencia = 1080f;

    private static bool estaPausado = false;

    private GestorNivel gestor;
    private bool mostrandoOpciones = false;

    // Otros scripts lo consultan para no reaccionar a teclas durante la pausa.
    public static bool EstaPausado
    {
        get { return estaPausado; }
    }

    private void Awake()
    {
        gestor = GetComponent<GestorNivel>();
        Reanudar();
    }

    private void Update()
    {
        Keyboard teclado = Keyboard.current;
        if (teclado == null || gestor.NivelTerminado == true)
        {
            return;
        }

        if (teclado.escapeKey.wasPressedThisFrame == true)
        {
            if (estaPausado == true)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    private void Pausar()
    {
        estaPausado = true;
        mostrandoOpciones = false;
        Time.timeScale = 0f;
    }

    private void Reanudar()
    {
        estaPausado = false;
        mostrandoOpciones = false;
        Time.timeScale = 1f;
    }

    // Al cambiar de escena el tiempo debe volver a correr, pase lo que pase.
    private void OnDestroy()
    {
        estaPausado = false;
        Time.timeScale = 1f;
    }

    private void OnGUI()
    {
        if (estaPausado == false)
        {
            return;
        }

        float escala = Screen.height / altoReferencia;
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, new Vector3(escala, escala, 1f));
        float anchoVirtual = Screen.width / escala;

        // Fondo oscuro sobre todo el nivel.
        Color colorAnterior = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, 0.75f);
        GUI.DrawTexture(new Rect(0f, 0f, anchoVirtual, altoReferencia), Texture2D.whiteTexture);
        GUI.color = colorAnterior;

        GUIStyle estiloTitulo = new GUIStyle(GUI.skin.label);
        estiloTitulo.fontSize = 72;
        estiloTitulo.fontStyle = FontStyle.Bold;
        estiloTitulo.alignment = TextAnchor.MiddleCenter;
        estiloTitulo.normal.textColor = new Color(1f, 0.88f, 0.4f);

        GUIStyle estiloBoton = new GUIStyle(GUI.skin.button);
        estiloBoton.fontSize = 38;

        float anchoBoton = 520f;
        float altoBoton = 80f;
        float posicionX = (anchoVirtual - anchoBoton) / 2f;

        if (mostrandoOpciones == true)
        {
            DibujarOpciones(anchoVirtual, posicionX, anchoBoton, altoBoton, estiloTitulo, estiloBoton);
            return;
        }

        GUI.Label(new Rect(0f, 220f, anchoVirtual, 100f), "Pausa", estiloTitulo);

        if (GUI.Button(new Rect(posicionX, 380f, anchoBoton, altoBoton), "Continuar", estiloBoton) == true)
        {
            Reanudar();
        }
        if (GUI.Button(new Rect(posicionX, 480f, anchoBoton, altoBoton), "Reiniciar nivel", estiloBoton) == true)
        {
            Reanudar();
            gestor.ReiniciarNivel();
        }
        if (GUI.Button(new Rect(posicionX, 580f, anchoBoton, altoBoton), "Opciones", estiloBoton) == true)
        {
            mostrandoOpciones = true;
        }
        if (GUI.Button(new Rect(posicionX, 680f, anchoBoton, altoBoton), "Salir al mapa", estiloBoton) == true)
        {
            Reanudar();
            gestor.VolverAlMapa();
        }
    }

    private void DibujarOpciones(float anchoVirtual, float posicionX, float anchoBoton, float altoBoton, GUIStyle estiloTitulo, GUIStyle estiloBoton)
    {
        GUI.Label(new Rect(0f, 220f, anchoVirtual, 100f), "Opciones", estiloTitulo);

        GUIStyle estiloTexto = new GUIStyle(GUI.skin.label);
        estiloTexto.fontSize = 36;
        estiloTexto.alignment = TextAnchor.MiddleCenter;
        estiloTexto.normal.textColor = Color.white;

        int porcentaje = Mathf.RoundToInt(Opciones.Volumen * 100f);
        GUI.Label(new Rect(0f, 370f, anchoVirtual, 60f), "Volumen: " + porcentaje + "%", estiloTexto);
        float nuevoVolumen = GUI.HorizontalSlider(new Rect(posicionX, 450f, anchoBoton, 40f), Opciones.Volumen, 0f, 1f);
        if (Mathf.Approximately(nuevoVolumen, Opciones.Volumen) == false)
        {
            Opciones.Volumen = nuevoVolumen;
        }

        string textoPantalla = "Pantalla completa: no";
        if (Opciones.PantallaCompleta == true)
        {
            textoPantalla = "Pantalla completa: sí";
        }
        if (GUI.Button(new Rect(posicionX, 530f, anchoBoton, altoBoton), textoPantalla, estiloBoton) == true)
        {
            Opciones.PantallaCompleta = !Opciones.PantallaCompleta;
        }

        if (GUI.Button(new Rect(posicionX, 650f, anchoBoton, altoBoton), "Volver", estiloBoton) == true)
        {
            Opciones.Guardar();
            mostrandoOpciones = false;
        }
    }
}
