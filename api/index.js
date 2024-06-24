const express = require('express')
const app = express()
const port = 3000

app.set('view engine', 'ejs');

let trashes = {
  1: {"open": false, "full": false},
  2: {"open": false, "full": false},
  3: {"open": false, "full": false},
};

app.use(express.json());
app.use(express.urlencoded({extended: true}));

function adminPage(res) {
  res.render('admin', {trashes});
}

app.get('/admin', (req, res) => {
  adminPage(res);
});

app.get('/trash', (req, res) => {
  res.json(trashes)
})

app.get('/trash/:id', (req, res) => {
  res.json(trashes[req.params.id])
})

app.post('/trash/:id/open', (req, res) => {
  trashes[req.params.id].open = true
  if (req.body.redirect) {
    return adminPage(res);
  }
  res.json(trashes[req.params.id])
})

app.post('/trash/:id/close', (req, res) => {
  trashes[req.params.id].open = false
  if (req.body.redirect) {
    return adminPage(res);
  }
  res.json(trashes[req.params.id])
})

app.post('/trash/:id/full', (req, res) => {
  trashes[req.params.id].full = true
  if (req.body.redirect) {
    return adminPage(res);
  }
  res.json(trashes[req.params.id])
})

app.post('/trash/:id/empty', (req, res) => {
  trashes[req.params.id].full = false
  if (req.body.redirect) {
    return adminPage(res);
  }
  res.json(trashes[req.params.id])
})

app.listen(port, () => {
  console.log(`Example app listening on port ${port}`)
})

