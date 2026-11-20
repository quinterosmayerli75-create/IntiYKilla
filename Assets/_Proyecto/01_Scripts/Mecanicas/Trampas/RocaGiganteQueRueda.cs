// =====================================================================
// RocaGiganteQueRueda.cs
// Proyecto: Inti y Killa
// Descripción: Cuando alguien pasa por un punto, aparece detrás de él una
//              roca enorme que tiembla un momento (aviso) y rueda hacia
//              adelante por todo el piso. Hay que correr sin detenerse hasta
//              un hueco o un escalón donde la roca no llega. La roca es más
//              lenta que el personaje: quien no se para, se salva.
// Uso: la dispara un SensorTrampa; el generador indica dónde aparece, hasta
//      dónde rueda y su tamaño.
// =====================================================================

using System.Collections;
using UnityEngine;

public class RocaGiganteQueRueda : TrampaActivable
{
    [SerializeField] private float inicioX = -12f;
    [SerializeField] private float finX = 10f;
    [SerializeField] private float alturaDelPiso = -3.5f;
    [SerializeField] private float diametro = 2.2f;
    [SerializeField] private float velocidad = 4.5f;
    [SerializeField] private float segundosDeAviso = 0.7f;
    [SerializeField] private Color colorRoca = new Color(0.5f, 0.42f, 0.36f);
    [SerializeField] private string mensajeDeMuerte = "Una roca gigante aplastó a {0}. ¡No había que detenerse!";

    private GameObject roca;

    public void Configurar(float nuevoInicioX, float nuevoFinX, float nuevaAlturaDelPiso, float nuevoDiametro)
    {
        inicioX = nuevoInicioX;
        finX = nuevoFinX;
        alturaDelPiso = nuevaAlturaDelPiso;
        diametro = nuevoDiametro;
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        if (causante != null)
        {
            causante.RecordarTrampa("Cuando aparece la roca gigante, hay que correr sin parar.");
        }
        StartCoroutine(AparecerYRodar());
    }

    private IEnumerator AparecerYRodar()
    {
        Vector3 posicionInicial = new Vector3(inicioX, alturaDelPiso + diametro / 2f, 0f);
        roca = new GameObject("Roca gigante");
        roca.transform.SetParent(transform);
        roca.transform.position = posicionInicial;
        roca.transform.localScale = new Vector3(diametro, diametro, 1f);
        SpriteRenderer dibujo = roca.AddComponent<SpriteRenderer>();
        dibujo.sprite = SpriteBase;
        dibujo.color = colorRoca;
        dibujo.sortingOrder = 9;

        // 1) Aviso: tiembla en su lugar.
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            roca.transform.position = posicionInicial + new Vector3(Mathf.Sin(tiempo * 40f) * 0.1f, 0f, 0f);
            yield return null;
        }
        roca.transform.position = posicionInicial;

        // 2) Rueda hasta el final y aplasta a quien toque.
        float direccion = Mathf.Sign(finX - inicioX);
        while ((finX - roca.transform.position.x) * direccion > 0f)
        {
            roca.transform.position = roca.transform.position + new Vector3(direccion * velocidad * Time.deltaTime, 0f, 0f);
            roca.transform.Rotate(0f, 0f, -direccion * velocidad * 60f * Time.deltaTime);
            ControladorJugador aplastado = BuscarJugadorEnRectangulo(roca.transform.position, new Vector2(diametro * 0.8f, diametro * 0.8f));
            if (aplastado != null)
            {
                aplastado.Morir(mensajeDeMuerte.Replace("{0}", aplastado.NombreJugador));
            }
            yield return null;
        }

        // 3) Al llegar al final se deshace: nunca bloquea el camino.
        Destroy(roca);
    }
}
