// =====================================================================
// CartelQueCae.cs
// Proyecto: Inti y Killa
// Descripción: Un letrero (por ejemplo "META" o "CUIDADO") que parece solo
//              un adorno. Cuando alguien pasa por debajo, sus letras se
//              desprenden una por una y caen como bloques pesados. Antes de
//              caer, cada letra tiembla (aviso). Al llegar al piso desaparecen.
// Uso: el generador crea un hijo por letra (con su dibujo y su texto).
// =====================================================================

using System.Collections;
using UnityEngine;

public class CartelQueCae : TrampaActivable
{
    [SerializeField] private Transform[] letras;
    [SerializeField] private float alturaDelPiso = -3.5f;
    [SerializeField] private float segundosEntreLetras = 0.35f;
    [SerializeField] private float segundosDeTemblor = 0.4f;
    [SerializeField] private float velocidadCaida = 15f;
    [SerializeField] private Vector2 tamanoLetra = new Vector2(0.8f, 0.8f);
    [SerializeField] private string mensajeDeMuerte = "Una letra del cartel le cayó encima a {0}.";

    public void Configurar(Transform[] nuevasLetras, float nuevaAlturaDelPiso)
    {
        letras = nuevasLetras;
        alturaDelPiso = nuevaAlturaDelPiso;
    }

    private void Update()
    {
        if (SeActivaSola == false || YaActivada == true || letras == null || letras.Length == 0)
        {
            return;
        }

        // Zona de disparo: debajo de todo el cartel, desde el piso hasta él.
        float izquierda = letras[0].position.x - tamanoLetra.x;
        float derecha = letras[letras.Length - 1].position.x + tamanoLetra.x;
        float alto = transform.position.y - alturaDelPiso;
        Vector2 centro = new Vector2((izquierda + derecha) / 2f, alturaDelPiso + alto / 2f);
        ControladorJugador jugador = BuscarJugadorEnRectangulo(centro, new Vector2(derecha - izquierda, alto));
        if (jugador != null)
        {
            Activar(jugador);
        }
    }

    protected override void AlActivarse(ControladorJugador causante)
    {
        if (causante != null)
        {
            causante.RecordarTrampa("Las letras del cartel tiemblan antes de caer.");
        }
        // Las letras empiezan a caer por el lado por donde entró el jugador: así
        // la "ola" de letras va detrás de él, y quien no se detiene alcanza a pasar.
        bool empezarPorLaDerecha = causante != null && causante.transform.position.x > transform.position.x;
        StartCoroutine(SoltarLetras(empezarPorLaDerecha));
    }

    private IEnumerator SoltarLetras(bool empezarPorLaDerecha)
    {
        for (int paso = 0; paso < letras.Length; paso++)
        {
            int indice = paso;
            if (empezarPorLaDerecha == true)
            {
                indice = letras.Length - 1 - paso;
            }
            StartCoroutine(TemblarYCaer(letras[indice]));
            yield return new WaitForSeconds(segundosEntreLetras);
        }
    }

    private IEnumerator TemblarYCaer(Transform letra)
    {
        Vector3 posicionInicial = letra.position;
        float tiempo = 0f;
        while (tiempo < segundosDeTemblor)
        {
            tiempo = tiempo + Time.deltaTime;
            letra.position = posicionInicial + new Vector3(Mathf.Sin(tiempo * 60f) * 0.05f, 0f, 0f);
            yield return null;
        }
        letra.position = posicionInicial;

        float alturaFinal = alturaDelPiso + tamanoLetra.y / 2f;
        while (letra.position.y > alturaFinal)
        {
            letra.position = letra.position + new Vector3(0f, -velocidadCaida * Time.deltaTime, 0f);
            ControladorJugador aplastado = BuscarJugadorEnRectangulo(letra.position, tamanoLetra * 0.85f);
            if (aplastado != null)
            {
                aplastado.Morir(mensajeDeMuerte.Replace("{0}", aplastado.NombreJugador));
            }
            yield return null;
        }

        // Nunca queda tapando el camino.
        letra.gameObject.SetActive(false);
    }
}
