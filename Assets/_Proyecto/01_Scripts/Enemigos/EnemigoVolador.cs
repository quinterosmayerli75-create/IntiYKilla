// =====================================================================
// EnemigoVolador.cs
// Proyecto: Inti y Killa
// Descripción: Vuela en zigzag o en círculo alrededor de su punto inicial
//              (murciélagos, avispas). Su vuelo siempre se repite igual: quien
//              lo observa aprende cuándo pasar. Se puede aturdir pisándolo.
// =====================================================================

using UnityEngine;

public enum TipoVuelo
{
    Zigzag,
    Circulo
}

public class EnemigoVolador : EnemigoBase
{
    [Header("Vuelo")]
    [SerializeField] private TipoVuelo tipoVuelo = TipoVuelo.Zigzag;
    [SerializeField] private Vector2 amplitud = new Vector2(2.5f, 1f);
    // Segundos que tarda una vuelta completa.
    [SerializeField] private float segundosPorVuelta = 5f;

    private Vector3 puntoCentral;
    private float tiempoDeVuelo = 0f;

    public void Configurar(TipoVuelo nuevoTipo, Vector2 nuevaAmplitud, float nuevosSegundos)
    {
        tipoVuelo = nuevoTipo;
        amplitud = nuevaAmplitud;
        segundosPorVuelta = nuevosSegundos;
    }

    protected override void Awake()
    {
        base.Awake();
        puntoCentral = transform.position;
    }

    protected override void MoverEnemigo()
    {
        // El tiempo de vuelo se detiene mientras está aturdido: retoma donde quedó.
        tiempoDeVuelo = tiempoDeVuelo + Time.deltaTime;
        float angulo = tiempoDeVuelo * Mathf.PI * 2f / segundosPorVuelta;

        Vector3 desplazamiento;
        if (tipoVuelo == TipoVuelo.Circulo)
        {
            desplazamiento = new Vector3(Mathf.Cos(angulo) * amplitud.x, Mathf.Sin(angulo) * amplitud.y, 0f);
        }
        else
        {
            // Vuela bajo en los extremos y alto en el centro.
            desplazamiento = new Vector3(Mathf.Sin(angulo) * amplitud.x, Mathf.Cos(angulo * 2f) * amplitud.y, 0f);
        }

        transform.position = puntoCentral + desplazamiento;
    }
}
