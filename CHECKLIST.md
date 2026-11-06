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
- [ ] EnemigoEnPicada y PiedraRodante (Mundo 3)
- [ ] EnemigoPatrulla y LanzadorProyectiles (Mundo 4)
- [ ] EnemigoPerseguidor (Mundo 5)
- [ ] Jefe final GuardianEclipse (nivel 6-5)
- [x] Vagonetas y plataformas móviles
- [x] Cajas empujables
- [x] Los jugadores se atraviesan, aura del color del elemento y efecto visual del eclipse
- [x] Rocas que caen más rápido y texto de ayuda legible
- [ ] Niebla y lluvia
- [ ] Sistema de día y noche

## Semana 3: Mundo 2 (11 – 17 nov)
- [x] Armar los niveles 2-1 a 2-5 con más trampas (Minas de Potosí)
- [ ] Lianas, hojas trampolín y monos
- [ ] Cuerdas de charango y puerta melódica
- [ ] Eclipse automático

## Semana 4: Mundos 3 y 4 (18 – 24 nov)
- [ ] Armar los niveles 3-1 a 3-5 con más trampas (Yungas; traición en 3-4 y 3-5)
- [ ] Armar los niveles 4-1 a 4-5 con más trampas (Trópico; traición en 4-4 y 4-5)

## Semana 5: Mundos 5 y 6 e interfaz (25 nov – 1 dic)
- [ ] Armar los niveles 5-1 a 5-5 con más trampas (Aiquile; traición en 5-4 y 5-5)
- [ ] Armar los niveles 6-1 a 6-5, los más tramposos (Isla del Sol; traición en 6-4 y 6-5)
- [x] Menú principal
- [ ] Selección de mundo con mapa de Bolivia (pantalla simple lista; falta el mapa dibujado)
- [ ] HUD definitivo con gemas y muertes (versión temporal lista)
- [ ] Pausa, opciones, victoria y derrota
- [x] Sistema de estrellas
- [x] Guardado con PlayerPrefs

## Semana 6: arte y sonido (2 – 8 dic)
- [ ] Animaciones finales de los personajes
- [ ] Fondos con parallax de los 6 mundos
- [ ] Música por mundo y efectos de sonido
- [ ] Audio Mixer con volúmenes separados
- [ ] Partículas, temblor de cámara y bloom
- [ ] Intro y escena final
- [ ] Traducción al inglés

## Semana 7: pruebas y publicación (9 – 15 dic)
- [ ] Poner `modoPruebaTodoAbierto` en false en GestorProgreso (ahora todo está desbloqueado para probar)
- [ ] Pruebas con al menos 3 parejas de jugadores y corrección de errores
- [ ] Build WebGL optimizada
- [ ] Página en itch.io con capturas, descripción y créditos
- [ ] Preparar la presentación para la defensa
