// =====================================================================
// ParedAplastadora.cs
// Proyecto: Inti y Killa
// Descripción: Una pared que de pronto avanza (de lado o hacia abajo) y
//              aplasta a quien encuentre en su camino. Antes de moverse
//              tiembla (regla de justicia). Después se queda quieta un rato,
//              vuelve a su lugar y se puede volver a disparar: siempre igual,
//              para que se aprenda cuándo pasar.
// Uso: plataforma con BoxCollider2D; se dispara sola cuando alguien entra a
//      su recorrido, o con un SensorTrampa.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ParedAplastadora : TrampaActivable
{
    // Hacia dónde y cuánto avanza desde su lugar.
    [SerializeField] private Vector2 recorrido = new Vector2(3f, 0f);
    [SerializeField] private float segundosDeTemblor = 0.5f;
    [SerializeField] private float velocidadAvance = 12f;
    [SerializeField] private float velocidadRegreso = 2f;
    [SerializeField] private float segundosAvanzada = 1f;
    [SerializeField] private float intensidadTemblor = 0.06f;
    [SerializeField] private string mensajeDeMuerte = "La pared se movió y aplastó a {0}.";

    private BoxCollider2D colisionador;
    private Vector3 posicionOriginal;
    private Vector2 tamanoPared;
    private bool estaAvanzando = false;

    public void Configurar(Vector2 nuevoRecorrido)
    {
        recorrido = nuevoRecorrido;
    }

    protected override void Awake()
    {
        base.Awake();
        colisionador = GetComponent<BoxCollider2D>();
        posicionOriginal = transform.position;
        tamanoPared = colisionador.bounds.size;
    }

    private void Update()
    {
        if (estaAvanzando == true)
        {
            // Un poco más chica que la pared: solo cuenta un golpe de verdad.
            ControladorJugador aplastado = BuscarJugadorEnRectangulo(transform.position, tamanoPared * 0.9f);
            if (aplastado != null)
            {
                aplastado.Morir(mensajeDeMuerte.Replace("{0}", aplastado.NombreJugador));
            }
            return;
        }

        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        // Zona de disparo: el espacio que la pared barre al avanzar.
        Vector2 inicio = posicionOriginal;
        Vector2 destino = inicio + recorrido;
        Vector2 centroBarrido = (inicio + destino) / 2f;
        Vector2 tamanoBarrido = new Vector2(Mathf.Abs(recorrido.x) + tamanoPared.x, Mathf.Abs(recorrido.y) + tamanoPared.y);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroBarrido, tamanoBarrido);
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        StartCoroutine(TemblarAvanzarYVolver());
    }

    private IEnumerator TemblarAvanzarYVolver()
    {
        // 1) Aviso: tiembla en su lugar.
        float tiempo = 0f;
        while (tiempo < segundosDeTemblor)
        {
            tiempo = tiempo + Time.deltaTime;
            Vector2 temblor = Random.insideUnitCircle * intensidadTemblor;
            transform.position = posicionOriginal + new Vector3(temblor.x, temblor.y, 0f);
            yield return null;
        }
        transform.position = posicionOriginal;

        // 2) Avanza rápido, sin collider para no empujar: aplasta por contacto.
        colisionador.enabled = false;
        estaAvanzando = true;
        Vector3 destino = posicionOriginal + new Vector3(recorrido.x, recorrido.y, 0f);
        while (Vector3.Distance(transform.position, destino) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, velocidadAvance * Time.deltaTime);
            yield return null;
        }
        estaAvanzando = false;
        colisionador.enabled = true;
        yield return new WaitForSeconds(segundosAvanzada);

        // 3) Regresa despacio (ya no hace daño) y queda lista otra vez.
        while (Vector3.Distance(transform.position, posicionOriginal) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, posicionOriginal, velocidadRegreso * Time.deltaTime);
            yield return null;
        }
        transform.position = posicionOriginal;
        Rearmar();
    }
}
