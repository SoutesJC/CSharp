# 🎵 Studio Schedule – Sistema de Agenda para Estúdios de Gravação

Aplicação em **C# (.NET)** que modela o domínio de um **estúdio de gravação**, permitindo o gerenciamento de salas, músicos e sessões de gravação com regras de negócio avançadas:

- Controle de conflitos de horário  
- Garantia de músicos sem duplicidade  
- Intervalos de data como Value Objects  
- Entidades com GUID  
- Imutabilidade e invariantes protegidos  

Este projeto demonstra boas práticas de **DDD (Domain-Driven Design)**, uso de **Value Objects**, **NRT (Nullable Reference Types)** e modelagem orientada a domínio.

---

## 📦 Tecnologias Utilizadas

- **.NET 7 / .NET 8**
- **C# 11**
- Programação orientada a objetos  
- Domain-Driven Design  
- Exceções personalizadas  
- Value Objects imutáveis

---

## 📁 Estrutura do Projeto
```bash
src/
├── Entities/
│ ├── Room.cs
│ ├── Musician.cs
│ ├── Session.cs
├── ValueObjects/
│ ├── DateRange.cs
│ └── UnionCard.cs
├── Schedule.cs
├── Exceptions.cs
├── Test.cs
└── Program.cs
```
### 🏢 Room
Representa uma sala de gravação.  
Possui:
- `Guid Id`
- `string Name`

### 🎤 Musician
Representa um músico que participa das sessões.  
Pode ter:
- `UnionCard` (carteira sindical)
- `FullName`

### 📅 DateRange (Value Object)
Intervalo de tempo imutável com:
- `Start`
- `End`
- Validação automática (Start < End)

### 🎬 Session
Uma sessão de gravação contém:
- Sala (`Room`)
- Intervalo (`DateRange`)
- Participantes (`List<Musician>`)
- Regras contra músicos duplicados

### 📘 Schedule
Uma agenda geral do estúdio, com:
- Lista de sessões
- Verificação de conflitos entre horários
- Sessões somente leitura (`IReadOnlyList`)

---

## 🚀 Como Executar o Projeto

### 1. Clonar o repositório
```bash
git clone https://github.com/seuusuario/studio-schedule.git
cd studio-schedule
```
## 🧱 Como Criar as Entidades do Domínio

A seguir estão instruções simples e diretas para criar cada entidade do sistema dentro do `Program.cs` ou em qualquer outro ponto da aplicação.

---

### 🏢 Criar uma Sala (Room)

```
var salaA = new Room(Guid.NewGuid(), "Sala A");
```
### 🎤 Criar um Músico (Musician)
```
var musico1 = new Musician(Guid.NewGuid(), "Carlos Silva", new UnionCard("12345"));

var musico2 = new Musician(Guid.NewGuid(), "Ana Souza");
```
### 🪪 Criar uma UnionCard (Carteira Sindical)
```
var carteirinha = new UnionCard("12345");
```
### 🕒 Criar um Intervalo de Tempo (DateRange)
```
var horario = new DateRange(
    DateTime.Today.AddHours(9),
    DateTime.Today.AddHours(12)
);
```
### 🎬 Criar uma Sessão de Gravação (Session)
```
var sessao = new Session(
    Guid.NewGuid(),
    salaA,
    horario,
    new[] { musico1, musico2 }
);
```
### Adicionar participantes depois
```
sessao.AddParticipant(musico3);
```
### 🗓 Criar a Agenda (Schedule)
```
var agenda = new Schedule();
agenda.AddSession(sessao);
```


