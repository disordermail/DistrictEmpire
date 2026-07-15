import { serve } from "https://deno.land/std@0.224.0/http/server.ts";
import { createClient } from "https://esm.sh/@supabase/supabase-js@2.45.4";

type JsonBody = Record<string, unknown>;

const supabaseUrl = Deno.env.get("SUPABASE_URL");
const serviceRoleKey = Deno.env.get("SUPABASE_SERVICE_ROLE_KEY");

if (!supabaseUrl || !serviceRoleKey) {
  throw new Error("SUPABASE_URL and SUPABASE_SERVICE_ROLE_KEY are required.");
}

const supabase = createClient(supabaseUrl, serviceRoleKey, {
  auth: { persistSession: false },
});

serve(async (request) => {
  const url = new URL(request.url);

  if (request.method === "GET" && url.pathname.endsWith("/v1/countries")) {
    const { data, error } = await supabase
      .from("countries")
      .select("code, source_locale, currency_code")
      .eq("active", true);

    if (error) {
      return json({ error: error.message }, 500);
    }

    return json({ countries: data });
  }

  if (request.method === "POST" && url.pathname.endsWith("/v1/properties/purchase")) {
    const idempotencyKey = request.headers.get("Idempotency-Key");

    if (!idempotencyKey) {
      return json({ error: "Idempotency-Key header is required." }, 400);
    }

    const body = await readJson(request);
    const playerId = String(body.player_id ?? "");
    const listingId = String(body.listing_id ?? "");

    if (!playerId || !listingId) {
      return json({ error: "player_id and listing_id are required." }, 400);
    }

    return json({
      accepted: true,
      server_time: new Date().toISOString(),
      idempotency_key: idempotencyKey,
      next_step: "Implement transactional listing purchase RPC.",
    });
  }

  return json({ error: "Not found." }, 404);
});

async function readJson(request: Request): Promise<JsonBody> {
  try {
    return await request.json();
  } catch {
    return {};
  }
}

function json(payload: unknown, status = 200): Response {
  return new Response(JSON.stringify(payload), {
    status,
    headers: {
      "content-type": "application/json; charset=utf-8",
    },
  });
}
