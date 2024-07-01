async function getTrashes() {
  let data = await fetch('trash')
    .then(response => response.json())
    .catch(error => {
      console.error('Erreur:', error);
    });

  Object.keys(data).forEach(key => {
    document.getElementById('trash-' + key + '-open').innerHTML = data[key].open ? "ouverte" : "fermée";
    document.getElementById('trash-' + key + '-full').innerHTML = data[key].full ? "pleine" : "vide";
  })
}

function setOpen(id, bool) {
  let route = 'trash/' + id + (bool ? '/open' : '/close');
  fetch(route, {method: 'POST'})
    .then(response => response.json())
    .catch(error => {
      console.error('Erreur:', error);
    });
}

function setFull(id, bool) {
  let route = 'trash/' + id + (bool ? '/full' : '/empty');
  fetch(route, {method: 'POST'})
    .then(response => response.json())
    .catch(error => {
      console.error('Erreur:', error);
    });
}

getTrashes();

setInterval(getTrashes, 1000);
