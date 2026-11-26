# PR1ME Jelly Theme (Jellyfin plugin)

A glass UI theme for the Jellyfin web client. It also shows up in the official Android and iOS apps and in Jellyfin Media Player, because those load the same web UI.

**Style only.** The plugin serves a stylesheet and a tiny script. It has no hooks on streaming, transcoding or any media endpoint, never patches Jellyfin's player, and scopes every rule so other plugins are left alone.

## Modules (each can be switched off in the plugin settings)

| Module | What it does |
|---|---|
| Colour, buttons, inputs and focus | Midnight base, accent colour everywhere, smooth hover and a clear focus ring |
| Top bar and menu | Frosted glass header, accent tab underline, restyled side menu |
| Rows and cards | Row titles, rounded cards, hover lift, progress bars, round scroll arrows |
| Details page | Bigger backdrop, metadata chips, cast and episode layout |
| Login page | Glass sign-in card, background image or glow, your login disclaimer styled as branding |

Settings page: accent colour, blur, glass tint, corner roundness, per-module toggles, login background.

The home-page hero is deliberately **not** included. Pair it with [TBD Exclusive Style](https://github.com/pr1megitty/jellyfin-tbd-exclusive-style); this theme never touches its carousel.

## Install

1. In Jellyfin go to Dashboard > Plugins > Repositories > +
2. Add this repository URL:

```
https://raw.githubusercontent.com/pr1megitty/jellyfin-plugin-pr1me-jelly-theme/main/manifest.json
```

3. Open the Catalog tab, install **PR1ME Jelly Theme**, then restart Jellyfin.
4. Requires the **File Transformation** plugin (`https://www.iamparadox.dev/jellyfin/plugins/manifest.json`).
5. Open the plugin's settings, pick an accent, and refresh your browser.

## Safety switch

If a style ever misbehaves, add `?pr1me=off` to the Jellyfin address to turn the theme off in that browser. Add `?pr1me=on` to bring it back. The master switch in the plugin settings turns it off for everyone.

## Login page text

The text comes from Dashboard > Branding > Login disclaimer. The theme restyles a block shaped like this (title, tagline, small note):

```html
<div style="text-align:center;line-height:1.6;">
  <div style="font-size:1.5em;font-weight:600;letter-spacing:3px;">PR1ME PRIVATE SCREENING</div>
  <div style="opacity:.75;">Curated movies &amp; series, ready when you are.</div>
  <div style="opacity:.5;font-size:.85em;margin-top:8px;">Members only · Please do not share your account</div>
</div>
```

## Release a new version

GitHub > Actions > "Build & Release" > Run workflow (use a new version number each time).

## Build locally

```
dotnet publish Jellyfin.Plugin.PR1MEJellyTheme/Jellyfin.Plugin.PR1MEJellyTheme.csproj -c Release -o out
```
