# ARKStatsExtractor

For the game ARK Survival Evolved. Extracts possible levelups of creatures to get the values that are inherited.

[![Screenshot](img/screenshot.png)](https://github.com/cadon/ARKStatsExtractor/releases/latest)

* Type in stats of creature
* Click Extract
* View possible wild and domesticated levelups
* Copy Stats in Breeding-Spreadsheet

## Download
Download the [latest release here](https://github.com/cadon/ARKStatsExtractor/releases/latest).

* The file stats.txt contains all the stats, it can be edited and updated if necessary.
* The file settings.txt contains all the multipliers for the levelup (introduced in v231)

## Patchnotes
* v0.13.1: added support for new creature-balance
* v0.12: fixed algorithm for just tamed creatures ("torpor too high after taming"-bug)
* v0.11.1: updated stats.txt for scorpions tamed before v225
* v0.11: updated creature-stats in stats.txt, changed algorithm to not needing xp anymore and removed level.txt (easier for custom server)
* v0.10: sum of levels and what it should be to easier see correct combinations, fixes
* v0.9.3: fixes, torpor-bug workaround, new row-output-format
* v0.9: fixes, improved algorithm for better results, support for already bred creatures
* v0.8: fix for max-level-determination, ui-improvements
* v0.7: fix for rounding-errors (occured in high-level-creatures)
* v0.6: small fixes and improvements
