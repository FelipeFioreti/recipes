# Recipes Web

Frontend Angular standalone para o backend `Recipes.Api`.

## Stack

- Angular 18
- Standalone components
- Lazy loading por rota
- Signals para estado de sessao
- SCSS com tema editorial responsivo

## Scripts

- `npm install`
- `npm start`
- `npm run build`

## Estrutura

- `src/app/core`: autenticacao, guards, interceptors, modelos e servicos globais
- `src/app/layout`: cascas da aplicacao autenticada e telas publicas
- `src/app/shared`: componentes reutilizaveis
- `src/app/features`: dominios da interface (`auth`, `home`, `recipes`, `recipe-types`)

## Integracao local

O `proxy.conf.json` encaminha chamadas `/api` para `https://localhost:7037`. Ajuste o alvo se o backend estiver exposto
em outra porta.
