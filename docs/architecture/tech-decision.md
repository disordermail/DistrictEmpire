# Tech Decision

## Decision

Use Unity 6 LTS for the mobile client, Supabase/PostgreSQL/PostGIS for authoritative game data, and TypeScript Supabase Edge Functions for the `/v1` API surface.

## Why

- The product targets iOS and Android with a 2D portrait UI, making Unity a strong fit without requiring separate native clients.
- UI Toolkit supports the requested app-like interface with cards, bottom sheets, and tabs.
- Supabase gives fast MVP velocity while keeping PostgreSQL, row level security, and server-side functions available for stricter economy rules.
- PostGIS supports city/district/viewport queries without exposing player position publicly.
- TypeScript Edge Functions are a good boundary for idempotency, server time, wallet ledger writes, rankings, taxes, and rewards.

## Boundaries

- Unity owns rendering, input, local view models, and optimistic display state.
- Edge Functions own all mutations, idempotency, prices, rewards, taxes, auctions, and wallet changes.
- PostgreSQL owns durable state, content versions, ledgers, country packs, and auditability.
- Country-specific content lives in data, not code.
