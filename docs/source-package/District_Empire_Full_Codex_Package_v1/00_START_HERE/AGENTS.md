# AGENTS.md

## Kontekst

Budujesz grę `District Empire`.

## Zasady bezwzględne

1. Nie wpisuj tekstów użytkownika na stałe w kodzie.
2. Każdy tekst musi mieć `localization_key`.
3. Polski jest językiem źródłowym.
4. Każdy system ma działać z Country Pack.
5. Kod nie może zakładać, że kraj to Polska.
6. PLN jest walutą pierwszego Country Pack, nie walutą silnika.
7. UI jest pionowe i jednoręczne.
8. Główny ekran to mapa.
9. Gra jest 2D, nie AR i nie Pokémon GO.
10. Używaj kart, bottom sheetów i zakładek zamiast wielu okien.
11. Ekonomia, ceny, czasy i nagrody pochodzą z konfiguracji.
12. Czas serwera jest źródłem prawdy.
13. Wszystkie transakcje mają ledger i idempotency key.
14. Klient nie rozstrzyga wyników aukcji, podatków i nagród.
15. Pozycja gracza nie jest publiczna.
16. Inni gracze widzą własność i aktywność, nie pozycję.
17. Nie implementuj funkcji spoza aktualnej fazy.
18. Każde zadanie kończ testami i działającym stanem.

## Styl kodu

- Unity 6 LTS
- C#
- UI Toolkit
- czysta domena bez zależności od Unity
- use cases w Application
- adaptery w Infrastructure
- ekrany i view models w Presentation

## Struktura

```text
Assets/DistrictEmpire/
  Domain/
  Application/
  Infrastructure/
  Presentation/
  Config/
  Localization/
  CountryPacks/
  Tests/
```

## Definition of Done

- kompilacja działa,
- testy przechodzą,
- teksty są lokalizowane,
- dane są konfigurowalne,
- UI działa pionowo,
- błędy są obsłużone,
- dokumentacja została zaktualizowana.
