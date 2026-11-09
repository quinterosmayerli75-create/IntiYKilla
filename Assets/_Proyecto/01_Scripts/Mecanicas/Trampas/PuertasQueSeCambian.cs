// =====================================================================
// PuertasQueSeCambian.cs
// Proyecto: Inti y Killa
// Descripción: Las dos puertas de salida se cambian de lugar la primera vez
//              que alguien se acerca a una de ellas. Inti y Killa tienen que
//              cruzarse para llegar cada uno a la suya. Ocurre una sola vez:
//              quien ya lo sabe llega por el lado correcto desde el inicio.
// Uso: objeto vacío en la escena, con las dos puertas asignadas.
// =====================================================================

using System.Collections;
using UnityEngine;

public class PuertasQueSeCambian : TrampaBase
{
    [SerializeField] private Transform puertaUno;
    [SerializeField] private Transform puertaDos;
    [SerializeField] private float distanciaDeDeteccion = 2.5f;
    [SerializeField] private float segundosDelCambio = 0.6f;
    [SerializeField] private float alturaDelSalto = 1.5f;

    private bool yaSeCambiaron = false;

    public void Configurar(Transform nuevaPuertaUno, Transform nuevaPuertaDos)
    {
        puertaUno = nuevaPuertaUno;
        puertaDos = nuevaPuertaDos;
    }

    private void Update()
    {
        if (yaSeCambiaron == true || puertaUno == null || puertaDos == null)
        {
            return;
        }

        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null || jugador.EstaVivo == false)
            {
                continue;
            }

            Vector2 posicionJugador = jugador.transform.position;
            float distanciaAUno = Vector2.Distance(posicionJugador, puertaUno.position);
            float distanciaADos = Vector2.Distance(posicionJugador, puertaDos.position);
            if (distanciaAUno <= distanciaDeDeteccion || distanciaADos <= distanciaDeDeteccion)
            {
                yaSeCambiaron = true;
                StartCoroutine(CambiarDeLugar());
                return;
            }
        }
    }

    // Las puertas saltan en arco por encima de los jugadores, así se ve
    // claramente que se cambiaron (regla de justicia).
    private IEnumerator CambiarDeLugar()
    {
        Vector3 inicioUno = puertaUno.position;
        Vector3 inicioDos = puertaDos.position;
        float tiempo = 0f;

        while (tiempo < segundosDelCambio)
        {
            tiempo = tiempo + Time.deltaTime;
            float avance = Mathf.Clamp01(tiempo / segundosDelCambio);
            float arco = Mathf.Sin(avance * Mathf.PI) * alturaDelSalto;

            Vector3 posicionUno = Vector3.Lerp(inicioUno, inicioDos, avance);
            Vector3 posicionDos = Vector3.Lerp(inicioDos, inicioUno, avance);
            puertaUno.position = new Vector3(posicionUno.x, posicionUno.y + arco, posicionUno.z);
            puertaDos.position = new Vector3(posicionDos.x, posicionDos.y + arco, posicionDos.z);
            yield return null;
        }

        puertaUno.position = inicioDos;
        puertaDos.position = inicioUno;
    }
}
