var BuildingIcon = L.Icon.extend({
    options: {
        iconSize: [25, 41],
        iconAnchor: [11, 41],
        popupAnchor: [1, -45]
    }
});

var bankIcon = new BuildingIcon({ iconUrl: 'images/bank.png' }),
    evilTowerIcon = new BuildingIcon({ iconUrl: 'images/evil-tower.png' }),
    marketIcon = new BuildingIcon({ iconUrl: 'images/market.png' }),
    microchipIcon = new BuildingIcon({ iconUrl: 'images/microchip.png' });

function ReadState(options) {
    window.external.ReadState(options);
};