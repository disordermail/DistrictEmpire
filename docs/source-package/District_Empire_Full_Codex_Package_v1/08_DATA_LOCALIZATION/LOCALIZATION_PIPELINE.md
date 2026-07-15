# Localization Pipeline

## Język źródłowy

Polski (`pl-PL`).

## Format

Excel z kolumnami:

- key,
- context,
- screen,
- pl-PL,
- en-US,
- de-DE,
- status,
- notes,
- max_length.

## Proces

1. Designer dodaje klucz.
2. Polski tekst jest obowiązkowy.
3. Importer waliduje unikalność.
4. Brak tłumaczenia używa polskiego fallbacku w środowisku developerskim.
5. Build produkcyjny blokuje brak wymaganych tłumaczeń dla aktywnego kraju.

## Reguły

- brak tekstów w kodzie,
- placeholdery nazwane,
- pluralizacja,
- formatowanie liczb per kraj,
- waluty per Country Pack.
