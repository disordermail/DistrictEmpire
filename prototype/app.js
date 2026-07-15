const properties = [
  {
    id: "mokotow-starter",
    name: "Mokotow Starter",
    district: "Mokotow",
    tier: 1,
    price: 18000,
    rent: 620,
    x: 18,
    y: 58,
    icon: "🏘",
    category: "Apartments",
    useType: "residential",
    description: "Small apartment block with stable tenant demand."
  },
  {
    id: "wola-corner",
    name: "Wola Corner",
    district: "Wola",
    tier: 2,
    price: 36000,
    rent: 1180,
    x: 38,
    y: 36,
    icon: "🏢",
    category: "Mixed use",
    useType: "commercial",
    description: "Mixed-use building near an office corridor."
  },
  {
    id: "praga-yard",
    name: "Praga Yard",
    district: "Praga",
    tier: 1,
    price: 22000,
    rent: 760,
    x: 68,
    y: 45,
    icon: "🏚",
    category: "Renovation",
    useType: "residential",
    description: "Older property with cheap upgrades and good upside."
  },
  {
    id: "srodmiescie-house",
    name: "Srodmiescie House",
    district: "Srodmiescie",
    tier: 3,
    price: 74000,
    rent: 2360,
    x: 49,
    y: 61,
    icon: "🏛",
    category: "Premium",
    useType: "commercial",
    description: "Central location with premium rent potential."
  },
  {
    id: "zoliborz-arcade",
    name: "Zoliborz Arcade",
    district: "Zoliborz",
    tier: 2,
    price: 45500,
    rent: 1460,
    x: 28,
    y: 19,
    icon: "🛍",
    category: "Retail",
    useType: "commercial",
    description: "Compact retail frontage with reliable foot traffic."
  }
];

const rivals = [
  { name: "Northbank Group", score: 132000 },
  { name: "Vistula Assets", score: 94000 },
  { name: "Old Town Capital", score: 51000 }
];

const commercialTenants = [
  { name: "Green Cross Pharmacy", category: "health", label: "Pharmacy", multiplier: 1.34, risk: "low" },
  { name: "Corner Grocery", category: "trade", label: "Grocery shop", multiplier: 1.12, risk: "low" },
  { name: "Pierogi Bar", category: "gastronomy", label: "Food service", multiplier: 1.28, risk: "medium" },
  { name: "Nova Beauty Studio", category: "beauty", label: "Beauty salon", multiplier: 1.18, risk: "medium" },
  { name: "EduKids Language School", category: "education", label: "Education", multiplier: 1.24, risk: "low" },
  { name: "Blue Desk Offices", category: "offices", label: "Office", multiplier: 1.16, risk: "low" },
  { name: "FitPoint Studio", category: "sport", label: "Sport", multiplier: 1.22, risk: "medium" },
  { name: "Arcade Room", category: "entertainment", label: "Entertainment", multiplier: 1.38, risk: "high" },
  { name: "FixIt Repairs", category: "repairs", label: "Repairs", multiplier: 1.15, risk: "medium" },
  { name: "Parcel Hub", category: "logistics", label: "Logistics", multiplier: 1.2, risk: "medium" },
  { name: "City Stay Desk", category: "tourism", label: "Tourism", multiplier: 1.3, risk: "high" },
  { name: "Local Services Point", category: "public services", label: "Public services", multiplier: 1.08, risk: "low" }
];

const residentialTenants = [
  { name: "Marta and Piotr", category: "family", label: "Family", multiplier: 1.02, risk: "low" },
  { name: "Kamil Nowak", category: "student", label: "Student", multiplier: 0.9, risk: "medium" },
  { name: "Alicja W.", category: "professional", label: "Professional", multiplier: 1.14, risk: "low" },
  { name: "Remote Duo", category: "remote-workers", label: "Remote workers", multiplier: 1.2, risk: "medium" },
  { name: "Problematic renter", category: "problematic", label: "High-risk tenant", multiplier: 1.42, risk: "high" }
];

const firstHourSteps = [
  { id: "welcome", time: "0-3 min", title: "Enter Warsaw", win: "Starter capital received" },
  { id: "first-purchase", time: "3-8 min", title: "Buy first apartment", win: "First property purchased" },
  { id: "first-renovation", time: "8-15 min", title: "Renovate once", win: "Value increased" },
  { id: "first-tenant", time: "15-20 min", title: "Choose tenant", win: "First lease signed" },
  { id: "first-rent", time: "20-25 min", title: "Collect rent", win: "Wallet updated" },
  { id: "first-issue", time: "25-35 min", title: "Resolve issue", win: "Neighborhood improved" },
  { id: "rankings", time: "35-45 min", title: "Check company rank", win: "Rivals discovered" },
  { id: "dream-preview", time: "45-60 min", title: "Preview next tier", win: "Luxury goal unlocked" }
];

const dreamProperty = {
  name: "Royal Riverside Tower",
  price: 320000,
  rent: 9200,
  district: "Srodmiescie",
  icon: "🏙"
};

const defaultState = {
  cash: 50000,
  influence: 35,
  xp: 0,
  playerLevel: 1,
  owned: [],
  conditions: {},
  propertyLevels: {},
  tenants: {},
  propertyStages: {},
  advertisements: {},
  tenantMemories: {},
  latePayments: {},
  evictionCases: {},
  listedForSale: {},
  rentAccrued: {
    residential: 0,
    commercial: 0
  },
  rentAccruedMigrated: true,
  claimedGoals: [],
  microWins: [],
  onboardingComplete: false,
  tutorialState: {
    currentStep: "welcome",
    completedSteps: [],
    skipped: false
  },
  dailyRewardClaimedDay: 0,
  dailyMomentum: [],
  cityEvents: [],
  streetIssuesResolved: [],
  repaired: [],
  selectedId: "mokotow-starter",
  infoPropertyId: "mokotow-starter",
  mapSheetOpen: false,
  sheetTab: "overview",
  day: 1,
  toast: "",
  ceremony: "",
  pendingLevel: null
};

let state = createDefaultState();
state = loadState();

const screens = document.querySelectorAll(".screen");
const tabs = document.querySelectorAll(".tab");
const mapStage = document.querySelector("#mapStage");
const buildingSheet = document.querySelector("#buildingSheet");
const sheetCloseButton = document.querySelector("#sheetCloseButton");
const sheetDistrict = document.querySelector("#sheetDistrict");
const sheetTitle = document.querySelector("#sheetTitle");
const sheetDescription = document.querySelector("#sheetDescription");
const sheetEmblem = document.querySelector("#sheetEmblem");
const sheetTier = document.querySelector("#sheetTier");
const sheetLevel = document.querySelector("#sheetLevel");
const sheetRent = document.querySelector("#sheetRent");
const sheetPrice = document.querySelector("#sheetPrice");
const sheetAction = document.querySelector("#sheetAction");
const sheetInsights = document.querySelector("#sheetInsights");
const sheetPanel = document.querySelector("#sheetPanel");
const sheetTabs = document.querySelectorAll("[data-sheet-tab]");
const propertyInfoDistrict = document.querySelector("#propertyInfoDistrict");
const propertyInfoTitle = document.querySelector("#propertyInfoTitle");
const propertyInfoView = document.querySelector("#propertyInfoView");
const missionStrip = document.querySelector("#missionStrip");
const firstHourPanel = document.querySelector("#firstHourPanel");
const activityFeed = document.querySelector("#activityFeed");
const portfolioFocus = document.querySelector("#portfolioFocus");
const toast = document.querySelector("#toast");
const ceremony = document.querySelector("#ceremony");
const levelModal = document.querySelector("#levelModal");
const levelModalValue = document.querySelector("#levelModalValue");
const levelModalText = document.querySelector("#levelModalText");
const welcomeModal = document.querySelector("#welcomeModal");
const tutorialChip = document.querySelector("#tutorialChip");
const portfolioBadge = document.querySelector("#portfolioBadge");
const issuesBadge = document.querySelector("#issuesBadge");

document.querySelector("#resetButton").addEventListener("click", () => {
  state = createDefaultState();
  saveState();
  render();
  showScreen("map");
});

document.querySelector("#collectRentButton").addEventListener("click", collectRent);
sheetCloseButton.addEventListener("click", () => {
  state.mapSheetOpen = false;
  saveState();
  render();
});
document.querySelector("#propertyBackButton").addEventListener("click", () => showScreen("portfolio"));
document.querySelector("#dailyRewardButton").addEventListener("click", claimDailyReward);
document.querySelector("#grantCashButton").addEventListener("click", () => {
  state.cash += 10000;
  pushToast("Test cash added.");
  saveState();
  render();
});
document.querySelector("#grantInfluenceButton").addEventListener("click", () => {
  state.influence += 25;
  pushToast("Influence added.");
  saveState();
  render();
});
document.querySelector("#simulateDayButton").addEventListener("click", () => {
  simulateDay();
});
document.querySelector("#claimLevelButton").addEventListener("click", () => {
  state.pendingLevel = null;
  saveState();
  render();
});
document.querySelector("#startOnboardingButton").addEventListener("click", () => {
  state.onboardingComplete = true;
  advanceTutorial("inspect");
  trackMicroWin("welcome");
  saveState();
  render();
});

tabs.forEach((tab) => {
  tab.addEventListener("click", () => {
    if (tab.dataset.screen === "map") {
      state.mapSheetOpen = false;
      saveState();
      render();
    }
    showScreen(tab.dataset.screen);
  });
});

sheetTabs.forEach((tab) => {
  tab.addEventListener("click", () => {
    state.sheetTab = tab.dataset.sheetTab;
    saveState();
    render();
  });
});

sheetAction.addEventListener("click", () => {
  const property = getSelectedProperty();
  if (!property) {
    return;
  }

  if (state.owned.includes(property.id)) {
    openPropertyInfo(property.id);
  } else {
    buyProperty(property.id);
  }
});

render();

function render() {
  normalizeState();
  renderStatus();
  renderToast();
  renderCeremony();
  renderLevelModal();
  renderMissions();
  renderFirstHourPanel();
  renderWelcomeModal();
  renderTutorialDirector();
  renderNavigationGates();
  renderActivityFeed();
  renderMap();
  renderSheet();
  renderMarket();
  renderPortfolio();
  renderPropertyInfo();
  renderIssues();
  renderRanking();
}

function renderStatus() {
  const ownedProperties = properties.filter((property) => state.owned.includes(property.id));
  const dailyRent = ownedProperties.reduce((total, property) => total + getActiveRent(property), 0);
  const homeRent = getAccruedRentByUseType("residential");
  const shopRent = getAccruedRentByUseType("commercial");
  const collectableRent = homeRent + shopRent;
  const issueCount = getIssueCount();
  const collectRentButton = document.querySelector("#collectRentButton");

  document.querySelector("#cashValue").textContent = formatMoney(state.cash);
  document.querySelector("#rentValue").textContent = formatMoney(dailyRent);
  document.querySelector("#ownedValue").textContent = String(ownedProperties.length);
  document.querySelector("#influenceValue").textContent = String(state.influence);
  document.querySelector("#playerLevelValue").textContent = String(state.playerLevel);
  document.querySelector("#homeRentValue").textContent = formatMoney(homeRent);
  document.querySelector("#shopRentValue").textContent = formatMoney(shopRent);
  collectRentButton.textContent = collectableRent > 0 ? `Collect ${formatMoney(collectableRent)}` : "No rent waiting";
  collectRentButton.disabled = collectableRent <= 0;
  collectRentButton.classList.toggle("rent-ready", collectableRent > 0);

  portfolioBadge.hidden = collectableRent <= 0;
  portfolioBadge.textContent = "!";
  issuesBadge.hidden = issueCount <= 0;
  issuesBadge.textContent = String(issueCount);
}

function renderToast() {
  toast.hidden = !state.toast;
  toast.textContent = state.toast || "";
}

function renderCeremony() {
  ceremony.hidden = !state.ceremony;
  ceremony.textContent = state.ceremony || "";
}

function renderLevelModal() {
  levelModal.hidden = !state.pendingLevel;
  levelModalValue.textContent = String(state.pendingLevel || state.playerLevel);
  levelModalText.textContent = `Investor level ${state.pendingLevel || state.playerLevel} unlocked. Bonus: ${formatMoney(getLevelReward(state.pendingLevel || state.playerLevel))} and 10 influence.`;
}

function renderMissions() {
  const goals = getGoals();
  missionStrip.innerHTML = "";

  goals.forEach((goal) => {
    const card = document.createElement("article");
    card.className = `mission-card${goal.complete ? " complete" : ""}`;
    card.innerHTML = `
      <div>
        <span>${goal.complete ? "Ready" : "Goal"}</span>
        <strong>${goal.title}</strong>
      </div>
      <button type="button"${goal.complete ? "" : " disabled"}>${goal.claimed ? "Claimed" : "Claim"}</button>
    `;
    card.querySelector("button").addEventListener("click", () => claimGoal(goal.id));
    missionStrip.appendChild(card);
  });
}

function renderFirstHourPanel() {
  firstHourPanel.hidden = !isTutorialComplete();
  if (!isTutorialComplete()) {
    return;
  }
  if ((state.playerLevel >= 2 || state.owned.length >= 3) && !state.microWins.includes("dream-preview")) {
    trackMicroWin("dream-preview", false);
  }

  const completed = firstHourSteps.filter((step) => state.microWins.includes(step.id));
  const nextStep = firstHourSteps.find((step) => !state.microWins.includes(step.id));
  const progress = Math.round((completed.length / firstHourSteps.length) * 100);

  firstHourPanel.innerHTML = `
    <div class="first-hour-header">
      <div>
        <span>First 60 minutes</span>
        <strong>${nextStep ? nextStep.title : "Tomorrow's session unlocked"}</strong>
      </div>
      <em>${progress}%</em>
    </div>
    <div class="first-hour-track"><span style="--first-hour-progress: ${progress}%"></span></div>
    <div class="micro-win-row">
      ${firstHourSteps.slice(0, 6).map((step) => `
        <span class="${state.microWins.includes(step.id) ? "done" : ""}" title="${step.win}">${state.microWins.includes(step.id) ? "✓" : "•"} ${step.time}</span>
      `).join("")}
    </div>
  `;
}

function renderWelcomeModal() {
  welcomeModal.hidden = state.onboardingComplete || state.tutorialState.skipped;
}

function renderTutorialDirector() {
  const step = state.tutorialState.currentStep;
  const guide = getTutorialGuide(step);
  tutorialChip.hidden = !state.onboardingComplete || isTutorialComplete() || !guide || state.mapSheetOpen;

  if (tutorialChip.hidden) {
    return;
  }

  tutorialChip.innerHTML = `<span>Next</span><strong>${guide.title}</strong><em>${guide.detail}</em><button type="button">${guide.cta}</button>`;
  tutorialChip.querySelector("button").addEventListener("click", () => handleTutorialAction(step));
}

function getTutorialGuide(step) {
  return {
    inspect: { title: "Inspect your first opportunity", detail: "Tap the pulsing starter property on the map.", cta: "Show me" },
    buy: { title: "Buy the starter apartment", detail: "Review the price and begin notary paperwork.", cta: "Open property" },
    notary: { title: "Paperwork is in progress", detail: "In this prototype, advance the day to receive the notary handover.", cta: "Advance day" },
    handover: { title: "Keys are ready", detail: "Choose whether to renovate or advertise first.", cta: "View property" },
    listing: { title: "Find your first tenant", detail: "Publish a listing to start attracting people.", cta: "Create listing" },
    applications: { title: "Watch interest build", detail: "Advance the day to receive views, viewings and applications.", cta: "Advance day" },
    tenant: { title: "Choose a person", detail: "Compare stories and offers before you sign.", cta: "Review applicants" },
    lease: { title: "Agreement prepared", detail: "Sign the first lease to arrange key handover.", cta: "Review lease" },
    rent: { title: "First rent is coming", detail: "Advance the day, then collect the transfer in Portfolio.", cta: "Advance day" },
    tasks: { title: "Care for your property", detail: "Resolve your first task to protect the new lease.", cta: "Open Tasks" },
    ranking: { title: "Meet the competition", detail: "See where your company stands in Warsaw.", cta: "View Company" },
    dream: { title: "Set tomorrow's goal", detail: "Preview the next-tier property waiting for you.", cta: "Browse Invest" }
  }[step] || null;
}

function handleTutorialAction(step) {
  const starterId = "mokotow-starter";
  if (step === "inspect") {
    state.selectedId = starterId;
    state.mapSheetOpen = true;
    showScreen("map");
  } else if (step === "buy") {
    state.selectedId = starterId;
    state.mapSheetOpen = true;
    showScreen("map");
  } else if (["notary", "applications", "rent"].includes(step)) {
    simulateDay();
    return;
  } else if (step === "dream") {
    showScreen("market");
    advanceTutorial("complete");
  } else if (["handover", "listing", "tenant", "lease"].includes(step)) {
    openPropertyInfo(starterId);
  } else if (["applications", "rent"].includes(step)) {
    showScreen("portfolio");
  } else if (step === "tasks") {
    showScreen("issues");
  } else if (step === "ranking") {
    showScreen("ranking");
    advanceTutorial("dream");
  }
  saveState();
  render();
}

function advanceTutorial(nextStep) {
  if (state.tutorialState.skipped || isTutorialComplete()) {
    return;
  }
  const current = state.tutorialState.currentStep;
  if (current && !state.tutorialState.completedSteps.includes(current)) {
    state.tutorialState.completedSteps.push(current);
  }
  state.tutorialState.currentStep = nextStep;
}

function isTutorialComplete() {
  return state.tutorialState.skipped || state.tutorialState.currentStep === "complete";
}

function renderNavigationGates() {
  const tutorialActive = state.onboardingComplete && !isTutorialComplete();
  const marketTab = document.querySelector("#tab-market");
  const tasksTab = document.querySelector("#tab-issues");
  const companyTab = document.querySelector("#tab-ranking");
  const hasLease = Object.keys(state.tenants).length > 0;

  marketTab.disabled = tutorialActive && !["notary", "handover", "listing", "applications", "tenant", "lease", "rent", "tasks", "ranking", "dream"].includes(state.tutorialState.currentStep);
  tasksTab.disabled = tutorialActive && !["tasks", "ranking", "dream"].includes(state.tutorialState.currentStep);
  companyTab.disabled = tutorialActive && !hasLease;
}

function renderActivityFeed() {
  const selected = getSelectedProperty();
  const district = selected?.district || "Warsaw";
  const tenant = selected ? getTenant(selected.id) : null;
  const watching = selected ? 2 + selected.tier + getPropertyLevel(selected.id) : 3;
  const cityMoment = state.cityEvents[0] || `${district}: two investors are watching ${selected ? selected.name : "new listings"}`;
  const feed = [cityMoment, tenant ? `${tenant.name} is settled in ${selected.name}` : `${watching} investors are watching ${selected ? selected.name : "Warsaw listings"}`];

  activityFeed.innerHTML = feed.map((item) => `<span>${item}</span>`).join("");
}

function renderMap() {
  mapStage.innerHTML = "";

  properties.forEach((property) => {
    const button = document.createElement("button");
    button.type = "button";
    const listed = Boolean(state.listedForSale[property.id]);
    button.className = [
      "property-pin",
      listed ? "listed" : state.owned.includes(property.id) ? "owned" : "available",
      state.selectedId === property.id ? "selected" : ""
    ].join(" ");
    button.style.left = `${property.x}%`;
    button.style.top = `${property.y}%`;
    button.innerHTML = `<span>${property.icon}</span><small>${getPropertyLevel(property.id)}</small>`;
    button.setAttribute("aria-label", property.name);
    button.addEventListener("click", () => {
      state.selectedId = property.id;
      state.sheetTab = "overview";
      state.mapSheetOpen = true;
      if (state.tutorialState.currentStep === "inspect" && property.id === "mokotow-starter") {
        advanceTutorial("buy");
      }
      saveState();
      render();
    });
    mapStage.appendChild(button);
  });
}

function renderSheet() {
  const property = getSelectedProperty();

  if (!property || !state.mapSheetOpen) {
    buildingSheet.hidden = true;
    return;
  }

  buildingSheet.hidden = false;
  const owned = state.owned.includes(property.id);
  const listed = Boolean(state.listedForSale[property.id]);
  sheetDistrict.textContent = `${property.district} · ${listed ? "Listed by you" : owned ? "Owned" : "For sale"}`;
  sheetEmblem.textContent = property.icon;
  sheetTitle.textContent = property.name;
  sheetDescription.textContent = property.description;
  sheetTier.textContent = `Tier ${property.tier}`;
  sheetLevel.textContent = `Lvl ${getPropertyLevel(property.id)}`;
  sheetRent.textContent = `${formatMoney(owned ? getActiveRent(property) : getPotentialRent(property))} / day`;
  sheetPrice.textContent = listed ? `${formatMoney(state.listedForSale[property.id])} ask` : formatMoney(property.price);
  sheetInsights.innerHTML = `
    <article><strong>${formatMoney((owned ? getActiveRent(property) : getPotentialRent(property)) * 30)}</strong><span>monthly rent</span></article>
    <article><strong>${getRoi(property)}%</strong><span>ROI</span></article>
    <article><strong>${getNeighborhoodScore(property)}</strong><span>neighborhood</span></article>
  `;
  renderSheetTabs(property);
  renderSheetPanel(property);
  sheetAction.textContent = owned ? "Info" : `Buy for ${formatMoney(property.price)}`;
  sheetAction.disabled = !owned && state.cash < property.price;
}

function renderSheetTabs(property) {
  const owned = state.owned.includes(property.id);

  sheetTabs.forEach((tab) => {
    const tabName = tab.dataset.sheetTab;
    tab.classList.toggle("active", state.sheetTab === tabName);
    tab.disabled = !owned && tabName !== "overview";
  });
}

function renderSheetPanel(property) {
  const owned = state.owned.includes(property.id);
  const tenant = getTenant(property.id);
  const evictionCase = getEvictionCase(property.id);

  if (!owned || state.sheetTab === "overview") {
    sheetPanel.innerHTML = `
      <div class="mini-metrics">
        <span>${getRentalStatus(property)}</span>
        <span>${property.category}</span>
        <span>${property.useType === "commercial" ? "Work property" : "Home property"}</span>
      </div>
    `;
    return;
  }

  if (state.sheetTab === "rent") {
    if (evictionCase) {
      sheetPanel.innerHTML = `<p>Contract is cancelling. Ready in ${evictionCase.daysRemaining} day${evictionCase.daysRemaining === 1 ? "" : "s"}.</p>`;
      return;
    }

    if (tenant) {
      sheetPanel.innerHTML = `
        <p>${tenant.name} · ${tenant.label} · ${formatMoney(tenant.rent)} / day</p>
        <button class="secondary-action sheet-small-action" id="sheetCancelRentButton" type="button">Cancel contract</button>
      `;
      document.querySelector("#sheetCancelRentButton")?.addEventListener("click", () => startContractCancellation(property.id));
      return;
    }

    const bestOffer = getTenantOffers(property)[0];
    sheetPanel.innerHTML = `
      <p>Best offer: ${bestOffer.name}, ${formatMoney(bestOffer.rent)} / day.</p>
      <button class="primary-action sheet-small-action" id="sheetAcceptTenantButton" type="button">Accept tenant</button>
    `;
    document.querySelector("#sheetAcceptTenantButton")?.addEventListener("click", () => acceptTenantOffer(property.id, bestOffer.id));
    return;
  }

  if (state.sheetTab === "repairs") {
    const maintenanceCost = getMaintenanceCost(property);
    sheetPanel.innerHTML = `
      <p>Condition ${getCondition(property.id)}%. Maintenance cost ${formatMoney(maintenanceCost)}.</p>
      <button class="primary-action sheet-small-action" id="sheetMaintainButton" type="button"${state.cash < maintenanceCost ? " disabled" : ""}>Maintain</button>
    `;
    document.querySelector("#sheetMaintainButton")?.addEventListener("click", () => maintainProperty(property.id));
  }
}

function renderMarket() {
  const marketList = document.querySelector("#marketList");
  const available = properties.filter((property) => !state.owned.includes(property.id));
  const playerListings = properties.filter((property) => state.owned.includes(property.id) && state.listedForSale[property.id]);
  const listings = [...playerListings, ...available];

  marketList.innerHTML = "";
  marketList.appendChild(createAuctionCard());
  marketList.insertAdjacentHTML("beforeend", `<p class="list-divider">Open opportunities</p>`);
  marketList.insertAdjacentHTML("beforeend", listings.length
    ? ""
    : `<article class="item-card"><h3>No listings left</h3><p>You own every MVP property in Warsaw and have no active sale listings.</p></article>`);

  playerListings.forEach((property) => {
    marketList.appendChild(createPropertyCard(property, "player-listing"));
  });

  available.forEach((property) => {
    marketList.appendChild(createPropertyCard(property, "buy"));
  });

  marketList.appendChild(createDreamPropertyCard());
}

function createDreamPropertyCard() {
  const unlocked = state.microWins.includes("dream-preview") || state.playerLevel >= 2 || state.owned.length >= 3;
  const card = document.createElement("article");
  card.className = `item-card dream-card${unlocked ? "" : " locked"}`;
  card.innerHTML = `
    <header>
      <h3>${dreamProperty.icon} ${dreamProperty.name}</h3>
      <strong>${formatMoney(dreamProperty.price)}</strong>
    </header>
    <p>${unlocked ? "A prestige property preview for the next market tier." : "Visible horizon: unlock by completing early micro-wins."}</p>
    <div class="item-meta">
      <span>${dreamProperty.district}</span>
      <span>${formatMoney(dreamProperty.rent)} / day</span>
      <span>Tier 2 preview</span>
    </div>
  `;
  return card;
}

function createAuctionCard() {
  const property = properties.find((item) => !state.owned.includes(item.id));
  const card = document.createElement("article");

  if (!property) {
    card.className = "auction-card quiet";
    card.innerHTML = `<span class="auction-kicker">LIVE AUCTION</span><h3>New auctions unlock with the next city tier</h3><p>Keep building your reputation to draw premium sellers into Warsaw.</p>`;
    return card;
  }

  const bid = Math.round(property.price * 0.88 / 500) * 500;
  card.className = "auction-card";
  card.innerHTML = `
    <div><span class="auction-kicker">LIVE AUCTION · 1 day left</span><h3>${property.icon} ${property.name}</h3><p>${property.district} · ${property.category}</p></div>
    <div class="auction-value"><span>Opening bid</span><strong>${formatMoney(bid)}</strong></div>
    <button class="primary-action" type="button"${state.cash < bid ? " disabled" : ""}>Place bid</button>
  `;
  card.querySelector("button")?.addEventListener("click", () => buyProperty(property.id, bid, "auction"));
  return card;
}

function renderPortfolio() {
  const portfolioList = document.querySelector("#portfolioList");
  const owned = properties.filter((property) => state.owned.includes(property.id));
  const homes = owned.filter((property) => property.useType === "residential");
  const workProperties = owned.filter((property) => property.useType === "commercial");

  renderPortfolioFocus(owned);

  if (!owned.length) {
    portfolioList.innerHTML = `<article class="item-card empty-state"><div class="empty-state-icon">⌂</div><h3>Your first building is waiting</h3><p>Start on the map or browse Invest for a property that fits your capital.</p><button class="primary-action" id="emptyInvestButton" type="button">Browse Invest</button></article>`;
    document.querySelector("#emptyInvestButton")?.addEventListener("click", () => showScreen("market"));
    return;
  }

  portfolioList.innerHTML = "";
  portfolioList.appendChild(createPortfolioGroup("People live here", homes, "No homes owned yet."));
  portfolioList.appendChild(createPortfolioGroup("For work and shops", workProperties, "No work properties owned yet."));
}

function renderPortfolioFocus(owned) {
  const tasks = getTodayTasks(owned);
  portfolioFocus.innerHTML = `
    <div class="portfolio-focus-header"><span>Today</span><strong>${tasks.length ? `${tasks.length} decision${tasks.length === 1 ? "" : "s"} waiting` : "Everything is moving"}</strong></div>
    <div class="focus-rail">${tasks.length ? tasks.slice(0, 3).map((task) => `
      <button class="focus-card ${task.tone}" type="button" data-focus-property="${task.propertyId}">
        <span>${task.icon}</span><strong>${task.title}</strong><em>${task.detail}</em>
      </button>`).join("") : `<article class="focus-card calm"><span>✓</span><strong>No urgent actions</strong><em>Use Invest to find your next move.</em></article>`}</div>
  `;

  portfolioFocus.querySelectorAll("[data-focus-property]").forEach((button) => {
    button.addEventListener("click", () => openPropertyInfo(button.dataset.focusProperty));
  });
}

function renderPropertyInfo() {
  const property = properties.find((item) => item.id === state.infoPropertyId) || getSelectedProperty();

  if (!property) {
    propertyInfoDistrict.textContent = "Building";
    propertyInfoTitle.textContent = "Property info";
    propertyInfoView.innerHTML = `<article class="item-card"><h3>No property selected</h3><p>Open a building from your portfolio.</p></article>`;
    return;
  }

  const owned = state.owned.includes(property.id);
  const listedPrice = state.listedForSale[property.id];
  const condition = getCondition(property.id);
  const propertyLevel = getPropertyLevel(property.id);
  const maintenanceCost = getMaintenanceCost(property);
  const suggestedSalePrice = getSuggestedSalePrice(property);
  const upgradeCost = getUpgradeCost(property, 1);
  const boostCost = getInfluenceBoostCost(property);
  const tenant = getTenant(property.id);
  const evictionCase = getEvictionCase(property.id);
  const canSell = owned && !tenant && !evictionCase && !isNotaryPending(property.id);
  const saleHelp = owned && !canSell
    ? "Cancel the current contract first. A rented property cannot be sold until it is ready again."
    : listedPrice
      ? `Listed at ${formatMoney(listedPrice)}. Simulated buyers can purchase it from the market.`
      : "Set an asking price and place this building on the market.";

  propertyInfoDistrict.textContent = `${property.district} · Tier ${property.tier}`;
  propertyInfoTitle.textContent = property.name;

  propertyInfoView.innerHTML = `
    <div class="detail-stack">
      <article class="property-hero-card">
        <div class="property-hero-icon">${property.icon}</div>
        <p>${property.category}</p>
        <h3>${property.name}</h3>
        <strong>Lvl ${propertyLevel}</strong>
        <div class="xp-track"><span style="--xp-width: ${getPropertyProgress(property.id)}%"></span></div>
      </article>
      <article class="detail-panel">
        <h3>Building info</h3>
        <p>${property.description}</p>
        <div class="item-meta">
          <span>Base value ${formatMoney(property.price)}</span>
          <span>Rent ${formatMoney(getActiveRent(property))} / day</span>
          <span>ROI ${getRoi(property)}%</span>
          <span>Neighborhood ${getNeighborhoodScore(property)}/100</span>
          <span>Popularity ${getPopularity(property.id)}</span>
          <span>${getRentalStatus(property)}</span>
          <span>${owned ? "Owned" : "Not owned"}</span>
        </div>
      </article>
      ${owned ? renderLifecyclePanel(property) : ""}
      <article class="detail-panel">
        <h3>Upgrade</h3>
        <p>Raise the building level to increase daily rent and valuation.</p>
        <div class="action-grid two">
          <button class="primary-action" id="upgradeOnceButton" type="button"${!owned || state.cash < upgradeCost ? " disabled" : ""}>Improve x1 · ${formatMoney(upgradeCost)}</button>
          <button class="secondary-action" id="boostPopularityButton" type="button"${!owned || state.influence < boostCost ? " disabled" : ""}>Boost popularity · ${boostCost} influence</button>
        </div>
      </article>
      <article class="detail-panel">
        <h3>Maintenance</h3>
        <p>${getConditionLabel(condition)}</p>
        <div class="condition-meter" style="--condition-width: ${condition}%"><span></span></div>
        <div class="item-meta">
          <span>Condition ${condition}%</span>
          <span>Cost ${formatMoney(maintenanceCost)}</span>
        </div>
        <button class="primary-action" id="maintainPropertyButton" type="button"${!owned || condition >= 100 || state.cash < maintenanceCost ? " disabled" : ""}>Maintain property</button>
      </article>
      <article class="detail-panel">
        <h3>Market sale</h3>
        <p>${saleHelp}</p>
        <div class="sale-price-row">
          <label>
            Asking price
            <input id="salePriceInput" type="number" min="1" step="500" value="${listedPrice || suggestedSalePrice}" ${!canSell ? "disabled" : ""}>
          </label>
          <span>${formatMoney(suggestedSalePrice)}</span>
        </div>
        <div class="action-grid">
          <button class="danger-action" id="listForSaleButton" type="button"${!canSell ? " disabled" : ""}>${listedPrice ? "Update listing" : "Put on market"}</button>
          <button class="secondary-action" id="cancelSaleButton" type="button"${!listedPrice ? " disabled" : ""}>Cancel listing</button>
        </div>
      </article>
    </div>
  `;

  const maintainButton = document.querySelector("#maintainPropertyButton");
  const upgradeButton = document.querySelector("#upgradeOnceButton");
  const boostButton = document.querySelector("#boostPopularityButton");
  const listButton = document.querySelector("#listForSaleButton");
  const cancelButton = document.querySelector("#cancelSaleButton");
  const cancelRentButton = document.querySelector("#cancelRentButton");
  const tenantButtons = document.querySelectorAll("[data-tenant-offer]");
  const evictionButtons = document.querySelectorAll("[data-eviction-action]");
  const renovateButton = document.querySelector("#renovateLifecycleButton");
  const advertiseButton = document.querySelector("#publishAdvertisementButton");
  const negotiationButtons = document.querySelectorAll("[data-negotiation]");
  const signLeaseButton = document.querySelector("#signLeaseButton");

  maintainButton?.addEventListener("click", () => maintainProperty(property.id));
  upgradeButton?.addEventListener("click", () => upgradeProperty(property.id));
  boostButton?.addEventListener("click", () => boostPopularity(property.id));
  cancelRentButton?.addEventListener("click", () => startContractCancellation(property.id));
  tenantButtons.forEach((button) => {
    button.addEventListener("click", () => acceptTenantOffer(property.id, button.dataset.tenantOffer));
  });
  renovateButton?.addEventListener("click", () => renovateForTenant(property.id));
  advertiseButton?.addEventListener("click", () => publishAdvertisement(property.id));
  negotiationButtons.forEach((button) => {
    button.addEventListener("click", () => negotiateOffer(property.id, button.dataset.tenantOffer, button.dataset.negotiation));
  });
  signLeaseButton?.addEventListener("click", () => signLease(property.id));
  evictionButtons.forEach((button) => {
    button.addEventListener("click", () => tryEvictionAction(property.id, button.dataset.evictionAction));
  });
  listButton?.addEventListener("click", () => {
    const input = document.querySelector("#salePriceInput");
    listPropertyForSale(property.id, Number(input.value));
  });
  cancelButton?.addEventListener("click", () => cancelSaleListing(property.id));
}

function createPortfolioGroup(title, groupProperties, emptyText) {
  const section = document.createElement("section");
  section.className = "portfolio-group";
  section.innerHTML = `
    <header>
      <h3>${title}</h3>
      <span>${groupProperties.length}</span>
    </header>
  `;

  if (!groupProperties.length) {
    const empty = document.createElement("p");
    empty.className = "portfolio-empty";
    empty.textContent = emptyText;
    section.appendChild(empty);
    return section;
  }

  [...groupProperties].sort((a, b) => getStatusPriority(a) - getStatusPriority(b)).forEach((property) => {
    section.appendChild(createPropertyCard(property, "owned"));
  });

  return section;
}

function renderLifecyclePanel(property) {
  const tenant = getTenant(property.id);
  const evictionCase = getEvictionCase(property.id);
  const stage = getPropertyStage(property.id);
  const advertisement = getAdvertisement(property.id);

  if (stage.stage === "notary") {
    return `
      <article class="detail-panel lifecycle-panel">
        <h3>Waiting for Notary</h3>
        <p>Ownership paperwork is in progress. The property becomes available after ${stage.daysRemaining} simulated day${stage.daysRemaining === 1 ? "" : "s"}.</p>
        <div class="lifecycle-track"><span class="active">Purchase</span><span class="active">Notary</span><span>Prepare</span><span>Advertise</span><span>Lease</span></div>
      </article>
    `;
  }

  if (stage.stage === "ready-renovation") {
    const cost = getRenovationCost(property);
    return `
      <article class="detail-panel lifecycle-panel">
        <h3>Ready for renovation</h3>
        <p>Improve the condition before advertising, or advertise immediately for a faster but weaker applicant pool.</p>
        <div class="action-grid two">
          <button class="primary-action" id="renovateLifecycleButton" type="button"${state.cash < cost ? " disabled" : ""}>Renovate · ${formatMoney(cost)}</button>
          <button class="secondary-action" id="publishAdvertisementButton" type="button">Advertise now</button>
        </div>
      </article>
    `;
  }

  if (stage.stage === "advertising" && advertisement && !advertisement.applications.length) {
    return `
      <article class="detail-panel lifecycle-panel">
        <h3>Advertisement live</h3>
        <p>Your listing is gathering demand. Simulate another day to generate more views, visits and applications.</p>
        <div class="item-meta">
          <span>${advertisement.views} views</span><span>${advertisement.visits} visits</span><span>0 applications</span>
        </div>
        <div class="lifecycle-track"><span class="active">Prepare</span><span class="active">Advertise</span><span>Applications</span><span>Lease</span></div>
      </article>
    `;
  }

  if (stage.stage === "advertising" && advertisement?.applications.length) {
    const offers = getTenantOffers(property).filter((offer) => advertisement.applications.includes(offer.id));
    return `
      <article class="detail-panel lifecycle-panel">
        <h3>Applications waiting</h3>
        <p>${advertisement.views} views / ${advertisement.visits} visits / ${offers.length} applications. Each applicant has a story, but their deeper traits will reveal themselves over time.</p>
        <div class="offer-list">
          ${offers.map((offer) => `
            <article class="offer-card story-offer">
              <strong>${offer.name}</strong>
              <span>${offer.story}</span>
              <em>${formatMoney(offer.rent)} / day${offer.penalty ? ` · ${offer.penalty}% saturation penalty` : ""}</em>
              <div class="offer-actions">
                <button type="button" class="primary-action" data-tenant-offer="${offer.id}">Accept</button>
                <button type="button" class="secondary-action" data-tenant-offer="${offer.id}" data-negotiation="counter">Counter</button>
                <button type="button" class="secondary-action" data-tenant-offer="${offer.id}" data-negotiation="deposit">Deposit</button>
              </div>
            </article>
          `).join("")}
        </div>
      </article>
    `;
  }

  if (stage.stage === "lease-pending" && tenant) {
    return `
      <article class="detail-panel lifecycle-panel">
        <h3>Agreement prepared</h3>
        <p>${tenant.name} accepted the terms. Sign the lease to arrange key handover and start the first payment cycle.</p>
        <div class="item-meta"><span>${formatMoney(tenant.rent)} / day</span><span>${tenant.label}</span><span>Move-in tomorrow</span></div>
        <button class="primary-action" id="signLeaseButton" type="button">Sign lease</button>
      </article>
    `;
  }

  if (evictionCase) {
    const chance = getEvictionChance(evictionCase);
    const actionLocked = evictionCase.lastActionDay === state.day;
    return `
      <article class="detail-panel warning">
        <h3>Problematic renter</h3>
        <p>${tenant ? tenant.name : "The tenant"} does not want to leave. The flat will be ready in ${evictionCase.daysRemaining} day${evictionCase.daysRemaining === 1 ? "" : "s"} unless an action works earlier.</p>
        <div class="item-meta">
          <span>Pressure ${evictionCase.pressure}</span>
          <span>Leave chance ${chance}%</span>
          <span>${actionLocked ? "Next action tomorrow" : "Action available today"}</span>
        </div>
        <div class="action-grid two">
          <button class="secondary-action" type="button" data-eviction-action="ask" ${actionLocked ? "disabled" : ""}>Ask to leave</button>
          <button class="secondary-action" type="button" data-eviction-action="mail" ${actionLocked ? "disabled" : ""}>Send mail</button>
          <button class="primary-action" type="button" data-eviction-action="lawyer" ${actionLocked || state.cash < 1200 ? "disabled" : ""}>Send lawyer · ${formatMoney(1200)}</button>
          <button class="danger-action" type="button" data-eviction-action="police" ${actionLocked || state.influence < 8 ? "disabled" : ""}>Call police · 8 influence</button>
        </div>
      </article>
    `;
  }

  if (tenant) {
    const memory = state.tenantMemories[tenant.id];
    const latePayment = Boolean(state.latePayments[property.id]);
    return `
      <article class="detail-panel rented-panel">
        <h3>${latePayment ? "Late payment" : "Occupied"}</h3>
        <p>${tenant.name} · ${tenant.label}. ${tenant.story || "Lease active."}</p>
        <div class="item-meta">
          <span>Rent ${formatMoney(tenant.rent)} / day</span>
          <span>${tenant.leaseDays || 0} days staying</span>
          <span>Relationship ${memory?.relationship ?? tenant.relationship ?? 50}</span>
          <span>${latePayment ? "Payment follow-up needed" : tenant.paymentHistory || "Payments on time"}</span>
        </div>
        <button class="danger-action" id="cancelRentButton" type="button">Cancel contract</button>
      </article>
    `;
  }

  return `
    <article class="detail-panel lifecycle-panel">
      <h3>Ready to advertise</h3>
      <p>${property.useType === "commercial" ? "Find a business tenant. Nearby businesses of the same type lower the rent you can ask." : "Publish an ad to begin the demand cycle and meet prospective tenants."}</p>
      <button class="primary-action" id="publishAdvertisementButton" type="button">Publish advertisement</button>
    </article>
  `;
}

function renderIssues() {
  const issuesList = document.querySelector("#issuesList");
  const needsRepair = properties.filter(
    (property) => state.owned.includes(property.id) && property.tier <= 2 && !state.repaired.includes(property.id)
  );
  const streetIssues = getStreetIssues();
  const applicationProperties = properties.filter((property) => canReviewApplications(property.id));

  issuesList.innerHTML = needsRepair.length || streetIssues.length || applicationProperties.length
    ? ""
    : `<article class="item-card"><h3>All clear</h3><p>No active issues for the current offline day.</p></article>`;

  streetIssues.forEach((issue) => {
    const card = document.createElement("article");
    card.className = "item-card warning";
    card.innerHTML = `
      <header>
        <h3>${issue.icon} ${issue.title}</h3>
        <strong>${issue.reward}</strong>
      </header>
      <p>${issue.description}</p>
      <button class="secondary-action" type="button">${issue.action}</button>
    `;
    card.querySelector("button").addEventListener("click", () => resolveStreetIssue(issue.id));
    issuesList.appendChild(card);
  });

  applicationProperties.forEach((property) => {
    const card = document.createElement("article");
    card.className = "item-card";
    card.innerHTML = `
      <header><h3>${property.icon} New applications</h3><strong>${getAdvertisement(property.id).applications.length}</strong></header>
      <p>${property.name} has applicants waiting for your decision.</p>
      <button class="primary-action" type="button">Review applications</button>
    `;
    card.querySelector("button").addEventListener("click", () => openPropertyInfo(property.id));
    issuesList.appendChild(card);
  });

  needsRepair.forEach((property) => {
    const repairCost = getMaintenanceCost(property);
    const card = document.createElement("article");
    card.className = "item-card warning";
    card.innerHTML = `
      <header>
        <h3>${property.icon} ${property.name}</h3>
        <strong>${formatMoney(repairCost)}</strong>
      </header>
      <p>Tenant satisfaction is falling. Maintenance improves condition and marks the issue as resolved.</p>
      <button class="secondary-action" type="button">Maintain</button>
    `;
    card.querySelector("button").addEventListener("click", () => repairProperty(property.id, repairCost));
    issuesList.appendChild(card);
  });
}

function getStreetIssues() {
  if (!state.owned.length) {
    return [];
  }

  return [
    {
      id: "graffiti",
      icon: "🧽",
      title: "Graffiti on entrance",
      description: "Clean the entrance to raise neighborhood confidence.",
      action: "Remove graffiti",
      reward: "+Neighborhood"
    },
    {
      id: "street-cleanup",
      icon: "✨",
      title: "Street cleanup",
      description: "A quick local improvement that makes tenants happier.",
      action: "Clean street",
      reward: "+Reputation"
    }
  ].filter((issue) => !state.streetIssuesResolved.includes(issue.id));
}

function resolveStreetIssue(issueId) {
  if (state.streetIssuesResolved.includes(issueId)) {
    return;
  }

  state.streetIssuesResolved.push(issueId);
  state.influence += 3;
  addXp(12);
  trackMicroWin("first-issue");
  if (state.tutorialState.currentStep === "tasks") {
    advanceTutorial("ranking");
  }
  pushToast("Neighborhood improved.");
  pushCeremony(issueId === "graffiti" ? "Graffiti removed" : "Street cleaned");
  saveState();
  render();
}

function renderRanking() {
  trackMicroWin("rankings", false);
  const rankingList = document.querySelector("#rankingList");
  const playerScore = state.cash + properties
    .filter((property) => state.owned.includes(property.id))
    .reduce((total, property) => total + getSuggestedSalePrice(property), 0);

  const rows = [
    ...rivals,
    { name: "You", score: playerScore }
  ].sort((a, b) => b.score - a.score);

  rankingList.innerHTML = "";
  rows.forEach((row, index) => {
    const card = document.createElement("article");
    card.className = "item-card";
    card.innerHTML = `
      <header>
        <h3>${index + 1}. ${row.name}</h3>
        <strong>${formatMoney(row.score)}</strong>
      </header>
      <p>${row.name === "You" ? "Your offline MVP valuation." : "Simulated local competitor."}</p>
    `;
    rankingList.appendChild(card);
  });
}

function createPropertyCard(property, mode) {
  const owned = state.owned.includes(property.id);
  const listedPrice = state.listedForSale[property.id];
  const rentalStatus = getPortfolioStatus(property);
  const rentForCard = owned ? getActiveRent(property) : getPotentialRent(property);
  const card = document.createElement("article");
  card.className = `item-card${listedPrice ? " listed" : ""}${mode === "owned" ? ` property-status-card ${getStatusTone(property)}` : ""}`;
  card.innerHTML = mode === "owned" ? `
    <header>
      <div class="property-card-title"><span class="property-category-icon">${getCategoryIcon(property)}</span><div><h3>${property.name}</h3><p>${property.district} · ${property.useType === "commercial" ? "Work space" : "Home"}</p></div></div>
      <strong>${formatMoney(getMonthlyRent(property))}</strong>
    </header>
    <div class="property-status-line"><span class="status-dot"></span>${rentalStatus}</div>
    <div class="property-card-facts"><span>Monthly rent</span><span>${getNextPaymentLabel(property)}</span><span>${getOccupancyLabel(property)}</span></div>
  ` : `
    <header><h3>${property.icon} ${property.name}</h3><strong>${formatMoney(listedPrice || property.price)}</strong></header>
    <p>${property.description}</p>
    <div class="item-meta"><span>${property.district}</span><span>Lvl ${getPropertyLevel(property.id)}</span><span>${formatMoney(rentForCard)} / day</span><span>ROI ${getRoi(property)}%</span></div>
  `;

  if (mode === "buy") {
    const button = document.createElement("button");
    button.className = state.cash >= property.price ? "primary-action" : "secondary-action";
    button.type = "button";
    button.textContent = state.cash >= property.price ? "Buy" : "Need more cash";
    button.disabled = state.cash < property.price;
    button.addEventListener("click", () => buyProperty(property.id));
    card.appendChild(button);
  } else if (mode === "player-listing") {
    const simulateButton = document.createElement("button");
    simulateButton.className = "danger-action";
    simulateButton.type = "button";
    simulateButton.textContent = "Accept simulated buyer";
    simulateButton.addEventListener("click", () => sellListedProperty(property.id));
    card.appendChild(simulateButton);

    const infoButton = document.createElement("button");
    infoButton.className = "secondary-action";
    infoButton.type = "button";
    infoButton.textContent = "Listing info";
    infoButton.addEventListener("click", () => openPropertyInfo(property.id));
    card.appendChild(infoButton);
  } else if (owned) {
    const actions = document.createElement("div");
    actions.className = "compact-actions";

    const infoButton = document.createElement("button");
    infoButton.className = "primary-action";
    infoButton.type = "button";
    infoButton.textContent = "Info";
    infoButton.addEventListener("click", () => openPropertyInfo(property.id));
    actions.appendChild(infoButton);

    const mapButton = document.createElement("button");
    mapButton.className = "secondary-action";
    mapButton.type = "button";
    mapButton.textContent = "Map";
    mapButton.addEventListener("click", () => {
      state.selectedId = property.id;
      state.mapSheetOpen = true;
      saveState();
      showScreen("map");
      render();
    });
    actions.appendChild(mapButton);
    card.appendChild(actions);
  }

  return card;
}

function buyProperty(propertyId, priceOverride = null, source = "listing") {
  const property = properties.find((item) => item.id === propertyId);
  const purchasePrice = priceOverride || property?.price;

  if (!property || state.owned.includes(propertyId) || state.cash < purchasePrice) {
    return;
  }

  state.cash -= purchasePrice;
  state.owned.push(propertyId);
  state.conditions[propertyId] = getCondition(propertyId);
  state.propertyLevels[propertyId] = getPropertyLevel(propertyId);
  state.propertyStages[propertyId] = { stage: "notary", daysRemaining: 1 };
  state.selectedId = propertyId;
  state.infoPropertyId = propertyId;
  addXp(20);
  trackMicroWin("first-purchase");
  advanceTutorial("notary");
  pushToast(`${property.name} reserved. Notary paperwork starts now.`);
  pushCeremony(source === "auction" ? "Auction won · Notary pending" : "Signed · Notary pending");
  saveState();
  render();
  showScreen("portfolio");
}

function repairProperty(propertyId, cost) {
  if (state.cash < cost || state.repaired.includes(propertyId)) {
    return;
  }

  state.cash -= cost;
  state.conditions[propertyId] = Math.min(100, getCondition(propertyId) + 25);
  if (!state.repaired.includes(propertyId)) {
    state.repaired.push(propertyId);
  }
  addXp(8);
  trackMicroWin("first-renovation");
  if (state.tutorialState.currentStep === "tasks") {
    advanceTutorial("ranking");
  }
  pushToast("Maintenance complete.");
  pushCeremony("Maintenance scheduled");
  saveState();
  render();
}

function maintainProperty(propertyId) {
  const property = properties.find((item) => item.id === propertyId);

  if (!property) {
    return;
  }

  repairProperty(propertyId, getMaintenanceCost(property));
}

function listPropertyForSale(propertyId, price) {
  if (!state.owned.includes(propertyId) || getTenant(propertyId) || getEvictionCase(propertyId) || isNotaryPending(propertyId) || !Number.isFinite(price) || price <= 0) {
    pushToast("Finish notary paperwork and cancel active contracts before selling.");
    saveState();
    render();
    return;
  }

  state.listedForSale[propertyId] = Math.round(price);
  addXp(5);
  trackMicroWin("dream-preview", false);
  pushToast("Property listed on the market.");
  saveState();
  render();
  showScreen("market");
}

function cancelSaleListing(propertyId) {
  delete state.listedForSale[propertyId];
  pushToast("Listing cancelled.");
  saveState();
  render();
}

function sellListedProperty(propertyId) {
  const listedPrice = state.listedForSale[propertyId];

  if (!state.owned.includes(propertyId) || getTenant(propertyId) || getEvictionCase(propertyId) || isNotaryPending(propertyId) || !listedPrice) {
    pushToast("This property cannot be sold while paperwork or a tenant contract is active.");
    saveState();
    render();
    return;
  }

  state.cash += listedPrice;
  state.owned = state.owned.filter((id) => id !== propertyId);
  state.repaired = state.repaired.filter((id) => id !== propertyId);
  delete state.conditions[propertyId];
  delete state.propertyLevels[propertyId];
  delete state.propertyStages[propertyId];
  delete state.advertisements[propertyId];
  delete state.latePayments[propertyId];
  delete state.listedForSale[propertyId];
  addXp(35);
  pushToast(`Sold for ${formatMoney(listedPrice)}.`);
  saveState();
  render();
}

function collectRent() {
  const collectableRent = getAccruedRentTotal();

  if (collectableRent <= 0) {
    pushToast("No rent waiting yet. Simulate a day or sign a lease first.");
    saveState();
    render();
    return;
  }

  state.cash += collectableRent;
  state.rentAccrued.residential = 0;
  state.rentAccrued.commercial = 0;
  addXp(Math.max(3, state.owned.length * 3));
  if (collectableRent > 0) {
    trackMicroWin("first-rent");
    if (state.tutorialState.currentStep === "rent") {
      advanceTutorial("tasks");
    }
  }
  pushToast(`Collected ${formatMoney(collectableRent)}. Rent queue cleared.`);
  pushCeremony(`+${formatMoney(collectableRent)}`);
  saveState();
  render();
}

function simulateDay() {
  state.day += 1;
  processPropertyLifecycle();
  processEvictionDays();
  const accrued = accrueDailyRent();
  addDailyCityMoment();
  awardDailyMomentum();
  pushToast(accrued > 0 ? `New day: ${formatMoney(accrued)} rent is waiting.` : "New day simulated. Check your property lifecycle.");
  saveState();
  render();
}

function claimDailyReward() {
  if (state.dailyRewardClaimedDay === state.day) {
    pushToast("Daily reward already claimed today.");
    saveState();
    render();
    return;
  }

  state.dailyRewardClaimedDay = state.day;
  state.cash += 1500 + state.playerLevel * 500;
  state.influence += 4;
  addXp(10);
  trackMicroWin("daily-reward");
  pushToast("Daily reward claimed.");
  pushCeremony("Daily reward claimed");
  saveState();
  render();
}

function acceptTenantOffer(propertyId, offerId) {
  const property = properties.find((item) => item.id === propertyId);
  const offer = getTenantOffers(property).find((item) => item.id === offerId);

  if (!property || !offer || !state.owned.includes(propertyId) || getTenant(propertyId) || getEvictionCase(propertyId) || !canReviewApplications(propertyId)) {
    return;
  }

  state.tenants[propertyId] = {
    id: offer.id,
    name: offer.name,
    category: offer.category,
    label: offer.label,
    risk: offer.risk,
    rent: offer.rent,
    problematic: offer.category === "problematic" || offer.risk === "high",
    startedDay: state.day,
    story: offer.story,
    leaseDays: 0,
    relationship: offer.risk === "low" ? 62 : offer.risk === "medium" ? 48 : 38,
    paymentHistory: "New lease"
  };
  state.propertyStages[propertyId] = { stage: "lease-pending" };
  delete state.advertisements[propertyId];
  state.tenantMemories[offer.id] = {
    name: offer.name,
    startedDay: state.day,
    relationship: state.tenants[propertyId].relationship,
    events: ["Agreement prepared"]
  };
  delete state.listedForSale[propertyId];
  addXp(10);
  trackMicroWin("first-tenant");
  advanceTutorial("lease");
  pushToast(`${offer.name} accepted. Review and sign the lease.`);
  pushCeremony("Agreement prepared");
  saveState();
  render();
}

function signLease(propertyId) {
  const tenant = getTenant(propertyId);
  const stage = getPropertyStage(propertyId);

  if (!tenant || stage.stage !== "lease-pending") {
    return;
  }

  state.propertyStages[propertyId] = { stage: "occupied" };
  const memory = state.tenantMemories[tenant.id];
  if (memory) {
    memory.events.push("Lease signed");
  }
  advanceTutorial("rent");
  pushToast("Lease signed. The first rent will arrive after the next day starts.");
  pushCeremony("Keys handed over");
  saveState();
  render();
}

function startContractCancellation(propertyId) {
  const tenant = getTenant(propertyId);

  if (!tenant || getEvictionCase(propertyId)) {
    return;
  }

  state.evictionCases[propertyId] = {
    daysRemaining: tenant.problematic ? 5 : 3,
    pressure: tenant.problematic ? 8 : 22,
    lastActionDay: null,
    problematic: tenant.problematic
  };
  delete state.listedForSale[propertyId];
  pushToast(tenant.problematic ? "Problematic renter opened. Try one action per day." : "Contract cancellation started.");
  saveState();
  render();
}

function tryEvictionAction(propertyId, action) {
  const evictionCase = getEvictionCase(propertyId);

  if (!evictionCase || evictionCase.lastActionDay === state.day) {
    return;
  }

  const actionConfig = {
    ask: { pressure: 10, cost: 0, influence: 0, label: "You asked them to leave." },
    mail: { pressure: 16, cost: 0, influence: 0, label: "Formal mail sent." },
    lawyer: { pressure: 28, cost: 1200, influence: 0, label: "Lawyer notice delivered." },
    police: { pressure: 36, cost: 0, influence: 8, label: "Police assistance requested." }
  }[action];

  if (!actionConfig || state.cash < actionConfig.cost || state.influence < actionConfig.influence) {
    return;
  }

  state.cash -= actionConfig.cost;
  state.influence -= actionConfig.influence;
  evictionCase.pressure += actionConfig.pressure;
  evictionCase.lastActionDay = state.day;

  const roll = deterministicRoll(`${propertyId}-${state.day}-${action}-${evictionCase.pressure}`);
  if (roll < getEvictionChance(evictionCase)) {
    releaseProperty(propertyId);
    addXp(18);
    pushToast("The tenant left. Property is ready.");
  } else {
    evictionCase.daysRemaining = Math.max(1, evictionCase.daysRemaining - 1);
    pushToast(`${actionConfig.label} They still refuse to leave.`);
  }

  saveState();
  render();
}

function processEvictionDays() {
  Object.entries(state.evictionCases).forEach(([propertyId, evictionCase]) => {
    evictionCase.daysRemaining -= 1;

    if (evictionCase.daysRemaining <= 0) {
      releaseProperty(propertyId);
    }
  });
}

function processPropertyLifecycle() {
  properties.forEach((property) => {
    if (!state.owned.includes(property.id)) {
      return;
    }

    const stage = getPropertyStage(property.id);
    const tenant = getTenant(property.id);

    if (stage.stage === "notary") {
      stage.daysRemaining -= 1;
      if (stage.daysRemaining <= 0) {
        state.propertyStages[property.id] = { stage: "ready-renovation" };
        if (state.tutorialState.currentStep === "notary") {
          advanceTutorial("handover");
        }
        pushCeremony(`${property.name} cleared by notary`);
      }
      return;
    }

    if (stage.stage === "advertising") {
      const advertisement = getAdvertisement(property.id);
      advertisement.views += 18 + property.tier * 9 + Math.round(getCondition(property.id) / 12);
      advertisement.visits += Math.max(1, Math.floor(advertisement.views / 22) - advertisement.visits);
      const desiredApplications = Math.min(3, Math.floor(advertisement.visits / 2));
      const offers = getTenantOffers(property);
      while (advertisement.applications.length < desiredApplications) {
        const nextOffer = offers[advertisement.applications.length];
        if (!nextOffer) {
          break;
        }
        advertisement.applications.push(nextOffer.id);
      }
      if (advertisement.applications.length && state.tutorialState.currentStep === "applications") {
        advanceTutorial("tenant");
      }
    }

    if (tenant) {
      tenant.leaseDays = Number(tenant.leaseDays || 0) + 1;
      const wasLate = Boolean(state.latePayments[property.id]);
      if (wasLate) {
        delete state.latePayments[property.id];
        tenant.paymentHistory = "Late payment received";
        addAccruedRent(property.useType, tenant.rent);
      } else if (tenant.risk === "high" && deterministicRoll(`${property.id}-late-${state.day}`) < 18) {
        state.latePayments[property.id] = state.day;
        tenant.paymentHistory = "Late payment today";
      } else {
        tenant.paymentHistory = "Payments on time";
      }
      const memory = state.tenantMemories[tenant.id];
      if (memory) {
        memory.relationship = Math.max(0, Math.min(100, Number(memory.relationship || tenant.relationship || 50) + (wasLate ? -4 : 1)));
      }
    }
  });
}

function renovateForTenant(propertyId) {
  const property = properties.find((item) => item.id === propertyId);
  const stage = getPropertyStage(propertyId);
  const cost = property ? getRenovationCost(property) : 0;

  if (!property || stage.stage !== "ready-renovation" || state.cash < cost) {
    return;
  }

  state.cash -= cost;
  state.conditions[propertyId] = Math.min(100, getCondition(propertyId) + 22);
  state.repaired = Array.from(new Set([...state.repaired, propertyId]));
  state.propertyStages[propertyId] = { stage: "ready-advertise" };
  addXp(14);
  trackMicroWin("first-renovation");
  if (state.tutorialState.currentStep === "handover") {
    advanceTutorial("listing");
  }
  pushToast("Renovation complete. The property is ready to advertise.");
  saveState();
  render();
}

function publishAdvertisement(propertyId) {
  const stage = getPropertyStage(propertyId);

  if (!["ready-renovation", "ready-advertise", "vacant"].includes(stage.stage) || getTenant(propertyId)) {
    return;
  }

  state.propertyStages[propertyId] = { stage: "advertising" };
  state.advertisements[propertyId] = { views: 0, visits: 0, applications: [], publishedDay: state.day };
  advanceTutorial("applications");
  pushToast("Advertisement published. Demand will grow each simulated day.");
  saveState();
  render();
}

function negotiateOffer(propertyId, offerId, action) {
  const property = properties.find((item) => item.id === propertyId);
  const offer = getTenantOffers(property).find((item) => item.id === offerId);

  if (!property || !offer || !canReviewApplications(propertyId)) {
    return;
  }

  if (action === "counter") {
    offer.rent = Math.max(1, offer.rent - 50);
    state.advertisements[propertyId].negotiated = {
      ...(state.advertisements[propertyId].negotiated || {}),
      [offerId]: offer.rent
    };
    pushToast(`${offer.name} accepted a lower counter-offer: ${formatMoney(offer.rent)} / day.`);
  } else if (action === "deposit") {
    state.cash += Math.round(offer.rent * 0.5);
    pushToast(`${offer.name} agreed to an additional deposit.`);
  }

  saveState();
  render();
}

function releaseProperty(propertyId) {
  delete state.tenants[propertyId];
  delete state.evictionCases[propertyId];
  delete state.latePayments[propertyId];
  state.propertyStages[propertyId] = { stage: "vacant" };
  pushToast("Property is empty and ready to rent or sell.");
}

function upgradeProperty(propertyId) {
  const property = properties.find((item) => item.id === propertyId);

  if (!property || !state.owned.includes(propertyId)) {
    return;
  }

  const cost = getUpgradeCost(property, 1);
  if (state.cash < cost) {
    return;
  }

  state.cash -= cost;
  state.propertyLevels[propertyId] = getPropertyLevel(propertyId) + 1;
  addXp(12 + property.tier * 2);
  pushToast(`${property.name} reached level ${state.propertyLevels[propertyId]}.`);
  saveState();
  render();
}

function boostPopularity(propertyId) {
  const property = properties.find((item) => item.id === propertyId);

  if (!property || !state.owned.includes(propertyId)) {
    return;
  }

  const cost = getInfluenceBoostCost(property);
  if (state.influence < cost) {
    return;
  }

  state.influence -= cost;
  state.propertyLevels[propertyId] = getPropertyLevel(propertyId) + 1;
  state.conditions[propertyId] = Math.min(100, getCondition(propertyId) + 8);
  addXp(18);
  pushToast("Popularity boosted.");
  saveState();
  render();
}

function showScreen(name) {
  screens.forEach((screen) => {
    screen.classList.toggle("active", screen.id === `screen-${name}`);
  });

  tabs.forEach((tab) => {
    tab.classList.toggle("active", tab.dataset.screen === name);
  });
}

function getSelectedProperty() {
  return properties.find((property) => property.id === state.selectedId);
}

function openPropertyInfo(propertyId) {
  state.infoPropertyId = propertyId;
  state.selectedId = propertyId;
  state.mapSheetOpen = false;
  saveState();
  render();
  showScreen("property");
}

function getCondition(propertyId) {
  return Math.max(20, Math.min(100, Number(state.conditions[propertyId] ?? getStartingCondition(propertyId))));
}

function getPropertyLevel(propertyId) {
  return Math.max(1, Math.min(100, Number(state.propertyLevels[propertyId] ?? 1)));
}

function getPropertyProgress(propertyId) {
  return Math.min(100, getPropertyLevel(propertyId));
}

function getPopularity(propertyId) {
  return Math.min(100, Math.round(getCondition(propertyId) * 0.45 + getPropertyLevel(propertyId) * 5));
}

function getRoi(property) {
  const yearlyRent = Math.max(getActiveRent(property), getPotentialRent(property)) * 365;
  return Math.round((yearlyRent / property.price) * 1000) / 10;
}

function getNeighborhoodScore(property) {
  return Math.min(100, Math.round(58 + property.tier * 8 + getCondition(property.id) * 0.18 + getPropertyLevel(property.id) * 2));
}

function getStartingCondition(propertyId) {
  const property = properties.find((item) => item.id === propertyId);

  if (!property) {
    return 80;
  }

  return Math.max(55, 92 - property.tier * 8);
}

function getPotentialRent(property) {
  if (state.listedForSale[property.id] || isNotaryPending(property.id)) {
    return 0;
  }

  return Math.round(property.rent * (0.65 + getCondition(property.id) / 200) * (1 + (getPropertyLevel(property.id) - 1) * 0.12));
}

function getActiveRent(property) {
  const tenant = getTenant(property.id);
  const evictionCase = getEvictionCase(property.id);
  const stage = getPropertyStage(property.id);

  if (!tenant || stage.stage !== "occupied" || evictionCase || state.listedForSale[property.id] || state.latePayments[property.id]) {
    return 0;
  }

  return tenant.rent;
}

function getRentByUseType(useType) {
  return properties
    .filter((property) => state.owned.includes(property.id) && property.useType === useType)
    .reduce((total, property) => total + getActiveRent(property), 0);
}

function getAccruedRentByUseType(useType) {
  return Math.max(0, Math.round(Number(state.rentAccrued?.[useType] || 0)));
}

function getAccruedRentTotal() {
  return getAccruedRentByUseType("residential") + getAccruedRentByUseType("commercial");
}

function addAccruedRent(useType, amount) {
  const bucket = useType === "commercial" ? "commercial" : "residential";
  state.rentAccrued[bucket] = getAccruedRentByUseType(bucket) + Math.max(0, Math.round(amount));
}

function accrueDailyRent() {
  let accrued = 0;

  properties.forEach((property) => {
    if (!state.owned.includes(property.id)) {
      return;
    }

    const rent = getActiveRent(property);
    if (rent <= 0) {
      return;
    }

    addAccruedRent(property.useType, rent);
    accrued += rent;
  });

  return accrued;
}

function getIssueCount() {
  const needsRepair = properties.filter(
    (property) => state.owned.includes(property.id) && property.tier <= 2 && !state.repaired.includes(property.id)
  ).length;
  const streetIssueCount = getStreetIssues().length;
  const evictionActions = Object.values(state.evictionCases).filter((evictionCase) => evictionCase.lastActionDay !== state.day).length;

  const applications = Object.values(state.advertisements).filter((advertisement) => advertisement.applications?.length).length;
  const latePayments = Object.keys(state.latePayments).length;

  return needsRepair + streetIssueCount + evictionActions + applications + latePayments;
}

function getTodayTasks(ownedProperties = properties.filter((property) => state.owned.includes(property.id))) {
  return ownedProperties.flatMap((property) => {
    const stage = getPropertyStage(property.id);
    const tenant = getTenant(property.id);
    const advertisement = getAdvertisement(property.id);

    if (state.latePayments[property.id]) {
      return [{ propertyId: property.id, icon: "!", tone: "urgent", title: "Late payment", detail: `${property.name} needs follow-up` }];
    }
    if (getEvictionCase(property.id)) {
      return [{ propertyId: property.id, icon: "!", tone: "urgent", title: "Tenant departure", detail: `${property.name} is in cancellation` }];
    }
    if (advertisement?.applications?.length) {
      return [{ propertyId: property.id, icon: "✦", tone: "attention", title: "${advertisement.applications.length} applications", detail: property.name }];
    }
    if (stage.stage === "ready-renovation") {
      return [{ propertyId: property.id, icon: "⌂", tone: "attention", title: "Prepare property", detail: `${property.name} is ready` }];
    }
    if (stage.stage === "ready-advertise" || stage.stage === "vacant") {
      return [{ propertyId: property.id, icon: "⌁", tone: "attention", title: "Find a tenant", detail: property.name }];
    }
    if (tenant && !getActiveRent(property) && !state.listedForSale[property.id]) {
      return [{ propertyId: property.id, icon: "•", tone: "calm", title: "Rent due", detail: `${property.name} is waiting` }];
    }
    return [];
  });
}

function getCategoryIcon(property) {
  if (property.useType === "commercial") {
    return "▦";
  }
  return "⌂";
}

function getStatusTone(property) {
  const stage = getPropertyStage(property.id);
  if (state.latePayments[property.id] || getEvictionCase(property.id)) {
    return "urgent";
  }
  if (getAdvertisement(property.id)?.applications?.length || ["ready-renovation", "ready-advertise", "vacant"].includes(stage.stage)) {
    return "attention";
  }
  return "calm";
}

function getStatusPriority(property) {
  const tone = getStatusTone(property);
  return tone === "urgent" ? 0 : tone === "attention" ? 1 : 2;
}

function getMonthlyRent(property) {
  const rent = getActiveRent(property) || getPotentialRent(property);
  return formatMoney(rent * 30);
}

function getNextPaymentLabel(property) {
  if (state.latePayments[property.id]) {
    return "Payment late";
  }
  if (getTenant(property.id)) {
    return getAccruedRentTotal() > 0 ? "Collect today" : "Due tomorrow";
  }
  return "No lease";
}

function getOccupancyLabel(property) {
  const stage = getPropertyStage(property.id);
  if (getTenant(property.id)) {
    return "Occupied";
  }
  if (stage.stage === "notary") {
    return `Notary ${stage.daysRemaining}d`;
  }
  return stage.stage === "advertising" ? "Advertising" : "Vacant";
}

function addDailyCityMoment() {
  const available = properties.filter((property) => !state.owned.includes(property.id));
  const subject = available[state.day % Math.max(available.length, 1)] || properties[0];
  const moments = [
    `Auction watch: ${subject.name} is drawing bids in ${subject.district}.`,
    `Neighborhood news: foot traffic is rising near ${subject.district}.`,
    `Local player activity: a new ${subject.category.toLowerCase()} deal closed in ${subject.district}.`
  ];
  state.cityEvents.unshift(moments[state.day % moments.length]);
  state.cityEvents = state.cityEvents.slice(0, 3);
}

function awardDailyMomentum() {
  if (state.dailyMomentum.includes(state.day)) {
    return;
  }
  state.dailyMomentum.push(state.day);
  const reward = 100 + state.owned.length * 40;
  state.cash += reward;
  pushCeremony(`Daily momentum · +${formatMoney(reward)}`);
}

function getTenant(propertyId) {
  return state.tenants[propertyId] || null;
}

function getEvictionCase(propertyId) {
  return state.evictionCases[propertyId] || null;
}

function getRentalStatus(property) {
  return getPortfolioStatus(property);
}

function getPortfolioStatus(property) {
  const tenant = getTenant(property.id);
  const evictionCase = getEvictionCase(property.id);
  const stage = getPropertyStage(property.id);
  const advertisement = getAdvertisement(property.id);

  if (stage.stage === "notary") {
    return `Waiting for Notary · ${stage.daysRemaining}d remaining`;
  }

  if (evictionCase) {
    return `Cancelling · ready in ${evictionCase.daysRemaining}d`;
  }

  if (state.latePayments[property.id]) {
    return `Late Payment · ${tenant?.name || "tenant"}`;
  }

  if (tenant) {
    const due = getAccruedRentTotal() > 0 ? "Rent Due Today" : "Rent due tomorrow";
    return `Occupied by ${tenant.name} · ${due}`;
  }

  if (stage.stage === "ready-renovation") {
    return "Ready for Renovation";
  }

  if (stage.stage === "advertising") {
    return advertisement?.applications?.length
      ? `Applications Waiting · ${advertisement.applications.length}`
      : `Advertisement Live · ${advertisement?.views || 0} views / ${advertisement?.visits || 0} visits`;
  }

  return "Ready to Advertise";
}

function getTenantOffers(property) {
  if (!property) {
    return [];
  }

  const source = property.useType === "commercial" ? commercialTenants : residentialTenants;

  return source.slice(0, 5).map((tenant, index) => {
    const saturationPenalty = property.useType === "commercial"
      ? getSaturationPenalty(property, tenant.category)
      : 0;
    const riskPremium = tenant.risk === "high" ? 1.1 : tenant.risk === "medium" ? 1.04 : 1;
    const calculatedRent = Math.round(getPotentialRent(property) * tenant.multiplier * riskPremium * (1 - saturationPenalty / 100));
    const existingNegotiation = getAdvertisement(property.id)?.negotiated?.[`${tenant.category}-${index}`];

    return {
      ...tenant,
      id: `${tenant.category}-${index}`,
      rent: existingNegotiation || calculatedRent,
      penalty: saturationPenalty,
      story: getApplicantStory(tenant, property)
    };
  });
}

function getApplicantStory(tenant, property) {
  const stories = {
    family: "We need a calm home near school and can move in this week.",
    student: "I am starting a new semester. Could the rent stay fixed for a year?",
    professional: "I am moving for a new role and can sign a longer lease.",
    "remote-workers": "We both work from home and would value a reliable space.",
    problematic: "I can pay more, but I have had disagreements with previous landlords.",
    health: "We want a visible neighborhood location for a local pharmacy.",
    trade: "We are a small local grocery looking for a steady long-term lease.",
    gastronomy: "We need a street-facing spot and can open quickly.",
    beauty: "Our studio has loyal clients and wants to join this district.",
    education: "We want a friendly space for evening classes.",
    offices: "Our growing team needs a practical base near the city.",
    sport: "We would bring new daily foot traffic to the block.",
    entertainment: "We can pay a premium for a lively evening location.",
    repairs: "We need a compact workshop close to our customers.",
    logistics: "We can operate a convenient pickup point for the district.",
    tourism: "We need a central desk for visitors during busy season.",
    "public services": "We would like to provide a practical local service point."
  };

  return stories[tenant.category] || `We would like to rent ${property.name}.`;
}

function getPropertyStage(propertyId) {
  return state.propertyStages[propertyId] || { stage: getTenant(propertyId) ? "occupied" : "ready-advertise" };
}

function getAdvertisement(propertyId) {
  return state.advertisements[propertyId] || null;
}

function isNotaryPending(propertyId) {
  return getPropertyStage(propertyId).stage === "notary";
}

function canReviewApplications(propertyId) {
  const advertisement = getAdvertisement(propertyId);
  return getPropertyStage(propertyId).stage === "advertising" && Boolean(advertisement?.applications?.length);
}

function getRenovationCost(property) {
  return Math.max(900, Math.round(property.price * 0.055));
}

function getSaturationPenalty(property, category) {
  const sameCategoryNearby = Object.entries(state.tenants).filter(([propertyId, tenant]) => {
    const otherProperty = properties.find((item) => item.id === propertyId);
    return otherProperty
      && otherProperty.district === property.district
      && tenant.category === category;
  }).length;

  return Math.min(45, sameCategoryNearby * 14);
}

function getEvictionChance(evictionCase) {
  return Math.min(88, 12 + evictionCase.pressure + (5 - evictionCase.daysRemaining) * 4);
}

function deterministicRoll(seed) {
  let hash = 0;

  for (let index = 0; index < seed.length; index += 1) {
    hash = (hash * 31 + seed.charCodeAt(index)) % 9973;
  }

  return hash % 100;
}

function getMaintenanceCost(property) {
  const missingCondition = 100 - getCondition(property.id);
  return Math.max(750, Math.round(property.price * (0.025 + missingCondition / 1000)));
}

function getSuggestedSalePrice(property) {
  const conditionMultiplier = 0.82 + getCondition(property.id) / 250 + (getPropertyLevel(property.id) - 1) * 0.05;
  return Math.round(property.price * conditionMultiplier / 500) * 500;
}

function getUpgradeCost(property, count) {
  return Math.round((property.price * 0.08 + getPropertyLevel(property.id) * property.tier * 420) * count);
}

function getInfluenceBoostCost(property) {
  return Math.max(6, property.tier * 5 + Math.floor(getPropertyLevel(property.id) / 2));
}

function addXp(amount) {
  state.xp += amount;

  while (state.xp >= getXpForNextLevel(state.playerLevel)) {
    state.xp -= getXpForNextLevel(state.playerLevel);
    state.playerLevel += 1;
    const reward = getLevelReward(state.playerLevel);
    state.cash += reward;
    state.influence += 10;
    state.pendingLevel = state.playerLevel;
  }
}

function getXpForNextLevel(level) {
  return 60 + level * 30;
}

function getLevelReward(level) {
  return level * 1250;
}

function getGoals() {
  return [
    {
      id: "first-building",
      title: "Own 1 building",
      complete: state.owned.length >= 1,
      rewardCash: 2500,
      rewardInfluence: 5
    },
    {
      id: "level-three",
      title: "Upgrade a building to Lvl 3",
      complete: Object.values(state.propertyLevels).some((level) => Number(level) >= 3),
      rewardCash: 4000,
      rewardInfluence: 8
    },
    {
      id: "market-listing",
      title: "Place a property on sale",
      complete: Object.keys(state.listedForSale).length > 0,
      rewardCash: 3000,
      rewardInfluence: 6
    }
  ].map((goal) => ({
    ...goal,
    claimed: state.claimedGoals.includes(goal.id)
  })).filter((goal) => !goal.claimed);
}

function claimGoal(goalId) {
  const goal = getGoals().find((item) => item.id === goalId);

  if (!goal || !goal.complete || goal.claimed) {
    return;
  }

  state.cash += goal.rewardCash;
  state.influence += goal.rewardInfluence;
  state.claimedGoals.push(goal.id);
  addXp(20);
  pushToast(`Goal claimed: ${formatMoney(goal.rewardCash)} + ${goal.rewardInfluence} influence.`);
  saveState();
  render();
}

function trackMicroWin(id, announce = true) {
  if (state.microWins.includes(id)) {
    return;
  }

  state.microWins.push(id);

  if (announce) {
    const step = firstHourSteps.find((item) => item.id === id);
    pushCeremony(step ? step.win : "Micro win unlocked");
  }
}

function pushToast(message) {
  state.toast = message;
  window.clearTimeout(pushToast.timeoutId);
  pushToast.timeoutId = window.setTimeout(() => {
    state.toast = "";
    saveState();
    render();
  }, 2600);
}

function pushCeremony(message) {
  state.ceremony = message;
  window.clearTimeout(pushCeremony.timeoutId);
  pushCeremony.timeoutId = window.setTimeout(() => {
    state.ceremony = "";
    saveState();
    render();
  }, 1800);
}

function getConditionLabel(condition) {
  if (condition >= 90) {
    return "Excellent condition. Tenants pay close to full rent and resale value is strong.";
  }

  if (condition >= 70) {
    return "Stable condition. A maintenance visit can improve rent and resale value.";
  }

  return "Needs attention. Maintenance is recommended before relying on this building.";
}

function formatMoney(value) {
  return new Intl.NumberFormat("pl-PL", {
    style: "currency",
    currency: "PLN",
    maximumFractionDigits: 0
  }).format(value);
}

function loadState() {
  const raw = localStorage.getItem("district-empire-offline-state");

  if (!raw) {
    return createDefaultState();
  }

  try {
    const parsedState = JSON.parse(raw);
    const hasAccruedRent = Object.prototype.hasOwnProperty.call(parsedState, "rentAccrued");
    const hasTutorialState = Object.prototype.hasOwnProperty.call(parsedState, "tutorialState");
    return normalizeState({
      ...defaultState,
      ...parsedState,
      rentAccruedMigrated: hasAccruedRent ? parsedState.rentAccruedMigrated ?? true : false,
      onboardingComplete: hasTutorialState ? parsedState.onboardingComplete : true,
      tutorialState: hasTutorialState ? parsedState.tutorialState : { currentStep: "complete", completedSteps: [], skipped: true }
    }, false);
  } catch {
    return createDefaultState();
  }
}

function saveState() {
  localStorage.setItem("district-empire-offline-state", JSON.stringify(state));
}

function createDefaultState() {
  return {
    ...defaultState,
    tutorialState: {
      currentStep: "welcome",
      completedSteps: [],
      skipped: false
    },
    owned: [],
    conditions: {},
    propertyLevels: {},
    tenants: {},
    propertyStages: {},
    advertisements: {},
    tenantMemories: {},
    latePayments: {},
    evictionCases: {},
    listedForSale: {},
    rentAccrued: {
      residential: 0,
      commercial: 0
    },
    rentAccruedMigrated: true,
    claimedGoals: [],
    microWins: [],
    dailyMomentum: [],
    cityEvents: [],
    streetIssuesResolved: [],
    repaired: []
  };
}

function normalizeState(nextState = state, assign = true) {
  nextState.owned = Array.isArray(nextState.owned) ? nextState.owned : [];
  nextState.repaired = Array.isArray(nextState.repaired) ? nextState.repaired : [];
  nextState.claimedGoals = Array.isArray(nextState.claimedGoals) ? nextState.claimedGoals : [];
  nextState.microWins = Array.isArray(nextState.microWins) ? nextState.microWins : [];
  nextState.dailyMomentum = Array.isArray(nextState.dailyMomentum) ? nextState.dailyMomentum : [];
  nextState.cityEvents = Array.isArray(nextState.cityEvents) ? nextState.cityEvents : [];
  nextState.streetIssuesResolved = Array.isArray(nextState.streetIssuesResolved) ? nextState.streetIssuesResolved : [];
  nextState.conditions = nextState.conditions && typeof nextState.conditions === "object" ? nextState.conditions : {};
  nextState.propertyLevels = nextState.propertyLevels && typeof nextState.propertyLevels === "object" ? nextState.propertyLevels : {};
  nextState.tenants = nextState.tenants && typeof nextState.tenants === "object" ? nextState.tenants : {};
  nextState.propertyStages = nextState.propertyStages && typeof nextState.propertyStages === "object" ? nextState.propertyStages : {};
  nextState.advertisements = nextState.advertisements && typeof nextState.advertisements === "object" ? nextState.advertisements : {};
  nextState.tenantMemories = nextState.tenantMemories && typeof nextState.tenantMemories === "object" ? nextState.tenantMemories : {};
  nextState.latePayments = nextState.latePayments && typeof nextState.latePayments === "object" ? nextState.latePayments : {};
  nextState.evictionCases = nextState.evictionCases && typeof nextState.evictionCases === "object" ? nextState.evictionCases : {};
  nextState.listedForSale = nextState.listedForSale && typeof nextState.listedForSale === "object" ? nextState.listedForSale : {};
  nextState.rentAccrued = nextState.rentAccrued && typeof nextState.rentAccrued === "object" ? nextState.rentAccrued : {};
  nextState.rentAccrued.residential = Math.max(0, Math.round(Number(nextState.rentAccrued.residential || 0)));
  nextState.rentAccrued.commercial = Math.max(0, Math.round(Number(nextState.rentAccrued.commercial || 0)));
  nextState.rentAccruedMigrated = Boolean(nextState.rentAccruedMigrated);
  nextState.infoPropertyId = nextState.infoPropertyId || nextState.selectedId || defaultState.selectedId;
  nextState.mapSheetOpen = Boolean(nextState.mapSheetOpen);
  nextState.sheetTab = ["overview", "rent", "repairs"].includes(nextState.sheetTab) ? nextState.sheetTab : defaultState.sheetTab;
  nextState.influence = Number.isFinite(Number(nextState.influence)) ? Number(nextState.influence) : defaultState.influence;
  nextState.xp = Number.isFinite(Number(nextState.xp)) ? Number(nextState.xp) : defaultState.xp;
  nextState.playerLevel = Number.isFinite(Number(nextState.playerLevel)) ? Number(nextState.playerLevel) : defaultState.playerLevel;
  nextState.pendingLevel = nextState.pendingLevel || null;
  nextState.toast = nextState.toast || "";
  nextState.ceremony = nextState.ceremony || "";
  nextState.onboardingComplete = Boolean(nextState.onboardingComplete);
  nextState.tutorialState = nextState.tutorialState && typeof nextState.tutorialState === "object" ? nextState.tutorialState : { currentStep: "complete", completedSteps: [], skipped: true };
  nextState.tutorialState.currentStep = nextState.tutorialState.currentStep || "welcome";
  nextState.tutorialState.completedSteps = Array.isArray(nextState.tutorialState.completedSteps) ? nextState.tutorialState.completedSteps : [];
  nextState.tutorialState.skipped = Boolean(nextState.tutorialState.skipped);
  nextState.dailyRewardClaimedDay = Number.isFinite(Number(nextState.dailyRewardClaimedDay)) ? Number(nextState.dailyRewardClaimedDay) : 0;

  Object.keys(nextState.listedForSale).forEach((propertyId) => {
    if (!nextState.owned.includes(propertyId) || nextState.tenants[propertyId] || nextState.evictionCases[propertyId]) {
      delete nextState.listedForSale[propertyId];
    }
  });

  Object.keys(nextState.tenants).forEach((propertyId) => {
    if (!nextState.owned.includes(propertyId)) {
      delete nextState.tenants[propertyId];
    }
  });

  nextState.owned.forEach((propertyId) => {
    if (!nextState.propertyStages[propertyId]) {
      nextState.propertyStages[propertyId] = { stage: nextState.tenants[propertyId] ? "occupied" : "ready-advertise" };
    }
  });

  Object.keys(nextState.advertisements).forEach((propertyId) => {
    if (!nextState.owned.includes(propertyId)) {
      delete nextState.advertisements[propertyId];
      return;
    }
    const advertisement = nextState.advertisements[propertyId];
    advertisement.views = Math.max(0, Number(advertisement.views || 0));
    advertisement.visits = Math.max(0, Number(advertisement.visits || 0));
    advertisement.applications = Array.isArray(advertisement.applications) ? advertisement.applications : [];
    advertisement.negotiated = advertisement.negotiated && typeof advertisement.negotiated === "object" ? advertisement.negotiated : {};
  });

  Object.keys(nextState.latePayments).forEach((propertyId) => {
    if (!nextState.owned.includes(propertyId) || !nextState.tenants[propertyId]) {
      delete nextState.latePayments[propertyId];
    }
  });

  Object.keys(nextState.evictionCases).forEach((propertyId) => {
    if (!nextState.owned.includes(propertyId)) {
      delete nextState.evictionCases[propertyId];
    }
  });

  if (!nextState.rentAccruedMigrated) {
    properties.forEach((property) => {
      const tenant = nextState.tenants[property.id];
      const blocked = nextState.evictionCases[property.id] || nextState.listedForSale[property.id];

      if (nextState.owned.includes(property.id) && tenant && !blocked) {
        const bucket = property.useType === "commercial" ? "commercial" : "residential";
        nextState.rentAccrued[bucket] += Math.max(0, Math.round(Number(tenant.rent || 0)));
      }
    });
    nextState.rentAccruedMigrated = true;
  }

  if (assign) {
    state = nextState;
  }

  return nextState;
}
