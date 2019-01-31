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
    microchipIcon = new BuildingIcon({ iconUrl: 'images/microchip.png' }),
    pcIcon = new BuildingIcon({ iconUrl: 'images/pc.png' }),
    coffeeCupIcon = new BuildingIcon({ iconUrl: 'images/coffee-cup.png' }),
    milkCartonIcon = new BuildingIcon({ iconUrl: 'images/milk-carton.png' }),
    wheelbarrowIcon = new BuildingIcon({ iconUrl: 'images/wheelbarrow.png' }),
    martiniIcon = new BuildingIcon({ iconUrl: 'images/martini.png' }),
    bucketIcon = new BuildingIcon({ iconUrl: 'images/bucket.png' });

function ReadState(options) {
    window.external.ReadState(options);
};