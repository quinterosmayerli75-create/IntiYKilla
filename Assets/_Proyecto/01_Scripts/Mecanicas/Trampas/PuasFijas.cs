// =====================================================================
// PuasFijas.cs
// Proyecto: Inti y Killa
// Descripción: Púas siempre visibles (por ejemplo en el techo). No engañan
//              solas: son el castigo de otra trampa, como el trampolín
//              catapulta que lanza al jugador contra ellas.
// Uso: objeto vacío; el generador dibuja las púas como hijos.
// =====================================================================

using UnityEngine;

public class PuasFijas : TrampaBase
{
    [SerializeField] private Vector2 tamanoZona = new Vector2(3f, 0.5f);
    [SerializeField] private string mensajeDeMuerte = "{0} salió volando contra las púas del techo.";

    public void Configurar(Vector2 nuevoTamano, string nuevoMensaje)
    {
        tamanoZona = nuevoTamano;
        mensajeDeMuerte = nuevoMensaje;
    }

    private void Update()
    {
        ControladorJugador herido = BuscarJugadorEnRectangulo(transform.position, tamanoZona);
        if (herido != null)
        {
            herido.Morir(mensajeDeMuerte.Replace("{0}", herido.NombreJugador));
        }
    }
}
