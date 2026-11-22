// =====================================================================
// VolverAlPerder.cs
// Proyecto: Inti y Killa
// Descripción: Castigo del nivel final: si alguien muere (o se reinicia),
//              no se repite el mismo nivel, sino que se vuelve a otra escena
//              (en el 6-5, al comienzo de la Isla del Sol, el 6-1). Así el
//              final es tenso: hay que llegar al Guardián sin perder.
// Uso: un solo objeto en la escena; GestorNivel lo consulta al reiniciar.
// =====================================================================

using UnityEngine;

public class VolverAlPerder : MonoBehaviour
{
    [SerializeField] private string escenaDestino = "Mundo6_Nivel1";
    [SerializeField] private string mensaje = "El eclipse los devolvió al comienzo de la Isla del Sol.";

    private static VolverAlPerder instancia;

    public static VolverAlPerder Instancia
    {
        get { return instancia; }
    }

    public string EscenaDestino
    {
        get { return escenaDestino; }
    }

    public string Mensaje
    {
        get { return mensaje; }
    }

    public void Configurar(string nuevaEscena, string nuevoMensaje)
    {
        escenaDestino = nuevaEscena;
        mensaje = nuevoMensaje;
    }

    private void Awake()
    {
        instancia = this;
    }

    private void OnDestroy()
    {
        if (instancia == this)
        {
            instancia = null;
        }
    }
}
