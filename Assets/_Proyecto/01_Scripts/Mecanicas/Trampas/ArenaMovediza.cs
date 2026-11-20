// =====================================================================
// ArenaMovediza.cs
// Proyecto: Inti y Killa
// Descripción: Un charco que parece agua (Killa cree que puede pasar). En
//              realidad es arena movediza: frena mucho a quien entra y lo va
//              hundiendo; si se queda adentro demasiado, se lo traga. Se sale
//              saltando varias veces. La pista: tiene puntitos de arena.
// Uso: objeto vacío a la altura del piso; el generador dibuja el charco.
// =====================================================================

using UnityEngine;

public class ArenaMovediza : TrampaBase
{
    [SerializeField] private float ancho = 3f;
    [SerializeField] private float factorDeFrenado = 0.35f;
    [SerializeField] private float velocidadDeHundimiento = 0.6f;
    [SerializeField] private float segundosHastaTragarse = 2.2f;
    [SerializeField] private string mensajeDeMuerte = "Ese \"agua\" era arena movediza y se tragó a {0}.";

    private float[] segundosAdentro;

    public void Configurar(float nuevoAncho)
    {
        ancho = nuevoAncho;
    }

    private void FixedUpdate()
    {
        if (segundosAdentro == null || segundosAdentro.Length != jugadores.Length)
        {
            segundosAdentro = new float[jugadores.Length];
        }

        // Zona desde un poco bajo el piso hasta la cintura del personaje.
        Vector2 centro = new Vector2(transform.position.x, transform.position.y + 0.25f);
        Vector2 tamano = new Vector2(ancho, 0.9f);

        for (int indice = 0; indice < jugadores.Length; indice++)
        {
            ControladorJugador jugador = jugadores[indice];
            bool estaAdentro = jugador.EstaVivo == true && JugadorTocaRectangulo(jugador, centro, tamano) == true;
            if (estaAdentro == false)
            {
                segundosAdentro[indice] = 0f;
                continue;
            }

            if (segundosAdentro[indice] == 0f)
            {
                jugador.RecordarTrampa("No era agua: era arena movediza. Sal saltando enseguida.");
            }
            segundosAdentro[indice] = segundosAdentro[indice] + Time.fixedDeltaTime;

            // Frena la caminata y, si no está saltando, lo hunde despacio.
            Rigidbody2D cuerpo = jugador.GetComponent<Rigidbody2D>();
            Vector2 velocidad = cuerpo.linearVelocity;
            float velocidadY = velocidad.y;
            if (velocidadY <= 0f)
            {
                velocidadY = -velocidadDeHundimiento;
            }
            else
            {
                velocidadY = velocidadY * 0.8f;
            }
            cuerpo.linearVelocity = new Vector2(velocidad.x * factorDeFrenado, velocidadY);

            if (segundosAdentro[indice] >= segundosHastaTragarse)
            {
                jugador.Morir(mensajeDeMuerte.Replace("{0}", jugador.NombreJugador));
            }
        }
    }
}
