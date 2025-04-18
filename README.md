# Gaïa’s Guardian - The FISE Awakening

| CI Status                                                                                                                                                                                                                          |
| ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| [![CI](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/actions/workflows/build-unity.yml/badge.svg)](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/actions/workflows/build-unity.yml?query=branch%3Adevelop) |

## About

**Gaïa’s Guardian - The FISE Awakening** is an open-source 2D platformer and action game developed by the **Automatic Team** from **CESI Corp**.

Set in a dystopian, polluted cyberpunk world, the game follows **Gaïa**, a powerful nature spirit striving to restore balance to a planet overtaken by technology and industry. Fight corrupted machines, traverse intricate biomes, and unleash nature’s wrath through unique elemental abilities.

## Editor & Environment

- **Unity Version:** `6000.0.40f1`
- **Target Platform:** Windows 64-bit
- **Minimum Requirements:** Unity 2022 LTS, .NET 6 SDK, GitHub CLI (for CI features)

## Installation

### 🧪 To Play

Download the latest build directly from the [Releases](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/releases) page.

### 🛠️ To Develop

1. Clone the repository:
   ```bash
   git clone https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian.git
   ```
2. Open the project using Unity version **6000.0.40f1** or higher.
3. Load the scene `MainMenu.unity` and hit **Play**.

## Features

- 🎮 Smooth 2D platforming and combat
- 🌱 Unlockable nature powers (Pollen Vortex, Water Jet, etc.)
- 🧠 Smart AI with behaviors: chase, patrol, jump, shoot
- 🗺️ Metroidvania-style progression and checkpoint system
- 💾 Persistent saving of abilities and level claims
- 🧪 Integrated CI with Unity tests and SonarCloud analysis

## 🎬 Gameplay Trailer

Watch the official gameplay trailer here:  
▶️ [https://youtu.be/fQOJP8FXoZU](https://youtu.be/fQOJP8FXoZU)

## Screenshots

### 🌿 Level 1

![Level 1](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/blob/develop/docs/Screenshots/Level1.png)

### 🔥 Level 2

![Level 2](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/blob/develop/docs/Screenshots/Level2.png)

### 🌊 Level 3

![Level 3](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/blob/develop/docs/Screenshots/Level3.png)

## User Manual

A full user guide is available here:  
📘 [User Manual (Markdown)](https://github.com/MrGautier256/Jeux-Video-Gaias-Guardian/blob/develop/docs/Manuel%20Utilisateur/gaias-gurdian-user-manual.md)

## CI/CD Pipeline

### 🔨 `Unity Build Pipeline`

- Triggered on:
  - Push to `develop`
  - Pull requests to `develop`
  - Manual dispatch
- Key jobs:
  - **Build** (Unity build with caching and artifact generation)
  - **Tests** (PlayMode automated tests)
  - **SonarCloud** (Code quality analysis via Docker container)

### 🚀 `Create Release from Artifacts`

- Triggered manually via GitHub UI
- Downloads build artifacts
- Zips them and attaches them to a new GitHub release

### 🧪 Legacy: `Pipeline Windows`

- Experimental
- Deprecated due to incompatibility with Unity DLL paths

## SonarCloud Setup

- Project: `MrGautier256_Jeux-Video-Gaias-Guardian`
- Organization: `mrgautier256`
- Exclusions: `TextMeshPro`, `Plugins`, `.json`, `.asmdef`
- Dashboard: [SonarCloud](https://sonarcloud.io/)

## Collaborators

**Gautier Mekhelian** -
**Thomas Ballini** -
**Guillem - Pairot** -
**Luca Legrand** -
**Mathis Brugeaud** -

## Contributions

We welcome contributions! Open issues, fork the project and submit PRs.

## License

Distributed under the **MIT License**.
