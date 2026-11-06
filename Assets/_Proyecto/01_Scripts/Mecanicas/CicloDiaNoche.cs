// =====================================================================
// CicloDiaNoche.cs
// Proyecto: Inti y Killa
// Descripción: Alterna día y noche cada cierto tiempo. El sol o la luna cruzan
//              el cielo durante cada fase, así se ve de un vistazo cuánto falta
//              para el cambio; además, la luz de todo el nivel cambia de color.
//              Las plataformas de día y de noche (PlataformaDiaNoche) le
//              preguntan a este componente si les toca existir.
// Uso: un solo objeto en la escena, creado por el generador.
// =====================================================================

using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CicloDiaNoche : MonoBehaviour
{
    public static CicloDiaNoche Instancia { get; private set; }

    [Header("Tiempos")]
    [SerializeField] private float segundosPorFase = 6f;
    [SerializeField] private float segundosDeAviso = 1.5f;

    [Header("Luz del nivel")]
    [SerializeField] private Light2D luzGlobal;
    [SerializeField] private Color colorDia = new Color(1f, 0.97f, 0.90f);
    [SerializeField] private Color colorNoche = new Color(0.55f, 0.62f, 1f);
    [SerializeField] private float intensidadDia = 1f;
    [SerializeField] private float intensidadNoche = 0.55f;
    [SerializeField] private float segundosDelCambioDeLuz = 0.8f;

    [Header("Astros")]
    [SerializeField] private Transform sol;
    [SerializeField] private Transform luna;
    [SerializeField] private float recorridoAstros = 15f;
    [SerializeField] private float alturaBaseAstros = 6f;
    [SerializeField] private float alturaArcoAstros = 5.5f;

    private float tiempoEnFase = 0f;

    public bool EsDeDia { get; private set; }

    public float SegundosEnFase
    {
        get { return tiempoEnFase; }
    }

    public float SegundosRestantes
    {
        get { return segundosPorFase - tiempoEnFase; }
    }

    // Las plataformas parpadean cuando esto es verdad (regla de justicia).
    public bool EstaPorCambiar
    {
        get { return SegundosRestantes <= segundosDeAviso; }
    }

    public void Configurar(Light2D nuevaLuz, Transform nuevoSol, Transform nuevaLuna)
    {
        luzGlobal = nuevaLuz;
        sol = nuevoSol;
        luna = nuevaLuna;
    }

    private void Awake()
    {
        Instancia = this;
        EsDeDia = true;
    }

    private void Update()
    {
        tiempoEnFase = tiempoEnFase + Time.deltaTime;

        if (tiempoEnFase >= segundosPorFase)
        {
            tiempoEnFase = 0f;
            EsDeDia = !EsDeDia;
        }

        ActualizarLuz();
        ActualizarAstros();
    }

    private void ActualizarLuz()
    {
        // La luz empieza en el valor de la fase anterior y llega al de esta.
        float progreso = Mathf.Clamp01(tiempoEnFase / segundosDelCambioDeLuz);
        float avanceHaciaElDia = progreso;
        if (EsDeDia == false)
        {
            avanceHaciaElDia = 1f - progreso;
        }

        luzGlobal.color = Color.Lerp(colorNoche, colorDia, avanceHaciaElDia);
        luzGlobal.intensity = Mathf.Lerp(intensidadNoche, intensidadDia, avanceHaciaElDia);
    }

    private void ActualizarAstros()
    {
        float progreso = tiempoEnFase / segundosPorFase;
        float posicionX = Mathf.Lerp(-recorridoAstros, recorridoAstros, progreso);
        float posicionY = alturaBaseAstros + Mathf.Sin(progreso * Mathf.PI) * alturaArcoAstros;
        Vector3 posicion = new Vector3(posicionX, posicionY, 0f);

        sol.gameObject.SetActive(EsDeDia);
        luna.gameObject.SetActive(EsDeDia == false);
        sol.position = posicion;
        luna.position = posicion;
    }

    private void OnDestroy()
    {
        if (Instancia == this)
        {
            Instancia = null;
        }
    }
}
