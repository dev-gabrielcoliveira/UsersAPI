# UsersAPI

> Microsserviço responsável pelo gerenciamento de usuários, autenticação e controle de perfis da plataforma FIAP Cloud Games (FCG).

---

## 💡 Sobre o projeto

O **UsersAPI** é o microsserviço centralizador de identidade da plataforma FIAP Cloud Games (FCG). 

Ele responde pelo cadastro de novos jogadores, autenticação segura (JWT), gestão de perfil e disponibilização de dados cadastrais para os demais microsserviços do ecossistema.

---

## 🎯 Responsabilidades

- **Gestão de Usuários:** Cadastro, atualização de dados cadastrais e consulta de perfis.
- **Autenticação e Autorização:** Autenticação via JWT (JSON Web Tokens) e controle de permissões.
- **Eventos de Domínio:** Emissão de eventos relacionados ao ciclo de vida do usuário (ex: cadastro realizado).

---

## 🛠️ Tecnologias Utilizadas

- **.NET 8** (ASP.NET Core Web API)
- **Entity Framework Core** & **SQL Server**
- **ASP.NET Core Identity** / **JWT** (Autenticação)
- **MassTransit** & **RabbitMQ**
- **Docker** & **Kubernetes**
- **Serilog** (Logs estruturados)

---

## 🏗️ Arquitetura Interna

O projeto adota Clean Architecture com separação clara de responsabilidades:

- **API:** Controllers, endpoints de autenticação/cadastro e middlewares de autorização.
- **Application:** Casos de uso, DTOs, validações e serviços de token.
- **Domain:** Entidades de usuário, regras de negócio e contratos de eventos.
- **Infrastructure:** Persistência de dados (EF Core), configuração do Identity e integrações externas.

## Banco de Dados

O serviço utiliza:

A persistência é realizada utilizando Entity Framework Core e migrations para controle da evolução do banco.

As informações sensíveis, como connection strings e chaves privadas, são armazenadas utilizando Kubernetes Secrets.

## Docker

O projeto possui Dockerfile utilizando multi-stage build.

O processo utiliza duas etapas:

1. Build da aplicação utilizando o SDK do .NET.
2. Execução utilizando uma imagem menor contendo apenas o runtime necessário.

Benefícios:

- Menor tamanho da imagem final.
- Melhor segurança.
- Separação entre ambiente de desenvolvimento e produção.

## Kubernetes

Os manifestos Kubernetes estão disponíveis na pasta: k8s

## Observabilidade

A aplicação utiliza Serilog para geração de logs estruturados em console.

Em ambiente Kubernetes os logs podem ser acompanhados utilizando os recursos nativos do cluster. E é Monitorada através do Grafana e do Prometheus

## Objetivo do serviço

A UsersAPI representa o microsserviço responsável pela manutenção do cadastro e autenticação de usuários. Ao cadastrar um novo usuário, o RabbitMQ foi substituído por uma fila no Azure Storage Queue que aciona uma Azure Function serverless para o envio de notificações, garantindo menor consumo de recursos e evitando pods ociosos na infraestrutura
