# API

Prefix `/v1`.

Główne endpointy:

- `/auth/guest`
- `/auth/link`
- `/countries`
- `/map/viewport`
- `/properties/{id}`
- `/properties/{id}/purchase`
- `/market`
- `/auctions/{id}/bid`
- `/rental-listings`
- `/issues`
- `/taxes`
- `/rankings`
- `/shop`
- `/sync/snapshot`

Każda mutacja wymaga `Idempotency-Key`.
