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
        private VisualElement content;
        private string screen = "Portfolio";
        private float nextClockRefresh;

        private void Awake()
        {
            game = new GameService(new JsonLocalGameRepository(), new GameClock());
            root = GetComponent<UIDocument>().rootVisualElement;
            var style = Resources.Load<StyleSheet>("DistrictEmpireStyle");
            if (style != null) root.styleSheets.Add(style);
            root.AddToClassList("app-shell");
            root.style.flexGrow = 1;
            root.style.backgroundColor = new Color(0.94f, 0.96f, 0.98f);
            Render();
        }

        private void Update()
        {
            if (Time.unscaledTime < nextClockRefresh) return;
            nextClockRefresh = Time.unscaledTime + 1f;
            game.Tick();
            if (screen == "Portfolio") Render();
        }

        private void Render()
        {
            root.Clear();
            root.Add(BuildHeader());
            content = new ScrollView { style = { flexGrow = 1 } };
            content.AddToClassList("screen-scroll");
            root.Add(content);
            if (screen == "Map") RenderMap();
            else if (screen == "Invest") RenderInvest();
            else if (screen == "Tasks") RenderTasks();
            else if (screen == "Property") RenderProperty();
            else RenderPortfolio();
            root.Add(BuildNav());
        }

        private VisualElement BuildHeader()
        {
            var header = new VisualElement();
            header.AddToClassList("top-header");
            var row = new VisualElement(); row.AddToClassList("header-row");
            var title = new VisualElement();
            title.Add(UiKit.Text("WARSAW · OFFLINE VERTICAL SLICE", 11, true, UiKit.Muted));
            title.Add(UiKit.Text(screen == "Portfolio" ? "Morning briefing" : screen, 24, true));
            row.Add(title);
            var controls = new VisualElement { style = { flexDirection = FlexDirection.Row, alignItems = Align.Center } };
            var level = new VisualElement(); level.AddToClassList("level-pill"); level.Add(UiKit.Text("LEVEL", 9, true, UiKit.Muted)); level.Add(UiKit.Text("1", 18, true, UiKit.Amber));
            controls.Add(level); controls.Add(UiKit.Button("☰", ShowMenu, "secondary"));
            row.Add(controls); header.Add(row);
            var wallet = new VisualElement(); wallet.AddToClassList("wallet-strip");
            wallet.Add(WalletChip("Cash", $"{game.State.Cash:N0} PLN"));
            wallet.Add(WalletChip("Rent/day", $"{game.State.Properties.Where(p => p.Stage == PropertyStage.Occupied).Sum(p => p.TenantDailyRent):N0} PLN"));
            wallet.Add(WalletChip("Buildings", game.State.Properties.Count(p => p.IsOwned).ToString()));
            wallet.Add(WalletChip("Influence", game.State.Influence.ToString()));
            header.Add(wallet);
            return header;
        }

        private VisualElement WalletChip(string label, string value)
        {
            var chip = new VisualElement(); chip.AddToClassList("wallet-chip"); chip.Add(UiKit.Text(label, 10, false, UiKit.Muted)); chip.Add(UiKit.Text(value, 12, true, UiKit.Green)); return chip;
        }

        private VisualElement BuildNav()
        {
            var nav = new VisualElement(); nav.AddToClassList("bottom-nav");
            foreach (var tab in new[] { "Portfolio", "Map", "Invest", "Tasks" })
            {
                var button = new Button(() => { screen = tab; Render(); }) { text = tab };
                button.AddToClassList("nav-tab"); if (screen == tab) button.AddToClassList("nav-tab-active");
                nav.Add(button);
            }
            return nav;
        }

        private void RenderPortfolio()
        {
            var state = game.State;
            var income = UiKit.Card(state.RentReady > 0 ? "income" : "neutral");
            income.Add(UiKit.Text(state.RentReady > 0 ? "RENT READY" : "NEXT RENT", 11, true, state.RentReady > 0 ? UiKit.Green : UiKit.Muted));
            income.Add(UiKit.Text(state.RentReady > 0 ? $"{state.RentReady:N0} PLN" : "Tomorrow", 25, true));
            income.Add(UiKit.Text(state.RentReady > 0 ? "From occupied properties" : "Payments arrive from occupied properties."));
            income.Add(UiKit.Button(state.RentReady > 0 ? $"Collect {state.RentReady:N0} PLN" : "View rent schedule", () => { game.CollectRent(); Render(); }, state.RentReady > 0 ? "income" : "secondary"));
            content.Add(income);
            foreach (var property in state.Properties.Where(p => p.IsOwned)) content.Add(PropertyCard(property));
        }

        private void RenderMap()
        {
            content.Add(UiKit.Text("What can I buy?", 20, true));
            content.Add(UiKit.Text("Available opportunities across Warsaw. Buy early to begin the ownership transfer.", 13, false, UiKit.Muted));
            foreach (var property in game.State.Properties.Where(p => !p.IsOwned))
            {
                var card = UiKit.Card(); card.Add(UiKit.Text($"{property.Icon}  {property.Name}", 18, true)); card.Add(UiKit.Text($"{property.District} · {property.Price:N0} PLN · Est. {property.BaseDailyRent:N0} PLN/day"));
                card.Add(UiKit.Button($"Buy for {property.Price:N0} PLN", () => { game.Buy(property.Id); screen = "Portfolio"; Render(); })); content.Add(card);
            }
        }

        private void RenderInvest() => RenderMap();

        private void RenderTasks()
        {
            content.Add(UiKit.Text("Tasks", 20, true));
            foreach (var property in game.State.Properties.Where(p => p.IsOwned && p.Condition < 90))
            {
                var card = UiKit.Card("attention"); card.Add(UiKit.Text($"Maintenance needed · {property.Name}", 17, true)); card.Add(UiKit.Text($"Condition {property.Condition}% · Repair cost 450 PLN"));
                card.Add(UiKit.Button("Repair property for 450 PLN", () => { game.Repair(property.Id); Render(); })); content.Add(card);
            }
        }

        private VisualElement PropertyCard(Property property)
        {
            var tone = property.Stage == PropertyStage.Occupied ? "income" : property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing ? "waiting" : "attention";
            var card = UiKit.Card(tone); card.Add(UiKit.Text($"{property.Icon}  {property.Name}", 18, true));
            var status = UiKit.Text(Status(property), 14, true); status.AddToClassList("property-status"); card.Add(status);
            card.Add(UiKit.Text(property.Stage == PropertyStage.Occupied ? $"{property.TenantDailyRent:N0} PLN/day · {property.TenantName}" : "0 PLN income until a use is chosen"));
            card.Add(UiKit.Button("View property", () => { selectedPropertyId = property.Id; screen = "Property"; Render(); }, "secondary"));
            return card;
        }

        private string selectedPropertyId;
        private void RenderProperty()
        {
            var property = game.State.Properties.First(p => p.Id == selectedPropertyId);
            content.Add(UiKit.Text(property.Name, 22, true));
            content.Add(UiKit.Text(Status(property), 15, true));
            if (property.Stage == PropertyStage.Notary || property.Stage == PropertyStage.Listing) content.Add(UiKit.Text(game.Countdown(property), 14, true, UiKit.Amber));
            if (property.Stage == PropertyStage.ChoosingUse)
            {
                var choice = UiKit.Card("attention"); choice.Add(UiKit.Text("How should this property earn money?", 17, true)); choice.Add(UiKit.Text("Choose a home rental or a business tenant. This choice controls the applicant pool."));
                choice.Add(UiKit.Button("Rent as a home", () => { game.ChooseUse(property.Id, PropertyUse.Residential); Render(); }));
                choice.Add(UiKit.Button("Use as a business", () => { game.ChooseUse(property.Id, PropertyUse.Business); Render(); }, "secondary")); content.Add(choice);
            }
            else if (property.Stage == PropertyStage.Available) content.Add(UiKit.Button("Create rental listing", () => { game.PublishListing(property.Id); Render(); }));
            else if (property.Stage == PropertyStage.Applications)
            {
                content.Add(UiKit.Text("Applicants", 18, true));
                foreach (var applicant in property.Applicants)
                {
                    var card = UiKit.Card(); card.Add(UiKit.Text(applicant.Name, 16, true)); card.Add(UiKit.Text($"{applicant.Role} · {applicant.DailyRent:N0} PLN/day")); card.Add(UiKit.Text(applicant.Story)); card.Add(UiKit.Button($"Choose {applicant.Name}", () => { game.SelectApplicant(property.Id, applicant.Id); Render(); })); content.Add(card);
                }
            }
            else content.Add(PropertyCard(property));
            content.Add(UiKit.Button("Back to Portfolio", () => { screen = "Portfolio"; Render(); }, "secondary"));
        }

        private string Status(Property property) => property.Stage switch
        {
            PropertyStage.Notary => "Waiting for notary",
            PropertyStage.ChoosingUse => "Not earning · choose a use",
            PropertyStage.Listing => "Listing live · applications expected soon",
            PropertyStage.Applications => $"{property.Applicants.Count} applications waiting",
            PropertyStage.Occupied => $"Occupied by {property.TenantName}",
            _ => "Ready to advertise"
        };

        private void ShowMenu()
        {
            var menu = UiKit.Card(); menu.AddToClassList("menu-panel");
            menu.Add(UiKit.Text("Paweł W. · Company", 17, true));
            foreach (var item in new[] { "Company", "Finances", "Employees", "Skills", "Auctions", "Rankings", "Events & News", "Shop", "Friends", "Settings & Help" })
                menu.Add(UiKit.Button(item, () => { root.Remove(menu); if (item == "Auctions") { screen = "Invest"; Render(); } }, "secondary"));
            root.Add(menu);
        }
    }
}
