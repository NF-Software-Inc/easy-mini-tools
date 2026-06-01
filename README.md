# Easy Mini-Tools

A small grab-bag of **one-shot utilities** — the kind of scripts/tools that are *too small*, *too specific*, or *too fragile* to deserve their own repo or polished package… but might still save someone an afternoon.

These tools are intentionally pragmatic:

*   **Minimal ceremony**
*   **Solve one problem**
*   **May break when upstream UIs/APIs change**
*   **Shared anyway**, because sometimes that’s still better than starting from scratch

> If you’re looking for curated, stable, long-term maintained tooling… this repo probably isn’t it.  
> If you’re looking for *“I just need this done once”* helpers — welcome. 🙂

# Contents

## ✅ Extract Copilot Chat

Export Copilot chat transcripts from **JSON files saved via browser DevTools (Network tab)** into **Markdown**.

Copilot UIs change frequently and export options are inconsistent across accounts/tenants. This tool is meant for the common workaround:

1.  Open DevTools → Network
2.  Save the conversation JSON response(s) to disk
3.  Convert to readable `.md` files you can archive, search, and sort yourself

## ✅ OFX to XLSX Converter

Convert basic **OFX (Open Financial Exchange)** bank export files into simple **Excel (.xlsx)** spreadsheets.

This is a **best-effort parser**, built for practical use with real-world bank exports — not a full spec-compliant OFX implementation.

# Extract Copilot Chat

### What it does

*   Reads all `*.json` files in a directory
*   Extracts `store → rawConversationResponse → messages[]`
*   Uses each message’s:
    *   `author`
    *   `timestamp`
    *   `text`
*   Outputs one Markdown file per JSON file
*   Deduplicates messages (common when exports include repeated turns)

### Expected JSON shape

```json
{
  "store": {
    "rawConversationResponse": {
      "messages": [
        {
          "author": "user",
          "timestamp": "2026-05-07T21:15:11.123Z",
          "text": "Hello"
        }
      ]
    }
  }
}
```

## How to use

### Requirements

*   .NET SDK (recommended: **.NET 10+**)

### Run

Assuming you have a console app project under `easy-mini-tools/ExtractCopilotChat/`:

```bash
dotnet run --project easy-mini-tools/ExtractCopilotChat -- "<input-json-directory>" "<output-md-directory>"
```

If you omit the input directory, the tool will prompt you to enter one.
If you omit the output directory, the tool will be configured to write `.md` files beside each `.json`.

### Example

```bash
dotnet run --project easy-mini-tools/ExtractCopilotChat -- "C:\Exports\Copilot\Json" "C:\Exports\Copilot\Md"
```

## Output format

Each markdown file includes:

*   A header indicating export time + source file
*   Messages grouped by speaker (`You` / `Copilot`)
*   Timestamps when available
*   Message text preserved as-is (including code fences if present)

## Known limitations / disclaimers

*   **Fragile by nature**: Copilot’s internal data format and UI network payloads may change without notice.
*   **Not an official export**: You’re exporting data you saved manually from DevTools.
*   **Deduping is heuristic**: It aims to remove common duplicates but may occasionally merge distinct messages that are identical in content.
*   **No guarantee of completeness**: If the network response didn’t include older turns, the output won’t either.

# OFX to XLSX Converter

### What it does

* Reads a single `.ofx` file
* Extracts common transaction fields, including:
  * Date
  * Amount
  * Description / Memo
  * Transaction type (if available)
  * Account / balance info (when present)
* Outputs a flat `.xlsx` file for easy filtering, sorting, and analysis

### Supported scenarios

* Typical bank export files (checking / credit accounts)
* OFX files that resemble:
  * SGML-style (older OFX 1.x)
  * Loosely structured, non-strict variants many banks produce

### Not supported

* Full OFX 2.x / XML compliance
* Every possible tag or edge case
* Investment account structures (partial at best)
* Multi-account aggregation in a single file

## How to use

### Run

```bash
dotnet run --project easy-mini-tools/OFX-To-XLSX/OFX-To-XLSX-Converter
```

## Output format

The generated spreadsheet includes:

* One row per transaction
* Columns for:
  * Type
  * Date
  * Amount
  * Id
  * Check Number
  * Name
  * Description
* Minimal formatting — intended for **post-processing in Excel**, not presentation

## Known limitations / disclaimers

* **Best-effort parsing**: OFX files are notoriously inconsistent between institutions.
* **Loose parsing strategy**: The tool prioritizes “getting usable data out” over strict correctness.
* **Field coverage varies**: Some banks omit fields or use non-standard tags.
* **No validation guarantees**: Always sanity-check totals and balances against your source data.
* **Encoding quirks**: Some OFX files include odd encoding/line break issues that may require manual cleanup.

# Contributing

PRs are welcome, but the spirit of this repo is **small + useful + low-maintenance**.

If you submit improvements, please keep them aligned with the theme:

*   No heavy frameworks
*   No dependency explosions
*   Prefer “drop-in and run” simplicity

# Repo structure

A simple layout that stays scalable as you add more mini-tools:

```
/
  easy-mini-tools/
    ExtractCopilotChat/
      Program.cs
      ExtractCopilotChat.slnx
    OFX-To-XLSX/
      Program.cs
      OFX-To-XLSX.sln
  README.md
  LICENSE
```

# Authors

* **NF Software Inc.**

# License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
