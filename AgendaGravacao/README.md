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
