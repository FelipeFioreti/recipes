# Recipes (ChefArchive API)

Backend ASP.NET Core do ChefArchive, em Clean Architecture. Ver `README.md` para a estrutura de camadas, como rodar localmente e como o deploy deste repositório funciona.

## Documentação centralizada do ChefArchive

Este repositório mantém `ARCHITECTURE.md` — a documentação de arquitetura de **todo** o ChefArchive (os 3 repositórios: `recipes`, `chefarchive-web`, `chefarchive-infra`), não só deste.

**Regra:** sempre que uma mudança aqui alterar como o sistema funciona ou é deployado — rede Docker, segredo, workflow de deploy/publicação de imagem, integração com outro repositório do ChefArchive, banco de dados — atualize `ARCHITECTURE.md` na mesma branch de trabalho. Não deixe a documentação centralizada dessincronizar do código.

Mudanças que só afetam a lógica interna da aplicação (uma regra de negócio, um endpoint novo que não muda a arquitetura) não precisam tocar em `ARCHITECTURE.md`.

Antes de qualquer commit, branch, merge ou PR: siga as convenções descritas no `docs/git-best-practices.md` do `chefarchive-infra` (privado) — commits em português, Conventional Commits, sem assinatura de ferramenta. Branches de trabalho partem da `main` e entram na `release/X.Y.Z` da versão por merge local (`--no-ff`, sem PR), nunca direto na `main`. O único PR é o da release para a `main`, após o deploy; a release é mantida depois do merge. Criar a tag `vX.Y.Z` dispara o deploy: nunca crie nem envie tags sem pedido explícito.
