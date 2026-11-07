// =====================================================================
// ProyectilArco.cs
// Proyecto: Inti y Killa
// Descripción: Objeto lanzado en arco (coco, escupitajo). Mata a quien toque y
//              desaparece al chocar con algo sólido. Lo crea LanzadorProyectiles.
// =====================================================================

using UnityEngine;

public class ProyectilArco : MonoBehaviour
{
    // No choca recién lanzado: si no, se rompería contra el piso de quien lo lanza.
    private const float segundosSinChocar = 0.25f;
    private const float alturaParaDesaparecer = -12f;

    private ControladorJugador[] jugadores;
    private Vector2 velocidad;
    private float gravedad;
    private float radio;
    private string mensajeDeMuerte;
    private float tiempo = 0f;
    // Si es verdadero, solo daña a quien tiene el elemento contrario al del proyectil.
    private bool daniaSoloAlContrario = false;
    private TipoElemento elementoDelProyectil = TipoElemento.Fuego;

    public void DaniarSoloAlContrarioDe(TipoElemento elemento)
    {
        daniaSoloAlContrario = true;
        elementoDelProyectil = elemento;
    }

    public void Iniciar(Vector2 nuevaVelocidad, float nuevaGravedad, float nuevoRadio, string nuevoMensaje)
    {
        velocidad = nuevaVelocidad;
        gravedad = nuevaGravedad;
        radio = nuevoRadio;
        mensajeDeMuerte = nuevoMensaje;
        jugadores = FindObjectsByType<ControladorJugador>();
    }

    private void Update()
    {
        tiempo = tiempo + Time.deltaTime;
        velocidad.y = velocidad.y - gravedad * Time.deltaTime;
        transform.position = transform.position + (Vector3)(velocidad * Time.deltaTime);

        Vector2 centro = transform.position;
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador == null || jugador.EstaVivo == false)
            {
                continue;
            }

            // El fuego no le hace nada a quien tiene el fuego, y el agua tampoco al agua.
            if (daniaSoloAlContrario == true && jugador.Elemento == elementoDelProyectil)
            {
                continue;
            }

            Bounds cuerpo = jugador.LimitesCuerpo;
            bool cruzaEnX = cuerpo.max.x > centro.x - radio && cuerpo.min.x < centro.x + radio;
            bool cruzaEnY = cuerpo.max.y > centro.y - radio && cuerpo.min.y < centro.y + radio;
            if (cruzaEnX == true && cruzaEnY == true)
            {
                jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
                Destroy(gameObject);
                return;
            }
        }

        if (centro.y < alturaParaDesaparecer || ChocoConAlgoSolido(centro) == true)
        {
            Destroy(gameObject);
        }
    }

    private bool ChocoConAlgoSolido(Vector2 centro)
    {
        if (tiempo < segundosSinChocar)
        {
            return false;
        }

        Collider2D[] objetos = Physics2D.OverlapPointAll(centro);
        foreach (Collider2D objeto in objetos)
        {
            bool esUnJugador = objeto.GetComponent<ControladorJugador>() != null;
            if (objeto.isTrigger == false && esUnJugador == false)
            {
                return true;
            }
        }

        return false;
    }
}
