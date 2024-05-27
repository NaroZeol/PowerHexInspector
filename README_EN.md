# PowerHexInspector

A simple PowerToys Run plugin.

Provides functionality to convert numbers to different bases.

## Usage

### Trigger Keyword

The current trigger keyword is `insp`.

### Input Format

The input format is:

    insp {base} {input}

The base can be one of the following values:

- `b` or `B`: Binary
- `d` or `D`: Decimal
- `h` or `H`: Hexadecimal

### Example Usage

![Example](./Images/examples/ep1.png)

## Installation
Download the latest release, extract it, and place the `PowerHexInspector` folder into the `%LOCALAPPDATA%\Microsoft\PowerToys\PowerToys Run\Plugins` directory. Then, restart PowerToys.

## Disclaimer
This plugin is still in development and may have various unknown bugs and crashes.

## TODO !!!
- [ ] Support formatted output (spacing, casing, etc.)
- [ ] Add icon
- [ ] Support big-endian
- [ ] Support settings for different bit lengths (currently fixed at 64-bit)
