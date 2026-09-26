# Arquitetura do ChefArchive

Documentação centralizada de como o **ChefArchive inteiro** funciona — os 3 repositórios, a infraestrutura na VPS, e como deploy e roteamento funcionam de ponta a ponta. Mantida aqui, no `recipes`, por ser o repositório público mais estável dos três (`chefarchive-infra`, onde a infra em si vive, é privado).

## Os repositórios

| Repositório | Visibilidade | Conteúdo | Publica no GHCR |
|---|---|---|---|
| [`recipes`](https://github.com/FelipeFioreti/recipes) (este) | público | Backend ASP.NET Core, Clean Architecture | `chefarchive-api`, `chefarchive-api-migrations` |
| [`chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web) | público | Frontend Angular | `chefarchive-web` |
| [`chefarchive-infra`](https://github.com/FelipeFioreti/chefarchive-infra) | **privado** | Proxy de borda (nginx-proxy + acme-companion), redes Docker compartilhadas, runbook operacional | usa imagens de terceiros prontas — não builda nada próprio |

Cada repositório builda e deploya sua própria parte de forma independente. Não existe um "deploy do ChefArchive" único que sobe tudo junto.

## Visão geral

```
Internet
   │  https://chefarchive.felipe-fioreti.tech
   ▼
┌──────────────────────────────────────────┐
│  nginx-proxy + acme-companion             │  projeto "chefarchive-proxy"
│  (repo chefarchive-infra)                 │  único ponto que publica portas
│  TLS automático, roteamento por container │  80/443 no host
└───────────┬────────────────┬──────────────┘
      /  ───┘                └───  /api/
            │                      │
            ▼                      ▼
    ┌───────────────┐      ┌───────────────┐
    │  web           │      │  api           │  projeto "chefarchive-api"
    │  (Angular)     │      │  (ASP.NET      │  (repo recipes, este)
    │  projeto       │      │   Core)        │
    │ "chefarchive-  │      └───────┬────────┘
    │  web"          │              │
    └───────────────┘              ▼
                             Neon Postgres
                             (gerenciado, externo à VPS)
```

## Os 3 projetos Docker Compose na VPS

Cada serviço roda como um projeto Docker Compose **independente** na VPS:

| Projeto na VPS | Deployado por | Serviços |
|---|---|---|
| `chefarchive-proxy` | `chefarchive-infra` | `nginx-proxy`, `acme-companion` |
| `chefarchive-api` | `recipes` (este repo) | `api`, `migration` |
| `chefarchive-web` | `chefarchive-web` | `web` |

Atualizar um projeto não recria os outros — um deploy da API não derruba o frontend nem o proxy, e vice-versa.

## Redes Docker compartilhadas

Mesmo sendo projetos separados, os containers se enxergam pelo nome porque compartilham 3 redes Docker, cada uma declarada ("dona") por um dos projetos e referenciada como `external: true` pelos outros:

| Rede | Dona | Quem usa | `internal: true`? | Por quê existe |
|---|---|---|---|---|
| `chefarchive_frontend` | `chefarchive-infra` | `nginx-proxy`, `acme-companion` | Não | Precisa de saída pra internet — publicar portas no host e falar com a Let's Encrypt |
| `chefarchive_backend` | `chefarchive-infra` | `nginx-proxy`, `api`, `web` | **Sim** | Isolamento deliberado: `api` e `web` não têm rota direta pra internet, só o proxy os alcança |
| `chefarchive_egress` | `recipes` (projeto `chefarchive-api`) | `api`, `migration` | Não | `api`/`migration` precisam de saída pra internet pra alcançar o banco (Neon Postgres, externo à VPS) — `backend` sozinha não bastaria, já que é `internal: true` |

## Roteamento e TLS

O `nginx-proxy` não usa um arquivo `.conf` escrito à mão — ele lê variáveis de ambiente dos outros containers via socket do Docker e gera a configuração sozinho:

- `api` declara `VIRTUAL_HOST=chefarchive.felipe-fioreti.tech` + `VIRTUAL_PATH=/api/` → responde só por esse caminho.
- `web` declara o mesmo `VIRTUAL_HOST` (sem `VIRTUAL_PATH`) → responde por tudo que sobrar (`/`).
- `web` também declara `LETSENCRYPT_HOST` → é o gatilho para o `acme-companion` emitir e renovar o certificado TLS automaticamente. Sem certbot, sem cron/systemd timer, sem hook manual no host.

Detalhes operacionais completos (como adicionar uma aplicação nova ao proxy, variáveis por caso de uso, diagnóstico de problemas) estão em `docs/nginx.md`, no `chefarchive-infra` (privado).

## Ciclo de deploy

Build de imagem e deploy em produção são coisas **separadas**, disparadas em momentos diferentes:

1. **As mudanças se juntam numa `release/X.Y.Z`** → cada branch de trabalho abre PR para a `release/*` da versão em que vai sair (nunca direto para a `main`). PRs e merges rodam só CI, nunca deploy.
2. **Você cria e envia uma tag de versão anotada no último commit da `release/*`** (`git tag -a v1.2.3 -m "Release 1.2.3" && git push origin v1.2.3`) → dispara o build, publicando `latest` e `1.2.3` no GHCR.
3. Essa mesma tag dispara o job `deploy` do workflow (só roda em tags `v*`), que chama a [Action da Hostinger](https://github.com/hostinger/deploy-on-vps) passando a versão exata recém-publicada.
4. A API da Hostinger recria **só o projeto correspondente** na VPS (`chefarchive-api`, `chefarchive-web` ou `chefarchive-proxy`, dependendo de qual repositório foi tagueado), puxando a imagem já publicada.
5. **Deploy com sucesso → PR da `release/*` para a `main`.** A `main` só recebe versões que já estão em produção, então sempre tem o mesmo código que está no ar. Em `recipes` e `chefarchive-web`, esse push na `main` builda de novo e publica uma imagem só com a tag `sha-<hash>`, sem mover `latest` e sem deploy.

Ou seja: **só a tag leva uma versão para produção.** Rollback é refazer o deploy da tag anterior; uma correção vira uma nova tag (`v1.2.4`), nunca uma tag movida ou recriada. O fluxo completo (numeração, hotfix, sincronização entre releases) está em `docs/git-best-practices.md`, no `chefarchive-infra` (privado).

### Como confirmar o que está rodando de verdade

Toda imagem publicada carrega o hash do commit de origem dentro de si mesma — um label OCI gravado automaticamente pelo `docker/metadata-action` no build:

```bash
docker pull ghcr.io/felipefioreti/chefarchive-api:1.2.3
docker inspect ghcr.io/felipefioreti/chefarchive-api:1.2.3 \
  --format '{{index .Config.Labels "org.opencontainers.image.revision"}}'
```

Compare o hash retornado com o commit que a tag `v1.2.3` aponta no GitHub. Se baterem, é prova de que aquele commit exato é o que está publicado — e, junto com a variável `API_TAG`/`WEB_TAG`/`PROXY_TAG` do projeto na VPS, o que está rodando em produção agora.

## Segredos — onde cada um mora

| Segredo | Fica em (GitHub Secrets) | Usado por |
|---|---|---|
| `DATABASE_CONNECTION_STRING`, `JWT_SECRET`, `ASPNETCORE_ENVIRONMENT` | `recipes` | Deploy da API |
| `LETSENCRYPT_EMAIL` | `chefarchive-infra` | Deploy do proxy |
| `HOSTINGER_API_KEY` | Nos 3 repositórios | Todo deploy — autentica na API da Hostinger |

Nenhum segredo entra em imagem Docker, código-fonte ou log — todos são injetados como variável de ambiente só no momento em que o container sobe na VPS.

## Decisões de arquitetura e por quê

- **3 repositórios em vez de 1**: deploy e evolução independentes de backend, frontend e infra; mantém segredos/config de infra fora dos repositórios públicos de aplicação.
- **3 projetos Compose independentes na VPS**: atualizar um serviço não derruba os outros.
- **Imagens públicas no GHCR** (mesmo com `chefarchive-infra` privado): o código do backend/frontend já é público, então a imagem compilada não expõe nada novo — e simplifica o deploy, sem autenticação de registry na VPS.
- **`chefarchive_backend` como `internal: true`**: API e frontend não são alcançáveis diretamente da internet, só através do proxy.
- **nginx-proxy + acme-companion em vez de nginx manual + certbot**: elimina a classe de bug em que um script de renovação de certificado fica com o nome de container desatualizado — isso já aconteceu de verdade e quase causou uma queda por certificado expirado.
- **Deploy só em tag, nunca em todo push**: separa "publiquei uma imagem" de "isso está em produção" — dá controle explícito sobre o que vai pro ar e quando.

## Repositórios relacionados

- [`FelipeFioreti/chefarchive-web`](https://github.com/FelipeFioreti/chefarchive-web) — frontend
- [`FelipeFioreti/chefarchive-infra`](https://github.com/FelipeFioreti/chefarchive-infra) — proxy de borda, redes, runbook operacional (privado)

## Mantendo esta doc atualizada

Este arquivo cobre os **3 repositórios**, não só este. A regra em `CLAUDE.md` (aqui e nos outros dois repositórios) diz: qualquer mudança que altere como o sistema é deployado, roteado, ou como os serviços se conectam entre si — rede, segredo, banco, proxy, workflow de deploy, imagem publicada — deve atualizar este arquivo como parte do mesmo PR, esteja a mudança em `recipes`, `chefarchive-web` ou `chefarchive-infra`.
