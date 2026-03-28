---
layout: home

hero:
  name: TimberBoostControl
  text: JSON-driven blueprint boosts for Timberborn
  tagline: A DLL-based Timberborn mod that exposes its state through a bottom-bar panel and regenerates blueprint overrides from settings.json.
  image:
    src: /boost-icon.png
    alt: TimberBoostControl boost icon
  actions:
    - theme: brand
      text: Get Started
      link: /getting-started
    - theme: alt
      text: Settings
      link: /settings
    - theme: alt
      text: GitHub
      link: https://github.com/Sunwood-ai-labs/TimberBoostControl

features:
  - title: Bottom-bar launch flow
    details: Open the panel from the Timberborn bottom bar. The launcher falls back to text if the icon is unavailable.
  - title: Reloadable settings.json
    details: Edit the tracked JSON file directly, then reload it in-game to regenerate blueprint overrides without rebuilding the mod.
  - title: Blueprint-first architecture
    details: Generated files live beside the mod, are tracked in .generated-files.txt, and are safely replaced on each regeneration pass.
---

## Why this repo matters

TimberBoostControl is intentionally small enough to study, but complete enough to show the full loop of a Timberborn DLL mod:

- startup initialization through `IModStarter`
- a Bindito configurator that wires the runtime services
- a bottom-bar UI entry point
- a settings store with legacy compatibility
- blueprint generation that writes JSON overrides into the mod folder

This repository workflow and the initial modding scaffold were developed with support from [timberborn-modding-skill](https://github.com/Sunwood-ai-labs/timberborn-modding-skill).

<div class="quick-facts">
  <div>
    <strong>Game target</strong>
    Timberborn 1.0.x
  </div>
  <div>
    <strong>Runtime model</strong>
    DLL mod plus generated blueprint overrides
  </div>
  <div>
    <strong>Primary workflow</strong>
    Edit settings.json, reload, restart the game
  </div>
</div>

## Latest release

- Read [Release Notes (v0.1.0)](/releases/v0.1.0) for the initial release scope and shipped behavior.
- Follow the [Walkthrough (v0.1.0)](/guide/articles/timberboostcontrol-v0-1-0) when you want a release-focused setup and runtime checklist.

## Documentation map

- Start with [Getting Started](/getting-started) for build, install, and runtime flow.
- Use [Settings](/settings) when you need the exact JSON keys and gameplay effects.
- Read [Architecture](/architecture) for code ownership and repo layout.
- Use [Troubleshooting](/troubleshooting) for invalid JSON, missing icon, and build issues.
