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
            content = new ScrollView { style = { flexGrow = 1, paddingLeft = 14, paddingRight = 14, paddingTop = 10 } };
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
            var header = new VisualElement { style = { flexDirection = FlexDirection.Row, justifyContent = Justify.SpaceBetween, alignItems = Align.Center, paddingLeft = 18, paddingRight = 18, paddingTop = 16, paddingBottom = 10, backgroundColor = Color.white } };
            var title = new VisualElement();
            title.Add(UiKit.Text("DISTRICT EMPIRE", 11, true, UiKit.Muted));
            title.Add(UiKit.Text(screen == "Portfolio" ? "Morning briefing" : screen, 24, true));
            header.Add(title);
            header.Add(UiKit.Button("☰", ShowMenu, "secondary"));
            return header;
        }

        private VisualElement BuildNav()
        {
            var nav = new VisualElement { style = { flexDirection = FlexDirection.Row, backgroundColor = Color.white, paddingLeft = 8, paddingRight = 8, paddingTop = 8, paddingBottom = 12 } };
            foreach (var tab in new[] { "Portfolio", "Map", "Invest", "Tasks" })
            {
                var button = new Button(() => { screen = tab; Render(); }) { text = tab };
                button.style.flexGrow = 1; button.style.minHeight = 54; button.style.marginLeft = button.style.marginRight = 3;
                button.style.backgroundColor = screen == tab ? new Color(0.92f, 0.95f, 1f) : Color.white;
                button.style.color = screen == tab ? UiKit.Blue : UiKit.Muted;
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
            content.Add(UiKit.Text("What can I buy?", 16, true));
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
            card.Add(UiKit.Text(Status(property), 14, true));
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
            var menu = UiKit.Card(); menu.style.position = Position.Absolute; menu.style.top = 60; menu.style.right = 12; menu.style.width = 280; menu.style.zIndex = 10;
            menu.Add(UiKit.Text("Paweł W. · Company", 17, true));
            foreach (var item in new[] { "Company", "Finances", "Employees", "Skills", "Auctions", "Rankings", "Events & News", "Shop", "Friends", "Settings & Help" })
                menu.Add(UiKit.Button(item, () => { root.Remove(menu); if (item == "Auctions") { screen = "Invest"; Render(); } }, "secondary"));
            root.Add(menu);
        }
    }
}
