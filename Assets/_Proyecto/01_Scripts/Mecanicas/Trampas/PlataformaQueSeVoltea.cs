// =====================================================================
// PlataformaQueSeVoltea.cs
// Proyecto: Inti y Killa
// Descripción: Plataforma que parece firme. Cuando alguien se para encima,
//              tiembla un momento (aviso) y se da vuelta: quien sigue encima
//              cae. Después de un rato vuelve a su lugar y puede voltearse
//              otra vez. Quien la conoce la cruza sin detenerse.
// Uso: plataforma con BoxCollider2D (el dibujo puede estar en piezas hijas).
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlataformaQueSeVoltea : TrampaActivable
{
    [SerializeField] private float segundosDeAviso = 0.45f;
    [SerializeField] private float intensidadTemblor = 0.06f;
    [SerializeField] private float segundosDelGiro = 0.2f;
    [SerializeField] private float segundosVolteada = 1.6f;
    [SerializeField] private string mensajeSiMuere = "La plataforma se dio vuelta con {0} encima.";

    private BoxCollider2D colisionador;
    private Vector3 posicionOriginal;
    private Vector2 centroZona;
    private Vector2 tamanoZona;

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        posicionOriginal = transform.position;

        Bounds limites = colisionador.bounds;
        tamanoZona = new Vector2(limites.size.x, 0.15f);
        centroZona = new Vector2(limites.center.x, limites.max.y + 0.05f);
    }

    private void Update()
    {
        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroZona, tamanoZona);
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(TemblarVoltearYVolver(causante));
    }

    private IEnumerator TemblarVoltearYVolver(ControladorJugador causante)
    {
        // Aviso: tiembla, como las rocas que caen.
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            Vector2 temblor = Random.insideUnitCircle * intensidadTemblor;
            transform.position = posicionOriginal + new Vector3(temblor.x, temblor.y, 0f);
            yield return null;
        }
        transform.position = posicionOriginal;

        // Si muere poco después, el mensaje será el de esta plataforma.
        if (causante != null)
        {
            causante.RecordarTrampa(mensajeSiMuere.Replace("{0}", causante.NombreJugador));
        }

        colisionador.enabled = false;
        yield return StartCoroutine(Girar(0f, 90f));

        yield return new WaitForSeconds(segundosVolteada);

        yield return StartCoroutine(Girar(90f, 0f));
        colisionador.enabled = true;
        Rearmar();
    }

    // Gira sobre su eje largo: de frente parece que se pone de canto.
    private IEnumerator Girar(float gradosInicio, float gradosFinal)
    {
        float tiempo = 0f;
        while (tiempo < segundosDelGiro)
        {
            tiempo = tiempo + Time.deltaTime;
            float avance = Mathf.Clamp01(tiempo / segundosDelGiro);
            float grados = Mathf.Lerp(gradosInicio, gradosFinal, avance);
            transform.rotation = Quaternion.Euler(grados, 0f, 0f);
            yield return null;
        }
        transform.rotation = Quaternion.Euler(gradosFinal, 0f, 0f);
    }
}
