// =====================================================================
// EnemigoBase.cs
// Proyecto: Inti y Killa
// Descripción: Lo que comparten los enemigos que caminan o vuelan:
//              - Saltar ENCIMA de ellos los aturde por unos segundos y hace
//                rebotar al jugador (estilo plataformas clásico).
//              - Tocarlos de costado o por debajo mata al jugador.
//              - Mientras no haya arte se ven como una forma de color; basta
//                con poner una imagen en "imagenPropia" para reemplazarla.
//              Cada enemigo concreto decide cómo se mueve en MoverEnemigo.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class EnemigoBase : MonoBehaviour
{
    [Header("Dibujo")]
    [SerializeField] private Sprite imagenPropia;

    [Header("Cuerpo")]
    [SerializeField] private Vector2 tamanoCuerpo = new Vector2(0.8f, 0.7f);

    [Header("Interacción con los jugadores")]
    [SerializeField] private float segundosAturdido = 3f;
    [SerializeField] private float fuerzaRebote = 11f;
    // {0} se reemplaza por el nombre del jugador.
    [SerializeField] private string mensajeDeMuerte = "Algo atrapó a {0} en la oscuridad.";

    private SpriteRenderer dibujo;
    private ControladorJugador[] jugadores;
    private Color colorNormal;
    private float tiempoAturdidoRestante = 0f;

    public bool EstaAturdido
    {
        get { return tiempoAturdidoRestante > 0f; }
    }

    public void ConfigurarMensaje(string nuevoMensaje)
    {
        mensajeDeMuerte = nuevoMensaje;
    }

    protected virtual void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        jugadores = FindObjectsByType<ControladorJugador>();

        if (imagenPropia != null)
        {
            dibujo.sprite = imagenPropia;
            dibujo.color = Color.white;
        }

        colorNormal = dibujo.color;
    }

    private void Update()
    {
        if (EstaAturdido == true)
        {
            ActualizarAturdimiento();
            return;
        }

        MoverEnemigo();
        RevisarJugadores();
    }

    protected abstract void MoverEnemigo();

    private void ActualizarAturdimiento()
    {
        tiempoAturdidoRestante = tiempoAturdidoRestante - Time.deltaTime;

        // Parpadea en el último segundo: es el aviso de que va a despertar.
        if (tiempoAturdidoRestante > 0f && tiempoAturdidoRestante < 1f)
        {
            bool mostrarGris = Mathf.Repeat(tiempoAturdidoRestante * 6f, 1f) < 0.5f;
            if (mostrarGris == true)
            {
                dibujo.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
            }
            else
            {
                dibujo.color = colorNormal;
            }
        }
        else if (tiempoAturdidoRestante <= 0f)
        {
            dibujo.color = colorNormal;
        }
    }

    private void RevisarJugadores()
    {
        Vector2 centro = transform.position;

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null || jugador.EstaVivo == false)
            {
                continue;
            }

            Bounds cuerpo = jugador.LimitesCuerpo;
            bool cruzaEnX = cuerpo.max.x > centro.x - tamanoCuerpo.x / 2f && cuerpo.min.x < centro.x + tamanoCuerpo.x / 2f;
            bool cruzaEnY = cuerpo.max.y > centro.y - tamanoCuerpo.y / 2f && cuerpo.min.y < centro.y + tamanoCuerpo.y / 2f;
            if (cruzaEnX == false || cruzaEnY == false)
            {
                continue;
            }

            // Pisar: viene cayendo y sus pies están por encima de la mitad del enemigo.
            bool vieneCayendo = jugador.VelocidadVertical < 0f;
            bool piesArriba = cuerpo.min.y > centro.y;
            if (vieneCayendo == true && piesArriba == true)
            {
                Aturdir();
                jugador.Rebotar(fuerzaRebote);
                return;
            }

            jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
            return;
        }
    }

    private void Aturdir()
    {
        tiempoAturdidoRestante = segundosAturdido;
        dibujo.color = new Color(0.5f, 0.5f, 0.5f, 0.7f);
    }
}
