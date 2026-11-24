// =====================================================================
// CriaturaQueSalta.cs
// Proyecto: Inti y Killa
// Descripción: Una criatura vive dentro de un charco (o de un hueco del
//              piso). Cuando alguien anda cerca, salta hacia arriba una y
//              otra vez, con un ritmo fijo. Antes de cada salto salen
//              burbujas en la superficie: ese es el aviso. Quien espera a
//              que vuelva a caer, cruza tranquilo.
// Uso: objeto vacío colocado en la superficie del charco. El generador le
//      pone el dibujo de la criatura y las burbujas como hijos.
// =====================================================================

using System.Collections;
using UnityEngine;

public class CriaturaQueSalta : TrampaBase
{
    [SerializeField] private float distanciaDeDeteccion = 4.5f;
    [SerializeField] private float alturaDelSalto = 3.6f;
    [SerializeField] private float segundosDeAviso = 0.6f;
    [SerializeField] private float segundosEntreSaltos = 1.4f;
    [SerializeField] private Vector2 tamanoCriatura = new Vector2(0.8f, 0.8f);
    [SerializeField] private string mensajeDeMuerte = "Algo vivía en ese charco y saltó sobre {0}.";

    private const float gravedad = 22f;

    private Transform dibujoCriatura;
    private Transform burbujas;
    private Vector3 posicionSuperficie;
    private bool estaEnElAire = false;
    private float momentoDeLaUltimaCaida = -10f;

    // El bot de pruebas lo usa para cruzar justo después de un salto.
    public bool EstaEnElAire
    {
        get { return estaEnElAire; }
    }

    public float SegundosDesdeLaUltimaCaida
    {
        get { return Time.time - momentoDeLaUltimaCaida; }
    }

    public void Configurar(float nuevaAltura, float nuevaDistancia)
    {
        alturaDelSalto = nuevaAltura;
        distanciaDeDeteccion = nuevaDistancia;
    }

    protected override void Awake()
    {
        base.Awake();
        posicionSuperficie = transform.position;
        dibujoCriatura = transform.Find("Criatura");
        burbujas = transform.Find("Burbujas");
        MostrarCriatura(false);
        MostrarBurbujas(false);
    }

    private void Start()
    {
        StartCoroutine(CicloDeSaltos());
    }

    private void Update()
    {
        if (estaEnElAire == false || dibujoCriatura == null)
        {
            return;
        }
        Vector2 tamano = tamanoCriatura * 0.85f;
        ControladorJugador atrapado = BuscarJugadorEnRectangulo(dibujoCriatura.position, tamano);
        if (atrapado != null)
        {
            atrapado.Morir(mensajeDeMuerte.Replace("{0}", atrapado.NombreJugador));
        }
    }

    private IEnumerator CicloDeSaltos()
    {
        while (true)
        {
            // Duerme mientras nadie anda cerca: así no gasta su sorpresa.
            while (HayAlguienCerca() == false)
            {
                yield return null;
            }

            yield return Burbujear();
            yield return Saltar();
            yield return new WaitForSeconds(segundosEntreSaltos);
        }
    }

    private bool HayAlguienCerca()
    {
        Vector2 centroZona = new Vector2(posicionSuperficie.x, posicionSuperficie.y + 3f);
        Vector2 tamanoZona = new Vector2(distanciaDeDeteccion * 2f, 8f);
        return BuscarJugadorEnRectangulo(centroZona, tamanoZona) != null;
    }

    // Aviso: las burbujas suben y bajan en la superficie.
    private IEnumerator Burbujear()
    {
        MostrarBurbujas(true);
        float tiempo = 0f;
        while (tiempo < segundosDeAviso)
        {
            tiempo = tiempo + Time.deltaTime;
            if (burbujas != null)
            {
                float vaiven = Mathf.Abs(Mathf.Sin(tiempo * 18f)) * 0.15f;
                burbujas.position = posicionSuperficie + new Vector3(0f, vaiven, 0f);
            }
            yield return null;
        }
        MostrarBurbujas(false);
    }

    // Sube y baja como una pelota lanzada hacia arriba.
    private IEnumerator Saltar()
    {
        float velocidadInicial = Mathf.Sqrt(2f * gravedad * alturaDelSalto);
        float duracion = 2f * velocidadInicial / gravedad;
        MostrarCriatura(true);
        estaEnElAire = true;
        float tiempo = 0f;
        while (tiempo < duracion)
        {
            tiempo = tiempo + Time.deltaTime;
            float altura = velocidadInicial * tiempo - 0.5f * gravedad * tiempo * tiempo;
            if (altura < 0f)
            {
                altura = 0f;
            }
            if (dibujoCriatura != null)
            {
                dibujoCriatura.position = posicionSuperficie + new Vector3(0f, altura, 0f);
            }
            yield return null;
        }
        estaEnElAire = false;
        momentoDeLaUltimaCaida = Time.time;
        MostrarCriatura(false);
    }

    private void MostrarCriatura(bool visible)
    {
        if (dibujoCriatura == null)
        {
            return;
        }
        SpriteRenderer[] dibujos = dibujoCriatura.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer dibujo in dibujos)
        {
            dibujo.enabled = visible;
        }
        if (visible == false)
        {
            dibujoCriatura.position = posicionSuperficie;
        }
    }

    private void MostrarBurbujas(bool visible)
    {
        if (burbujas == null)
        {
            return;
        }
        SpriteRenderer[] dibujos = burbujas.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer dibujo in dibujos)
        {
            dibujo.enabled = visible;
        }
    }
}
