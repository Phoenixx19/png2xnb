 # png2xnb <img src="https://raw.githubusercontent.com/Phoenixx19/png2xnb/master/png2xnb/appIcon.png" width="80px" alt="png2xnb logo" align="right">

*An XNB packer for modern times.* 📦

> A fork of [sullerandras/png_to_xnb](https://github.com/sullerandras/png_to_xnb).<br>
> Icon beautifully crafted by [kkaero](https://kkaeero.tumblr.com/).

<br>
<img src="https://raw.githubusercontent.com/Phoenixx19/png2xnb/master/preview.png" alt="Preview" height="400px" />

## Overview

*(checked = done; unchecked = uncomplete atm)*<br>
The basic `png_to_xnb` features such as:

- [x] PNG file conversion
- [x] PNG folder conversion
- [x] Toggle for premultiplying alpha
- [x] Reach and HiDef formats support
- [x] Image compression

but `png2xnb` adds:
- [x] Automatic output fill
- [x] Settings-that-save-on-closing-the-application 🤯
- [x] Feedback on inputs (data validation)
- [x] System notification on auto convert
- [ ] Auto convert (listens for file modifications and converts automatically)
- [ ] Recursive folder conversion (compiling every PNG inside every subfolder too)

## Motivation

While working on JumpKingPlus, two fellow friends of mine introduced me to png_to_xnb for updating their levels' hitboxes and I quickly realized how useful and quick this tool was in Jump King Level Making

...so why not make it better by completely rewriting the UI in WPF?!?

## Requirements

The program *requires* `.NET 4.8`. <br>The program **only** runs **on Windows** unless a miracle happens with `wine` or `mono`.

Requires as in:
> "I've compiled it in `4.8` because I haven't bothered seeing if it can be lowered further. It should work with newer .NET versions and probably older versions too if you dare to recompile and deal with dependencies."

## License

Following sullerandras' legacy with **GPLv3** (see LICENSE file).
