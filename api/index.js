const express = require('express')
const app = express()
const port = 3000

let trashes = {
  1: {"open": false, "full": false},
  2: {"open": false, "full": false},
  3: {"open": false, "full": false},
};

app.use(express.json());
app.use(express.urlencoded({extended: true}));

app.get('/trash', (req, res) => {
  res.json(trashes)
})

app.get('/trash/:id', (req, res) => {
  res.json(trashes[req.params.id])
})

app.put('/trash/:id/open', (req, res) => {
  trashes[req.params.id].open = true
  res.json(trashes[req.params.id])
})

app.put('/trash/:id/close', (req, res) => {
  trashes[req.params.id].open = false
  res.json(trashes[req.params.id])
})

app.put('/trash/:id/full', (req, res) => {
  trashes[req.params.id].full = true
  res.json(trashes[req.params.id])
})

app.put('/trash/:id/empty', (req, res) => {
  trashes[req.params.id].full = false
  res.json(trashes[req.params.id])
})

app.listen(port, () => {
  console.log(`Example app listening on port ${port}`)
})

