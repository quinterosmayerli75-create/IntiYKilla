// =====================================================================
// HojaTrampolin.cs
// Proyecto: Inti y Killa
// Descripción: Hoja gigante del Trópico: quien la pisa sale disparado hacia
//              arriba, mucho más alto que un salto normal. Se aplasta un
//              momento al rebotar, para que se note qué pasó.
// Uso: se apoya sobre un piso. No necesita colisionador: revisa si los pies
//      de un jugador que no va subiendo tocan su zona.
// =====================================================================

using UnityEngine;

public class HojaTrampolin : MonoBehaviour
{
    [SerializeField] private float fuerzaRebote = 21f;
    [SerializeField] private Vector2 tamanoZona = new Vector2(2f, 0.35f);
    [SerializeField] private float segundosAplastada = 0.25f;

    private ControladorJugador[] jugadores;
    private Vector3 escalaNormal;
    private float tiempoAplastada = 0f;

    // La usa el trampolín catapulta: una hoja igual a las demás que lanza
    // con demasiada fuerza (contra las púas del techo).
    public void ConfigurarFuerza(float nuevaFuerza)
    {
        fuerzaRebote = nuevaFuerza;
    }

    public void Configurar(Vector2 nuevoTamanoZona)
    {
        tamanoZona = nuevoTamanoZona;
    }

    private void Awake()
    {
        jugadores = FindObjectsByType<ControladorJugador>();
        escalaNormal = transform.localScale;
    }

    private void Update()
    {
        Vector2 centro = transform.position;

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.EstaVivo == false || jugador.VelocidadVertical > 0.1f)
            {
                continue;
            }

            Bounds cuerpo = jugador.LimitesCuerpo;
            bool cruzaEnX = cuerpo.max.x > centro.x - tamanoZona.x / 2f && cuerpo.min.x < centro.x + tamanoZona.x / 2f;
            bool cruzaEnY = cuerpo.min.y < centro.y + tamanoZona.y / 2f && cuerpo.max.y > centro.y - tamanoZona.y / 2f;
            if (cruzaEnX == true && cruzaEnY == true)
            {
                jugador.Rebotar(fuerzaRebote);
                tiempoAplastada = segundosAplastada;
            }
        }

        tiempoAplastada = Mathf.Max(0f, tiempoAplastada - Time.deltaTime);
        float aplastamiento = 1f;
        if (tiempoAplastada > 0f)
        {
            aplastamiento = 0.5f;
        }
        transform.localScale = new Vector3(escalaNormal.x, escalaNormal.y * aplastamiento, escalaNormal.z);
    }
}
