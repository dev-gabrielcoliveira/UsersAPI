# FCG.UsersAPI

Microsserviço responsável pelo gerenciamento de usuários da plataforma FIAP Cloud Games (FCG).

## Sobre o projeto

O FCG.UsersAPI é responsável pelo cadastro, autenticação e autorização dos usuários da plataforma.

O serviço foi desenvolvido seguindo uma arquitetura de microsserviços, permitindo evolução independente dos componentes e comunicação assíncrona através de eventos.

## Responsabilidades

- Cadastro de usuários
- Consulta de usuários
- Autenticação utilizando JWT
- Autorização baseada em perfil
- Publicação de eventos de domínio

## Tecnologias utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- MassTransit
- RabbitMQ
- Docker
- Kubernetes

## Arquitetura

O projeto possui separação de responsabilidades:

- **API**
  - Controllers
  - Endpoints HTTP

- **Application**
  - Casos de uso
  - Serviços da aplicação
  - Eventos

- **Domain**
  - Entidades
  - Regras de negócio

- **Infrastructure**
  - Persistência
  - Repositórios
  - Configurações externas

## Mensageria

Após o cadastro de um usuário, o serviço publica o evento:

Fluxo:

UsersAPI
|
| UserCreatedEvent
↓
RabbitMQ
↓
NotificationsAPI

O evento é consumido pelo NotificationsAPI para simular o envio de e-mail de boas-vindas ao usuário cadastrado.

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

Os manifestos Kubernetes estão disponíveis na pasta: