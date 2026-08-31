# DoubleEcho

A fast-paced 2D action platformer prototype built in Unity. The project includes a title screen, in-game pause flow, animated sprite-based combat, and a Windows build script for packaging a standalone player.

## Overview

DoubleEcho is a playable prototype focused on responsive movement, environmental traversal, and a stylized combat loop. The project includes:

- side-scrolling platform movement
- jumping, crouching, dashing, and attacks
- sprite-sheet animation driven from Resources-loaded sheets
- title menu flow and pause menu behavior
- custom Unity build support for Windows standalone deployment

## Current Status

This is a working prototype and build target setup for Windows. The project is in active development and is intended to be a playable foundation for further level, enemy, and polish work.

## Features

- Character movement with acceleration and platformer physics
- Combat actions including standard attacks and special attacks
- Player animation system using row-based sprite slicing / loading
- Menu flow for title screen and pause controls
- Unity editor build helper for Windows export

## Controls

- A / D or Left / Right Arrow: move
- W / Up Arrow / Space: jump
- S / Down Arrow / Left Ctrl: crouch
- Left Shift / Right Shift / E: dash
- F / J / Left Mouse: basic attack
- Q / R / Right Mouse: special attack
- Escape: pause / resume

## How to Run

### In Unity

1. Open the project in Unity 6000.5.10f1 or a compatible version.
2. Load the main scene.
3. Press Play in the editor to test gameplay.

### Windows Build

1. In the Unity editor, open the menu:
   - Tools > Build Windows Player
2. The project will generate a Windows executable under:
   - Builds/Windows/DoubleEcho.exe

## Project Structure

- Assets/Scripts: gameplay, menu, animation, and runtime logic
- Assets/Scenes: TitleScreen and MainGame scenes
- Assets/Editor: editor-only build and tool scripts
- Assets/Resources: runtime-loaded sprite resources
- ProjectSettings: Unity project configuration

## Notes

- Runtime code avoids using UnityEditor APIs in gameplay scripts.
- Sprite loading is handled through Resources.LoadAll<Sprite>() for runtime-safe asset access.
- The custom build script is defined in Assets/Editor/BuildWindows.cs.

## Development

This project was developed with:

- Unity 6000.5.10f1
- C#
- Unity UI and TextMeshPro
- URP-based project settings

## Repository Purpose

This repository is intended to preserve the project source, scripts, scenes, and build configuration for continued development and sharing.
