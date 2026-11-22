// =====================================================================
// TransicionReinicio.cs
// Proyecto: Inti y Killa
// Descripción: Hace que se NOTE cuando el nivel se reinicia: al morir, la
//              pantalla se oscurece hasta quedar negra, el nivel se vuelve a
//              cargar y aparece de nuevo desde negro, con el número de intento.
//              Vive entre escenas (DontDestroyOnLoad) porque el oscurecido
//              empieza en una escena y termina en la siguiente.
// Uso: GestorNivel llama a Oscurecer() al morir; nadie más lo usa.
// =====================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class TransicionReinicio : MonoBehaviour
{
    private const float segundosAclarando = 0.6f;
    private const float segundosMostrandoIntento = 1.2f;

    private static TransicionReinicio instancia;

    private float opacidad = 0f;
    private float velocidadOscurecer = 0f;
    private bool oscureciendo = false;
    private bool aclarando = false;
    private float tiempoAclarando = 0f;
    private string textoIntento = "";

    // Empieza a oscurecer la pantalla; llega a negro en "segundos" y, en negro,
    // muestra el texto ("Intento 3", o adónde los devuelve el nivel final).
    public static void Oscurecer(float segundos, string texto)
    {
        if (instancia == null)
        {
            GameObject objeto = new GameObject("Transicion de reinicio");
            DontDestroyOnLoad(objeto);
            instancia = objeto.AddComponent<TransicionReinicio>();
            SceneManager.sceneLoaded += instancia.AlCargarEscena;
        }

        instancia.oscureciendo = true;
        instancia.aclarando = false;
        instancia.velocidadOscurecer = 1f / Mathf.Max(0.05f, segundos);
        instancia.textoIntento = texto;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= AlCargarEscena;
        if (instancia == this)
        {
            instancia = null;
        }
    }

    // Cuando el nivel ya se volvió a cargar, la pantalla empieza a aclarar.
    private void AlCargarEscena(Scene escena, LoadSceneMode modo)
    {
        if (oscureciendo == false && opacidad <= 0f)
        {
            return;
        }

        oscureciendo = false;
        aclarando = true;
        opacidad = 1f;
        tiempoAclarando = 0f;
    }

    private void Update()
    {
        // unscaledDeltaTime: así funciona aunque el juego esté en pausa.
        float paso = Time.unscaledDeltaTime;

        if (oscureciendo == true)
        {
            opacidad = Mathf.Min(1f, opacidad + velocidadOscurecer * paso);
        }
        else if (aclarando == true)
        {
            tiempoAclarando = tiempoAclarando + paso;
            // Primero se queda negra un momento con el número de intento, y después aclara.
            if (tiempoAclarando > segundosMostrandoIntento * 0.5f)
            {
                opacidad = Mathf.Max(0f, opacidad - paso / segundosAclarando);
            }
            if (opacidad <= 0f)
            {
                aclarando = false;
            }
        }
    }

    private void OnGUI()
    {
        if (opacidad <= 0f)
        {
            return;
        }

        // Se dibuja encima de todo lo demás.
        GUI.depth = -1000;
        Color colorAnterior = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, opacidad);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);

        if (textoIntento != "" && opacidad > 0.5f)
        {
            GUIStyle estilo = new GUIStyle(GUI.skin.label);
            estilo.alignment = TextAnchor.MiddleCenter;
            estilo.fontSize = Mathf.RoundToInt(Screen.height * 0.06f);
            estilo.fontStyle = FontStyle.Bold;
            estilo.normal.textColor = new Color(1f, 0.85f, 0.3f, opacidad);
            GUI.color = Color.white;
            GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), textoIntento, estilo);
        }

        GUI.color = colorAnterior;
    }
}
