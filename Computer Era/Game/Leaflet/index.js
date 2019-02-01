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
    bucketIcon = new BuildingIcon({ iconUrl: 'images/bucket.png' }),
    filmProjectorIcon = new BuildingIcon({ iconUrl: 'images/film-projector.png' }), 
    jerrycanIcon = new BuildingIcon({ iconUrl: 'images/jerrycan.png' }),
    weightLiftingIcon = new BuildingIcon({ iconUrl: 'images/weight-lifting.png' }),
    compactDiscIcon = new BuildingIcon({ iconUrl: 'images/compact-disc.png' }),
    carIcon = new BuildingIcon({ iconUrl: 'images/car.png' }),
    houseIcon = new BuildingIcon({ iconUrl: 'images/house.png' }),
    modernCityIcon = new BuildingIcon({ iconUrl: 'images/modern-city.png' });

function ReadState(options) {
    window.external.ReadState(options);
}; 