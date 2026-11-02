// =====================================================================
// PlataformaOculta.cs
// Proyecto: Inti y Killa
// Descripción: Plataforma de cristal que solo existe dentro de la luz de
//              Inti (o de quien tenga el fuego). Fuera de la luz se ve
//              como un reflejo tenue y NO se puede pisar.
//              Cuando la luz se aleja, espera un instante antes de
//              desaparecer, para que el compañero alcance a saltar.
// Uso: bloque con SpriteRenderer + Collider2D. Funciona junto a
//      LuzPersonaje (que debe estar en los jugadores).
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PlataformaOculta : MonoBehaviour
{
    [Header("Apariencia")]
    // Transparencia cuando está oculta (0 = invisible, 1 = sólida).
    // Se deja un poco visible como pista de que ahí hay algo.
    [SerializeField] private float transparenciaOculta = 0.12f;
    [SerializeField] private float velocidadFundido = 8f;

    [Header("Tiempo de gracia")]
    // Segundos que sigue sólida después de salir de la luz.
    [SerializeField] private float tiempoAntesDeOcultarse = 0.4f;

    private SpriteRenderer dibujo;
    private Collider2D colisionador;
    private Color colorBase;
    private float contadorParaOcultarse = 0f;

    private void Awake()
    {
        dibujo = GetComponent<SpriteRenderer>();
        colisionador = GetComponent<Collider2D>();
        colorBase = dibujo.color;
    }

    private void Start()
    {
        // Empieza oculta; la luz la revelará en el primer frame si está cerca.
        colisionador.enabled = false;
        CambiarTransparencia(transparenciaOculta);
    }

    private void Update()
    {
        // Se usan los límites del dibujo porque los del collider desaparecen
        // cuando el collider está desactivado.
        bool estaEnLaLuz = LuzPersonaje.EstaIluminado(dibujo.bounds);

        if (estaEnLaLuz == true)
        {
            contadorParaOcultarse = tiempoAntesDeOcultarse;
        }
        else
        {
            contadorParaOcultarse = contadorParaOcultarse - Time.deltaTime;
        }

        bool debeSerSolida = contadorParaOcultarse > 0f;
        colisionador.enabled = debeSerSolida;

        // La transparencia cambia de a poco para que aparezca y se desvanezca suave.
        float transparenciaObjetivo = transparenciaOculta;
        if (debeSerSolida == true)
        {
            transparenciaObjetivo = 1f;
        }

        float transparenciaNueva = Mathf.MoveTowards(dibujo.color.a, transparenciaObjetivo, velocidadFundido * Time.deltaTime);
        CambiarTransparencia(transparenciaNueva);
    }

    private void CambiarTransparencia(float transparencia)
    {
        dibujo.color = new Color(colorBase.r, colorBase.g, colorBase.b, colorBase.a * transparencia);
    }
}
