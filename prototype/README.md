# District Empire Offline MVP

This is a browser-only prototype for testing the first app loop without Supabase, Unity services, authentication, analytics, or network calls.

## Run

From the repository root:

```sh
python3 -m http.server 8080 --directory prototype
```

Then open `http://localhost:8080`.

## Included Loop

- Guide a fresh player through a persistent, action-based tutorial: inspect, buy, wait for notary, list, choose an applicant, sign a lease, collect rent, and resolve a task.
- Keep existing saved games outside the new onboarding flow; use Reset progress to experience it from the beginning.
- Browse a portrait city map.
- Use the map-first property sheet with Overview, Rent, and Repairs tabs.
- Follow the first-60-minutes progress panel.
- Start through a welcome/city onboarding moment.
- See city activity feed hints and investor-watching context.
- Select properties.
- Buy listings with local cash.
- Inspect a dedicated property info view.
- Upgrade property levels to raise rent and valuation.
- Spend influence to boost popularity.
- Maintain owned properties to improve condition and rent.
- Move each purchase through notary paperwork, renovation/preparation, advertising, applications, and an active lease.
- See a one-line lifecycle status on every portfolio card, including notary, ad demand, applications, occupied, rent due, late payment, and contract cancellation.
- Use Portfolio as the daily management hub: collect rent, review a short attention queue, then manage compact live-status cards.
- Browse Invest for ordinary listings and a playable discounted live auction.
- Receive city activity moments and a small daily momentum reward when advancing the offline day.
- Publish advertisements that gather views and visits each simulated day before applications arrive.
- Review applicants with short stories, accept them, counter-offer, or ask for a deposit.
- Keep tenant memory for lease length, payment history, and relationship level.
- Choose residential or commercial tenants, including shop/service offers from the design docs.
- Handle problematic renters with daily actions such as asking, mailing, lawyer notices, or police.
- Block property sales until active rental contracts are cancelled and the unit is ready.
- Put owned properties on the market for resale.
- Claim lightweight goals and level-up rewards.
- Claim a daily reward.
- Unlock micro-wins for first purchase, renovation, tenant, rent, issue resolution, rankings, and dream property preview.
- Get short feedback ceremonies for purchases, rent, maintenance, and lease signing.
- Collect daily rent.
- Resolve simple building issues.
- Compare valuation against simulated competitors.
- Reset progress.

All state is stored in `localStorage`.
