// =====================================================================
// Gema.cs
// Proyecto: Inti y Killa
// Descripción: Gema que solo puede recoger el personaje de su mismo
//              elemento (naranja = Inti, azul = Killa). Flota suavemente
//              arriba y abajo para llamar la atención.
// =====================================================================

using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Gema : MonoBehaviour
{
    [SerializeField] private TipoElemento elemento = TipoElemento.Fuego;

    [Header("Animación de flotar")]
    [SerializeField] private float alturaFlotacion = 0.15f;
    [SerializeField] private float velocidadFlotacion = 3f;

    private Vector3 posicionInicial;

    public TipoElemento Elemento
    {
        get { return elemento; }
    }

    public void ConfigurarGema(TipoElemento nuevoElemento)
    {
        elemento = nuevoElemento;
    }

    private void Start()
    {
        posicionInicial = transform.position;
    }

    private void Update()
    {
        // Mathf.Sin genera una onda que sube y baja entre -1 y 1.
        float desplazamiento = Mathf.Sin(Time.time * velocidadFlotacion) * alturaFlotacion;
        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + desplazamiento, posicionInicial.z);
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        ControladorJugador jugador = otro.GetComponent<ControladorJugador>();
        if (jugador == null)
        {
            return;
        }

        // Solo la recoge el personaje del mismo elemento.
        if (jugador.Elemento != elemento)
        {
            return;
        }

        if (GestorNivel.Instancia != null)
        {
            GestorNivel.Instancia.RegistrarGema(elemento);
        }

        Destroy(gameObject);
    }
}
