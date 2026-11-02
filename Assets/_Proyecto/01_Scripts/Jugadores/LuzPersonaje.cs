// =====================================================================
// LuzPersonaje.cs
// Proyecto: Inti y Killa
// Descripción: Luz 2D que acompaña al personaje que tiene el elemento
//              FUEGO (al empezar es Inti; después de un eclipse, la luz
//              pasa a quien tenga el fuego). Sirve para los niveles
//              oscuros de las Minas de Potosí.
//              - La luz normal ilumina un círculo pequeño.
//              - Mientras se mantiene la tecla de habilidad (S o Flecha
//                abajo) el círculo crece y llega más lejos.
//              Otros scripts (como PlataformaOculta) preguntan con
//              LuzPersonaje.EstaIluminado(...) si un punto está en la luz.
// Uso: ponerlo en los DOS jugadores y asignarle un Light2D de tipo Point
//      (hijo del jugador). Solo se enciende en el que tenga fuego.
// =====================================================================

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(ControladorJugador))]
public class LuzPersonaje : MonoBehaviour
{
    [Header("Luz 2D (hijo del jugador, tipo Point)")]
    [SerializeField] private Light2D luz;

    [Header("Alcance de la luz (en unidades)")]
    [SerializeField] private float radioNormal = 3.5f;
    [SerializeField] private float radioAmpliado = 5.5f;
    // Qué tan rápido crece o se achica el círculo de luz.
    [SerializeField] private float velocidadCambioRadio = 6f;

    [Header("Aspecto")]
    [SerializeField] private float intensidadLuz = 1.2f;
    [SerializeField] private Color colorLuz = new Color(1f, 0.78f, 0.45f);

    // Lista compartida con todas las luces de la escena, para que las
    // plataformas puedan preguntar sin buscar objetos cada frame.
    private static List<LuzPersonaje> lucesEnEscena = new List<LuzPersonaje>();

    private ControladorJugador jugador;
    private float radioActual = 0f;

    // Radio que de verdad está iluminando en este momento (0 si está apagada).
    public float RadioActual
    {
        get { return radioActual; }
    }

    // Lo usa el generador de escenas para conectar la luz creada por código.
    public void Configurar(Light2D nuevaLuz)
    {
        luz = nuevaLuz;
    }

    private void Awake()
    {
        jugador = GetComponent<ControladorJugador>();
    }

    private void OnEnable()
    {
        lucesEnEscena.Add(this);
    }

    private void OnDisable()
    {
        lucesEnEscena.Remove(this);
    }

    private void Update()
    {
        // Solo alumbra quien tiene el fuego y sigue vivo.
        bool tieneFuego = jugador.Elemento == TipoElemento.Fuego && jugador.EstaVivo == true;

        float radioObjetivo = 0f;
        if (tieneFuego == true)
        {
            radioObjetivo = radioNormal;
            if (jugador.MantieneHabilidadPresionada() == true)
            {
                radioObjetivo = radioAmpliado;
            }
        }

        // MoveTowards cambia el radio poco a poco: la luz "respira" en vez de saltar.
        radioActual = Mathf.MoveTowards(radioActual, radioObjetivo, velocidadCambioRadio * Time.deltaTime);

        ActualizarLuzVisible();
    }

    private void ActualizarLuzVisible()
    {
        if (luz == null)
        {
            return;
        }

        if (radioActual <= 0.01f)
        {
            luz.enabled = false;
            return;
        }

        luz.enabled = true;
        luz.color = colorLuz;
        luz.intensity = intensidadLuz;
        luz.pointLightOuterRadius = radioActual;
        // El centro (radio interior) brilla al máximo; hacia el borde se apaga suave.
        luz.pointLightInnerRadius = radioActual * 0.35f;
    }

    // ¿Algún personaje con luz alcanza este punto? (lo usan las plataformas ocultas)
    public static bool EstaIluminado(Vector2 punto)
    {
        foreach (LuzPersonaje luzPersonaje in lucesEnEscena)
        {
            float distancia = Vector2.Distance(luzPersonaje.transform.position, punto);
            if (luzPersonaje.RadioActual > 0f && distancia <= luzPersonaje.RadioActual)
            {
                return true;
            }
        }

        return false;
    }

    // Igual que el anterior, pero para un objeto con tamaño: basta con que
    // cualquier parte de su rectángulo esté dentro de la luz.
    public static bool EstaIluminado(Bounds limites)
    {
        foreach (LuzPersonaje luzPersonaje in lucesEnEscena)
        {
            Vector3 posicionLuz = luzPersonaje.transform.position;
            Vector3 puntoMasCercano = limites.ClosestPoint(new Vector3(posicionLuz.x, posicionLuz.y, limites.center.z));
            float distancia = Vector2.Distance(posicionLuz, puntoMasCercano);

            if (luzPersonaje.RadioActual > 0f && distancia <= luzPersonaje.RadioActual)
            {
                return true;
            }
        }

        return false;
    }

    // Dibuja el alcance en la vista Scene para poder diseñar los niveles.
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.8f, 0.3f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, radioNormal);
        Gizmos.color = new Color(1f, 0.5f, 0.1f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, radioAmpliado);
    }
}
