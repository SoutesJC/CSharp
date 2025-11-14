#nullable enable
using System;
using StudioSchedule.Entities;
using StudioSchedule.ValueObjects;

namespace StudioSchedule
{
    internal class Program
    {
        static void Main()
        {
            Console.WriteLine("=== Agenda de Estúdio ===\n");

            // 1️⃣ Criar sala
            var salaA = new Room(Guid.NewGuid(),"Sala A");

            // 2️⃣ Criar músicos
            var musico1 = new Musician(Guid.NewGuid(), "Carlos Silva", new UnionCard("12345"));
            var musico2 = new Musician(Guid.NewGuid(), "Ana Souza");
            var musico3 = new Musician(Guid.NewGuid(), "Pedro Lima");

            // 3️⃣ Criar intervalos de tempo (DateRange)
            var hoje = DateTime.Now.Date;
            var horarioManha = new DateRange(hoje.AddHours(9), hoje.AddHours(12));
            var horarioTarde = new DateRange(hoje.AddHours(13), hoje.AddHours(16));

            // 4️⃣ Criar sessões de gravação
            var sessao1 = new Session(Guid.NewGuid(), salaA, horarioManha, new[] { musico1, musico2 });

            var sessao2 = new Session(Guid.NewGuid(), salaA, horarioTarde, new[] { musico3 });

            // 5️⃣ Criar agenda geral (Schedule)
            var agenda = new Schedule();
            agenda.AddSession(sessao1);
            agenda.AddSession(sessao2);

            // 6️⃣ Listar sessões
            Console.WriteLine("Sessões agendadas:");
            foreach (var s in agenda.Sessions)
            {
                Console.WriteLine($"- {s.Room.Name} | {s.When.Start:HH:mm} - {s.When.End:HH:mm}");
                Console.WriteLine($"  Músicos: {string.Join(", ", s.Participants.Select(m => m.FullName))}");
            }

            // 7️⃣ Teste de regra de negócio (colisão)
            /*try
            {
                Console.WriteLine("\nTentando agendar sessão conflitante...");
                var sessaoConflitante = new Session(Guid.NewGuid(), salaA, new DateRange(hoje.AddHours(11), hoje.AddHours(14)), new[] { musico2 });
                agenda.AddSession(sessaoConflitante); // Deve lançar exceção
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro: {ex.Message}");
            }*/

            Console.WriteLine("\nPrograma finalizado com sucesso!");
        }
    }
}
