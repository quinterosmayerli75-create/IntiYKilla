// =====================================================================
// NubeAsesina.cs
// Proyecto: Inti y Killa
// Descripción: Una nube que parece decoración del fondo. Cuando alguien pasa
//              debajo, se oscurece y tiembla (aviso) y suelta un rayo que
//              baja hasta el piso. Después vuelve a ser una nube tranquila y
//              se puede disparar otra vez: siempre igual, para aprenderla.
// Uso: objeto con SpriteRenderer (la nube); el generador indica la altura del
//      piso donde cae el rayo.
// =====================================================================

using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NubeAsesina : TrampaActivable
{
    [SerializeField] private float alturaDelPiso = -3.5f;
    [SerializeField] private float anchoDelRayo = 1f;
    [SerializeField] private float segundosDeAviso = 0.6f;
    [SerializeField] private float segundosDeRayo = 0.35f;
    [SerializeField] private float segundosDeDescanso = 1.5f;
    [SerializeField] private Color colorDeAviso = new Color(0.35f, 0.35f, 0.45f);
    [SerializeField] private string mensajeDeMuerte = "Esa nube no era de adorno: un rayo alcanzó a {0}.";

    private SpriteRenderer dibujo;
    private Color colorNormal;
    private Vector3 posicionOriginal;
    private GameObject rayo;
    private bool rayoActivo = false;

    public void Configurar(float nuevaAlturaDelPiso)
    {
        alturaDelPiso = nuevaAlturaDelPiso;
    }

    protected override void Awake()
    {
        base.Awake();
        dibujo = GetComponent<SpriteRenderer>();
        colorNormal = dibujo.color;
        posicionOriginal = transform.position;
    }

    private void Update()
    {
        Vector2 centroColumna = ObtenerCentroColumna();
        Vector2 tamanoColumna = new Vector2(anchoDelRayo, posicionOriginal.y - alturaDelPiso);

        if (rayoActivo == true)
        {
            ControladorJugador alcanzado = BuscarJugadorEnRectangulo(centroColumna, tamanoColumna);
            if (alcanzado != null)
            {
                alcanzado.Morir(mensajeDeMuerte.Replace("{0}", alcanzado.NombreJugador));
            }
            return;
        }

        if (SeActivaSola == false || YaActivada == true)
        {
            return;
        }

        ControladorJugador jugador = BuscarJugadorEnRectangulo(centroColumna, tamanoColumna);
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    private Vector2 ObtenerCentroColumna()
    {
        float alto = posicionOriginal.y - alturaDelPiso;
        return new Vector2(posicionOriginal.x, alturaDelPiso + alto / 2f);
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        if (causante != null)
        {
            causante.RecordarTrampa("Cuando la nube se oscurece, hay que salir de abajo.");
        }
        StartCoroutine(AvisarYTronar());
    }

    private IEnumerator AvisarYTronar()
    {
        // 1) Aviso: la nube se oscurece y tiembla.
        dibujo.color = colorDeAviso;
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            float temblor = Mathf.Sin(tiempo * 50f) * 0.08f;
            transform.position = posicionOriginal + new Vector3(temblor, 0f, 0f);
            yield return null;
        }
        transform.position = posicionOriginal;

        // 2) El rayo: una columna amarilla que llega hasta el piso.
        float alto = posicionOriginal.y - alturaDelPiso;
        rayo = new GameObject("Rayo");
        rayo.transform.SetParent(transform);
        rayo.transform.position = new Vector3(posicionOriginal.x, alturaDelPiso + alto / 2f, 0f);
        Vector3 escalaPadre = transform.lossyScale;
        rayo.transform.localScale = new Vector3(anchoDelRayo * 0.6f / escalaPadre.x, alto / escalaPadre.y, 1f);
        SpriteRenderer dibujoRayo = rayo.AddComponent<SpriteRenderer>();
        dibujoRayo.sprite = SpriteBase;
        dibujoRayo.color = new Color(1f, 0.95f, 0.4f, 0.9f);
        dibujoRayo.sortingOrder = 12;
        rayoActivo = true;
        yield return new WaitForSeconds(segundosDeRayo);

        // 3) Vuelve a ser una nube tranquila; descansa y se puede disparar otra vez.
        rayoActivo = false;
        Destroy(rayo);
        dibujo.color = colorNormal;
        yield return new WaitForSeconds(segundosDeDescanso);
        Rearmar();
    }
}
