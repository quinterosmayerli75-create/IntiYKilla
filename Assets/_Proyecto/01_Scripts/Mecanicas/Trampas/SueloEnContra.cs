// =====================================================================
// SueloEnContra.cs
// Proyecto: Inti y Killa
// Descripción: Un tramo de piso que arrastra hacia un lado a quien camine
//              sobre él, como una cinta. Hace que un salto que parecía fácil
//              se quede corto, o empuja hacia un charco si uno se queda quieto.
//              Las marcas del piso se mueven en la dirección del arrastre:
//              mirándolas se entiende qué pasa (regla de justicia).
// Uso: objeto vacío a la altura del piso; el generador crea las marcas hijas.
// =====================================================================

using UnityEngine;

public class SueloEnContra : TrampaBase
{
    [SerializeField] private float ancho = 4f;
    // Positivo arrastra a la derecha; negativo, a la izquierda.
    [SerializeField] private float velocidadArrastre = -2.5f;
    [SerializeField] private Transform[] marcas;

    private bool yaAviso = false;

    public void Configurar(float nuevoAncho, float nuevaVelocidad, Transform[] nuevasMarcas)
    {
        ancho = nuevoAncho;
        velocidadArrastre = nuevaVelocidad;
        marcas = nuevasMarcas;
    }

    private void Update()
    {
        MoverMarcas();

        // Solo arrastra a quien tiene los pies sobre este tramo.
        Vector2 centroZona = new Vector2(transform.position.x, transform.position.y + 0.3f);
        Vector2 tamanoZona = new Vector2(ancho, 0.6f);
        foreach (ControladorJugador jugador in jugadores)
        {
            if (jugador.EstaVivo == true && jugador.EstaEnElSuelo() == true && JugadorTocaRectangulo(jugador, centroZona, tamanoZona) == true)
            {
                jugador.AplicarEmpuje(velocidadArrastre);
                if (yaAviso == false)
                {
                    yaAviso = true;
                    jugador.RecordarTrampa("Ese piso arrastra: miren hacia dónde corren sus marcas.");
                }
            }
        }
    }

    // Las marcas avanzan y, al llegar al borde, vuelven a entrar por el otro lado.
    private void MoverMarcas()
    {
        if (marcas == null)
        {
            return;
        }

        float bordeIzquierdo = -ancho / 2f;
        foreach (Transform marca in marcas)
        {
            if (marca == null)
            {
                continue;
            }

            float nuevaX = marca.localPosition.x + velocidadArrastre * 0.5f * Time.deltaTime;
            nuevaX = bordeIzquierdo + Mathf.Repeat(nuevaX - bordeIzquierdo, ancho);
            marca.localPosition = new Vector3(nuevaX, marca.localPosition.y, marca.localPosition.z);
        }
    }
}
