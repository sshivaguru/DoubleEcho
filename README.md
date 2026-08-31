# Double Echo

A 2D puzzle-platformer prototype built for a game jam under the theme **DOUBLE**.

## Concept

Double Echo is about two versions of the same person, separated across time, 
who have to work together to escape.

The game is split into two phases per level:

- **Phase 1 — Helper:** You control a character moving through a level, 
  interacting with the environment — hitting switches, opening doors, 
  disarming traps. Every move you make is recorded.
- **Phase 2 — Runner:** The level resets and you now control a second 
  character on the exact same map. A translucent "Echo" of your Phase 1 
  run plays back alongside you in real time, showing you what your past 
  self already did. Anything you handled in Phase 1 (an opened door, a 
  disarmed trap) stays resolved in Phase 2 — anything you missed becomes a 
  live obstacle you now have to deal with directly, sometimes while facing 
  enemies along the way.

The twist tying it to the jam theme: by the end, it's revealed the two 
characters are actually the same person — a future version who came back 
to help their past self survive, at a cost neither of them expected.

## Current Status (Prototype — built in one jam session)

This is an early, partial build. What's actually implemented so far:

- Two separate playable characters exist in the same scene: a **red** 
  character and a **blue** character.
- Both characters currently share basic movement controls (left/right, 
  jump).
- The red character can walk to and activate a yellow button/switch.
- Activating the switch drops/opens a door.
- The blue character exists in the scene but does **not** yet interact 
  with the switch or door — this interaction is one-directional 
  (red only) at this stage.

### Not yet implemented (intended for the full version)
- The core **recording/playback "Echo" system** — currently the two 
  characters are just two independently controlled objects, not a 
  recorded-and-replayed sequence. In the full vision, the blue character's 
  actions would be a real-time playback of a previous recorded run, not a 
  second live-controlled character.
- Phase transition (Helper → Runner) and the visual "flip" between them.
- Traps/hazards that respond differently based on Phase 1 actions.
- Enemies/henchmen for the Runner phase.
- Title screen, pause menu, and win/lose states.
- Narrative text and the final reveal beat.

## How to Play (current build)

- **A/D or Arrow Keys** — move
- **Space** — jump
- Move the red character to the yellow switch to open the door.

## Built With

- Unity 6
- C#
- Free "Industrial Zone" tileset assets for environment art

## Why It's Incomplete

This was built solo/quickly within a jam timeframe. The core mechanical 
idea (record-and-replay echo puzzle system) is more involved than a single 
switch/door interaction, and time ran out before the recording/playback 
logic, phase system, and narrative elements could be finished. What's here 
is the foundational movement and basic interaction layer the rest of the 
game was meant to build on top of.
