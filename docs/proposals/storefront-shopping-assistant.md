# Storefront shopping assistant (agent + cart)

## Summary

Add a first-party storefront agent: a chat widget shoppers can talk to, that searches the live catalog and can add products to **their** shopping cart. It reuses nopCommerce cart, pricing, stock, and AI provider settings. It does not replace checkout.

This is a plan only. Do not implement until the approach is approved.

## Why this is not Jotform

`Nop.Plugin.Widgets.Jotform` already embeds a third-party chatbot. That iframe cannot call `IShoppingCartService`, cannot honor catalog visibility, and cannot refresh the header/mini-cart. A cart-capable assistant has to run on the storefront session and execute tools in process.

Catalog AI (product descriptions / meta tags) stays as-is. It is admin-only, single-prompt, and has no tool loop.

## Goals

- Shopper can open a persistent chat on public pages and ask in natural language (e.g. “running shoes under $80”).
- Assistant searches published, store-visible products and shows them in chat (name, price, image, PDP link).
- Assistant can add a **simple** product to the current customer’s cart (guest or registered) with quantity.
- After a successful add, header cart quantity and mini-cart HTML update the same way as `public.ajaxcart.js`.
- Configurable products: assistant loads required attributes, asks the shopper, then adds. If attributes are still missing, it does not add; it returns `IShoppingCartService` warnings and a PDP link.
- Merchant can enable/disable the widget, allow browse-only (no add-to-cart), and reuse existing Gemini / ChatGPT / DeepSeek keys.

## Non-goals (v1)

- Placing orders, capturing payment, or skipping checkout.
- Applying coupons, gift cards, or checkout attributes.
- Cross-customer data, admin catalog edits, or order history of other people.
- Native provider tool-calling APIs (OpenAI tools, Gemini function calling). v1 uses a provider-agnostic JSON tool protocol on top of `ArtificialIntelligenceHttpClient.SendQueryAsync`.
- Replacing Jotform. Both widgets can exist; merchants pick one or both.
- Streaming tokens, voice, or multi-storefront theming beyond DefaultClean + widget CSS.

## Recommended shape: plugin + one core hook

Ship **`Nop.Plugin.Misc.ShoppingAssistant`** (widget plugin, `IWidgetPlugin`).

| Layer | Responsibility |
| --- | --- |
| Core (small) | Expose a generic completion on `IArtificialIntelligenceService` (wrap existing `SendQueryAsync`). Optional setting `AllowStorefrontAssistant`. |
| Plugin | Widget, chat UI, `/shopping-assistant/chat` endpoint, tool executor, settings, localization. |
| Existing services | `IProductService.SearchProductsAsync`, `IPriceCalculationService.GetFinalPriceAsync`, `IShoppingCartService.AddToCartAsync` / `GetShoppingCartAsync`, `IPermissionService` (`PublicStore.ENABLE_SHOPPING_CART`). |

A plugin keeps the storefront JS/CSS optional and matches Jotform/Swiper. Putting the full agent in `Nop.Web` would couple DefaultClean to an optional LLM feature.

**Do not** inject `ArtificialIntelligenceHttpClient` from the plugin. Add:

```csharp
Task<string> CompleteAsync(string prompt);
```

to `IArtificialIntelligenceService` so catalog generation and the assistant share provider, timeout, and request logging.

## UX

- Launcher button in `PublicWidgetZones.BodyEndHtmlTagBefore` (same zone as Jotform). If both are enabled, offset the assistant so it does not cover Jotform.
- Panel: message list, textarea, send, optional starter chips (“What’s in my cart?”, “Show bestsellers”).
- Assistant messages may include product cards. Cards have “View” (PDP) and, when add-to-cart is allowed and the product is simple/in stock, “Add to cart”.
- User-clicked Add to cart hits the same plugin add endpoint as the tool (no silent second path).
- Loading and error states; never show raw model JSON to the shopper.
- Respect `StandardPermission.PublicStore.ENABLE_SHOPPING_CART`. If disabled, chat can still search; add tools and buttons are hidden.

Accessibility: `role="dialog"` on the panel, focus trap when open, `aria-label` on the launcher, `aria-live="polite"` for new messages, `focus-visible` on controls.

## Conversation protocol (v1)

Server holds a short transcript in the current session (not a new DB table). Cap messages (e.g. last 12 turns) and tool iterations (e.g. 5 per user message).

Each model call is one prompt: system instructions + tool schemas + transcript + latest user text. The model must return **only** JSON:

```json
{
  "message": "string, shopper-visible; empty when calling tools",
  "products": [ { "productId": 0, "name": "", "price": "", "url": "" } ],
  "tools": [ { "name": "search_products", "arguments": { } } ]
}
```

- If `tools` is non-empty: execute server-side, append tool results to the transcript, call the model again.
- If `tools` is empty: return `message` + optional `products` to the client.
- If parse fails: one retry with “return valid JSON only”; then a localized fallback, no cart mutation.

This works with today’s ChatGPT Responses / Gemini / DeepSeek **text** helpers. Native tool APIs can replace the JSON envelope later without changing the executor.

## Tools

All tools run as the **current** customer and **current** store. They never accept `customerId`.

| Tool | Behavior |
| --- | --- |
| `search_products` | `SearchProductsAsync` with keywords, optional `priceMin`/`priceMax`, `pageSize` capped (default 5, max 8), `visibleIndividuallyOnly: true`, current `storeId` / `languageId`. Return id, name, SKU, formatted price, stock flag, product URL, picture URL. |
| `get_product` | Published product in this store. Include short description, price, stock, required attributes (id, name, values), associated products for grouped parents, PDP URL. |
| `add_to_cart` | Load product, call `AddToCartAsync` with quantity (default 1) and optional attribute XML built from named values. Return warnings or success + cart summary. |
| `get_cart` | Current shopping cart: names, qty, unit/line prices, item count. |

v1 add-to-cart rules:

1. Grouped parent: do not add; return associated products and ask which child.
2. Required attributes missing: do not add; return attribute schema + warnings.
3. Rental / customer-entered price: do not add; return PDP URL.
4. `GetShoppingCartItemWarningsAsync` first; persist only when empty.
5. Quantity clamped to catalog min/max and `ShoppingCartSettings.MaximumShoppingCartItems`.

Optional v1 follow-up (only if add-to-cart is on): `update_cart_item` / `remove_cart_item` by **cart item id** from `get_cart`, not by raw product id guessing.

## HTTP API (plugin)

`POST /shopping-assistant/chat`

- Body: `{ "message": "..." }` (size-capped).
- Auth: storefront cookie session + anti-forgery (same as ajax cart).
- Response: `{ "message", "products": [...], "cart": { "itemCount", "updatetopcartsectionhtml", "updateflyoutcartsectionhtml" } }`. Cart HTML is only present after a successful mutation.

`POST /shopping-assistant/add-to-cart`

- Body: `{ "productId", "quantity" }` for simple products from UI cards.
- Same validation as the tool.

Rate limit: per-customer (and IP fallback) N chat posts per minute. Disabled plugin or missing API key → 404, no model call.

## Settings

Plugin settings (overridable per store), plus one catalog AI flag:

**Core `ArtificialIntelligenceSettings`**

- `AllowStorefrontAssistant` — nested under `Enabled`; requires a configured provider key.

**Plugin `ShoppingAssistantSettings`**

- `Enabled`
- `AllowAddToCart` (browse-only when false)
- `StarterPrompts` (newline-separated)
- `MaxProductsPerReply`
- `MaxToolIterations`
- `SystemPrompt` (optional override; default localized)

Admin: plugin Configure page. If core AI is disabled, show a link to Catalog AI settings instead of duplicating API keys.

## Security

- Tools cannot change another customer’s cart; they use `IWorkContext.GetCurrentCustomerAsync`.
- Strip HTML from model `message` before render (`WebUtility.HtmlEncode` or existing encoding helpers). Product URLs only from `INopUrlHelper` / known routes, never from model-supplied hrefs.
- Do not send PII of other customers, payment data, or admin secrets in prompts. Cart tool results include product names and totals only.
- Prompt injection: tool arguments are validated (int ids, decimal prices, quantity bounds). The model cannot pass arbitrary XML into `attributesXml`; the executor maps attribute **value ids** through `IProductAttributeParser`.
- Log failures via existing `ILogger`; honor `LogRequests` for the completion call.
- Activity log on successful agent add: e.g. `PublicStore.ShoppingAssistant.AddToCart`.

## Files to add or touch (when implementing)

**Core**

- `IArtificialIntelligenceService` / `ArtificialIntelligenceService` — `CompleteAsync`
- `ArtificialIntelligenceSettings` (+ admin model, catalog AI view, locale resources)

**Plugin `src/Plugins/Nop.Plugin.Misc.ShoppingAssistant/`**

- `plugin.json`, csproj, `Notes.txt`, solution entry (Plugins folder)
- `ShoppingAssistantPlugin` (`IWidgetPlugin`), defaults, settings
- `Infrastructure/RouteProvider`, `NopStartup`
- `Controllers/ShoppingAssistantController` (public), `ShoppingAssistantAdminController` (configure)
- `Services/ShoppingAssistantService` (loop + JSON parse)
- `Services/ShoppingAssistantToolExecutor` (search / get / cart)
- `Components/ShoppingAssistantViewComponent` + `Views/PublicInfo.cshtml`
- `Content/js/shopping-assistant.js`, `Content/css/shopping-assistant.css`
- `Views/Configure.cshtml`, localization on install

**Tests (`Nop.Tests`)**

- Executor: search keywords; add simple product; missing attributes does not add; grouped parent does not add; guest cart works.
- JSON parse: valid tool call; invalid JSON; extra keys ignored.
- Controller: disabled → 404; add-to-cart flag off rejects add.

Do not add a project reference from `Nop.Tests` to the plugin if that recreates the MEYER-16 issue. Prefer testing executor types in a way consistent with other plugin tests, or extract the executor contracts so core tests can cover cart rules.

## Implementation order

1. Core `CompleteAsync` + `AllowStorefrontAssistant` (no UI yet).
2. Plugin skeleton: install, widget zone, empty panel, configure page.
3. Tool executor against existing catalog/cart services (tests first).
4. Chat loop + public POST; wire product cards.
5. Add-to-cart tool + ajax cart HTML refresh.
6. Attribute / grouped / rental fallbacks.
7. Rate limit, encoding, activity log, locales.
8. Manual pass: guest add, registered add, stock warning, Jotform both enabled.

## Test plan (manual)

- AI disabled or no key: no launcher.
- Browse-only: search works; add tool and buttons absent.
- “Add 2 of [simple product]”: cart count +2, mini-cart lists it, cart page matches.
- Configurable product: assistant asks for attributes; add without them fails; after values, add succeeds.
- Guest then login: existing `MigrateShoppingCartAsync` still applies; no assistant-specific merge.
- Permission off for guests: add refused with the same policy as the catalog.
- Invalid model JSON: localized error, cart unchanged.

## Trade-offs

**JSON envelope vs native tools.** Envelope is one code path for three providers and matches `CreateRequest(query)`. It is worse at schema adherence than OpenAI tools. Mitigate with a repair retry and a hard iteration cap. Revisit native tools if ChatGPT becomes the only supported assistant provider.

**Session transcript vs DB.** Session dies on app recycle and is not shared across devices. That is acceptable for v1 and avoids GDPR tables. Persistent threads can be a later entity keyed by customer.

**Auto-add vs confirm.** Auto-add for simple in-stock products is the actual “agent added it for me” demo. Configurable / expensive items should confirm in chat first (`AllowAddToCart` does not mean “never ask”). If merchants want confirm-always, add a setting rather than weakening the happy path.

**Jotform overlap.** Two launchers is ugly. Document “enable one conversational widget.” Do not uninstall Jotform from this feature.

## Open questions (defaults if unanswered)

1. Confirm-always vs auto-add for simple products → **auto-add** when `AllowAddToCart` is true.
2. Plugin system name → `Misc.ShoppingAssistant`.
3. Default starter chips → “Help me find a product”, “What’s in my cart?”.
4. Persist chats → **no** in v1.
