using System;
using System.Linq;
using DistrictEmpire.Application;
using DistrictEmpire.Domain;
using DistrictEmpire.Infrastructure;
using UnityEngine;
using UnityEngine.UIElements;

namespace DistrictEmpire.Presentation
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class DistrictEmpireBootstrap : MonoBehaviour
    {
        private GameService game;
        private VisualElement root;
        private ScrollView content;
        private string screen = "Today";
        private string selectedPropertyId = "old-town";
        private bool mapSheetOpen;
        private bool menuOpen;
        private float nextClockRefresh;
        private float auctionPullStartY;
        private bool auctionPullArmed;
        private int auctionRefreshCount;

        private void Awake()
        {
            game = new GameService(new JsonLocalGameRepository(), new GameClock());
            root = GetComponent<UIDocument>().rootVisualElement;
            var style = Resources.Load<StyleSheet>("DistrictEmpireStyle");
            if (style != null) root.styleSheets.Add(style);
            root.AddToClassList("app-shell");
            root.style.flexGrow = 1;
            Render();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextClockRefresh) return;
            nextClockRefresh = Time.unscaledTime + 1f;
            var lifecycleBefore = LifecycleSignature();
            game.Tick();
            if (!menuOpen && lifecycleBefore != LifecycleSignature()) Render();
        }

        private void Render()
        {
            root.Clear();
            root.Add(BuildHeader());
            content = new ScrollView { name = "ScreenContent", verticalScrollerVisibility = ScrollerVisibility.Hidden };
            content.touchScrollBehavior = ScrollView.TouchScrollBehavior.Elastic;
            content.AddToClassList("screen-scroll");
            root.Add(content);
            switch (screen)
            {
                case "City": RenderMap(); break;
                case "Auctions": RenderAuctions(); break;
                case "Tasks": RenderTasks(); break;
                case "Property": RenderProperty(); break;
                case "Empire": RenderCompany(); break;
                case "Shop": RenderShop(); break;
                case "Events": RenderEvents(); break;
                default: RenderToday(); break;
            }
            root.Add(BuildNav());
            if (screen == "City" && mapSheetOpen) root.Add(BuildMapSheet());
        }

        private VisualElement BuildHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("top-header");
            var row = UiKit.Row("header-row");
            var title = new VisualElement();
            title.Add(UiKit.Text("WARSZAWA · OFFLINE MVP", 11, true, UiKit.Muted));
            title.Add(UiKit.Text("District Empire", 25, true));
            row.Add(title);
            var controls = UiKit.Row("header-controls");
            var level = new VisualElement(); level.AddToClassList("level-pill");
            level.Add(UiKit.Text("LVL", 9, true, UiKit.Muted));
            level.Add(UiKit.Text(game.State.CompanyLevel.ToString(), 18, true, UiKit.Amber));
            controls.Add(level);
            var menu = UiKit.Button("☰", ShowMenu, "icon"); menu.tooltip = "Company headquarters"; controls.Add(menu);
            row.Add(controls);
            header.Add(row);
            var wallet = new VisualElement(); wallet.AddToClassList("wallet-strip");
            wallet.Add(WalletChip("Cash", Money(game.State.Cash)));
            wallet.Add(WalletChip("Rent/day", Money(OccupiedRent())));
            wallet.Add(WalletChip("Buildings", game.State.Properties.Count(p => p.IsOwned).ToString()));
            wallet.Add(WalletChip("Influence", game.State.Influence.ToString()));
            header.Add(wallet);
            return header;
        }

        private VisualElement WalletChip(string label, string value)
        {
            var chip = new VisualElement(); chip.AddToClassList("wallet-chip");
            chip.Add(UiKit.Text(label, 10, false, UiKit.Muted));
            var amount = UiKit.Text(value, 11, true, UiKit.Green); amount.style.whiteSpace = WhiteSpace.NoWrap; chip.Add(amount);
            return chip;
        }

        private VisualElement BuildNav()
        {
            var nav = new VisualElement(); nav.AddToClassList("bottom-nav");
            AddNavButton(nav, "City", "CITY", "⌖");
            AddNavButton(nav, "Today", "TODAY", "●");
            AddNavButton(nav, "Empire", "EMPIRE", "▣");
            AddNavButton(nav, "Shop", "SHOP", "+");
            return nav;
        }

        private void AddNavButton(VisualElement nav, string destination, string label, string icon)
        {
            var button = new Button(() => { screen = destination; Render(); });
            button.AddToClassList("nav-tab");
            if (screen == destination) button.AddToClassList("nav-tab-active");
            button.Add(UiKit.Text(icon, 16, true));
            button.Add(UiKit.Text(label, 9, true));
            if (destination == "Today" && ImportantActionCount() > 0)
            {
                var badge = UiKit.Text(ImportantActionCount().ToString(), 9, true); badge.AddToClassList("nav-badge"); button.Add(badge);
            }
            nav.Add(button);
        }

        private void RenderToday()
        {
            var owned = game.State.Properties.Where(p => p.IsOwned).ToList();
            var desk = UiKit.Card("briefing"); desk.AddToClassList("today-hero");
            desk.Add(UiKit.Text("DISTRICT EMPIRE", 11, true, UiKit.Muted));
            desk.Add(UiKit.Text("Good morning, Paweł.", 24, true));
            desk.Add(UiKit.Text("Empire value " + Money(CompanyValue()), 14, true, UiKit.Blue));
            var metrics = new VisualElement(); metrics.AddToClassList("briefing-metrics");
            metrics.Add(Metric("Rent ready", Money(game.State.RentReady), game.State.RentReady > 0 ? "income" : "neutral"));
            metrics.Add(Metric("Repairs", TaskCount().ToString(), TaskCount() > 0 ? "attention" : "neutral"));
            metrics.Add(Metric("Applications", owned.Sum(p => p.Stage == PropertyStage.Applications ? p.Applicants.Count : 0).ToString(), "neutral"));
            metrics.Add(Metric("Next action", ImportantActionCount().ToString(), ImportantActionCount() > 0 ? "attention" : "neutral"));
            desk.Add(metrics); content.Add(desk);

            var hint = game.GetNextHint();
            var next = game.State.Properties.FirstOrDefault(property => property.Id == hint.PropertyId);
            var continueCard = UiKit.Card(next == null ? "neutral" : ToneFor(next)); continueCard.AddToClassList("continue-card");
            continueCard.Add(UiKit.Text("CONTINUE MANAGING", 10, true, UiKit.Muted));
            continueCard.Add(UiKit.Text(hint.Title, 18, true));
            continueCard.Add(UiKit.Text(hint.Detail, 12, false, UiKit.Muted));
            continueCard.Add(UiKit.Button(hint.Action, ContinueManaging, "primary")); content.Add(continueCard);

            var rent = UiKit.Card(game.State.RentReady > 0 ? "income" : "neutral"); rent.AddToClassList("rent-card");
            var copy = new VisualElement(); copy.AddToClassList("rent-copy");
            copy.Add(UiKit.Text(game.State.RentReady > 0 ? "RENT READY TO COLLECT" : "NEXT RENT", 10, true, game.State.RentReady > 0 ? UiKit.Green : UiKit.Muted));
            copy.Add(UiKit.Text(game.State.RentReady > 0 ? Money(game.State.RentReady) : "Tomorrow", 24, true));
            copy.Add(UiKit.Text($"{Money(ResidentialRent())} homes · {Money(BusinessRent())} businesses", 11, false, UiKit.Muted));
            rent.Add(copy);
            var collect = UiKit.Button(game.State.RentReady > 0 ? "$ Collect rent" : "View rent schedule", () =>
            {
                var amount = game.State.RentReady;
                var collected = game.CollectRent();
                Render();
                if (collected) ShowCelebration("RENT COLLECTED", "+" + Money(amount), "Maria paid her rent. +25 XP · +1 influence");
                else ShowToast("Maria's next payment is due tomorrow.");
            }, game.State.RentReady > 0 ? "income" : "secondary");
            collect.AddToClassList("rent-action"); rent.Add(collect); content.Add(rent);
            content.Add(SectionHeading("CITY STORIES", "What changed while you were away"));
            RenderLivingBriefing(owned);
            RenderGoals();
        }

        private void AddPropertyGroup(string title, System.Collections.Generic.List<Property> properties)
        {
            if (properties.Count == 0) return;
            var heading = UiKit.Text(title + " · " + properties.Count, 10, true, UiKit.Muted); heading.AddToClassList("portfolio-group-title"); content.Add(heading);
            foreach (var property in properties.OrderBy(StatusRank)) content.Add(PropertyCard(property, true));
        }

        private void RenderLivingBriefing(System.Collections.Generic.List<Property> owned)
        {
            var card = UiKit.Card("neutral"); card.AddToClassList("news-card");
            card.Add(UiKit.Text("TODAY'S CITY NEWS", 10, true, UiKit.Muted));
            card.Add(NewsLine("Maria paid rent", "Mokotow Starter · relationship +1", "income"));
            if (TaskCount() > 0) card.Add(NewsLine("Kitchen pipe needs attention", "Maria is waiting for a repair", "attention"));
            foreach (var activity in game.State.NpcActivities.Take(2))
                card.Add(NewsLine(activity.Investor + " " + activity.Title, activity.Detail, "npc"));
            var cityEvent = game.State.Events.FirstOrDefault();
            if (cityEvent != null) card.Add(NewsLine(cityEvent.Title, cityEvent.Detail, "neutral"));
            if (owned.Any(p => p.Stage == PropertyStage.Applications)) card.Add(NewsLine("New applicants are waiting", "A decision can start a new lease today", "income"));
            content.Add(card);
        }

        private void RenderGoals()
        {
            var available = game.State.Goals.Where(goal => !goal.Claimed).ToList();
            if (available.Count == 0) return;
            var card = UiKit.Card("briefing"); card.AddToClassList("news-card");
            card.Add(UiKit.Text("COMPANY GOALS", 10, true, UiKit.Muted));
            foreach (var goal in available)
            {
                card.Add(UiKit.Text(goal.Title + " · " + Math.Min(goal.Progress, goal.Target) + "/" + goal.Target, 13, true));
                card.Add(UiKit.Text(goal.Detail, 10, false, UiKit.Muted));
                if (goal.Progress >= goal.Target)
                    card.Add(UiKit.Button("Claim 500 PLN + 2 influence", () => { if (game.ClaimGoal(goal.Id)) { Render(); ShowCelebration("GOAL COMPLETE", "500 PLN · +2 influence", "Your company is gaining momentum."); } }, "income"));
            }
            content.Add(card);
        }

        private VisualElement NewsLine(string title, string detail, string tone)
        {
            var line = UiKit.Row("news-line"); line.AddToClassList("news-" + tone);
            var dot = new VisualElement(); dot.AddToClassList("news-dot"); line.Add(dot);
            var copy = new VisualElement(); copy.Add(UiKit.Text(title, 12, true)); copy.Add(UiKit.Text(detail, 10, false, UiKit.Muted)); line.Add(copy); return line;
        }

        private VisualElement Metric(string label, string value, string tone)
        {
            var metric = new VisualElement(); metric.AddToClassList("briefing-metric"); metric.AddToClassList("metric-" + tone);
            metric.Add(UiKit.Text(label, 10, false, UiKit.Muted)); metric.Add(UiKit.Text(value, 14, true)); return metric;
        }

        private VisualElement SectionHeading(string eyebrow, string title)
        {
            var heading = new VisualElement(); heading.AddToClassList("section-heading");
            heading.Add(UiKit.Text(eyebrow, 10, true, UiKit.Muted)); heading.Add(UiKit.Text(title, 16, true)); return heading;
        }

        private void RenderMap()
        {
            content.Add(SectionHeading("WARSAW INVESTMENT MAP", "What can I buy?"));
            content.Add(UiKit.Text("Tap a building to inspect its price, rent potential and current ownership.", 12, false, UiKit.Muted));
            var map = new VisualElement(); map.AddToClassList("map-stage");
            var live = game.State.NpcActivities.FirstOrDefault();
            var activity = UiKit.Text(live == null ? "Market activity is loading" : live.Investor + " " + live.Title, 10, true); activity.AddToClassList("map-activity"); map.Add(activity);
            foreach (var property in game.State.Properties)
            {
                var pin = new Button(() => { selectedPropertyId = property.Id; OpenPropertyFromMap(property); });
                pin.AddToClassList("map-pin");
                pin.AddToClassList(MapPinClass(property));
                pin.style.left = new Length(property.MapX, LengthUnit.Percent);
                pin.style.top = new Length(property.MapY, LengthUnit.Percent);
                pin.Add(UiKit.Text(property.Icon, 9, true));
                var tier = UiKit.Text(property.Tier.ToString(), 9, true); tier.AddToClassList("pin-tier"); pin.Add(tier);
                map.Add(pin);
            }
            content.Add(map);
            var legend = UiKit.Card("neutral"); legend.AddToClassList("map-legend");
            legend.Add(UiKit.Text("YOUR BUILDINGS", 10, true, UiKit.Green)); legend.Add(UiKit.Text("Available properties are blue. Your assets are green.", 12, false, UiKit.Muted)); content.Add(legend);
            var npc = UiKit.Card("briefing"); npc.AddToClassList("npc-card"); npc.Add(UiKit.Text("NEARBY INVESTOR ACTIVITY", 10, true, UiKit.Muted));
            foreach (var item in game.State.NpcActivities) { npc.Add(UiKit.Text(item.Investor + " " + item.Title, 13, true)); npc.Add(UiKit.Text(item.Detail, 11, false, UiKit.Muted)); }
            content.Add(npc);
        }

        private void OpenPropertyFromMap(Property property)
        {
            selectedPropertyId = property.Id;
            mapSheetOpen = true;
            Render();
        }

        private VisualElement BuildMapSheet()
        {
            var property = game.State.Properties.FirstOrDefault(candidate => candidate.Id == selectedPropertyId);
            if (property == null) return new VisualElement();
            var sheet = UiKit.Card(ToneFor(property)); sheet.AddToClassList("map-bottom-sheet");
            var top = UiKit.Row("card-top");
            var name = new VisualElement(); name.Add(UiKit.Text(property.Name, 19, true)); name.Add(UiKit.Text(property.District + " · " + Status(property), 11, true, StatusColor(property))); top.Add(name);
            top.Add(UiKit.Button("×", () => { mapSheetOpen = false; Render(); }, "icon")); sheet.Add(top);
            sheet.Add(UiKit.Text(NextStory(property), 13, false, UiKit.Muted));
            sheet.Add(UiKit.Text(property.Stage == PropertyStage.Occupied ? "Income " + Money(game.EffectiveDailyRent(property)) + " / day" : "Potential " + Money(property.BaseDailyRent) + " / day", 13, true, UiKit.Green));
            var share = property.BuildingTotalUnits == 0 ? 0 : property.BuildingOwnedUnits * 100 / property.BuildingTotalUnits;
            sheet.Add(UiKit.Text("Building share " + property.BuildingOwnedUnits + "/" + property.BuildingTotalUnits + " · " + share + "% controlled", 11, true, UiKit.Blue));
            var affordable = property.IsOwned || game.State.Cash >= property.Price;
            var action = UiKit.Button(affordable ? MapActionLabel(property) : "Need " + Money(property.Price - game.State.Cash), () => RunMapAction(property), affordable ? "primary" : "locked");
            if (!affordable)
            {
                action.SetEnabled(false);
                action.tooltip = "Collect rent or choose a more affordable opportunity first.";
            }
            sheet.Add(action);
            return sheet;
        }

        private void ContinueManaging()
        {
            var hint = game.GetNextHint();
            var next = game.State.Properties.FirstOrDefault(property => property.Id == hint.PropertyId);
            screen = "City";
            mapSheetOpen = next != null;
            if (next != null) selectedPropertyId = next.Id;
            Render();
        }

        private void RunMapAction(Property property)
        {
            if (!property.IsOwned) { BuyFromMarket(property); return; }
            if (property.Stage == PropertyStage.Occupied && game.State.RentReady > 0)
            {
                var collected = game.State.RentReady;
                game.CollectRent();
                Render();
                ShowCelebration("RENT COLLECTED", "+" + Money(collected), property.TenantName + " paid today. +25 XP · +1 influence");
                return;
            }
            if (property.Condition < 90)
            {
                if (game.Repair(property.Id)) { Render(); ShowCelebration("REPAIR COMPLETE", "+condition", "Your tenant can get back to normal."); }
                else ShowToast("You need 450 PLN for this repair.");
                return;
            }
            mapSheetOpen = false;
            screen = "Property";
            Render();
        }

        private string MapActionLabel(Property property)
        {
            if (!property.IsOwned) return "Place bid";
            if (property.Condition < 90) return "Repair now";
            if (property.Stage == PropertyStage.Occupied && game.State.RentReady > 0) return "Collect rent";
            return PropertyActionLabel(property);
        }

        private void RenderAuctions()
        {
            ConfigureAuctionPullToRefresh();
            content.Add(SectionHeading("AUCTION HOUSE", "What can I win today?"));
            var refresh = UiKit.Text(auctionRefreshCount == 0 ? "Pull down to refresh live auctions" : "Auction board refreshed just now", 11, true, UiKit.Blue); refresh.AddToClassList("auction-refresh-hint"); content.Add(refresh);
            var tabs = new VisualElement(); tabs.AddToClassList("market-tabs");
            foreach (var category in new[] { "Live", "Starter", "Retail", "Luxury" })
            {
                var tab = UiKit.Button(category, () => ShowToast(category + " auction filter selected."), "filter");
                if (category == "Live") tab.AddToClassList("filter-active"); tabs.Add(tab);
            }
            content.Add(tabs);
            foreach (var property in game.State.Properties.Where(p => !p.IsOwned).OrderBy(p => p.Price))
            {
                var card = UiKit.Card(); card.AddToClassList("market-card");
                var top = UiKit.Row("card-top");
                var identity = new VisualElement();
                identity.Add(UiKit.Text(property.Icon, 11, true, UiKit.Blue)); identity.Add(UiKit.Text(property.Name, 17, true)); identity.Add(UiKit.Text(property.District + " · " + property.Category, 11, false, UiKit.Muted));
                top.Add(identity); top.Add(Badge("TIER " + property.Tier, "tier-badge")); card.Add(top);
                card.Add(UiKit.Text("Bid " + Money(property.Price) + " · " + (property.Tier == 3 ? "Ends tomorrow" : "Ends in 18 min") + " · Est. " + Money(property.BaseDailyRent) + " / day", 12, false, UiKit.Muted));
                var affordable = game.State.Cash >= property.Price;
                var buy = UiKit.Button(affordable ? "Place bid · " + Money(property.Price) : "Need " + Money(property.Price - game.State.Cash) + " more", () => BuyFromMarket(property), affordable ? "income" : "locked");
                if (!affordable) { buy.SetEnabled(false); buy.tooltip = "Build cash flow to afford this property."; }
                card.Add(buy); content.Add(card);
            }
            var dream = UiKit.Card("waiting"); dream.AddToClassList("dream-card"); dream.Add(UiKit.Text("DREAM PROPERTY", 10, true, UiKit.Amber)); dream.Add(UiKit.Text("Royal Riverside Tower", 19, true)); dream.Add(UiKit.Text("Locked · unlock at Company Level 4", 12, true, UiKit.Amber)); dream.Add(UiKit.Text("Estimated rent 9,200 PLN / day · Luxury auction tomorrow", 11, false, UiKit.Muted)); content.Add(dream);
        }

        private void ConfigureAuctionPullToRefresh()
        {
            content.RegisterCallback<PointerDownEvent>(evt =>
            {
                auctionPullStartY = evt.position.y;
                auctionPullArmed = content.scrollOffset.y <= 0;
            });
            content.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (auctionPullArmed && evt.position.y - auctionPullStartY > 60f && content.scrollOffset.y <= 2f) RefreshAuctions();
                auctionPullArmed = false;
            });
        }

        private void RefreshAuctions()
        {
            auctionRefreshCount++;
            Render();
            ShowCelebration("AUCTION BOARD REFRESHED", "New bids visible", "Anna Kowalska and Nova Estates are watching nearby listings.");
        }

        private void BuyFromMarket(Property property)
        {
            if (game.Buy(property.Id)) { selectedPropertyId = property.Id; screen = "City"; mapSheetOpen = true; ShowToast("Purchase accepted. Notary transfer has started."); }
            else ShowToast("You need more cash to buy " + property.Name + ".");
            Render();
        }

        private void RenderTasks()
        {
            content.Add(SectionHeading("OPERATIONS", "Tasks"));
            var tasks = game.State.Properties.Where(p => p.IsOwned && p.Condition < 90).ToList();
            if (tasks.Count == 0) { content.Add(EmptyCard("All clear", "Your portfolio has no maintenance work today.")); return; }
            foreach (var property in tasks)
            {
                var card = UiKit.Card("attention"); card.AddToClassList("task-card");
                card.Add(UiKit.Text("MAINTENANCE REQUIRED", 10, true, UiKit.Amber)); card.Add(UiKit.Text("Repair " + property.Name, 17, true));
                card.Add(UiKit.Text("Condition " + property.Condition + "% · Plumbing inspection and repair", 12, false, UiKit.Muted));
                card.Add(UiKit.Button("Repair for 450 PLN", () => { if (game.Repair(property.Id)) ShowToast("Maintenance completed. Condition improved."); else ShowToast("You need 450 PLN for this repair."); Render(); }, "primary")); content.Add(card);
            }
        }

        private void RenderProperty()
        {
            var property = game.State.Properties.FirstOrDefault(p => p.Id == selectedPropertyId);
            if (property == null) { screen = "Today"; Render(); return; }
            var hero = UiKit.Card(property.IsOwned ? "income" : "neutral"); hero.AddToClassList("property-hero");
            hero.Add(UiKit.Text(property.District.ToUpperInvariant() + " · " + property.Category.ToUpperInvariant(), 10, true, UiKit.Muted));
            hero.Add(UiKit.Text(property.Icon, 13, true, UiKit.Blue)); hero.Add(UiKit.Text(property.Name, 23, true));
            hero.Add(UiKit.Text(Status(property), 13, true, StatusColor(property)));
            hero.Add(PropertyFacts(property)); content.Add(hero);

            if (!property.IsOwned)
            {
                content.Add(UiKit.Button("Buy for " + Money(property.Price), () => BuyFromMarket(property), "primary"));
            }
            else if (property.Stage == PropertyStage.Notary)
            {
                var notary = FlowCard("WAITING FOR NOTARY", game.Countdown(property), "Documents are being checked. Expected today.");
                notary.Add(UiKit.Button("Use 5 influence to sign documents", () =>
                {
                    var completed = game.SpeedUpNotary(property.Id);
                    Render();
                    if (completed) ShowCelebration("DOCUMENTS SIGNED", "+25 XP", "Your property is ready for a purpose.");
                    else ShowToast("You need 5 influence to speed up the notary.");
                }, "waiting")); content.Add(notary);
            }
            else if (property.Stage == PropertyStage.ChoosingUse)
            {
                var choice = FlowCard("OWNERSHIP TRANSFER COMPLETE", "How should this property earn money?", "Choose a home rental or a business tenant.");
                choice.Add(UiKit.Button("Rent as a home", () => { game.ChooseUse(property.Id, PropertyUse.Residential); Render(); }, "primary"));
                choice.Add(UiKit.Button("Use as a business", () => { game.ChooseUse(property.Id, PropertyUse.Business); Render(); }, "secondary")); content.Add(choice);
            }
            else if (property.Stage == PropertyStage.Available)
            {
                var advertise = FlowCard("READY TO ADVERTISE", "Find the right tenant", "Your listing will accumulate views and applications shortly.");
                if (!property.Renovated) advertise.Add(UiKit.Button("Renovate · " + Money(900 + property.Tier * 300), () => Renovate(property), "secondary"));
                advertise.Add(UiKit.Button("Publish rental listing", () => { game.PublishListing(property.Id); Render(); }, "primary")); content.Add(advertise);
            }
            else if (property.Stage == PropertyStage.Listing)
            {
                content.Add(FlowCard("ADVERTISEMENT LIVE", game.Countdown(property), "Views and visits are growing. Applications will arrive soon."));
            }
            else if (property.Stage == PropertyStage.CancellingContract)
            {
                content.Add(FlowCard("CONTRACT ENDING", game.Countdown(property), "The tenant has been notified. The property can be sold once it is vacant."));
            }
            else if (property.Stage == PropertyStage.ForSale)
            {
                var sale = FlowCard("ON THE MARKET", Money(property.SalePrice), "City Bank has made a local offer for your vacant property.");
                var saleValue = property.SalePrice;
                sale.Add(UiKit.Button("Accept sale offer", () => { if (game.AcceptSaleOffer(property.Id)) { screen = "Today"; Render(); ShowCelebration("PROPERTY SOLD", "+" + Money(saleValue), "Sale completed. Your cash and company XP increased."); } }, "income")); content.Add(sale);
            }
            else if (property.Stage == PropertyStage.Applications)
            {
                content.Add(SectionHeading("APPLICATIONS WAITING", "Choose a tenant"));
                foreach (var applicant in property.Applicants)
                {
                    var applicantCard = UiKit.Card(); applicantCard.AddToClassList("applicant-card"); applicantCard.Add(UiKit.Text("APPLICANT", 10, true, UiKit.Muted));
                    applicantCard.Add(UiKit.Text(applicant.Name, 17, true)); applicantCard.Add(UiKit.Text(applicant.Role + " · " + Money(applicant.DailyRent) + " / day", 12, true, UiKit.Green)); applicantCard.Add(UiKit.Text(applicant.Story, 12, false, UiKit.Muted));
                    if (!applicant.Negotiated) applicantCard.Add(UiKit.Button("Negotiate rent +" + Money(applicant.IsBusiness ? 120 : 60), () => { if (game.NegotiateApplicant(property.Id, applicant.Id)) { Render(); ShowToast("Offer improved. Decide whether to sign."); } }, "secondary"));
                    applicantCard.Add(UiKit.Button("Accept applicant", () => { game.SelectApplicant(property.Id, applicant.Id); ShowToast("Lease signed with " + applicant.Name + "."); Render(); }, "primary")); content.Add(applicantCard);
                }
            }
            else
            {
                var occupied = FlowCard("OCCUPIED", property.TenantName + " · " + property.TenantRole, property.TenantStory + "\nRelationship " + property.Relationship + "/100 · Next payment tomorrow");
                if (property.Condition < 90) occupied.Add(UiKit.Button("Maintain property", () => { game.Repair(property.Id); Render(); }, "secondary")); content.Add(occupied);
                if (!property.Renovated) occupied.Add(UiKit.Button("Renovate · " + Money(900 + property.Tier * 300), () => Renovate(property), "secondary"));
                if (property.Level < 3) occupied.Add(UiKit.Button("Upgrade to level " + (property.Level + 1) + " · " + Money(1100 * property.Level * property.Tier), () => Upgrade(property), "primary"));
                if (property.Popularity < 3) occupied.Add(UiKit.Button("Promote locally · 3 influence", () => Promote(property), "secondary"));
                occupied.Add(UiKit.Button("End tenancy", () => { if (game.StartContractCancellation(property.Id)) { Render(); ShowToast("Contract cancellation started. The tenant has been notified."); } }, "danger"));
            }
            if (property.IsOwned && property.Stage == PropertyStage.Available)
            {
                var saleOption = UiKit.Card("neutral"); saleOption.Add(UiKit.Text("SELL THIS PROPERTY", 10, true, UiKit.Muted)); saleOption.Add(UiKit.Text("Vacant properties can be put on the market.", 12, false, UiKit.Muted)); saleOption.Add(UiKit.Button("Put property on market", () => { if (game.ListForSale(property.Id)) { Render(); ShowToast("Property is now listed for sale."); } }, "secondary")); content.Add(saleOption);
            }
            if (property.IsOwned) content.Add(BuildingCollectionCard(property));
            content.Add(UiKit.Button("Back to Today", () => { screen = "Today"; Render(); }, "secondary"));
        }

        private VisualElement PropertyFacts(Property property)
        {
            var facts = new VisualElement(); facts.AddToClassList("property-facts");
            facts.Add(Fact("LEVEL", property.Level.ToString())); facts.Add(Fact("VALUE", Money(property.Price))); facts.Add(Fact("RENT/DAY", Money(property.IsOwned && property.TenantDailyRent > 0 ? game.EffectiveDailyRent(property) : property.BaseDailyRent))); facts.Add(Fact("CONDITION", property.Condition + "%")); return facts;
        }

        private VisualElement BuildingCollectionCard(Property property)
        {
            var card = UiKit.Card("briefing"); card.AddToClassList("collection-card");
            card.Add(UiKit.Text("BUILDING COLLECTION", 10, true, UiKit.Muted)); card.Add(UiKit.Text(property.BuildingName, 17, true));
            var percent = property.BuildingTotalUnits == 0 ? 0 : property.BuildingOwnedUnits * 100 / property.BuildingTotalUnits;
            var progress = new VisualElement(); progress.AddToClassList("collection-progress");
            var fill = new VisualElement(); fill.AddToClassList("collection-fill"); fill.style.width = new Length(percent, LengthUnit.Percent); progress.Add(fill); card.Add(progress);
            card.Add(UiKit.Text(property.BuildingOwnedUnits + "/" + property.BuildingTotalUnits + " apartments controlled · " + percent + "% owned", 12, true, UiKit.Blue)); return card;
        }

        private VisualElement Fact(string label, string value)
        {
            var fact = new VisualElement(); fact.AddToClassList("fact"); fact.Add(UiKit.Text(label, 9, true, UiKit.Muted)); fact.Add(UiKit.Text(value, 12, true)); return fact;
        }

        private VisualElement FlowCard(string eyebrow, string title, string text)
        {
            var card = UiKit.Card("waiting"); card.AddToClassList("flow-card"); card.Add(UiKit.Text(eyebrow, 10, true, UiKit.Amber)); card.Add(UiKit.Text(title, 18, true)); card.Add(UiKit.Text(text, 12, false, UiKit.Muted)); return card;
        }

        private void RenderCompany()
        {
            content.Add(SectionHeading("YOUR COMPANY", "District Empire"));
            var card = UiKit.Card("briefing"); card.Add(UiKit.Text("PAWEŁ W. · CEO", 11, true, UiKit.Muted)); card.Add(UiKit.Text("Empire value " + Money(CompanyValue()), 24, true)); card.Add(UiKit.Text("Level " + game.State.CompanyLevel + " · " + game.State.Xp + "/" + (game.State.CompanyLevel * 100) + " XP · Warsaw", 12, false, UiKit.Muted));
            var metrics = new VisualElement(); metrics.AddToClassList("briefing-metrics");
            metrics.Add(Metric("Residential", game.State.Properties.Count(property => property.IsOwned && property.Use == PropertyUse.Residential).ToString(), "income"));
            metrics.Add(Metric("Commercial", game.State.Properties.Count(property => property.IsOwned && property.Use == PropertyUse.Business).ToString(), "neutral"));
            metrics.Add(Metric("Buildings", game.State.Properties.Count(property => property.IsOwned).ToString(), "neutral"));
            metrics.Add(Metric("Rating", "★★★☆☆", "neutral")); card.Add(metrics); content.Add(card);
            var landlordCost = 6 + game.State.LandlordSkill * 4;
            var landlord = UiKit.Card("income"); landlord.Add(UiKit.Text("LANDLORD", 10, true, UiKit.Green)); landlord.Add(UiKit.Text("Global rent +" + game.State.LandlordSkill + "%", 17, true)); landlord.Add(UiKit.Text("Every level raises rent from all occupied properties.", 12, false, UiKit.Muted));
            if (game.State.LandlordSkill < 5) landlord.Add(UiKit.Button("Train · " + landlordCost + " influence", () => UpgradeCompanySkill("landlord"), "income")); content.Add(landlord);
            var lawyerCost = 6 + game.State.LawyerSkill * 4;
            var lawyer = UiKit.Card("waiting"); lawyer.Add(UiKit.Text("LAWYER", 10, true, UiKit.Amber)); lawyer.Add(UiKit.Text("Paperwork " + (game.State.LawyerSkill == 0 ? "standard" : "-" + game.State.LawyerSkill * 2 + "m"), 17, true)); lawyer.Add(UiKit.Text("Each level shortens future property transfers by 2 minutes.", 12, false, UiKit.Muted));
            if (game.State.LawyerSkill < 5) lawyer.Add(UiKit.Button("Hire · " + lawyerCost + " influence", () => UpgradeCompanySkill("lawyer"), "waiting")); content.Add(lawyer);
            foreach (var item in new[] { "Finances", "Employees", "Agents" })
            {
                var row = UiKit.Card(); row.Add(UiKit.Text(item, 16, true)); row.Add(UiKit.Text(item == "Finances" ? "Income and expenses across your Warsaw empire." : "Build your organization as your company reaches higher levels.", 12, false, UiKit.Muted)); content.Add(row);
            }
        }

        private void UpgradeCompanySkill(string skillId)
        {
            if (game.UpgradeCompanySkill(skillId))
            {
                var level = skillId == "landlord" ? game.State.LandlordSkill : game.State.LawyerSkill;
                Render();
                ShowCelebration(skillId == "landlord" ? "LANDLORD TRAINED" : "LAWYER HIRED", skillId == "landlord" ? "Global rent +" + level + "%" : "Future paperwork -" + level * 2 + "m", "Your company level and influence updated.");
            }
            else ShowToast("You need more influence or this skill is fully trained.");
        }

        private void RenderShop()
        {
            content.Add(SectionHeading("COMPANY SUPPLIES", "Shop"));
            content.Add(UiKit.Text("Choose one useful boost for today's property work.", 12, false, UiKit.Muted));

            var reward = UiKit.Card(game.State.DailyRewardClaimed ? "neutral" : "income"); reward.AddToClassList("shop-card");
            reward.Add(UiKit.Text("DAILY REWARD", 10, true, game.State.DailyRewardClaimed ? UiKit.Muted : UiKit.Green)); reward.Add(UiKit.Text(game.State.DailyRewardClaimed ? "Claimed today" : "750 PLN + 15 XP", 19, true));
            if (!game.State.DailyRewardClaimed) reward.Add(UiKit.Button("Claim daily reward", () => { if (game.ClaimDailyReward()) { Render(); ShowCelebration("DAILY REWARD", "+750 PLN  ·  +15 XP", "Your company has fresh capital for today's decisions."); } }, "income"));
            content.Add(reward);

            var influence = UiKit.Card("briefing"); influence.AddToClassList("shop-card"); influence.Add(UiKit.Text("INFLUENCE PACK", 10, true, UiKit.Blue)); influence.Add(UiKit.Text("+5 influence", 19, true)); influence.Add(UiKit.Text("Useful for speeding up notary documents.", 12, false, UiKit.Muted));
            influence.Add(UiKit.Button("Buy for 600 PLN", () => { if (game.BuyInfluence()) { Render(); ShowToast("Influence added to your company."); } else ShowToast("You need 600 PLN."); }, "primary")); content.Add(influence);

            var reset = UiKit.Button("Reset local profile", ShowResetConfirmation, "danger"); reset.AddToClassList("reset-profile-button"); content.Add(reset);
            content.Add(UiKit.Button("Back to Today", () => { screen = "Today"; Render(); }, "secondary"));
        }

        private void RenderEvents()
        {
            content.Add(SectionHeading("CITY EVENTS", "What happened in Warsaw?"));
            content.Add(UiKit.Text("Claim one small company boost from each current city event.", 12, false, UiKit.Muted));
            foreach (var cityEvent in game.State.Events)
            {
                var card = UiKit.Card(cityEvent.Claimed ? "neutral" : "waiting"); card.AddToClassList("event-card");
                card.Add(UiKit.Text(cityEvent.Claimed ? "COMPLETED" : "CITY EVENT", 10, true, cityEvent.Claimed ? UiKit.Muted : UiKit.Amber)); card.Add(UiKit.Text(cityEvent.Title, 18, true)); card.Add(UiKit.Text(cityEvent.Detail, 12, false, UiKit.Muted)); card.Add(UiKit.Text(cityEvent.Reward, 12, true, UiKit.Green));
                if (!cityEvent.Claimed) card.Add(UiKit.Button("Join event", () => { if (game.ClaimEvent(cityEvent.Id)) { Render(); ShowCelebration("EVENT COMPLETE", cityEvent.Reward, "Your company is better known in the district."); } }, "waiting"));
                content.Add(card);
            }
            content.Add(UiKit.Button("Back to Today", () => { screen = "Today"; Render(); }, "secondary"));
        }

        private void ShowMenu()
        {
            if (menuOpen) return;
            menuOpen = true;
            var overlay = new VisualElement(); overlay.AddToClassList("menu-overlay");
            overlay.RegisterCallback<ClickEvent>(evt => { if (evt.target == overlay) CloseMenu(overlay); });
            var menu = new VisualElement(); menu.AddToClassList("company-menu");
            var profile = UiKit.Row("menu-profile"); profile.Add(UiKit.Text("PW", 16, true, UiKit.Blue));
            var profileText = new VisualElement(); profileText.Add(UiKit.Text("Paweł W.", 17, true)); profileText.Add(UiKit.Text("CEO · District Empire", 11, false, UiKit.Muted)); profile.Add(profileText); menu.Add(profile);
            menu.Add(UiKit.Text("Company value " + Money(CompanyValue()), 16, true));
            AddMenuSection(menu, "YOUR COMPANY", new[] { "Company", "Finances", "Employees", "Skills" }, overlay);
            AddMenuSection(menu, "OPPORTUNITIES", new[] { "Rankings", "Events & News" }, overlay);
            AddMenuSection(menu, "ACCOUNT", new[] { "Shop", "Friends", "Settings & Help" }, overlay);
            overlay.Add(menu); root.Add(overlay);
        }

        private void AddMenuSection(VisualElement menu, string title, string[] entries, VisualElement overlay)
        {
            var heading = UiKit.Text(title, 10, true, UiKit.Muted); heading.AddToClassList("menu-heading"); menu.Add(heading);
            foreach (var entry in entries)
            {
                var isLocked = entry == "Friends" || entry == "Settings & Help" || entry == "Employees" || entry == "Skills";
                var item = UiKit.Button(isLocked ? entry + " · Locked" : entry, () =>
                {
                    root.Remove(overlay);
                    menuOpen = false;
                    if (entry == "Company" || entry == "Finances" || entry == "Employees" || entry == "Skills") { screen = "Empire"; Render(); }
                    else if (entry == "Shop") { screen = "Shop"; Render(); }
                    else if (entry == "Events & News") { screen = "Events"; Render(); }
                    else ShowToast(entry + " is coming in the next local prototype pass.");
                }, isLocked ? "locked" : "menu");
                if (isLocked)
                {
                    item.SetEnabled(false);
                    item.tooltip = entry + " unlocks as your company grows.";
                }
                menu.Add(item);
            }
        }

        private void CloseMenu(VisualElement overlay)
        {
            menuOpen = false;
            if (overlay.parent != null) overlay.parent.Remove(overlay);
        }

        private void ShowToast(string message)
        {
            var toast = UiKit.Text(message, 12, true, new StyleColor(Color.white)); toast.AddToClassList("toast"); root.Add(toast);
            toast.schedule.Execute(() => { if (toast.parent != null) toast.parent.Remove(toast); }).StartingIn(2600);
        }

        private void ShowCelebration(string title, string rewards, string message)
        {
            var overlay = new VisualElement(); overlay.AddToClassList("celebration-overlay");
            var card = UiKit.Card("income"); card.AddToClassList("celebration-card"); card.Add(UiKit.Text(title, 11, true, UiKit.Green)); card.Add(UiKit.Text(rewards, 23, true)); card.Add(UiKit.Text(message, 12, false, UiKit.Muted));
            card.Add(UiKit.Button("Continue", () => root.Remove(overlay), "income")); overlay.Add(card); root.Add(overlay);
        }

        private void ShowResetConfirmation()
        {
            var overlay = new VisualElement(); overlay.AddToClassList("celebration-overlay");
            var card = UiKit.Card("attention"); card.AddToClassList("celebration-card");
            card.Add(UiKit.Text("RESET LOCAL PROFILE?", 11, true, UiKit.Amber)); card.Add(UiKit.Text("Start District Empire again", 20, true)); card.Add(UiKit.Text("This deletes the local cash, properties, shop rewards and lifecycle progress on this device.", 12, false, UiKit.Muted));
            card.Add(UiKit.Button("Reset profile", () => { game.ResetProgress(); root.Remove(overlay); screen = "Today"; Render(); ShowCelebration("PROFILE RESET", "Fresh start", "Maria and your starter apartment are ready again."); }, "danger"));
            card.Add(UiKit.Button("Keep my progress", () => root.Remove(overlay), "secondary")); overlay.Add(card); root.Add(overlay);
        }

        private void Renovate(Property property)
        {
            if (game.Renovate(property.Id))
            {
                Render();
                ShowCelebration("RENOVATION COMPLETE", "+rent potential", "The property is now more attractive to tenants.");
            }
            else ShowToast("You need more cash for this renovation.");
        }

        private void Upgrade(Property property)
        {
            if (game.UpgradeProperty(property.Id))
            {
                Render();
                ShowCelebration("PROPERTY UPGRADED", "+rent potential", "A stronger property creates a stronger company.");
            }
            else ShowToast("You need more cash for this upgrade.");
        }

        private void Promote(Property property)
        {
            if (game.PromoteProperty(property.Id)) { Render(); ShowToast("Local popularity increased. Rent potential improved."); }
            else ShowToast("You need 3 influence to promote this property.");
        }

        private VisualElement PropertyCard(Property property, bool showAction)
        {
            var tone = property.Stage == PropertyStage.Occupied ? "income" : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing ? "waiting" : "attention";
            var card = UiKit.Card(tone); card.AddToClassList("property-card");
            var top = UiKit.Row("card-top");
            var identity = new VisualElement(); identity.Add(UiKit.Text(property.Icon, 10, true, UiKit.Blue)); identity.Add(UiKit.Text(property.Name, 17, true)); identity.Add(UiKit.Text(property.District + " · " + property.Category, 11, false, UiKit.Muted)); top.Add(identity); top.Add(Badge("TIER " + property.Tier, "tier-badge")); card.Add(top);
            var status = UiKit.Text(Status(property), 12, true, StatusColor(property)); status.AddToClassList("status-chip"); status.AddToClassList("status-" + StatusTone(property)); card.Add(status);
            card.Add(UiKit.Text(PropertyDescription(property), 12, false, UiKit.Muted));
            var bottom = UiKit.Row("property-bottom"); bottom.Add(UiKit.Text("Monthly " + Money((property.TenantDailyRent > 0 ? property.TenantDailyRent : property.BaseDailyRent) * 30), 11, true)); bottom.Add(UiKit.Text("Condition " + property.Condition + "%", 11, true)); card.Add(bottom);
            if (showAction) card.Add(UiKit.Button(PropertyActionLabel(property), () => { selectedPropertyId = property.Id; screen = "Property"; Render(); }, property.Condition < 90 ? "danger" : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing ? "waiting" : property.Stage == PropertyStage.Applications || property.Stage == PropertyStage.ChoosingUse ? "primary" : "secondary"));
            return card;
        }

        private VisualElement Badge(string value, string className)
        {
            var badge = UiKit.Text(value, 9, true); badge.AddToClassList(className); return badge;
        }

        private VisualElement EmptyCard(string title, string description)
        {
            var card = UiKit.Card("neutral"); card.AddToClassList("empty-card"); card.Add(UiKit.Text(title, 18, true)); card.Add(UiKit.Text(description, 12, false, UiKit.Muted)); return card;
        }

        private int OccupiedRent() => game.State.Properties.Where(p => p.IsOwned && p.Stage == PropertyStage.Occupied).Sum(game.EffectiveDailyRent);
        private int ResidentialRent() => game.State.Properties.Where(p => p.IsOwned && p.Use == PropertyUse.Residential && p.Stage == PropertyStage.Occupied).Sum(game.EffectiveDailyRent);
        private int BusinessRent() => game.State.Properties.Where(p => p.IsOwned && p.Use == PropertyUse.Business && p.Stage == PropertyStage.Occupied).Sum(game.EffectiveDailyRent);
        private int TaskCount() => game.State.Properties.Count(p => p.IsOwned && p.Condition < 90);
        private int ImportantActionCount() => (game.State.RentReady > 0 ? 1 : 0) + game.State.Properties.Count(property => property.IsOwned && (property.Condition < 90 || property.Stage == PropertyStage.Applications || property.Stage == PropertyStage.ChoosingUse));
        private Property NextImportantProperty()
        {
            var owned = game.State.Properties.Where(property => property.IsOwned).ToList();
            if (game.State.RentReady > 0) return owned.FirstOrDefault(property => property.Stage == PropertyStage.Occupied);
            return owned.OrderBy(property => NextPriority(property)).FirstOrDefault();
        }
        private static int NextPriority(Property property)
        {
            if (property.Condition < 90) return 0;
            if (property.Stage == PropertyStage.Applications) return 1;
            if (property.Stage == PropertyStage.ChoosingUse) return 2;
            if (property.Stage == PropertyStage.Notary) return 3;
            if (property.Stage == PropertyStage.Listing || property.Stage == PropertyStage.CancellingContract) return 4;
            return 5;
        }
        private static string NextStory(Property property)
        {
            if (property.Condition < 90) return property.TenantName + " reported a repair issue.";
            if (property.Stage == PropertyStage.Applications) return property.Applicants.FirstOrDefault()?.Name + " wants to rent this property.";
            if (property.Stage == PropertyStage.ChoosingUse) return "Your keys have arrived. Choose how this property earns.";
            if (property.Stage == PropertyStage.Notary) return "Documents are being checked by the notary.";
            if (property.Stage == PropertyStage.Listing) return "Your advertisement is gathering visits.";
            if (property.Stage == PropertyStage.Occupied) return property.TenantName + " is settled here.";
            return "This property is waiting for your decision.";
        }
        private static string ToneFor(Property property) => property.Condition < 90 ? "attention" : property.Stage == PropertyStage.Occupied ? "income" : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing || property.Stage == PropertyStage.CancellingContract ? "waiting" : "briefing";
        private int CompanyValue() => game.State.Properties.Where(p => p.IsOwned).Sum(p => p.Price) + game.State.Cash;
        private string LifecycleSignature() => game.State.Day + ":" + game.State.RentReady + ":" + game.State.DailyRewardClaimed + ":" + string.Join(";", game.State.NpcActivities.Select(activity => activity.Title)) + "|" + string.Join("|", game.State.Properties.Where(property => property.IsOwned).Select(property => property.Id + ":" + property.Stage + ":" + property.Applicants.Count + ":" + property.Condition + ":" + property.Level + ":" + property.Popularity));
        private static string Money(int value) => value.ToString("N0") + " PLN";
        private static int StatusRank(Property property) => property.Stage == PropertyStage.Applications ? 0 : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.CancellingContract ? 1 : property.Condition < 90 ? 2 : 3;
        private static string PropertyDescription(Property property) => property.Stage == PropertyStage.Occupied ? "Occupied by " + property.TenantName + " · " + Money(property.TenantDailyRent) + " / day" : property.Stage == PropertyStage.Applications ? property.Applicants.Count + " applicants waiting for a decision" : property.Stage == PropertyStage.Notary ? "Notary transfer in progress" : property.Stage == PropertyStage.CancellingContract ? "Tenant is preparing to leave" : property.Stage == PropertyStage.ForSale ? "Sale offer is waiting" : "Choose a use and advertise to start earning";
        private static string PropertyActionLabel(Property property) => property.Condition < 90 ? "! Repair pipe" : property.Stage == PropertyStage.Notary ? "> Sign documents" : property.Stage == PropertyStage.CancellingContract ? "> Check move-out" : property.Stage == PropertyStage.ForSale ? "$ Review sale offer" : property.Stage == PropertyStage.ChoosingUse ? "+ Choose property use" : property.Stage == PropertyStage.Listing ? "> Check listing progress" : property.Stage == PropertyStage.Applications ? "+ Choose tenant" : property.Stage == PropertyStage.Occupied ? "> View tenant" : "+ Publish listing";
        private static StyleColor StatusColor(Property property) => property.Stage == PropertyStage.Occupied ? UiKit.Green : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing ? UiKit.Amber : UiKit.Blue;
        private static string StatusTone(Property property) => property.Condition < 90 ? "problem" : property.Stage == PropertyStage.Occupied ? "income" : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing || property.Stage == PropertyStage.CancellingContract || property.Stage == PropertyStage.ForSale ? "waiting" : property.Stage == PropertyStage.Applications || property.Stage == PropertyStage.ChoosingUse ? "decision" : "managed";
        private string MapPinClass(Property property) => property.Condition < 90 && property.IsOwned ? "map-pin-problem" : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing || property.Stage == PropertyStage.CancellingContract || property.Stage == PropertyStage.ForSale ? "map-pin-waiting" : property.IsOwned ? "map-pin-owned" : property.Price > game.State.Cash ? "map-pin-locked" : property.Tier >= 3 ? "map-pin-dream" : "map-pin-available";
        private string Status(Property property) => property.Stage == PropertyStage.Notary ? "Waiting for notary" : property.Stage == PropertyStage.ChoosingUse ? "Not earning · choose a use" : property.Stage == PropertyStage.Listing ? "Listing live · applications expected soon" : property.Stage == PropertyStage.CancellingContract ? "Contract ending · tenant moving out" : property.Stage == PropertyStage.ForSale ? "On the market · offer waiting" : property.Stage == PropertyStage.Applications ? property.Applicants.Count + " applications waiting" : property.Stage == PropertyStage.Occupied ? "Occupied by " + property.TenantName : property.IsOwned ? "Ready to advertise" : "Available to invest";
    }
}
