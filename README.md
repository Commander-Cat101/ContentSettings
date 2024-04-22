# ContentSettings

A library for adding custom settings to Content Warning.

[![Build](https://github.com/Commander-Cat101/ContentSettings/workflows/Build/badge.svg)](https://github.com/Commander-Cat101/ContentSettings/actions?query=workflow:"Build")
[![GitHub release](https://img.shields.io/github/release/Commander-Cat101/ContentSettings?include_prereleases=&sort=semver&color=blue)](https://github.com/Commander-Cat101/ContentSettings/releases/)
[![Thunderstore - ContentSettings](https://img.shields.io/badge/Thunderstore-ContentSettings-blue?logo=thunderstore&logoColor=white)](https://thunderstore.io/c/content-warning/p/CommanderCat101/ContentSettings/)
[![NuGet - CommanderCat101.ContentSettings](https://img.shields.io/badge/NuGet-CommanderCat101.ContentSettings-blue?logo=nuget)](https://www.nuget.org/packages/CommanderCat101.ContentSettings)

## Features
- [x] Register custom settings
  - [x] Manual registration
  - [x] Attribute-based registration
  - [x] Custom categories
- [x] Addition inputs
  - [x] Text input (TextField)
  - [x] Integer input (Slider)
  - [x] Boolean input (Checkbox)

### Planned
- [ ] More input types
- [ ] Networking support


## Usage

If you just want to look at some code, take a look at the [Example Plugin](./SettingsTemplate/Main.cs).

### Registering a Setting

```csharp
void Awake()
{
    SettingsLoader.RegisterSetting("YOUR TAB", new ExampleSetting());
}

public class ExampleSetting : FloatSetting, ICustomSetting
{
    public override void ApplyValue()
    {
        // Do something with the value
    }

    public override float GetDefaultValue() => 0.5f;

    public override float2 GetMinMaxValue() => new(0f, 1f);

    public string GetDisplayName() => "Example Setting";
}  
```

## Contributing

This repository follows [Conventional Commits](https://www.conventionalcommits.org/en/v1.0.0/)

Commits will be rejected automatically if they do not follow the format specified below.

### Format

`<type>(optional scope): <description>`

Example: `feat(pre-event): add speakers section`

## Credits

- [CommanderCat](https://github.com/Commander-Cat101) - Creator and Developer
- [dhkatz](https://github.com/dhkatz) - Developer and Maintainer

## License

Released under [GPL-3.0](./LICENSE) by [@Commander-Cat101](https://github.com/Commander-Cat101).