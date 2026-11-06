// =====================================================================
// AguaCongelable.cs
// Proyecto: Inti y Killa
// Descripción: Charco que se puede congelar. Quien tiene el poder del agua
//              lo congela con la tecla de habilidad, y por unos segundos el
//              charco es hielo firme: su compañero de fuego puede cruzarlo.
//              Quien tiene el poder del fuego puede derretirlo antes de tiempo.
//              Parpadea antes de derretirse (regla de justicia).
// Uso: en un charco de agua que ya tenga ZonaElemental y su trigger. Como el
//      charco está apoyado sobre un piso, para que sea firme basta con apagar
//      el trigger que mata: no hace falta agregar otro colisionador.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class AguaCongelable : MonoBehaviour
{
    [SerializeField] private float segundosCongelada = 5f;
    [SerializeField] private float segundosDeAviso = 1.2f;
    // Distancia al charco desde la que se puede usar la habilidad.
    [SerializeField] private float alcance = 1.2f;
    [SerializeField] private Color colorHielo = new Color(0.72f, 0.93f, 1f);

    private SpriteRenderer dibujo;
    private Collider2D zonaQueMata;
    private ControladorJugador[] jugadores;
    private Color colorAgua;
    private float tiempoRestante = 0f;

    public bool EstaCongelada
    {
        get { return tiempoRestante > 0f; }
    }

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        zonaQueMata = GetComponent<Collider2D>();
        jugadores = FindObjectsByType<ControladorJugador>();
        colorAgua = dibujo.color;
    }

    private void Update()
    {
        RevisarJugadores();

        if (EstaCongelada == false)
        {
            return;
        }

        tiempoRestante = tiempoRestante - Time.deltaTime;

        if (tiempoRestante <= 0f)
        {
            Derretir();
        }
        else if (tiempoRestante < segundosDeAviso)
        {
            bool mostrarAgua = Mathf.Repeat(tiempoRestante * 6f, 1f) < 0.5f;
            if (mostrarAgua == true)
            {
                dibujo.color = colorAgua;
            }
            else
            {
                dibujo.color = colorHielo;
            }
        }
    }

    private void RevisarJugadores()
    {
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.PresionoHabilidadEsteFrame() == true && EstaAlAlcance(jugador) == true)
            {
                if (jugador.Elemento == TipoElemento.Agua)
                {
                    Congelar();
                }
                else if (EstaCongelada == true)
                {
                    Derretir();
                }
            }
        }
    }

    private bool EstaAlAlcance(ControladorJugador jugador)
    {
        // Se usan los límites del dibujo porque el collider se apaga con el hielo.
        Bounds charco = dibujo.bounds;
        Bounds cuerpo = jugador.LimitesCuerpo;
        bool cercaEnX = cuerpo.max.x > charco.min.x - alcance && cuerpo.min.x < charco.max.x + alcance;
        bool cercaEnY = Mathf.Abs(cuerpo.min.y - charco.min.y) < alcance;
        return cercaEnX == true && cercaEnY == true;
    }

    private void Congelar()
    {
        tiempoRestante = segundosCongelada;
        zonaQueMata.enabled = false;
        dibujo.color = colorHielo;
    }

    private void Derretir()
    {
        tiempoRestante = 0f;
        zonaQueMata.enabled = true;
        dibujo.color = colorAgua;
    }
}
