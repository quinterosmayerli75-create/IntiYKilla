// =====================================================================
// CuerdaCharango.cs
// Proyecto: Inti y Killa
// Descripción: Cuerda gigante de charango (Aiquile). Funciona como trampolín
//              y, al rebotar, toca su nota: cada nota tiene su color, y las
//              puertas melódicas escuchan qué notas se tocan y en qué orden.
//              Algunas cuerdas solo suenan de día o solo de noche: el resto
//              del tiempo se ven apagadas y no hacen nada.
// Uso: se apoya sobre un piso. No necesita colisionador: revisa si los pies
//      de un jugador que no va subiendo tocan su zona.
// =====================================================================

using UnityEngine;

public enum MomentoCuerda
{
    Siempre,
    SoloDeDia,
    SoloDeNoche
}

[RequireComponent(typeof(SpriteRenderer))]
public class CuerdaCharango : MonoBehaviour
{
    // Las puertas melódicas se suscriben aquí para enterarse de cada nota.
    public static event System.Action<int> AlSonarNota;

    // Un color por nota (do, re, mi, fa, sol), siempre en el mismo orden.
    private static readonly Color[] coloresNotas = new Color[]
    {
        new Color(0.95f, 0.30f, 0.30f),
        new Color(1.00f, 0.62f, 0.20f),
        new Color(1.00f, 0.90f, 0.25f),
        new Color(0.35f, 0.85f, 0.40f),
        new Color(0.35f, 0.70f, 1.00f)
    };

    [SerializeField] private int nota = 0;
    [SerializeField] private MomentoCuerda momento = MomentoCuerda.Siempre;
    [SerializeField] private float fuerzaRebote = 21f;
    [SerializeField] private Vector2 tamanoZona = new Vector2(1.4f, 0.35f);
    // Para cuando haya sonido: la nota que suena al rebotar.
    [SerializeField] private AudioClip sonidoNota;

    private SpriteRenderer dibujo;
    private ControladorJugador[] jugadores;
    private Vector3 escalaNormal;
    private float tiempoVibrando = 0f;

    public int Nota
    {
        get { return nota; }
    }

    public static Color ObtenerColorNota(int numeroNota)
    {
        int indice = Mathf.Clamp(numeroNota, 0, coloresNotas.Length - 1);
        return coloresNotas[indice];
    }

    public void Configurar(int nuevaNota, MomentoCuerda nuevoMomento, Vector2 nuevoTamanoZona)
    {
        nota = nuevaNota;
        momento = nuevoMomento;
        tamanoZona = nuevoTamanoZona;
    }

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        jugadores = FindObjectsByType<ControladorJugador>();
        escalaNormal = transform.localScale;
    }

    public bool EstaActiva()
    {
        if (momento == MomentoCuerda.Siempre || CicloDiaNoche.Instancia == null)
        {
            return true;
        }

        bool esDeDia = CicloDiaNoche.Instancia.EsDeDia;
        if (momento == MomentoCuerda.SoloDeDia)
        {
            return esDeDia;
        }
        return esDeDia == false;
    }

    private void Update()
    {
        bool activa = EstaActiva();
        Color color = ObtenerColorNota(nota);
        if (activa == false)
        {
            color.a = 0.25f;
        }
        dibujo.color = color;

        if (activa == true)
        {
            RevisarJugadores();
        }

        // Vibra un momento después de sonar.
        tiempoVibrando = Mathf.Max(0f, tiempoVibrando - Time.deltaTime);
        float vibracion = 1f + Mathf.Sin(Time.time * 80f) * 0.6f * tiempoVibrando;
        transform.localScale = new Vector3(escalaNormal.x, escalaNormal.y * vibracion, escalaNormal.z);
    }

    private void RevisarJugadores()
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
                Sonar(jugador);
            }
        }
    }

    private void Sonar(ControladorJugador jugador)
    {
        jugador.Rebotar(fuerzaRebote);
        tiempoVibrando = 0.4f;

        if (sonidoNota != null)
        {
            AudioSource.PlayClipAtPoint(sonidoNota, transform.position);
        }

        if (AlSonarNota != null)
        {
            AlSonarNota(nota);
        }
    }
}
