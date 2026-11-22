// =====================================================================
// TiempoLimite.cs
// Proyecto: Inti y Killa
// Descripción: Cuenta regresiva para pasar el nivel (por ejemplo, 1 minuto).
//              Se ve grande arriba al centro y se pone roja en los últimos 10
//              segundos. Si llega a cero antes de ganar, cuenta como muerte:
//              el nivel se reinicia (o, en el final, se vuelve al 6-1).
// Uso: un solo objeto en la escena, creado por el generador.
// =====================================================================

using UnityEngine;

public class TiempoLimite : MonoBehaviour
{
    [SerializeField] private float segundosDisponibles = 60f;

    private float segundosRestantes;
    private bool seAcabo = false;

    public float SegundosRestantes
    {
        get { return segundosRestantes; }
    }

    public void Configurar(float nuevosSegundos)
    {
        segundosDisponibles = nuevosSegundos;
    }

    private void Awake()
    {
        segundosRestantes = segundosDisponibles;
    }

    private void Update()
    {
        if (seAcabo == true || GestorNivel.Instancia == null || GestorNivel.Instancia.NivelTerminado == true)
        {
            return;
        }

        segundosRestantes = segundosRestantes - Time.deltaTime;
        if (segundosRestantes <= 0f)
        {
            segundosRestantes = 0f;
            seAcabo = true;
            GestorNivel.Instancia.JugadorMurio("¡Se acabó el tiempo! Hay que pasar el nivel en menos de " + Mathf.RoundToInt(segundosDisponibles) + " segundos.");
        }
    }

    private void OnGUI()
    {
        if (GestorNivel.Instancia != null && GestorNivel.Instancia.NivelTerminado == true)
        {
            return;
        }

        int segundos = Mathf.CeilToInt(segundosRestantes);
        string texto = (segundos / 60) + ":" + (segundos % 60).ToString("00");

        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.alignment = TextAnchor.UpperCenter;
        estilo.fontSize = Mathf.RoundToInt(Screen.height * 0.055f);
        estilo.fontStyle = FontStyle.Bold;
        // En los últimos 10 segundos se pone roja: el aviso de que se acaba.
        if (segundosRestantes <= 10f)
        {
            estilo.normal.textColor = new Color(1f, 0.25f, 0.2f);
        }
        else
        {
            estilo.normal.textColor = new Color(1f, 0.9f, 0.4f);
        }

        GUI.Label(new Rect(0f, Screen.height * 0.01f, Screen.width, Screen.height * 0.08f), texto, estilo);
    }
}
