# BadgeSystem (C#) - Implementação TDD

Projeto de exemplo que implementa a concessão de badges atreladas à conclusão de missões em um contexto acadêmico/gamificado.

### Objetivos
- Transformar regras de domínio em testes.
- Garantir invariantes e associação correta entre entidades.
- Tratar erros de forma explícita.
- Ser simples, testável e pronto para ser executado com `dotnet test`.

---

## Estrutura (exemplificativa)

- src/
  - BadgeSystem.Core/
    - Entities/
      - User.cs
      - Mission.cs
      - Badge.cs
      - MissionCompletion.cs
    - Exceptions/
      - DomainException.cs
    - Repositories/
      - IUserRepository.cs
      - IMissionRepository.cs
      - IBadgeRepository.cs
      - ICompletionRepository.cs
      - InMemory* (implementações usadas nos testes)
    - Services/
      - BadgeService.cs
  - BadgeSystem.Tests/
    - BadgeServiceTests.cs

---

## Como usar (localmente)
1. `dotnet new sln -n BadgeSystem`
2. `dotnet new classlib -n BadgeSystem.Core`
3. `dotnet new xunit -n BadgeSystem.Tests`
4. `dotnet sln add BadgeSystem.Core/BadgeSystem.Core.csproj BadgeSystem.Tests/BadgeSystem.Tests.csproj`
5. Na pasta dos testes, adicionar referência ao projeto: `dotnet add BadgeSystem.Tests/BadgeSystem.Tests.csproj reference ../BadgeSystem.Core/BadgeSystem.Core.csproj`
6. Colar os arquivos abaixo nos projetos correspondentes.
7. `dotnet test` para executar os testes.

---

## Testes (TDD) - BadgeSystem.Tests/BadgeServiceTests.cs
```csharp
using System;
using System.Linq;
using Xunit;
using BadgeSystem.Core.Entities;
using BadgeSystem.Core.Repositories;
using BadgeSystem.Core.Services;
using BadgeSystem.Core.Exceptions;
using BadgeSystem.Core.Repositories.InMemory;

namespace BadgeSystem.Tests
{
    public class BadgeServiceTests
    {
        [Fact]
        public void CompletingSingleMissionThatDirectlyMapsToBadge_ShouldAwardBadge()
        {
            var users = new InMemoryUserRepository();
            var missions = new InMemoryMissionRepository();
            var badges = new InMemoryBadgeRepository();
            var completions = new InMemoryCompletionRepository();

            var user = new User(Guid.NewGuid(), "Aluno 1");
            users.Add(user);

            var mission = new Mission(Guid.NewGuid(), "Missão A");
            missions.Add(mission);

            var badge = new Badge(Guid.NewGuid(), "Badge A", requiredMissionIds: new [] { mission.Id });
            badges.Add(badge);

            var service = new BadgeService(users, missions, badges, completions);

            service.MarkMissionCompleted(user.Id, mission.Id);

            var awarded = badges.GetAwardedBadgesForUser(user.Id);
            Assert.Contains(awarded, b => b.Id == badge.Id);
        }

        [Fact]
        public void CompletingSameMissionTwice_ShouldNotDuplicateBadgeOrCompletion()
        {
            var users = new InMemoryUserRepository();
            var missions = new InMemoryMissionRepository();
            var badges = new InMemoryBadgeRepository();
            var completions = new InMemoryCompletionRepository();

            var user = new User(Guid.NewGuid(), "Aluno 2");
            users.Add(user);

            var mission = new Mission(Guid.NewGuid(), "Missão B");
            missions.Add(mission);

            var badge = new Badge(Guid.NewGuid(), "Badge B", requiredMissionIds: new [] { mission.Id });
            badges.Add(badge);

            var service = new BadgeService(users, missions, badges, completions);

            service.MarkMissionCompleted(user.Id, mission.Id);
            // segunda chamada deve ser idempotente e não lançar erro, nem duplicar
            service.MarkMissionCompleted(user.Id, mission.Id);

            var allCompletions = completions.GetCompletionsForUser(user.Id).ToList();
            Assert.Single(allCompletions);

            var awarded = badges.GetAwardedBadgesForUser(user.Id);
            Assert.Single(awarded);
        }

        [Fact]
        public void BadgeWithMultipleRequiredMissions_ShouldBeAwardedOnlyAfterAllCompleted()
        {
            var users = new InMemoryUserRepository();
            var missions = new InMemoryMissionRepository();
            var badges = new InMemoryBadgeRepository();
            var completions = new InMemoryCompletionRepository();

            var user = new User(Guid.NewGuid(), "Aluno 3");
            users.Add(user);

            var m1 = new Mission(Guid.NewGuid(), "M1");
            var m2 = new Mission(Guid.NewGuid(), "M2");
            missions.Add(m1); missions.Add(m2);

            var badge = new Badge(Guid.NewGuid(), "Combo Badge", requiredMissionIds: new [] { m1.Id, m2.Id });
            badges.Add(badge);

            var service = new BadgeService(users, missions, badges, completions);

            service.MarkMissionCompleted(user.Id, m1.Id);
            Assert.Empty(badges.GetAwardedBadgesForUser(user.Id));

            service.MarkMissionCompleted(user.Id, m2.Id);
            var awarded = badges.GetAwardedBadgesForUser(user.Id);
            Assert.Contains(awarded, b => b.Id == badge.Id);
        }

        [Fact]
        public void CompletingUnknownMission_ShouldThrow()
        {
            var users = new InMemoryUserRepository();
            var missions = new InMemoryMissionRepository();
            var badges = new InMemoryBadgeRepository();
            var completions = new InMemoryCompletionRepository();

            var user = new User(Guid.NewGuid(), "Aluno 4");
            users.Add(user);

            var service = new BadgeService(users, missions, badges, completions);

            Assert.Throws<DomainException>(() => service.MarkMissionCompleted(user.Id, Guid.NewGuid()));
        }

        [Fact]
        public void NonexistentUser_ShouldThrow()
        {
            var users = new InMemoryUserRepository();
            var missions = new InMemoryMissionRepository();
            var badges = new InMemoryBadgeRepository();
            var completions = new InMemoryCompletionRepository();

            var mission = new Mission(Guid.NewGuid(), "Missão X");
            missions.Add(mission);

            var service = new BadgeService(users, missions, badges, completions);

            Assert.Throws<DomainException>(() => service.MarkMissionCompleted(Guid.NewGuid(), mission.Id));
        }
    }
}
```

---

## Implementação (BadgeSystem.Core)

### Exceptions/DomainException.cs
```csharp
using System;

namespace BadgeSystem.Core.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
```

### Entities/User.cs
```csharp
using System;

namespace BadgeSystem.Core.Entities
{
    public class User
    {
        public Guid Id { get; }
        public string Name { get; }

        public User(Guid id, string name)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id inválido", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome inválido", nameof(name));

            Id = id;
            Name = name.Trim();
        }
    }
}
```

### Entities/Mission.cs
```csharp
using System;

namespace BadgeSystem.Core.Entities
{
    public class Mission
    {
        public Guid Id { get; }
        public string Title { get; }

        public Mission(Guid id, string title)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id inválido", nameof(id));
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Título inválido", nameof(title));
            Id = id;
            Title = title.Trim();
        }
    }
}
```

### Entities/Badge.cs
```csharp
using System;
using System.Collections.Generic;
using System.Linq;

namespace BadgeSystem.Core.Entities
{
    public class Badge
    {
        public Guid Id { get; }
        public string Name { get; }
        // Lista de missões necessárias para ganhar o badge (pode ser vazia para badges atribuídos manualmente)
        public IReadOnlyCollection<Guid> RequiredMissionIds { get; }

        public Badge(Guid id, string name, IEnumerable<Guid> requiredMissionIds = null)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id inválido", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Nome inválido", nameof(name));

            Id = id;
            Name = name.Trim();
            RequiredMissionIds = (requiredMissionIds ?? Enumerable.Empty<Guid>()).ToArray();
        }

        public bool IsSatisfiedBy(IEnumerable<Guid> completedMissionIds)
        {
            if (RequiredMissionIds == null || RequiredMissionIds.Count == 0) return false; // regras: badge automático precisa de requisitos
            var set = new HashSet<Guid>(completedMissionIds ?? Enumerable.Empty<Guid>());
            return RequiredMissionIds.All(r => set.Contains(r));
        }
    }
}
```

### Entities/MissionCompletion.cs
```csharp
using System;

namespace BadgeSystem.Core.Entities
{
    public class MissionCompletion
    {
        public Guid UserId { get; }
        public Guid MissionId { get; }
        public DateTime CompletedAt { get; }

        public MissionCompletion(Guid userId, Guid missionId, DateTime? completedAt = null)
        {
            if (userId == Guid.Empty) throw new ArgumentException("UserId inválido", nameof(userId));
            if (missionId == Guid.Empty) throw new ArgumentException("MissionId inválido", nameof(missionId));

            UserId = userId;
            MissionId = missionId;
            CompletedAt = completedAt ?? DateTime.UtcNow;
        }
    }
}
```

### Repositories (interfaces)
```csharp
using System;
using System.Collections.Generic;
using BadgeSystem.Core.Entities;

namespace BadgeSystem.Core.Repositories
{
    public interface IUserRepository { User Get(Guid id); void Add(User user); }
    public interface IMissionRepository { Mission Get(Guid id); void Add(Mission mission); IEnumerable<Mission> GetAll(); }
    public interface IBadgeRepository { Badge Get(Guid id); void Add(Badge badge); IEnumerable<Badge> GetAll(); void AwardBadgeToUser(Guid badgeId, Guid userId); IEnumerable<Badge> GetAwardedBadgesForUser(Guid userId); }
    public interface ICompletionRepository { void Add(MissionCompletion completion); bool Exists(Guid userId, Guid missionId); IEnumerable<MissionCompletion> GetCompletionsForUser(Guid userId); }
}
```

### Repositories/InMemory implementations (para testes)
```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using BadgeSystem.Core.Entities;
using BadgeSystem.Core.Repositories;

namespace BadgeSystem.Core.Repositories.InMemory
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly Dictionary<Guid, User> _store = new();
        public void Add(User user) => _store[user.Id] = user;
        public User Get(Guid id) => _store.TryGetValue(id, out var u) ? u : null;
    }

    public class InMemoryMissionRepository : IMissionRepository
    {
        private readonly Dictionary<Guid, Mission> _store = new();
        public void Add(Mission mission) => _store[mission.Id] = mission;
        public Mission Get(Guid id) => _store.TryGetValue(id, out var m) ? m : null;
        public IEnumerable<Mission> GetAll() => _store.Values.ToList();
    }

    public class InMemoryBadgeRepository : IBadgeRepository
    {
        private readonly Dictionary<Guid, Badge> _badges = new();
        private readonly Dictionary<Guid, HashSet<Guid>> _awardsByUser = new();

        public void Add(Badge badge) => _badges[badge.Id] = badge;
        public Badge Get(Guid id) => _badges.TryGetValue(id, out var b) ? b : null;
        public IEnumerable<Badge> GetAll() => _badges.Values.ToList();

        public void AwardBadgeToUser(Guid badgeId, Guid userId)
        {
            if (!_badges.ContainsKey(badgeId)) throw new ArgumentException("Badge desconhecido", nameof(badgeId));
            var set = _awardsByUser.GetValueOrDefault(userId) ?? new HashSet<Guid>();
            set.Add(badgeId);
            _awardsByUser[userId] = set;
        }

        public IEnumerable<Badge> GetAwardedBadgesForUser(Guid userId)
        {
            if (!_awardsByUser.TryGetValue(userId, out var set)) return Enumerable.Empty<Badge>();
            return set.Select(id => _badges[id]);
        }
    }

    public class InMemoryCompletionRepository : ICompletionRepository
    {
        private readonly HashSet<(Guid userId, Guid missionId)> _store = new();
        private readonly List<MissionCompletion> _completions = new();

        public void Add(MissionCompletion completion)
        {
            var key = (completion.UserId, completion.MissionId);
            if (_store.Contains(key)) return; // idempotente: não adiciona duplicado
            _store.Add(key);
            _completions.Add(completion);
        }

        public bool Exists(Guid userId, Guid missionId) => _store.Contains((userId, missionId));

        public IEnumerable<MissionCompletion> GetCompletionsForUser(Guid userId) => _completions.Where(c => c.UserId == userId).ToList();
    }
}
```

### Services/BadgeService.cs
```csharp
using System;
using System.Linq;
using BadgeSystem.Core.Repositories;
using BadgeSystem.Core.Entities;
using BadgeSystem.Core.Exceptions;

namespace BadgeSystem.Core.Services
{
    public class BadgeService
    {
        private readonly IUserRepository _users;
        private readonly IMissionRepository _missions;
        private readonly IBadgeRepository _badges;
        private readonly ICompletionRepository _completions;

        public BadgeService(IUserRepository users, IMissionRepository missions, IBadgeRepository badges, ICompletionRepository completions)
        {
            _users = users ?? throw new ArgumentNullException(nameof(users));
            _missions = missions ?? throw new ArgumentNullException(nameof(missions));
            _badges = badges ?? throw new ArgumentNullException(nameof(badges));
            _completions = completions ?? throw new ArgumentNullException(nameof(completions));
        }

        // Marca missão como completada e avalia concessão de badges
        public void MarkMissionCompleted(Guid userId, Guid missionId)
        {
            var user = _users.Get(userId) ?? throw new DomainException("Usuário não encontrado");
            var mission = _missions.Get(missionId) ?? throw new DomainException("Missão não encontrada");

            // invariantes: se já completou, operação é idempotente (não duplica)
            if (!_completions.Exists(userId, missionId))
            {
                var completion = new MissionCompletion(userId, missionId);
                _completions.Add(completion);
            }

            // avaliar badges automáticos: listar badges e verificar se requisitos satisfeitos
            var allBadges = _badges.GetAll();
            var completedIds = _completions.GetCompletionsForUser(userId).Select(c => c.MissionId).ToList();

            foreach (var badge in allBadges)
            {
                // regra: badges automáticos têm RequiredMissionIds não vazio
                if (!badge.IsSatisfiedBy(completedIds)) continue;

                // se já concedido, o repositório é responsável por garantir idempotência
                _badges.AwardBadgeToUser(badge.Id, userId);
            }
        }
    }
}
```

---

## Notas sobre decisões de projeto e invariantes
- **Idempotência**: completar a mesma missão várias vezes não cria múltiplos registros nem múltiplas concessões de badges.
- **Badges com requisitos**: um `Badge` contém uma coleção `RequiredMissionIds`. O `BadgeService` verifica se o usuário possui todas as missões necessárias.
- **Validações**: construtores das entidades validam argumentos (IDs e strings não vazias).
- **Tratamento de erros**: `DomainException` é lançada quando usuário ou missão não existem; repositórios podem lançar `ArgumentException` em casos de uso incorreto.
- **Separação de responsabilidades**: repositórios cuidam de armazenamento; `BadgeService` cuida da orquestração.

---



