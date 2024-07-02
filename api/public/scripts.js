async function getTrashes() {
  let data = await fetch('/trash')
    .then(response => response.json())
    .catch(error => {
      console.error('Erreur:', error);
    });
  

    Object.keys(data).forEach(key => {
        // Pour "pleine/vide"
        let fullCircle = document.createElement('div');
        fullCircle.style.width = '20px';
        fullCircle.style.height = '20px';
        fullCircle.style.borderRadius = '50%';
        fullCircle.style.display = 'inline-block';
        fullCircle.style.backgroundColor = data[key].full ? 'blue' : 'red';
        document.getElementById('trash-' + key + '-full').innerHTML = ''; // Nettoyer l'élément
        document.getElementById('trash-' + key + '-full').appendChild(fullCircle);
        document.getElementById('trash-' + key + '-open').innerHTML = data[key].open ? "ouverte" : "fermée";
    });
}

function setOpen(id, bool) {
  let route = '/trash/' + id + (bool ? '/open' : '/close');
  fetch(route, {method: 'POST'})
    .then(response => response.json())
    .catch(error => {
      console.error('Erreur:', error);
    });
}

function setFull(id, bool) {
  let route = '/trash/' + id + (bool ? '/full' : '/empty');
  fetch(route, {method: 'POST'})
    .then(response => response.json())
    .catch(error => {
      console.error('Erreur:', error);
    });
}

getTrashes();

setInterval(getTrashes, 1000);
