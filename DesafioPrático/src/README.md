Este projeto demonstra o uso de C# 13 / .NET 9 com Domain Model e Guard 
Clauses para representar um sistema de gerenciamento de eventos.
O objetivo é aplicar boas práticas de design de domínio, validações 
robustas e cobertura de testes com xUnit.

Estrutura das pastas
ManageEvent/
 ├── src/
 │   ├── ManageDomain/
 │   └── ManageConsole/
 ├── testes/
 │   └── Testes/
 ├── EventManagement.sln
 └── README.md

Entidades Principais

Speaker: representa o palestrante, validando nome e e-mail.
Venue: representa o local do evento, com capacidade e nome obrigatórios.
Event: vincula palestrante e local, validando datas e capacidade.

Cada entidade usa Guard Clauses para manter invariantes de domínio.