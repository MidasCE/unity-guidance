# Guidance

Rule-driven tooltips and highlights for Unity. Tag anything (a UI button, a 2D sprite, a 3D mesh) with an id, describe when a tip should appear, and let the evaluator pick what to show.

> Status: early. The rule engine and targeting work; the built-in popup view is still to come.

## Install

Package Manager → **Add package from git URL…**

```
https://github.com/<you>/unity-guidance.git?path=Packages/com.pattarapol.guidance
```

Requires Unity 2022.3 or newer.

## Quick start

1. Add a **GuidanceTarget** component to the object you want to point at and give it an id, e.g. `play-button`.
   UI elements use their RectTransform; world objects use their renderers' bounds and `Camera.main` (or the camera you assign).

2. Describe your rules:

```csharp
using Guidance;
using Guidance.Core;

var rules = new[]
{
    new GuidanceRule("first-play",
        new GuidancePrompt("play-button", "Tap here to start!", Placement.Above),
        new[]
        {
            Condition.Is("screen", "home"),
            Condition.Compare("level", ComparisonOperator.Less, 2),
        }),
};

var evaluator = new GuidanceEvaluator(rules, new PlayerPrefsProgressStore());
```

3. When your game state changes, build a context and ask what to show:

```csharp
var context = new GuidanceContext()
    .Set("screen", "home")
    .Set("level", player.Level);

foreach (var rule in evaluator.Candidates(context))
{
    if (!GuidanceTargetRegistry.Default.TryResolve(rule.Prompt.TargetId, out var target))
        continue; // target not on screen, try the next rule

    target.TryGetScreenRect(out var rect);
    // show your tooltip at rect
    evaluator.MarkShown(rule);
    break;
}
```

## Concepts

| Type | Role |
|------|------|
| `IGuidanceContext` | Read-only game state. Implement it over your own game or use `GuidanceContext`. |
| `ICondition` | A yes/no check against the context. Use `Condition.*` or write your own. |
| `GuidanceRule` | Conditions + prompt + priority + show-once. |
| `GuidanceEvaluator` | Returns rules that may show now, best first. |
| `IGuidanceProgressStore` | Remembers which show-once rules were seen. |
| `IGuidanceTarget` | Anything that can report a screen rect. `GuidanceTarget` covers UI, 2D and 3D. |

`Guidance.Core` has no Unity references, so rules can be unit-tested outside the editor.

## Roadmap

- Overlay view: dim layer with cutout, arrow, auto placement, safe-area clamping
- Runtime driver (re-evaluate on events, wait for UI to settle, busy gate)
- ScriptableObject and JSON rule authoring
- Samples: 2D UI, 3D world

## License

MIT, see [LICENSE.md](LICENSE.md).
