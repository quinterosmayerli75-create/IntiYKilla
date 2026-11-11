# Checklist – Inti y Killa

Marca cada tarea con [x] al terminarla. Orden según el cronograma del GDD. Plazo: 15 de diciembre de 2026.

## Semana 1: base (28 oct – 3 nov)
- [x] Instalar Unity 6 LTS con el módulo WebGL
- [x] Instalar VS Code y la extensión Claude Code
- [ ] Instalar el plugin oficial de Unity para Claude Code
- [x] Crear el proyecto con la plantilla Universal 2D
- [x] Instalar los paquetes del GDD (sección 9)
- [x] Crear el repositorio en GitHub con .gitignore de Unity
- [x] Agregar GDD.md, CHECKLIST.md y CLAUDE.md al repositorio
- [x] Crear la estructura de carpetas
- [x] Configurar el Input System para 2 jugadores
- [x] Programar movimiento y salto de Inti y Killa
- [x] Programar charcos de lava, agua y veneno
- [x] Programar muerte y reinicio del nivel
- [x] Programar gemas y puertas de salida
- [x] Configurar la cámara Cinemachine para los dos
- [ ] Crear tileset base, paleta y Rule Tiles (bloques de sal listos; faltan los demás mundos)
- [x] Armar los niveles 1-1, 1-2 y 1-3

## Semana 2: estructura de mundos y Mundo 1 (4 – 10 nov)
- [x] Cambiar a 6 mundos con 5 niveles cada uno (DatosJuego y GestorProgreso)
- [x] Pantallas SeleccionMundo y SeleccionNivel
- [x] Mensaje "¡Mundo completado!" al ganar el nivel 5
- [x] Botones y palancas
- [x] Altar de eclipse (intercambio de poderes)
- [x] Luces 2D e iluminación de Inti
- [x] Armar los niveles 1-4 y 1-5 (Salar de Uyuni)
- [x] Trampas engañosas: suelo falso, bloque invisible, púas ocultas, techo que cae, gema trampa, puerta falsa, plataforma que huye, botón engañoso y enemigo sorpresa
- [x] Trampas de traición: botón de traición, palanca sospechosa y placa vinculada
- [x] Sistema de muertes: reinicio rápido, contador, récord y mensajes graciosos
- [x] Estrellas por pocas muertes (máximo configurable por nivel)
- [x] Poner trampas en los niveles 1-1 a 1-5 y 2-1
- [x] RemolinoViento y remolinos de sal en los niveles 1-4 y 1-5
- [x] Mundo 1 completo según la plantilla del GDD
- [x] Aturdir enemigos al saltar encima (3 segundos)
- [x] PeligroQueCae y EnemigoVolador (Mundo 2)
- [x] EnemigoEnPicada y PiedraRodante (Mundo 3)
- [x] EnemigoPatrulla y LanzadorProyectiles (Mundo 4)
- [x] EnemigoPerseguidor (Mundo 5)
- [x] Jefe final GuardianEclipse (nivel 6-5)
- [x] Vagonetas y plataformas móviles
- [x] Cajas empujables
- [x] Los jugadores se atraviesan, aura del color del elemento y efecto visual del eclipse
- [x] Rocas que caen más rápido y texto de ayuda legible
- [x] Niebla y lluvia
- [x] Habilidades: Killa congela charcos e Inti derrite el hielo (AguaCongelable)
- [x] Sistema de día y noche

## Semana 3: Mundo 2 (11 – 17 nov)
- [x] Armar los niveles 2-1 a 2-5 con más trampas (Minas de Potosí)
- [x] Lianas, hojas trampolín y monos
- [x] Cuerdas de charango y puerta melódica
- [x] Eclipse automático (con un instante de gracia en los charcos al cambiar)

## Semana 4: Mundos 3 y 4 (18 – 24 nov)
- [x] Armar los niveles 3-1 a 3-5 con más trampas (Yungas; traición en 3-4 y 3-5)
- [x] Armar los niveles 4-1 a 4-5 con más trampas (Trópico; traición en 4-4 y 4-5)
- [x] Crecida del río en el nivel 4-4

## Versión difícil (pedido del revisor, 9 nov)
- [x] Trampas nuevas: PisoQueSeCalienta, PuertasQueSeCambian, GemaQueHuye, PlataformaQueSeVoltea, ZonaControlesInvertidos y flecha engañosa
- [x] Monos y llamas solo apuntan a quien está a su altura (el mono del 4-2 mataba desde la copa)
- [x] Más trampas en los 30 niveles, con dificultad progresiva fuerte
- [x] Jugar los 30 niveles en la versión difícil y confirmar que todos se pueden ganar (prueba automática: 30 de 30 ganados seguidos, sin muertes)
- [x] Trampas del desafío en los niveles: lluvia de rocas con sombras (1-3, 2-1, 3-1, 6-1), poción engañosa (1-2), piso que arrastra (2-2), bicho o mono que cae del techo (1-5, 2-3, 3-2, 4-4) y bloque que baja a aplastar (2-4)
- [x] Menos trampas repetidas: el cambio de puertas queda solo en los finales de mundo (2-5, 3-5, 5-5, 6-5) y hay menos pisos que se calientan
- [x] Quitar trampas imposibles de superar: púas del 3-2 y del 3-5, enemigo de la salida del 5-2 y veneno del centro del 6-1
- [x] Colocar en los niveles el suelo que desaparece al saltar (salida del 1-2) y el sensor con el bloque que cierra el regreso (1-1); probados por separado y con los 30 niveles otra vez ganados seguidos
- [x] Animar la lava y el agua (que fluyan como un río)
- [ ] Plataformas de roca flotante y decoración como en las imágenes de referencia (imágenes de Leonardo AI, ver PROPUESTA_TRAMPAS.md)

## Versión engañosa (ver PROPUESTA_TRAMPAS.md, aprobada el 11 nov)
- [x] Pruebas del punto de partida: 30 de 30 niveles cargan, el bot gana los 30, sistema y piezas del desafío sin fallos
- [ ] Etapa 0: bot de pruebas guardado en el proyecto, con prueba de reinicio y de gemas alcanzables
- [ ] Etapa 1: piezas nuevas (gema explosiva, gema que persigue, plataforma fantasma, palanca engañosa, roca por detrás, nube tramposa, salida que se aleja, sensor al llegar a la salida)
- [ ] Etapa 2: Mundos 1 y 2 (6 y 7 trampas por nivel, con momentos memorables)
- [ ] Etapa 3: Mundos 3 y 4 (8 y 9 trampas por nivel)
- [ ] Etapa 4: Mundos 5 y 6 (10 y 11-12 trampas por nivel)
- [ ] Bot sobre los 30 niveles y partida de prueba después de cada etapa

## Semana 5: Mundos 5 y 6 e interfaz (25 nov – 1 dic)
- [x] Armar los niveles 5-1 a 5-5 con más trampas (Aiquile; traición en 5-4 y 5-5)
- [x] Armar los niveles 6-1 a 6-5 (Isla del Sol; traición en 6-4; jefe en 6-5)
- [x] Menú principal
- [x] Selección de mundo con tarjetas y selección de nivel con cuadros (arte provisional)
- [x] Arte de los 6 mundos (tarjetas y fondos en `03_Sprites/Interfaz`; originales en ArteOriginal/Mundos)
- [ ] HUD definitivo con gemas y muertes (versión temporal lista)
- [x] Pausa (Esc), opciones (volumen y pantalla completa), ventana de victoria y reinicio al morir
- [x] Sistema de estrellas
- [x] Guardado con PlayerPrefs

## Semana 6: arte y sonido (2 – 8 dic)
- [ ] Animaciones finales de los personajes
- [ ] Fondos con parallax de los 6 mundos
- [ ] Música por mundo y efectos de sonido
- [ ] Audio Mixer con volúmenes separados
- [ ] Partículas, temblor de cámara y bloom
- [x] Intro y escena final (historia en páginas; textos provisionales)
- [ ] Traducción al inglés

## Semana 7: pruebas y publicación (9 – 15 dic)
- [ ] Poner `modoPruebaTodoAbierto` en false en GestorProgreso (ahora todo está desbloqueado para probar)
- [ ] Pruebas con al menos 3 parejas de jugadores y corrección de errores
- [ ] Build WebGL optimizada
- [ ] Página en itch.io con capturas, descripción y créditos
- [ ] Preparar la presentación para la defensa
