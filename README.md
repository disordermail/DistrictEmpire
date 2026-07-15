# District Empire

Mobile-first 2D real-estate strategy game for iOS and Android.

## Recommended Tech

- Client: Unity 6 LTS, C#, UI Toolkit, portrait orientation.
- Architecture: Domain, Application, Infrastructure, Presentation.
- Backend: Supabase with PostgreSQL + PostGIS.
- Server logic: TypeScript Supabase Edge Functions under `/v1`.
- Analytics/live services: Firebase Analytics, Crashlytics, FCM.
- Monetization: Unity IAP and rewarded ads.
- Content: data-driven country packs and localization files generated from source spreadsheets.

This matches the supplied product package and is the right stack for a geolocation-like 2D mobile game with server-authoritative economy rules.

## Repo Layout

```text
unity/                     Unity project root placeholder
  Assets/DistrictEmpire/   Game source organized by layer
backend/                   Supabase schema and Edge Functions
content/                   Country packs and localization source exports
docs/                      Architecture notes and imported source package
prototype/                 Offline browser MVP for quick loop testing
scripts/                   Project utilities
```

## Offline MVP

An initial no-server prototype is available in `prototype/`.

Run it with:

```sh
python3 -m http.server 8080 --directory prototype
```

Then open `http://localhost:8080`.

## First Build Target

The MVP keeps the final architecture but limits content scope:

- Poland as the first country pack.
- One or two cities.
- Five market tiers.
- Five property types.
- Server time, ledger entries, idempotency keys, localization, and data-driven config from day one.

## Next Setup Steps

1. Create a Unity 6 LTS project in `unity/`.
2. Open the project and keep the existing `Assets/DistrictEmpire` folder.
3. Create a Supabase project and apply SQL migrations from `backend/supabase/migrations`.
4. Export spreadsheet data into `content/` using the specs in `docs/source-package`.
