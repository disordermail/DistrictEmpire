create extension if not exists postgis;
create extension if not exists pgcrypto;

create table countries (
  code text primary key check (char_length(code) = 2),
  source_locale text not null,
  currency_code text not null check (char_length(currency_code) = 3),
  active boolean not null default false
);

create table country_pack_versions (
  id uuid primary key default gen_random_uuid(),
  country_code text not null references countries(code),
  version text not null,
  payload jsonb not null,
  created_at timestamptz not null default now(),
  unique (country_code, version)
);

create table players (
  id uuid primary key default gen_random_uuid(),
  auth_user_id uuid unique,
  home_country_code text references countries(code),
  created_at timestamptz not null default now()
);

create table cities (
  id uuid primary key default gen_random_uuid(),
  country_code text not null references countries(code),
  slug text not null,
  localization_key text not null,
  boundary geography(multipolygon, 4326),
  unique (country_code, slug)
);

create table districts (
  id uuid primary key default gen_random_uuid(),
  city_id uuid not null references cities(id),
  slug text not null,
  localization_key text not null,
  boundary geography(multipolygon, 4326),
  unique (city_id, slug)
);

create table buildings (
  id uuid primary key default gen_random_uuid(),
  district_id uuid not null references districts(id),
  localization_key text not null,
  location geography(point, 4326) not null,
  market_tier integer not null check (market_tier > 0),
  created_at timestamptz not null default now()
);

create table market_listings (
  id uuid primary key default gen_random_uuid(),
  building_id uuid not null references buildings(id),
  price_minor_units bigint not null check (price_minor_units >= 0),
  currency_code text not null check (char_length(currency_code) = 3),
  status text not null default 'active',
  created_at timestamptz not null default now()
);

create table ownerships (
  id uuid primary key default gen_random_uuid(),
  player_id uuid not null references players(id),
  building_id uuid not null references buildings(id),
  acquired_at timestamptz not null default now(),
  unique (player_id, building_id)
);

create table wallet_ledger (
  id uuid primary key default gen_random_uuid(),
  player_id uuid not null references players(id),
  amount_minor_units bigint not null,
  currency_code text not null check (char_length(currency_code) = 3),
  reason text not null,
  idempotency_key text not null,
  created_at timestamptz not null default now(),
  unique (player_id, idempotency_key)
);

create table localization_entries (
  locale text not null,
  key text not null,
  value text not null,
  updated_at timestamptz not null default now(),
  primary key (locale, key)
);

alter table countries enable row level security;
alter table country_pack_versions enable row level security;
alter table players enable row level security;
alter table cities enable row level security;
alter table districts enable row level security;
alter table buildings enable row level security;
alter table market_listings enable row level security;
alter table ownerships enable row level security;
alter table wallet_ledger enable row level security;
alter table localization_entries enable row level security;
