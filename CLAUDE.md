# Instrucciones para el agente – Inti y Killa

Proyecto final individual: juego de plataformas y puzzles cooperativo 2D para 2 jugadores en el mismo teclado, en Unity 6 LTS (Universal 2D / URP), para publicar en itch.io (WebGL). Plazo: 27 de noviembre de 2026.

## Lee primero
- `GDD.md`: diseño completo del juego (personajes, mecánicas, 20 niveles, escenas, tecnología). Es la fuente de verdad.
- `CHECKLIST.md`: tareas por semana. Al terminar una tarea, márcala con [x].

## Reglas de código (importante: hay defensa académica)
- C# explícito y detallado, sin atajos: nada de `var` cuando el tipo no es obvio, nada de expresiones lambda innecesarias ni operadores ternarios anidados.
- Nombres de clases, variables y métodos en español y descriptivos (por ejemplo `ControladorJugador`, `velocidadMovimiento`, `ReiniciarNivel()`).
- Comentarios en español que expliquen el porqué de cada bloque.
- Usar `[SerializeField] private` en lugar de campos públicos.
- Un script por responsabilidad; cada mecánica es un componente reutilizable que se pueda colocar en cualquier nivel.
- Entregar siempre scripts completos, listos para pegar, no fragmentos.
- Input System nuevo (no `Input.GetKey`), con mapas separados para Jugador 1 (WASD) y Jugador 2 (flechas).

## Estructura
- Todo el contenido propio va en `Assets/_Proyecto/` con: `00_Scenes`, `01_Scripts`, `02_Prefabs`, `03_Sprites`, `Animations`, `Audio`, `Input`, `Materiales`, `Tiles`, `UI`. Respetar estos nombres exactos.
- No volver a ejecutar el menú "Inti y Killa → Crear carpetas del proyecto": crearía carpetas duplicadas con los nombres antiguos.
- Escenas: `Intro`, `MenuPrincipal`, `SeleccionNivel`, `Nivel01` … `Nivel20`, `Final`.
- Guardado con PlayerPrefs.

## Flujo de trabajo
- Si hay un Editor de Unity abierto, usar `unity status` y el skill unity-cli para modificar escenas; no editar a mano archivos `.unity`, `.prefab` o `.asset`.
- Instalar paquetes con el skill unity-package-management.
- Hacer commits pequeños y descriptivos en español.
- Si una tarea no está en el GDD, preguntar antes de inventarla.
- Priorizar que el juego funcione completo antes de agregar detalles (ver plan B en el GDD).
